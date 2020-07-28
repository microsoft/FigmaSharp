﻿// Authors:
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
using System.ComponentModel;
using System.Linq;

using Xamarin.Forms;

using FigmaSharp;
using FigmaSharp.Forms;
using FigmaSharp.Services;

using LiteForms;
using LiteForms.Forms;

using ExampleFigma;

namespace BasicRendering.Forms
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : FigmaContentPage
    {
		const string LoginPage = "Log-In Page";
		const string ExhibitionPage = "Exhibition";
		const string HomePage = "Home";
		const string MenuPage = "Menu";

		public MainPage()
        {
            InitializeComponent();

			LoadDocument("fKugSkFGdwOF4vDsPGnJee");

			//this converters are reused in all the renderings
			AddConverter(new CustomButtonConverter(), new NavigationMenuButtonConverter());

			ProcessTransitionNodeID (StartNodeID);
		}

		void ProcessTransitionNodeID(string transitionNodeId)
		{
			var node = RendererService.FindNodeById(transitionNodeId);
			ProcessTransitionNode(node);
		}

		void ProcessTransitionNode(FigmaSharp.Models.FigmaNode node)
		{
			if (node == null)
				return;

			if (node.name == LoginPage)
				OnLoadLoginPage(node);
			else if (node.name == HomePage)
				OnLoadHomePage(node);
			else if (node.name == ExhibitionPage)
				OnLoadExhibitionPage(node);
			else if (node.name == MenuPage)
			{
				OnLoadMenuPage(node);
				ProcessAllTransitableButtons(ContentView);
			}
			else
			{
				BackgroundColor = Xamarin.Forms.Color.White;
				RenderByNode<IView>(node);
				ProcessAllTransitableButtons(ContentView);
			}
		}

		void RenderViewWithConverters (FigmaSharp.Models.FigmaNode node, params FigmaViewConverter[] converters)
		{
			if (converters != null && converters.Length > 0)
				AddConverter(converters);
			RenderByNode<IView>(node);
			if (converters != null && converters.Length > 0)
				RemoveConverter(converters);
		}

		void ProcessAllTransitableButtons(IView view)
		{
			if (view is ITransitableButton transitableButton)
			{
				transitableButton.Clicked += (s, e) => {
					if (s is IViewTransitable figmaTransition)
					{
						ProcessTransitionNodeID(figmaTransition.TransitionNodeID);
					}
				};
			}
			foreach (var child in view.Children)
				ProcessAllTransitableButtons(child);
		}

		void OnLoadLoginPage(FigmaSharp.Models.FigmaNode node)
		{
			//Background color
			SetValue(NavigationPage.BarBackgroundColorProperty, Xamarin.Forms.Color.Black);
			//Title color
			SetValue(NavigationPage.BarTextColorProperty, Xamarin.Forms.Color.White);
			BackgroundColor = Xamarin.Forms.Color.Black;

			//we add some temporal converters process all the textfields into real views
			RenderViewWithConverters(node, new LoginTextFieldConverter());

			//adds some logic to the current rendered views
			var loginButton = RendererService.FindViewByName<IButton>("LoginCustomButton");

			#region Logic code

			if (loginButton == null)
				return;

			loginButton.Clicked += (s, e) => {
				var emailTextField = FindNativeViewByName<Entry>("EmailTextField");
				var passwordTextField = FindNativeViewByName<Entry>("PasswordTextField");

				if (emailTextField.Text == "1234" && passwordTextField.Text == "1234")
				{
					if (s is IViewTransitable figmaTransition)
						ProcessTransitionNodeID(figmaTransition.TransitionNodeID);
				}
				else
				{
					DisplayAlert("Credentials error", "You have entered a wrong user or password", "OK");
				}
			};

			#endregion
		}

		void OnLoadHomePage(FigmaSharp.Models.FigmaNode node)
		{
			BackgroundColor = Xamarin.Forms.Color.White;

			RenderViewWithConverters(node);

			var navigationMenuButton = RendererService.FindViewByName<IButton>("HomeNavigationMenuButton");
			navigationMenuButton.Clicked += (s, e) => {
				if (s is IViewTransitable figmaTransition) {
					ProcessTransitionNodeID(figmaTransition.TransitionNodeID);
				}
			};

			var planVisitButton = RendererService.FindViewByName<IButton>("PlanVisitCustomButton");
			planVisitButton.Clicked += (s, e) =>
			{
				if (s is IViewTransitable figmaTransition)
				{
					ProcessTransitionNodeID(figmaTransition.TransitionNodeID);
				}
			};
		}

		void OnLoadMenuPage(FigmaSharp.Models.FigmaNode node)
		{
			BackgroundColor = Xamarin.Forms.Color.White;

			RenderViewWithConverters(node, new CustomLinkConverter());
		}

		void OnLoadExhibitionPage(FigmaSharp.Models.FigmaNode node)
		{
			BackgroundColor = Xamarin.Forms.Color.White;

			RenderViewWithConverters(node);

			var navigationMenuButton = RendererService.FindViewByName<IButton>("ExhibitionNavigationMenuButton");
			navigationMenuButton.Clicked += (s, e) => {
				if (s is IViewTransitable figmaTransition)
				{
					ProcessTransitionNodeID(figmaTransition.TransitionNodeID);
				}
			};
		}
	}
}