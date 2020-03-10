// /* 
//  * Author:
//  *   netonjm <josmed@microsoft.com>
//  *
//  * Copyright (C) 2020 Microsoft, Corp
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining
//  * a copy of this software and associated documentation files (the
//  * "Software"), to deal in the Software without restriction, including
//  * without limitation the rights to use, copy, modify, merge, publish,
//  * distribute, sublicense, and/or sell copies of the Software, and to permit
//  * persons to whom the Software is furnished to do so, subject to the
//  * following conditions:
//  * 
//  * The above copyright notice and this permission notice shall be included in
//  * all copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
//  * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
//  * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//  * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
//  * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
//  * USE OR OTHER DEALINGS IN THE SOFTWARE.
//  */
//
//
using System;
using MonoDevelop.Core.Serialization;
using MonoDevelop.Projects;

namespace MonoDevelop.Figma
{
	[DataItem]
	[ExportProjectItemType("FigmaPackageReference")]
	class FigmaPackageReference : ProjectItem
	{
		//public FigmaPackageReference CreatePackageReference()
		//{
		//	var version = GetVersion();
		//	var identity = GetPackageIdentity(version);
		//	var framework = GetFramework();
		//	return CreatePackageReference(identity, version, framework);
		//}

		//FigmaPackageReference CreatePackageReference()
		//{
		//	//if (version != null && !version.IsFloating)
		//	//	version = null;

		//	return new FigmaPackageReference(
		//		identity, framework,
		//		userInstalled: true,
		//		developmentDependency: false,
		//		requireReinstallation: false,
		//		allowedVersions: version);
		//}

		[ItemProperty]
		public string Version { get; set; }

		[ItemProperty]
		public string FileId { get; set; }

		public FigmaPackageReference ()
		{
			//Include = id;
		}

		//internal void SetMetadataValue(string name, LibraryIncludeFlags flags)
		//{
		//	Metadata.SetMetadataValue(name, flags);
		//}
		//VersionRange GetVersion()
		//{
		//	string version = Metadata.GetValue("Version");
		//	if (string.IsNullOrEmpty(version))
		//		return null;

		//	return VersionRange.Parse(version);
		//}
	}
}
