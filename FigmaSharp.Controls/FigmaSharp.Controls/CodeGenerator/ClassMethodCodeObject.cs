using System.Collections.Generic;
using System.Text;

namespace FigmaSharp
{
    public class ClassMethodCodeObject : CodeObject
    {
        public List<(string, string)> Args = new List<(string, string)>();

        public ClassMethodCodeObject(string name) : base(name)
        {

        }

        public override void Write(FigmaClassBase figmaClassBase, StringBuilder sb)
        {
            figmaClassBase.GenerateMethod(sb, Name, CodeObjectModifierTypes.Public);
            Write(figmaClassBase, sb);
            figmaClassBase.CloseBracket(sb);
        }
    }
}
