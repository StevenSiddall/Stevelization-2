using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexBehavior : MonoBehaviour
{
    public Hex hex;
    public HexMap hexMap;

    public void UpdatePosition() {
        this.transform.position = hex.PositionFromCamera(
                                        Camera.main.transform.position,
                                        hexMap.numCols,
                                        hexMap.numRows);
    }
}
