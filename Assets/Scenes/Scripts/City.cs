using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MapObject {
    // Default values
    public static readonly int BASE_MAX_AGRICULTURE_ZONES = 2;
    public static readonly int BASE_MAX_FORESTRY_ZONES = 2;
    public static readonly int BASE_MAX_MINING_ZONES = 2;
    public static readonly int BASE_MAX_TOWN_ZONES = 2;
    public static readonly int BASE_MAX_MILITARY_ZONES = 2;
    public static readonly int BASE_MAX_HARBOR_ZONES = 2;

    // Yields
    private float [] totalYields = {0, 0, 0, 0};

    // City Stats
    private int totalPopulation;

    private HashSet<Zone> zones;

    private int[] maxNumZones = {1,
                                BASE_MAX_AGRICULTURE_ZONES,
                                BASE_MAX_FORESTRY_ZONES,
                                BASE_MAX_MINING_ZONES,
                                BASE_MAX_TOWN_ZONES,
                                BASE_MAX_MILITARY_ZONES,
                                BASE_MAX_HARBOR_ZONES};

    // Combat
    private float hitPoints;
    private float fortification;

    // Territory
    private List<Hex> hexes;


    public City() {
        name = "a city";
        hitPoints = 100f;
        hexes = new List<Hex>();
        zones = new HashSet<Zone>();
    }

    public override void SetHex(Hex newHex) {
        if(hex != null) {
            hex.RemoveCity();
        }

        base.SetHex(newHex);
        hex.AddCity(this);

        AddHex(hex);
        Hex[] neighbors = hex.GetNeighbors();
        foreach(Hex h in neighbors) {
            AddHex(h);
        }
    }

    public void AddHex(Hex hex) {
        hexes.Add(hex);
    }

    public bool AddZone(Zone zone) {
        if(GetNumZones()[(int) zone.GetZoneType()] >= maxNumZones[(int) zone.GetZoneType()]) {
            return false;
        }

        zones.Add(zone);
        return true;
    }

    public Hex[] GetHexes() {
        return hexes.ToArray();
    }

    public int[] GetNumZones() {
        int[] zoneCounts = {0, 0, 0, 0, 0, 0, 0};
        foreach(Zone z in zones) {
            zoneCounts[(int) z.GetZoneType()]++;
        }
        return zoneCounts;
    }

    public int GetNumAgricultureZones() {
        return GetNumZones()[(int) Zone.ZONE_TYPE.AGRICULTURE];
    }

    public int GetNumForestryZones() {
        return GetNumZones()[(int) Zone.ZONE_TYPE.FORESTRY];
    }

    public int GetNumMiningZones() {
        return GetNumZones()[(int) Zone.ZONE_TYPE.MINING];
    }

    public int GetNumTownZones() {
        return GetNumZones()[(int) Zone.ZONE_TYPE.TOWN];
    }

    public int GetNumMilitaryZones() {
        return GetNumZones()[(int) Zone.ZONE_TYPE.MILITARY];
    }

    public int GetNumHarborZones() {
        return GetNumZones()[(int) Zone.ZONE_TYPE.HARBOR];
    }

    public int GetNumCityCenterZones() {
        return GetNumZones()[(int) Zone.ZONE_TYPE.CITY_CENTER];
    }

}