using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject shopBase;
    public List<Transform> cardTransforms;
    public ShopCard shopCardPrefab;
    public Button exitButton;
    public Button buyButton;
    public GameObject shopCardSelectorPrefab;
    public TMP_Text digitalCurrencyText;
    public Button rerollButton;
    public TMP_Text rerollPriceText;
    int rerollPrice;
    int rerollPriceRiced = 0;

    [Header("Internal")]
    public int shopSize;
    public List<ShopCard> shopCards;
    public ShopCard chosenShopCard;

    public static ShopManager I;
    GameObject shopCardSelector;

    void Awake() {
        if (I != null) Destroy(gameObject);
        else I = this;
        shopSize = cardTransforms.Count;
        shopBase.SetActive(false);
        shopCardSelector = Instantiate(shopCardSelectorPrefab);
        ChooseShopCard(null);
    }

    void Update() {
        SetExitButtonActive();
        SetBuyButtonActive();
        SetDCText();
        SetRerollPrice();
    }

    public void InitializeShop() {
        shopBase.SetActive(true);
        ResetReroll();
        SetShopCards();
    }

    void EmptyShopCards() {
        ChooseShopCard(null);
        shopCards.ForEach(sc => { if (sc != null) Destroy(sc.gameObject); });
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
        SetShopCardSelectorCard(shopCard);
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
        Destroy(chosenShopCard.gameObject);
        ChooseShopCard(null);
        ResetReroll();
    }

    bool CanExitShop() {
        return shopCards.Count == 0 || shopCards.All(sc => sc.cardCost > Player.I.digitalCurrency);
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

    void SetShopCardSelectorCard(ShopCard shopCard) {
        if (shopCard == null) shopCardSelector.SetActive(false);
        else {
            shopCardSelector.SetActive(true);
            shopCardSelector.transform.SetPositionAndRotation(shopCard.transform.position, shopCard.transform.rotation);
        }
    }

    void SetDCText() {
        digitalCurrencyText.text = $"x{Player.I.digitalCurrency}";
    }

    void ResetReroll() {
        rerollPrice = (int)(GameManager.I.currentLevel * 0.1) * 25 + 50;
        rerollPriceRiced = 0;
    }

    bool CanReroll() {
        return Player.I.digitalCurrency >= rerollPrice && rerollPriceRiced < 2;
    }

    public void Reroll() {
        if (!CanReroll()) return;
        Player.I.digitalCurrency -= rerollPrice;
        SetShopCards();
        rerollPrice *= 2;
        rerollPriceRiced += 1;
    }

    void SetRerollPrice() {
        rerollPriceText.text = $"x{rerollPrice}";
        rerollButton.gameObject.SetActive(CanReroll());
        rerollPriceText.gameObject.SetActive(rerollPriceRiced < 2);
    }
}
