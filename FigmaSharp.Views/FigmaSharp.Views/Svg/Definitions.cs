using System;
using System.Xml;
using System.Xml.Serialization;

namespace FigmaSharp.Views.Graphics
{
    [Serializable()]
    public class Definition : HtmlElement
    {
    }

    [Serializable()]
    public class GradienStop
    {
        [XmlAttribute(attributeName: "stop-color")]
        public string Color { get; set; }

        [XmlAttribute(attributeName: "offset")]
        public float Offset { get; set; }
    }

    [Serializable()]
    public class LinearGradient : Definition
    {
        [XmlAttribute(attributeName: "x1")]
        public float X1 { get; set; }

        [XmlAttribute(attributeName: "x2")]
        public float X2 { get; set; }

        [XmlAttribute(attributeName: "y1")]
        public float Y1 { get; set; }

        [XmlAttribute(attributeName: "y2")]
        public float Y2 { get; set; }

        [XmlAttribute(attributeName: "gradientUnits")]
        public string GradientUnits { get; set; }

        [XmlElement("stop", Type = typeof(GradienStop))]
        public GradienStop[] Stops { get; set; }
    }

    [Serializable()]
    public class StyleDefinition : Definition
    {
        [XmlAttribute(attributeName: "type")]
        public string ContentType { get; set; }

        public string Content { get; set; }

        [XmlText]
        public XmlNode[] CDataContent
        {
            get
            {
                var dummy = new XmlDocument();
                return new XmlNode[] { dummy.CreateCDataSection(Content) };
            }
            set
            {
                if (value == null)
                {
                    Content = null;
                    return;
                }

                if (value.Length != 1)
                {
                    throw new InvalidOperationException(
                        String.Format(
                            "Invalid array length {0}", value.Length));
                }

                Content = value[0].Value;
            }
        }
    }

    [Serializable()]
    public class MarkerDefinition : Definition
    {
        [XmlAttribute(attributeName: "type")]
        public float StyleType { get; set; }

        [XmlAttribute(attributeName: "markerWidth")]
        public float MarkerWidth { get; set; }

        [XmlAttribute(attributeName: "markerHeight")]
        public float MarkerHeight { get; set; }

        [XmlAttribute(attributeName: "orient")]
        public string Orient { get; set; }

        [XmlAttribute(attributeName: "viewBox")]
        public string viewBox { get; set; }

        [XmlIgnore()]
        public ViewBoxRectangle ViewBox
        {
            get
            {
                if (!string.IsNullOrEmpty(viewBox))
                {
                    var splitt = viewBox.Split(' ');
                    if (splitt.Length == 4)
                    {
                        var rectangle = new ViewBoxRectangle()
                        {
                            X = float.Parse(splitt[0]),
                            Y = float.Parse(splitt[1]),
                            Width = float.Parse(splitt[2]),
                            Height = float.Parse(splitt[3]),
                        };

                        return rectangle;
                    }
                }
                return null;
            }
        }

        [XmlElement("g", Type = typeof(GPath))]
        [XmlElement("path", Type = typeof(Path))]
        [XmlElement("circle", Type = typeof(CirclePath))]
        public HtmlElement[] Vectors { get; set; }
    }
}
