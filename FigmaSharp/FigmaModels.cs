/* 
 * FigmaModels.cs - Models to parse json from Figma API
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

namespace FigmaSharp
{
    public class FigmaElipse : FigmaVectorEntity
    {
        public override bool ImageSupported => false;
    }

    public class FigmaStar : FigmaVectorEntity
    {
        public override bool ImageSupported => false;
    }

    public class FigmaInstance : FigmaFrameEntity
    {
        public float componentIdString { get; set; }
    }

    public class FigmaRegularPoligon : FigmaVectorEntity
    {
        public override bool ImageSupported => false;
    }

    public class FigmaLine : FigmaVectorEntity
    {
        public override bool ImageSupported => false;
    }

    public class FigmaRectangleVector : FigmaVectorEntity
    {
        public float cornerRadius { get; set; }
        public float[] rectangleCornerRadii { get; set; }
    }

    public class FigmaResponse
    {
        public string name { get; set; }
        public string lastModified { get; set; }
        public string version { get; set; }
        public FigmaDocument document { get; set; }
        public int schemaVersion { get; set; }
    }

    public class FigmaPaint
    {
        public string ID { get; internal set; }

        public string type { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool visible { get; set; }

        [DefaultValue(1f)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public float opacity { get; set; }

        public FigmaColor color { get; set; }
        public FigmaVector[] gradientHandlePositions { get; set; }
        public FigmaColorStop[] gradientStops { get; set; }
        public string scaleMode { get; set; }
        public FigmaTransform imageTrandform { get; set; }

        public float scalingFactor { get; set; }
        public string imageRef { get; set; }
    }

    public class FigmaColorStop
    {
    }

    public class FigmaConstraint
    {
        public string type { get; set; }
        public float value { get; set; }
    }

    public class FigmaExportSetting
    {
        public string suffix { get; set; }
        public string format { get; set; }
        public FigmaConstraint constraint { get; set; }
    }

    public class FigmaColor
    {
        public float r { get; set; }
        public float g { get; set; }
        public float b { get; set; }
        public float a { get; set; }
    }

    public enum FigmaEasingType
    {
        EASE_IN, EASE_OUT, EASE_IN_AND_OUT,
    }

    public enum FigmaBlendMode
    {
        //normal
        PASS_THROUGH,
        NORMAL,

        //Darken:
        DARKEN,
        MULTIPLY,
        LINEAR_BURN,
        COLOR_BURN,

        //Lighten:
        LIGHTEN,
        SCREEN,
        LINEAR_DODGE,
        COLOR_DODGE,

        //Contrast:
        OVERLAY,
        SOFT_LIGHT,
        HARD_LIGHT,

        //Inversion:
        DIFFERENCE,
        EXCLUSION,

        //Component:
        HUE,
        SATURATION,
        COLOR,
        LUMINOSITY
    }

    public class FigmaGroup : FigmaFrameEntity
    {

    }
   
	public interface IFigmaDocumentContainer : IFigmaNodeContainer, IAbsoluteBoundingBox
	{
		FigmaColor backgroundColor { get; set; }
    }

    public class FigmaBackground
    {
        public string type { get; set; }
        public string blendMode { get; set; }

        public FigmaColor color { get; set; }
    }

    public class FigmaFrameEntity : FigmaNode, IFigmaDocumentContainer, IAbsoluteBoundingBox, IConstraints
    {
        public FigmaColor backgroundColor { get; set; }
        public FigmaExportSetting[] exportSettings { get; set; }
        public string blendMode { get; set; }
        public bool preserveRatio { get; set; }

        public FigmaLayoutConstraint constraints { get; set; }

        public FigmaBackground[] background { get; set; }

        public string transitionNodeID { get; set; }
        public float transitionDuration { get; set; }
        public string transitionEasing { get; set; }

        [DefaultValue(1f)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public float opacity { get; set; }

        public FigmaRectangle absoluteBoundingBox { get; set; }
        public FigmaVector size { get; set; }
        public FigmaTransform relativeTransform { get; set; }
        public bool clipsContent { get; set; }
        public FigmaLayoutGrid[] layoutGrids { get; set; }
        public FigmaEffect[] effects { get; set; }
        public bool isMask { get; set; }
        public FigmaNode[] children { get; set; }
    }

    public class FigmaVector : FigmaNode
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    public interface IAbsoluteBoundingBox
    {
        FigmaRectangle absoluteBoundingBox { get; set; }
    }

    public interface IConstraints
    {
        FigmaLayoutConstraint constraints { get; set; }
    }

    public class FigmaVectorEntity : FigmaNode, IAbsoluteBoundingBox, IConstraints
    {
        public virtual bool ImageSupported => true;

        public FigmaExportSetting[] exportSettings { get; set; }
        public FigmaBlendMode blendMode { get; set; }
        public bool preserveRatio { get; set; }
        public FigmaLayoutConstraint constraints { get; set; }

        public string transitionNodeID { get; set; }
        public float transitionDuration { get; set; }
        public FigmaEasingType transitionEasing { get; set; }

        [DefaultValue(1f)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public float opacity { get; set; }

        public FigmaRectangle absoluteBoundingBox { get; set; }
        public FigmaEffect[] effects { get; set; }
        public FigmaVector size { get; set; }
        public FigmaTransform relativeTransform { get; set; }
        public bool isMask { get; set; }
        public FigmaPaint[] fills { get; set; }
        public FigmaPath[] fillGeometry { get; set; }
        public FigmaPaint[] strokes { get; set; }
        public int strokeWeight { get; set; }
        public FigmaPath[] strokeGeometry { get; set; }
        public string strokeAlign { get; set; }

        public bool HasFills => fills?.Length > 0;
    }

    public class FigmaPath
    {

    }

    public interface IFigmaNodeContainer
    {
        FigmaNode[] children { get; set; }
    }

    public class FigmaBoolean : FigmaVectorEntity, IFigmaNodeContainer
    {
        public override bool ImageSupported => false;
        public FigmaNode[] children { get; set; }
        public string booleanOperation { get; set; }
    }

    public class FigmaTransform
    {
        //public int[][] data { get; set; }
    }

    public class FigmaEffect
    {
        public string type { get; set; }

        [DefaultValue(true)]
        public bool visible { get; set; }

        public float radius { get; set; }
        public FigmaColor color { get; set; }
        public FigmaBlendMode blendMode { get; set; }
        public FigmaVector offset { get; set; }
    }

    public class FigmaCanvas : FigmaNode, IFigmaDocumentContainer
    {
        public FigmaPaint[] background { get; set; }
        public FigmaColor backgroundColor { get; set; }
        public string prototypeStartNodeID { get; set; }
        public FigmaExportSetting[] exportSettings { get; set; }
        public FigmaNode[] children { get; set; }
		public FigmaRectangle absoluteBoundingBox { get; set; }
	}

	public class FigmaLayoutConstraint
    {
        public string vertical { get; set; }
        public string horizontal { get; set; }
    }

    public class FigmaLayoutGrid
    {
        public string pattern { get; set; }
        public float sectionSize { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool visible { get; set; }

        public FigmaColor color { get; set; }
        public string aligment { get; set; }
        public int gutterSize { get; set; }
        public int offset { get; set; }
        public int count { get; set; }
    }

	public class FigmaRectangle : IEquatable<FigmaRectangle>
	{
		public float x { get; set; }
		public float y { get; set; }
		public float width { get; set; }
		public float height { get; set; }

		public static FigmaRectangle Zero { get; set; } = new FigmaRectangle { x=0,y =0,width =0,height= 0 };

        public FigmaRectangle ()
        {

        }

        public FigmaRectangle(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public FigmaRectangle UnionWith (params FigmaRectangle[] allocation)
        {
            float xMin = allocation.Min(s => s.x);
            float yMin = allocation.Min(s => s.y);
            float xMax = allocation.Max(s => s.x + s.width);
            float yMax = allocation.Max(s => s.y + s.height);
            return new FigmaRectangle(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        public bool Equals(FigmaRectangle other)
        {
            if (x != other.x || y != other.y || width != other.width || height != other.height)
            {
                return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is FigmaRectangle)) return false;
            return Equals((FigmaRectangle)obj);
        }
    }

    public class FigmaDocument : FigmaNode
    {
        public FigmaCanvas[] children { get; set; }
    }

    public class FigmaComponent : FigmaFrameEntity
    {
    
    }

    public class ProcessedNode
    {
        public FigmaNode FigmaNode { get; set; }
        public IViewWrapper View { get; set; }
        public ProcessedNode ParentView { get; set; }
    }

    public class FigmaNode
    {
        public FigmaNode Parent { get; set; }

        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }

        [DefaultValue (true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool visible { get; set; }
    }

    public class FigmaText : FigmaVectorEntity
    {
        public override bool ImageSupported => false;
        public string characters { get; set; }
        public FigmaTypeStyle style { get; set; }
        public int[] characterStyleOverrides { get; set; }
        public Dictionary<string, FigmaTypeStyle> styleOverrideTable { get; set; }
    }

    public class FigmaTypeStyle
    {
        public string fontFamily { get; set; }
        public string fontPostScriptName { get; set; }
        public int fontWeight { get; set; }
        public int fontSize { get; set; }
        public string textAlignHorizontal { get; set; }
        public string textAlignVertical { get; set; }
        public float letterSpacing { get; set; }
        public float lineHeightPx { get; set; }
        public int lineHeightPercent { get; set; }
        public FigmaPaint[] fills { get; set; }
    }
}