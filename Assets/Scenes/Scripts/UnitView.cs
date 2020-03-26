using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour {
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
        this.transform.position = Vector3.SmoothDamp(this.transform.position, newPos, ref currentVelocity, smoothTime);
    }
}
