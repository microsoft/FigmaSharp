using System;
using System.Collections.Generic;
using FigmaSharp.Models;

namespace FigmaSharp.NativeControls
{
    public enum NativeControlType
    {
        NotDefined = 0,

        ButtonStandard = 10,
        ButtonLarge = 12,
        ButtonSmall = 13,
        ButtonLargeDark = 14,
        ButtonStandardDark = 15,
        ButtonSmallDark = 16,

        TextFieldStandard = 20,
        TextFieldSmall = 21,
        TextFieldStandardDark = 22,
        TextFieldSmallDark = 23,

        FilterSmallDark = 30,
        FilterStandardDark =31,
        FilterSmall = 32,
        FilterStandard = 33,

        RadioSmallDark = 40,
        RadioStandardDark = 41,
        RadioSmall = 42,
        RadioStandard = 43,

        RadioSingleStandard = 46,

        CheckboxSmallDark = 50,
        CheckboxStandardDark = 51,
        CheckboxSmall = 52,
        CheckboxStandard = 53,

        PopUpButtonSmall = 60,
        PopUpButtonStandard = 61,
        PopUpButtonSmallDark = 62,
        PopUpButtonStandardDark = 63,

        ComboBoxStandard = 70,
        ComboBoxSmall = 71,
        ComboBoxStandardDark = 72,
        ComboBoxSmallDark = 73

    }

    public static class FigmaControlsExtension
    {
        static Dictionary<string, NativeControlType> data = new Dictionary<string, NativeControlType>()
        {
            { "65e801586bf62f77a4d79085aaeb6e2bbd8a48ea", NativeControlType.ButtonStandard },
            { "52a371ab5df2ad1fe9f7ae1d51712ec9dc493325", NativeControlType.ButtonLarge },
            { "cc3e8e90f86a174cc22a27b7f8661f53f94b5d21", NativeControlType.ButtonSmall },
            { "101d73f2d75f295756728ee1fbb2ba3ce33fee5e", NativeControlType.ButtonLargeDark },
            { "4a606f23d957d508b11707faba119cbd7edf4565", NativeControlType.ButtonStandardDark },
            { "c252a811c880b9ab074d2ba63d3e279e7e3bad4a", NativeControlType.ButtonSmallDark },

			{ "65722325467a63cc1dcb0851cd7cdc1bb3b8bd7c", NativeControlType.ButtonStandardDark }, //not sure why we have 2 instances

			{ "a31379072ea9b5b2ed501c707537df70e61e867d", NativeControlType.TextFieldStandard },
            { "6881fe7b35babca25147cb0cbadf401a266cda26", NativeControlType.TextFieldSmall },
            { "d0dfb49c121f3f2619096456026e91e224ac4248", NativeControlType.TextFieldStandardDark },
            { "646b4c57510bd605393f887599a1665d828216ef", NativeControlType.TextFieldSmallDark },

            { "92af27bcd788e4f3a291b8776adda4a2efc060ef", NativeControlType.FilterSmallDark },
            { "be1d8114b486b8e1fe1e8a01c7521e806d856c5d", NativeControlType.FilterStandardDark },
            { "64b610597e8a6ca2a54f66f6e810457d9d12c633", NativeControlType.FilterSmall },
            { "ebb7f47060094711b295e18af548a27df0a1c2ce", NativeControlType.FilterStandard },

            { "708b4ca16b3b72bfb1c9ec9a25071b74744f5cf1", NativeControlType.RadioSmallDark },
            { "844a988e2231ee8c794221e1a8b04522649caca4", NativeControlType.RadioStandardDark },
            { "b71c0bed2374cf42b3990fcff961b577d0d912b4", NativeControlType.RadioSmall },
            { "be9ae04fb622ea797dc9659e6d3f05987b8567c2", NativeControlType.RadioStandard },

            { "92f3aa0d8397ca5f0bf76e818e2300152394836a", NativeControlType.RadioSingleStandard },

            { "bea94829972c38261c31abceec35bb7cfd2ab440", NativeControlType.CheckboxSmallDark },
            { "26e8dc2aa029ec68aa6c42d12935dadb85017bde", NativeControlType.CheckboxStandardDark },
            { "0ad661c90900c3a974cf1dd819614fa4704ee4ad", NativeControlType.CheckboxSmall },
            { "88c2e67236d56db9147ffa6f1034ba203b794393", NativeControlType.CheckboxStandard },

            { "76ab0c7d6eab51ff9c4a4b0c1e356c45daaa8857", NativeControlType.PopUpButtonSmall },
            { "f2f690e55f3ec52f312a18f3ce9a0f5c6ec9a516", NativeControlType.PopUpButtonStandard },
            { "9bb29e36e1af3f4f93b3aecde65b22afa5fd2ce3", NativeControlType.PopUpButtonSmallDark },
            { "b70a003c6523ae1058868643bd1b36170c2b7a1f", NativeControlType.PopUpButtonStandardDark },

            { "3bc48ea321b06cfa5a05801c9d33d2f3e1b6095a", NativeControlType.ComboBoxStandard },
            { "9d78fd2ff7b81a8cf56115504bab1bf8e3e413e9", NativeControlType.ComboBoxSmall },
            { "cdb09381b061dddfe1f02fcad38a247b3724d562", NativeControlType.ComboBoxStandardDark },
            { "97d0aa7c49386dd69f430b29bd82e89038e386d4", NativeControlType.ComboBoxSmallDark },
        };

        public static NativeControlType ToControlType (this FigmaInstance figmaInstance)
        {
            if (figmaInstance.Component != null)
            {
                if (data.TryGetValue (figmaInstance.Component.key, out var nativeControls))
                {
                    return nativeControls;
                }
                Console.WriteLine("Component Key not found: {0} - {1}", figmaInstance.Component.key, figmaInstance.Component.name);
                //throw new KeyNotFoundException(figmaInstance.Component.key);
            }
            return NativeControlType.NotDefined;
        }
    }
}
