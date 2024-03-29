﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * Defines grid position, world position, size, neighbors, etc of a hex tile
 * */
public class Hex {

    public static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2f; //used to calculate height
    public static readonly float RADIUS = 1f;      //distance from center of hex to corner
    public static readonly float HEIGHT = RADIUS * 2f;      //vertical height of a hex (corner to opposite corner)
    public static readonly float WIDTH = WIDTH_MULTIPLIER * HEIGHT;        //distance from flat edge to opposite flat edge
    public static readonly float VERT_SPACING = HEIGHT * 0.75f;
    public static readonly float HORIZ_SPACING = WIDTH;

    // Unit movement stuff
    public static readonly float BASE_MOVECOST = 1f; //cost of entering an unimproved flat tile
    public static readonly float HILL_MOVECOST = 2f; //cost of entering an unimproved hill tile
    public static readonly float FOREST_MOVECOST = 2f; //cost of entering an unimproved forest tile
    public static readonly float RAINFOREST_MOVECOST = 2f; //cost of entering an unimproved rainforest tile
    public static readonly float MOUNTAIN_MOVECOST = Mathf.Infinity; //cost of entering an unimproved mountain tile
    public static readonly float WATER_MOVECOST = Mathf.Infinity; //cost of entering an unimproved water tile

    // Terrain stuff
    public float elevation;
    public float moisture;

    public enum TERRAIN_TYPE { PLAINS, GRASSLANDS, DESERT, WATER }
    public enum ELEVATION_TYPE { FLAT, HILL, MOUNTAIN, WATER }
    public enum FEATURE_TYPE { NONE, FOREST, RAINFOREST }

    private TERRAIN_TYPE terrainType;
    private ELEVATION_TYPE elevationType;
    private FEATURE_TYPE featureType;

    // Hexmap stuff
    public readonly int Q;
    public readonly int R;
    public readonly int S;

    public readonly HexMap hexMap;

    HashSet<Unit> units;
    public City city { get; protected set; }

    private float movementCost = Mathf.Infinity;

    // Zone stuff
    private Zone zone;

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
    public Vector3 Position() {
        return new Vector3(
            HORIZ_SPACING * (this.Q + this.R / 2f),
            0,
            VERT_SPACING * this.R);
    }

    public Vector3 PositionFromCamera() {
        return hexMap.GetHexPosition(this);
    }

    public Vector3 PositionFromCamera(Vector3 camPos, float numCols, float numRows) {
        float mapWidth = numCols * HORIZ_SPACING;

        Vector3 pos = Position();
        float numWidthsFromCam = (pos.x - camPos.x) / mapWidth;

        numWidthsFromCam = Mathf.Round(numWidthsFromCam);

        pos.x -= numWidthsFromCam * mapWidth;
        return pos;
    }

    public static float Distance(Hex a, Hex b) {
        //TODO: test this. probably wrong for wrapping
        int dQ = Mathf.Min(Mathf.Abs(a.Q - b.Q),
                            Mathf.Abs(a.hexMap.numCols - Mathf.Abs(a.Q - b.Q)));

        int dS = Mathf.Min(Mathf.Abs(a.S - b.S),
                            Mathf.Abs(a.hexMap.numCols - Mathf.Abs(a.S - b.S)));

        return Mathf.Max(dQ,
                         Mathf.Abs(a.R - b.R),
                         dS);
    }

    public void AddUnit(Unit unit) {
        if(units == null) {
            units = new HashSet<Unit>();
        }

        units.Add(unit);
    }

    public void AddCity(City newCity) {
        if(this.city != null) {
            throw new UnityException("Trying to add a city to a tile that already has one! UI shouldn't allow this!");
        }

        this.city = newCity;
    }

    public void RemoveCity() {
        if(this.city == null) {
            Debug.LogError("Trying to remove a city that isn't present!");
        }

        this.city = null;
    }

    public void RemoveUnit(Unit unit) {
        if(units == null) {
            return;
        }

        if(units.Remove(unit) == false) {
            Debug.LogError("No unit to remove");
        }
    }

    public float GetMovementCost() {
        return movementCost;
    }

    public Unit[] GetUnitArray() {
        return units.ToArray<Unit>();
    }

    public Hex[] GetNeighbors() {
        //TODO: what if this hex is at the north or south edge of the map
        Hex[] hexes = new Hex[6];
        hexes[(int)HexMap.DIRECTION.NORTHEAST] = hexMap.GetHex(this.Q, this.R + 1); 
        hexes[(int)HexMap.DIRECTION.EAST] = hexMap.GetHex(this.Q + 1, this.R); 
        hexes[(int)HexMap.DIRECTION.SOUTHEAST] = hexMap.GetHex(this.Q + 1, this.R - 1);
        hexes[(int)HexMap.DIRECTION.SOUTHWEST] = hexMap.GetHex(this.Q, this.R - 1);
        hexes[(int)HexMap.DIRECTION.WEST] = hexMap.GetHex(this.Q - 1, this.R);
        hexes[(int)HexMap.DIRECTION.NORTHWEST] = hexMap.GetHex(this.Q - 1, this.R + 1);

        List<Hex> neighbors = new List<Hex>();
        for(int i = 0; i < hexes.Length; i++) {
            if(hexes[i] != null) {
                neighbors.Add(hexes[i]);
            }
        }

        return neighbors.ToArray();
    }

    public Hex GetNeighbor(HexMap.DIRECTION dir) {
        switch(dir) {
            case HexMap.DIRECTION.NORTHEAST:
                return hexMap.GetHex(this.Q, this.R + 1);
            case HexMap.DIRECTION.EAST:
                return hexMap.GetHex(this.Q + 1, this.R);
            case HexMap.DIRECTION.SOUTHEAST:
                return hexMap.GetHex(this.Q + 1, this.R - 1);
            case HexMap.DIRECTION.SOUTHWEST:
                return hexMap.GetHex(this.Q, this.R - 1);
            case HexMap.DIRECTION.WEST:
                return hexMap.GetHex(this.Q - 1, this.R);
            case HexMap.DIRECTION.NORTHWEST:
                return hexMap.GetHex(this.Q - 1, this.R + 1);
        }
        return null;
    }

    public string CoordsString() {
        return ("(" + this.Q + "," + this.R + ")");
    }

    public void SetTerrainType(TERRAIN_TYPE newterrain) {
        this.terrainType = newterrain;
        UpdateMovementCost();
    }

    public void SetElevationType(ELEVATION_TYPE newelev) {
        this.elevationType = newelev;
        UpdateMovementCost();
    }

    public void SetFeature(FEATURE_TYPE newfeature) {
        this.featureType = newfeature;
        UpdateMovementCost();
    }

    public TERRAIN_TYPE GetTerrainType () {
        return terrainType;
    }

    public ELEVATION_TYPE GetElevationType () {
        return elevationType;
    }

    public FEATURE_TYPE GetFeatureType() {
        return featureType;
    }

    public Zone.ZONE_TYPE GetZoneType() {
        if (zone == null) {
            return Zone.ZONE_TYPE.NONE;
        }
        return zone.GetZoneType();
    }

    public Zone GetZone() {
        return zone;
    }

    public void SetZone(Zone newZone) {
        zone = newZone;
    }

    //updates this tile's movement cost based on its type and features
    //shouldn't need to ever call this from outside this class since setters should call it
    public void UpdateMovementCost() {
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
