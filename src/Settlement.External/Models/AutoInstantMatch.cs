using System.Xml.Serialization;

namespace Settlement.External.Models;

[XmlRoot("Match")]
public class AutoInstantMatch
{
    [XmlAttribute]
    public int Id { get; set; }

    [XmlElement("Score")]
    public AutoInstantScore[] Scores { get; set; }
}