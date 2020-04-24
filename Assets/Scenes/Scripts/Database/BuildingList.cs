using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Analytics;

/* Loads a list of lists of building types from Buildings.xml
 * The sublists are indexed by enum in Hex.SPECIALIZATION_TYPE
 */

[XmlRoot("BuildingCollection")]
public class BuildingList {

    [XmlArray("Buildings")]
    [XmlArrayItem("Building")]
    public List<Building> buildings = new List<Building>();

    public static BuildingList load(string filepath) {

        TextAsset _xml = Resources.Load<TextAsset>(filepath);

        XmlSerializer serializer = new XmlSerializer(typeof(BuildingList));

        StringReader reader = new StringReader(_xml.text);

        BuildingList b = serializer.Deserialize(reader) as BuildingList;

        reader.Close();

        return b;
    }
}

