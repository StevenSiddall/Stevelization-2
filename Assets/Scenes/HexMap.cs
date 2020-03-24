using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour {

    public readonly float MOUNTAIN_HTHRESH = 0.85f; //tiles above this elevation are mountains
    public readonly float HILL_HTHRESH = 0.6f; //tiles above this elevation are hills
    public readonly float FLAT_HTHRESH = 0.0f; //tiles above this elevation are flat

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

    public Material MatOcean;
    public Material MatPlains;
    public Material MatGrassland;
    public Material MatMountain;

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
                h.elevation = -0.5f;

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
                hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", col, row);

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
                if(h.elevation >= MOUNTAIN_HTHRESH) {
                    mr.material = MatMountain;
                } else if (h.elevation >= HILL_HTHRESH) {
                    mr.material = MatGrassland;
                } else if (h.elevation >= FLAT_HTHRESH) {
                    mr.material = MatPlains;
                } else {
                    mr.material = MatOcean;
                }

                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();
                mf.mesh = MeshWater;
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
