/* 
 * FigmaViewContent.cs 
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using FigmaSharp;
using MonoDevelop.Core.Serialization;
using System.Collections;
using FigmaSharp.Models;

namespace MonoDevelop.Figma
{
    class FigmaColorWrapper : IExtendedDataItem
    {
        public IDictionary ExtendedProperties => null;

        Color color;
        public FigmaColorWrapper(Color color) 
        {
            this.color = color;
        }

        public double R
        {
            get => color.R;
            set => color.R = value;
        }
        public double G
        {
            get => color.G;
            set => color.G = value;
        }
        public double B
        {
            get => color.B;
            set => color.B = value;
        }
        public double A
        {
            get => color.A;
            set => color.A = value;
        }
    }

    class FigmaFrameEntityWrapper : FigmaNodeWrapper
    {
        FigmaFrame vector => (FigmaFrame)node;

        public FigmaFrameEntityWrapper(FigmaFrame node) : base(node)
        {

        }

        public FigmaBackground[] Background
        {
            get => vector.background;
            set => vector.background = value;
        }

        public string BackgroundColor {
            get
            {
                if (vector.backgroundColor == null)
                {
                    return "0;0;0;0";
                }
                return string.Format("{0};{1};{2};{3}", vector.backgroundColor.R, vector.backgroundColor.G, vector.backgroundColor.B, vector.backgroundColor.A);
            }
            set
            {
                try
                {
                    var splitted = value.Split(';');
                    vector.backgroundColor.R = double.Parse (splitted[0]);
                    vector.backgroundColor.G = double.Parse (splitted[1]);
                    vector.backgroundColor.B = double.Parse (splitted[2]);
                    vector.backgroundColor.A = double.Parse (splitted[3]);
                }
                catch (System.Exception)
                {

                }
            }
        }

        public bool IsMask
        {
            get => vector.isMask;
            set => vector.isMask = value;
        }

        public string BlendMode
        {
            get => vector.blendMode;
            set => vector.blendMode = value;
        }

        public float Opacity
        {
            get => vector.opacity;
            set => vector.opacity = value;
        }

        public float X
        {
            get => vector.absoluteBoundingBox?.X ?? 0;
            set
            {
                if (vector.absoluteBoundingBox != null)
                {
                    vector.absoluteBoundingBox.X = value;
                }
            }
        }

        public float Y
        {
            get => vector.absoluteBoundingBox?.Y ?? 0;
            set
            {
                if (vector.absoluteBoundingBox != null)
                {
                    vector.absoluteBoundingBox.Y = value;
                }
            }
        }

        public float Width
        {
            get => vector.absoluteBoundingBox?.Width ?? 0;
            set
            {
                if (vector.absoluteBoundingBox != null)
                {
                    vector.absoluteBoundingBox.Width = value;
                }
            }
        }

        public float Heigth
        {
            get => vector.absoluteBoundingBox?.Height ?? 0;
            set
            {
                if (vector.absoluteBoundingBox != null)
                {
                    vector.absoluteBoundingBox.Height = value;
                }
            }
        }
    }

    class FigmaRectangleVectorWrapper : FigmaVectorEntityWrapper
    {
        RectangleVector rectangleVector => (RectangleVector)node;
        public FigmaRectangleVectorWrapper(RectangleVector node) : base(node)
        {
        }

        public float CornerRadius
        {
            get => rectangleVector.cornerRadius;
            set => rectangleVector.cornerRadius = value;
        }

        //public float[] RectangleCornerRadii
        //{
        //    get => rectangleVector.rectangleCornerRadii;
        //    set => rectangleVector.rectangleCornerRadii = value;
        //}
    }

    class FigmaVectorEntityWrapper : FigmaNodeWrapper
    {
        FigmaVectorEntity vector;
        public FigmaVectorEntityWrapper(FigmaVectorEntity node) : base(node)
        {
            vector = node;
        }

        public bool IsMask
        {
            get => vector.isMask;
            set => vector.isMask = value;
        }

        public int StrokeWeight
        {
            get => vector.strokeWeight;
            set => vector.strokeWeight = value;
        }

        public string StrokeAlign
        {
            get => vector.strokeAlign;
            set => vector.strokeAlign = value;
        }

        public float Opacity
        {
            get => vector.opacity;
            set => vector.opacity = value;
        }

        //public FigmaPaint[] Fills
        //{
        //    get => vector.fills;
        //    set => vector.fills = value;
        //}
        //public FigmaPaint[] Strokes
        //{
        //    get => vector.strokes;
        //    set => vector.strokes = value;
        //}

        //public FigmaPath[] StrokeGeometry
        //{
        //    get => vector.strokeGeometry;
        //    set => vector.strokeGeometry = value;
        //}

        public float X
        {
            get => vector.absoluteBoundingBox?.X ?? 0;
            set
            {
                if (vector.absoluteBoundingBox != null)
                {
                    vector.absoluteBoundingBox.X = value;
                }
            }
        }

        public float Y
        {
            get => vector.absoluteBoundingBox?.Y ?? 0;
            set
            {
                if (vector.absoluteBoundingBox != null)
                {
                    vector.absoluteBoundingBox.Y = value;
                }
            }
        }

        public float Width
        {
            get => vector.absoluteBoundingBox?.Width ?? 0;
            set
            {
                if (vector.absoluteBoundingBox != null)
                {
                    vector.absoluteBoundingBox.Width = value;
                }
            }
        }

        public float Heigth
        {
            get => vector.absoluteBoundingBox?.Height ?? 0;
            set
            {
                if (vector.absoluteBoundingBox != null)
                {
                    vector.absoluteBoundingBox.Height = value;
                }
            }
        }
    }

    class FigmaTextWrapper : FigmaVectorEntityWrapper
    {
        FigmaText text;
        public FigmaTextWrapper(FigmaText node) : base(node)
        {
            this.text = node;
        }

        public string Characters
        {
            get => text.characters;
            set => text.characters = value;
        }

        public FigmaTypeStyle Style
        {
            get => text.style;
            set => text.style = value;
        }
    }

    class FigmaCanvasWrapper : FigmaNodeWrapper
    {
        FigmaCanvas vector => (FigmaCanvas)node;
        public FigmaCanvasWrapper(FigmaCanvas node) : base(node)
        {
        }

        public string BackgroundColor
        {
            get
            {
                if (vector.backgroundColor == null)
                {
                    return "0;0;0;0";
                }
                return string.Format("{0};{1};{2};{3}", vector.backgroundColor.R, vector.backgroundColor.G, vector.backgroundColor.B, vector.backgroundColor.A);
            }
            set
            {
                try
                {
                    var splitted = value.Split(';');
                    vector.backgroundColor.R = float.Parse(splitted[0]);
                    vector.backgroundColor.G = float.Parse(splitted[1]);
                    vector.backgroundColor.B = float.Parse(splitted[2]);
                    vector.backgroundColor.A = float.Parse(splitted[3]);
                }
                catch (System.Exception)
                {
                }
            }
        }
    }

    class FigmaNodeWrapper : IExtendedDataItem
    {
        public IDictionary ExtendedProperties => null;
        protected FigmaNode node;
        public FigmaNodeWrapper(FigmaNode node)
        {
            this.node = node;
        }

        public string Id
        {
            get => node.id;
            set => node.id = value;
        }

        public string Name
        {
            get => node.name;
            set => node.name = value;
        }

        public string NodeType
        {
            get => node.type;
            set => node.type = value;
        }
    }
}