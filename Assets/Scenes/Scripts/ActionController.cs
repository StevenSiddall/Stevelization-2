using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: split this into turn controller stuff and mid-turn stuff if it gets too crowded
public class ActionController : MonoBehaviour
{
    public bool animationIsPlaying = false;

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
        selectedZone = null;
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
        hexMap.SpawnCityAt(city, hexMap.cityPrefab, selectedUnit.hex.Q, selectedUnit.hex.R);
        uiController.UpdateSelection(selectedUnit, selectedCity, selectedZone);

        hexMap.SpawnZoneAt(new TownZone(hexMap.GetHex(selectedUnit.hex.Q + 1, selectedUnit.hex.R)), city, hexMap.militaryPrefab, selectedUnit.hex.Q + 1, selectedUnit.hex.R);
    }
}
