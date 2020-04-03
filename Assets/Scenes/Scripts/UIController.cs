using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    GameObject infoPanel;
    InfoPanelBehavior infoPanelBehavior;
    TurnController turnController;

    Button nextButton;

    // Start is called before the first frame update
    void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        infoPanelBehavior = canvas.GetComponentInChildren<InfoPanelBehavior>();
        infoPanel = infoPanelBehavior.gameObject;
        nextButton = findButtonByName("NextTurnButton", GameObject.FindObjectsOfType<Button>());
        turnController = FindObjectOfType<TurnController>();

        nextButton.onClick.AddListener(turnController.nextTurn);
        updateSelection(null); //disable info panel by default
    }

    public void updateSelection(Unit unit) {
        if(unit != null) {
            infoPanel.SetActive(true);
            infoPanelBehavior.updateSelection(unit);
        } else {
            infoPanel.SetActive(false);
        }
    }

    private Button findButtonByName(string btnName, Button[] btns) {
        foreach(Button b in btns) {
            if(b.name == btnName) {
                return b;
            }
        }

        return null;
    }
}
