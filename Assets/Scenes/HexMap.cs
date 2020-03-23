using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour {
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

    public readonly int numCols = 50;
    public readonly int numRows = 20;

    private Hex[,] hexes;
    private Dictionary<Hex, GameObject> hexToGOMap;

    public Hex getHex(int x, int y) {
        if(hexes == null) {
            Debug.LogError("Hexes array not yet instantiated");
            return null;
        }

        //world wrap horizontal
        x = x % numCols;

        return hexes[x, y];
    }

    virtual public void GenerateMap() {

        // Generate a map filled with ocean
        hexes = new Hex[numCols, numRows];
        hexToGOMap = new Dictionary<Hex, GameObject>();

        for(int col = 0; col < numCols; col++) {
            for(int row = 0; row < numRows; row++) {
                Hex h = new Hex(col, row);
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
                hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", row, col);

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
                if(h.elevation >= 0) {
                    mr.material = MatGrassland;
                }else {
                    mr.material = MatOcean;
                }

                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();
                mf.mesh = MeshWater;
            }
        }
    }

    public Hex[] getHexesWithinRange(Hex centerHex, int range) {
        List<Hex> results = new List<Hex>();

        for(int dx = -range; dx < range - 1; dx++) {
            for(int dy = Mathf.Max(-range + 1, -dx - range); dy < Mathf.Min(range, -dx + range - 1); dy++) {
                results.Add(hexes[centerHex.Q + dx, centerHex.R + dy]);
            }
        }

        return results.ToArray();
    }
}
