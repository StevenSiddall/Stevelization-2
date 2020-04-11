using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    HexMap hexMap;

    GameObject infoPanel;
    UnitInfoPanelBehavior infoPanelBehavior;
    ActionController actionController;
    MouseController mouseController;

    Button nextButton;
    Button buildCityButton;

    LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        hexMap = GameObject.FindObjectOfType<HexMap>();

        infoPanelBehavior = GameObject.Find("Canvas_GameUI").GetComponentInChildren<UnitInfoPanelBehavior>();
        infoPanel = infoPanelBehavior.gameObject;
        actionController = FindObjectOfType<ActionController>();
        mouseController = FindObjectOfType<MouseController>();

        Button[] allButtons = GameObject.FindObjectsOfType<Button>();
        nextButton = findButtonByName("NextTurnButton", allButtons);
        buildCityButton = findButtonByName("BuildCityButton", allButtons);

        lineRenderer = transform.GetComponentInChildren<LineRenderer>();

        nextButton.onClick.AddListener(actionController.nextTurn);
        buildCityButton.onClick.AddListener(actionController.buildCity);
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

    public void drawPath(Hex[] path) {
        if (path == null || path.Length == 0) {
            lineRenderer.enabled = false;
            return;
        }
        lineRenderer.enabled = true;

        Vector3[] positions = new Vector3[path.Length + 1];
        positions[0] = hexMap.getGOFromHex(actionController.getSelectedUnit().hex).transform.position + Vector3.up * .1f;
        for (int i = 1; i < path.Length + 1; i++) {
            GameObject hexGO = hexMap.getGOFromHex(path[i - 1]);
            positions[i] = hexGO.transform.position + Vector3.up * .1f;
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
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
