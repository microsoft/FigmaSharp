using System;
using System.Linq;

namespace FigmaSharp.Cocoa
{
	public static class CodeGenerationHelpers
	{
		//we need to move this into a better place than a Helper
		//we also work in a better approach in a more dynamic way

		#region Static Resources

		public const string This = "this";

        public static class Math
        {
			public static string Max (float min, float max)
            {
				return $"{typeof(Math).FullName}.{nameof(Math.Max)}({min.ToDesignerString()}, {max.ToDesignerString ()})";
			}
        }

		public static class Font
		{
			public static string SystemFontOfSize(string font)
			{
				return GetMethod(typeof(AppKit.NSFont).FullName, nameof(AppKit.NSFont.SystemFontOfSize), font);
			}

			public static string BoldSystemFontOfSize(string font)
			{
				return GetMethod(typeof(AppKit.NSFont).FullName, nameof(AppKit.NSFont.BoldSystemFontOfSize), font);
			}

			public static string SystemFontSize { get; } = $"{typeof(AppKit.NSFont).FullName}.{nameof(AppKit.NSFont.SystemFontSize)}";
			public static string SmallSystemFontSize { get; } = $"{typeof(AppKit.NSFont).FullName}.{nameof(AppKit.NSFont.SmallSystemFontSize)}";
		}

		public static string StringEmpty { get; } = $"{typeof(string).FullName}.{nameof(string.Empty)}";

		#endregion

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
			if (inQuotes) {
				var isMultiLine = value.Contains ('\n');
				//maybe we want to detect here if is multiline
				value = string.Format ("{0}\"{1}\"",
							isMultiLine ? "@" : "",
								isMultiLine ? value.Replace ("\"", "\"\"") : value);
			}

			var instanciateText = instanciate ? "var " : "";

			if (string.IsNullOrEmpty (propertyName))
				return $"{instanciateText}{viewName} = {value};";
			else
				return $"{instanciateText}{viewName}.{propertyName} = {value};";
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
			return $"{viewName}.{nameof(AppKit.NSView.WidthAnchor)}.{nameof(AppKit.NSView.WidthAnchor.ConstraintEqualToConstant)} ({value})";
		}

		public static string GetHeightConstraintEqualToConstant(string viewName, string value)
		{
			return $"{viewName}.{nameof(AppKit.NSView.HeightAnchor)}.{nameof(AppKit.NSView.HeightAnchor.ConstraintEqualToConstant)} ({value})";
		}

		public static string GetLeftConstraintEqualToAnchor(string firstViewName, float firstViewValue, string secondViewName)
		{
			return GetConstraintEqualToAnchor(firstViewName, nameof(AppKit.NSView.LeftAnchor), firstViewValue, secondViewName, nameof(AppKit.NSView.LeftAnchor));
		}

		public static string GetTopConstraintEqualToAnchor(string firstViewName, float firstViewValue, string secondViewName)
		{
			return GetConstraintEqualToAnchor(firstViewName, nameof(AppKit.NSView.TopAnchor), firstViewValue, secondViewName, nameof(AppKit.NSView.TopAnchor));
		}

		public static string GetBottomConstraintEqualToAnchor(string firstViewName, float firstViewValue, string secondViewName)
		{
			return GetConstraintEqualToAnchor(firstViewName, nameof(AppKit.NSView.BottomAnchor), firstViewValue, secondViewName, nameof(AppKit.NSView.BottomAnchor));
		}

		public static string GetRightConstraintEqualToAnchor(string firstViewName, float firstViewValue, string secondViewName)
		{
			return GetConstraintEqualToAnchor(firstViewName, nameof(AppKit.NSView.RightAnchor), firstViewValue, secondViewName, nameof(AppKit.NSView.RightAnchor));
		}


		public static string GetConstraintEqualToAnchor(string firstViewName, string firstAnchorPropertyName, float firstViewValue, string secondViewName, string secondAnchorPropertyName)
		{
			return $"{firstViewName}.{firstAnchorPropertyName}.{nameof(AppKit.NSView.TopAnchor.ConstraintEqualToAnchor)} ({secondViewName}.{secondAnchorPropertyName}, {firstViewValue}f)";
		}
	}
}
