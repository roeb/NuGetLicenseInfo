using System;
using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot(ElementName = "package")]
public class Package
{
    [XmlAttribute(AttributeName = "id")]
    public string Id { get; set; }
    [XmlAttribute(AttributeName = "version")]
    public string Version { get; set; }
    [XmlAttribute(AttributeName = "targetFramework")]
    public string TargetFramework { get; set; }
}

[XmlRoot(ElementName = "packages")]
public class Packages
{
    [XmlElement(ElementName = "package")]
    public List<Package> Package { get; set; }
}


