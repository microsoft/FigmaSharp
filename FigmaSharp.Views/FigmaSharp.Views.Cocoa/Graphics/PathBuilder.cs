﻿// Authors:
//   James Clancey
//
// Copyright (C) 2020 Microsoft, Corp
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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

using CoreGraphics;

namespace FigmaSharp.Views.Cocoa.Graphics
{
	public class PathBuilder
	{
		public static CGPath Build(string definition)
		{
			if (string.IsNullOrEmpty(definition))
				return new CGPath();

			var pathBuilder = new PathBuilder();
			var path = pathBuilder.BuildPath(definition);
			return path;
		}

		private readonly Stack<string> _commandStack = new Stack<string>();
		private bool _closeWhenDone;
		private char _lastCommand = '~';

		private CGPoint? _lastCurveControlPoint;
		private CGPoint? _relativePoint;

		private CGPath _path;

		private float NextValue
		{
			get
			{
				var valueAsString = _commandStack.Pop();
				return ParseFloat(valueAsString);
			}
		}

		public static float ParseFloat(string value)
		{
			if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var number))
				return number;

			// Note: Illustrator will sometimes export numbers that look like "5.96.88", so we need to be able to handle them
			var split = value.Split('.');
			if (split.Length > 2)
				if (float.TryParse($"{split[0]}.{split[1]}", NumberStyles.Any, CultureInfo.InvariantCulture, out number))
					return number;

			var stringValue = GetNumbersOnly(value);
			if (float.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out number))
				return number;

			throw new Exception($"Error parsing {value} as a float.");
		}

		private static string GetNumbersOnly(string value)
		{
			var builder = new StringBuilder(value.Length);
			foreach (var c in value)
				if (char.IsDigit(c) || c == '.' || c == '-')
					builder.Append(c);

			return builder.ToString();
		}

		public CGPath BuildPath(string pathAsString)
		{
			try
			{
				_lastCommand = '~';
				_lastCurveControlPoint = null;
				_path = null;
				_commandStack.Clear();
				_relativePoint = new CGPoint(0, 0);
				_closeWhenDone = false;

				pathAsString = pathAsString.Replace("Infinity", "0");
				pathAsString = Regex.Replace(pathAsString, "([a-zA-Z])", " $1 ");
				pathAsString = pathAsString.Replace("-", " -");
				pathAsString = pathAsString.Replace(" E  -", "E-");
				pathAsString = pathAsString.Replace(" e  -", "e-");

				var args = pathAsString.Split(new[] { ' ', '\r', '\n', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
				for (var i = args.Length - 1; i >= 0; i--)
				{
					var entry = args[i];
					var c = entry[0];
					if (char.IsLetter(c))
					{
						if (entry.Length > 1)
						{
							entry = entry.Substring(1);
							if (char.IsLetter(entry[0]))
							{
								if (entry.Length > 1)
								{
									_commandStack.Push(entry.Substring(1));
								}

								_commandStack.Push(entry[0].ToString(CultureInfo.InvariantCulture));
							}
							else
							{
								_commandStack.Push(entry);
							}
						}

						_commandStack.Push(c.ToString(CultureInfo.InvariantCulture));
					}
					else
					{
						_commandStack.Push(entry);
					}
				}

				while (_commandStack.Count > 0)
				{
					if (_path == null)
					{
						_path = new CGPath();
					}

					var vCommand = _commandStack.Pop();
					HandleCommand(vCommand);
				}

				if (_path != null)
				{
					if (_closeWhenDone)
					{
						_path.CloseSubpath();
					}
				}
			}
			catch (Exception exc)
			{
				throw new Exception($"An error occurred parsing the path: {pathAsString}", exc);
			}

			return _path;
		}

		private void HandleCommand(string command)
		{
			var c = command[0];

			if (_lastCommand != '~' && (char.IsDigit(c) || c == '-'))
			{
				if (_lastCommand == 'M')
				{
					_commandStack.Push(command);
					HandleCommand('L');
				}
				else if (_lastCommand == 'm')
				{
					_commandStack.Push(command);
					HandleCommand('l');
				}
				else if (_lastCommand == 'L')
				{
					_commandStack.Push(command);
					HandleCommand('L');
				}
				else if (_lastCommand == 'l')
				{
					_commandStack.Push(command);
					HandleCommand('l');
				}
				else if (_lastCommand == 'H')
				{
					_commandStack.Push(command);
					HandleCommand('H');
				}
				else if (_lastCommand == 'h')
				{
					_commandStack.Push(command);
					HandleCommand('h');
				}
				else if (_lastCommand == 'V')
				{
					_commandStack.Push(command);
					HandleCommand('V');
				}
				else if (_lastCommand == 'v')
				{
					_commandStack.Push(command);
					HandleCommand('v');
				}
				else if (_lastCommand == 'C')
				{
					_commandStack.Push(command);
					HandleCommand('C');
				}
				else if (_lastCommand == 'c')
				{
					_commandStack.Push(command);
					HandleCommand('c');
				}
				else if (_lastCommand == 'S')
				{
					_commandStack.Push(command);
					HandleCommand('S');
				}
				else if (_lastCommand == 's')
				{
					_commandStack.Push(command);
					HandleCommand('s');
				}
				else if (_lastCommand == 'Q')
				{
					_commandStack.Push(command);
					HandleCommand('Q');
				}
				else if (_lastCommand == 'q')
				{
					_commandStack.Push(command);
					HandleCommand('q');
				}
				else if (_lastCommand == 'T')
				{
					_commandStack.Push(command);
					HandleCommand('T');
				}
				else if (_lastCommand == 't')
				{
					_commandStack.Push(command);
					HandleCommand('t');
				}
				else if (_lastCommand == 'A')
				{
					_commandStack.Push(command);
					HandleCommand('A');
				}
				else if (_lastCommand == 'a')
				{
					_commandStack.Push(command);
					HandleCommand('a');
				}
				else
				{
					Console.WriteLine("Don't know how to handle the path command: " + command);
				}
			}
			else
			{
				HandleCommand(c);
			}
		}

		private void HandleCommand(char command)
		{
			if (command == 'M')
			{
				MoveTo(false);
			}
			else if (command == 'm')
			{
				MoveTo(true);
				if (_lastCommand == '~')
				{
					command = 'm';
				}
			}
			else if (command == 'z' || command == 'Z')
			{
				ClosePath();
			}
			else if (command == 'L')
			{
				LineTo(false);
			}
			else if (command == 'l')
			{
				LineTo(true);
			}
			else if (command == 'Q')
			{
				QuadTo(false);
			}
			else if (command == 'q')
			{
				QuadTo(true);
			}
			else if (command == 'C')
			{
				CurveTo(false);
			}
			else if (command == 'c')
			{
				CurveTo(true);
			}
			else if (command == 'S')
			{
				SmoothCurveTo(false);
			}
			else if (command == 's')
			{
				SmoothCurveTo(true);
			}
			else if (command == 'A')
			{
				ArcTo(false);
			}
			else if (command == 'a')
			{
				ArcTo(true);
			}
			else if (command == 'H')
			{
				HorizontalLineTo(false);
			}
			else if (command == 'h')
			{
				HorizontalLineTo(true);
			}
			else if (command == 'V')
			{
				VerticalLineTo(false);
			}
			else if (command == 'v')
			{
				VerticalLineTo(true);
			}
			else
			{
				Console.WriteLine("Don't know how to handle the path command: " + command);
			}

			if (!(command == 'C' || command == 'c' || command == 's' || command == 'S'))
			{
				_lastCurveControlPoint = null;
			}

			_lastCommand = command;
		}

		private void ClosePath()
		{
			_path.CloseSubpath();
		}

		private void MoveTo(bool isRelative)
		{
			var point = NewPoint(NextValue, NextValue, isRelative, true);
			_path.MoveToPoint(point);
		}

		private void LineTo(bool isRelative)
		{
			var point = NewPoint(NextValue, NextValue, isRelative, true);
			_path.AddLineToPoint(point);
		}

		private void HorizontalLineTo(bool isRelative)
		{
			var point = NewHorizontalPoint(NextValue, isRelative, true);
			_path.AddLineToPoint(point);
		}

		private void VerticalLineTo(bool isRelative)
		{
			var point = NewVerticalPoint(NextValue, isRelative, true);
			_path.AddLineToPoint(point);
		}

		private void CurveTo(bool isRelative)
		{
			var point = NewPoint(NextValue, NextValue, isRelative, false);
			var x = NextValue;
			var y = NextValue;

			var isQuad = char.IsLetter(_commandStack.Peek()[0]);
			var point2 = NewPoint(x, y, isRelative, isQuad);

			if (isQuad)
			{
				_path.AddQuadCurveToPoint(point.X, point.Y, point2.X, point2.Y);
			}
			else
			{
				var point3 = NewPoint(NextValue, NextValue, isRelative, true);
				_path.AddCurveToPoint(point, point2, point3);
				_lastCurveControlPoint = point2;
			}
		}

		private void QuadTo(bool isRelative)
		{
			var point1 = NewPoint(NextValue, NextValue, isRelative, false);
			var x = NextValue;
			var y = NextValue;

			var point2 = NewPoint(x, y, isRelative, true);
			_path.AddQuadCurveToPoint(point1.X, point1.Y, point2.X, point2.Y);
		}

		private void SmoothCurveTo(bool isRelative)
		{
			var point1 = new CGPoint();
			var point2 = NewPoint(NextValue, NextValue, isRelative, false);

			// ReSharper disable ConvertIfStatementToNullCoalescingExpression
			if (_relativePoint != null)
			{
				if (_lastCurveControlPoint == null)
				{
					// ReSharper restore ConvertIfStatementToNullCoalescingExpression
					point1 = GetOppositePoint((CGPoint)_relativePoint, point2);
				}
				else if (_relativePoint != null)
				{
					point1 = GetOppositePoint((CGPoint)_relativePoint, (CGPoint)_lastCurveControlPoint);
				}
			}

			var point3 = NewPoint(NextValue, NextValue, isRelative, true);
			_path.AddCurveToPoint(point1, point2, point3);
			_lastCurveControlPoint = point2;
		}

		private void ArcTo(bool isRelative)
		{
			if (_commandStack.Count < 7)
				throw new Exception();
			var point1 = NewPoint(NextValue, NextValue, isRelative, false);
			var lol = NextValue;
			lol = NextValue;
			lol = NextValue;
			lol = NextValue;
			lol = NextValue;

			//_path.AddArcToPoint (point1.X, point1.X, point2.X, point2.Y, 1);
		}

		private CGPoint NewPoint(float x, float y, bool isRelative, bool isReference)
		{
			CGPoint point;

			if (isRelative && _relativePoint != null)
			{

				point = new CGPoint(((CGPoint)_relativePoint).X + x, ((CGPoint)_relativePoint).Y + y);
			}
			else
			{
				point = new CGPoint(x, y);
			}

			// If this is the reference point, we want to store the location before
			// we translate it into the final coordinates.  This way, future relative
			// points will start from an un-translated position.
			if (isReference)
			{
				_relativePoint = point;
			}

			return point;
		}

		private CGPoint NewVerticalPoint(float y, bool isRelative, bool isReference)
		{
			var point = new CGPoint();

			if (isRelative && _relativePoint != null)
			{
				point = new CGPoint(((CGPoint)_relativePoint).X, ((CGPoint)_relativePoint).Y + y);
			}
			else if (_relativePoint != null)
			{
				point = new CGPoint(((CGPoint)_relativePoint).X, y);
			}

			if (isReference)
				_relativePoint = point;

			return point;
		}

		private CGPoint NewHorizontalPoint(float x, bool isRelative, bool isReference)
		{
			var point = new CGPoint();

			if (isRelative && _relativePoint != null)
			{
				point = new CGPoint(((CGPoint)_relativePoint).X + x, ((CGPoint)_relativePoint).Y);
			}
			else if (_relativePoint != null)
			{
				point = new CGPoint(x, ((CGPoint)_relativePoint).Y);
			}

			if (isReference)
				_relativePoint = point;

			return point;
		}

		public static CGPoint GetOppositePoint(CGPoint pivot, CGPoint oppositePoint)
		{
			var dx = oppositePoint.X - pivot.X;
			var dy = oppositePoint.Y - pivot.Y;
			return new CGPoint(pivot.X - dx, pivot.Y - dy);
		}
	}
}
