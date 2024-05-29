using System.Globalization;
using System.Xml;

using System.Xml.Serialization;

namespace Settlement.External.Common;

public static class Helper
{
    public static bool IsNumber(object value)
    {
        if (value == null)
        {
            return false;
        }

        return double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out double _);
    }

    public static string GeneralXmlString<T>(T t) where T : class
    {
        var serializer = new XmlSerializer(typeof(T));
        var ns = new XmlSerializerNamespaces();
        ns.Add("", "");
        var settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true
        };
        using (var stringWriter = new StringWriter())
        {
            using (var writer = XmlWriter.Create(stringWriter, settings))
            {
                serializer.Serialize(writer, t, ns);
                return stringWriter.ToString();
            }
        }
    }
}