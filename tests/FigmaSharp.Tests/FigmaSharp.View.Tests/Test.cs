using FigmaSharp.Views.Graphics;
using FigmaSharp.Views.Helpers;
using NUnit.Framework;
using System;
using FigmaSharp.Views;
using System.Linq;
using ExCSS;

namespace FigmaSharp.View.Tests
{
    [TestFixture()]
    public class Test
    {
        [Test()]
        public void TestCase()
        {
            var fileData = FileHelper.GetManifestResource (this.GetType().Assembly, "cookie_monster.svg");
            var fileService = Svg.FromData(fileData);
            var lol = fileService.Style.GetStyleRule(".st1");
            var e = lol.Children.OfType<StyleDeclaration>().FirstOrDefault();

            var eeeeee = e.Fill.GetColor();
            Console.WriteLine("");
        }
    }
}
