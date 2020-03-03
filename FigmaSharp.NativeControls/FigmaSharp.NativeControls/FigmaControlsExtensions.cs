/* 
 * CustomTextFieldConverter.cs
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
using System.Linq;

using FigmaSharp.Models;

namespace FigmaSharp.NativeControls
{
    public enum NativeControlType
	{
        NotDefined = 0,
		Button = 1,
		TextField = 2,
		Filter = 3,
		RadioButton = 4,
		CheckBox = 5,
		PopupButton = 6,
		ComboBox = 7,
		ProgressSpinner = 8,
        DisclosureTriange = 9,
        Stepper = 10,
		Label = 11,
        WindowStandard = 50,
		WindowSheet = 51,
		WindowPanel = 52,
    }

    public enum NativeControlComponentType
    {
        NotDefined = 0,

        ButtonStandard = 10,
        ButtonLarge = 12,
        ButtonSmall = 13,
        ButtonLargeDark = 14,
        ButtonStandardDark = 15,
        ButtonSmallDark = 16,

        ButtonHelp = 17,
        ButtonHelpDark = 18,

        TextFieldStandard = 20,
        TextFieldSmall = 21,
        TextFieldStandardDark = 22,
        TextFieldSmallDark = 23,

        MultilineTextFieldStandard = 25,
        MultilineTextFieldStandardDark = 26,
        MultilineTextFieldSmall = 27,
        MultilineTextFieldSmallDark = 28,

        FilterSmallDark = 30,
        FilterStandardDark =31,
        FilterSmall = 32,
        FilterStandard = 33,

        RadioSmallDark = 40,
        RadioStandardDark = 41,
        RadioSmall = 42,
        RadioStandard = 43,

        RadioSingleStandard = 46,

        CheckboxSmallDark = 50,
        CheckboxStandardDark = 51,
        CheckboxSmall = 52,
        CheckboxStandard = 53,

        PopUpButtonSmall = 60,
        PopUpButtonStandard = 61,
        PopUpButtonSmallDark = 62,
        PopUpButtonStandardDark = 63,

        LabelStandard = 65,
        LabelSmall = 66,
        LabelStandardDark = 67,
        LabelSmallDark = 68,

        LabelGroup = 69,

        ComboBoxStandard = 70,
        ComboBoxSmall = 71,
        ComboBoxStandardDark = 72,
        ComboBoxSmallDark = 73,

        ProgressSpinnerSmallDark = 80,
        ProgressSpinnerStandardDark = 81,
        ProgressSpinnerSmall = 82,
        ProgressSpinnerStandard = 83,

        DisclosureTriangleStandard = 85,
        DisclosureTriangleStandardDark = 86,

        StepperSmallDark = 90,
		StepperStandardDark = 91,
        StepperSmall = 92,
        StepperStandard = 93,

        //Windows
        WindowSheet = 100,
        WindowSheetDark = 102,
        WindowPanel = 110,
        WindowPanelDark = 111,
        WindowStandard = 120,
        WindowStandardDark = 121,

	}

    public static class FigmaControlsExtension
    {
        static IReadOnlyList<(string name, NativeControlComponentType nativeControlComponentType, NativeControlType nativeControlType)> data = new List<(string name, NativeControlComponentType nativeControlComponentType, NativeControlType nativeControlType)>
        {
            ( "Button/Standard", NativeControlComponentType.ButtonStandard,  NativeControlType.Button),
            ( "Button/Standard HC", NativeControlComponentType.ButtonStandard,  NativeControlType.Button),
            ( "Button/Large", NativeControlComponentType.ButtonLarge,  NativeControlType.Button ),
            ( "Button/Large HC", NativeControlComponentType.ButtonLarge,  NativeControlType.Button ),
            ( "Button/Small", NativeControlComponentType.ButtonSmall,  NativeControlType.Button ),
            ( "Button/Small HC", NativeControlComponentType.ButtonSmall,  NativeControlType.Button ),

            ( "Button/Large Dark", NativeControlComponentType.ButtonLargeDark,  NativeControlType.Button),
            ( "Button/Standard Dark", NativeControlComponentType.ButtonStandardDark,  NativeControlType.Button),
            ( "Button/Small Dark", NativeControlComponentType.ButtonSmallDark,  NativeControlType.Button),

            ( "Button/Help", NativeControlComponentType.ButtonHelp,  NativeControlType.Button),
            ( "Button/Help Dark", NativeControlComponentType.ButtonHelpDark,  NativeControlType.Button),
            ( "Button/Help HC", NativeControlComponentType.ButtonHelp,  NativeControlType.Button),

            ( "Text Field/Standard", NativeControlComponentType.TextFieldStandard, NativeControlType.TextField),
            ( "Text Field/Small", NativeControlComponentType.TextFieldSmall, NativeControlType.TextField),
            ( "Text Field/Standard Dark", NativeControlComponentType.TextFieldStandardDark, NativeControlType.TextField),
            ( "Text Field/Small Dark", NativeControlComponentType.TextFieldSmallDark, NativeControlType.TextField),

            ( "Multiline Text Field/Standard", NativeControlComponentType.MultilineTextFieldStandard, NativeControlType.TextField),
            ( "Multiline Text Field/Small", NativeControlComponentType.MultilineTextFieldSmall, NativeControlType.TextField),
            ( "Multiline Text Field/Standard Dark", NativeControlComponentType.MultilineTextFieldStandardDark, NativeControlType.TextField),
            ( "Multiline Text Field/Small Dark", NativeControlComponentType.MultilineTextFieldSmallDark, NativeControlType.TextField),

            ( "Search/Small Dark", NativeControlComponentType.FilterSmallDark, NativeControlType.Filter),
            ( "Search/Standard Dark", NativeControlComponentType.FilterStandardDark, NativeControlType.Filter),
            ( "Search/Small", NativeControlComponentType.FilterSmall, NativeControlType.Filter),
            ( "Search/Standard", NativeControlComponentType.FilterStandard, NativeControlType.Filter),

            ( "Radio/Small Dark", NativeControlComponentType.RadioSmallDark, NativeControlType.RadioButton),
            ( "Radio/Standard Dark", NativeControlComponentType.RadioStandardDark, NativeControlType.RadioButton),
            ( "Radio/Small", NativeControlComponentType.RadioSmall, NativeControlType.RadioButton),
            ( "Radio/Standard", NativeControlComponentType.RadioStandard, NativeControlType.RadioButton), //Radio/Label/Standard
			( "Radio/Small HC", NativeControlComponentType.RadioSmall, NativeControlType.RadioButton),
            ( "Radio/Standard HC", NativeControlComponentType.RadioStandard, NativeControlType.RadioButton), //Radio/Label/Standard

            ( "Checkbox/Standard", NativeControlComponentType.CheckboxStandard, NativeControlType.CheckBox),
            ( "Checkbox/Standard HC", NativeControlComponentType.CheckboxStandard, NativeControlType.CheckBox),
            ( "Checkbox/Standard Dark", NativeControlComponentType.CheckboxStandardDark, NativeControlType.CheckBox),
            ( "Checkbox/Small", NativeControlComponentType.CheckboxSmall, NativeControlType.CheckBox),
			( "Checkbox/Small HC", NativeControlComponentType.CheckboxSmall, NativeControlType.CheckBox),
            ( "Checkbox/Small Dark", NativeControlComponentType.CheckboxSmallDark, NativeControlType.CheckBox),
          
			//Pop Up Button
            ( "Pop Up Button/Standard", NativeControlComponentType.PopUpButtonStandard, NativeControlType.PopupButton),
            ( "Pop Up Button/Standard Dark", NativeControlComponentType.PopUpButtonStandardDark , NativeControlType.PopupButton),
            ( "Pop Up Button/Small", NativeControlComponentType.PopUpButtonSmall, NativeControlType.PopupButton),
            ( "Pop Up Button/Small Dark", NativeControlComponentType.PopUpButtonSmallDark, NativeControlType.PopupButton),

			//Standard HC
            ( "Pop Up Button/Standard HC", NativeControlComponentType.PopUpButtonStandard, NativeControlType.PopupButton),
            ( "Pull Down Button/Standard HC", NativeControlComponentType.PopUpButtonStandard , NativeControlType.PopupButton),
            ( "Pop Up Button/Small HC", NativeControlComponentType.PopUpButtonSmall, NativeControlType.PopupButton),
            ( "Pull Down Button/Small HC", NativeControlComponentType.PopUpButtonSmall, NativeControlType.PopupButton),

			//Pull Down Button
			( "Pull Down Button/Standard", NativeControlComponentType.PopUpButtonStandard, NativeControlType.PopupButton),
            ( "Pull Down Button/Standard Dark", NativeControlComponentType.PopUpButtonStandardDark , NativeControlType.PopupButton),
            ( "Pull Down Button/Small", NativeControlComponentType.PopUpButtonSmall, NativeControlType.PopupButton),
            ( "Pull Down Button/Small Dark", NativeControlComponentType.PopUpButtonStandardDark, NativeControlType.PopupButton),

			//General
            ( "Combo Box/Standard", NativeControlComponentType.ComboBoxStandard, NativeControlType.ComboBox),
            ( "Combo Box/Small", NativeControlComponentType.ComboBoxSmall, NativeControlType.ComboBox),
            ( "Combo Box/Standard Dark", NativeControlComponentType.ComboBoxStandardDark, NativeControlType.ComboBox),
            ( "Combo Box/Small Dark", NativeControlComponentType.ComboBoxSmallDark, NativeControlType.ComboBox),

            ( "Progress Spinner/Small", NativeControlComponentType.ProgressSpinnerSmall, NativeControlType.ProgressSpinner),
            ( "Progress Spinner/Small Dark", NativeControlComponentType.ProgressSpinnerSmallDark, NativeControlType.ProgressSpinner),
            ( "Progress Spinner/Standard", NativeControlComponentType.ProgressSpinnerStandard, NativeControlType.ProgressSpinner),
            ( "Progress Spinner/Standard Dark", NativeControlComponentType.ProgressSpinnerStandardDark, NativeControlType.ProgressSpinner),

            //( "Disclosure Triangle/Standard Dark", NativeControlComponentType.DisclosureTriangleStandardDark, NativeControlType.ProgressSpinner),
            //( "Disclosure Triangle/Standard Dark", NativeControlComponentType.ProgressSpinnerSmallDark, NativeControlType.ProgressSpinner),
            ( "Disclosure Triangle/Standard", NativeControlComponentType.DisclosureTriangleStandard, NativeControlType.DisclosureTriange),
            ( "Disclosure Triangle/Standard Dark", NativeControlComponentType.DisclosureTriangleStandardDark, NativeControlType.DisclosureTriange),

            ( "Stepper/Standard", NativeControlComponentType.StepperStandard, NativeControlType.Stepper),
            ( "Stepper/Standard Dark", NativeControlComponentType.StepperStandardDark, NativeControlType.Stepper),
            ( "Stepper/Small", NativeControlComponentType.StepperSmall, NativeControlType.Stepper),
            ( "Stepper/Small Dark", NativeControlComponentType.StepperSmallDark, NativeControlType.Stepper),


            ( "Label/Standard", NativeControlComponentType.LabelStandard, NativeControlType.Label),
            ( "Label/Standard Dark", NativeControlComponentType.LabelStandardDark, NativeControlType.Label),
            ( "Label/Small", NativeControlComponentType.LabelSmall, NativeControlType.Label),
            ( "Label/Small Dark", NativeControlComponentType.LabelSmallDark, NativeControlType.Label),

            ( "Label/Standard HC Dark", NativeControlComponentType.LabelStandard, NativeControlType.Label),
            ( "Label/Secondary HC Dark", NativeControlComponentType.LabelStandardDark, NativeControlType.Label),
            ( "Label/Small HC Dark", NativeControlComponentType.LabelSmallDark, NativeControlType.Label),

            ( "Label/Group", NativeControlComponentType.LabelGroup, NativeControlType.Label),

            ( "Window/Standard", NativeControlComponentType.WindowStandard, NativeControlType.WindowStandard),
            ( "Window/Standard Dark", NativeControlComponentType.WindowStandardDark, NativeControlType.WindowStandard),
            ( "Window/Sheet", NativeControlComponentType.WindowSheet, NativeControlType.WindowSheet),
            ( "Window/Sheet Dark", NativeControlComponentType.WindowSheetDark, NativeControlType.WindowSheet),
            ( "Window/Panel", NativeControlComponentType.WindowPanel, NativeControlType.WindowPanel),
            ( "Window/Panel Dark", NativeControlComponentType.WindowPanelDark, NativeControlType.WindowPanel),
        };

        public static bool IsWindowContent (this FigmaNode node)
        {
            if (!node.Parent?.IsDialogParentContainer (NativeControlType.WindowStandard) ?? true) {
                return false;
            }
            return node.IsNodeWindowContent ();
        }

        public static bool IsMainDocumentView (this FigmaNode node)
        {
            return node.Parent is FigmaCanvas && !node.IsDialogParentContainer ();
        }

        public static bool IsNodeWindowContent (this FigmaNode node)
		{
            return node.GetNodeTypeName () == "content";
        }

        public static bool HasChildren (this FigmaNode node)
        {
            return node is IFigmaNodeContainer;
        }

        public static bool IsDialogParentContainer (this FigmaNode figmaNode)
        {
            return figmaNode is IFigmaNodeContainer container && container.children.Any (s => s.IsDialog ());
        }

        public static bool IsDialogParentContainer (this FigmaNode figmaNode, NativeControlType controlType)
        {
            return figmaNode is IFigmaNodeContainer container && container.children.OfType<FigmaInstance> ().Any (s => s.IsWindowOfType (controlType));
        }

        public static FigmaInstance GetDialogInstanceFromParentContainer (this FigmaNode figmaNode)
        {
            if (!(figmaNode is IFigmaNodeContainer cont))
                return null;
            return cont.children.OfType<FigmaInstance> ()
                .FirstOrDefault (s => s.IsDialog ());
        }

        public static bool IsDialog (this FigmaNode figmaNode)
        {
            if (TryGetNativeControlType (figmaNode, out var value) &&
				(value == NativeControlType.WindowPanel || value == NativeControlType.WindowSheet || value == NativeControlType.WindowStandard)) {
                return true;
            }
            return false;
        }

        public static bool IsWindowOfType (this FigmaNode figmaNode, NativeControlType controlType)
        {
			if (figmaNode.TryGetNativeControlType (out var value) && value == controlType) {
                return true;
			}
            return false;
        }

        public static bool TryGetNativeControlComponentType (this FigmaNode node, out NativeControlComponentType nativeControlComponentType)
        {
            nativeControlComponentType = NativeControlComponentType.NotDefined;
            if (node is FigmaComponentEntity) {
                nativeControlComponentType = GetNativeControlComponentType (node.name);
                if (nativeControlComponentType == NativeControlComponentType.NotDefined) {
                    nativeControlComponentType = GetNativeControlComponentType (node.id);
                }
                return nativeControlComponentType != NativeControlComponentType.NotDefined;
            }

            if (node is FigmaInstance figmaInstance && figmaInstance.Component != null) {
                nativeControlComponentType = figmaInstance.Component.ToNativeControlComponentType ();
                return nativeControlComponentType != NativeControlComponentType.NotDefined;
            }

            return false;
        }

        public static bool TryGetNativeControlType (this FigmaNode node, out NativeControlType nativeControlType)
        {
            nativeControlType = NativeControlType.NotDefined;
			if (node is FigmaComponentEntity) {
                nativeControlType = GetNativeControlType (node.name);
				if (nativeControlType == NativeControlType.NotDefined) {
                    nativeControlType = GetNativeControlType (node.id);
                }
                return nativeControlType != NativeControlType.NotDefined;
            }

            if (node is FigmaInstance figmaInstance && figmaInstance.Component != null) {
                nativeControlType = figmaInstance.Component.ToNativeControlType ();
                return nativeControlType != NativeControlType.NotDefined;
            }

            return false;
        }

        public static NativeControlComponentType ToNativeControlComponentType (this FigmaComponent figmaComponent)
        {
            var type = GetNativeControlComponentType (figmaComponent.name);
            if (type == NativeControlComponentType.NotDefined) {
                return GetNativeControlComponentType (figmaComponent.key);
            }
            return type;
        }

        public static NativeControlType ToNativeControlType (this FigmaComponent figmaComponent)
        {
            var type = GetNativeControlType (figmaComponent.name);
			if (type == NativeControlType.NotDefined) {
                return GetNativeControlType (figmaComponent.key);
            }
            return type;
        }

		static NativeControlType GetNativeControlType (string name)
		{
            var found = data.FirstOrDefault (s => s.name == name || s.name == name);
            if (found.Equals (default)) {
                Console.WriteLine ("Component Key not found: {0}", name);
                return NativeControlType.NotDefined;
            }
            return found.nativeControlType;
        }

        static NativeControlComponentType GetNativeControlComponentType (string name)
        {
            var found = data.FirstOrDefault (s => s.name == name);
            if (found.Equals (default)) {
                Console.WriteLine ("Component Key not found: {0}", name);
                return NativeControlComponentType.NotDefined;
            }
            return found.nativeControlComponentType;
        }

        //public static NativeControlComponentType ToControlType (this FigmaInstance figmaInstance)
        //{
        //    if (figmaInstance.Component != null) {
        //        var found = data.FirstOrDefault (s => s.name == figmaInstance.Component.key || s.name == figmaInstance.Component.name);
        //        if (!found.Equals (default)) {
        //            return found.nativeControlComponentType;
        //        }

        //        Console.WriteLine ("Component Key not found: {0} - {1}", figmaInstance.Component.key, figmaInstance.Component.name);
        //        //throw new KeyNotFoundException(figmaInstance.Component.key);
        //    }
        //    return NativeControlComponentType.NotDefined;
        //}
    }
}
