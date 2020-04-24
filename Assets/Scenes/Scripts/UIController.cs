using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

//TODO: switch city name plates (possibly other UI elements as well) to update
//their visuals in the update() function instead of calling updates from here

public class UIController : MonoBehaviour
{
    HexMap hexMap;

    GameObject unitInfoPanel;
    UnitInfoPanelBehavior unitInfoPanelBehavior;
    GameObject cityInfoPanel;
    CityInfoPanelBehavior cityInfoPanelBehavior;

    ActionController actionController;
    MouseController mouseController;

    Button nextButton;
    Button buildCityButton;

    public LineRenderer movementLineRenderer;
    public LineRenderer borderLineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        hexMap = GameObject.FindObjectOfType<HexMap>();

        unitInfoPanelBehavior = GameObject.Find("Canvas_GameUI").GetComponentInChildren<UnitInfoPanelBehavior>();
        unitInfoPanel = unitInfoPanelBehavior.gameObject;
        cityInfoPanelBehavior = GameObject.Find("Canvas_GameUI").GetComponentInChildren<CityInfoPanelBehavior>();
        cityInfoPanel = cityInfoPanelBehavior.gameObject;
        actionController = FindObjectOfType<ActionController>();
        mouseController = FindObjectOfType<MouseController>();

        Button[] allButtons = GameObject.FindObjectsOfType<Button>();
        nextButton = findButtonByName("NextTurnButton", allButtons);
        buildCityButton = findButtonByName("BuildCityButton", allButtons);

        borderLineRenderer.enabled = true;
        hexMap.onCityCreated += updateCityBorder;

        nextButton.onClick.AddListener(actionController.nextTurn);
        buildCityButton.onClick.AddListener(actionController.buildCity);
        updateSelection(null, null); //disable info panel by default
    }

    public void updateSelection(Unit unit, City city) {
        if(unit != null) {
            unitInfoPanel.SetActive(true);
            unitInfoPanelBehavior.updateSelection(unit);
        } else {
            unitInfoPanel.SetActive(false);
        }

        if(city != null) {
            cityInfoPanel.SetActive(true);
            cityInfoPanelBehavior.updateSelection(city);
        } else {
            cityInfoPanel.SetActive(false);
        }
    }

    public void drawPath(Hex[] path) {
        if (path == null || path.Length == 0) {
            movementLineRenderer.enabled = false;
            return;
        }
        movementLineRenderer.enabled = true;

        Vector3[] positions = new Vector3[path.Length + 1];
        positions[0] = hexMap.getGOFromHex(actionController.getSelectedUnit().hex).transform.position + Vector3.up * .1f;
        for (int i = 1; i < path.Length + 1; i++) {
            GameObject hexGO = hexMap.getGOFromHex(path[i - 1]);
            positions[i] = hexGO.transform.position + Vector3.up * .1f;
        }

        movementLineRenderer.positionCount = positions.Length;
        movementLineRenderer.SetPositions(positions);
    }

    //redraws city borders according to hexes it owns
    public void updateCityBorder(City city, GameObject cityGO) {
        //need a hash set for fast set operations and an array because order matters
        Hex[] hexes = city.getHexes();
        HashSet<Hex> hexSet = new HashSet<Hex>(hexes);

        //need a hashset of positions for fast set operations and array because order matters
        List<Vector3> positions = new List<Vector3>();
        HashSet<Vector3> posSet = new HashSet<Vector3>();

        (Vector3, Vector3) corners = getCorners(HexMap.DIRECTION.NORTHEAST, hexes[1]);
        Vector3[] c = new Vector3[2];
        c[0] = corners.Item1;
        c[1] = corners.Item2;
        borderLineRenderer.positionCount = 2;
        borderLineRenderer.SetPositions(c);

        foreach(Hex h in hexes) {
            print(h.coordsString());
        }
        //loop through each hex that this city owns
        /*foreach(Hex h in hexes) {
            //check if this hex has any neighbors not owned by this city
            Hex[] neighbors = h.getNeighbors();
            foreach(Hex n in neighbors) {
                if(!hexes.Contains<Hex>(n)){
                    //set a line position on the corners that make the border
                    (Vector3, Vector3) corners = getCorners(HexMap.DIRECTION)
                }

            }

        }*/
    }

    //takes a hex and a direction and returns the two vectors of the positions of the
    //corners that make up the border between the hex and its neighbor in the given direction
    //in clockwise order around the city center
    private (Vector3, Vector3) getCorners(HexMap.DIRECTION d, Hex h) {
        //for readability purposes
        float r = Hex.RADIUS;
        float w = Hex.WIDTH / 2;
        float y = .01f;

        switch (d) {
            case HexMap.DIRECTION.NORTHEAST:
                Vector3 corner1 = hexMap.getGOFromHex(h).transform.position + new Vector3(0,y,r);
                Vector3 corner2 = hexMap.getGOFromHex(h).transform.position + new Vector3(w, y, .5f * r);
                return (corner1, corner2);
            /*case HexMap.DIRECTION.EAST:
                break;
            case HexMap.DIRECTION.SOUTHEAST:
                break;
            case HexMap.DIRECTION.SOUTHWEST:
                break;
            case HexMap.DIRECTION.WEST:
                break;
            case HexMap.DIRECTION.NORTHWEST:
                break;*/
        }
        return (Vector3.zero, Vector3.zero);
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
