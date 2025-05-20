using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Transform playOpportunitiesParent;
    List<GameObject> playOpportunities;
    int poActiveCount;
    public TMP_Text playerHp;
    public Button playCardButton;
    public Button cancelButton;
    public Button restButton;
    public Button endTurnButton;
    public GameObject morningEveningOverlay;
    public GameObject nightOverlay;
    public GameObject newTurnTextPrefab;
    public Sprite playerTurnSprite;
    public Sprite zombieTurnSprite;
    public TMP_Text descriptionText;
    public TMP_Text digitalCurrencyText;
    public Animator dayTimeCycleAnimator;
    DayTime lastSeenDayTime = DayTime.Day;

    GameObject newTurnText;

    public static MenuManager I;

    void Awake() {
        if (I != null) Destroy(gameObject);
        else I = this;
        playOpportunities = playOpportunitiesParent.GetComponentsInChildren<Transform>().Where(t => t != playOpportunitiesParent).Select(t => t.gameObject).ToList();
        playOpportunities.ForEach(po => po.SetActive(false));
        poActiveCount = 0;
        morningEveningOverlay.SetActive(false);
        nightOverlay.SetActive(false);
        newTurnText = Instantiate(newTurnTextPrefab);
    }

    void Update() {
        SetPlayOpportunities();
        SetPlayerHp();
        SetButtonsAvailable();
        SetDayTimeOverlays();
        SetCardDescription();
        SetDCText();
        UpdateDayTime();
    }

    void SetPlayOpportunities() {
        int playerPO = Player.I.playOpportunities;
        if (poActiveCount != playerPO) {
            poActiveCount = playerPO;
            for (int i = 0; i < playOpportunities.Count; ++i) playOpportunities[i].SetActive(i < poActiveCount);
        }
    }

    void SetPlayerHp() {
        playerHp.text = $"x{Player.I.playerHp}";
    }

    void SetButtonsAvailable() {
        playCardButton.gameObject.SetActive(Player.I.CanPlayChosenCard());
        cancelButton.gameObject.SetActive(Player.I.chosenCard != null);
        restButton.gameObject.SetActive(Player.I.canRest);
        endTurnButton.gameObject.SetActive(GameManager.I.currentState == GameState.PlayerTurn);
    }

    void SetDayTimeOverlays() {
        DayTime currentDayTime = GameManager.I.GetCurrentDayTime();
        if (currentDayTime == DayTime.Morning || currentDayTime == DayTime.Evening) {
            morningEveningOverlay.SetActive(true);
            nightOverlay.SetActive(false);
        } else if (currentDayTime == DayTime.Night) {
            morningEveningOverlay.SetActive(false);
            nightOverlay.SetActive(true);
        } else {
            morningEveningOverlay.SetActive(false);
            nightOverlay.SetActive(false);
        }
    }

    public void ShowNewTurnText(GameState state) {
        if (state == GameState.PlayerTurn) {
            newTurnText.GetComponent<Animation>().Stop();
            newTurnText.GetComponent<SpriteRenderer>().sprite = playerTurnSprite;
            newTurnText.GetComponent<Animation>().Play();
        } else if (state == GameState.ZombieTurn) {
            newTurnText.GetComponent<Animation>().Stop();
            newTurnText.GetComponent<SpriteRenderer>().sprite = zombieTurnSprite;
            newTurnText.GetComponent<Animation>().Play();
        }
    }

    void SetCardDescription() {
        if (Player.I.chosenCard != null) descriptionText.text = Player.I.chosenCard.cardData.cardDescription;
        else descriptionText.text = "";
    }

    void SetDCText() {
        digitalCurrencyText.text = $"x{Player.I.digitalCurrency}";
    }

    void UpdateDayTime() {
        DayTime currentDayTime = GameManager.I.GetCurrentDayTime();
        if (lastSeenDayTime != currentDayTime) {
            switch (currentDayTime) {
                case DayTime.Day: dayTimeCycleAnimator.SetTrigger("day"); break;
                case DayTime.Evening: dayTimeCycleAnimator.SetTrigger("evening"); break;
                case DayTime.Night: dayTimeCycleAnimator.SetTrigger("night"); break;
                case DayTime.Morning: dayTimeCycleAnimator.SetTrigger("morning"); break;
            }
            lastSeenDayTime = currentDayTime;
        }
    }
}
