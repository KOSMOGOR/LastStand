using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public Transform playOpportunitiesParent;
    List<GameObject> playOpportunities;
    int poActiveCount;
    public TMP_Text playerHp;

    void Awake() {
        playOpportunities = playOpportunitiesParent.GetComponentsInChildren<Transform>().Where(t => t != playOpportunitiesParent).Select(t => t.gameObject).ToList();
        playOpportunities.ForEach(po => po.SetActive(false));
        poActiveCount = 0;
    }

    void Update() {
        SetPlayOpportunities();
        SetPlayerHp();
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
}
