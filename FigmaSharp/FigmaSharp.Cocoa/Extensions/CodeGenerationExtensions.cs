using System;
using System.Text;

namespace FigmaSharp.Cocoa
{
	public static class CodeGenerationExtensions
	{
		//typeof (CoreGraphics.CGPoint).FullName
		public static string GetConstructor (this Type type, params string [] parameters)
		{
			string args;
			if (parameters.Length > 0) {
				args = string.Join (", ", parameters);
			} else {
				args = string.Empty;
			}
			return $"new {type.FullName} ({args})";
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

		public static string GetEquality (this Services.CodeNode sender, string propertyName, Enum value)
		{
			return GetEquality (sender, propertyName, value.GetFullName ());
		}

		public static string GetEquality (this Services.CodeNode sender, string propertyName, bool value)
		{
			return GetEquality (sender, propertyName, value.ToDesignerString ());
		}

		public static string GetEquality (this Services.CodeNode sender, string propertyName, string value, bool inQuotes = false)
		{
			return CodeGenerationHelpers.GetEquality (sender.Name, propertyName, value, inQuotes);
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

		public static void WriteEquality (this StringBuilder builder, string viewName, string propertyName, Enum value)
		{
			WriteEquality (builder, viewName, propertyName, value.GetFullName ());
		}

		public static void WriteEquality (this StringBuilder builder, string viewName, string propertyName, bool value)
		{
			WriteEquality (builder, viewName, propertyName, value.ToDesignerString ());
		}

		public static void WriteEquality (this StringBuilder builder, string viewName, string propertyName, string value, bool inQuotes = false, bool instanciate = false)
		{
			builder.AppendLine (CodeGenerationHelpers.GetEquality (viewName, propertyName, value, inQuotes, instanciate));
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
			WriteEquality (builder, $"{LayerName}.{viewName}", propertyName, value, inQuotes);
		}

		#endregion

	}
}
