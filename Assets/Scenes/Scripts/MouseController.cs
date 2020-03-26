using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public static readonly float MAX_ZOOMOUT = 20; //farthest they can zoom out
    public static readonly float MAX_ZOOMIN = 2; //farthest they can zoom in
    public static readonly float MAX_CAMANGLE = 60; //steepest angle the camera will reach while zooming out
    public static readonly float MIN_CAMANGLE = 35; //shallowest angle the camera will reach while zooming in
    public static readonly float CAMANGLE_CHANGE_FACTOR = 1.5f; //determines when the camera starts to change angles while zooming
    public static readonly float ZOOM_THRESH = 0.01f; //minimum change in zoom to process

    public static readonly int BUTTON_LEFTMOUSE = 0;


    Vector3 lastMousePos; //from input.mouseposition

    //camera dragging stuff
    Vector3 lastMouseGroundPlanePos;

    delegate void updateFunc();
    updateFunc update_CurrentFunc;

    // Start is called before the first frame update
    void Start() {
        update_CurrentFunc = update_DetectModeStart;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            cancelUpdateFunc();
        }
        update_CurrentFunc();

        //always allow for camera zooming with scrollwheel
        update_ScrollZoom();

        lastMousePos = Input.mousePosition;
    }

    void update_DetectModeStart() {
        if (Input.GetMouseButtonDown(BUTTON_LEFTMOUSE)) {   // LMB was pressed down this frame

        } else if (Input.GetMouseButtonUp(BUTTON_LEFTMOUSE)) {
            //TODO: are we click on a tile with a unit?

        } else if (Input.GetMouseButton(0) && Input.mousePosition != lastMousePos) { // LMB is being held down

            // LMB held down and mouse moved -- camera drag
            lastMouseGroundPlanePos = mouseToGroundPlane(Input.mousePosition);
            update_CurrentFunc = update_clickAndDrag;
            update_CurrentFunc();
        }
    }

    void update_ScrollZoom() {
        this.lastMouseGroundPlanePos = mouseToGroundPlane(Input.mousePosition);
        //handle scrollwheel zooming
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollAmount) > ZOOM_THRESH) {
            handleZoom(lastMouseGroundPlanePos, scrollAmount);
        }
    }

    private void handleZoom(Vector3 hitPos, float scrollAmount) {
        //move camera towards hitpoint
        Vector3 dir = hitPos - Camera.main.transform.position;
        Vector3 p = Camera.main.transform.position;

        // allow them to scroll if they're in bounds or they're scrolling back in bounds
        if ((p.y <= MAX_ZOOMOUT || scrollAmount > 0) && (p.y >= MAX_ZOOMIN || scrollAmount < 0)) {
            Camera.main.transform.Translate(dir * scrollAmount, Space.World);
        }

        //change angle of camera
        Camera.main.transform.rotation = Quaternion.Euler(
            Mathf.Lerp(MIN_CAMANGLE, MAX_CAMANGLE, p.y / (MAX_ZOOMOUT / CAMANGLE_CHANGE_FACTOR)),
            Camera.main.transform.rotation.eulerAngles.y,
            Camera.main.transform.rotation.eulerAngles.z);
    }

    void update_clickAndDrag() {
        if (Input.GetMouseButtonUp(BUTTON_LEFTMOUSE)) {
            cancelUpdateFunc();
            return;
        }

        Vector3 hitPos = mouseToGroundPlane(Input.mousePosition);

        Vector3 diff = lastMouseGroundPlanePos - hitPos;
        Camera.main.transform.Translate(diff, Space.World);
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        //what is the point at which the mouse ray intersects Y=0
        if (mouseRay.direction.y >= 0) {
            Debug.Log("Mouse is pointing up!");
            return;
        }
        float rayLength = mouseRay.origin.y / mouseRay.direction.y;
        this.lastMouseGroundPlanePos = mouseRay.origin - (mouseRay.direction * rayLength);
    }

    private Vector3 mouseToGroundPlane(Vector3 mousePos) {
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);

        //what is the point at which the mouse ray intersects Y=0
        if (mouseRay.direction.y >= 0) {
            Debug.Log("Mouse is pointing up!");
            return Vector3.zero;
        }
        float rayLength = mouseRay.origin.y / mouseRay.direction.y;
        return mouseRay.origin - (mouseRay.direction * rayLength);
    }

    private void cancelUpdateFunc() {
        update_CurrentFunc = update_DetectModeStart;

        //clean up any UI stuff associated with modes
    }
}
