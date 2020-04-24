using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;

public class Building {

    [XmlAttribute("name")]
    public string name;

    [XmlElement("Specialization")]
    public string specialization;

    //yield modifiers
    [XmlElement("Food")]
    public float food;

    [XmlElement("RawMaterials")]
    public float rawMaterials;
}