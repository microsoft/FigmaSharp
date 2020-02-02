﻿using System;
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
            ( "65e801586bf62f77a4d79085aaeb6e2bbd8a48ea", NativeControlComponentType.ButtonStandard,  NativeControlType.Button),
            ( "52a371ab5df2ad1fe9f7ae1d51712ec9dc493325", NativeControlComponentType.ButtonLarge,  NativeControlType.Button ),
            ( "cc3e8e90f86a174cc22a27b7f8661f53f94b5d21", NativeControlComponentType.ButtonSmall,  NativeControlType.Button ),
            ( "101d73f2d75f295756728ee1fbb2ba3ce33fee5e", NativeControlComponentType.ButtonLargeDark,  NativeControlType.Button),
            ( "4a606f23d957d508b11707faba119cbd7edf4565", NativeControlComponentType.ButtonStandardDark,  NativeControlType.Button),
            ( "c252a811c880b9ab074d2ba63d3e279e7e3bad4a", NativeControlComponentType.ButtonSmallDark,  NativeControlType.Button),
            ( "65722325467a63cc1dcb0851cd7cdc1bb3b8bd7c", NativeControlComponentType.ButtonStandardDark,  NativeControlType.Button),

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
            ( "be9ae04fb622ea797dc9659e6d3f05987b8567c2", NativeControlComponentType.RadioStandard, NativeControlType.RadioButton),

            ( "92f3aa0d8397ca5f0bf76e818e2300152394836a", NativeControlComponentType.RadioSingleStandard, NativeControlType.RadioButton),

            ( "bea94829972c38261c31abceec35bb7cfd2ab440", NativeControlComponentType.CheckboxSmallDark, NativeControlType.CheckBox),
            ( "26e8dc2aa029ec68aa6c42d12935dadb85017bde", NativeControlComponentType.CheckboxStandardDark, NativeControlType.CheckBox),
            ( "0ad661c90900c3a974cf1dd819614fa4704ee4ad", NativeControlComponentType.CheckboxSmall, NativeControlType.CheckBox),
            ( "88c2e67236d56db9147ffa6f1034ba203b794393", NativeControlComponentType.CheckboxStandard, NativeControlType.CheckBox),

            ( "76ab0c7d6eab51ff9c4a4b0c1e356c45daaa8857", NativeControlComponentType.PopUpButtonSmall, NativeControlType.PopupButton),
            ( "f2f690e55f3ec52f312a18f3ce9a0f5c6ec9a516", NativeControlComponentType.PopUpButtonStandard , NativeControlType.PopupButton),
            ( "9bb29e36e1af3f4f93b3aecde65b22afa5fd2ce3", NativeControlComponentType.PopUpButtonSmallDark, NativeControlType.PopupButton),
            ( "b70a003c6523ae1058868643bd1b36170c2b7a1f", NativeControlComponentType.PopUpButtonStandardDark, NativeControlType.PopupButton),

            ( "3bc48ea321b06cfa5a05801c9d33d2f3e1b6095a", NativeControlComponentType.ComboBoxStandard, NativeControlType.ComboBox),
            ( "9d78fd2ff7b81a8cf56115504bab1bf8e3e413e9", NativeControlComponentType.ComboBoxSmall, NativeControlType.ComboBox),
            ( "cdb09381b061dddfe1f02fcad38a247b3724d562", NativeControlComponentType.ComboBoxStandardDark, NativeControlType.ComboBox),
            ( "97d0aa7c49386dd69f430b29bd82e89038e386d4", NativeControlComponentType.ComboBoxSmallDark, NativeControlType.ComboBox),

            ( "Progress Spinner/Small", NativeControlComponentType.ProgressSpinnerSmall, NativeControlType.ProgressSpinner),
            ( "Progress Spinner/Small Dark", NativeControlComponentType.ProgressSpinnerSmallDark, NativeControlType.ProgressSpinner),
            ( "Progress Spinner/Standard", NativeControlComponentType.ProgressSpinnerStandard, NativeControlType.ProgressSpinner),
            ( "Progress Spinner/Standard Dark", NativeControlComponentType.ProgressSpinnerStandardDark, NativeControlType.ProgressSpinner),

            ( "Window/Standard", NativeControlComponentType.WindowStandard, NativeControlType.WindowStandard),
            ( "Window/Standard Dark", NativeControlComponentType.WindowStandardDark, NativeControlType.WindowStandard),
            ( "Window/Sheet", NativeControlComponentType.WindowSheet, NativeControlType.WindowSheet),
            ( "Window/Sheet Dark", NativeControlComponentType.WindowSheetDark, NativeControlType.WindowSheet),
            ( "Window/Panel", NativeControlComponentType.WindowPanel, NativeControlType.WindowPanel),
            ( "Window/Panel Dark", NativeControlComponentType.WindowPanelDark, NativeControlType.WindowPanel),
        };

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
