using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapObjectNamePlate : MonoBehaviour
{

    public GameObject target;
    public Vector3 worldPositionOffset = new Vector3(0, 1, 0);
    public Vector3 screenPositionOffset = new Vector3(0, 30, 0);
    public Camera gameCamera;
    RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        if(gameCamera == null) {
            gameCamera = Camera.main;
        }
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(target == null) {
            Destroy(gameObject);
            return;
        }
        // set position based on screen position of object and offset
        Vector3 screenPos = gameCamera.WorldToScreenPoint(target.transform.position + worldPositionOffset);

        rectTransform.anchoredPosition = screenPos + screenPositionOffset;
    }
}
