//#define DEBUG_LABELS

using System.Collections;
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
    public GameObject cityPrefab;

    private HashSet<Unit> units;
    private Dictionary<Unit, GameObject> unitToGOMap;

    private HashSet<City> cities;
    private Dictionary<City, GameObject> cityToGOMap;
    private Dictionary<GameObject, City> goToCityMap;

    //event stuff
    public delegate void cityCreatedDelegate(City city, GameObject cityGO);
    public event cityCreatedDelegate onCityCreated;

    // Start is called before the first frame update
    void Start() {
        GenerateMap();
    }


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

#if DEBUG_LABELS
                hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", col, row);
#else
                hexGO.GetComponentInChildren<TextMesh>().text = "";
#endif
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
                        h.setFeature(Hex.FEATURE_TYPE.RAINFOREST);
                        h.setTerrainType(Hex.TERRAIN_TYPE.GRASSLANDS);
                        if (h.elevation >= HILL_HTHRESH) { //check if we should use rainforest for hills or flat land
                            GameObject.Instantiate(rainForestHillPrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                        } else {
                            GameObject.Instantiate(rainForestPrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                        }
                        
                    } else if (h.moisture >= FOREST_MTHRESH) {
                        mr.material = matGrassland;
                        h.setFeature(Hex.FEATURE_TYPE.FOREST);
                        h.setTerrainType(Hex.TERRAIN_TYPE.GRASSLANDS);
                        if (h.elevation >= HILL_HTHRESH) { //check if we should use forest for hills or flat land
                            GameObject.Instantiate(forestHillPrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                        } else {
                            GameObject.Instantiate(forestPrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                        }
                    } else if (h.moisture >= GRASSLAND_MTHRESH) {
                        mr.material = matGrassland;
                        h.setFeature(Hex.FEATURE_TYPE.NONE);
                        h.setTerrainType(Hex.TERRAIN_TYPE.GRASSLANDS);
                    } else if (h.moisture >= PLAINS_MTHRESH) {
                        mr.material = matPlains;
                        h.setFeature(Hex.FEATURE_TYPE.NONE);
                        h.setTerrainType(Hex.TERRAIN_TYPE.PLAINS);
                    } else {
                        mr.material = matDesert;
                        h.setFeature(Hex.FEATURE_TYPE.NONE);
                        h.setTerrainType(Hex.TERRAIN_TYPE.DESERT);
                    }
                }

                //set model and mesh based on elevation
                if (h.elevation >= MOUNTAIN_HTHRESH) {
                    mr.material = matMountain;
                    mf.mesh = meshMountain;
                    h.setElevationType(Hex.ELEVATION_TYPE.MOUNTAIN);
                } else if (h.elevation >= HILL_HTHRESH) {
                    mf.mesh = meshHill;
                    h.setElevationType(Hex.ELEVATION_TYPE.HILL);
                } else if (h.elevation >= FLAT_HTHRESH) {
                    mf.mesh = meshFlat;
                    h.setElevationType(Hex.ELEVATION_TYPE.FLAT);
                } else {
                    mr.material = matOcean;
                    mf.mesh = meshWater;
                    h.setElevationType(Hex.ELEVATION_TYPE.WATER);
                    h.setFeature(Hex.FEATURE_TYPE.NONE);
                    h.setTerrainType(Hex.TERRAIN_TYPE.WATER);
                }

#if DEBUG_LABELS
                hexGO.GetComponentInChildren<TextMesh>().text += ("\n" + h.getMovementCost());
#endif
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

    public Hex getHexFromGO(GameObject hexGO) {
        if (goToHexMap.ContainsKey(hexGO)) {
            return goToHexMap[hexGO];
        }

        return null;
    }

    public GameObject getGOFromHex(Hex h) {
        if (hexToGOMap.ContainsKey(h)) {
            return hexToGOMap[h];
        }

        return null;
    }

    public City getCityFromGO(GameObject cityGO) {
        if (goToCityMap.ContainsKey(cityGO)) {
            return goToCityMap[cityGO];
        }

        return null;
    }

    public HashSet<Unit> getUnits() {
        return units;
    }

    public void spawnUnitAt(Unit unit, GameObject prefab, int q, int r) {
        if(units == null) {
            units = new HashSet<Unit>();
            unitToGOMap = new Dictionary<Unit, GameObject>();
        }

        GameObject unitHexGO = hexToGOMap[getHex(q, r)];
        unit.setHex(getHex(q, r));

        GameObject unitGO = Instantiate(prefab, unitHexGO.transform.position, Quaternion.identity, unitHexGO.transform);
        UnitView unitView = unitGO.GetComponent<UnitView>();
        unit.unitView = unitView;
        unit.onObjectMoved += unitView.onUnitMoved;

        units.Add(unit);
        unitToGOMap[unit] = unitGO;
    }

    public void spawnCityAt(City city, GameObject prefab, int q, int r) {
        if (cities == null) {
            cities = new HashSet<City>();
            cityToGOMap = new Dictionary<City, GameObject>();
            goToCityMap = new Dictionary<GameObject, City>();
        }

        Hex cityHex = getHex(q, r);
        GameObject cityHexGO = hexToGOMap[cityHex];

        try {
            city.setHex(cityHex);
        } catch (UnityException e) {
            Debug.LogError(e.Message);
            return;
        }

        GameObject cityGO = Instantiate(prefab, cityHexGO.transform.position, Quaternion.identity, cityHexGO.transform);

        cities.Add(city);
        cityToGOMap[city] = cityGO;
        goToCityMap[cityGO] = city;

        if(onCityCreated != null) {
            onCityCreated(city, cityGO);
        }
    }
}
