using System;
using System.Xml.Serialization;

namespace FigmaSharp.Views.Graphics
{
    [Serializable()]
    public class HtmlElement
    {
        [XmlAttribute(attributeName: "class")]
        public string Class { get; set; }

        [XmlAttribute(attributeName: "id")]
        public string Id { get; set; }
    }

    [Serializable()]
    public class Mask : HtmlElement
    {
        [XmlAttribute(attributeName: "x")]
        public float X { get; set; }

        [XmlAttribute(attributeName: "y")]
        public float Y { get; set; }

        [XmlAttribute(attributeName: "width")]
        public float Width { get; set; }

        [XmlAttribute(attributeName: "height")]
        public float Height { get; set; }

        [XmlAttribute(attributeName: "maskUnits")]
        public string Units { get; set; }

        [XmlAttribute(attributeName: "mask-type")]
        public string MaskType { get; set; }

        [XmlElement("g", Type = typeof(GPath))]
        [XmlElement("path", Type = typeof(Path))]
        [XmlElement("circle", Type = typeof(CirclePath))]
        public HtmlElement[] Paths { get; set; }
    }

    [Serializable()]
    public class TextPath : HtmlElement
    {
        [XmlAttribute(attributeName: "font-size")]
        public float FontSize { get; set; }

        [XmlAttribute(attributeName: "x")]
        public float X { get; set; }

        [XmlAttribute(attributeName: "rx")]
        public float RX { get; set; }

        [XmlAttribute(attributeName: "y")]
        public float Y { get; set; }

        [XmlAttribute(attributeName: "ry")]
        public float RY { get; set; }
    }

    [Serializable()]
    public class RectanglePath : LayerHtmlElement
    {
        [XmlAttribute(attributeName: "x")]
        public float X { get; set; }

        [XmlAttribute(attributeName: "y")]
        public float Y { get; set; }

        [XmlAttribute(attributeName: "rx")]
        public float RX { get; set; }

        [XmlAttribute(attributeName: "ry")]
        public float RY { get; set; }

        [XmlAttribute(attributeName: "width")]
        public float Width { get; set; }

        [XmlAttribute(attributeName: "height")]
        public float Height { get; set; }
    }

    [Serializable()]
    public class GPath : HtmlElement
    {
        [XmlAttribute(attributeName: "mask")]
        public string Mask { get; set; }

        [XmlElement("g", Type = typeof(GPath))]
        [XmlElement("path", Type = typeof(Path))]
        [XmlElement("circle", Type = typeof(CirclePath))]
        [XmlElement("rect", Type = typeof(RectanglePath))]
        [XmlElement("line", Type = typeof(LinePath))]
        [XmlElement("text", Type = typeof(TextPath))]
        public HtmlElement[] Paths { get; set; }
    }

    [Serializable()]
    public abstract class LayerHtmlElement : HtmlElement
    {
        [XmlAttribute(attributeName: "fill-opacity")]
        public float FillOpacity { get; set; }

        [XmlAttribute(attributeName: "transform")]
        public string Transform { get; set; }

        [XmlAttribute(attributeName: "fill")]
        public string Fill { get; set; }

        [XmlAttribute(attributeName: "stroke")]
        public string Stroke { get; set; }

        [XmlAttribute(attributeName: "stroke-width")]
        public float StrokeWidth { get; set; }
    }

    [Serializable()]
    public class CirclePath : LayerHtmlElement
    {
        [XmlAttribute(attributeName: "x")]
        public float X { get; set; }

        [XmlAttribute(attributeName: "rx")]
        public float RX { get; set; }

        [XmlAttribute(attributeName: "y")]
        public float Y { get; set; }

        [XmlAttribute(attributeName: "ry")]
        public float RY { get; set; }

        [XmlAttribute(attributeName: "r")]
        public float Radio { get; set; }
    }

    [Serializable()]
    public class LinePath : LayerHtmlElement
    {
        [XmlAttribute(attributeName: "x1")]
        public float X1 { get; set; }

        [XmlAttribute(attributeName: "x2")]
        public float X2 { get; set; }

        [XmlAttribute(attributeName: "y1")]
        public float Y1 { get; set; }

        [XmlAttribute(attributeName: "y2")]
        public float Y2 { get; set; }
    }

    [Serializable()]
    public class Path : LayerHtmlElement
    {
        [XmlAttribute(attributeName: "d")]
        public string d { get; set; }
    }
}
