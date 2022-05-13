using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoneNamePlateController : MonoBehaviour
{
    public GameObject zoneNamePlatePrefab;
    public ActionController actionController;
    public UIController uIController;
    public HexMap hexMap;

    public Sprite[] zoneSprites;


    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindObjectOfType<HexMap>().onZoneCreated += CreateZoneNamePlate;
        actionController = GameObject.FindObjectOfType<ActionController>();
        uIController = GameObject.FindObjectOfType<UIController>();
        hexMap = GameObject.FindObjectOfType<HexMap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateZoneNamePlate(Zone zone, GameObject zoneGO, Dictionary<Zone, GameObject> zoneToGOMap) {
        // Don't want zone nameplates for city centers -- just a city nameplate
        if(zone.GetZoneType() == Zone.ZONE_TYPE.CITY_CENTER) {
            return;
        }
        
        GameObject nameGO = Instantiate(zoneNamePlatePrefab, this.transform);
        MapObjectNamePlate namePlateGO = nameGO.GetComponent<MapObjectNamePlate>();
        namePlateGO.target = zoneGO;
        namePlateGO.GetComponentInChildren<Text>().text = zone.GetPopulation().ToString();
        namePlateGO.GetComponentsInChildren<Image>()[1].sprite = zoneSprites[(int) zone.GetZoneType()];
        nameGO.GetComponentInChildren<Button>().onClick.AddListener(delegate { actionController.SelectZone(zone); });
        uIController.MapZoneToNameplate(zone, namePlateGO);
    }
}
