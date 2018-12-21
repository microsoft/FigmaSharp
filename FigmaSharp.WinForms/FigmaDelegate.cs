using FigmaSharp.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace FigmaSharp
{
    public class FigmaDelegate : IFigmaDelegate
    {
        static readonly FigmaViewConverter[] figmaViewConverters = {
            new FigmaVectorViewConverter (),
            new FigmaFrameEntityConverter (),
            new FigmaTextConverter (),
            new FigmaVectorEntityConverter (),
            new FigmaRectangleVectorConverter (),
            new FigmaElipseConverter (),
            new FigmaLineConverter ()
        };
        public FigmaDelegate()
        {
        }

        public FigmaViewConverter[] GetFigmaConverters()
        {
            return figmaViewConverters;
        }

        Image GetFromUrl (string url)
        {
            try {
                var request = WebRequest.Create (url);

                using (var response = request.GetResponse ())
                using (var stream = response.GetResponseStream ()) {
                    return Bitmap.FromStream (stream);
                }
            } catch (System.Exception ex) {
                Console.WriteLine (ex);
            }
            return null;
        }

        public IImageWrapper GetImage(string url)
        {
            var image = GetFromUrl (url);
            return new ImageWrapper (image);
        }

        public IImageWrapper GetImageFromFilePath(string filePath)
        {
            var image = Image.FromFile (filePath);
            return new ImageWrapper (image);
        }

        public IImageWrapper GetImageFromManifest(Assembly assembly, string imageRef)
        {
            var assemblyImage = FigmaViewsHelper.GetManifestImageResource (assembly, string.Format ("{0}.png", imageRef));
            return new ImageWrapper (assemblyImage);
        }

        public IImageViewWrapper GetImageView(FigmaPaint figmaPaint)
        {
            return new ImageViewWrapper (new PictureBox ()) {
                Data = figmaPaint
            };
        }


        public void LoadFigmaFromFrameEntity(IViewWrapper contentView, IFigmaDocumentContainer document, List<IImageViewWrapper> figmaImages, string figmaFileName)
        {

        }

        public string GetFigmaFileContent(string file, string token) =>
             FigmaApiHelper.GetFigmaFileContent (file, token);

        public string GetManifestResource(Assembly assembly, string file) =>
            FigmaApiHelper.GetManifestResource (assembly, file);

        public IFigmaDocumentContainer GetFigmaDialogFromContent(string template) =>
            FigmaApiHelper.GetFigmaDialogFromContent (template);

        public IViewWrapper CreateEmptyView() => new ViewWrapper ();
    }
}
