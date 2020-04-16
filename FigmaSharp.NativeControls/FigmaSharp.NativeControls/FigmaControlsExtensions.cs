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
        NotDefined,
		Button,
		TextField,
		TextView,
		Filter,
		RadioButton,
		CheckBox,
		PopupButton,
		ComboBox,
		ProgressSpinner,
        DisclosureTriange,
        Stepper,
		Label,
        WindowStandard,
		WindowSheet,
		WindowPanel
    }

    public enum NativeControlComponentType
    {
        NotDefined,

        ButtonStandard,
        ButtonLarge,
        ButtonSmall,
        ButtonLargeDark,
        ButtonStandardDark,
        ButtonSmallDark,

        ButtonHelp,
        ButtonHelpDark,

        TextFieldStandard,
        TextFieldSmall,
        TextFieldStandardDark,
        TextFieldSmallDark,

        TextViewStandard,
        TextViewStandardDark,
        TextViewSmall,
        TextViewSmallDark,

        FilterSmallDark,
        FilterStandardDark,
        FilterSmall,
        FilterStandard,

        RadioSmallDark,
        RadioStandardDark,
        RadioSmall,
        RadioStandard,

        RadioSingleStandard,

        CheckboxSmallDark,
        CheckboxStandardDark,
        CheckboxSmall,
        CheckboxStandard,

        PopUpButtonSmall,
        PopUpButtonStandard,
        PopUpButtonSmallDark,
        PopUpButtonStandardDark,

        LabelStandard,
        LabelSmall,
        LabelStandardDark,
        LabelSmallDark,
        LabelSecondary,
        LabelSecondaryDark,

        LinkStandard,
        LinkSmall,

        LabelGroup,

        ComboBoxStandard,
        ComboBoxSmall,
        ComboBoxStandardDark,
        ComboBoxSmallDark,

        ProgressSpinnerSmallDark,
        ProgressSpinnerStandardDark,
        ProgressSpinnerSmall,
        ProgressSpinnerStandard,

        DisclosureTriangleStandard,
        DisclosureTriangleStandardDark,

        StepperSmallDark,
        StepperStandardDark,
        StepperSmall,
        StepperStandard,

        // Windows
        WindowSheet,
        WindowSheetDark,
        WindowPanel,
        WindowPanelDark,
        WindowStandard,
        WindowStandardDark
    }

    public static class FigmaControlsExtension
    {
        static IReadOnlyList<(string name, NativeControlComponentType nativeControlComponentType, NativeControlType nativeControlType)> data = new List<(string name, NativeControlComponentType nativeControlComponentType, NativeControlType nativeControlType)>
        {
            ( "Button/Standard", NativeControlComponentType.ButtonStandard,  NativeControlType.Button),
            ( "Button/Large", NativeControlComponentType.ButtonLarge,  NativeControlType.Button ),
            ( "Button/Small", NativeControlComponentType.ButtonSmall,  NativeControlType.Button ),

            ( "Button/Large Dark", NativeControlComponentType.ButtonLargeDark,  NativeControlType.Button),
            ( "Button/Standard Dark", NativeControlComponentType.ButtonStandardDark,  NativeControlType.Button),
            ( "Button/Small Dark", NativeControlComponentType.ButtonSmallDark,  NativeControlType.Button),

            ( "Button/Help", NativeControlComponentType.ButtonHelp,  NativeControlType.Button),
            ( "Button/Help Dark", NativeControlComponentType.ButtonHelpDark,  NativeControlType.Button),

            ( "TextField/Standard", NativeControlComponentType.TextViewStandard, NativeControlType.TextField),
            ( "TextField/Small", NativeControlComponentType.TextViewSmall, NativeControlType.TextField),
            ( "TextField/Standard Dark", NativeControlComponentType.TextViewStandardDark, NativeControlType.TextField),
            ( "TextField/Small Dark", NativeControlComponentType.TextViewSmallDark, NativeControlType.TextField),

            ( "TextView/Standard", NativeControlComponentType.TextViewStandard, NativeControlType.TextView),
            ( "TextView/Small", NativeControlComponentType.TextViewSmall, NativeControlType.TextView),
            ( "TextView/Standard Dark", NativeControlComponentType.TextViewStandardDark, NativeControlType.TextView),
            ( "TextView/Small Dark", NativeControlComponentType.TextViewSmallDark, NativeControlType.TextView),

            ( "Search/Small Dark", NativeControlComponentType.FilterSmallDark, NativeControlType.Filter),
            ( "Search/Standard Dark", NativeControlComponentType.FilterStandardDark, NativeControlType.Filter),
            ( "Search/Small", NativeControlComponentType.FilterSmall, NativeControlType.Filter),
            ( "Search/Standard", NativeControlComponentType.FilterStandard, NativeControlType.Filter),

            ( "Radio/Small Dark", NativeControlComponentType.RadioSmallDark, NativeControlType.RadioButton),
            ( "Radio/Standard Dark", NativeControlComponentType.RadioStandardDark, NativeControlType.RadioButton),
            ( "Radio/Small", NativeControlComponentType.RadioSmall, NativeControlType.RadioButton),
            ( "Radio/Standard", NativeControlComponentType.RadioStandard, NativeControlType.RadioButton), //Radio/Label/Standard

            ( "Checkbox/Standard", NativeControlComponentType.CheckboxStandard, NativeControlType.CheckBox),
            ( "Checkbox/Standard Dark", NativeControlComponentType.CheckboxStandardDark, NativeControlType.CheckBox),
            ( "Checkbox/Small", NativeControlComponentType.CheckboxSmall, NativeControlType.CheckBox),
            ( "Checkbox/Small Dark", NativeControlComponentType.CheckboxSmallDark, NativeControlType.CheckBox),
          
			//Pop Up Button
            ( "PopUp Button/Standard", NativeControlComponentType.PopUpButtonStandard, NativeControlType.PopupButton),
            ( "PopUp Button/Standard Dark", NativeControlComponentType.PopUpButtonStandardDark , NativeControlType.PopupButton),
            ( "PopUp Button/Small", NativeControlComponentType.PopUpButtonSmall, NativeControlType.PopupButton),
            ( "PopUp Button/Small Dark", NativeControlComponentType.PopUpButtonSmallDark, NativeControlType.PopupButton),

			//Pull Down Button
			( "PullDown Button/Standard", NativeControlComponentType.PopUpButtonStandard, NativeControlType.PopupButton),
            ( "PullDown Button/Standard Dark", NativeControlComponentType.PopUpButtonStandardDark , NativeControlType.PopupButton),
            ( "PullDown Button/Small", NativeControlComponentType.PopUpButtonSmall, NativeControlType.PopupButton),
            ( "PullDown Button/Small Dark", NativeControlComponentType.PopUpButtonStandardDark, NativeControlType.PopupButton),

			//General
            ( "ComboBox/Standard", NativeControlComponentType.ComboBoxStandard, NativeControlType.ComboBox),
            ( "ComboBox/Small", NativeControlComponentType.ComboBoxSmall, NativeControlType.ComboBox),
            ( "ComboBox/Standard Dark", NativeControlComponentType.ComboBoxStandardDark, NativeControlType.ComboBox),
            ( "ComboBox/Small Dark", NativeControlComponentType.ComboBoxSmallDark, NativeControlType.ComboBox),

            ("ProgressIndicator/Circular", NativeControlComponentType.ProgressSpinnerStandard, NativeControlType.ProgressSpinner),
            ("ProgressIndicator/Circular/Dark", NativeControlComponentType.ProgressSpinnerStandardDark, NativeControlType.ProgressSpinner),
            ("ProgressIndicator/Circular/Small", NativeControlComponentType.ProgressSpinnerSmall, NativeControlType.ProgressSpinner),
            ("ProgressIndicator/Circular/Small/Dark", NativeControlComponentType.ProgressSpinnerSmallDark, NativeControlType.ProgressSpinner),

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
            ( "Label/Secondary", NativeControlComponentType.LabelSecondary, NativeControlType.Label),
            ( "Label/Secondary Dark", NativeControlComponentType.LabelSecondaryDark, NativeControlType.Label),

            ( "Label/Group", NativeControlComponentType.LabelGroup, NativeControlType.Label),

            ( "Link/Standard", NativeControlComponentType.LinkStandard, NativeControlType.Label),
            ( "Link/Small", NativeControlComponentType.LinkSmall, NativeControlType.Label),

            ( "Window/Standard", NativeControlComponentType.WindowStandard, NativeControlType.WindowStandard),
            ( "Window/Standard Dark", NativeControlComponentType.WindowStandardDark, NativeControlType.WindowStandard),
            ( "Window/Sheet", NativeControlComponentType.WindowSheet, NativeControlType.WindowSheet),
            ( "Window/Sheet Dark", NativeControlComponentType.WindowSheetDark, NativeControlType.WindowSheet),
            ( "Window/Panel", NativeControlComponentType.WindowPanel, NativeControlType.WindowPanel),
            ( "Window/Panel Dark", NativeControlComponentType.WindowPanelDark, NativeControlType.WindowPanel),
        };

        public static bool IsWindowContent (this FigmaNode node)
        {
            return (node.Parent?.IsDialogParentContainer() ?? false) && node.IsNodeWindowContent ();
        }

        public static FigmaNode GetWindowContent (this FigmaNode node)
        {
            if (node is IFigmaNodeContainer nodeContainer)
            {
                if (node.IsDialogParentContainer())
                {
                    var content = nodeContainer.children.FirstOrDefault(s => s.IsWindowContent());
                    return content;
                }
            }
            return null;
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

        public static IEnumerable<FigmaNode> GetChildren (this FigmaNode figmaNode)
        {
            var figmaInstance = figmaNode.GetDialogInstanceFromParentContainer();
            var contentNode = figmaNode.GetWindowContent() ?? figmaNode;

            if (contentNode is IFigmaNodeContainer container) {
                foreach (var item in container.children) {
                    if (item == figmaInstance)
                        continue;
                    yield return item;
                }
            }
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
            return type;
        }

        public static NativeControlType ToNativeControlType (this FigmaComponent figmaComponent)
        {
            var type = GetNativeControlType (figmaComponent.name);
            return type;
        }

		static NativeControlType GetNativeControlType (string name)
		{
            var found = data.FirstOrDefault (s => s.name == name);
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
