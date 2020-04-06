using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap_Continent : HexMap
{
    public static readonly int N_SPLATS_MIN = 5; //minimum number of splats per continent
    public static readonly int N_SPLATS_MAX = 8; //maximum number of splats per continent
    public static readonly int SPLAT_RANGE_MIN = 8; //minimum size of each splat
    public static readonly int SPLAT_RANGE_MAX = 11; //maximum size of each splat

    public static readonly float ELEV_NOISE_RES = .05f; //resolution of noise map
    public static readonly float ELEV_NOISE_SCALE = .75f; //strength of noise: stronger noise = more small islands, snakier continents

    public static readonly float MOISTURE_NOISE_RES = .05f;
    public static readonly float MOISTURE_NOISE_SCALE = 1f;



    public override void GenerateMap() {
        //generate base map i.e. water world
        base.GenerateMap();

        int numContinents = 3;
        int continentSpacing = numCols / numContinents;

        //generate each continent
        Random.InitState(0);
        for(int c = 0; c < numContinents; c++) {
            //generate raised area for land
            int numSplats = Random.Range( N_SPLATS_MIN, N_SPLATS_MAX );
            for (int i = 0; i < numSplats; i++) {
                int range = Random.Range( SPLAT_RANGE_MIN, SPLAT_RANGE_MAX );
                int r = Random.Range(range, numRows - range);
                int q = Random.Range(0, 10) + (c * continentSpacing);

                elevateArea(q, r, range);
            }
        }

        //add elevation noise
        // perlin noise is pseudo-random, but deterministic i.e. sampling the same
        // coords repeatedly will yield the same values, but the plane is infinite,
        // so we can sample from a random "origin to get different noise ever time
        float xOrigin = Random.Range(0, 1000);
        float yOrigin = Random.Range(0, 1000);

        for(int col = 0; col < numCols; col++) {
            for(int row = 0; row < numRows; row++) {
                Hex h = getHex(col, row);
                float n = Mathf.PerlinNoise( ((float)col / (float) Mathf.Max(numCols, numRows) / ELEV_NOISE_RES) + xOrigin, 
                                             ((float)row / (float) Mathf.Max(numCols, numRows) / ELEV_NOISE_RES) + yOrigin);
                h.elevation += ((n * 2f) - 1f) * ELEV_NOISE_SCALE; //scale to [0,2] then shift to [-1,1] and scale
            }
        }


        //add moisture
        xOrigin = Random.Range(0, 1000);
        yOrigin = Random.Range(0, 1000);

        for (int col = 0; col < numCols; col++) {
            for (int row = 0; row < numRows; row++) {
                Hex h = getHex(col, row);
                float n = Mathf.PerlinNoise(((float)col / (float)Mathf.Max(numCols, numRows) / MOISTURE_NOISE_RES) + xOrigin,
                                             ((float)row / (float)Mathf.Max(numCols, numRows) / MOISTURE_NOISE_RES) + yOrigin);
                h.moisture = n * MOISTURE_NOISE_SCALE; //keep as [0,1] before scaling
            }
        }

        updateHexVisuals();

        Unit unit = new Unit();
        unit.canBuildCities = true;
        spawnUnitAt(unit, unitFootsoldierPrefab, 62, 6);

    }

    public void elevateArea(int q, int r, int range, float centerHeight = 1f) {
        Hex centerHex = getHex(q, r);

        Hex[] areaHexes = getHexesWithinRange(centerHex, range);

        foreach(Hex h in areaHexes) {
            h.elevation += .35f * Mathf.Lerp(1f, 0.25f, Mathf.Pow(Hex.distance(centerHex, h) / range, 1.5f));
        }
    }
}
