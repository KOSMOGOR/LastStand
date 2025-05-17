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

    void Awake() {
        playOpportunities = playOpportunitiesParent.GetComponentsInChildren<Transform>().Where(t => t != playOpportunitiesParent).Select(t => t.gameObject).ToList();
        playOpportunities.ForEach(po => po.SetActive(false));
        poActiveCount = 0;
    }

    void Update() {
        SetPlayOpportunities();
        SetPlayerHp();
        SetButtonsAvailable();
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
}
