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
using System.Linq;
using System.Windows.Controls;
using FigmaSharp.Controls;
using FigmaSharp.Converters;
using FigmaSharp.Extensions;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Wpf;
using System.Windows.Automation;
using System.Collections.Generic;

namespace FigmaSharp.Wpf.Converters
{
    class ComboBoxItem
    {
        public string Value { get; private set; }
        public ComboBoxItem(string _value)
        {
            Value = _value;
        }

    }

    public class ComboBoxConverter : FrameConverterBase
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(IComboBox);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);
            return controlType == FigmaControlType.ComboBox;
        }

        public override IView ConvertToView (FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var comboBox = new ComboBox();
            var comboBoxItems = new List<ComboBoxItem>();
            //comboBox.IsEditable = true;

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            switch (controlType)
            {
                case FigmaControlType.ComboBox:
                    // apply any styles here
                    break;
            }

            comboBox.Configure(frame);
            comboBox.ConfigureAutomationProperties(frame);
            comboBox.ConfigureTooltip(frame);
            comboBox.ConfigureTabIndex(frame);

            FigmaText text = frame.children
                    .OfType<FigmaText>()
                    .FirstOrDefault(s => s.name == ComponentString.TITLE);

            //Add comboBoxItems as the ItemsSource
            comboBoxItems.Add(new ComboBoxItem(text.characters));
            comboBox.ItemsSource = comboBoxItems;
            comboBox.DisplayMemberPath = "Value";
            comboBox.SelectedValue = comboBoxItems[0];

            comboBox.Foreground = text.fills[0].color.ToColor();
            comboBox.Foreground.Opacity = text.opacity;

            //TODO: investigate how to apply style to check box

            FigmaVector rect = frame.children
                    .OfType<FigmaVector>()
                    .FirstOrDefault(s => s.name == ComponentString.BACKGROUND);

            if (rect != null)
            {
                if (rect.fills.Length > 0)
                {
                    if (rect.fills[0].type == "SOLID")
                    {
                        comboBox.Background = rect.fills[0].color.ToColor();
                    }
                }

                comboBox.Background.Opacity = rect.opacity;
                if (rect.strokes.Length > 0)
                {
                    comboBox.BorderBrush = rect.strokes[0].color.ToColor();
                    comboBox.BorderThickness = new System.Windows.Thickness(rect.strokeWeight);
                }
                else
                {
                    comboBox.BorderThickness = new System.Windows.Thickness(0);
                }

            }

            //FigmaGroup group = frame.children
            //    .OfType<FigmaGroup>()
            //    .FirstOrDefault(s => s.name.In(ComponentString.STATE_ON,
            //                                    ComponentString.STATE_OFF) && s.visible);

            //if (group != null)
            //{
            //    if(group.name == ComponentString.STATE_ON)
            //    {
            //        comboBox.IsChecked = true;
            //    }
            //    if (group.name == ComponentString.STATE_OFF)
            //    {
            //        comboBox.IsChecked = false;
            //    }
            //}

            var wrapper = new View(comboBox);
            return wrapper;
        }
         
        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        } 
    }
}
