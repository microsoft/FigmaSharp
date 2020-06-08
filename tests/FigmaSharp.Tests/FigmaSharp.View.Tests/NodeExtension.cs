using System.Xml.Linq;

namespace FigmaSharp.View.Tests
{
    public static class NodeExtension
    {
        public static bool TryGetFloat(this XElement element, string attributeName, out float value)
        {
            var attribute = element.Attribute(attributeName);
            return float.TryParse(attribute?.Value, out value);
        }

        public static bool TryGetInt(this XElement element, string attributeName, out int value)
        {
            var attribute = element.Attribute(attributeName);
            return int.TryParse(attribute?.Value, out value);
        }

        public static bool TryGetString(this XElement element, string attributeName, out string value)
        {
            var attribute = element.Attribute(attributeName);
            value = attribute?.Value;
            return value != null;
        }

        public static bool TryGetViewBox(this XElement element, string attributeName, out Rectangle value)
        {
            var attribute = element.Attribute(attributeName)?.Value;

            if (!string.IsNullOrEmpty(attribute))
            {
                var split = attribute.Split(' ');
                if (split.Length == 4)
                {
                    value = new Rectangle()
                    {
                        X = float.Parse(split[0]),
                        Y = float.Parse(split[1]),
                        Width = float.Parse(split[2]),
                        Height = float.Parse(split[3]),
                    };
                    return true;
                }
            }
            value = null;
            return false;
        }
    }
}
