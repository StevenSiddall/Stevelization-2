using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ZoneInfoPanelBehavior : MonoBehaviour
{
    public Text zoneName;
    public TextMeshProUGUI popText;
    public TextMeshProUGUI upkeepText;
    public TextMeshProUGUI devLevelText;
    public TextMeshProUGUI[] resourceText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void UpdateSelection(Zone zone)
    {
        zoneName.text = zone.name;
        popText.text = "Pop: " + zone.GetPopulation() + "/" + zone.GetPopCapByDevLevel()[zone.GetDevLevel()];
        upkeepText.text = "Upkeep: " + Math.Round(zone.GetUpkeep());
        devLevelText.text = "Dev. Level: " + zone.GetDevLevel();

        for(int i = 0; i < Enum.GetNames(typeof(Resource.RESOURCE_TYPE)).Length; i++) {
            resourceText[i].text = Math.Round(zone.GetResourceOutputs()[i]).ToString();
        }
    }
}
