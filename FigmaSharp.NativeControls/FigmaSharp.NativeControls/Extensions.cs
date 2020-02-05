using System;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using System.Linq;

namespace FigmaSharp
{
    public static class Extensions
    {
		public static void RenderInWindow(this FigmaViewRendererService sender, IWindow mainWindow,  params string[] path)
		{
			RenderInWindow(sender, mainWindow, new FigmaViewRendererServiceOptions(), path);
		}

		public static void RenderInWindow(this FigmaViewRendererService sender, IWindow mainWindow, FigmaViewRendererServiceOptions options, params string[] path)
        {
            var windowFigmaNode = sender.FileProvider.FindByName ("bundleFigmaDocument");
            var windowBoundingBox = (IAbsoluteBoundingBox)windowFigmaNode;

            //var contentPath = path.Concat(new[] { "content" }).ToArray();

            var contentFigmaNode = (IAbsoluteBoundingBox)sender.FileProvider.FindByName ("content \"page1\"");
            var contentView = sender.RenderByNode<IView>((FigmaNode)contentFigmaNode, options);

            mainWindow.Size = windowBoundingBox.absoluteBoundingBox.Size;

            var windowNodeContainer = (IFigmaNodeContainer)windowFigmaNode;
            var windowComponent = windowNodeContainer.children
                .OfType<FigmaInstance>()
                .FirstOrDefault(s => s.Component.key == "b666bac2b68d976c9e3e5b52a0956a9f1857c1f2");

            var windowLabel = windowComponent.children
                .OfType<FigmaText>()
                .FirstOrDefault();

            mainWindow.Title = windowLabel.characters;

            var difX = contentFigmaNode.absoluteBoundingBox.X - windowBoundingBox.absoluteBoundingBox.X;
            var difY = contentFigmaNode.absoluteBoundingBox.Y - windowBoundingBox.absoluteBoundingBox.Y;
            const int WindowTitleHeight = 20;
            difY -= WindowTitleHeight;

            mainWindow.Content.AddChild(contentView);
            contentView.SetPosition(difX, difY);
        }
    }
}
