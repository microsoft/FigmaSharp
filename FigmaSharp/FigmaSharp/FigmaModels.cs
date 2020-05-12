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

using FigmaSharp.Views;

using Newtonsoft.Json;

namespace FigmaSharp.Models
{
	public interface INodeImage
    {
        bool HasImage();
    }

    public class FigmaElipse : FigmaVectorEntity
    {
        
    }

    public class FigmaStar : FigmaVectorEntity
    {
        
    }

    public class FigmaInstance : FigmaFrameEntity
    {
        [Category ("General")]
        [DisplayName ("Component Id")]
        public string componentId { get; set; }

        [Category ("General")]
        [DisplayName ("Component")]
        public FigmaComponent Component { get; set; }

        public override bool HasImage()
        {
            return false;
        }
    }

    public class FigmaRegularPolygon : FigmaVectorEntity
    {
        
    }

    public class FigmaLine : FigmaVectorEntity
    {
        
    }

    public class RectangleVector : FigmaVectorEntity
    {
        [Category ("General")]
        [DisplayName ("Corner Radius")]
        public float cornerRadius { get; set; }

        [Category ("General")]
        [DisplayName ("RectangleCornerRadii")]
        public float[] rectangleCornerRadii { get; set; }
    }

    public class FigmaPaint
    {
        [Category ("General")]
        [DisplayName ("ID")]
        public string ID { get; internal set; }

        [Category ("General")]
        [DisplayName ("Type")]
        public string type { get; set; }

        [Category ("General")]
        [DisplayName ("Visible")]
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool visible { get; set; }

        [Category ("General")]
        [DisplayName ("Opacity")]
        [DefaultValue(1f)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public float opacity { get; set; }

        [Category ("General")]
        [DisplayName ("Color")]
        public Color color { get; set; }

        [Category ("General")]
        [DisplayName ("GradientHandlePositions")]
        public FigmaVector[] gradientHandlePositions { get; set; }

        [Category ("General")]
        [DisplayName ("Gradient Stops")]
        public ColorStop[] gradientStops { get; set; }

        [Category ("General")]
        [DisplayName ("Scale Mode")]
        public string scaleMode { get; set; }

        [Category ("General")]
        [DisplayName ("ImageTransform")]
        public FigmaTransform imageTrandform { get; set; }

        [Category ("General")]
        [DisplayName ("Scaling Factor")]
        public float scalingFactor { get; set; }

        [Category ("General")]
        [DisplayName ("ImageRef")]
        public string imageRef { get; set; }
    }

    public class ColorStop
    {
    }

    public class FigmaConstraint
    {
        [Category ("General")]
        [DisplayName ("Type")]
        public string type { get; set; }

        [Category ("General")]
        [DisplayName ("Value")]
        public float value { get; set; }
    }

    public class FigmaExportSetting
    {
        [Category ("General")]
        [DisplayName ("Suffix")]
        public string suffix { get; set; }

        [Category ("General")]
        [DisplayName ("Format")]
        public string format { get; set; }

        [Category ("General")]
        [DisplayName ("Constraint")]
        public FigmaConstraint constraint { get; set; }
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
        public override bool HasImage()
        {
            return false;
        }
    }
   
	public interface IFigmaDocumentContainer : IFigmaNodeContainer, IAbsoluteBoundingBox
	{
		Color backgroundColor { get; set; }
    }

    public class FigmaBackground
    {
        [Category ("General")]
        [DisplayName ("Type")]
        public string type { get; set; }

        [Category ("General")]
        [DisplayName ("BlendMode")]
        public string blendMode { get; set; }

        [Category ("General")]
        [DisplayName ("Color")]
        public Color color { get; set; }
    }

    public class FigmaSlice : FigmaNode, IAbsoluteBoundingBox, IConstraints
    {
        [Category ("General")]
        [DisplayName ("Absolute BoundingBox")]
        public Rectangle absoluteBoundingBox { get; set; }

        [Category ("General")]
        [DisplayName ("Constraints")]
        public FigmaLayoutConstraint constraints { get; set; }
    }

    public class FigmaFrameEntity : FigmaNode, IFigmaDocumentContainer, IAbsoluteBoundingBox, IConstraints, IFigmaImage
    {
        public virtual bool HasImage()
        {
            return false;
        }

        [Category ("General")]
        [Obsolete ("Please use the fills field instead")]
        [DisplayName ("Background Color")]
        [Description ("[DEPRECATED] Background color of the node. This is deprecated, as frames now support more than a solid color as a background. Please use the fills field instead.")]
        public Color backgroundColor { get; set; }

        [Category ("General")]
        [DisplayName ("Export Settings")]
        public FigmaExportSetting[] exportSettings { get; set; }

        [Category ("General")]
        [DisplayName ("Blend Mode")]
        public string blendMode { get; set; }

        [Category ("General")]
        [DisplayName ("Preserve Ratio")]
        public bool preserveRatio { get; set; }

        [Category ("General")]
        [DisplayName ("Constraints")]
        public FigmaLayoutConstraint constraints { get; set; }

        [Category ("General")]
        [Obsolete ("Please use the fills field instead")]
        [DisplayName ("Background")]
        [Description ("[DEPRECATED] Background of the node. This is deprecated, as backgrounds for frames are now in the fills field.")]
        public FigmaBackground[] background { get; set; }

        [Category ("Transition")]
        [DisplayName ("Transition Node ID")]
        public string transitionNodeID { get; set; }

        [Category ("Transition")]
        [DisplayName ("Transition Duration")]
        public float transitionDuration { get; set; }

        [Category ("Transition")]
        [DisplayName ("Transition Easing")]
        public string transitionEasing { get; set; }

        [Category ("General")]
        [DisplayName ("Opacity")]
        [Description ("Opacity of the node")]
        [DefaultValue(1f)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public float opacity { get; set; }

        [Category ("General")]
        [DisplayName ("Absolute BoundingBox")]
        [Description ("Bounding box of the node in absolute space coordinates")]
        public Rectangle absoluteBoundingBox { get; set; }

        [Category ("General")]
        [DisplayName ("Size")]
        [Description ("Width and height of element. This is different from the width and height of the bounding box in that the absolute bounding box represents the element after scaling and rotation. Only present if geometry=paths is passed")]
        public FigmaVector size { get; set; }

        [Category ("General")]
        [DisplayName ("Relative Transform")]
        [Description ("The top two rows of a matrix that represents the 2D transform of this node relative to its parent. The bottom row of the matrix is implicitly always (0, 0, 1). Use to transform coordinates in geometry. Only present if geometry=paths is passed")]
        public FigmaTransform relativeTransform { get; set; }

        [Category ("General")]
        [DisplayName ("Clips Content")]
        public bool clipsContent { get; set; }

        [Category ("General")]
        [DisplayName ("Layout Grids")]
        public FigmaLayoutGrid[] layoutGrids { get; set; }

        [Category ("General")]
        [DisplayName ("Effects")]
        public FigmaEffect[] effects { get; set; }

        [Category ("General")]
        [System.ComponentModel.DisplayName ("Is Mask")]
        [System.ComponentModel.Description ("Does this node mask sibling nodes in front of it?")]
        public bool isMask { get; set; }

        [DisplayName ("Children")]
        public FigmaNode[] children { get; set; }

        [DisplayName ("Fills")]
        public FigmaPaint[] fills { get; set; }
        public bool HasFills => fills?.Length > 0;

        public string layoutMode { get; set; }

        [Category("AutoLayout")]
        [DisplayName("Layout Mode")]
        public FigmaLayoutMode LayoutMode
        {
            get
            {
                if (string.IsNullOrEmpty (layoutMode))
                    return FigmaLayoutMode.None;

                foreach (var item in Enum.GetValues (typeof (FigmaLayoutMode)))
                {
                    if (item.ToString().ToUpper() == layoutMode)
                        return (FigmaLayoutMode) item;
                }
                return FigmaLayoutMode.None;
            }
        }

        [Category("AutoLayout")]
        [DisplayName("Item Spacing")]
        public float itemSpacing { get; set; }

        [Category("AutoLayout")]
        [DisplayName("Horizontal Padding")]
        public float horizontalPadding { get; set; }

        [DisplayName("Vertical Padding")]
        public float verticalPadding { get; set; } 


    }

    public enum FigmaLayoutMode
    {
        None,
        Vertical,
        Horizontal
    }

    public class FigmaVector : FigmaNode
    {
        [Category ("General")]
        [DisplayName ("X")]
        public float x { get; set; }

        [Category ("General")]
        [DisplayName ("Y")]
        public float y { get; set; }
    }

    public interface IAbsoluteBoundingBox
    {
        Rectangle absoluteBoundingBox { get; set; }
    }

    public interface IConstraints
    {
        FigmaLayoutConstraint constraints { get; set; }
    }

    public interface IFigmaImage
    {
        bool HasImage();
    }

    public class FigmaVectorEntity : FigmaNode, IAbsoluteBoundingBox, IConstraints, IFigmaImage
    {
        public virtual bool HasImage ()
        {
            return true;
        }

		[Category ("General")]
        [DisplayName ("Export Settings")]
        public FigmaExportSetting[] exportSettings { get; set; }

        [Category ("General")]
        [DisplayName ("Blend Mode")]
        public FigmaBlendMode blendMode { get; set; }

        [Category ("General")]
        [DisplayName ("Preserve Ratio")]
        public bool preserveRatio { get; set; }

        [Category ("General")]
        [DisplayName ("Constraints")]
        public FigmaLayoutConstraint constraints { get; set; }

        [Category ("Transition")]
        [DisplayName ("Transition Node ID")]
        public string transitionNodeID { get; set; }

        [Category ("Transition")]
        [DisplayName ("Transition Duration")]
        public float transitionDuration { get; set; }

        [Category ("Transition")]
        [DisplayName ("Transition Easing")]
        public FigmaEasingType transitionEasing { get; set; }

        [Category ("General")]
        [DisplayName ("Opacity")]
        [DefaultValue(1f)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public float opacity { get; set; }

        [Category ("General")]
        [DisplayName ("Absolute BoundingBox")]
        public Rectangle absoluteBoundingBox { get; set; }

        [Category ("General")]
        [DisplayName ("Effects")]
        public FigmaEffect[] effects { get; set; }

        [Category ("General")]
        [DisplayName ("Size")]
        public FigmaVector size { get; set; }

        [Category ("General")]
        [DisplayName ("Relative Transform")]
        public FigmaTransform relativeTransform { get; set; }

        [Category ("General")]
        [DisplayName ("Is Mask")]
        public bool isMask { get; set; }

        [Category ("General")]
        [DisplayName ("FillGeometry")]
        public FigmaPath[] fillGeometry { get; set; }

		[Category ("Stroke")]
        [DisplayName ("Strokes")]
        public FigmaPaint[] strokes { get; set; }

        [Category ("Stroke")]
        [DisplayName ("Stroke Weight")]
        public int strokeWeight { get; set; }

        [Category ("Stroke")]
        [DisplayName ("Stroke Geometry")]
        public FigmaPath[] strokeGeometry { get; set; }

        [Category ("Stroke")]
        [DisplayName ("Stroke Align")]
        public string strokeAlign { get; set; }

        [Category ("Stroke")]
        [DisplayName ("Stroke Dashes")]
        public float[] strokeDashes { get; set; }

        [DisplayName ("Fills")]
        public FigmaPaint[] fills { get; set; }
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
        [Category ("General")]
        [DisplayName ("Children")]
        public FigmaNode[] children { get; set; }

        [Category ("General")]
        [DisplayName ("Boolean Operation")]
        public string booleanOperation { get; set; }

        public override bool HasImage()
        {
            return false;
        }
    }

    public class FigmaTransform
    {
        //public int[][] data { get; set; }
    }

    public class FigmaEffect
    {
        [Category ("General")]
        [DisplayName ("Type")]
        public string type { get; set; }

        [Category ("General")]
        [DisplayName ("Visible")]
        [DefaultValue(true)]
        public bool visible { get; set; }

        [Category ("General")]
        [DisplayName ("Radius")]
        public float radius { get; set; }

        [Category ("General")]
        [DisplayName ("Color")]
        public Color color { get; set; }

        [Category ("General")]
        [DisplayName ("Blend Mode")]
        public FigmaBlendMode blendMode { get; set; }

        [Category ("General")]
        [DisplayName ("Offset")]
        public FigmaVector offset { get; set; }
    }

    public class FigmaCanvas : FigmaNode, IFigmaDocumentContainer
    {
        [Category ("General")]
        [DisplayName ("Background")]
        public FigmaPaint[] background { get; set; }

        [Category ("General")]
        [DisplayName ("Background Color")]
        public Color backgroundColor { get; set; }

        [Category ("General")]
        [DisplayName ("PrototypeStartNodeID")]
        public string prototypeStartNodeID { get; set; }

        [Category ("General")]
        [DisplayName ("Export Settings")]
        public FigmaExportSetting[] exportSettings { get; set; }

        [Category ("General")]
        [DisplayName ("Children")]
        public FigmaNode[] children { get; set; }

        [Category ("General")]
        [DisplayName ("Absolute BoundingBox")]
        public Rectangle absoluteBoundingBox { get; set; }
	}

	public class FigmaLayoutConstraint
    {
        [Category ("General")]
        [DisplayName ("Vertical")]
        public string vertical { get; set; }

        [Category ("General")]
        [DisplayName ("Horizontal")]
        public string horizontal { get; set; }

        public bool IsFlexibleHorizontal => (horizontal.Contains("LEFT") && horizontal.Contains("RIGHT")) || horizontal == "SCALE";
        public bool IsFlexibleVertical => (vertical.Contains("TOP") && vertical.Contains("BOTTOM")) || vertical == "SCALE";

        public bool HasAnyCenterConstraint => vertical == "CENTER" || vertical == "SCALE" || horizontal == "CENTER" || horizontal == "SCALE";
    }

    public class FigmaLayoutGrid
    {
        [Category ("General")]
        [DisplayName ("Pattern")]
        public string pattern { get; set; }

        [Category ("General")]
        [DisplayName ("Section Size")]
        public float sectionSize { get; set; }

        [Category ("General")]
        [DisplayName ("Visible")]
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool visible { get; set; }

        [Category ("General")]
        [DisplayName ("Color")]
        public Color color { get; set; }

        [Category ("General")]
        [DisplayName ("Aligment")]
        public string aligment { get; set; }

        [Category ("General")]
        [DisplayName ("Gutter Size")]
        public int gutterSize { get; set; }

        [Category ("General")]
        [DisplayName ("Offset")]
        public int offset { get; set; }

        [Category ("General")]
        [DisplayName ("Count")]
        public int count { get; set; }
    }

    public class FigmaDocument : FigmaNode
    {
        public FigmaCanvas[] children { get; set; }
    }

    public class ProcessedNode
    {
        public FigmaNode FigmaNode { get; set; }
        public IView View { get; set; }
        public ProcessedNode ParentView { get; set; }

        public override string ToString() =>
            FigmaNode?.ToString() ?? base.ToString();
    }

    public class FigmaComponentEntity : FigmaFrameEntity
    {

    }

    public class FigmaNode
    {
        [Category ("General")]
        [DisplayName ("Parent")]
        public FigmaNode Parent { get; set; }

        [Category ("General")]
        [DisplayName ("Id")]
        public string id { get; set; }

        [Category ("General")]
        [DisplayName ("Name")]
        public string name { get; set; }

        [Category ("General")]
        [DisplayName ("Type")]
        public string type { get; set; }

        [Category ("General")]
        [DisplayName ("Visible")]
        [DefaultValue (true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool visible { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}:{1}:{2}]", type, id, name);
        }
    }

    public class FigmaText : FigmaVectorEntity
    {
        public override bool HasImage()
        {
            return false;
        }

        [Category ("Text Data")]
        [DisplayName ("Characters")]
        [DefaultValue (true)]
        [JsonProperty (DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string characters { get; set; }

        [Category ("Text Data")]
        [DisplayName ("Style")]
        public FigmaTypeStyle style { get; set; }

        [Category("Text Data")]
        [DisplayName("Styles")]
        public Dictionary<string, string> styles { get; set; }

        [Category ("Text Data")]
        [DisplayName ("Character Style Overrides")]
        public int[] characterStyleOverrides { get; set; }

        [Category ("Text Data")]
        [DisplayName ("Style Override Table")]
        public Dictionary<string, FigmaTypeStyle> styleOverrideTable { get; set; }
    }

    public class FigmaTypeStyle
    {
        [Category ("General")]
        [DisplayName ("Font Family")]
        public string fontFamily { get; set; }

        [Category ("General")]
        [DisplayName ("Font PostScriptName")]
        public string fontPostScriptName { get; set; }

        [Category ("General")]
        [DisplayName ("Font Weight")]
        public int fontWeight { get; set; }

        [Category ("General")]
        [DisplayName ("Font Size")]
        public int fontSize { get; set; }

        [Category ("General")]
        [DisplayName ("TextAlignHorizontal")]
        public string textAlignHorizontal { get; set; }

        [Category ("General")]
        [DisplayName ("TextAlignVertical")]
        public string textAlignVertical { get; set; }

        [Category ("General")]
        [DisplayName ("Letter Spacing")]
        public float letterSpacing { get; set; }

        [Category ("General")]
        [DisplayName ("Line Height Px")]
        public float lineHeightPx { get; set; }

        [Category ("General")]
        [DisplayName ("Line Height Percent")]
        public int lineHeightPercent { get; set; }

        [Category ("General")]
        [DisplayName ("Fills")]
        public FigmaPaint[] fills { get; set; }
    }
}