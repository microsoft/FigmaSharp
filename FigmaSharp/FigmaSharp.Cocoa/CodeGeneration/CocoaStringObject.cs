/* 
 * ElipseConverter.cs 
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Text;
using System.Linq;
using FigmaSharp.Cocoa.Helpers;

namespace FigmaSharp.Cocoa.CodeGeneration
{
    public class CocoaStringObject
    {
        public class CocoaStringObjectDraw
        {
            CocoaStringObject parent;

            internal CocoaStringObjectDraw (CocoaStringObject parent)
            {
                this.parent = parent;
            }

            public string ToCGPath() => string.Format("{0}.{1} ()", parent.Name, Members.Draw.ToCGPath);
        }

        public readonly CocoaStringObjectDraw Draw;

        StringBuilder builder = new StringBuilder();

        public string Name { get; }
        public Type ObjectType { get; }

        public CocoaStringObject(string name, Type type)
        {
            ObjectType = type;
            Name = name;
            builder = new StringBuilder(Name);
            Draw = new CocoaStringObjectDraw(this);
        }

        public CocoaStringObject AddChild(string child)
        {
            builder.Append(string.Format(".{0}", child));
            return this;
        }

        public CocoaStringObject AddArrayChild(string child, int index)
        {
            AddChild(string.Format("{0}[{1}]", child, index));
            return this;
        }

        public CocoaStringObject AddEnclose()
        {
            builder = new StringBuilder(string.Format("({0})", builder.ToString()));
            return this;
        }

        public CocoaStringObject AddCast(Type type)
        {
            builder = new StringBuilder(string.Format("({0}){1}", type.FullName, builder.ToString()));
            return this;
        }

        public CocoaStringObject AddMethod(string methodName, CocoaStringObject cocoaStringObject, bool inQuotes = false, bool includesSemicolon = true)
        {
            builder.Append(CodeGenerationHelpers.GetMethod(this.Name, methodName, cocoaStringObject.Name, inQuotes, includesSemicolon));
            return this;
        }

        public CocoaStringObject AddMethod (string methodName, string parameters, bool inQuotes = false, bool includesSemicolon = true)
        {
            builder.Append(CodeGenerationHelpers.GetMethod (this.Name, methodName, parameters, inQuotes, includesSemicolon));
            return this;
        }

        public virtual object Clone()
        {
            return new CocoaStringObject (Name, ObjectType);
        }

        public CocoaStringObject CreatePropertyStringObject (string propertyName, Type type)
        {
            return new CocoaStringObject(this.CreatePropertyName(propertyName), type);
        }

        public CocoaStringObject CreateChildStringObject(string propertyName, Type type)
        {
            return new CocoaStringObject(this.CreateChildObjectName(propertyName), type);
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}
