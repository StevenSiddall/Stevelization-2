using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MapObject {
    public UnitView unitView;

    public static readonly Vector3 FLATHEIGHT = Vector3.down * .3f;
    
    public float strength = 8f;
    public float movement = 2f;
    public float movementRemaining = 2f;

    public bool canBuildCities = false;

    Queue<Hex> hexPath;

    override public void SetHex(Hex newHex) {
        if (this.hex != null) {
            this.hex.RemoveUnit(this);
        }

        base.SetHex(newHex);

        this.hex.AddUnit(this);
    }

    public Hex[] GetHexPath() {
        if(hexPath == null) {
            return null;
        }
        return this.hexPath.ToArray();
    }

    public void SetHexPath(Hex[] hexPath) {
        if(hexPath == null) {
            return;
        }
        this.hexPath = new Queue<Hex>(hexPath);
    }

    public bool Move() {
        //pop first hex from queue (if present)
        if(hexPath == null || hexPath.Count == 0) {
            return false;
        }

        Hex newHex = hexPath.Dequeue();
        SetHex(newHex);
        return CanMoveAgain();
    }

    public bool CanMoveAgain() {
        return (movementRemaining > 0 && hexPath != null && hexPath.Count > 0 && movementRemaining >= hexPath.Peek().GetMovementCost());
    }

    public float GetUnitSpeed() {
        return unitView.currentVelocity.magnitude;
    }

    public float DistFromNextHex() {
        return Vector3.Distance(unitView.oldPos, unitView.newPos);
    }

    public float MovementCostToEnterHex(Hex hex) {
        return hex.GetMovementCost();
    }

    //TODO: revisit this: might be necessary for perfect pathfinding -- not used currently
    // turn cost != movement cost
    public float AggregateTurnsToEnterHex(Hex hex, float turnsToDate) {
        float baseTurnsToEnterHex = MovementCostToEnterHex(hex) / movement; //ex. entering forest is "1" turn

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
