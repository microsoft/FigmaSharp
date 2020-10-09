// Authors:
//   Jose Medrano <josmed@microsoft.com>
//   Hylke Bons <hylbo@microsoft.com>
//   Vaclav Vancura <vacvan@microsoft.com>
//
// Copyright (C) 2020 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using System.Collections.Generic;

namespace FigmaSharp.Controls
{
    public enum FigmaControlType
    {
        NotDefined,

        // Buttons
        Button,
        ButtonHelp,
        ButtonRoundRect,

        Stepper,
        SegmentedControl,

        // Labels
        Label,
        LabelHeader,
        LabelGroup,
        LabelSecondary,

        // TextFields
        TextField,
        TextView,
        SearchField,

        // Selections
        PopUpButton,
        PopUpButtonPullDown,
        ComboBox,
        CheckBox,
        Radio,
        ColorWell,
        Switch,

        // Status
        ProgressIndicatorBar,
        ProgressIndicatorCircular,
        SliderCircular,
        SliderLinear,

        // Containers
        TabView,
        DisclosureView,

        Box,
        BoxCustom,
        Separator,

        // Windows
        Window,
        WindowSheet,
        WindowPanel
    }

    public enum NativeControlVariant
    {
        NotDefined,

        Regular,
        Small,
        Large
    }

    public static class ControlTypeService
    {
        public static (string name, FigmaControlType nativeControlType, NativeControlVariant nativeControlVariant) GetByName(string name)
            => controlsList.FirstOrDefault(s => s.name == name);

        static IReadOnlyList<(string name, FigmaControlType nativeControlType, NativeControlVariant nativeControlVariant)> controlsList =
            new List<(string name, FigmaControlType nativeControlType, NativeControlVariant nativeControlVariant)>
        {
            // Buttons
            ("Button",       FigmaControlType.Button, NativeControlVariant.Regular),
            ("Button Small", FigmaControlType.Button, NativeControlVariant.Small),
            ("Button Large", FigmaControlType.Button,     NativeControlVariant.Large),

            ("Button/RoundRect",       FigmaControlType.ButtonRoundRect, NativeControlVariant.Regular),
            ("Button/RoundRect Small", FigmaControlType.ButtonRoundRect, NativeControlVariant.Small),
            ("Button/RoundRect Large", FigmaControlType.ButtonRoundRect, NativeControlVariant.Large),

            ("Button/Help",       FigmaControlType.ButtonHelp, NativeControlVariant.Regular),
            ("Button/Help Small", FigmaControlType.ButtonHelp, NativeControlVariant.Small),
            ("Button/Help Large", FigmaControlType.ButtonHelp, NativeControlVariant.Large),

            ("Stepper",       FigmaControlType.Stepper, NativeControlVariant.Regular),
            ("Stepper Small", FigmaControlType.Stepper, NativeControlVariant.Small),
            ("Stepper Large", FigmaControlType.Stepper, NativeControlVariant.Large),

            ("SegmentedControl",       FigmaControlType.SegmentedControl, NativeControlVariant.Regular),
            ("SegmentedControl Small", FigmaControlType.SegmentedControl, NativeControlVariant.Small),
            ("SegmentedControl Large", FigmaControlType.SegmentedControl, NativeControlVariant.Large),


            // Labels
            ("Label",                 FigmaControlType.Label,          NativeControlVariant.Regular),
            ("Label Small",           FigmaControlType.Label,          NativeControlVariant.Small),
            ("Label/Group",           FigmaControlType.LabelGroup,     NativeControlVariant.Regular),
            ("Label/Header",          FigmaControlType.LabelHeader,    NativeControlVariant.Regular),
            ("Label/Secondary",       FigmaControlType.LabelSecondary, NativeControlVariant.Regular),
            ("Label/Secondary Small", FigmaControlType.LabelSecondary, NativeControlVariant.Small),


            // TextFields
            ("TextField",       FigmaControlType.TextField, NativeControlVariant.Regular),
            ("TextField Small", FigmaControlType.TextField, NativeControlVariant.Small),
            ("TextField Large", FigmaControlType.TextField, NativeControlVariant.Large),

            ("TextView",       FigmaControlType.TextView, NativeControlVariant.Regular),
            ("TextView Small", FigmaControlType.TextView, NativeControlVariant.Small),
            ("TextView Large", FigmaControlType.TextView, NativeControlVariant.Large),

            ("SearchField",       FigmaControlType.SearchField, NativeControlVariant.Regular),
            ("SearchField Small", FigmaControlType.SearchField, NativeControlVariant.Small),
            ("SearchField Large", FigmaControlType.SearchField, NativeControlVariant.Large),


            // Selections
            ("Radio",       FigmaControlType.Radio, NativeControlVariant.Regular),
            ("Radio Small", FigmaControlType.Radio, NativeControlVariant.Small),
            ("Radio Large", FigmaControlType.Radio, NativeControlVariant.Large),

            ("Checkbox",       FigmaControlType.CheckBox, NativeControlVariant.Regular),
            ("Checkbox Small", FigmaControlType.CheckBox, NativeControlVariant.Small),
            ("Checkbox Large", FigmaControlType.CheckBox, NativeControlVariant.Large),

            ("PopUpButton",       FigmaControlType.PopUpButton, NativeControlVariant.Regular),
            ("PopUpButton Small", FigmaControlType.PopUpButton, NativeControlVariant.Small),
            ("PopUpButton Large", FigmaControlType.PopUpButton, NativeControlVariant.Large),

            ("PopUpButton/PullDown",       FigmaControlType.PopUpButtonPullDown, NativeControlVariant.Regular),
            ("PopUpButton/PullDown Small", FigmaControlType.PopUpButtonPullDown, NativeControlVariant.Small),
            ("PopUpButton/PullDown Large", FigmaControlType.PopUpButtonPullDown, NativeControlVariant.Large),

            ("ComboBox",       FigmaControlType.ComboBox, NativeControlVariant.Regular),
            ("ComboBox Small", FigmaControlType.ComboBox, NativeControlVariant.Small),
            ("ComboBox Large", FigmaControlType.ComboBox, NativeControlVariant.Large),

            ("Switch", FigmaControlType.Switch, NativeControlVariant.Regular),
            ("Switch Small", FigmaControlType.Switch, NativeControlVariant.Small),
            ("Switch Large", FigmaControlType.Switch, NativeControlVariant.Large),

            ("ColorWell", FigmaControlType.ColorWell, NativeControlVariant.Regular),


            // Status
            ("ProgressIndicator/Circular",       FigmaControlType.ProgressIndicatorCircular, NativeControlVariant.Regular),
            ("ProgressIndicator/Circular Small", FigmaControlType.ProgressIndicatorCircular, NativeControlVariant.Small),
            ("ProgressIndicator/Circular Large", FigmaControlType.ProgressIndicatorCircular, NativeControlVariant.Large),

            ("ProgressIndicator/Bar",            FigmaControlType.ProgressIndicatorBar,      NativeControlVariant.Regular),
            ("ProgressIndicator/Bar Small",      FigmaControlType.ProgressIndicatorBar,      NativeControlVariant.Small),
            ("ProgressIndicator/Bar Large",      FigmaControlType.ProgressIndicatorBar,      NativeControlVariant.Large),

            ("Slider/Linear/Horizontal",       FigmaControlType.SliderLinear,   NativeControlVariant.Regular),
            ("Slider/Linear/Horizontal Small", FigmaControlType.SliderLinear,   NativeControlVariant.Small),
            ("Slider/Linear/Horizontal Large", FigmaControlType.SliderLinear,   NativeControlVariant.Large),

            ("Slider/Linear/Vertical",         FigmaControlType.SliderLinear,   NativeControlVariant.Regular),
            ("Slider/Linear/Vertical Small",   FigmaControlType.SliderLinear,   NativeControlVariant.Small),
            ("Slider/Linear/Vertical Large",   FigmaControlType.SliderLinear,   NativeControlVariant.Large),

            ("Slider/Circular",                FigmaControlType.SliderCircular, NativeControlVariant.Regular),
            ("Slider/Circular Small",          FigmaControlType.SliderCircular, NativeControlVariant.Small),
            ("Slider/Circular Large",          FigmaControlType.SliderCircular, NativeControlVariant.Large),


            // Containers
            ("TabView",        FigmaControlType.TabView,        NativeControlVariant.Regular),
            ("TabView Small",  FigmaControlType.TabView,        NativeControlVariant.Small),

            ("DisclosureView", FigmaControlType.DisclosureView, NativeControlVariant.Regular),

            ("Box",                  FigmaControlType.Box,       NativeControlVariant.Regular),
            ("Box/Custom",           FigmaControlType.BoxCustom, NativeControlVariant.Regular),
            ("Separator/Vertical",   FigmaControlType.Separator, NativeControlVariant.Regular),
            ("Separator/Horizontal", FigmaControlType.Separator, NativeControlVariant.Regular),


            // Windows
            ( "Window",       FigmaControlType.Window,      NativeControlVariant.Regular),
            ( "Window/Sheet", FigmaControlType.WindowSheet, NativeControlVariant.Regular),
            ( "Window/Panel", FigmaControlType.WindowPanel, NativeControlVariant.Regular)
        };
    }
}
