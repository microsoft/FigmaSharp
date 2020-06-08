using System;
using System.Xml.Serialization;

namespace FigmaSharp.Views.Graphics
{
    [Serializable()]
    public class ViewBoxRectangle
    {
        [XmlAttribute(attributeName: "x")]
        public float X { get; set; }

        [XmlAttribute(attributeName: "rx")]
        public float RX { get; set; }

        [XmlAttribute(attributeName: "y")]
        public float Y { get; set; }

        [XmlAttribute(attributeName: "ry")]
        public float RY { get; set; }

        [XmlAttribute(attributeName: "width")]
        public float Width { get; set; }

        [XmlAttribute(attributeName: "height")]
        public float Height { get; set; }
    }

}