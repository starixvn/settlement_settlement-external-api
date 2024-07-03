using System.Xml.Serialization;

namespace Settlement.External.Models;

public class AutoInstantScore
{
    [XmlAttribute]
    public short Type { get; set; }

    [XmlAttribute]
    public string HomeValue { get; set; }

    [XmlAttribute]
    public string AwayValue { get; set; }
}