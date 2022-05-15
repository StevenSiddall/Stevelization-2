using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//TODO: split this into turn controller stuff and mid-turn stuff if it gets too crowded
public class ActionController : MonoBehaviour
{
    public bool animationIsPlaying = false;

    private bool zonePlacementMode = false;
    private Zone ghostZone;
    private GameObject ghostZonePrefab;
    private City ghostZoneCity;

    HexMap hexMap;
    Unit selectedUnit;
    City selectedCity;
    Zone selectedZone;

    UIController uiController;
    MouseController mouseController;


    void Start() {
        hexMap = FindObjectOfType<HexMap>();
        uiController = FindObjectOfType<UIController>();
        mouseController = FindObjectOfType<MouseController>();
    }

    void Update() {
        if(zonePlacementMode) {
            Debug.Log("Display ghost zone");
            uiController.DisplayGhostZone(ghostZone, ghostZoneCity);
        }
    }

    public void NextTurn() {
        if (animationIsPlaying) {
            return;
        }

        HashSet<Unit> units = hexMap.GetUnits();
        MoveUnits(units);
        ReplenishUnitMovement(units);
        uiController.UpdateSelection(selectedUnit, selectedCity, selectedZone);
    }

    public void ReplenishUnitMovement(HashSet<Unit> units) {
        foreach(Unit u in units) {
            u.movementRemaining = u.movement;
        }
    }

    public void MoveUnits(HashSet<Unit> units) {
        if (units != null) {
            foreach (Unit u in units) {
                StartCoroutine(DoUnitMovement(u));
            }
        }
    }

    public IEnumerator DoUnitMovement(Unit unit) {
        if (animationIsPlaying) {
            yield break;
        }

        while (unit.CanMoveAgain()) {
            animationIsPlaying = true;
            unit.Move();
            while (animationIsPlaying) {
                yield return null; //wait for animation to get close to finishing
            }
            unit.movementRemaining -= unit.hex.GetMovementCost();
            uiController.UpdateSelection(selectedUnit, selectedCity, selectedZone);
            animationIsPlaying = false;
        }
    }

    public void SelectUnit(Unit unit) {
        selectedUnit = unit;
        selectedCity = null;
        selectedZone = null;
        uiController.UpdateSelection(selectedUnit, selectedCity, selectedZone);
    }

    public void SelectCity(City city) {
        selectedCity = city;
        selectedUnit = null;
        selectedZone = city.GetZones(Zone.ZONE_TYPE.CITY_CENTER)[0];
        uiController.UpdateSelection(selectedUnit, selectedCity, selectedZone);
    }

    public void SelectZone(Zone zone) {
        selectedZone = zone;
        selectedCity = null;
        selectedUnit = null;
        uiController.UpdateSelection(selectedUnit, selectedCity, selectedZone);
    }

    public void Select(Hex hexUnderMouse) {

        if (hexUnderMouse == null) {
            Debug.Log("didn't find hex to select");
            return;
        }

        Debug.Log("Hex selected");

        if(zonePlacementMode) {
            if(IsValidZonePlacement(ghostZone, hexUnderMouse)) {
                BuildZone(hexUnderMouse.Q, hexUnderMouse.R);
            }

            ExitZonePlacementMode();
            return;
        }

        Unit[] units = hexUnderMouse.GetUnitArray();
        if (units == null || units.Length == 0) {
            //no unit present -- deselect
            SelectUnit(null);
            print("No units present");
            return;
        }

        //check if we already have a unit selected
        if(selectedUnit == null || selectedUnit.hex != hexUnderMouse) {
            //no unit selected -- get the first one
            SelectUnit(units[0]);
        } else {
            //loop through the units on the hex and select the next one
            bool foundUnit = false;
            for (int i = 0; i < units.Length; i++) {
                if (units[i] == selectedUnit) {
                    SelectUnit(units[(i + 1) % units.Length]);
                    foundUnit = true;
                }
            }

            //make sure we found the unit we have selected
            if (!foundUnit) {
                Debug.LogError("Couldn't find currently selected unit. selecting top unit");
                SelectUnit(units[0]);
            }
        }
    }

    public void RenameCity(City city, string newName) {
        if(city != null) {
            city.name = newName;
            uiController.UpdateSelection(this.selectedUnit, this.selectedCity, this.selectedZone);
        }
    }

    public Unit GetSelectedUnit() {
        return selectedUnit;
    }

    public City GetSelectedCity() {
        return selectedCity;
    }

    public void BuildCity() {
        if (animationIsPlaying) {
            return;
        }

        City city = new City();
        hexMap.SpawnCityAt(city, hexMap.zonePrefabs[(int) Zone.ZONE_TYPE.CITY_CENTER], selectedUnit.hex.Q, selectedUnit.hex.R);
        
        Hex cityCenterHex = hexMap.GetHex(selectedUnit.hex.Q, selectedUnit.hex.R);
        Zone cityCenterZone = new CityCenterZone(cityCenterHex);
        hexMap.SpawnZoneAt(cityCenterZone,
                            city,
                            null,
                            selectedUnit.hex.Q,
                            selectedUnit.hex.R);
        city.AddZone(cityCenterZone);
        
        uiController.UpdateSelection(selectedUnit, selectedCity, selectedZone);
    }

    public void BuildZone(int q, int r) {
        Hex zoneHex = hexMap.GetHex(q, r);

        if(!ghostZone.SetHex(zoneHex) || !selectedCity.AddZone(ghostZone)) {
            return;
        }

        ghostZone.SetHex(zoneHex);

        hexMap.SpawnZoneAt(ghostZone, selectedCity, hexMap.zonePrefabs[(int) ghostZone.GetZoneType()], q, r);
    }

    public void EnterZonePlacementMode(Zone.ZONE_TYPE type) {
        zonePlacementMode = true;
        switch(type) {
            case Zone.ZONE_TYPE.AGRICULTURE:
                ghostZone = new AgricultureZone();
                break;
            case Zone.ZONE_TYPE.FORESTRY:
                ghostZone = new ForestryZone();
                break;
            case Zone.ZONE_TYPE.MINING:
                ghostZone = new MiningZone();
                break;
            case Zone.ZONE_TYPE.TOWN:
                ghostZone = new TownZone();
                break;
            case Zone.ZONE_TYPE.MILITARY:
                ghostZone = new MilitaryZone();
                break;
            case Zone.ZONE_TYPE.HARBOR:
                ghostZone = new HarborZone();
                break;
            default: // Unsupported zone type
                zonePlacementMode = false;
                Debug.Log("EnterZonePlacementMode failed: Invalid zone type: " + type);
                return;
        }

        ghostZonePrefab = hexMap.zonePrefabs[(int) type];
        ghostZoneCity = selectedCity;
        uiController.SetGhostZoneType(ghostZonePrefab, ghostZoneCity);
    }

    public void ExitZonePlacementMode() {
        zonePlacementMode = false;
        ghostZone = null;
        ghostZoneCity = null;
        ghostZonePrefab = null;
        uiController.SetGhostZoneType(null, null);
    }

    public bool IsValidZonePlacement(Zone zone, Hex hex) {
        return ghostZone.IsValidHex(hex) &&
                hex.GetZone() == null &&
                Array.Exists(selectedCity.GetHexes(), h => h == hex);
    }
}
