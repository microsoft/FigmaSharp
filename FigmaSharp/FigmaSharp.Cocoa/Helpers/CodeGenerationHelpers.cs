using System;
using System.Linq;

namespace FigmaSharp.Cocoa
{
	public static class CodeGenerationHelpers
	{
		public const string This = "this";

		public static string StringEmpty { get; } = $"{typeof (string).Name}.{nameof (string.Empty)}";

		public static string GetConstructor (string viewName, Type type)
		{
			return GetConstructor (viewName, type.FullName);
		}

		public static string GetConstructor (string viewName, string typeFullName)
		{
			return $"var {viewName} = new {typeFullName}();";
		}

		public static string GetEquality (string viewName, string propertyName, Enum value)
		{
			return GetEquality (viewName, propertyName, value.GetFullName ());
		}

		public static string GetEquality (string viewName, string propertyName, bool value)
		{
			return GetEquality (viewName, propertyName, value.ToDesignerString ());
		}

		public static string GetEquality (string viewName, string propertyName, string value, bool inQuotes = false, bool instanciate = false)
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
	}
}
