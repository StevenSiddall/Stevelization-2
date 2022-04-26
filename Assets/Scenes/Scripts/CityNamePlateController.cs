using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityNamePlateController : MonoBehaviour
{

    public GameObject cityNamePlatePrefab;
    public ActionController actionController;
    public UIController uiController;
    public HexMap hexMap;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindObjectOfType<HexMap>().onCityCreated += CreateCityNamePlate;
        actionController = GameObject.FindObjectOfType<ActionController>();
        uiController = GameObject.FindObjectOfType<UIController>();
        hexMap = GameObject.FindObjectOfType<HexMap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateCityNamePlate(City city, GameObject cityGO, Dictionary<City, GameObject> cityToGOMap, Dictionary<GameObject, City> goToCityMap) {
        GameObject nameGO = Instantiate(cityNamePlatePrefab, this.transform);
        MapObjectNamePlate namePlateGO = nameGO.GetComponent<MapObjectNamePlate>();
        namePlateGO.target = cityGO;
        namePlateGO.GetComponentInChildren<Text>().text = city.name;
        nameGO.GetComponentInChildren<Button>().onClick.AddListener(delegate { actionController.SelectCity(hexMap.GetCityFromGO(cityGO)); });
        uiController.MapCityToNameplate(city, namePlateGO);
    }
}
