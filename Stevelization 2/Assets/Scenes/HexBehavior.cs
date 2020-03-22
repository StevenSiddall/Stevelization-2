using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexBehavior : MonoBehaviour
{
    public Hex hex;
    public HexMap hexMap;

    public void updatePosition() {
        this.transform.position = hex.positionFromCamera(
                                        Camera.main.transform.position,
                                        hexMap.numCols,
                                        hexMap.numRows);
    }
}
