// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
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

using System;
using Gtk;
using MonoDevelop.Core;
using AppKit;
using MonoDevelop.Components.Mac;

namespace MonoDevelop.Figma
{
    class FigmaOptionsViewController : NSViewController
    {
        NSSecureTextField tokenEntry;

        public FigmaOptionsViewController()
        {
            var stackView = new NSStackView() {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Orientation = NSUserInterfaceLayoutOrientation.Vertical,
                Alignment = NSLayoutAttribute.Leading,
                Spacing = 10,
                Distribution = NSStackViewDistribution.Fill
            };

            View = stackView;

            var personalTokenContainer = new NSView () { TranslatesAutoresizingMaskIntoConstraints = false };
            stackView.AddArrangedSubview(personalTokenContainer);

            personalTokenContainer.LeadingAnchor.ConstraintEqualTo(stackView.LeadingAnchor).Active = true;
            personalTokenContainer.TrailingAnchor.ConstraintEqualTo(stackView.TrailingAnchor).Active = true;
            personalTokenContainer.HeightAnchor.ConstraintEqualTo(100).Active = true;

            var tokenHorizontalStack = new NSStackView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Orientation = NSUserInterfaceLayoutOrientation.Horizontal,
                Alignment = NSLayoutAttribute.CenterY,
                Spacing = 10,
                Distribution = NSStackViewDistribution.Fill,
            };

            personalTokenContainer.AddSubview (tokenHorizontalStack);
            tokenHorizontalStack.LeadingAnchor.ConstraintEqualTo(personalTokenContainer.LeadingAnchor).Active = true;
            tokenHorizontalStack.TrailingAnchor.ConstraintEqualTo(personalTokenContainer.TrailingAnchor).Active = true;
            tokenHorizontalStack.TopAnchor.ConstraintEqualTo(personalTokenContainer.TopAnchor).Active = true;

            var tokenLabel = new NSLabel()
            {
                StringValue = GettextCatalog.GetString("Personal Access Token:")
            };

            tokenHorizontalStack.AddArrangedSubview(tokenLabel);

            tokenEntry = new NSSecureTextField()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                StringValue = FigmaRuntime.Token
            };
            tokenHorizontalStack.AddArrangedSubview(tokenEntry);

            tokenEntry.WidthAnchor.ConstraintEqualTo(400).Active = true;
         
            var tokenTip = new NSLabel()
            {
                StringValue = GettextCatalog.GetString("Get your token from the Figma app:\n" +
                "Menu → Help and Account → Personal Access Tokens"),
                TextColor = NSColor.SecondaryLabelColor
            };

            personalTokenContainer.AddSubview(tokenTip);

            tokenTip.LeadingAnchor.ConstraintEqualTo(tokenEntry.LeadingAnchor).Active = true;
            tokenTip.TopAnchor.ConstraintEqualTo(tokenEntry.BottomAnchor).Active = true;

            tokenTip.WidthAnchor.ConstraintEqualTo(400).Active = true;

            tokenHorizontalStack.AddArrangedSubview(new NSView() { TranslatesAutoresizingMaskIntoConstraints =false });

            //TODO: Disabled for now
            //var convertersLayout = new HBox();
            //reloadButton = new Button() { Label = "Reload Converters" };
            //reloadButton.Activated += RefresButton_Activated;
            //convertersLayout.PackStart(reloadButton, false, false, 6);
            //PackStart(tokenLayout, false, false, 0);
            //PackStart(tokenTip, false, false, 6);
            //PackStart(new Label(""), true, true, 0);
            //PackStart(new Label("<b>Debugging</b>") { UseMarkup = true, Xalign = 0 }, false, false, 0);
            //PackStart(convertersLayout, false, false, 6);
            //ShowAll();
        }

        private void RefresButton_Activated(object sender, EventArgs e)
        {

        }

        internal void ApplyChanges()
        {
            FigmaRuntime.Token = tokenEntry.StringValue;
        }
    }
}
