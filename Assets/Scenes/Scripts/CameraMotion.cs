using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour
{
    Vector3 oldPos;

    // Start is called before the first frame update
    void Start() {
        oldPos = this.transform.position;
    }

    // Update is called once per frame
    void Update() {
        CheckIfCameraMoved();
    }

    public void PanToHex(Hex hex) {
        //TODO: move camera to hex
    }

    public void CheckIfCameraMoved() {
        if(oldPos != this.transform.position) {
            oldPos = this.transform.position;

            HexBehavior[] hexes = GameObject.FindObjectsOfType<HexBehavior>();

            foreach(HexBehavior hex in hexes) {
                hex.UpdatePosition();
            }
        }
    }
}
