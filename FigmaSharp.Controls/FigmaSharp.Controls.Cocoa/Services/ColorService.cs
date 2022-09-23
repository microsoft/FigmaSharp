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
			("Black", NSColor.Black, nameof(NSColor.Black)),
			("White", NSColor.White, nameof(NSColor.White)),

			// System color palette
			("System/Red", NSColor.SystemRed, nameof(NSColor.SystemRed)),
			("System/Green", NSColor.SystemGreen, nameof(NSColor.SystemGreen)),
			("System/Blue", NSColor.SystemBlue, nameof(NSColor.SystemBlue)),
			("System/Orange", NSColor.SystemOrange, nameof(NSColor.SystemOrange)),
			("System/Yellow", NSColor.SystemYellow, nameof(NSColor.SystemYellow)),
			("System/Brown", NSColor.SystemBrown, nameof(NSColor.SystemBrown)),
			("System/Pink", NSColor.SystemPink, nameof(NSColor.SystemPink)),
			("System/Purple", NSColor.SystemPurple, nameof(NSColor.SystemPurple)),
			("System/Teal", NSColor.SystemTeal, nameof(NSColor.SystemTeal)),
			("System/Indigo", NSColor.SystemIndigo, nameof(NSColor.SystemIndigo)),
			("System/Gray", NSColor.SystemGray, nameof(NSColor.SystemGray)),

			// Text
			("Text/Label", NSColor.Label, nameof(NSColor.Label)),
			("Text/Label Secondary", NSColor.SecondaryLabel, nameof(NSColor.SecondaryLabel)),
			("Text/Label Tertiary", NSColor.TertiaryLabel, nameof(NSColor.TertiaryLabel)),
			("Text/Label Quaternary", NSColor.QuaternaryLabel, nameof(NSColor.QuaternaryLabel)),
			("Text/Link", NSColor.Link, nameof(NSColor.Link)),
			("Text/Placeholder", NSColor.PlaceholderText, nameof(NSColor.PlaceholderText)),
			("Text/Window Frame", NSColor.WindowFrameText, nameof(NSColor.WindowFrameText)),
			("Text/MenuItem Selected", NSColor.SelectedMenuItemText, nameof(NSColor.SelectedMenuItemText)),
			("Text/Control Selected Alternate", NSColor.AlternateSelectedControlText, nameof(NSColor.AlternateSelectedControlText)),
			("Text/Header", NSColor.HeaderText, nameof(NSColor.HeaderText)),
			("Text/Text", NSColor.Text, nameof(NSColor.Text)),
			("Text/Selected", NSColor.SelectedText, nameof(NSColor.SelectedText)),
			("Text/Selected Unemphasized", NSColor.UnemphasizedSelectedText, nameof(NSColor.UnemphasizedSelectedText)),
			("Text/Control", NSColor.ControlText, nameof(NSColor.ControlText)),
			("Text/Control Selected", NSColor.SelectedControlText, nameof(NSColor.SelectedControlText)),
			("Text/Control Disabled", NSColor.DisabledControlText, nameof(NSColor.DisabledControlText)),

			// Chrome
			("Chrome/Separator", NSColor.Separator, nameof(NSColor.Separator)),
			("Chrome/Grid", NSColor.Grid, nameof(NSColor.Grid)),
			("Chrome/Text BG", NSColor.TextBackground, nameof(NSColor.TextBackground)),
			("Chrome/Text Selected BG", NSColor.SelectedTextBackground, nameof(NSColor.SelectedTextBackground)),
			("Chrome/Text Selected Unemphasized BG", NSColor.UnemphasizedSelectedTextBackground, nameof(NSColor.UnemphasizedSelectedTextBackground)),
			("Chrome/Window BG", NSColor.WindowBackground, nameof(NSColor.WindowBackground)),
			("Chrome/Under Page BG", NSColor.UnderPageBackground, nameof(NSColor.UnderPageBackground)),
			("Chrome/Control BG", NSColor.ControlBackground, nameof(NSColor.ControlBackground)),
			("Chrome/Content Selected BG", NSColor.SelectedContentBackground, nameof(NSColor.SelectedContentBackground)),
			("Chrome/Content Selected Unemphasized BG", NSColor.UnemphasizedSelectedContentBackground, nameof(NSColor.UnemphasizedSelectedContentBackground)),
			("Chrome/Content Alternating BG", NSColor.AlternatingContentBackgroundColors[1], nameof(NSColor.AlternatingContentBackgroundColors) + "[1]"),
			("Chrome/Control", NSColor.Control, nameof(NSColor.Control)),
			("Chrome/Control Selected", NSColor.SelectedControl, nameof(NSColor.SelectedControl)),
			("Chrome/Control Accent", NSColor.ControlAccent, nameof(NSColor.ControlAccent)),

			// Misc
			("Misc/Find Highlight", NSColor.FindHighlight, nameof(NSColor.FindHighlight)),
			("Misc/Keyboard Focus Indicator", NSColor.KeyboardFocusIndicator, nameof(NSColor.KeyboardFocusIndicator)),
		};
	}
}
