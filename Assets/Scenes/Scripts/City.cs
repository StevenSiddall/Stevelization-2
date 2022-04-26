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

    public override void SetHex(Hex newHex) {
        if(hex != null) {
            hex.RemoveCity();
        }

        base.SetHex(newHex);
        hex.AddCity(this);

        AddHex(hex);
        Hex[] neighbors = hex.GetNeighbors();
        foreach(Hex h in neighbors) {
            AddHex(h);
        }
    }

    public void AddHex(Hex hex) {
        hexes.Add(hex);
    }

    public Hex[] GetHexes() {
        return hexes.ToArray();
    }
}