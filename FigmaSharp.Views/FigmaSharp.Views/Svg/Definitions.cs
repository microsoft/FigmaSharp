using System;
using System.Xml;
using System.Xml.Serialization;

namespace FigmaSharp.Views.Graphics
{
    [Serializable()]
    public class Definitions : HtmlElement
    {
    }

    [Serializable()]
    public class StyleDefinition : Definitions
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
    public class MarkerDefinition : Definitions
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
