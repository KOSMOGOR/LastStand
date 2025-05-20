using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public int playerMaxHp;
    public Transform parentHandTransform;
    public Transform topDiscardTransform;
    public Card cardPrefab;
    public CardDeck startDeck;
    public GameObject cardSelectorPrefab;

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
    public int digitalCurrency;
    public int playOpportunities = 0;
    public List<Card> cardsPlayedThisTurn = new();
    public bool canRest = false;
    public Dictionary<string, CardData> allCardDatas;

    public static Player I;
    Transform cardParentTransform;
    GameObject cardSelector;

    void Awake() {
        if (I != null) Destroy(gameObject);
        else I = this;
        allCardDatas = Resources.LoadAll<CardData>("CardDatas").ToList().ToDictionary(c => c.cardName, elementSelector: c => c);
        cardParentTransform = new GameObject("Cards").transform;
        cardSelector = Instantiate(cardSelectorPrefab);
        SetCardSelectorCard(null);
    }

    public void SetDeckToBase() {
        EmptyAllCards();
        deck = startDeck.cards.Select(cd => InitializeCard(cd)).ToList();
        ShuffleDeck();
        handTransforms = parentHandTransform.GetComponentsInChildren<Transform>().Where(t => t != parentHandTransform).ToList();
        maxHandSize = handTransforms.Count;
        handTransformsOccupied = Enumerable.Repeat(false, maxHandSize).ToList();
    }

    void EmptyAllCards() {
        hand.ForEach(c => Destroy(c.gameObject));
        hand.Clear();
        deck.ForEach(c => Destroy(c.gameObject));
        deck.Clear();
        discardPile.ForEach(c => Destroy(c.gameObject));
        discardPile.Clear();
    }

    public void ChooseCard(Card card) {
        GridManager.I.SetCurrentGridSelectionType(GridSelectionType.None);
        if (chosenCard != null) {
            chosenCard.SetChosenStatus(false);
            chosenCard = null;
        }
        if (GameManager.I.currentState != GameState.PlayerTurn) return;
        SetCardSelectorCard(card);
        AudioManager.I.PlaySound(SoundType.UIClick);
        if (card != null && card.IsInHand()) {
            chosenCard = card;
            chosenCard.SetChosenStatus(true);
            GridManager.I.SetCurrentGridSelectionType(chosenCard.cardData.gridSelectionType);
        }
    }

    public void ChooseNullCard() {
        ChooseCard(null);
    }

    void SetCardSelectorCard(Card card) {
        if (card == null) cardSelector.SetActive(false);
        else {
            cardSelector.SetActive(true);
            cardSelector.transform.SetPositionAndRotation(card.transform.position, card.transform.rotation);
        }
    }

    public bool CanPlayChosenCard() {
        if (GameManager.I.currentState != GameState.PlayerTurn || chosenCard == null) return false;
        if (GridManager.I.currentSelectedTile == null && chosenCard.cardData.gridSelectionType != GridSelectionType.None) return false;
        if (playOpportunities == 0) return false;
        return chosenCard.CanPlay();
    }

    public void PlayChosenCard() {
        if (!CanPlayChosenCard()) return;
        playOpportunities -= 1;
        canRest = false;
        Card card = chosenCard;
        cardsPlayedThisTurn.Add(card);
        card.Play();
        MoveCardFromHandToDiscard(card);
        ChooseCard(null);
    }

    public Card InitializeCard(CardData cardData) {
        Card card = Instantiate(cardPrefab, cardParentTransform);
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
        int handInd = card.handInd;
        card.SetHandInd(-1);
        card.SetShown(true);
        handTransformsOccupied[handInd] = false;
        hand.Remove(card);
        if (topDiscardCard != null) {
            topDiscardCard.SetShown(false);
            topDiscardCard = null;
        }
        topDiscardCard = card;
        card.transform.SetPositionAndRotation(topDiscardTransform.position, topDiscardTransform.rotation);
        discardPile.Add(card);
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
        int handTransformInd = handTransformsOccupied.FindIndex(hto => !hto);
        card.SetHandInd(handTransformInd);
        hand.Add(card);
        handTransformsOccupied[handTransformInd] = true;
        Transform handTransform = handTransforms[handTransformInd];
        card.transform.SetPositionAndRotation(handTransform.position, handTransform.rotation);
    }

    public void DiscardHand() {
        while (hand.Count() > 0) MoveCardFromHandToDiscard(hand.First());
    }

    public void TakeDamage(int dmg) {
        playerHp -= dmg;
        Messenger<int>.Broadcast(EventMessages.ON_PLAYER_TAKE_DAMAGE, dmg);
        if (playerHp <= 0) GameManager.I.ChangeState(GameState.Defeated);
    }

    public void TakeHeal(int heal) {
        playerHp = Math.Min(playerHp + heal, playerMaxHp);
    }

    public void OnPlayerTurnStart() {
        for (int i = 0; i < 3; ++i) DrawCardFromDeck();
        playOpportunities = 2;
        canRest = true;
    }

    public void EndPlayerTurn(bool discardHand = true) {
        canRest = false;
        ChooseCard(null);
        cardsPlayedThisTurn = new();
        if (discardHand) DiscardHand();
        playOpportunities = 0;
        Messenger.Broadcast(EventMessages.ON_PLAYER_END_TURN);
        ChangeStateToNew();
    }

    void ChangeStateToNew() {
        GameState newState = GameState.ZombieTurn;
        if (GameManager.I.zombiesAggressionPoints == 0 && GridManager.I.GetZombiesCount() == 0) newState = GameState.Shopping;
        if (newState == GameState.Shopping) {
            Messenger.Broadcast(EventMessages.ON_ZOMBIE_END_TURN);
            DiscardHand();
        }
        GameManager.I.ChangeState(newState);
    }

    public void MakeRest() {
        if (GameManager.I.currentState != GameState.PlayerTurn) return;
        ShuffleDiscardToDeck();
        Messenger.Broadcast(EventMessages.ON_PLAYER_MAKE_REST);
        EndPlayerTurn(false);
    }
}
