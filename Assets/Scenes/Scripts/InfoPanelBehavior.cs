using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelBehavior : MonoBehaviour
{

    public Text title;
    public Text movement;

    public GameObject buildCityButton;

    public void updateSelection(Unit unit) {
        if (unit != null) {
            title.text = "Unit: " + unit.name;
            movement.text = string.Format("{0}/{1}", unit.movementRemaining, unit.movement);
            buildCityButton.SetActive(unit.canBuildCities);
        }
    }
}
