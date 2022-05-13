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

    public void UpdateSelection(Zone zone)
    {
        if(zone == null) {
            return;
        }

        zoneName.text = zone.name;
        popText.text = "Pop: " + zone.GetPopulation() + "/" + zone.GetPopCap();
        upkeepText.text = "Upkeep: " + SpriteText.RESOURCE_SPRITE[(int) Resource.RESOURCE_TYPE.MONEY] + " " + Math.Round(zone.GetUpkeep());
        devLevelText.text = "Dev. Level: " + zone.GetDevLevel();

        float[] outputs = zone.GetResourceOutputs();
        for(int i = 0; i < outputs.Length; i++) {
            resourceText[i].text = SpriteText.RESOURCE_SPRITE[i] + " " + Math.Round(outputs[i]).ToString();
        }
    }
}
