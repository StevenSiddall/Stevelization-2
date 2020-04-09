using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityNamePlateController : MonoBehaviour
{

    public GameObject cityNamePlatePrefab;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindObjectOfType<HexMap>().onCityCreated += createCityNamePlate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createCityNamePlate(City city, GameObject cityGO) {
        GameObject nameGO = Instantiate(cityNamePlatePrefab, this.transform);
        nameGO.GetComponent<MapObjectNamePlate>().target = cityGO;
    }
}
