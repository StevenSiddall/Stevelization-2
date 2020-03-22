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

    virtual public void GenerateMap() {
        for(int col = 0; col < numCols; col++) {
            for(int row = 0; row < numRows; row++) {
                Hex h = new Hex(col, row);

                Vector3 pos = h.positionFromCamera(
                    Camera.main.transform.position,
                    numCols,
                    numRows
                );

                GameObject hexGO = Instantiate(HexPrefab,
                            pos,
                            Quaternion.identity,
                            this.transform);

                hexGO.GetComponent<HexBehavior>().hex = h;
                hexGO.GetComponent<HexBehavior>().hexMap = this;
                hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", row, col);

                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                mr.material = HexMaterials[Random.Range(0, HexMaterials.Length)];
            }
        }
    }
}
