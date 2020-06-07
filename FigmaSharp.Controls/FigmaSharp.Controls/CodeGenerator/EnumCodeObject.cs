using System.Collections.Generic;
using System.Text;

namespace FigmaSharp
{
    public class EnumCodeObject : CodeObject
    {
        public List<Models.FigmaFrame> Values;

        public EnumCodeObject(string name, List<Models.FigmaFrame> values) : base(name)
        {
            Values = values;
        }

        public override void Write(FigmaClassBase figmaClassBase, StringBuilder sb)
        {
            figmaClassBase.AddTabLevel();
            figmaClassBase.GenerateEnum(sb, Name, CodeObjectModifierTypes.Public);
            for (int i = 0; i < Values.Count; i++)
            {
                var comma = (i < Values.Count - 1) ? "," : "";
                figmaClassBase.AppendLine(sb, $"{Values[i].GetClassName()}{comma}");
            }
            figmaClassBase.RemoveTabLevel();
            figmaClassBase.CloseBracket(sb);
        }
    }
}
