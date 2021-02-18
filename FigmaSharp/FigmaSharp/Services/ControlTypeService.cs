using System.Linq;
using System.Collections.Generic;

namespace FigmaSharp.Controls
{
    public enum FigmaControlType
    {
        NotDefined,

        //Buttons
        Button,
        Hyperlink,

        //TextBoxes
        TextBox,
        PasswordBox,

        //TextBlocks
        TextBlock,

        //Labels
        Label,

        //Selections
        Checkbox,
        ComboBox,
        RadioButton,

        //Sliders
        Slider,

        //ProgressBars
        ProgressBar,

        //Separators
        Separator,

        //TreeViews
        TreeView,

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
                ("Hyperlink", FigmaControlType.Hyperlink, NativeControlVariant.Regular),

                //TextBoxes
                ("TextBox", FigmaControlType.TextBox, NativeControlVariant.Regular),
                ("TextBox", FigmaControlType.TextBox, NativeControlVariant.Default),
                ("PasswordBox", FigmaControlType.PasswordBox, NativeControlVariant.Regular),

                //Labels
                ("TextBlock", FigmaControlType.TextBlock, NativeControlVariant.Regular),
                ("Label", FigmaControlType.Label, NativeControlVariant.Regular),

                //Checkboxes
                ("Checkbox", FigmaControlType.Checkbox, NativeControlVariant.Regular),
                ("Checkbox", FigmaControlType.Checkbox, NativeControlVariant.Default),

                //ComboBoxes
                ("ComboBox", FigmaControlType.ComboBox, NativeControlVariant.Regular),
                ("ComboBox", FigmaControlType.ComboBox, NativeControlVariant.Default),

                //Radios
                ("RadioButton", FigmaControlType.RadioButton, NativeControlVariant.Regular),
                ("RadioButton", FigmaControlType.RadioButton, NativeControlVariant.Default),

                //Sliders
                ("Slider/Horizontal", FigmaControlType.Slider, NativeControlVariant.Regular),
                ("Slider/Vertical", FigmaControlType.Slider, NativeControlVariant.Regular),

                //ProgressBars
                ("ProgressBar", FigmaControlType.ProgressBar, NativeControlVariant.Regular),
                ("ProgressBar", FigmaControlType.ProgressBar, NativeControlVariant.Default),

                //Separators
                ("Separator", FigmaControlType.Separator, NativeControlVariant.Regular),

                //TreeViews
                ("TreeView", FigmaControlType.TreeView, NativeControlVariant.Regular)
            };
    }
}
