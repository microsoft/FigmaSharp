using System;
using System.Collections.Generic;
using FigmaSharp.Models;
using System.Linq;

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

            ( "Text Field/Standard", NativeControlComponentType.TextFieldStandard, NativeControlType.TextField),
            ( "Text Field/Small", NativeControlComponentType.TextFieldSmall, NativeControlType.TextField),
            ( "Text Field/Standard Dark", NativeControlComponentType.TextFieldStandardDark, NativeControlType.TextField),
            ( "Text Field/Small Dark", NativeControlComponentType.TextFieldSmallDark, NativeControlType.TextField),

            ( "92af27bcd788e4f3a291b8776adda4a2efc060ef", NativeControlComponentType.FilterSmallDark, NativeControlType.Filter),
            ( "be1d8114b486b8e1fe1e8a01c7521e806d856c5d", NativeControlComponentType.FilterStandardDark, NativeControlType.Filter),
            ( "64b610597e8a6ca2a54f66f6e810457d9d12c633", NativeControlComponentType.FilterSmall, NativeControlType.Filter),
            ( "ebb7f47060094711b295e18af548a27df0a1c2ce", NativeControlComponentType.FilterStandard, NativeControlType.Filter),

            ( "708b4ca16b3b72bfb1c9ec9a25071b74744f5cf1", NativeControlComponentType.RadioSmallDark, NativeControlType.RadioButton),
            ( "844a988e2231ee8c794221e1a8b04522649caca4", NativeControlComponentType.RadioStandardDark, NativeControlType.RadioButton),
            ( "b71c0bed2374cf42b3990fcff961b577d0d912b4", NativeControlComponentType.RadioSmall, NativeControlType.RadioButton),
            ( "be9ae04fb622ea797dc9659e6d3f05987b8567c2", NativeControlComponentType.RadioStandard, NativeControlType.RadioButton), //Radio/Label/Standard

            ( "92f3aa0d8397ca5f0bf76e818e2300152394836a", NativeControlComponentType.RadioSingleStandard, NativeControlType.RadioButton),

            ( "bea94829972c38261c31abceec35bb7cfd2ab440", NativeControlComponentType.CheckboxSmallDark, NativeControlType.CheckBox),
            ( "26e8dc2aa029ec68aa6c42d12935dadb85017bde", NativeControlComponentType.CheckboxStandardDark, NativeControlType.CheckBox),
            ( "0ad661c90900c3a974cf1dd819614fa4704ee4ad", NativeControlComponentType.CheckboxSmall, NativeControlType.CheckBox),
            ( "88c2e67236d56db9147ffa6f1034ba203b794393", NativeControlComponentType.CheckboxStandard, NativeControlType.CheckBox),

			//Pop Up Button
            ( "Pop Up Button/Standard", NativeControlComponentType.PopUpButtonStandard, NativeControlType.PopupButton),
            ( "Pop Up Button/Standard Dark", NativeControlComponentType.PopUpButtonStandard , NativeControlType.PopupButton),
            ( "Pop Up Button/Small", NativeControlComponentType.PopUpButtonSmall, NativeControlType.PopupButton),
            ( "Pop Up Button/Small Dark", NativeControlComponentType.PopUpButtonSmallDark, NativeControlType.PopupButton),

			//Standard HC
            ( "Pop Up Button/Standard HC", NativeControlComponentType.PopUpButtonStandard, NativeControlType.PopupButton),
            ( "Pull Down Button/Standard HC", NativeControlComponentType.PopUpButtonStandard , NativeControlType.PopupButton),
            ( "Pop Up Button/Small HC", NativeControlComponentType.PopUpButtonSmall, NativeControlType.PopupButton),
            ( "Pull Down Button/Small HC", NativeControlComponentType.PopUpButtonSmall, NativeControlType.PopupButton),

			//Pull Down Button
			( "Pull Down Button/Standard", NativeControlComponentType.PopUpButtonStandard, NativeControlType.PopupButton),
            ( "Pull Down Button/Standard Dark", NativeControlComponentType.PopUpButtonStandard , NativeControlType.PopupButton),
            ( "Pull Down Button/Small", NativeControlComponentType.PopUpButtonSmallDark, NativeControlType.PopupButton),
            ( "Pull Down Button/Small Dark", NativeControlComponentType.PopUpButtonStandardDark, NativeControlType.PopupButton),

			//General
            ( "3bc48ea321b06cfa5a05801c9d33d2f3e1b6095a", NativeControlComponentType.ComboBoxStandard, NativeControlType.ComboBox),
            ( "9d78fd2ff7b81a8cf56115504bab1bf8e3e413e9", NativeControlComponentType.ComboBoxSmall, NativeControlType.ComboBox),
            ( "cdb09381b061dddfe1f02fcad38a247b3724d562", NativeControlComponentType.ComboBoxStandardDark, NativeControlType.ComboBox),
            ( "97d0aa7c49386dd69f430b29bd82e89038e386d4", NativeControlComponentType.ComboBoxSmallDark, NativeControlType.ComboBox),

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
            return figmaNode is IFigmaNodeContainer container && container.children.OfType<FigmaInstance> ().Any (s => s.IsDialog ());
        }

        public static bool IsDialogParentContainer (this FigmaNode figmaNode, NativeControlType controlType)
        {
            return figmaNode is IFigmaNodeContainer container && container.children.OfType<FigmaInstance> ().Any (s => s.IsWindowOfType (controlType));
        }
        
        public static bool IsDialog (this FigmaNode figmaNode)
        {
            return figmaNode is FigmaInstance instance &&
				(
				instance.ToNativeControlType () == NativeControlType.WindowPanel ||
                instance.ToNativeControlType () == NativeControlType.WindowSheet ||
                instance.ToNativeControlType () == NativeControlType.WindowStandard
                );
        }

        public static bool IsWindowOfType (this FigmaNode figmaNode, NativeControlType controlType)
        {
            return  figmaNode is FigmaInstance instance && instance.ToNativeControlType () == controlType;
        }

        public static NativeControlComponentType ToNativeControlComponentType (this FigmaInstance figmaInstance)
        {
            if (figmaInstance.Component != null)
            {
                var found = data.FirstOrDefault (s => s.name == figmaInstance.Component.key || s.name == figmaInstance.Component.name);
				if (!found.Equals (default)) {
                    return found.nativeControlComponentType;
                }
				
                Console.WriteLine("Component Key not found: {0} - {1}", figmaInstance.Component.key, figmaInstance.Component.name);
                //throw new KeyNotFoundException(figmaInstance.Component.key);
            }
            return NativeControlComponentType.NotDefined;
        }

        public static NativeControlType ToNativeControlType (this FigmaInstance figmaInstance)
        {
            if (figmaInstance.Component != null) {
                var found = data.FirstOrDefault (s => s.name == figmaInstance.Component.key || s.name == figmaInstance.Component.name);
                if (!found.Equals (default)) {
                    return found.nativeControlType;
                }

                Console.WriteLine ("Component Key not found: {0} - {1}", figmaInstance.Component.key, figmaInstance.Component.name);
                //throw new KeyNotFoundException(figmaInstance.Component.key);
            }
            return NativeControlType.NotDefined;
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
