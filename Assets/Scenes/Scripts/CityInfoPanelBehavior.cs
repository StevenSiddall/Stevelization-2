using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityInfoPanelBehavior : MonoBehaviour
{
    public Text cityName; //should be set in editor
    public InputField nameInput;


    // Start is called before the first frame update
    void Awake()
    {
        nameInput = cityName.gameObject.GetComponent<InputField>();
        print("start called");
    }

    public void updateSelection(City city) {
        print("update selection called");
        if(city != null) {
            nameInput.text = city.name;
        }
    }
}
