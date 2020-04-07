using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: split this into turn controller stuff and mid-turn stuff if it gets too crowded
public class ActionController : MonoBehaviour
{
    public bool animationIsPlaying = false;

    HexMap hexMap;
    Unit selectedUnit;

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
        uiController.updateSelection(selectedUnit);
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
            uiController.updateSelection(selectedUnit);
            animationIsPlaying = false;
        }
    }

    public void selectUnit(Unit unit) {
        selectedUnit = unit;
    }

    public Unit getSelectedUnit() {
        return selectedUnit;
    }

    public void buildCity() {
        if (animationIsPlaying) {
            return;
        }

        City city = new City();
        hexMap.spawnCityAt(city, hexMap.cityPrefab, selectedUnit.hex.Q, selectedUnit.hex.R);
        uiController.updateSelection(selectedUnit);
    }
}
