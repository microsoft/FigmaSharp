using FigmaSharp.Services;
using System.Reflection;
using FigmaSharp.Converters;

namespace Xamarin.Forms
{
    public class AssemblyResourceContentPage : BaseContentPage
    {
        public override void InitializeFigmaComponent()
        {
            InternalInitializeComponent();
            FileProvider = new AssemblyResourceNodeProvider(Assembly.GetCallingAssembly(), "");
            RendererService = new ViewRenderService(FileProvider, GetFigmaViewConverters());
        }

        protected override NodeConverter[] GetFigmaViewConverters()
        {
            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();
            return converters;
        }
    }
}
