using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour {
    public static readonly int MAX_SMOOTHMOVE_DIST = 12; //units moving to a new position greater than this number away will teleport instead of move smoothly

    Vector3 oldPos;
    Vector3 newPos;

    Vector3 currentVelocity;
    float smoothTime = 0.5f;

    public void onUnitMoved(Hex oldHex, Hex newHex) {
        oldPos = oldHex.positionFromCamera();
        newPos = newHex.positionFromCamera();
    }

    void Start() {
        oldPos = newPos = this.transform.position;
    }

    void Update() {
        if(Vector3.Distance(newPos, this.transform.position) <= MAX_SMOOTHMOVE_DIST) {
            this.transform.position = Vector3.SmoothDamp(this.transform.position, newPos, ref currentVelocity, smoothTime);
        }
    }
}
