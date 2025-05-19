using TMPro;
using UnityEngine;

public class ShopCard : MonoBehaviour
{
    public CardData cardData;
    public int cardCost;
    public TMP_Text cardCostText;

    SpriteRenderer sprite;

    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetCardData(CardData cardData) {
        this.cardData = cardData;
        cardCost = Random.Range(25, 51) * cardData.aggressionCost;
        cardCostText.text = cardCost.ToString();
        sprite.sprite = cardData.cardSprite;
    }

    void OnMouseDown() {
        ShopManager.I.ChooseShopCard(this);
    }
}
