namespace FigmaSharp.Services
{
    public class RenderServiceOptions
    {
        public bool TranslateLabels { get; set; }
    }

    public class CodeRenderServiceOptions : RenderServiceOptions
    {
        public bool RendersConstructorFirstElement { get; set; }
        
        public bool ScanChildren { get; set; } = true;

        public bool ShowComments { get; set; } = true;
        public bool ShowAddChild { get; set; } = true;
        public bool ShowSize { get; set; } = true;
        public bool ShowConstraints { get; set; } = true;
    }
}
