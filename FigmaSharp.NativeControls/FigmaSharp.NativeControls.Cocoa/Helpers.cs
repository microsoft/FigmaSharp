﻿/* 
 * CustomTextFieldConverter.cs
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

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class NativeControlHelper
	{
		public static string GetTranslatableString(string text, bool needsTranslation = false)
		{
			if (needsTranslation)
				return $"TranslationCatalog.GetString (\"{text}\")";
			return text;
		}
	}

    public class TransitionHelper
	{
		public static IView CreateButtonFromFigmaNode(FNSButton button, FigmaNode currentNode)
		{
			IView btn = null;
			if (currentNode is FigmaFrame FigmaFrame)
			{
				if (!string.IsNullOrEmpty(FigmaFrame.transitionNodeID))
				{
					btn = new FigmaTransitionButton(button)
					{
						TransitionDuration = FigmaFrame.transitionDuration,
						TransitionEasing = FigmaFrame.transitionEasing,
						TransitionNodeID = FigmaFrame.transitionNodeID,
					};
				}
			}
			if (btn == null)
				btn = new View(button);
			return btn;
		}

		public static IView CreateButtonFromFigmaNode(FigmaNode currentNode)
		{
			IView btn = null;
			if (currentNode is FigmaFrame FigmaFrame)
			{
				if (!string.IsNullOrEmpty(FigmaFrame.transitionNodeID))
				{
					btn = new FigmaTransitionButton()
					{
						TransitionDuration = FigmaFrame.transitionDuration,
						TransitionEasing = FigmaFrame.transitionEasing,
						TransitionNodeID = FigmaFrame.transitionNodeID,
					};
				}
			}
			if (btn == null)
				btn = new View();
			return btn;
		}

		public static IView CreateImageButtonFromFigmaNode(FNSButton button, FigmaNode currentNode)
		{
			IView btn = null;
			if (currentNode is FigmaFrame FigmaFrame)
			{
				if (!string.IsNullOrEmpty(FigmaFrame.transitionNodeID))
				{
					btn = new FigmaTransitionImageButton(button)
					{
						TransitionDuration = FigmaFrame.transitionDuration,
						TransitionEasing = FigmaFrame.transitionEasing,
						TransitionNodeID = FigmaFrame.transitionNodeID,
					};
				}
			}
			if (btn == null)
				btn = new View(button);
			return btn;
		}

		public static IView CreateImageButtonFromFigmaNode(FigmaNode currentNode)
		{
			IView btn = null;
			if (currentNode is FigmaFrame FigmaFrame)
			{
				if (!string.IsNullOrEmpty(FigmaFrame.transitionNodeID))
				{
					btn = new FigmaTransitionImageButton()
					{
						TransitionDuration = FigmaFrame.transitionDuration,
						TransitionEasing = FigmaFrame.transitionEasing,
						TransitionNodeID = FigmaFrame.transitionNodeID,
					};
				}
			}
			if (btn == null)
				btn = new View();
			return btn;
		}

	}
}
