/* 
 * FigmaViewExtensions.cs - Extension methods for NSViews
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
using System.Collections.Generic;

using FigmaSharp.Models;
using LiteForms;

namespace FigmaSharp
{
    public abstract class FigmaViewConverter : CustomViewConverter
    {
    }

    public abstract class FigmaInstanceConverter : FigmaViewConverter
    {
        public override bool ScanChildren(FigmaNode currentNode)
        {
            return false;
        }
    }

    public abstract class CustomViewConverter
    {
        public virtual bool IsLayer { get; }

        public virtual bool ScanChildren (FigmaNode currentNode)
        {
            return true;
            //return !(currentNode is FigmaInstance);
        }

        protected T ToEnum<T> (string value)
		{
			try {
				foreach (T suit in (T[])Enum.GetValues (typeof (T))) {
					if (suit.ToString ().ToLower ().Equals (value, StringComparison.InvariantCultureIgnoreCase)) {
						return suit;
					}
				}
			} catch (System.Exception ex) {
				Console.WriteLine (ex);

			}
			return default (T);
		}

		protected Dictionary<string, string> GetKeyValues (FigmaNode currentNode)
        {
            Dictionary<string, string> ids = new Dictionary<string, string>();
			var index = currentNode.name.IndexOf ($"type:", System.StringComparison.InvariantCultureIgnoreCase);
			if (index > -1) {
				var properties = currentNode.name.Split (' ');
				foreach (var property in properties) {
					var data = property.Split (':');
					if (data.Length != 2) {
						Console.WriteLine ($"Error format in parameter: '{property}'");
						continue;
					}
					ids.Add (data[0], data[1]);
				}
			} else {
				ids.Add ("type", currentNode.name);
			}
			return ids;
        }
     
        string GetIdentifierValue (string data, string parameter)
        {
            var index = data.IndexOf($"{parameter}:", System.StringComparison.InvariantCultureIgnoreCase);
            if (index > -1)
            {
                var delta = data.Substring(index + $"{parameter}=".Length);
                var endIndex = delta.IndexOf(" ", System.StringComparison.InvariantCultureIgnoreCase);

                if (endIndex == -1)
                    return delta;
                return delta.Substring(0, endIndex);
            }
			return null;
        }

        protected bool ContainsType (FigmaNode currentNode, string name)
        {
			var identifier = GetIdentifierValue (currentNode.name, "type") ?? currentNode.name;
			return identifier == name;
		}

        public abstract bool CanConvert (FigmaNode currentNode);

        public abstract IView ConvertTo(FigmaNode currentNode, ProcessedNode parent);

        public abstract string ConvertToCode(FigmaNode currentNode);
    }
}
