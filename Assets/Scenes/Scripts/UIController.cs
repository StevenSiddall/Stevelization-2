using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.Networking;

//TODO: switch city name plates (possibly other UI elements as well) to update
//their visuals in the update() function instead of calling updates from here

public class UIController : MonoBehaviour
{
    public Material borderMaterial;
    public InputField cityInfoPanelNameText;


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
    public GameObject borderLineContainer;
    public Dictionary<City, LineRenderer> borderLineRenderers;
    public Dictionary<City, MapObjectNamePlate> cityNamePlates;

    //useful constants for getting corner locations of a hex
    private readonly float r = Hex.RADIUS;
    private readonly float w = Hex.WIDTH / 2;
    private readonly float z = Hex.RADIUS / 2;
    private readonly float y = .01f;

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

        borderLineRenderers = new Dictionary<City, LineRenderer>();
        cityNamePlates = new Dictionary<City, MapObjectNamePlate>();

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
        //get line renderer for this city
        LineRenderer borderLineRenderer;
        if (!borderLineRenderers.ContainsKey(city)) {
            GameObject lineGO = new GameObject("LineRenderer: " + city.name);
            lineGO.transform.position = Vector3.zero;
            lineGO.transform.parent = borderLineContainer.transform;
            lineGO.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            lineGO.AddComponent<LineRenderer>();
            LineRenderer lr = lineGO.GetComponent<LineRenderer>();
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
            lr.allowOcclusionWhenDynamic = false;
            lr.useWorldSpace = true;
            lr.loop = true;
            lr.positionCount = 0;
            lr.alignment = LineAlignment.TransformZ;
            lr.material = borderMaterial;
            lr.startWidth = .1f;
            lr.endWidth = .1f;
            lr.enabled = true;

            borderLineRenderers[city] = lr;
        }
        borderLineRenderer = borderLineRenderers[city];


        //need a hash set for fast set operations and an array because order matters
        Hex[] hexes = city.getHexes();
        HashSet<Hex> hexSet = new HashSet<Hex>(hexes);

        //need a hashset of positions for fast set operations and array because order matters
        List<Vector3> positions = new List<Vector3>();
        HashSet<Vector3> posSet = new HashSet<Vector3>();

        //loop through each hex that this city owns
        foreach(Hex h in hexes) {
            //check if this hex has any neighbors not owned by this city

            //the order that we check the neighbors is very important.
            //We need to start in the direction of the city center then
            //move clockwise
            //This probably only works if the hexes array is in clockwise order

            //figure out which direction the city center is in
            HexMap.DIRECTION dir = getCityDir(city.hex, h);

            Hex[] neighbors = h.getNeighbors();
            //print(h.coordsString() + ", " + Enum.GetName(typeof(HexMap.DIRECTION), dir));
            for (int i = 0; i < 6; i++) {
                Hex n = neighbors[(int) dir];
                if(n != null && !hexSet.Contains<Hex>(n)) {
                    //set a line position on the corners that make the border
                    (Vector3, Vector3) corners = getCorners(dir, h);

                    //round values to we don't miss duplicates due to floating point error
                    corners.Item1.x = (float)Math.Round((Decimal)corners.Item1.x, 3, MidpointRounding.AwayFromZero);
                    corners.Item1.y = (float)Math.Round((Decimal)corners.Item1.y, 3, MidpointRounding.AwayFromZero);
                    corners.Item1.z = (float)Math.Round((Decimal)corners.Item1.z, 3, MidpointRounding.AwayFromZero);

                    corners.Item2.x = (float)Math.Round((Decimal)corners.Item2.x, 3, MidpointRounding.AwayFromZero);
                    corners.Item2.y = (float)Math.Round((Decimal)corners.Item2.y, 3, MidpointRounding.AwayFromZero);
                    corners.Item2.z = (float)Math.Round((Decimal)corners.Item2.z, 3, MidpointRounding.AwayFromZero);

                    //make sure we don't add duplicates
                    if (!posSet.Contains(corners.Item1)) {
                        positions.Add(corners.Item1);
                        posSet.Add(corners.Item1);
                    }

                    if (!posSet.Contains(corners.Item2)) {
                        positions.Add(corners.Item2);
                        posSet.Add(corners.Item2);
                    }

                    dir = (HexMap.DIRECTION) (((int) dir + 1) % 6);
                }
            }
        }

        borderLineRenderer.positionCount = positions.Count;
        borderLineRenderer.loop = true;
        borderLineRenderer.SetPositions(positions.ToArray());
    }

    //part of the onClick for the input text on the city info panel
    //set in editor
    public void renameCity(string doNotUseMe) {
        //for some reason the arg string isn't what the player typed in
        //its the default string (empty)
        string newName = cityInfoPanelNameText.GetComponent<InputField>().text;
        City city = actionController.getSelectedCity();
        actionController.renameCity(city, newName);
        cityNamePlates[city].GetComponentInChildren<Text>().text = city.name;
        print("city has been renamed to " + city.name);
    }

    public void mapCityToNameplate(City city, MapObjectNamePlate np) {
        cityNamePlates[city] = np;
    }

    //takes a hex and a direction and returns the two vectors of the positions of the
    //corners that make up the border between the hex and its neighbor in the given direction
    //in clockwise order around the city center
    private (Vector3, Vector3) getCorners(HexMap.DIRECTION d, Hex h) {

        Vector3 pos = hexMap.getGOFromHex(h).transform.position;

        //vectors to get us the corner from the hex transform position
        Vector3 n  = pos + new Vector3(0,  y, r);  //north
        Vector3 ne = pos + new Vector3(w,  y, z);  //north east
        Vector3 se = pos + new Vector3(w,  y, -z); //south east
        Vector3 s  = pos + new Vector3(0,  y, -r); //south
        Vector3 sw = pos + new Vector3(-w, y, -z); //south west
        Vector3 nw = pos + new Vector3(-w, y, z);  //north west

        switch (d) {
            case HexMap.DIRECTION.NORTHEAST:
                return (n, ne);
            case HexMap.DIRECTION.EAST:
                return (ne, se);
            case HexMap.DIRECTION.SOUTHEAST:
                return (se, s);
            case HexMap.DIRECTION.SOUTHWEST:
                return (s, sw);
            case HexMap.DIRECTION.WEST:
                return (sw, nw);
            case HexMap.DIRECTION.NORTHWEST:
                return (nw, n);
        }
        return (Vector3.zero, Vector3.zero);
    }

    private HexMap.DIRECTION getCityDir(Hex city, Hex h) {
        Transform hexT = hexMap.getGOFromHex(h).transform;
        Transform cityT = hexMap.getGOFromHex(city).transform;

        float angle = Vector3.SignedAngle(hexT.right, hexT.position - cityT.position, hexT.up);
        if(angle < 0) {
            angle += 360;
        }

        if(angle > 270 && angle <= 330) {
            return HexMap.DIRECTION.NORTHEAST;
        } else if(angle > 330 || angle <= 30) {
            return HexMap.DIRECTION.EAST;
        } else if(angle > 30 && angle <= 90) {
            return HexMap.DIRECTION.SOUTHEAST;
        } else if(angle > 90 && angle <= 150) {
            return HexMap.DIRECTION.SOUTHWEST;
        } else if (angle > 150 && angle <= 210) {
            return HexMap.DIRECTION.WEST;
        } else if (angle > 210 && angle <= 270) {
            return HexMap.DIRECTION.NORTHWEST;
        } else {
            Debug.LogError("Couldn't find angle between hex and city");
            return 0;
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
