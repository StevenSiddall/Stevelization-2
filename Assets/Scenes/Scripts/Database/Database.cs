using System;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour {

    public const string BUILDINGS_PATH = "Buildings";

    private List<Building>[] buildings; //buildings in sublists by specialization
    private List<Building> allBuildings; //all buildings

    public void Start() {
        loadBuildings();

        //print("Buildings Loaded: ");
        //printBuildings();
    }

    public void printBuildings() {
        print("Total count: " + allBuildings.Count);
        for(int i = 0; i < buildings.Length; i++) {
            print(((Hex.SPECIALIZATION_TYPE)i).ToString());
            foreach(Building b in buildings[i]) {
                print("Name: " + b.name + ", Food: " + b.food + ", Raw Materials: " + b.rawMaterials);
            }
        }
    }

    private void loadBuildings() {
        allBuildings = BuildingList.load(BUILDINGS_PATH).buildings;
        buildings = new List<Building>[Enum.GetNames(typeof(Hex.SPECIALIZATION_TYPE)).Length];
        for(int i = 0; i < buildings.Length; i++) {
            buildings[i] = new List<Building>();
        }

        //organize buildings into sublists by specialization
        foreach(Building b in allBuildings) {
            switch (b.specialization) {
                case "Farmland":
                    buildings[(int)Hex.SPECIALIZATION_TYPE.FARMLAND].Add(b);
                    break;
                case "Mining":
                    buildings[(int)Hex.SPECIALIZATION_TYPE.MINING].Add(b);
                    break;
                case "Forestry":
                    buildings[(int)Hex.SPECIALIZATION_TYPE.FORESTRY].Add(b);
                    break;
                case "Town":
                    buildings[(int)Hex.SPECIALIZATION_TYPE.TOWN].Add(b);
                    break;
                case "Military":
                    buildings[(int)Hex.SPECIALIZATION_TYPE.MILITARY].Add(b);
                    break;
                default:
                    Debug.LogError("Couldn't find valid specialization for building: " +
                                    "\n Name: " + b.name + "\nSpecialization: " + b.specialization);
                    break;
            }

        }
    }
}
