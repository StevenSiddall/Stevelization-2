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
    public static readonly int BUTTON_RIGHTMOUSE = 1;

    bool mouseIsOverUIElement;

    //so we can update the UI
    UIController uiController;
    ActionController actionController;

    //general bookkeeping
    Hex hexUnderMouse;
    Hex lastHexUnderMouse;

    //unit movement stuff
    Hex[] hexPath;
    HexMap hexMap;
    public LayerMask LayerIDForHexTiles;

    Vector3 lastMousePos; //from input.mouseposition

    //camera dragging stuff
    Vector3 lastMouseGroundPlanePos;

    delegate void updateFunc();
    updateFunc update_CurrentFunc;

    // Start is called before the first frame update
    void Start() {
        update_CurrentFunc = UpdateDetectModeStart;
        hexMap = GameObject.FindObjectOfType<HexMap>();
        uiController = FindObjectOfType<UIController>();
        actionController = FindObjectOfType<ActionController>();
    }

    void Update() {
        hexUnderMouse = MouseToHex();
        mouseIsOverUIElement = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            CancelUpdateFunc();
        }
        update_CurrentFunc();

        //always allow for camera zooming with scrollwheel
        UpdateScrollZoom();


        if(actionController.GetSelectedUnit() != null) {
            //draw path
            uiController.DrawPath( (hexPath != null) ? hexPath : actionController.GetSelectedUnit().GetHexPath());
        } else {
            uiController.DrawPath(null);
        }

        lastMousePos = Input.mousePosition;
        lastHexUnderMouse = hexUnderMouse;
    }

    void UpdateDetectModeStart() {
        if (Input.GetMouseButtonDown(BUTTON_LEFTMOUSE)) {   // LMB was pressed down this frame

        } else if (Input.GetMouseButtonUp(BUTTON_LEFTMOUSE)) {
            //are we click on a tile with a unit?
            //Debug.Log("left click");
            actionController.Select(hexUnderMouse);
        } else if (Input.GetMouseButton(BUTTON_LEFTMOUSE) && Input.mousePosition != lastMousePos) { // LMB is being held down
            // LMB held down and mouse moved -- camera drag
            lastMouseGroundPlanePos = MouseToGroundPlane(Input.mousePosition);
            update_CurrentFunc = UpdateClickAndDrag;
            update_CurrentFunc();
        } else if (actionController.GetSelectedUnit() != null && Input.GetMouseButton(BUTTON_RIGHTMOUSE)) {
            //right clicked somewhere -- tell action controller
            update_CurrentFunc = UpdateUnitMovement;
        } else if (Input.GetMouseButton(BUTTON_RIGHTMOUSE)) {
            actionController.ExitZonePlacementMode();
        }
    }

    void UpdateUnitMovement() {

        if (mouseIsOverUIElement) {
            return;
        }

        if (Input.GetMouseButtonUp(BUTTON_RIGHTMOUSE) || actionController.GetSelectedUnit() == null) {
            //queue movement for unit
            if(actionController.GetSelectedUnit() != null) {
                actionController.GetSelectedUnit().SetHexPath(hexPath);
                StartCoroutine(actionController.DoUnitMovement(actionController.GetSelectedUnit()));
            }

            CancelUpdateFunc();
            return;
        }

        if(Input.GetMouseButton(BUTTON_RIGHTMOUSE) && (hexPath == null || hexUnderMouse != lastHexUnderMouse)) {
            // calculate route to hex under mouse
            hexPath = HexPathfinder.GetPath(actionController.GetSelectedUnit().hex, hexUnderMouse);
        }
    }

    void UpdateScrollZoom() {

        if (mouseIsOverUIElement) {
            return;
        }

        this.lastMouseGroundPlanePos = MouseToGroundPlane(Input.mousePosition);
        //handle scrollwheel zooming
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollAmount) > ZOOM_THRESH) {
            HandleZoom(lastMouseGroundPlanePos, scrollAmount);
        }
    }

    private void HandleZoom(Vector3 hitPos, float scrollAmount) {

        if (mouseIsOverUIElement) {
            return;
        }

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

    void UpdateClickAndDrag() {
        if (Input.GetMouseButtonUp(BUTTON_LEFTMOUSE)) {
            CancelUpdateFunc();
            return;
        }

        Vector3 hitPos = MouseToGroundPlane(Input.mousePosition);

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

    public Hex GetHexUnderMouse() {
        return hexUnderMouse;
    }

    public Hex GetLastHexUnderMouse() {
        return lastHexUnderMouse;
    }

    private Vector3 MouseToGroundPlane(Vector3 mousePos) {
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);

        //what is the point at which the mouse ray intersects Y=0
        if (mouseRay.direction.y >= 0) {
            Debug.Log("Mouse is pointing up!");
            return Vector3.zero;
        }
        float rayLength = mouseRay.origin.y / mouseRay.direction.y;
        return mouseRay.origin - (mouseRay.direction * rayLength);
    }

    private void CancelUpdateFunc() {
        update_CurrentFunc = UpdateDetectModeStart;
        hexPath = null;
        //clean up any UI stuff associated with modes
    }

    private Hex MouseToHex() {

        if (mouseIsOverUIElement) {
            return null;
        }

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        int layerMask = LayerIDForHexTiles.value;

        if (Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, layerMask)) {
            // Something got hit
            //Debug.Log( hitInfo.rigidbody.gameObject.transform.parent.gameObject );

            // The collider is a child of the "correct" game object that we want.
            GameObject hexGO = hitInfo.rigidbody.gameObject.transform.parent.gameObject;

            return hexMap.GetHexFromGO(hexGO);
        }

        //no hex there
        return null;
    }
}
