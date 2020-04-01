using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour {
    public static readonly int MAX_SMOOTHMOVE_DIST = 12; //units moving to a new position greater than this number away will teleport instead of move smoothly
    public static readonly float MAX_EXPECTED_VELO = 2.6f; //for normalizing animation weight
    public static readonly int RUNNING_ANIMLAYER = 1; //layer for the running animation

    Vector3 oldPos;
    Vector3 newPos;

    Vector3 currentVelocity;
    float smoothTime = 0.5f;

    float maxVelo = 0;

    //should only ever move a unit one tile at a time
    public void onUnitMoved(Hex oldHex, Hex newHex) {
        oldPos = oldHex.positionFromCamera();
        newPos = newHex.positionFromCamera();
        updateAngle();
    }

    void Start() {
        oldPos = newPos = this.transform.position;
    }

    void Update() {
        if(Vector3.Distance(newPos, this.transform.position) <= MAX_SMOOTHMOVE_DIST) {
            this.transform.position = Vector3.SmoothDamp(this.transform.position, newPos, ref currentVelocity, smoothTime);
            if(currentVelocity.magnitude > maxVelo) {
                maxVelo = currentVelocity.magnitude;
                Debug.Log("max:" + maxVelo);
            }
        }
        this.gameObject.GetComponentInChildren<Animator>().SetLayerWeight(RUNNING_ANIMLAYER, currentVelocity.magnitude / MAX_EXPECTED_VELO);
    }

    void updateAngle() {
        this.transform.forward = newPos - oldPos;
    }
}
