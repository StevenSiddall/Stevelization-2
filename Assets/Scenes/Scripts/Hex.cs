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

    //unit movement stuff
    static readonly float BASE_MOVECOST = 1f; //cost of entering an unimproved flat tile
    static readonly float HILL_MOVECOST = 2f; //cost of entering an unimproved hill tile
    static readonly float FOREST_MOVECOST = 2f; //cost of entering an unimproved forest tile
    static readonly float RAINFOREST_MOVECOST = 2f; //cost of entering an unimproved rainforest tile
    static readonly float MOUNTAIN_MOVECOST = Mathf.Infinity; //cost of entering an unimproved mountain tile
    static readonly float WATER_MOVECOST = Mathf.Infinity; //cost of entering an unimproved water tile

    //hexmap stuff
    public readonly int Q;
    public readonly int R;
    public readonly int S;

    public readonly HexMap hexMap;

    //terrain stuff
    public float elevation;
    public float moisture;

    public enum TERRAIN_TYPE { PLAINS, GRASSLANDS, DESERT, WATER }
    public enum ELEVATION_TYPE { FLAT, HILL, MOUNTAIN, WATER }
    public enum FEATURE_TYPE { NONE, FOREST, RAINFOREST }

    private TERRAIN_TYPE terrainType;
    private ELEVATION_TYPE elevationType;
    private FEATURE_TYPE featureType;
    //TODO: add a list of modifiers -- includes improvements, buildings, etc.
    //TODO: add data about owner, population, etc


    private float movementCost = Mathf.Infinity;

    HashSet<Unit> units;

    public Hex(HexMap hexmap, int q, int r) {
        this.hexMap = hexmap;
        this.Q = q;
        this.R = r;
        this.S = -(q + r);

        units = new HashSet<Unit>();
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
            Debug.LogError("No unit to remove");
        }
    }

    public float getMovementCost() {
        return movementCost;
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

    public void setTerrainType(TERRAIN_TYPE newterrain) {
        this.terrainType = newterrain;
        updateMovementCost();
    }

    public void setElevationType(ELEVATION_TYPE newelev) {
        this.elevationType = newelev;
        updateMovementCost();
    }

    public void setFeature(FEATURE_TYPE newfeature) {
        this.featureType = newfeature;
        updateMovementCost();
    }

    public TERRAIN_TYPE getTerrainType () {
        return terrainType;
    }

    public ELEVATION_TYPE getElevationType () {
        return elevationType;
    }

    public FEATURE_TYPE getFeatureType() {
        return featureType;
    }

    //updates this tile's movement cost based on its type and features
    //shouldn't need to ever call this from outside this class since setters should call it
    public void updateMovementCost() {
        movementCost = BASE_MOVECOST;

        //check the elevation
        switch (elevationType) {
        case ELEVATION_TYPE.FLAT:
            break;
        case ELEVATION_TYPE.HILL:
            movementCost = HILL_MOVECOST;
            break;
        case ELEVATION_TYPE.MOUNTAIN:
            movementCost = MOUNTAIN_MOVECOST;
            break;
        case ELEVATION_TYPE.WATER:
            movementCost = WATER_MOVECOST;
            break;
        }

        //check feature.
        switch (featureType) {
        case FEATURE_TYPE.NONE:
            break;
        case FEATURE_TYPE.RAINFOREST:
            movementCost = Mathf.Max(movementCost, RAINFOREST_MOVECOST);
            break;
        case FEATURE_TYPE.FOREST:
            movementCost = Mathf.Max(movementCost, FOREST_MOVECOST);
            break;
        }

        //this is where we will add checks for improvements, etc.

    }


}
