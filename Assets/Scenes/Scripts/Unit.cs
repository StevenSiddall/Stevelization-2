﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit {

    public Hex hex { get; protected set; }

    public string name = "No name";
    public int hitPoints = 100;
    public int strength = 8;
    public int movement = 2;
    public int movementRemaining = 2;

    Queue<Hex> hexPath;

    public delegate void UnitMovedDelegate(Hex oldHex, Hex newHex);

    public event UnitMovedDelegate onUnitMoved;

    public void setHex(Hex newHex) {
        Hex oldHex = this.hex;
        if(this.hex != null) {
            this.hex.removeUnit(this);
        }

        this.hex = newHex;
        this.hex.addUnit(this);

        if(onUnitMoved != null) {
            onUnitMoved(oldHex, newHex);
        }
    }

    public void setHexPath(Hex[] hexPath) {
        this.hexPath = new Queue<Hex>(hexPath);
    }

    public void doTurn() {
        //pop first hex from queue (if present)
        if(hexPath == null || hexPath.Count == 0) {
            return;
        }

        Hex newHex = hexPath.Dequeue();
        setHex(newHex);
    }

    public float movementCostToEnterHex(Hex hex) {
        //TODO: factor in movement mode + terrain type
        return hex.movementCost();
    }

    // turn cost != movement cost
    public float aggregateTurnsToEnterHex(Hex hex, float turnsToDate) {
        float baseTurnsToEnterHex = movementCostToEnterHex(hex) / movement; //ex. entering forest is "1" turn

        float turnsRemaining = movementRemaining / movement; //ex. 1/2 move pts => .5 turns left

        float turnsToDateWhole = Mathf.Floor(turnsToDate);
        float turnsToDateRem = turnsToDate - turnsToDateWhole;

        //detect floating point drift
        if(turnsToDateRem < .01f || turnsToDateRem > .99f) {
            Debug.LogError("Floating point drift occurring");
            //correct drift
            if(turnsToDateRem < .01f) {
                turnsToDateRem = 0;
            }else {
                turnsToDateRem = 0;
                turnsToDateWhole++;
            }
        }

        float turnsUsedAfterThisMove = turnsToDateRem + baseTurnsToEnterHex;

        if(turnsUsedAfterThisMove > 1) {
            // dont have enough movement
            // can't enter tile this move

            if(turnsToDateRem == 0) {
                // have full movement but cant enter tile
            }else {
                // not on fresh turn -- need to wait for this turn
                turnsToDateWhole++;
                turnsToDateRem = 0;
            }

            // we're starting the move into difficult terrain on a fresh turn
            turnsUsedAfterThisMove = baseTurnsToEnterHex;
        }

        //turnsusedafterthismvoe is now [0,1]
        return turnsToDateWhole + turnsUsedAfterThisMove;
    }
}