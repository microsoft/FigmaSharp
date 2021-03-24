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

using MonoDevelop.Core;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Figma.Services;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Figma
{
    static class IdeProgressMonitorManagerExtensions
    {
        public static ProgressMonitor GetFigmaProgressMonitor(
            this IdeProgressMonitorManager manager,
            string title,
            string successMessage = null)
        {
            var consoleMonitor = IdeApp.Workbench.ProgressMonitors.GetOutputProgressMonitor(
                "FigmaConsole",
                GettextCatalog.GetString("Figma Console"),
                Stock.Console,
                false,
                true);

            Pad pad = IdeApp.Workbench.ProgressMonitors.GetPadForMonitor(consoleMonitor);

            var statusMonitor = manager.GetStatusProgressMonitor(
                title,
                Stock.StatusSolutionOperation,
                false,
                true,
                false,
                pad);

            var monitor = new FigmaAggregatedProgressMonitor(consoleMonitor, successMessage);
            monitor.AddFollowerMonitor(statusMonitor);
            return monitor;
        }

        class FigmaAggregatedProgressMonitor : AggregatedProgressMonitor
        {
            readonly string successMessage;

            public FigmaAggregatedProgressMonitor(ProgressMonitor monitor, string successMessage)
                : base(monitor)
            {
                this.successMessage = successMessage;
                ExtensionLoggingService.AddProgressMonitor(this);
            }

            protected override void OnDispose(bool disposing)
            {
                if (!string.IsNullOrEmpty(successMessage) && HasErrors && !HasWarnings)
                {
                    ReportSuccess(successMessage);
                }

                ExtensionLoggingService.RemoveProgressMonitor(this);
                base.OnDispose(disposing);
            }
        }
    }
}
