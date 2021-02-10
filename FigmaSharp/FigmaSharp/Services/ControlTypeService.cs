using System.Linq;
using System.Collections.Generic;

namespace FigmaSharp.Controls
{
    public enum FigmaControlType
    {
        NotDefined,

        //Buttons
        Button,

        //TextBoxes
        TextBox,

        //TextBlocks
        TextBlock,

        //Selections
        Checkbox,
        ComboBox,
        RadioButton,

        //Windows
        Window,
        WindowPanel
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
                ("Button", FigmaControlType.Button, NativeControlVariant.Default),

                //TextFields
                ("TextField", FigmaControlType.TextBox, NativeControlVariant.Regular),
                ("TextField", FigmaControlType.TextBox, NativeControlVariant.Default),

                //Labels
                ("TextBlock", FigmaControlType.TextBlock, NativeControlVariant.Regular),

                //Checkboxes
                ("Checkbox", FigmaControlType.Checkbox, NativeControlVariant.Regular),
                ("Checkbox", FigmaControlType.Checkbox, NativeControlVariant.Default),

                //ComboBoxes
                ("ComboBox", FigmaControlType.ComboBox, NativeControlVariant.Regular),
                ("ComboBox", FigmaControlType.ComboBox, NativeControlVariant.Default),

                //Radios
                ("RadioButton", FigmaControlType.RadioButton, NativeControlVariant.Regular),
                ("RadioButton", FigmaControlType.RadioButton, NativeControlVariant.Default)
            };
    }
}
