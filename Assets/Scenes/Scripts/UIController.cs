using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    InfoPanelBehavior infoPanelBehavior;
    GameObject infoPanel;
    // Start is called before the first frame update
    void Start()
    {
        infoPanelBehavior = FindObjectOfType<Canvas>().GetComponentInChildren<InfoPanelBehavior>();
        infoPanel = infoPanelBehavior.gameObject;
        updateSelection(null);
    }

    public void updateSelection(Unit unit) {
        if(unit != null) {
            infoPanel.SetActive(true);
            infoPanelBehavior.updateSelection(unit);
        } else {
            infoPanel.SetActive(false);
        }
    }
}
