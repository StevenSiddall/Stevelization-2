using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityInfoPanelBehavior : MonoBehaviour
{
    public Text cityName; //should be set in editor


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void updateSelection(City city) {
        if(city != null) {
            cityName.text = city.name;
        }
    }
}
