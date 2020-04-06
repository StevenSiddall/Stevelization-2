﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: split this into turn controller stuff and mid-turn stuff if it gets too crowded
public class ActionController : MonoBehaviour
{
    public bool animationIsPlaying = false;

    HexMap hexMap;
    UIController uiController;
    MouseController mouseController;


    void Start() {
        hexMap = FindObjectOfType<HexMap>();
        uiController = FindObjectOfType<UIController>();
        mouseController = FindObjectOfType<MouseController>();
    }

    public void nextTurn() {
        HashSet<Unit> units = hexMap.getUnits();
        moveUnits(units);
        replenishUnitMovement(units);
        uiController.updateSelection(mouseController.getSelectedUnit());
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
        while (unit.canMoveAgain()) {
            animationIsPlaying = true;
            unit.move();
            while (animationIsPlaying) {
                yield return null; //wait for animation to get close to finishing
            }
            unit.movementRemaining -= unit.hex.getMovementCost();
            uiController.updateSelection(mouseController.getSelectedUnit());
            animationIsPlaying = false;
        }

        
    }
}
