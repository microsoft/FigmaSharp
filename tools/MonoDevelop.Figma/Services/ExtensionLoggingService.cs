// Authors:
//   Matt Ward <matt.ward@microsoft.com>
//
// Copyright (C) 2021 Microsoft, Corp
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
using FigmaSharp.Services;
using MonoDevelop.Core;

namespace MonoDevelop.Figma.Services
{
    static class ExtensionLoggingService
    {
        static ILogger logger;
        static readonly HashSet<ProgressMonitor> monitors = new HashSet<ProgressMonitor>();

        public static void Init()
        {
            logger = new CustomLogger();
            FigmaSharp.Services.LoggingService.RegisterDefaultLogger(logger);
        }

        class CustomLogger : FigmaSharp.Services.ILogger
        {
            public void Log(LogLevel level, string message)
            {
                lock (monitors)
                {
                    foreach (var monitor in monitors)
                    {
                        switch (level)
                        {
                            case LogLevel.Warn:
                                monitor.ReportWarning(message);
                                break;
                            case LogLevel.Error:
                            case LogLevel.Fatal:
                                monitor.ReportError(message);
                                break;
                            default:
                                monitor.Log.WriteLine(message);
                                break;
                        }
                    }

                    if (monitors.Count > 0)
                    {
                        return;
                    }
                }

                MonoDevelop.Core.LoggingService.Log((MonoDevelop.Core.Logging.LogLevel)level, "[FIGMA] " + message);
            }
        }

        internal static void AddProgressMonitor(ProgressMonitor monitor)
        {
            lock (monitors)
            {
                monitors.Add(monitor);
            }
        }

        internal static void RemoveProgressMonitor(ProgressMonitor monitor)
        {
            lock (monitors)
            {
                monitors.Remove(monitor);
            }
        }
    }
}
