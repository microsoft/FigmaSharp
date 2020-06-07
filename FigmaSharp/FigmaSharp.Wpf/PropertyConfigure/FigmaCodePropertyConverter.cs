using FigmaSharp.PropertyConfigure;
using FigmaSharp.Services;

namespace FigmaSharp.Wpf.PropertyConfigure
{
    public class CodePropertyConfigure : CodePropertyConfigureBase
    {
        public override string ConvertToCode(string propertyName, CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
        //    if (propertyName == CodeProperties.Frame)
        //    {

        //        if (currentNode.Node.Parent is FigmaCanvas)
        //            return string.Empty;

        //        if (currentNode.Node is IAbsoluteBoundingBox absoluteBounding && currentNode.Node.Parent is IAbsoluteBoundingBox parentAbsoluteBoundingBox)
        //        {
        //            var x = absoluteBounding.absoluteBoundingBox.X - parentAbsoluteBoundingBox.absoluteBoundingBox.X;

        //            var parentY = parentAbsoluteBoundingBox.absoluteBoundingBox.Y + parentAbsoluteBoundingBox.absoluteBoundingBox.Height;
        //            var actualY = absoluteBounding.absoluteBoundingBox.Y + absoluteBounding.absoluteBoundingBox.Height;
        //            var y = parentY - actualY;

        //            var rectangeConstructor = typeof(CoreGraphics.CGRect).GetConstructor(
        //new string[] {
        //                                x.ToDesignerString (),
        //                y.ToDesignerString (),
        //                                absoluteBounding.absoluteBoundingBox.Width.ToDesignerString (),
        //                                absoluteBounding.absoluteBoundingBox.Height.ToDesignerString ()
        //            });

        //            var getFrameForAlignmentRectMethod = currentNode.GetMethod(nameof(NSView.GetFrameForAlignmentRect), rectangeConstructor);
        //            currentNode.GetEquality(nameof(NSView.Frame), getFrameForAlignmentRectMethod);

        //            return currentNode.GetEquality(nameof(NSView.Frame), getFrameForAlignmentRectMethod);
        //        }
        //        return string.Empty;
        //    }
        //    if (propertyName == CodeProperties.AddChild)
        //    {
        //        return parentNode?.GetMethod(nameof(NSView.AddSubview), currentNode.Name);
        //    }
        //    if (propertyName == CodeProperties.Size)
        //    {

        //        if (currentNode.Node is IAbsoluteBoundingBox container)
        //        {

        //            if (currentNode.Node is FigmaLine line)
        //            {
        //                var width = container.absoluteBoundingBox.Width == 0 ? 1 : container.absoluteBoundingBox.Width;
        //                var height = container.absoluteBoundingBox.Height == 0 ? 1 : container.absoluteBoundingBox.Height;
        //                var size = typeof(CoreGraphics.CGSize).GetConstructor(new string[] { width.ToDesignerString(), height.ToDesignerString() });
        //                return currentNode.GetMethod(nameof(NSView.SetFrameSize), size);
        //            }

        //            var sizeConstructor = typeof(CoreGraphics.CGSize).GetConstructor(
        //             container.absoluteBoundingBox.Width.ToDesignerString(),
        //             container.absoluteBoundingBox.Height.ToDesignerString());
        //            return currentNode.GetMethod(nameof(NSView.SetFrameSize), sizeConstructor);

        //        }
        //        return string.Empty;
        //    }
        //    if (propertyName == CodeProperties.Position)
        //    {
        //        //first level has an special behaviour on positioning 
        //        if (currentNode.Node.Parent is FigmaCanvas)
        //            return string.Empty;

        //        if (currentNode.Node is IAbsoluteBoundingBox absoluteBounding && currentNode.Node.Parent is IAbsoluteBoundingBox parentAbsoluteBoundingBox)
        //        {
        //            var x = absoluteBounding.absoluteBoundingBox.X - parentAbsoluteBoundingBox.absoluteBoundingBox.X;

        //            var parentY = parentAbsoluteBoundingBox.absoluteBoundingBox.Y + parentAbsoluteBoundingBox.absoluteBoundingBox.Height;
        //            var actualY = absoluteBounding.absoluteBoundingBox.Y + absoluteBounding.absoluteBoundingBox.Height;
        //            var y = parentY - actualY;

        //            if (x != default || y != default)
        //            {
        //                var pointConstructor = CodeGenerationExtensions.GetConstructor(
        //                 typeof(CoreGraphics.CGPoint),
        //                 x.ToDesignerString(),
        //                 y.ToDesignerString()
        //            );
        //                return currentNode.GetMethod(nameof(AppKit.NSView.SetFrameOrigin), pointConstructor);
        //            }
        //        }
        //        return string.Empty;
        //    }

            throw new System.NotImplementedException(propertyName);
        }
    }
}
