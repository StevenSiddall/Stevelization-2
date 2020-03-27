﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour {

    public readonly float MOUNTAIN_HTHRESH = 1.2f; //tiles above this elevation are mountains
    public readonly float HILL_HTHRESH = 0.6f; //tiles above this elevation are hills
    public readonly float FLAT_HTHRESH = 0.0f; //tiles above this elevation are flat

    public readonly float RAINFOREST_MTHRESH = .9f;
    public readonly float FOREST_MTHRESH = .6f;
    public readonly float GRASSLAND_MTHRESH = .35f;
    public readonly float PLAINS_MTHRESH = .2f;

    public readonly float INIT_ELEVATION = -0.35f;

    public readonly float FLAT_MOVECOST = 1;
    public readonly float HILL_MOVECOST = 2;
    public readonly float FOREST_MOVECOST = 2;
    public readonly float RAINFOREST_MOVECOST = 2;
    public readonly float MOUNTAIN_MOVECOST = Mathf.Infinity;
    public readonly float WATER_MOVECOST = Mathf.Infinity;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if(units != null) {
                foreach(Unit u in units) {
                    u.doTurn();
                }
            }
        }
    }

    //terrain stuff
    public GameObject hexPrefab;

    public Mesh meshWater;
    public Mesh meshFlat;
    public Mesh meshHill;
    public Mesh meshMountain;

    public GameObject rainForestPrefab;
    public GameObject forestPrefab;
    public GameObject rainForestHillPrefab;
    public GameObject forestHillPrefab;

    public Material matOcean;
    public Material matPlains;
    public Material matGrassland;
    public Material matMountain;
    public Material matDesert;

    public readonly int numCols = 80;
    public readonly int numRows = 40;

    private Hex[,] hexes;
    private Dictionary<Hex, GameObject> hexToGOMap;
    private Dictionary<GameObject, Hex> goToHexMap;

    //unit stuff
    public GameObject unitFootsoldierPrefab;

    public Unit selectedUnit;
    private HashSet<Unit> units;
    private Dictionary<Unit, GameObject> unitToGOMap;

    public Hex getHex(int q, int r) {
        if(hexes == null) {
            Debug.LogError("Hexes array not yet instantiated");
            return null;
        }

        //world wrap horizontal
        q = q % numCols;
        if (q < 0) {
            q += numCols;
        }

        if (q < 0 || q >= numCols) {
            Debug.Log("q out of bounds: " + q);
            return null;
        } else if (r < 0 || r >= numRows) {
            Debug.Log("r out of bounds: " + r);
            return null;
        }

        return hexes[q, r];
    }

    virtual public void GenerateMap() {

        // Generate a map filled with ocean
        hexes = new Hex[numCols, numRows];
        hexToGOMap = new Dictionary<Hex, GameObject>();
        goToHexMap = new Dictionary<GameObject, Hex>();

        for(int col = 0; col < numCols; col++) {
            for(int row = 0; row < numRows; row++) {
                Hex h = new Hex(this, col, row);
                h.elevation = INIT_ELEVATION;

                hexes[col, row] = h;

                Vector3 pos = h.positionFromCamera(
                    Camera.main.transform.position,
                    numCols,
                    numRows
                );

                GameObject hexGO = Instantiate(hexPrefab,
                            pos,
                            Quaternion.identity,
                            this.transform);

                hexToGOMap[h] = hexGO;
                goToHexMap[hexGO] = h;

                hexGO.GetComponent<HexBehavior>().hex = h;
                hexGO.GetComponent<HexBehavior>().hexMap = this;
                //hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", col, row);
                hexGO.GetComponentInChildren<TextMesh>().text = "";
                hexGO.name = string.Format("HEX: {0},{1}", col, row);


                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                mr.material = matOcean;

                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();
                mf.mesh = meshWater;
            }
        }
    }

    public void updateHexVisuals() {
        for (int col = 0; col < numCols; col++) {
            for (int row = 0; row < numRows; row++) {
                Hex h = hexes[col, row];
                GameObject hexGO = hexToGOMap[h];

                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();

                //set model and movement cost based on moisture
                if(h.elevation >= FLAT_HTHRESH && h.elevation < MOUNTAIN_HTHRESH) {
                    if (h.moisture >= RAINFOREST_MTHRESH) {
                        mr.material = matGrassland;
                        h.setBaseMovementCost(RAINFOREST_MOVECOST);
                        if (h.elevation >= HILL_HTHRESH) { //check if we should use rainforest for hills or flat land
                            GameObject.Instantiate(rainForestHillPrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                        } else {
                            GameObject.Instantiate(rainForestPrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                        }
                        
                    } else if (h.moisture >= FOREST_MTHRESH) {
                        mr.material = matGrassland;
                        h.setBaseMovementCost(FOREST_MOVECOST);
                        if (h.elevation >= HILL_HTHRESH) { //check if we should use forest for hills or flat land
                            GameObject.Instantiate(forestHillPrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                        } else {
                            GameObject.Instantiate(forestPrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                        }
                    } else if (h.moisture >= GRASSLAND_MTHRESH) {
                        mr.material = matGrassland;
                        h.setBaseMovementCost(FLAT_MOVECOST);
                    } else if (h.moisture >= PLAINS_MTHRESH) {
                        mr.material = matPlains;
                        h.setBaseMovementCost(FLAT_MOVECOST);
                    } else {
                        mr.material = matDesert;
                        h.setBaseMovementCost(FLAT_MOVECOST);
                    }
                }

                //set model and mesh based on elevation
                if (h.elevation >= MOUNTAIN_HTHRESH) {
                    mr.material = matMountain;
                    mf.mesh = meshMountain;
                    h.setBaseMovementCost(MOUNTAIN_MOVECOST);
                } else if (h.elevation >= HILL_HTHRESH) {
                    mf.mesh = meshHill;
                    h.setBaseMovementCost(HILL_MOVECOST);
                } else if (h.elevation >= FLAT_HTHRESH) {
                    mf.mesh = meshWater;
                } else {
                    mr.material = matOcean;
                    mf.mesh = meshFlat;
                    h.setBaseMovementCost(WATER_MOVECOST);
                }

                //hexGO.GetComponentInChildren<TextMesh>().text += ("\n" + h.movementCost());
            }
        }
    }

    public Vector3 getHexPosition(int q, int r) {
        Hex h = getHex(q, r);

        return getHexPosition(h);
    }

    public Vector3 getHexPosition(Hex h) {
        return h.positionFromCamera(Camera.main.transform.position, numCols, numRows);
    }

    public Hex[] getHexesWithinRange(Hex centerHex, int range) {
        List<Hex> results = new List<Hex>();

        for(int dx = -range; dx <= range; dx++) {
            for(int dy = Mathf.Max(-range, -dx-range); dy <= Mathf.Min(range, -dx+range); dy++) {
                results.Add(getHex(centerHex.Q + dx, centerHex.R + dy));
            }
        }

        return results.ToArray();
    }

    public Hex GetHexFromGameObject(GameObject hexGO) {
        if (goToHexMap.ContainsKey(hexGO)) {
            return goToHexMap[hexGO];
        }

        return null;
    }

    public void spawnUnitAt(Unit unit, GameObject prefab, int q, int r) {
        if(units == null) {
            units = new HashSet<Unit>();
            unitToGOMap = new Dictionary<Unit, GameObject>();
        }

        GameObject unitHex = hexToGOMap[getHex(q, r)];
        unit.setHex(getHex(q, r));

        GameObject unitGO = Instantiate(prefab, unitHex.transform.position, Quaternion.identity, unitHex.transform);
        unit.onUnitMoved += unitGO.GetComponent<UnitView>().onUnitMoved;

        units.Add(unit);
        unitToGOMap[unit] = unitGO;
    }
}
