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
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

using FigmaSharp.Models;
using FigmaSharp.Converters;
using System.IO;
using FigmaSharp.Views;

namespace FigmaSharp.Services
{
	public interface IFigmaFileProvider
	{
		string File { get; }
		bool NeedsImageLinks { get; }
		event EventHandler ImageLinksProcessed;
		List<FigmaNode> Nodes { get; }
		FigmaFileResponse Response { get; }
		void Load (string file);
		void Save (string filePath);
		string GetContentTemplate (string file);
		void OnStartImageLinkProcessing (List<ProcessedNode> imageVectors);

		FigmaNode FindByFullPath (string fullPath);
		FigmaNode FindByPath (params string[] path);
		FigmaNode FindByName (string nodeName);
	}

	public class FigmaLocalFileProvider : FigmaFileProvider
	{
		public FigmaLocalFileProvider (string resourcesDirectory)
		{
			ResourcesDirectory = resourcesDirectory;
		}

		public override string GetContentTemplate (string file)
		{
			return System.IO.File.ReadAllText (file);
		}

		public string ResourcesDirectory { get; set; }

		public string ImageFormat { get; set; } = ".png";

		public override void OnStartImageLinkProcessing (List<ProcessedNode> imageFigmaNodes)
		{
			//not needed in local files
			Console.WriteLine ($"Loading images..");

			if (imageFigmaNodes.Count > 0) {
				foreach (var vector in imageFigmaNodes) {
					try {
						var recoveredKey = FigmaResourceConverter.FromResource (vector.FigmaNode.id);
						string filePath = Path.Combine (ResourcesDirectory, string.Concat (recoveredKey, ImageFormat));

						if (!System.IO.File.Exists (filePath)) {
							throw new FileNotFoundException (filePath);
						}

						if (vector.View is IImageView imageView) {
							var image = AppContext.Current.GetImageFromFilePath (filePath);
							imageView.Image = image;
						}
					} catch (FileNotFoundException ex) {
						Console.WriteLine ("[FIGMA.RENDERER] Resource '{0}' not found.", ex.Message);
					} catch (Exception ex) {
						Console.WriteLine (ex);
					}
				}
			}

			Console.WriteLine ("Ended image link processing");
			OnImageLinkProcessed ();
		}
	}

	public class FigmaRemoteFileProvider : FigmaFileProvider
	{
		public override bool NeedsImageLinks => true;

		public override string GetContentTemplate (string file)
		{
			return AppContext.Api.GetContentFile (new FigmaFileQuery (file));
		}

		public IEnumerable<string> GetKeys (List<FigmaImageResponse> responses, string image)
		{
			foreach (var item in responses) {
				foreach (var keys in item.images.Where (s => s.Value == image)) {
					yield return keys.Key;
				}
			}
		}

		void ProcessRemoteImages (List<ProcessedNode> imageFigmaNodes, ImageQueryFormat imageFormat)
		{
			try {
				var totalImages = imageFigmaNodes.Count ();
				//TODO: figma url has a limited character in urls we fixed the limit to 10 ids's for each call
				var numberLoop = (totalImages / CallNumber) + 1;

				//var imageCache = new Dictionary<string, List<string>>();
				List<Tuple<string, List<string>>> imageCacheResponse = new List<Tuple<string, List<string>>> ();
				Console.WriteLine ("Detected a total of {0} possible {1} images.  ", totalImages, imageFormat);

				var images = new List<string> ();
				for (int i = 0; i < numberLoop; i++) {
					var vectors = imageFigmaNodes.Skip (i * CallNumber).Take (CallNumber);
					Console.WriteLine ("[{0}/{1}] Processing Images ... {2} ", i, numberLoop, vectors.Count ());
					var ids = vectors.Select (s => s.FigmaNode.id).ToArray ();
					var figmaImageResponse = AppContext.Api.GetImages (File, ids, imageFormat);
					if (figmaImageResponse != null) {
						foreach (var image in figmaImageResponse.images) {
							if (image.Value == null) {
								continue;
							}

							var img = imageCacheResponse.FirstOrDefault (s => image.Value == s.Item1);
							if (img?.Item1 != null) {
								img.Item2.Add (image.Key);
							} else {
								imageCacheResponse.Add (new Tuple<string, List<string>> (image.Value, new List<string> () { image.Key }));
							}
						}
					}
				}

				//get images not dupplicates
				Console.WriteLine ("Finished image to download {0}", images.Count);

				if (imageFormat == ImageQueryFormat.svg) {
					throw new NotImplementedException ("svg not implemented");
					//with all the keys now we get the dupplicated images
					//foreach (var imageUrl in imageCacheResponse)
					//{
					//	var image = FigmaApiHelper.GetUrlContent(imageUrl.Item1);

					//	foreach (var figmaNodeId in imageUrl.Item2)
					//	{
					//		var vector = imageFigmaNodes.FirstOrDefault(s => s.FigmaNode.id == figmaNodeId);
					//		Console.Write("[{0}:{1}:{2}] {3}...", vector.FigmaNode.GetType(), vector.FigmaNode.id, vector.FigmaNode.name, imageUrl);

					//		if (vector != null && vector.View is FigmaSharp.Graphics.ISvgShapeView imageView) {
					//			AppContext.Current.BeginInvoke(() => {
					//				imageView.Load(image);
					//			});
					//		}
					//		Console.Write("OK \n");
					//	}
					//}
				} else {
					//with all the keys now we get the dupplicated images
					foreach (var imageUrl in imageCacheResponse) {
						var Image = AppContext.Current.GetImage (imageUrl.Item1);
						foreach (var figmaNodeId in imageUrl.Item2) {
							var vector = imageFigmaNodes.FirstOrDefault (s => s.FigmaNode.id == figmaNodeId);
							Console.WriteLine ("[{0}:{1}:{2}] {3}...", vector.FigmaNode.GetType (), vector.FigmaNode.id, vector.FigmaNode.name, imageUrl);

							if (vector != null) {
								AppContext.Current.BeginInvoke (() => {
									if (vector.View is IImageView imageView) {
										imageView.Image = Image;
									} else if (vector.View is IImageButton imageButton) {
										imageButton.Image = Image;
									} else {
										Console.WriteLine ("[{0}:{1}:{2}] Error cannot assign the image to the current view {3}", vector.FigmaNode.GetType (), vector.FigmaNode.id, vector.FigmaNode.name, vector.View.GetType ().FullName);
									}
								});
							}
							Console.Write ("OK \n");
						}
					}
				}
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}
		}

		public override void OnStartImageLinkProcessing (List<ProcessedNode> imageFigmaNodes)
		{
			if (imageFigmaNodes.Count == 0) {
				OnImageLinkProcessed ();
				return;
			}

			Task.Run (() => {

				var images = imageFigmaNodes.ToList ();
				ProcessRemoteImages (images, ImageQueryFormat.png);

				OnImageLinkProcessed ();
			});
		}
		const int CallNumber = 250;
	}

	public class FigmaManifestFileProvider : FigmaFileProvider
	{
		public Assembly Assembly { get; set; }

		public FigmaManifestFileProvider (Assembly assembly, string file)
		{
			Assembly = assembly;
			File = file;
		}

		public override string GetContentTemplate (string file)
		{
			return AppContext.Current.GetManifestResource (Assembly, file);
		}

		public override void OnStartImageLinkProcessing (List<ProcessedNode> imageFigmaNodes)
		{
			Console.WriteLine ($"Loading images..");

			if (imageFigmaNodes.Count > 0) {
				foreach (var vector in imageFigmaNodes) {
					var recoveredKey = FigmaResourceConverter.FromResource (vector.FigmaNode.id);
					var image = AppContext.Current.GetImageFromManifest (Assembly, recoveredKey);
					if (image != null && vector.View is IImageView imageView) {
						imageView.Image = image;
					}
				}
			}

			Console.WriteLine ("Ended image link processing");
			OnImageLinkProcessed ();
		}
	}

	public abstract class FigmaFileProvider : IFigmaFileProvider
	{
		public virtual bool NeedsImageLinks => false;

		public event EventHandler ImageLinksProcessed;

		public FigmaFileResponse Response { get; protected set; }
		public List<FigmaNode> Nodes { get; } = new List<FigmaNode> ();

		public bool ImageProcessed;

		internal void OnImageLinkProcessed ()
		{
			ImageProcessed = true;
			ImageLinksProcessed?.Invoke (this, new EventArgs ());
		}

		public string File { get; set; }

		public void Load (string file)
		{
			this.File = file;

			ImageProcessed = false;
			try {
				Nodes.Clear ();

				var contentTemplate = GetContentTemplate (file);

				//parse the json into a model format
				Response = FigmaApiHelper.GetFigmaResponseFromFileContent (contentTemplate);

				//proceses all the views recursively
				foreach (var item in Response.document.children)
					ProcessNodeRecursively (item, null);
			} catch (System.Net.WebException ex) {
				if (!AppContext.Current.IsApiConfigured)
					Console.Error.WriteLine ($"Cannot connect to Figma server: TOKEN not configured.");
				else
					Console.Error.WriteLine ($"Cannot connect to Figma server: wrong TOKEN?");

				Console.WriteLine (ex);
			} catch (Exception ex) {
				Console.WriteLine ($"Error reading remote resources. Ensure you added NewtonSoft nuget or cannot parse the to json?");
				Console.WriteLine (ex);
			}
		}

		public FigmaNode FindByFullPath (string fullPath)
		{
			return FindByPath (fullPath.Split ('/'));
		}

		/// <summary>
		/// Finds a node using the path of the views, returns null in case of no data
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public FigmaNode FindByPath (params string[] path)
		{
			if (path.Length == 0) {
				return null;
			}

			FigmaNode figmaNode = null;
			for (int i = 0; i < path.Length; i++) {
				if (i == 0)
					figmaNode = Nodes.FirstOrDefault (s => s.name == path[i]);
				else
					figmaNode = Nodes.FirstOrDefault (s => s.name == path[i] && s.Parent.id == figmaNode.id);

				if (figmaNode == null)
					return null;
			}
			return figmaNode;
		}

		public FigmaNode FindByName (string name)
		{
			var quotedName = string.Format ("\"{0}\"", name);
			var found = Nodes.FirstOrDefault (s => s.name.Contains (quotedName));
			if (found != null) {
				return found;
			}
			return Nodes.FirstOrDefault (s => s.name == name);
		}

		void ProcessNodeRecursively (FigmaNode node, FigmaNode parent)
		{
			node.Parent = parent;
			Nodes.Add (node);

			if (node is FigmaInstance instance) {
				if (Response.components.TryGetValue (instance.componentId, out var figmaComponent))
					instance.Component = figmaComponent;
			}

			if (node is IFigmaNodeContainer nodeContainer) {
				foreach (var item in nodeContainer.children)
					ProcessNodeRecursively (item, node);
			}
		}

		public abstract string GetContentTemplate (string file);

		public abstract void OnStartImageLinkProcessing (List<ProcessedNode> imageFigmaNodes);

		public void Save (string filePath)
		{
			Response.Save (filePath);
		}
	}
}
