using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Defines grid position, world position, size, neighbors, etc of a hex tile
 * */
public class Hex {

    static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2f; //used to calculate height
    static readonly float RADIUS = 1f;      //distance from center of hex to corner
    static readonly float HEIGHT = RADIUS * 2f;      //vertical height of a hex (corner to opposite corner)
    static readonly float WIDTH = WIDTH_MULTIPLIER * HEIGHT;        //distance from flat edge to opposite flat edge
    static readonly float VERT_SPACING = HEIGHT * 0.75f;
    static readonly float HORIZ_SPACING = WIDTH;

    public readonly int Q;
    public readonly int R;
    public readonly int S;

    public Hex(int q, int r) {
        this.Q = q;
        this.R = r;
        this.S = -(q + r);
    }

    /*
     * Returns world space position of this hex
     * */
    public Vector3 position() {
        return new Vector3(
            HORIZ_SPACING * (this.Q + this.R / 2f),
            0,
            VERT_SPACING * this.R);
    }

    public Vector3 positionFromCamera(Vector3 camPos, float numCols, float numRows) {
        float mapWidth = numCols * HORIZ_SPACING;

        Vector3 pos = position();
        float numWidthsFromCam = (pos.x - camPos.x) / mapWidth;

        numWidthsFromCam = Mathf.Round(numWidthsFromCam);

        pos.x -= numWidthsFromCam * mapWidth;
        return pos;
    }
}
