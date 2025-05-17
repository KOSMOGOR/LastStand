using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public Transform playOpportunitiesParent;
    List<GameObject> playOpportunities;
    int poActiveCount;

    void Awake() {
        playOpportunities = playOpportunitiesParent.GetComponentsInChildren<Transform>().Where(t => t != playOpportunitiesParent).Select(t => t.gameObject).ToList();
        playOpportunities.ForEach(po => po.SetActive(false));
        poActiveCount = 0;
    }

    void Update() {
        SetPlayOpportunities();
    }

    void SetPlayOpportunities() {
        int playerPO = Player.I.playOpportunities;
        if (poActiveCount != playerPO) {
            poActiveCount = playerPO;
            for (int i = 0; i < playOpportunities.Count; ++i) playOpportunities[i].SetActive(i < poActiveCount);
        }
    }
}
