using FigmaSharp.View.Graphics;
using FigmaSharp.Views.Helpers;
using NUnit.Framework;
using System;

namespace FigmaSharp.View.Tests
{
    [TestFixture()]
    public class Test
    {
        [Test()]
        public void TestCase()
        {
            var fileData = FileHelper.GetManifestResource (this.GetType().Assembly, "machine.svg");
            var fileService = Svg.FromData(fileData);
            Console.WriteLine("");
        }
    }
}
