namespace FigmaSharp.Services
{
    public class CodeRenderServiceOptions
    {
        public bool RendersConstructorFirstElement { get; set; }
        public bool TranslateLabels { get; set; }
        public bool ScanChildren { get; set; } = true;

        public bool ShowComments { get; set; } = true;
        public bool ShowAddChild { get; set; } = true;
        public bool ShowSize { get; set; } = true;
        public bool ShowConstraints { get; set; } = true;
    }
}
