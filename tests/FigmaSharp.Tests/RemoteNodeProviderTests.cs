// Authors:
//   jmedrano <josmed@microsoft.com>
//
// Copyright (C) 2020 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Linq;
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Services;
using NUnit.Framework;

namespace FigmaSharp.Tests
{
    [TestFixture]
    public class RemoteNodeProviderTests
    {
        [Test]
        public void NodeProviderTests()
        {
            AppContext.Api.Token = "6192-6b8c676e-270f-4fff-ada3-7b6dd1cfde6d";
            var provider = new ControlRemoteNodeProvider();
            provider.Load("IGskSVtfvnHGLEsl9buz0j");
            var images = provider.SearchImageNodes()
                .ToArray();
            Assert.AreEqual(2, images.Length);
        }
    }
}
