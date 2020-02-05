using System;
using AppKit;
using FigmaSharp.Cocoa;
using FigmaSharp.Services;
using FigmaSharp.NativeControls;
using FigmaSharp.NativeControls.Cocoa;
using FigmaSharp;
using FigmaSharp.Models;

namespace FigmaSharp.NativeControls.Cocoa.Converters
{
    public class NativeControlsPropertyConverter : FigmaSharp.Cocoa.Converters.FigmaCodePropertyConverter
	{
        public override string ConvertToCode (string propertyName, FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
			if (currentNode.Node.Parent != null && propertyName == CodeProperties.AddChild) {
				//window case
				if (currentNode.Node.Parent.IsWindowContent () || currentNode.Node.Parent.IsDialogParentContainer (NativeControlType.WindowSheet) || currentNode.Node.Parent.IsDialogParentContainer (NativeControlType.WindowPanel)) {

                    var contentView = $"{CodeGenerationHelpers.This}.{nameof (NSWindow.ContentView)}";
                    return CodeGenerationHelpers.GetMethod (contentView, nameof (NSView.AddSubview), currentNode.Name);
                }

				if (currentNode.Node.Parent.IsMainDocumentView ()) {
                    return CodeGenerationHelpers.GetMethod (CodeGenerationHelpers.This, nameof (NSView.AddSubview), currentNode.Name);
                }

            }
            return base.ConvertToCode (propertyName, currentNode, parentNode, rendererService);
        }
    }
}
