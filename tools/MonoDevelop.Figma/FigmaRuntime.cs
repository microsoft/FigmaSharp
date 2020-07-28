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
using MonoDevelop.Core;
using FigmaSharp.Cocoa;

namespace MonoDevelop.Figma
{
    static class FigmaRuntime
    {
        const string FigmaSetting = "FigmaToken";
        const string TokenMessage = "Cannot Access to TokenStore. Using default VS4Mac storage";

        internal static bool UsesTokenStore { get; private set; } = true;

        static FigmaRuntime ()
        {
#if DEBUG
            //in debug we don't want alert messages
            UsesTokenStore = false;
#endif
        }

        public static string Token
        {
            get
            {
                if (UsesTokenStore)
                {
                    try
                    {
                        return TokenStore.Current.GetToken();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(TokenMessage);
                        Console.WriteLine(ex.ToString());
                    }
                }
                return PropertyService.Get<string>(FigmaSetting) ?? string.Empty;
            }
            set
            {
                Exception exception = null;
                if (UsesTokenStore)
                {
                    try
                    {
                        TokenStore.Current.SetToken(value);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        Console.WriteLine(TokenMessage);
                        Console.WriteLine(exception.ToString());
                    }
                }

                if (!UsesTokenStore || exception != null)
                    PropertyService.Set (FigmaSetting, value);

                FigmaSharp.AppContext.Current.SetAccessToken(value);
                TokenChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static event EventHandler TokenChanged;
    }
}
