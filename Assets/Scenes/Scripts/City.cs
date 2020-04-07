using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MapObject {

    public City() {
        name = "a city";
        hitPoints = 100f;
    }

    public override void setHex(Hex newHex) {
        if(hex != null) {
            hex.removeCity();
        }

        base.setHex(newHex);

        hex.addCity(this);
    }
}