using FigmaSharp.Services;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using System.Linq;
using FigmaSharp.Models;

namespace FigmaSharp.Tests
{
    [TestFixture()]
    public class ModulesTestsiOS
    {
        static bool started;
        static void Init ()
        {
            if (!started)
            {
                started = true;
                var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                directory = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(directory)), "nativecontrols", "ios");
                ModuleService.LoadModuleDirectory(directory);
                Assert.IsTrue(ModuleService.Converters.Count > 0);
            }
        }


        [Test()]
        public void LoadDirectoryiOS()
        {
            Init();
            var figmaNode = new FigmaNode() { id = "button", name = "button" };
            var button = ModuleService.Converters.FirstOrDefault(s => s.Platform == ModuleService.Platform.iOS && s.Converter.CanConvert(figmaNode));
            Assert.IsNotNull(button);
            //var code = button.Converter.ConvertToCode(figmaNode);
            //Assert.IsNotNull(code);
        }
    }
}
