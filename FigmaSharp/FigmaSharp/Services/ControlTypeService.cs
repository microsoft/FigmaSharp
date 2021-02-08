using System.Linq;
using System.Collections.Generic;

namespace FigmaSharp.Controls
{
    public enum FigmaControlType
    {
        NotDefined,

        //Buttons
        Button
    }

    public enum NativeControlVariant
    {
        NotDefined,
        Regular,
        Default
    }
    class ControlTypeService
    {
        public static (string name, FigmaControlType nativeControlType, NativeControlVariant nativeControlVariant) GetByName(string name)
            => controlsList.FirstOrDefault(s => s.name == name);

        static IReadOnlyList<(string name, FigmaControlType nativeControlType, NativeControlVariant nativeControlVariant)> controlsList =
            new List<(string name, FigmaControlType nativeControlType, NativeControlVariant nativeControlVariant)>
            {
                //Buttons
                ("Button", FigmaControlType.Button, NativeControlVariant.Regular),
                ("Button", FigmaControlType.Button, NativeControlVariant.Default)
            };
    }
}
