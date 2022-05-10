using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Zone : MapObject {
    // Static zone stuff
    public enum ZONE_TYPE { CITY_CENTER, AGRICULTURE, FORESTRY, MINING, TOWN, MILITARY, HARBOR, NONE}

    // Class members
    protected ZONE_TYPE zoneType;
    protected int population;
    protected float upkeep;
    protected int developmentLevel;

    protected float[] resourceOutputs = {0, 0, 0, 0}; // indexed by RESOURCE_TYPE

    //TODO: project implementation
    // protected Project currentProject;
    // protected Project[] availableProjects;

    public Zone() {
        population = 1;
    }

    public Zone(Hex newHex) {
        this.hex = newHex;
        population = 1;
        UpdateResourceOutputs();
    }

    public ZONE_TYPE GetZoneType() {
        return zoneType;
    }

    public int GetPopulation() {
        return population;
    }

    public void SetPopulation(int newPop) {
        population = newPop;
    }

    public int GetDevLevel() {
        return developmentLevel;
    }

    public void SetDevLevel(int newDevLevel) {
        developmentLevel = newDevLevel;
    }

    public float GetUpkeep() {
        return upkeep;
    }

    public void SetUpkeep(float newUpkeep) {
        upkeep = newUpkeep;
    }

    public float[] GetResourceOutputs() {
        return resourceOutputs;
    }

    public void SetResourceOutput(Resource.RESOURCE_TYPE type, float value) {
        resourceOutputs[(int) type] = value;
    }

    public bool IsValidHex() {
        return IsValidHex(this.hex);
    }

    public abstract int[] GetPopCapByDevLevel();

    public abstract bool IsValidHex(Hex newHex);

    protected bool IsValidHex(Hex newHex,
                              Hex.TERRAIN_TYPE[] allowedTerrainTypes,
                              Hex.ELEVATION_TYPE[] allowedElevationTypes,
                              Hex.FEATURE_TYPE[] allowedFeatureTypes) {
        Debug.Log("hex: " + hex);
        return (Array.IndexOf(allowedTerrainTypes, newHex.GetTerrainType()) != -1) &&
               (Array.IndexOf(allowedElevationTypes, newHex.GetElevationType()) != -1) &&
               (Array.IndexOf(allowedFeatureTypes, newHex.GetFeatureType()) != -1);
    }

    public abstract void UpdateResourceOutputs();

    protected void UpdateResourceOutputs(float[] BASE_OUTPUT_PER_POP,
                                         float[] BASE_OUTPUT_TERRAIN_TYPE_MODIFIER,
                                         float[] BASE_OUTPUT_ELEVATION_TYPE_MODIFIER) {
        if (!IsValidHex()) {
            resourceOutputs = new float[] {0, 0, 0, 0};
            return;
        }

        for (int i = 0; i < resourceOutputs.Length; i++) {
            resourceOutputs[i] = BASE_OUTPUT_PER_POP[i] * 
                                 population * 
                                 BASE_OUTPUT_TERRAIN_TYPE_MODIFIER[(int) hex.GetTerrainType()] * 
                                 BASE_OUTPUT_ELEVATION_TYPE_MODIFIER[(int) hex.GetElevationType()];
        }
    }

    public abstract void UpdateUpkeep();

    protected void UpdateUpkeep(float BASE_UPKEEP_PER_POP,
                                float[] BASE_UPKEEP_BY_DEV_LEVEL) {
        upkeep = (BASE_UPKEEP_PER_POP * population) +
                 (BASE_UPKEEP_BY_DEV_LEVEL[developmentLevel]);
    }
}

public class CityCenterZone : Zone {
    // Development
    public static readonly int MAX_DEVELOPMENT_LEVEL = 3;
    public static readonly int[] POP_CAP_BY_DEV_LEVEL = {4, 7, 10};

    // Resource outputs
    public static readonly float[] BASE_OUTPUT_PER_POP = {1f, 1f, 2f, 1f}; // indexed by RESOURCE_TYPE
    public static readonly float[] BASE_OUTPUT_TERRAIN_TYPE_MODIFIER = {1f, 1f, 0.75f, -1f}; // indexed by TERRAIN_TYPE
    public static readonly float[] BASE_OUTPUT_ELEVATION_TYPE_MODIFIER = {1f, 1f, -1f, -1f}; // indexed by ELEVATION_TYPE

    // Upkeep
    public static readonly float BASE_UPKEEP_PER_POP = 0.5f;
    public static readonly float[] BASE_UPKEEP_BY_DEV_LEVEL = {1f, 4f, 10f};

    // Hex requirements
    public static readonly Hex.TERRAIN_TYPE[] allowedTerrainTypes = {Hex.TERRAIN_TYPE.PLAINS,
                                                                     Hex.TERRAIN_TYPE.GRASSLANDS,
                                                                     Hex.TERRAIN_TYPE.DESERT};
    public static readonly Hex.ELEVATION_TYPE[] allowedElevationTypes = {Hex.ELEVATION_TYPE.FLAT,
                                                                         Hex.ELEVATION_TYPE.HILL};
    public static readonly Hex.FEATURE_TYPE[] allowedFeatureTypes = {Hex.FEATURE_TYPE.NONE};

    public CityCenterZone() : base() {
        name = "City Center Zone";
        zoneType = ZONE_TYPE.CITY_CENTER;
    }

    public CityCenterZone(Hex hex) : base(hex) {
        name = "City Center Zone";
        zoneType = ZONE_TYPE.CITY_CENTER;
    }

    public override int[] GetPopCapByDevLevel() {
        return POP_CAP_BY_DEV_LEVEL;
    }

    public override bool IsValidHex(Hex newHex) {
        return IsValidHex(newHex, allowedTerrainTypes, allowedElevationTypes, allowedFeatureTypes);
    }

    public override void UpdateResourceOutputs() {
        UpdateResourceOutputs(BASE_OUTPUT_PER_POP,
                              BASE_OUTPUT_TERRAIN_TYPE_MODIFIER,
                              BASE_OUTPUT_ELEVATION_TYPE_MODIFIER);
    }

    public override void UpdateUpkeep() {
        UpdateUpkeep(BASE_UPKEEP_PER_POP, BASE_UPKEEP_BY_DEV_LEVEL);
    }
}

public class AgricultureZone : Zone {
    // Development
    public static readonly int MAX_DEVELOPMENT_LEVEL = 3;
    public static readonly int[] POP_CAP_BY_DEV_LEVEL = {3, 6, 9};

    // Resource outputs
    public static readonly float[] BASE_OUTPUT_PER_POP = {4f, 0f, 0f, 0f}; // indexed by RESOURCE_TYPE
    public static readonly float[] BASE_OUTPUT_TERRAIN_TYPE_MODIFIER = {1f, 1.5f, 0.5f, -1f}; // indexed by TERRAIN_TYPE
    public static readonly float[] BASE_OUTPUT_ELEVATION_TYPE_MODIFIER = {1f, .75f, -1f, -1f}; // indexed by ELEVATION_TYPE

    // Upkeep
    public static readonly float BASE_UPKEEP_PER_POP = 0.5f;
    public static readonly float[] BASE_UPKEEP_BY_DEV_LEVEL = {1f, 3f, 6f};

    // Hex requirements
    public static readonly Hex.TERRAIN_TYPE[] allowedTerrainTypes = {Hex.TERRAIN_TYPE.PLAINS,
                                                                     Hex.TERRAIN_TYPE.GRASSLANDS,
                                                                     Hex.TERRAIN_TYPE.DESERT};
    public static readonly Hex.ELEVATION_TYPE[] allowedElevationTypes = {Hex.ELEVATION_TYPE.FLAT,
                                                                         Hex.ELEVATION_TYPE.HILL};
    public static readonly Hex.FEATURE_TYPE[] allowedFeatureTypes = {Hex.FEATURE_TYPE.NONE};

    public AgricultureZone() : base() {
        name = "Agriculture Zone";
        zoneType = ZONE_TYPE.AGRICULTURE;
    }

    public AgricultureZone(Hex hex) : base(hex) {
        name = "Agriculture Zone";
        zoneType = ZONE_TYPE.AGRICULTURE;
    }

    public override int[] GetPopCapByDevLevel() {
        return POP_CAP_BY_DEV_LEVEL;
    }

    public override bool IsValidHex(Hex newHex) {
        return IsValidHex(newHex, allowedTerrainTypes, allowedElevationTypes, allowedFeatureTypes);
    }

    public override void UpdateResourceOutputs() {
        UpdateResourceOutputs(BASE_OUTPUT_PER_POP,
                              BASE_OUTPUT_TERRAIN_TYPE_MODIFIER,
                              BASE_OUTPUT_ELEVATION_TYPE_MODIFIER);
    }

    public override void UpdateUpkeep() {
        UpdateUpkeep(BASE_UPKEEP_PER_POP, BASE_UPKEEP_BY_DEV_LEVEL);
    }
}

public class ForestryZone : Zone {
    // Development
    public static readonly int MAX_DEVELOPMENT_LEVEL = 3;
    public static readonly int[] POP_CAP_BY_DEV_LEVEL = {2, 5, 8};

    // Resource outputs
    public static readonly float[] BASE_OUTPUT_PER_POP = {1f, 3f, 0f, 0f}; // indexed by RESOURCE_TYPE
    public static readonly float[] BASE_OUTPUT_TERRAIN_TYPE_MODIFIER = {0.5f, 1f, 0.1f, -1f}; // indexed by TERRAIN_TYPE
    public static readonly float[] BASE_OUTPUT_ELEVATION_TYPE_MODIFIER = {1f, .75f, -1f, -1f}; // indexed by ELEVATION_TYPE

    // Upkeep
    public static readonly float BASE_UPKEEP_PER_POP = 0.5f;
    public static readonly float[] BASE_UPKEEP_BY_DEV_LEVEL = {1f, 3f, 6f};

    // Hex requirements
    public static readonly Hex.TERRAIN_TYPE[] allowedTerrainTypes = {Hex.TERRAIN_TYPE.PLAINS,
                                                                     Hex.TERRAIN_TYPE.GRASSLANDS};
    public static readonly Hex.ELEVATION_TYPE[] allowedElevationTypes = {Hex.ELEVATION_TYPE.FLAT,
                                                                         Hex.ELEVATION_TYPE.HILL};
    public static readonly Hex.FEATURE_TYPE[] allowedFeatureTypes = {Hex.FEATURE_TYPE.FOREST,
                                                                     Hex.FEATURE_TYPE.RAINFOREST};

    public ForestryZone() : base() {
        name = "Forestry Zone";
        zoneType = ZONE_TYPE.FORESTRY;
    }

    public ForestryZone(Hex hex) : base(hex) {
        name = "Forestry Zone";
        zoneType = ZONE_TYPE.FORESTRY;
    }

    public override int[] GetPopCapByDevLevel() {
        return POP_CAP_BY_DEV_LEVEL;
    }

    public override bool IsValidHex(Hex newHex) {
        return IsValidHex(newHex, allowedTerrainTypes, allowedElevationTypes, allowedFeatureTypes);
    }

    public override void UpdateResourceOutputs() {
        UpdateResourceOutputs(BASE_OUTPUT_PER_POP,
                              BASE_OUTPUT_TERRAIN_TYPE_MODIFIER,
                              BASE_OUTPUT_ELEVATION_TYPE_MODIFIER);
    }

    public override void UpdateUpkeep() {
        UpdateUpkeep(BASE_UPKEEP_PER_POP, BASE_UPKEEP_BY_DEV_LEVEL);
    }
}

public class MiningZone : Zone {
    // Development
    public static readonly int MAX_DEVELOPMENT_LEVEL = 3;
    public static readonly int[] POP_CAP_BY_DEV_LEVEL = {2, 5, 8};

    // Resource outputs
    public static readonly float[] BASE_OUTPUT_PER_POP = {0f, 3f, 1f, 0f}; // indexed by RESOURCE_TYPE
    public static readonly float[] BASE_OUTPUT_TERRAIN_TYPE_MODIFIER = {1f, 1f, 1f, -1f}; // indexed by TERRAIN_TYPE
    public static readonly float[] BASE_OUTPUT_ELEVATION_TYPE_MODIFIER = {1f, 1.25f, -1f, -1f}; // indexed by ELEVATION_TYPE

    // Upkeep
    public static readonly float BASE_UPKEEP_PER_POP = 0.5f;
    public static readonly float[] BASE_UPKEEP_BY_DEV_LEVEL = {1f, 3f, 6f};

    // Hex requirements
    public static readonly Hex.TERRAIN_TYPE[] allowedTerrainTypes = {Hex.TERRAIN_TYPE.PLAINS,
                                                                     Hex.TERRAIN_TYPE.GRASSLANDS,
                                                                     Hex.TERRAIN_TYPE.DESERT};
    public static readonly Hex.ELEVATION_TYPE[] allowedElevationTypes = {Hex.ELEVATION_TYPE.FLAT,
                                                                         Hex.ELEVATION_TYPE.HILL};
    public static readonly Hex.FEATURE_TYPE[] allowedFeatureTypes = {Hex.FEATURE_TYPE.NONE};

    public MiningZone() : base() {
        name = "Mining Zone";
        zoneType = ZONE_TYPE.MINING;
    }

    public MiningZone(Hex hex) : base(hex) {
        name = "Mining Zone";
        zoneType = ZONE_TYPE.MINING;
    }

    public override int[] GetPopCapByDevLevel() {
        return POP_CAP_BY_DEV_LEVEL;
    }

    public override bool IsValidHex(Hex newHex) {
        return IsValidHex(newHex, allowedTerrainTypes, allowedElevationTypes, allowedFeatureTypes);
    }

    public override void UpdateResourceOutputs() {
        UpdateResourceOutputs(BASE_OUTPUT_PER_POP,
                              BASE_OUTPUT_TERRAIN_TYPE_MODIFIER,
                              BASE_OUTPUT_ELEVATION_TYPE_MODIFIER);
    }

    public override void UpdateUpkeep() {
        UpdateUpkeep(BASE_UPKEEP_PER_POP, BASE_UPKEEP_BY_DEV_LEVEL);
    }
}

public class TownZone : Zone {
    // Development
    public static readonly int MAX_DEVELOPMENT_LEVEL = 3;
    public static readonly int[] POP_CAP_BY_DEV_LEVEL = {4, 7, 10};

    // Resource outputs
    public static readonly float[] BASE_OUTPUT_PER_POP = {1f, 1f, 2f, 1f}; // indexed by RESOURCE_TYPE
    public static readonly float[] BASE_OUTPUT_TERRAIN_TYPE_MODIFIER = {1f, 1f, 0.75f, -1f}; // indexed by TERRAIN_TYPE
    public static readonly float[] BASE_OUTPUT_ELEVATION_TYPE_MODIFIER = {1f, 1f, -1f, -1f}; // indexed by ELEVATION_TYPE

    // Upkeep
    public static readonly float BASE_UPKEEP_PER_POP = 0.5f;
    public static readonly float[] BASE_UPKEEP_BY_DEV_LEVEL = {1f, 4f, 10f};

    // Hex requirements
    public static readonly Hex.TERRAIN_TYPE[] allowedTerrainTypes = {Hex.TERRAIN_TYPE.PLAINS,
                                                                     Hex.TERRAIN_TYPE.GRASSLANDS,
                                                                     Hex.TERRAIN_TYPE.DESERT};
    public static readonly Hex.ELEVATION_TYPE[] allowedElevationTypes = {Hex.ELEVATION_TYPE.FLAT,
                                                                         Hex.ELEVATION_TYPE.HILL};
    public static readonly Hex.FEATURE_TYPE[] allowedFeatureTypes = {Hex.FEATURE_TYPE.NONE};

    public TownZone() : base() {
        name = "Town Zone";
        zoneType = ZONE_TYPE.TOWN;
    }

    public TownZone(Hex hex) : base(hex) {
        name = "Town Zone";
        zoneType = ZONE_TYPE.TOWN;
    }

    public override int[] GetPopCapByDevLevel() {
        return POP_CAP_BY_DEV_LEVEL;
    }

    public override bool IsValidHex(Hex newHex) {
        return IsValidHex(newHex, allowedTerrainTypes, allowedElevationTypes, allowedFeatureTypes);
    }

    public override void UpdateResourceOutputs() {
        UpdateResourceOutputs(BASE_OUTPUT_PER_POP,
                              BASE_OUTPUT_TERRAIN_TYPE_MODIFIER,
                              BASE_OUTPUT_ELEVATION_TYPE_MODIFIER);
    }

    public override void UpdateUpkeep() {
        UpdateUpkeep(BASE_UPKEEP_PER_POP, BASE_UPKEEP_BY_DEV_LEVEL);
    }
}

public class MilitaryZone : Zone {
    // Development
    public static readonly int MAX_DEVELOPMENT_LEVEL = 1;
    public static readonly int[] POP_CAP_BY_DEV_LEVEL = {1, 1, 1};

    // Resource outputs
    public static readonly float[] BASE_OUTPUT_PER_POP = {0f, 0f, 0f, 0f}; // indexed by RESOURCE_TYPE
    public static readonly float[] BASE_OUTPUT_TERRAIN_TYPE_MODIFIER = {0f, 0f, 0f, -1f}; // indexed by TERRAIN_TYPE
    public static readonly float[] BASE_OUTPUT_ELEVATION_TYPE_MODIFIER = {0f, 0f, -1f, -1f}; // indexed by ELEVATION_TYPE

    // Upkeep
    public static readonly float BASE_UPKEEP_PER_POP = 0.5f;
    public static readonly float[] BASE_UPKEEP_BY_DEV_LEVEL = {1f, 1f, 1f};

    // Hex requirements
    public static readonly Hex.TERRAIN_TYPE[] allowedTerrainTypes = {Hex.TERRAIN_TYPE.PLAINS,
                                                                     Hex.TERRAIN_TYPE.GRASSLANDS,
                                                                     Hex.TERRAIN_TYPE.DESERT};
    public static readonly Hex.ELEVATION_TYPE[] allowedElevationTypes = {Hex.ELEVATION_TYPE.FLAT,
                                                                         Hex.ELEVATION_TYPE.HILL};
    public static readonly Hex.FEATURE_TYPE[] allowedFeatureTypes = {Hex.FEATURE_TYPE.NONE};

    public MilitaryZone() : base() {
        name = "Military Zone";
        zoneType = ZONE_TYPE.MILITARY;
    }
    
    public MilitaryZone(Hex hex) : base(hex) {
        name = "Military Zone";
        zoneType = ZONE_TYPE.MILITARY;
    }

    public override int[] GetPopCapByDevLevel() {
        return POP_CAP_BY_DEV_LEVEL;
    }

    public override bool IsValidHex(Hex newHex) {
        return IsValidHex(newHex, allowedTerrainTypes, allowedElevationTypes, allowedFeatureTypes);
    }

    public override void UpdateResourceOutputs() {
        UpdateResourceOutputs(BASE_OUTPUT_PER_POP,
                              BASE_OUTPUT_TERRAIN_TYPE_MODIFIER,
                              BASE_OUTPUT_ELEVATION_TYPE_MODIFIER);
    }

    public override void UpdateUpkeep() {
        UpdateUpkeep(BASE_UPKEEP_PER_POP, BASE_UPKEEP_BY_DEV_LEVEL);
    }
}

public class HarborZone : Zone {
    // Development
    public static readonly int MAX_DEVELOPMENT_LEVEL = 3;
    public static readonly int[] POP_CAP_BY_DEV_LEVEL = {2, 5, 8};

    // Resource outputs
    public static readonly float[] BASE_OUTPUT_PER_POP = {3f, 0f, 2f, 0f}; // indexed by RESOURCE_TYPE
    public static readonly float[] BASE_OUTPUT_TERRAIN_TYPE_MODIFIER = {-1f, -1f, -1f, 1f}; // indexed by TERRAIN_TYPE
    public static readonly float[] BASE_OUTPUT_ELEVATION_TYPE_MODIFIER = {-1f, -1f, -1f, 1f}; // indexed by ELEVATION_TYPE

    // Upkeep
    public static readonly float BASE_UPKEEP_PER_POP = 0.5f;
    public static readonly float[] BASE_UPKEEP_BY_DEV_LEVEL = {1f, 3f, 6f};

    // Hex requirements
    public static readonly Hex.TERRAIN_TYPE[] allowedTerrainTypes = {Hex.TERRAIN_TYPE.WATER};
    public static readonly Hex.ELEVATION_TYPE[] allowedElevationTypes = {Hex.ELEVATION_TYPE.WATER};
    public static readonly Hex.FEATURE_TYPE[] allowedFeatureTypes = {Hex.FEATURE_TYPE.NONE};

    public HarborZone() : base() {
        name = "Harbor Zone";
        zoneType = ZONE_TYPE.HARBOR;
    }
    public HarborZone(Hex hex) : base(hex) {
        name = "Harbor Zone";
        zoneType = ZONE_TYPE.HARBOR;
    }

    public override int[] GetPopCapByDevLevel() {
        return POP_CAP_BY_DEV_LEVEL;
    }

    public override bool IsValidHex(Hex newHex) {
        return IsValidHex(newHex, allowedTerrainTypes, allowedElevationTypes, allowedFeatureTypes);
    }

    public override void UpdateResourceOutputs() {
        UpdateResourceOutputs(BASE_OUTPUT_PER_POP,
                              BASE_OUTPUT_TERRAIN_TYPE_MODIFIER,
                              BASE_OUTPUT_ELEVATION_TYPE_MODIFIER);
    }

    public override void UpdateUpkeep() {
        UpdateUpkeep(BASE_UPKEEP_PER_POP, BASE_UPKEEP_BY_DEV_LEVEL);
    }
}