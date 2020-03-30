using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour {

    HexMap hexMap;
    private Hex selectedHex;

    // Start is called before the first frame update
    void Start()
    {
        hexMap = GameObject.FindObjectOfType<HexMap>();
        selectedHex = null;
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    public void selectHex(Hex newHex) {
        if(newHex == null) {
            Debug.Log("didn't find hex to select");
            return;
        }
        //TODO: update UI elements
        Debug.Log("Hex selected");
        selectedHex = newHex;
    }

    //called when user is holding RMB to move a unit but hasn't released yet
    //want to show the path the unit will take to that hex
    public void showPathTo(Hex toHex) {

    }

    //called when user realeases RMB to move the selected unit to toHex
    public void moveOrder(Hex toHex) {
        if(toHex == null) {
            Debug.Log("Can't give move order -- no unit selected");
            return;
        }

        Debug.Log("Giving move order");

        Unit[] units = selectedHex.getUnitArray();
        if(units == null || units.Length == 0) {
            //no unit selected -- do nothing
            return;
        }

        //make sure we found a path
        Hex[] path = HexPathfinder.getPath(selectedHex, toHex);
        if (path == null) {
            Debug.Log("Couldn't find path to hex");
            return;
        }

        //have a unit there -- give move order
        units[0].setHexPath(path);

        //TODO? call doTurn() if we want the unit to move now, or don't
        //if we want it to move at the end of the turn
    }
}
