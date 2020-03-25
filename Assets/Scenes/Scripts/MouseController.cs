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

    bool isDraggingCam = false;
    Vector3 lastMousePos;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        //what is the point at which the mouse ray intersects Y=0
        if (mouseRay.direction.y >= 0) {
            Debug.Log("Mouse is pointing up!");
            return;
        }
        float rayLength = mouseRay.origin.y / mouseRay.direction.y;
        Vector3 hitPos = mouseRay.origin - (mouseRay.direction * rayLength);

        //update fields with current input data
        updateFields(hitPos);

        //handle click and drag camera
        if (isDraggingCam) {
            handleDrag(hitPos, rayLength);
        }

        //handle scrollwheel zooming
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollAmount) > ZOOM_THRESH) {
            handleZoom(hitPos, scrollAmount);
        }
    }

    private void updateFields(Vector3 hitPos) {
        if (Input.GetMouseButtonDown(BUTTON_LEFTMOUSE)) {
            // mouse button just went down
            this.isDraggingCam = true;
            this.lastMousePos = hitPos;

        } else if (Input.GetMouseButtonUp(BUTTON_LEFTMOUSE)) {
            this.isDraggingCam = false;
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

    private void handleDrag(Vector3 hitPos, float rayLength) {
        Vector3 diff = lastMousePos - hitPos;
        Camera.main.transform.Translate(diff, Space.World);
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        //what is the point at which the mouse ray intersects Y=0
        if (mouseRay.direction.y >= 0) {
            Debug.Log("Mouse is pointing up!");
            return;
        }
        rayLength = mouseRay.origin.y / mouseRay.direction.y;
        this.lastMousePos = mouseRay.origin - (mouseRay.direction * rayLength);
    }
}
