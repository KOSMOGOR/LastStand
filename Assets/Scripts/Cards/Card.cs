using System.Linq;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CardData cardData;
    public bool chosen = false;
    public int handInd = -1; // -1 - not in hand
    public bool shown = false;
    public Animator cardRevealAnimator;

    SpriteRenderer spriteRenderer;
    BoxCollider2D col;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        SetHandInd(-1);
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
        Messenger<Card>.Broadcast(EventMessages.ON_CARD_PLAYED, this);
    }

    public void SetChosenStatus(bool status) {
        chosen = status;
    }

    public void SetHandInd(int handInd) {
        this.handInd = handInd;
        SetShown(handInd != -1);
    }

    public void SetShown(bool shown) {
        this.shown = shown;
        cardRevealAnimator.SetBool("revealed", shown);
        spriteRenderer.enabled = shown;
        col.enabled = shown;
        cardRevealAnimator.GetComponent<SpriteRenderer>().enabled = shown;
    }

    public bool IsInHand() { return handInd != -1; }

    public bool CanPlay() {
        if (cardData.playConditions.Count == 0) return true;
        return cardData.playConditions.All(pc => pc.CanPlay());
    }
}
