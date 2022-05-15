using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CityInfoPanelBehavior : MonoBehaviour
{
    public Text cityName; //should be set in editor
    public InputField nameInput;

    public TextMeshProUGUI popText;
    public TextMeshProUGUI upkeepText;
    public TextMeshProUGUI defensiveStrengthText;

    public TextMeshProUGUI[] resourceText;

    public TextMeshProUGUI[] zoneText;

    public Button[] buildZoneButtons;

    ActionController actionController;


    // Start is called before the first frame update
    void Awake()
    {
        nameInput = cityName.gameObject.GetComponent<InputField>();
        actionController = FindObjectOfType<ActionController>();
        
        buildZoneButtons[(int)Zone.ZONE_TYPE.AGRICULTURE].GetComponent<Button>().onClick.AddListener(delegate{actionController.EnterZonePlacementMode(Zone.ZONE_TYPE.AGRICULTURE);});
        buildZoneButtons[(int)Zone.ZONE_TYPE.FORESTRY].GetComponent<Button>().onClick.AddListener(delegate{actionController.EnterZonePlacementMode(Zone.ZONE_TYPE.FORESTRY);});
        buildZoneButtons[(int)Zone.ZONE_TYPE.MINING].GetComponent<Button>().onClick.AddListener(delegate{actionController.EnterZonePlacementMode(Zone.ZONE_TYPE.MINING);});
        buildZoneButtons[(int)Zone.ZONE_TYPE.TOWN].GetComponent<Button>().onClick.AddListener(delegate{actionController.EnterZonePlacementMode(Zone.ZONE_TYPE.TOWN);});
        buildZoneButtons[(int)Zone.ZONE_TYPE.MILITARY].GetComponent<Button>().onClick.AddListener(delegate{actionController.EnterZonePlacementMode(Zone.ZONE_TYPE.MILITARY);});
        buildZoneButtons[(int)Zone.ZONE_TYPE.HARBOR].GetComponent<Button>().onClick.AddListener(delegate{actionController.EnterZonePlacementMode(Zone.ZONE_TYPE.HARBOR);});

        print("start called");
    }

    public void UpdateSelection(City city) {
        if(city == null) {
            return;
        }

        nameInput.text = city.name;
        
        popText.text = "Total Population: " + city.GetTotalPopulation() + "/" + city.GetTotalMaxPopulation();
        upkeepText.text = "Total Upkeep: " + SpriteText.RESOURCE_SPRITE[(int) Resource.RESOURCE_TYPE.MONEY] + " " + Math.Round(city.GetTotalUpkeep());
        defensiveStrengthText.text = "Defensive Strength: " + city.GetDefensiveStrength().ToString();

        float[] yields = city.GetTotalYields();
        for(int i = 0; i < resourceText.Length; i++) {
            resourceText[i].text = SpriteText.RESOURCE_SPRITE[i] + " " + Math.Round(yields[i]);
        }

        int[] zoneCounts = city.GetNumZones();
        int[] maxZoneCounts = city.GetMaxNumZones();
        // Skip i = 0 aka city center count -- not depicted on city UI
        for(int i = 1; i < zoneCounts.Length; i++) {
            zoneText[i].text = SpriteText.ZONE_SPRITE[i] + " " + zoneCounts[i] + "/" + maxZoneCounts[i];
        }
    }
}
