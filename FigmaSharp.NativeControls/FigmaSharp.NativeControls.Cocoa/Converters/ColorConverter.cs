// Authors:
//   Jose Medrano <josmed@microsoft.com>
//   Hylke Bons <hylbo@microsoft.com>
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

using System.Collections.Generic;
using System.Linq;

using AppKit;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class ColorConverter
	{
		public ColorConverter()
		{
		}

		const string LIGHT_THEME_NAME    = "Light";
		const string LIGHT_HC_THEME_NAME = "Light HC";
		const string DARK_THEME_NAME     = "Dark;";
		const string DARK_HC_THEME_NAME  = "Dark HC";

		public NSColor GetThemeColor(string colorStyleName)
		{
			foreach (string themeName in new[] { LIGHT_THEME_NAME, LIGHT_HC_THEME_NAME, DARK_THEME_NAME, DARK_HC_THEME_NAME })
				colorStyleName = colorStyleName.Replace(string.Format("{0}/", themeName), string.Empty);

			return ThemeColors.FirstOrDefault(c => c.styleName == colorStyleName).themeColor;
		}

		public static IReadOnlyList<(string styleName, NSColor themeColor)> ThemeColors = new List<(string styleName, NSColor themeColor)>
		{
			// System color palette
			("System/Red", NSColor.SystemRedColor),
			("System/Green", NSColor.SystemGreenColor),
			("System/Blue", NSColor.SystemBlueColor),
			("System/Orange", NSColor.SystemOrangeColor),
			("System/Yellow", NSColor.SystemYellowColor),
			("System/Brown", NSColor.SystemBrownColor),
			("System/Pink", NSColor.SystemPinkColor),
			("System/Purple", NSColor.SystemPurpleColor),
			("System/Teal", NSColor.SystemTealColor),
			("System/Indigo", NSColor.SystemIndigoColor),
			("System/Gray", NSColor.SystemGrayColor),

			// Text
			("Text/Label", NSColor.LabelColor),
			("Text/Label Secondary", NSColor.SecondaryLabelColor),
			("Text/Label Tertiary", NSColor.TertiaryLabelColor),
			("Text/Label Quaternary", NSColor.QuaternaryLabelColor),
			("Text/Link", NSColor.LinkColor),
			("Text/Placeholder", NSColor.PlaceholderTextColor),
			("Text/Window Frame", NSColor.WindowFrameText),
			("Text/Menu Item Selected", NSColor.SelectedMenuItemText),
			("Text/Control Selected Alternate", NSColor.AlternateSelectedControlText),
			("Text/Header", NSColor.HeaderText),
			("Text/Text", NSColor.Text),
			("Text/Selected", NSColor.SelectedText),
			("Text/Selected Unemphasized", NSColor.UnemphasizedSelectedTextColor),
			("Text/Control", NSColor.ControlText),
			("Text/Control Selected", NSColor.SelectedControlText),
			("Text/Control Disabled", NSColor.DisabledControlText),

			// Chrome
			("Chrome/Separator", NSColor.SeparatorColor),
			("Chrome/Grid", NSColor.Grid),
			("Chrome/Text BG", NSColor.TextBackground),
			("Chrome/Text Selected BG", NSColor.SelectedTextBackground),
			("Chrome/Text Selected Unemphasized BG", NSColor.UnemphasizedSelectedTextBackgroundColor),
			("Chrome/Window BG", NSColor.WindowBackground),
			("Chrome/Under Page BG", NSColor.UnderPageBackgroundColor),
			("Chrome/Control BG", NSColor.ControlBackground),
			("Chrome/Content Selected BG", NSColor.SelectedContentBackgroundColor),
			("Chrome/Content Selected Unemphasized BG", NSColor.UnemphasizedSelectedContentBackgroundColor),
			("Chrome/Content Alternating BG", NSColor.AlternatingContentBackgroundColors[1]),
			("Chrome/Control", NSColor.Control),
			("Chrome/Control Selected", NSColor.SelectedControl),
			("Chrome/Control Accent", NSColor.ControlAccentColor),

			// Misc
			("Misc/Find Highlight", NSColor.FindHighlightColor),
			("Misc/Keyboard Focus Indicator", NSColor.KeyboardFocusIndicator),
		};
	}
}
