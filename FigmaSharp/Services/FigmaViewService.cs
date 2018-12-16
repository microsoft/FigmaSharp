/* 
 * FigmaViewExtensions.cs - Extension methods for NSViews
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
using System.Collections.Generic;
using System;

namespace FigmaSharp.Services
{
    public class FigmaViewService
    {
        readonly public List<CustomViewConverter> CustomConverters = new List<CustomViewConverter>();
        readonly FigmaViewConverter[] FigmaDefaultConverters;

        public FigmaViewService()
        {
            FigmaDefaultConverters = AppContext.Current.GetFigmaConverters();
        }

        public void Start()
        {

        }

        //TODO: This 
        IViewWrapper Recursively (FigmaNode currentNode, IViewWrapper parentView, FigmaNode parentNode)
        {
            Console.WriteLine("[{0}({1})] Processing {2}..", currentNode.id, currentNode.name, currentNode.GetType());
            IViewWrapper nextView = null;

            foreach (var customConverter in CustomConverters)
            {
                if (customConverter.CanConvert(currentNode))
                {
                    var view = customConverter.ConvertTo(currentNode, parentNode, parentView);
                    parentView.AddChild(view);
                    view.CreateConstraints(parentNode, parentView);
                    nextView = view;
                    break;
                }
            }

            if (nextView == null)
            {
                foreach (var converter in FigmaDefaultConverters)
                {
                    if (converter.CanConvert(currentNode))
                    {
                        var view = converter.ConvertTo(currentNode, parentNode, parentView);
                        if (view != null)
                        {
                            parentView.AddChild(view);
                            view.CreateConstraints(parentNode, parentView);
                            nextView = view;
                        }
                        break;
                    }
                }
            }

            Console.WriteLine("[{1}({2})] Not implemented: {0}", currentNode.GetType(), currentNode.id, currentNode.name);
            if (currentNode is IFigmaNodeContainer nodeContainer)
            {
                foreach (var item in nodeContainer.children)
                {
                    Recursively(parentNode, parentView, item);
                }
            }
            return nextView;
        }
    }
}
