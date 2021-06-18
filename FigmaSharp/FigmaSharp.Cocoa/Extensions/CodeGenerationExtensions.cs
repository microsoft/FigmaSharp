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

using FigmaSharp.Cocoa.CodeGeneration;
using FigmaSharp.Cocoa.Helpers;
using FigmaSharp.Models;
using FigmaSharp.Services;

namespace FigmaSharp.Cocoa
{
	public static class CodeGenerationExtensions
	{
		public static string GetFullProperty (this Enum enumeration, string property)
		{
			return enumeration.GetType().WithProperty(property.ToCamelCase());
		}

		public static string WithProperty(this Type type, string property)
		{
			return string.Format("{0}.{1}", type.FullName, property);
		}

		public static string CreatePropertyName(this Services.CodeNode sender, string propertyName)
		{
			return string.Format("{0}.{1}", sender.Name, propertyName);
		}

		public static string CreatePropertyName(this CocoaStringObject sender, string propertyName)
		{
			return string.Format("{0}.{1}", sender.Name, propertyName);
		}

		public static string CreateChildObjectName(this Services.CodeNode sender, string propertyName)
		{
			return string.Format("{0}{1}", sender.Name, propertyName);
		}

		public static string CreateChildObjectName(this CocoaStringObject sender, string propertyName)
		{
			return string.Format("{0}{1}", sender.Name, propertyName);
		}

		//typeof (CoreGraphics.CGPoint).FullName
		public static string GetConstructor(this Type type, params string[] parameters)
		{
			string args;
			if (parameters.Length > 0) {
				args = string.Join(", ", parameters);
			} else {
				args = string.Empty;
			}
			return $"new {type.FullName} ({args})";
		}

		public static string GetMethod(this Type type, string methodName, string parameters, bool inQuotes = false, bool includesSemicolon = true)
		{
			return CodeGenerationHelpers.GetMethod(type.FullName, methodName, parameters, inQuotes, includesSemicolon);
		}

		public static string GetFullName (this Enum myEnum)
		{
			return string.Format ("{0}.{1}", myEnum.GetType ().Name, myEnum.ToString ());
		}

		public static string GetConstructor (this Services.CodeNode sender, Type type, bool includesVar = true)
		{
			return GetConstructor (sender, type.FullName, includesVar);
		}

		public static string GetConstructor (this Services.CodeNode sender, string typeFullName, bool includesVar = true)
		{
			return CodeGenerationHelpers.GetConstructor (sender.Name, typeFullName, includesVar);
		}

		public static string GetPropertyEquality (this Services.CodeNode sender, string propertyName, Enum value)
		{
			return GetPropertyEquality (sender, propertyName, value.GetFullName ());
		}

		public static string GetPropertyEquality (this Services.CodeNode sender, string propertyName, bool value)
		{
			return GetPropertyEquality (sender, propertyName, value.ToDesignerString ());
		}

		public static string GetPropertyEquality (this Services.CodeNode sender, string propertyName, string value, bool inQuotes = false)
		{
			return CodeGenerationHelpers.GetPropertyEquality (sender.Name, propertyName, value, inQuotes);
		}

		public static string GetMethod (this Services.CodeNode sender, string methodName, Enum parameter)
		{
			return GetMethod (sender, methodName, parameter.GetFullName ());
		}

		public static string GetMethod (this Services.CodeNode sender, string methodName, bool parameter)
		{
			return CodeGenerationHelpers.GetMethod (sender.Name, methodName, parameter);
		}

		public static string GetMethod (this Services.CodeNode sender, string methodName, string parameters, bool inQuotes = false)
		{
			return CodeGenerationHelpers.GetMethod (sender.Name, methodName, parameters, inQuotes);
		}

		#region StringBuilder Code Generation

		public static void WriteConstructor (this StringBuilder builder, string viewName, Type type, bool includesVar = true)
		{
			WriteConstructor (builder, viewName, type.FullName, includesVar);
		}

		public static void WriteConstructor (this StringBuilder builder, string viewName, string typeFullName, bool includesVar = true)
		{
			builder.AppendLine (CodeGenerationHelpers.GetConstructor (viewName, typeFullName, includesVar));
		}

		public static void WritePropertyEquality (this StringBuilder builder, string viewName, string propertyName, Enum value)
		{
			WritePropertyEquality (builder, viewName, propertyName, value.GetFullName ());
		}

		public static void WriteEquality(this StringBuilder builder, string viewName, string value, bool inQuotes = false, bool instanciate = false)
		{
			builder.AppendLine(CodeGenerationHelpers.GetEquality (viewName, value, inQuotes, instanciate));
		}

		public static void WritePropertyEquality (this StringBuilder builder, string viewName, string propertyName, bool value)
		{
			WritePropertyEquality (builder, viewName, propertyName, value.ToDesignerString ());
		}

		public static void WritePropertyEquality (this StringBuilder builder, string viewName, string propertyName, string value, bool inQuotes = false, bool instanciate = false)
		{
			builder.AppendLine (CodeGenerationHelpers.GetPropertyEquality (viewName, propertyName, value, inQuotes, instanciate));
		}

		public static void WriteTranslatedEquality(this StringBuilder builder, string viewName, string propertyName, FigmaText value, ICodeRenderService codeRenderService,  bool instanciate = false)
		{
			WriteTranslatedEquality(builder, viewName, propertyName, value.characters, codeRenderService, instanciate, value.visible);
		}

		public static void WriteTranslatedEquality(this StringBuilder builder, string viewName, string propertyName, string value, ICodeRenderService codeRenderService, bool instanciate = false, bool textCondition = true)
		{
			bool needQuotes;
			string result;
			if (textCondition && !string.IsNullOrEmpty(value))
			{
				if (codeRenderService.Options != null && codeRenderService.Options.TranslateLabels)
                {
					result = codeRenderService.GetTranslatedText(value);
					needQuotes = false;
				} else
                {
					result = value;
					needQuotes = true;
				}
			}
			else
			{
				result = string.Empty;
				needQuotes = true;
			}
			builder.AppendLine(CodeGenerationHelpers.GetPropertyEquality(viewName, propertyName, result, inQuotes: needQuotes, instanciate: instanciate));
		}

		public static void WriteMethod (this StringBuilder builder, string viewName, string methodName, Enum parameter)
		{
			WriteMethod (builder, viewName, methodName, parameter.GetFullName ());
		}

		public static void WriteMethod (this StringBuilder builder, string viewName, string methodName, bool parameter)
		{
			WriteMethod (builder, viewName, methodName, parameter.ToDesignerString ());
		}

		public static void WriteMethod (this StringBuilder builder, string viewName, string methodName, string parameters, bool inQuotes = false)
		{
			builder.AppendLine (CodeGenerationHelpers.GetMethod (viewName, methodName, parameters, inQuotes));
		}

		#endregion

		#region Layer

		static string LayerName = nameof (AppKit.NSView.Layer);

		public static void WriteLayerEquality (this StringBuilder builder, string viewName, string propertyName, bool value)
		{
			WriteLayerEquality (builder, viewName, propertyName, value.ToDesignerString ());
		}

		public static void WriteLayerEquality (this StringBuilder builder, string viewName, string propertyName, Enum value)
		{
			WriteLayerEquality (builder, viewName, propertyName, value.GetFullName ());
		}

		public static void WriteLayerEquality (this StringBuilder builder, string viewName, string propertyName, string value, bool inQuotes = false)
		{
			WritePropertyEquality (builder, $"{LayerName}.{viewName}", propertyName, value, inQuotes);
		}

		#endregion

	}
}
