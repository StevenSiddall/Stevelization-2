using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MapObject {

    //combat stuff
    private float hitPoints;

    //territory stuff
    private List<Hex> hexes;

    public City() {
        name = "a city";
        hitPoints = 100f;
        hexes = new List<Hex>();
    }

    public override void setHex(Hex newHex) {
        if(hex != null) {
            hex.removeCity();
        }

        base.setHex(newHex);
        hex.addCity(this);

        addHex(hex);
        Hex[] neighbors = hex.getNeighbors();
        foreach(Hex h in neighbors) {
            addHex(h);
        }
    }

    public void addHex(Hex hex) {
        hexes.Add(hex);
    }

    public Hex[] getHexes() {
        return hexes.ToArray();
    }
}