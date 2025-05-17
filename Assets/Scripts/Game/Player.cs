using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerMaxHp;
    public Transform parentHandTransform;
    public Transform topDiscardTransform;
    public Card cardPrefab;

    [Header("Internal")]
    public int playerHp;
    public List<Transform> handTransforms;
    public List<Card> hand = new(); // card will be drawn from last index
    public List<Card> deck = new();
    public List<Card> discardPile = new(); // top card is located at last index
    public List<bool> handTransformsOccupied;
    public Card topDiscardCard;
    public Card chosenCard;
    public int maxHandSize;
    public int digitalCurrency = 0;
    public int playOpportunities = 0;
    public List<Card> cardsPlayedThisTurn = new();
    public Dictionary<string, CardData> allCardDatas;

    public static Player I;

    void Awake() {
        if (I != null) Destroy(gameObject);
        I = this;
        allCardDatas = Resources.LoadAll<CardData>("CardDatas").ToList().ToDictionary(c => c.cardName, elementSelector: c => c);
    }

    public void SetDeckToBase() {
        List<string> cardNames = new() {"Pistol", "Shotgun", "Basic Defence", "Minefield", "Additional Ammo", "Construction", "Overcharge", "Tranquility", "Torch"};
        deck = cardNames.Select(cn => InitializeCard(cn)).ToList();
        ShuffleDeck();
        handTransforms = parentHandTransform.GetComponentsInChildren<Transform>().Where(t => t != parentHandTransform).ToList();
        maxHandSize = handTransforms.Count;
        handTransformsOccupied = Enumerable.Repeat(false, maxHandSize).ToList();
    }

    public void ChooseCard(Card card) {
        if (GameManager.I.currentState != GameState.PlayerTurn) return;
        if (chosenCard != null) {
            chosenCard.SetChosenStatus(false);
            chosenCard = null;
            GridManager.I.SetCurrentGridSelectionType(GridSelectionType.None);
        }
        if (card && card.inHand) {
            chosenCard = card;
            chosenCard.SetChosenStatus(true);
            GridManager.I.SetCurrentGridSelectionType(chosenCard.cardData.gridSelectionType);
        }
    }

    public void PlayChosenCard() {
        if (GameManager.I.currentState != GameState.PlayerTurn || chosenCard == null) return;
        if (GridManager.I.currentSelectedTile == null && chosenCard.cardData.gridSelectionType != GridSelectionType.None) return;
        if (playOpportunities == 0) return;
        playOpportunities -= 1;
        chosenCard.Play();
        MoveCardFromHandToDiscard(chosenCard);
        ChooseCard(null);
        cardsPlayedThisTurn.Add(chosenCard);
        if (playOpportunities == 0) EndPlayerTurn();
    }

    public Card InitializeCard(CardData cardData) {
        Card card = Instantiate(cardPrefab);
        card.SetCardData(cardData);
        return card;
    }

    public Card InitializeCard(string cardName) {
        if (!allCardDatas.ContainsKey(cardName)) return null;
        CardData cardData = allCardDatas[cardName];
        return InitializeCard(cardData);
    }

    public void AddCardToDeck(Card card) {
        deck.Add(card);
        card.transform.parent = transform;
    }

    public void MoveCardFromHandToDiscard(Card card) {
        card.SetInHand(false);
        card.SetShown(true);
        int handInd = hand.IndexOf(card);
        handTransformsOccupied[handInd] = false;
        hand.Remove(card);
        if (topDiscardCard != null) {
            topDiscardCard.SetShown(false);
            topDiscardCard = null;
        }
        topDiscardCard = card;
        card.transform.SetPositionAndRotation(topDiscardTransform.position, topDiscardTransform.rotation);
    }

    public void ShuffleDiscardToDeck() {
        if (topDiscardCard != null) {
            topDiscardCard.SetShown(false);
            topDiscardCard = null;
        }
        deck.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();
    }

    public void ShuffleDeck() {
        deck = deck.OrderBy(_ => Random.value).ToList();
    }

    public bool DrawCardFromDeck() {
        if (deck.Count == 0) return false;
        if (hand.Count >= maxHandSize) return false;
        int cardInd = deck.Count - 1;
        Card card = deck[cardInd];
        deck.RemoveAt(cardInd);
        AddCardToHand(card);
        return true;
    }

    void AddCardToHand(Card card) {
        if (hand.Count >= maxHandSize) return;
        card.SetInHand(true);
        hand.Add(card);
        int handTransformInd = handTransformsOccupied.FindIndex(hto => !hto);
        handTransformsOccupied[handTransformInd] = true;
        Transform handTransform = handTransforms[handTransformInd];
        card.transform.SetPositionAndRotation(handTransform.position, handTransform.rotation);
    }

    public void DiscardHand() {
        foreach (Card card in hand) MoveCardFromHandToDiscard(card);
    }

    public void TakeDamage(int dmg) {
        playerHp -= dmg;
        Messenger<int>.Broadcast(EventMessages.ON_PLAYER_TAKE_DAMAGE, dmg);
        if (playerHp <= 0) GameManager.I.ChangeState(GameState.Defeated);
    }

    public void OnPlayerTurnStart() {
        for (int i = 0; i < 3; ++i) DrawCardFromDeck();
        playOpportunities = 2;
    }

    public void EndPlayerTurn(bool discardHand = true) {
        cardsPlayedThisTurn = new();
        if (discardHand) DiscardHand();
        playOpportunities = 0;
        Messenger.Broadcast(EventMessages.ON_PLAYER_END_TURN);
        GameManager.I.ChangeState(GameState.ZombieTurn);
    }

    public void MakeRest() {
        ShuffleDiscardToDeck();
        Messenger.Broadcast(EventMessages.ON_PLAYER_MAKE_REST);
        EndPlayerTurn(false);
    }
}
