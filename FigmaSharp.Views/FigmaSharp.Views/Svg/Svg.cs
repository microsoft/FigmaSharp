using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FigmaSharp.Views.Graphics
{
    [Serializable()]
    [XmlRoot("svg")]
    public class Svg
    {
        [XmlAttribute(attributeName: "id")]
        public string Id { get; set; }

        [XmlAttribute(attributeName: "width")]
        public float Width { get; set; }

        [XmlAttribute(attributeName: "height")]
        public float Height { get; set; }

        [XmlAttribute(attributeName: "version")]
        public float Version { get; set; }

        #region Titles

        [XmlElement(elementName: "title")]
        public string Title { get; set; }
        [XmlElement(elementName: "drawing-type")]
        public string DrawingType { get; set; }

        #endregion

        [XmlElement("mask")]
        public Mask[] Mask { get; set; }

        [XmlElement("svg")]
        public Svg[] Svgs { get; set; }

        [XmlElement("g", Type = typeof(GPath))]
        [XmlElement("path", Type = typeof(Path))]
        [XmlElement("circle", Type = typeof(CirclePath))]
        [XmlElement("rect", Type = typeof(RectanglePath))]
        [XmlElement("line", Type = typeof(LinePath))]
        [XmlElement("text", Type = typeof(TextPath))]
        public HtmlElement[] Vectors { get; set; }

        [XmlArray("defs")]
        [XmlArrayItem("style", typeof(StyleDefinition))]
        [XmlArrayItem("marker", typeof(MarkerDefinition))]
        [XmlArrayItem("linearGradient", typeof(LinearGradient))]
        public Definition[] Definitions { get; set; }

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

        public static Svg FromData (string data)
        {
            data = data
                .Replace("xmlns=\"http://www.w3.org/2000/svg\"", "")
                .Replace("xmlns=\"http://www.w3.org/1999/xlink\"", "");

            XmlSerializer serializer = new XmlSerializer(typeof(Svg));

            Svg result;
            using (TextReader r = new StringReader(data))
                result = (Svg)serializer.Deserialize(r);
            return result;
            //Parse(xml.Root);
        }
    }
}