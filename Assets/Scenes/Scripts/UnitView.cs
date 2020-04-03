using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour {
    public static readonly int MAX_SMOOTHMOVE_DIST = 12; //units moving to a new position greater than this number away will teleport instead of move smoothly
    public static readonly float MAX_EXPECTED_VELO = 2.6f; //for normalizing animation weight
    public static readonly int RUNNING_ANIMLAYER = 1; //layer for the running animation
    public static readonly float MAX_ANIM_WAIT_DIST = 0.2f; //distance from next tile above which move animations halt execution

    TurnController turnController;

    public Vector3 oldPos;
    public Vector3 newPos;

    public Vector3 currentVelocity;
    float smoothTime = 0.5f;

    //should only ever move a unit one tile at a time
    public void onUnitMoved(Hex oldHex, Hex newHex) {
        oldPos = oldHex.positionFromCamera() + Unit.FLATHEIGHT;
        newPos = newHex.positionFromCamera() + Unit.FLATHEIGHT;
        updateAngle();
    }

    void Start() {
        oldPos = newPos = this.transform.position + Unit.FLATHEIGHT;
        turnController = FindObjectOfType<TurnController>();
    }

    void Update() {
        if(Vector3.Distance(newPos, this.transform.position) <= MAX_SMOOTHMOVE_DIST) {
            this.transform.position = Vector3.SmoothDamp(this.transform.position, newPos, ref currentVelocity, smoothTime);
            this.gameObject.GetComponentInChildren<Animator>().SetLayerWeight(RUNNING_ANIMLAYER, currentVelocity.magnitude / MAX_EXPECTED_VELO);
            turnController.animationIsPlaying = (Vector3.Distance(this.transform.position, newPos) > MAX_ANIM_WAIT_DIST);
        }
        
    }

    void updateAngle() {
        this.transform.forward = newPos - oldPos;
    }
}
