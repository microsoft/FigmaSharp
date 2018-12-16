using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigmaSharp
{
    public class FigmaFile : IFigmaFile
    {
        string file;
        public List<IImageViewWrapper> FigmaImages { get; private set; }
        public IFigmaDocumentContainer Document { get; private set; }
        public IViewWrapper ContentView { get; private set; }

        public FigmaFile(string file)
        {
            this.file = file;
        }

        public void Reload(bool includeImages = false)
        {
            Console.WriteLine ($"Loading views..");

            if (includeImages) {
                ReloadImages ();
            }
        }

        public void ReloadImages()
        {
            Console.WriteLine ($"Loading images..");
            //if (FigmaImages != null && FigmaImages.Count > 0) {
            //    FigmaImages.LoadFromLocalImageResources ();
            //}
        }

        public void Initialize()
        {
            try {
                Console.WriteLine ($"Reading {file} from resources");
                var template = FigmaApiHelper.GetManifestResource (GetType ().Assembly, file);

                Document = FigmaApiHelper.GetFigmaDialogFromContent (template);
                Console.WriteLine ($"Reading successfull");

                FigmaImages = new List<IImageViewWrapper> ();
                //Reload ();
            } catch (Exception ex) {
                Console.WriteLine ($"Error reading resource");
                Console.WriteLine (ex);
            }
        }
    }
}
