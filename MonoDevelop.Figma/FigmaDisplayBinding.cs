using System;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Figma
{
    class FigmaDisplayBinding : IViewDisplayBinding
    {
        public FigmaDisplayBinding()
        {
        }

        public string Name
        {
            get
            {
                return "Figma Renderer";
            }
        }

        public bool CanUseAsDefault
        {
            get
            {
                return true;
            }
        }

        public bool CanHandle(FilePath fileName, string mimeType, Projects.Project ownerProject)
        {
            return fileName.IsNotNull && fileName.HasExtension(".figma");
        }

        public ViewContent CreateContent(FilePath fileName, string mimeType, Projects.Project ownerProject)
        {
            return new FigmaViewContent(fileName);
        }
    }
}