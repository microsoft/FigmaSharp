using FigmaSharp.Services;
using FigmaSharp.Converters;

namespace Xamarin.Forms
{
    public class RemoteContentPage : BaseContentPage
    {
        public override void InitializeFigmaComponent()
        {
            InternalInitializeComponent();
            FileProvider = new RemoteNodeProvider();
            RendererService = new ViewRenderService(FileProvider, GetFigmaViewConverters());
        }

        protected override NodeConverter[] GetFigmaViewConverters()
        {
            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();
            return converters;
        }
    }
}
