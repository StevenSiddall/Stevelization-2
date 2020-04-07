using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionBarBehavior : MonoBehaviour
{
    public GameObject buildCityButton; //has to be a game object so we can use SetActive()
         
    public void updateSelection(Unit unit) {
        buildCityButton.SetActive(unit.hex.city == null && unit.canBuildCities);
    }
}
