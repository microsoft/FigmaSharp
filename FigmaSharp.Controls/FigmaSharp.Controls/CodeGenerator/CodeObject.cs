using System.Text;

namespace FigmaSharp
{
    public abstract class CodeObject
    {
        public string Name { get; private set; }

        public CodeObject(string name)
        {
            Name = name;

        }

        public CodeObjectModifierTypes MethodModifier { get; set; } = CodeObjectModifierTypes.Private;

        abstract public void Write(FigmaClassBase figmaClassBase, StringBuilder sb);
    }
}
