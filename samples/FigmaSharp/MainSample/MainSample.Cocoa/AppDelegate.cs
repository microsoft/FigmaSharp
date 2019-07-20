/* 
 * Author:
 *   Hylke Bons <hylbo@microsoft.com>
 *
 * Copyright (C) 2019 Microsoft, Corp
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

using AppKit;
using Foundation;

namespace FigmaSharp.Samples
{
    [Register("AppDelegate")]
    public partial class AppDelegate : NSApplicationDelegate
    {
        public AppDelegate()
        {
        }


        public override void DidFinishLaunching(NSNotification notification)
        {
            
        }


        public override void WillTerminate(NSNotification notification)
        {
        }


        string wiki_address = "https://github.com/netonjm/FigmaSharp/wiki";
        string api_documentation_address = "https://netonjm.github.io/FigmaSharp/api/";
        string source_code_address = "https://github.com/netonjm/FigmaSharp";

        partial void WikiClicked(NSObject sender)
        {
            var url = new NSUrl(wiki_address);
            NSWorkspace.SharedWorkspace.OpenUrl(url);
        }

        partial void APIDocumentationClicked(NSObject sender)
        {
            var url = new NSUrl(api_documentation_address);
            NSWorkspace.SharedWorkspace.OpenUrl(url);
        }

        partial void SourceCodeClicked(NSObject sender)
        {
            var url = new NSUrl(source_code_address);
            NSWorkspace.SharedWorkspace.OpenUrl(url);
        }
    }
}
