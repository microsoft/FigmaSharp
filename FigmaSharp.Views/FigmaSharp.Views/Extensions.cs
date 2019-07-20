using System;
using System.Linq;

namespace FigmaSharp
{
	public static class Extensions
	{
		public static bool In (this string sender, params string[] values) => 
			values.Any (s => s == sender);
	}
}
