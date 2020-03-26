using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit {
    public Hex hex { get; protected set; }

    public string name = "No name";
    public int hitPoints = 100;
    public int strength = 8;
    public int movement = 2;
    public int movementRemaining = 2;

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

    public void doTurn() {
        //TESTING: move unit to the right
        Hex oldHex = this.hex;
        Hex newHex = oldHex.hexMap.getHex(oldHex.Q + 1, oldHex.R);

        setHex(newHex);
    }
}
