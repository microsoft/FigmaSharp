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
using FigmaSharp.Services;

namespace FigmaSharp.Cocoa.CodeGeneration
{
    public class CocoaStringBuilder
    {
        StringBuilder builder = new StringBuilder();

        public void AppendLine(string value = "") => builder.AppendLine(value);

        public void WriteConstructor(CocoaStringObject cocoaNodeStringObject, bool includesVar = true)
           => builder.WriteConstructor(cocoaNodeStringObject.Name, cocoaNodeStringObject.ObjectType, includesVar);
        public void WriteConstructor(string viewName, Type type, bool includesVar = true)
            => builder.WriteConstructor(viewName, type, includesVar);
        public void WriteConstructor(string viewName, string typeFullName, bool includesVar = true)
          => builder.WriteConstructor(viewName, typeFullName, includesVar);

        public void WritePropertyEquality(string viewName, string propertyName, bool value)
            => builder.WritePropertyEquality(viewName, propertyName, value);
        public void WritePropertyEquality(CodeNode codeNode, string propertyName, bool value)
           => WritePropertyEquality(codeNode.Name, propertyName, value);
        public void WritePropertyEquality(CocoaStringObject codeNode, string propertyName, bool value)
         => WritePropertyEquality(codeNode.Name, propertyName, value);

        public void WritePropertyEquality(string viewName, string propertyName, string value, bool inQuotes = false, bool instanciate = false)
            => builder.WritePropertyEquality(viewName, propertyName, value, inQuotes, instanciate);
        public void WritePropertyEquality(CodeNode codeNode, string propertyName, string value, bool inQuotes = false, bool instanciate = false)
           => WritePropertyEquality(codeNode.Name, propertyName, value, inQuotes, instanciate);
        public void WritePropertyEquality(CocoaStringObject cocoaStringObject, string propertyName, string value, bool inQuotes = false, bool instanciate = false)
             => WritePropertyEquality(cocoaStringObject.Name, propertyName, value, inQuotes, instanciate);

        public void WritePropertyEquality(string viewName, string propertyName, Enum value)
            => builder.WritePropertyEquality(viewName, propertyName, value);
        public void WritePropertyEquality(CodeNode codeNode, string propertyName, Enum value)
            => builder.WritePropertyEquality(codeNode.Name, propertyName, value);

        public void WriteEquality(string viewName, string value, bool inQuotes = false, bool instanciate = false)
           => builder.WriteEquality (viewName, value, inQuotes, instanciate);

        public void WriteEquality(string viewName, CocoaStringObject value, bool inQuotes = false, bool instanciate = false)
       => builder.WriteEquality(viewName, value.Name, inQuotes, instanciate);

        public void WriteEquality(CocoaStringObject stringObject, string value, bool inQuotes = false, bool instanciate = false)
         => builder.WriteEquality(stringObject.Name, value, inQuotes, instanciate);

        public void WriteEquality(CocoaStringObject stringObject, CocoaStringObject value, bool inQuotes = false, bool instanciate = false)
        => builder.WriteEquality(stringObject.Name, value.Name, inQuotes, instanciate);

        public void WriteMethod(string viewName, string methodName, Enum parameter)
            => builder.WriteMethod(viewName, methodName, parameter);
        public void WriteMethod(string viewName, string methodName, bool parameter)
          => builder.WriteMethod(viewName, methodName, parameter);


        public void WriteMethod(string viewName, string methodName, string parameters, bool inQuotes = false)
          => builder.WriteMethod(viewName, methodName, parameters, inQuotes);
        public void WriteMethod(string viewName, string methodName, CocoaStringObject cocoaStringObject, bool inQuotes = false)
        => builder.WriteMethod(viewName, methodName, cocoaStringObject.Name, inQuotes);

        public override string ToString()
        {
            return builder.ToString();
        }

        public void WriteLine (CocoaStringObject viewLayer)
        {
            builder.AppendLine(viewLayer.ToString());
        }
    }
}
