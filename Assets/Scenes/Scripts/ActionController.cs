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

    UIController uiController;
    MouseController mouseController;


    void Start() {
        hexMap = FindObjectOfType<HexMap>();
        uiController = FindObjectOfType<UIController>();
        mouseController = FindObjectOfType<MouseController>();
    }

    public void nextTurn() {
        if (animationIsPlaying) {
            return;
        }

        HashSet<Unit> units = hexMap.getUnits();
        moveUnits(units);
        replenishUnitMovement(units);
        uiController.updateSelection(selectedUnit, selectedCity);
    }

    public void replenishUnitMovement(HashSet<Unit> units) {
        foreach(Unit u in units) {
            u.movementRemaining = u.movement;
        }
    }

    public void moveUnits(HashSet<Unit> units) {
        if (units != null) {
            foreach (Unit u in units) {
                StartCoroutine(doUnitMovement(u));
            }
        }
    }

    public IEnumerator doUnitMovement(Unit unit) {
        if (animationIsPlaying) {
            yield break;
        }

        while (unit.canMoveAgain()) {
            animationIsPlaying = true;
            unit.move();
            while (animationIsPlaying) {
                yield return null; //wait for animation to get close to finishing
            }
            unit.movementRemaining -= unit.hex.getMovementCost();
            uiController.updateSelection(selectedUnit, selectedCity);
            animationIsPlaying = false;
        }
    }

    public void selectUnit(Unit unit) {
        selectedUnit = unit;
        selectedCity = null;
        uiController.updateSelection(selectedUnit, selectedCity);
    }

    public void selectCity(City city) {
        selectedCity = city;
        selectedUnit = null;
        uiController.updateSelection(selectedUnit, selectedCity);
    }

    public void select(Hex hexUnderMouse) {

        if (hexUnderMouse == null) {
            Debug.Log("didn't find hex to select");
            return;
        }

        Debug.Log("Hex selected");
        Unit[] units = hexUnderMouse.getUnitArray();
        if (units == null || units.Length == 0) {
            //no unit present -- deselect
            selectUnit(null);
            print("No units present");
            return;
        }

        //check if we already have a unit selected
        if(selectedUnit == null || selectedUnit.hex != hexUnderMouse) {
            //no unit selected -- get the first one
            selectUnit(units[0]);
        } else {
            //loop through the units on the hex and select the next one
            bool foundUnit = false;
            for (int i = 0; i < units.Length; i++) {
                if (units[i] == selectedUnit) {
                    selectUnit(units[(i + 1) % units.Length]);
                    foundUnit = true;
                }
            }

            //make sure we found the unit we have selected
            if (!foundUnit) {
                Debug.LogError("Couldn't find currently selected unit. selecting top unit");
                selectUnit(units[0]);
            }
        }
    }

    public void renameCity(City city, string newName) {
        if(city != null) {
            city.name = newName;
            uiController.updateSelection(this.selectedUnit, this.selectedCity);
        }
    }

    public Unit getSelectedUnit() {
        return selectedUnit;
    }

    public City getSelectedCity() {
        return selectedCity;
    }

    public void buildCity() {
        if (animationIsPlaying) {
            return;
        }

        City city = new City();
        hexMap.spawnCityAt(city, hexMap.cityPrefab, selectedUnit.hex.Q, selectedUnit.hex.R);
        uiController.updateSelection(selectedUnit, selectedCity);
    }
}
