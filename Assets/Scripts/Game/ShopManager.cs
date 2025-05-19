using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject shopBase;
    public List<Transform> cardTransforms;
    public ShopCard shopCardPrefab;
    public Button exitButton;
    public Button buyButton;

    [Header("Internal")]
    public int shopSize;
    public List<ShopCard> shopCards;
    public ShopCard chosenShopCard;

    public static ShopManager I;

    void Awake() {
        if (I != null) Destroy(gameObject);
        else I = this;
        shopSize = cardTransforms.Count;
        shopBase.SetActive(false);
    }

    void Update() {
        SetExitButtonActive();
        SetBuyButtonActive();
    }

    public void InitializeShop() {
        shopBase.SetActive(true);
        SetShopCards();
    }

    void EmptyShopCards() {
        shopCards.ForEach(sc => Destroy(sc.gameObject));
        shopCards.Clear();
    }

    void SetShopCards() {
        EmptyShopCards();
        for (int i = 0; i < shopSize; ++i) {
            List<CardData> cardDatas = Player.I.allCardDatas.Values.ToList();
            CardData cardData = cardDatas[Random.Range(0, cardDatas.Count)];
            ShopCard shopCard = InitializeShopCard(cardData);
            shopCard.transform.SetPositionAndRotation(cardTransforms[i].position, cardTransforms[i].rotation);
            shopCards.Add(shopCard);
        }
    }

    ShopCard InitializeShopCard(CardData cardData) {
        ShopCard shopCard = Instantiate(shopCardPrefab);
        shopCard.SetCardData(cardData);
        return shopCard;
    }

    public void ChooseShopCard(ShopCard shopCard) {
        if (shopCard == null) { chosenShopCard = null; return; }
        if (GameManager.I.currentState != GameState.Shopping || !shopCards.Contains(shopCard)) return;
        chosenShopCard = shopCard;
    }

    bool CanBuyChosenCard() {
        if (chosenShopCard == null) return false;
        if (Player.I.digitalCurrency < chosenShopCard.cardCost) return false;
        return true;
    }

    public void BuyChosenCard() {
        if (!CanBuyChosenCard()) return;
        Player.I.digitalCurrency -= chosenShopCard.cardCost;
        Card card = Player.I.InitializeCard(chosenShopCard.cardData);
        Player.I.AddCardToDeck(card);
        Destroy(chosenShopCard);
        chosenShopCard = null;
    }

    bool CanExitShop() {
        return shopCards.Count == 0 || shopCards.Any(sc => sc.cardCost <= Player.I.digitalCurrency);
    }

    public void ExitShop() {
        if (!CanExitShop()) return;
        EmptyShopCards();
        shopBase.SetActive(false);
        GameManager.I.ChangeState(GameState.PrepareLevel);
    }

    void SetExitButtonActive() {
        exitButton.gameObject.SetActive(CanExitShop());
    }

    void SetBuyButtonActive() {
        buyButton.gameObject.SetActive(CanBuyChosenCard());
    }
}
