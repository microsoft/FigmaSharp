using FigmaSharp.Models;

namespace FigmaSharp
{
    public class FigmaNodeView : Node
    {
        static string GetName(FigmaNode view)
        {
            var name = string.Format("[{2}] {0} ({1})", view.name, view.id ?? "N.I", view.type);
            if (!view.visible)
            {
                name += " (hidden)";
            }
            return name;
        }


        public readonly FigmaNode Wrapper;

        public FigmaNodeView(FigmaNode view) : base(GetName(view))
        {
            this.Wrapper = view;
        }
    }
}