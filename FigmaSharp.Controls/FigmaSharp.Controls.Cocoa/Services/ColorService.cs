// Authors:
//   Jose Medrano <josmed@microsoft.com>
//   Hylke Bons <hylbo@microsoft.com>
//
// Copyright (C) 2020 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software", nameof(NSColor.)), to deal in the Software without restriction, including
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

using AppKit;

namespace FigmaSharp.Controls.Cocoa.Services
{
	public static class ColorService
	{
		public static string GetNSColorString(string colorStyleName)
		{
			return $"{nameof(NSColor)}.{ThemeColors.FirstOrDefault(c => c.StyleName == colorStyleName).ColorName}";
		}

		public static NSColor GetNSColor(string colorStyleName)
		{
			return ThemeColors.FirstOrDefault(c => c.StyleName == colorStyleName).Color;
		}

		static readonly IReadOnlyList<(string StyleName, NSColor Color, string ColorName)> ThemeColors
		                   = new List<(string StyleName, NSColor Color, string ColorName)>
		{
			// System color palette
			("System/Red", NSColor.SystemRedColor, nameof(NSColor.SystemRedColor)),
			("System/Green", NSColor.SystemGreenColor, nameof(NSColor.SystemGreenColor)),
			("System/Blue", NSColor.SystemBlueColor, nameof(NSColor.SystemBlueColor)),
			("System/Orange", NSColor.SystemOrangeColor, nameof(NSColor.SystemOrangeColor)),
			("System/Yellow", NSColor.SystemYellowColor, nameof(NSColor.SystemYellowColor)),
			("System/Brown", NSColor.SystemBrownColor, nameof(NSColor.SystemBrownColor)),
			("System/Pink", NSColor.SystemPinkColor, nameof(NSColor.SystemPinkColor)),
			("System/Purple", NSColor.SystemPurpleColor, nameof(NSColor.SystemPurpleColor)),
			("System/Teal", NSColor.SystemTealColor, nameof(NSColor.SystemTealColor)),
			("System/Indigo", NSColor.SystemIndigoColor, nameof(NSColor.SystemIndigoColor)),
			("System/Gray", NSColor.SystemGrayColor, nameof(NSColor.SystemGrayColor)),

			// Text
			("Text/Label", NSColor.LabelColor, nameof(NSColor.LabelColor)),
			("Text/Label Secondary", NSColor.SecondaryLabelColor, nameof(NSColor.SecondaryLabelColor)),
			("Text/Label Tertiary", NSColor.TertiaryLabelColor, nameof(NSColor.TertiaryLabelColor)),
			("Text/Label Quaternary", NSColor.QuaternaryLabelColor, nameof(NSColor.QuaternaryLabelColor)),
			("Text/Link", NSColor.LinkColor, nameof(NSColor.LinkColor)),
			("Text/Placeholder", NSColor.PlaceholderTextColor, nameof(NSColor.PlaceholderTextColor)),
			("Text/Window Frame", NSColor.WindowFrameText, nameof(NSColor.WindowFrameText)),
			("Text/MenuItem Selected", NSColor.SelectedMenuItemText, nameof(NSColor.SelectedMenuItemText)),
			("Text/Control Selected Alternate", NSColor.AlternateSelectedControlText, nameof(NSColor.AlternateSelectedControlText)),
			("Text/Header", NSColor.HeaderText, nameof(NSColor.HeaderText)),
			("Text/Text", NSColor.Text, nameof(NSColor.Text)),
			("Text/Selected", NSColor.SelectedText, nameof(NSColor.SelectedText)),
			("Text/Selected Unemphasized", NSColor.UnemphasizedSelectedTextColor, nameof(NSColor.UnemphasizedSelectedTextColor)),
			("Text/Control", NSColor.ControlText, nameof(NSColor.ControlText)),
			("Text/Control Selected", NSColor.SelectedControlText, nameof(NSColor.SelectedControlText)),
			("Text/Control Disabled", NSColor.DisabledControlText, nameof(NSColor.DisabledControlText)),

			// Chrome
			("Chrome/Separator", NSColor.SeparatorColor, nameof(NSColor.SeparatorColor)),
			("Chrome/Grid", NSColor.Grid, nameof(NSColor.Grid)),
			("Chrome/Text BG", NSColor.TextBackground, nameof(NSColor.TextBackground)),
			("Chrome/Text Selected BG", NSColor.SelectedTextBackground, nameof(NSColor.SelectedTextBackground)),
			("Chrome/Text Selected Unemphasized BG", NSColor.UnemphasizedSelectedTextBackgroundColor, nameof(NSColor.UnemphasizedSelectedTextBackgroundColor)),
			("Chrome/Window BG", NSColor.WindowBackground, nameof(NSColor.WindowBackground)),
			("Chrome/Under Page BG", NSColor.UnderPageBackgroundColor, nameof(NSColor.UnderPageBackgroundColor)),
			("Chrome/Control BG", NSColor.ControlBackground, nameof(NSColor.ControlBackground)),
			("Chrome/Content Selected BG", NSColor.SelectedContentBackgroundColor, nameof(NSColor.SelectedContentBackgroundColor)),
			("Chrome/Content Selected Unemphasized BG", NSColor.UnemphasizedSelectedContentBackgroundColor, nameof(NSColor.UnemphasizedSelectedContentBackgroundColor)),
			("Chrome/Content Alternating BG", NSColor.AlternatingContentBackgroundColors[1], nameof(NSColor.AlternatingContentBackgroundColors) + "[1]"),
			("Chrome/Control", NSColor.Control, nameof(NSColor.Control)),
			("Chrome/Control Selected", NSColor.SelectedControl, nameof(NSColor.SelectedControl)),
			("Chrome/Control Accent", NSColor.ControlAccentColor, nameof(NSColor.ControlAccentColor)),

			// Misc
			("Misc/Find Highlight", NSColor.FindHighlightColor, nameof(NSColor.FindHighlightColor)),
			("Misc/Keyboard Focus Indicator", NSColor.KeyboardFocusIndicator, nameof(NSColor.KeyboardFocusIndicator)),
		};
	}
}
