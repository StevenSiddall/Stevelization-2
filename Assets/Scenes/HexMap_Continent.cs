using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap_Continent : HexMap
{
    public override void GenerateMap() {
        //generate base map i.e. water world
        base.GenerateMap();

        //generate raised area for land
        int numSplats = Random.Range(5, 8);
        for(int i = 0; i < numSplats; i++) {
            int range = Random.Range(3, 8);
            int y = Random.Range(range, numRows - range);
            int x = Random.Range(0, 10) - y/2 + 20;

            elevateArea(x, y, range);
        }

        updateHexVisuals();
    }

    public void elevateArea(int q, int r, int range, float centerHeight = 1f) {
        Hex centerHex = getHex(q, r);

        Hex[] areaHexes = getHexesWithinRange(centerHex, range);

        foreach(Hex h in areaHexes) {
            /*if(h.elevation < 0) {
                h.elevation = 0;
            }*/
            h.elevation += 0.5f * Mathf.Lerp(1f, 0.25f, Hex.distance(centerHex, h) / range);
        }
    }
}
