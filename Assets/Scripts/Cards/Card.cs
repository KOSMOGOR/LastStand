using UnityEngine;

public class Card : MonoBehaviour
{
    public CardData cardData;
    public bool chosen = false;
    public bool inHand = false;
    public bool shown = false;

    SpriteRenderer spriteRenderer;
    BoxCollider2D col;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        SetInHand(false);
    }

    void OnMouseDown() {
        Player.I.ChooseCard(this);
    }

    public void SetCardData(CardData cardData) {
        this.cardData = cardData;
        spriteRenderer.sprite = cardData.cardSprite;
    }

    public void Play() {
        foreach (CardEffect effect in cardData.effects) {
            effect.Activate();
        }
    }

    public void SetChosenStatus(bool status) {
        chosen = status;
    }

    public void SetInHand(bool inHand) {
        this.inHand = inHand;
        SetShown(inHand);
    }

    public void SetShown(bool shown) {
        this.shown = shown;
        spriteRenderer.enabled = shown;
        col.enabled = shown;
    }
}
