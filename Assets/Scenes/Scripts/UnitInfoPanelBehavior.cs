using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoPanelBehavior : MonoBehaviour
{

    public Text title;
    public Text movement;

    UnitActionBarBehavior actionBarBehavior;

    void Start() {
        actionBarBehavior = GetComponentInChildren<UnitActionBarBehavior>();
    }

    public void UpdateSelection(Unit unit) {
        if (unit != null) {
            title.text = "Unit: " + unit.name;
            movement.text = string.Format("{0}/{1}", unit.movementRemaining, unit.movement);
            actionBarBehavior.UpdateSelection(unit);
        }
    }
}
