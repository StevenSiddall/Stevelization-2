using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityNamePlateController : MonoBehaviour
{

    public GameObject cityNamePlatePrefab;
    public ActionController actionController;
    public HexMap hexMap;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindObjectOfType<HexMap>().onCityCreated += createCityNamePlate;
        actionController = GameObject.FindObjectOfType<ActionController>();
        hexMap = GameObject.FindObjectOfType<HexMap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createCityNamePlate(City city, GameObject cityGO) {
        GameObject nameGO = Instantiate(cityNamePlatePrefab, this.transform);
        MapObjectNamePlate namePlateGO = nameGO.GetComponent<MapObjectNamePlate>();
        namePlateGO.target = cityGO;
        namePlateGO.GetComponentInChildren<Text>().text = city.name;
        nameGO.GetComponentInChildren<Button>().onClick.AddListener(delegate { actionController.select(hexMap.getCityFromGO(cityGO)); });
    }
}
