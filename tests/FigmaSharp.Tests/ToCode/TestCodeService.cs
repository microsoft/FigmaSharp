// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
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
using System.Text;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;

namespace FigmaSharp.Tests
{
    class CodeTestBase
    {
        public TestCodeService CodeService = new TestCodeService();

        bool TryGetValue (string[] data, string parameter, out string value, bool removeQuotes = true)
        {
            foreach (var line in data)
            {
                var split = line.Split('=');
                var param = split[0].Trim();
                var index = param.LastIndexOf('.');
                if (index > -1)
                {
                    var current = param.Substring (index + 1).Trim();
                    if (current == parameter)
                    {
                        if (removeQuotes)
                            value = split[1].Replace("\"", "");
                        else
                            value = split[1];

                        if (value.Length > 0 && value[value.Length - 1] == ';')
                            value = value.Substring(0, value.Length - 1);

                        value = value.Trim();
                        return true;
                    }
                    
                }
            }
            value = null;
            return false;
        }

        public void AssertEquals(string[] data, string parameter, string value)
        {
            if (TryGetValue(data, parameter, out var val))
            {
                if (val != value)
                {
                    throw new Exception($"the parameter '{parameter}' has value {val} but not '{value}'");
                }
                return;
            }
            throw new Exception($"Parameter '{parameter}' doesn't exists in code generated");
        }

        public void AssertEquals(string[] data, string parameter, bool value)
        {
            if (TryGetValue(data, parameter, out var val))
            {
                var expected = value.ToString().ToLower();
                if (val != expected)
                {
                    throw new Exception($"the parameter '{parameter}' has value {val} but not '{expected}'");
                }
                return;
            }
            throw new Exception($"Parameter '{parameter}' doesn't exists in code generated");
        }

        public void AssertEquals(string[] data, string parameter, Enum value)
        {
            if (TryGetValue(data, parameter, out var val))
            {
                var expected = value.GetFullName();
                if (val != expected)
                {
                    throw new Exception($"the parameter '{parameter}' has value {val} but not '{expected}'");
                }
                return;
            }
            throw new Exception($"Parameter '{parameter}' doesn't exists in code generated");
        }
    }

    class TestCodeService : ICodeRenderService
    {
        public CodeRenderServiceOptions Options { get; set; }

        public INodeProvider NodeProvider { get; set; }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void GetCode(StringBuilder builder, CodeNode node, CodeNode parent = null, CodeRenderServiceOptions currentRendererOptions = null, ITranslationService translateService = null)
        {
            throw new NotImplementedException();
        }

        public bool NeedsConstructor { get; set; } = true;

        public bool RendersVar { get; set; } = true;

        public string GetTranslatedText(FigmaText text)
        {
            return text.characters;
        }

        public string GetTranslatedText(string text, bool textCondition = true)
        {
            return text;
        }

        public bool NeedsRenderConstructor(CodeNode node, CodeNode parent)
        {
            return NeedsConstructor;
        }

        public bool NodeRendersVar(CodeNode currentNode, CodeNode parentNode)
        {
            return RendersVar;
        }
    }
}
