/* 
 * CustomTextFieldConverter.cs
 * 
 * Author:
 *   Hylke Bons <hylbo@microsoft.com>
 *
 * Copyright (C) 2020 Microsoft, Corp
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


using System.Collections.Generic;

namespace FigmaSharp.NativeControls
{
    public enum NativeControlType
    {
        NotDefined,

        // Buttons
        Button,
        ButtonHelp,

        // Labels
        Label,
        LabelHeader,
        LabelGroup,
        LabelSecondary,

        // TextFields
        TextField,
        Stepper,
        SearchField,
        TextView,

        // Selections
        PopUp,
        PullDown,
        ComboBox,
        CheckBox,
        Radio,

        // Status
        ProgressIndicatorBar,
        ProgressIndicatorCircular,

        // Containers
        TabView,
        DisclosureView,
        Box,

        // Windows
        Window,
        WindowSheet,
        WindowPanel
    }

    public enum NativeControlVariant
    {
        NotDefined,

        Regular,
        Small
    }


    public static partial class FigmaControlsExtension
    {
        static IReadOnlyList<(string name, NativeControlType nativeControlType, NativeControlVariant nativeControlVariant)> controlsList = 
            new List<(string name, NativeControlType nativeControlType, NativeControlVariant nativeControlVariant)>
        {
            // Buttons
            ("Button",            NativeControlType.Button,     NativeControlVariant.Regular),
            ("Button Small",      NativeControlType.Button,     NativeControlVariant.Small),
            ("Button/Help",       NativeControlType.ButtonHelp, NativeControlVariant.Regular),
            ("Button/Help Small", NativeControlType.ButtonHelp, NativeControlVariant.Small),


            // Labels
            ("Label",                 NativeControlType.Label,          NativeControlVariant.Regular),
            ("Label Small",           NativeControlType.Label,          NativeControlVariant.Small),
            ("Label/Group",           NativeControlType.LabelGroup,     NativeControlVariant.Regular),
            ("Label/Group Small",     NativeControlType.LabelGroup,     NativeControlVariant.Small),
            ("Label/Header",          NativeControlType.LabelHeader,    NativeControlVariant.Regular),
            ("Label/Header Small",    NativeControlType.LabelHeader,    NativeControlVariant.Small),
            ("Label/Secondary",       NativeControlType.LabelSecondary, NativeControlVariant.Regular),
            ("Label/Secondary Small", NativeControlType.LabelSecondary, NativeControlVariant.Small),


            // TextFields
            ("TextField",       NativeControlType.TextField, NativeControlVariant.Regular),
            ("TextField Small", NativeControlType.TextField, NativeControlVariant.Small),

            ("Stepper",       NativeControlType.Stepper, NativeControlVariant.Regular),
            ("Stepper Small", NativeControlType.Stepper, NativeControlVariant.Small),

            ("TextView",       NativeControlType.TextView, NativeControlVariant.Regular),
            ("TextView Small", NativeControlType.TextView, NativeControlVariant.Small),

            ("SearchField",       NativeControlType.SearchField, NativeControlVariant.Regular),
            ("SearchField Small", NativeControlType.SearchField, NativeControlVariant.Small),


            // Selections
            ("Radio",       NativeControlType.Radio, NativeControlVariant.Regular),
            ("Radio Small", NativeControlType.Radio, NativeControlVariant.Small),

            ("Checkbox",       NativeControlType.CheckBox, NativeControlVariant.Regular),
            ("Checkbox Small", NativeControlType.CheckBox, NativeControlVariant.Small),

            ("PopUp Button",       NativeControlType.PopUp, NativeControlVariant.Regular),
            ("PopUp Button Small", NativeControlType.PopUp, NativeControlVariant.Small),

            ("PullDown Button",       NativeControlType.PullDown, NativeControlVariant.Regular),
            ("PullDown Button Small", NativeControlType.PullDown, NativeControlVariant.Small),

            ("ComboBox",       NativeControlType.ComboBox, NativeControlVariant.Regular),
            ("ComboBox Small", NativeControlType.ComboBox, NativeControlVariant.Small),


            // Status
            ("ProgressIndicator/Circular",       NativeControlType.ProgressIndicatorCircular, NativeControlVariant.Regular),
            ("ProgressIndicator/Circular Small", NativeControlType.ProgressIndicatorCircular, NativeControlVariant.Small),
            ("ProgressIndicator/Bar",            NativeControlType.ProgressIndicatorBar,      NativeControlVariant.Regular),
            ("ProgressIndicator/Bar Small",      NativeControlType.ProgressIndicatorBar,      NativeControlVariant.Small),


            // Containers
            ("TabView",        NativeControlType.TabView,        NativeControlVariant.Regular),
            ("DisclosureView", NativeControlType.DisclosureView, NativeControlVariant.Regular),
            ("Box",            NativeControlType.Box,            NativeControlVariant.Regular),


            // Windows
            ( "Window",       NativeControlType.Window,      NativeControlVariant.Regular),
            ( "Window/Sheet", NativeControlType.WindowSheet, NativeControlVariant.Regular),
            ( "Window/Panel", NativeControlType.WindowPanel, NativeControlVariant.Regular)
        };
    }
}
