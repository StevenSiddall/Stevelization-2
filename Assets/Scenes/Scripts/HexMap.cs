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

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    public GameObject HexPrefab;

    public Mesh MeshWater;
    public Mesh MeshFlat;
    public Mesh MeshHill;
    public Mesh MeshMountain;

    public GameObject rainForestPrefab;
    public GameObject forestPrefab;
    public GameObject rainForestHillPrefab;
    public GameObject forestHillPrefab;

    public Material MatOcean;
    public Material MatPlains;
    public Material MatGrassland;
    public Material MatMountain;
    public Material MatDesert;

    public readonly int numCols = 80;
    public readonly int numRows = 40;

    private Hex[,] hexes;
    private Dictionary<Hex, GameObject> hexToGOMap;

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
            Debug.LogError("q out of bounds: " + q);
            return null;
        } else if (r < 0 || r >= numRows) {
            Debug.LogError("r out of bounds: " + r);
            return null;
        }

        return hexes[q, r];
    }

    virtual public void GenerateMap() {

        // Generate a map filled with ocean
        hexes = new Hex[numCols, numRows];
        hexToGOMap = new Dictionary<Hex, GameObject>();

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

                GameObject hexGO = Instantiate(HexPrefab,
                            pos,
                            Quaternion.identity,
                            this.transform);

                hexToGOMap[h] = hexGO;

                hexGO.GetComponent<HexBehavior>().hex = h;
                hexGO.GetComponent<HexBehavior>().hexMap = this;
                //hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", col, row);
                hexGO.GetComponentInChildren<TextMesh>().text = "";


                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                mr.material = MatOcean;

                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();
                mf.mesh = MeshWater;
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

                //set model based on moisture
                if(h.elevation >= FLAT_HTHRESH && h.elevation < MOUNTAIN_HTHRESH) {
                    if (h.moisture >= RAINFOREST_MTHRESH) {
                        mr.material = MatGrassland;
                        if(h.elevation >= HILL_HTHRESH) { //check if we should use rainforest for hills or flat land
                            GameObject.Instantiate(rainForestHillPrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                        } else {
                            GameObject.Instantiate(rainForestPrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                        }
                        
                    } else if (h.moisture >= FOREST_MTHRESH) {
                        mr.material = MatGrassland;
                        if (h.elevation >= HILL_HTHRESH) { //check if we should use forest for hills or flat land
                            GameObject.Instantiate(forestHillPrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                        } else {
                            GameObject.Instantiate(forestPrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                        }
                    } else if (h.moisture >= GRASSLAND_MTHRESH) {
                        mr.material = MatGrassland;
                    } else if (h.moisture >= PLAINS_MTHRESH) {
                        mr.material = MatPlains;
                    } else {
                        mr.material = MatDesert;
                    }
                }

                //set model and mesh based on elevation
                if (h.elevation >= MOUNTAIN_HTHRESH) {
                    mr.material = MatMountain;
                    mf.mesh = MeshMountain;
                } else if (h.elevation >= HILL_HTHRESH) {
                    mf.mesh = MeshHill;
                } else if (h.elevation >= FLAT_HTHRESH) {
                    mf.mesh = MeshWater;
                } else {
                    mr.material = MatOcean;
                    mf.mesh = MeshFlat;
                }
            }
        }
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
}
