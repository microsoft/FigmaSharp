using FigmaSharp.Services;
using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using System.Linq;

namespace FigmaSharp.Tests
{

    [TestFixture()]
    public class ModulesTestsCocoa
    {

        static bool started;
        static void Init()
        {
            if (!started)
            {
                started = true;
                directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                directory = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(directory)), "nativecontrols", "cocoa");
                ModuleService.LoadModuleDirectory(directory);
                Assert.IsTrue(ModuleService.Converters.Count > 0);
            }
        }

        static string directory = "";

        [Test()]
        public void LoadModuleCocoa()
        {
            Init();
            var enumeratedFiles = Directory.EnumerateFiles(directory, "*.dll").ToArray();
            ModuleService.LoadModule(ModuleService.Platform.MAC, enumeratedFiles);

            Assert.IsTrue(ModuleService.Converters.Count > 0);
        }

        [Test()]
        public void LoadDirectoryCocoa()
        {
            Init();
            ModuleService.LoadModuleDirectory(directory);
            Assert.IsTrue(ModuleService.Converters.Count > 0);
        }
    }
}
