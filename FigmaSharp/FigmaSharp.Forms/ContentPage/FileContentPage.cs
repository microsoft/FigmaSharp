using FigmaSharp.Services;
using FigmaSharp.Converters;

namespace Xamarin.Forms
{
    public class FileContentPage : BaseContentPage
    {
        public override void InitializeFigmaComponent()
        {
            InternalInitializeComponent();
            FileProvider = new FileNodeProvider("Resources");
            RendererService = new ViewRenderService(FileProvider);
        }

        protected override NodeConverter[] GetFigmaViewConverters()
        {
            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();
            return converters;
        }
    }
}
