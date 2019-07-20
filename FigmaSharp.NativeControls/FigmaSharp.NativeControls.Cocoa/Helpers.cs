using System;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class TransitionHelper
	{
		public static IButton CreateButtonFromFigmaNode(FNSButton button, FigmaNode currentNode)
		{
			IButton btn = null;
			if (currentNode is FigmaFrameEntity figmaFrameEntity)
			{
				if (!string.IsNullOrEmpty(figmaFrameEntity.transitionNodeID))
				{
					btn = new FigmaTransitionButton(button)
					{
						TransitionDuration = figmaFrameEntity.transitionDuration,
						TransitionEasing = figmaFrameEntity.transitionEasing,
						TransitionNodeID = figmaFrameEntity.transitionNodeID,
					};
				}
			}
			if (btn == null)
				btn = new Button(button);
			return btn;
		}

		public static IButton CreateButtonFromFigmaNode(FigmaNode currentNode)
		{
			IButton btn = null;
			if (currentNode is FigmaFrameEntity figmaFrameEntity)
			{
				if (!string.IsNullOrEmpty(figmaFrameEntity.transitionNodeID))
				{
					btn = new FigmaTransitionButton()
					{
						TransitionDuration = figmaFrameEntity.transitionDuration,
						TransitionEasing = figmaFrameEntity.transitionEasing,
						TransitionNodeID = figmaFrameEntity.transitionNodeID,
					};
				}
			}
			if (btn == null)
				btn = new Button();
			return btn;
		}

		public static IImageButton CreateImageButtonFromFigmaNode(FNSButton button, FigmaNode currentNode)
		{
			IImageButton btn = null;
			if (currentNode is FigmaFrameEntity figmaFrameEntity)
			{
				if (!string.IsNullOrEmpty(figmaFrameEntity.transitionNodeID))
				{
					btn = new FigmaTransitionImageButton(button)
					{
						TransitionDuration = figmaFrameEntity.transitionDuration,
						TransitionEasing = figmaFrameEntity.transitionEasing,
						TransitionNodeID = figmaFrameEntity.transitionNodeID,
					};
				}
			}
			if (btn == null)
				btn = new ImageButton(button);
			return btn;
		}

		public static IImageButton CreateImageButtonFromFigmaNode(FigmaNode currentNode)
		{
			IImageButton btn = null;
			if (currentNode is FigmaFrameEntity figmaFrameEntity)
			{
				if (!string.IsNullOrEmpty(figmaFrameEntity.transitionNodeID))
				{
					btn = new FigmaTransitionImageButton()
					{
						TransitionDuration = figmaFrameEntity.transitionDuration,
						TransitionEasing = figmaFrameEntity.transitionEasing,
						TransitionNodeID = figmaFrameEntity.transitionNodeID,
					};
				}
			}
			if (btn == null)
				btn = new ImageButton();
			return btn;
		}

	}
}
