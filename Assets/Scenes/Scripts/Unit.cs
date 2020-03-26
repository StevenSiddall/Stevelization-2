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

    public void setHex(Hex otherhex) {
        if(this.hex != null) {
            this.hex.removeUnit(this);
        }

        this.hex = otherhex;
        this.hex.addUnit(this);
    }
}
