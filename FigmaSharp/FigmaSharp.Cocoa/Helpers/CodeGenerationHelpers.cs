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

namespace FigmaSharp.Cocoa.Helpers
{
    public static class CodeGenerationHelpers
	{
		//we need to move this into a better place than a Helper
		//we also work in a better approach in a more dynamic way

		public static string GetConstructor (string viewName, Type type, bool includesVar = true)
		{
			return GetConstructor (viewName, type.FullName, includesVar);
		}

		public static string GetConstructor (string viewName, string typeFullName, bool includesVar = true)
		{
			return $"{(includesVar ? "var " : string.Empty)}{viewName} = new {typeFullName}();";
		}

		public static string GetPropertyEquality (string viewName, string propertyName, Enum value)
		{
			return GetPropertyEquality (viewName, propertyName, value.GetFullName ());
		}

		public static string GetPropertyEquality (string viewName, string propertyName, bool value)
		{
			return GetPropertyEquality (viewName, propertyName, value.ToDesignerString ());
		}

		public static string GetPropertyEquality (string viewName, string propertyName, string value, bool inQuotes = false, bool instanciate = false)
		{
			string fullPropertyName;
			if (string.IsNullOrEmpty (propertyName))
				fullPropertyName = viewName;
			else
				fullPropertyName = $"{viewName}.{propertyName}";
			return GetEquality(fullPropertyName, value, inQuotes, instanciate);
		}

		public static string GetEquality(string viewName, string value, bool inQuotes = false, bool instanciate = false)
		{
			if (inQuotes)
			{
				value = string.Format("\"{0}\"", value.Replace("\n", "\\n"));
			}

			var instanciateText = instanciate ? "var " : "";
			return $"{instanciateText}{viewName} = {value};";
		}

		public static string GetMethod (string viewName, string methodName, Enum parameter)
		{
			return GetMethod (viewName, methodName, parameter.GetFullName ());
		}

		public static string GetMethod (string viewName, string methodName, bool value)
		{
			return GetMethod (viewName, methodName, value.ToDesignerString ());
		}

		public static string GetMethod (string viewName, string methodName, string parameters, bool inQuotes = false, bool includesSemicolon = true)
		{
			parameters = inQuotes ? $"\"{parameters}\"" : parameters;
			var semicolon = includesSemicolon ? ";" : "";
			return $"{viewName}.{methodName} ({parameters}){semicolon}";
		}


		public static string GetWidthConstraintEqualToConstant(string viewName, string value)
		{
			return $"{viewName}.{nameof(AppKit.NSView.WidthAnchor)}.{nameof(AppKit.NSView.WidthAnchor.ConstraintEqualTo)} ({value})";
		}

		public static string GetHeightConstraintEqualToConstant(string viewName, string value)
		{
			return $"{viewName}.{nameof(AppKit.NSView.HeightAnchor)}.{nameof(AppKit.NSView.HeightAnchor.ConstraintEqualTo)} ({value})";
		}

		public static string GetLeadingConstraintEqualToAnchor(string firstViewName, float firstViewValue, string secondViewName)
		{
			return GetConstraintEqualToAnchor(firstViewName, nameof(AppKit.NSView.LeadingAnchor), firstViewValue, secondViewName, nameof(AppKit.NSView.LeadingAnchor));
		}

		public static string GetTopConstraintEqualToAnchor(string firstViewName, float firstViewValue, string secondViewName)
		{
			return GetConstraintEqualToAnchor(firstViewName, nameof(AppKit.NSView.TopAnchor), firstViewValue, secondViewName, nameof(AppKit.NSView.TopAnchor));
		}

		public static string GetBottomConstraintEqualToAnchor(string firstViewName, float firstViewValue, string secondViewName)
		{
			return GetConstraintEqualToAnchor(firstViewName, nameof(AppKit.NSView.BottomAnchor), firstViewValue, secondViewName, nameof(AppKit.NSView.BottomAnchor));
		}

		public static string GetTrailingConstraintEqualToAnchor(string firstViewName, float firstViewValue, string secondViewName)
		{
			return GetConstraintEqualToAnchor(firstViewName, nameof(AppKit.NSView.TrailingAnchor), firstViewValue, secondViewName, nameof(AppKit.NSView.TrailingAnchor));
		}

		public static string GetConstraintEqualToAnchor(string firstViewName, string firstAnchorPropertyName, float firstViewValue, string secondViewName, string secondAnchorPropertyName)
		{
			return $"{firstViewName}.{firstAnchorPropertyName}.{nameof(AppKit.NSView.TopAnchor.ConstraintEqualTo)} ({secondViewName}.{secondAnchorPropertyName}, {firstViewValue}f)";
		}
	}
}
