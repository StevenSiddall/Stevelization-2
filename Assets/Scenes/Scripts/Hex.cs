using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


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

    public readonly HexMap hexMap;

    //terrain data for terrain generation + weather effects
    public float elevation;
    public float moisture;

    private float baseMovementCost = Mathf.Infinity;

    HashSet<Unit> units;

    public Hex(HexMap hexmap, int q, int r) {
        this.hexMap = hexmap;
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

    public Vector3 positionFromCamera() {
        return hexMap.getHexPosition(this);
    }

    public Vector3 positionFromCamera(Vector3 camPos, float numCols, float numRows) {
        float mapWidth = numCols * HORIZ_SPACING;

        Vector3 pos = position();
        float numWidthsFromCam = (pos.x - camPos.x) / mapWidth;

        numWidthsFromCam = Mathf.Round(numWidthsFromCam);

        pos.x -= numWidthsFromCam * mapWidth;
        return pos;
    }

    public static float distance(Hex a, Hex b) {
        //TODO: test this. probably wrong for wrapping
        int dQ = Mathf.Min(Mathf.Abs(a.Q - b.Q),
                            Mathf.Abs(a.hexMap.numCols - Mathf.Abs(a.Q - b.Q)));

        int dS = Mathf.Min(Mathf.Abs(a.S - b.S),
                            Mathf.Abs(a.hexMap.numCols - Mathf.Abs(a.S - b.S)));

        return Mathf.Max(dQ,
                         Mathf.Abs(a.R - b.R),
                         dS);
    }

    public void addUnit(Unit unit) {
        if(units == null) {
            units = new HashSet<Unit>();
        }

        units.Add(unit);
    }

    public void removeUnit(Unit unit) {
        if(units == null) {
            return;
        }

        if(units.Remove(unit) == false) {
            Debug.LogError("Unit not present!");
        }
    }

    public float movementCost() {
        return baseMovementCost;
    }

    public Unit[] getUnitArray() {
        return units.ToArray<Unit>();
    }

    public Hex[] getNeighbors() {
        //TODO: what if this hex is at the north or south edge of the map
        Hex[] hexes = new Hex[6];
        hexes[0] = hexMap.getHex(this.Q,        this.R - 1);
        hexes[1] = hexMap.getHex(this.Q + 1,    this.R - 1);
        hexes[2] = hexMap.getHex(this.Q + 1,    this.R);
        hexes[3] = hexMap.getHex(this.Q,        this.R + 1);
        hexes[4] = hexMap.getHex(this.Q - 1,    this.R + 1);
        hexes[5] = hexMap.getHex(this.Q - 1,    this.R);

        List<Hex> neighbors = new List<Hex>();
        for(int i = 0; i < hexes.Length; i++) {
            if(hexes[i] != null) {
                neighbors.Add(hexes[i]);
            }
        }

        return neighbors.ToArray();
    }

    public string coordsString() {
        return ("(" + this.Q + "," + this.R + ")");
    }

    public void setBaseMovementCost(float newCost) {
        baseMovementCost = newCost;
    }
}
