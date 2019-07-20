using System;
using LiteForms;
using LiteForms.Cocoa;

namespace BasicGraphics.Cocoa
{
	public class OptionsPanelGradienContentView : ExampleGradientContentView
	{
		StackView optionContainer;

		Slider sliderX, sliderY, sliderScale, sliderRotate;

		protected float ItemSize = 120;
		protected float LinePadding = 15;

		const float maxValue = 200f;
		const float midValue = maxValue / 2f;

		public OptionsPanelGradienContentView()
		{
			//content of first 
			optionContainer = new StackView { Orientation = LayoutOrientation.Vertical, Padding = new Padding(20) };

			OperationsView.AddChild(optionContainer);

			//x
			var line1 = new StackView { Orientation = LayoutOrientation.Horizontal, Padding = new Padding(LinePadding), ItemSpacing = 20 };
			optionContainer.AddChild(line1);

			line1.AddChild(new Label { Text = "X", ForegroundColor = Color.White });

			sliderX = new Slider
			{
				MaxValue = maxValue,
				MinValue = 0,
				Value = midValue,
				Size = new Size(100, 20)
			};

			line1.AddChild(sliderX);

			//y
			var line2 = new StackView { Orientation = LayoutOrientation.Horizontal, Padding = new Padding(LinePadding), ItemSpacing = 20 };
			optionContainer.AddChild(line2);

			line2.AddChild(new Label { Text = "Y", ForegroundColor = Color.White });
			sliderY = new Slider
			{
				MaxValue = maxValue,
				MinValue = 0,
				Value = midValue,
				Size = new Size(100, 20)
			};

			line2.AddChild(sliderY);

			//rotate
			var line3 = new StackView { Orientation = LayoutOrientation.Horizontal, Padding = new Padding(LinePadding), ItemSpacing = 20 };
			optionContainer.AddChild(line3);

			line3.AddChild(new Label { Text = "Scale", ForegroundColor = Color.White });
			sliderScale = new Slider
			{
				MaxValue = 2,
				MinValue = 0,
				Value = 1,
				Size = new Size(100, 20)
			};
			line3.AddChild(sliderScale);

			//rotate
			var line4 = new StackView { Orientation = LayoutOrientation.Horizontal, Padding = new Padding(LinePadding), ItemSpacing = 20 };
			optionContainer.AddChild(line4);

			line4.AddChild(new Label { Text = "Rotate", ForegroundColor = Color.White });
			sliderRotate = new Slider
			{
				MaxValue = 90,
				MinValue = 0,
				Value = 0,
				Size = new Size(100, 20)
			};
			line4.AddChild(sliderRotate);
			sliderX.ValueChanged += (s, e) => view?.ApplyTransform(GetActualTranformation());
			sliderY.ValueChanged += (s, e) => view?.ApplyTransform(GetActualTranformation());
			sliderScale.ValueChanged += (s, e) => view?.ApplyTransform(GetActualTranformation());
			sliderRotate.ValueChanged += (s, e) => view?.ApplyTransform(GetActualTranformation());
		}

		public event EventHandler SliderXValueChanged;
		public event EventHandler SliderYValueChanged;
		public event EventHandler SliderScaleValueChanged;
		public event EventHandler SliderRotateValueChanged;

		public LiteForms.Cocoa.View view;

		public void SetView(LiteForms.Cocoa.View shapeView)
		{
			RemoveView();
			view = shapeView;
			ContentView.AddChild(shapeView);
			CenterView();
			view?.ApplyTransform(GetActualTranformation());
		}

		public void RemoveView()
		{
			if (view != null)
			{
				ContentView.RemoveChild(view);
				view = null;
			}
		}

		public void CenterView()
		{
			if (view != null && view.Parent != null)
			{
				var center = view.Parent.Allocation.Center();
				view.SetPosition(new Point(center.X - (view.Size.Width / 2f), center.Y - (view.Size.Height / 2f)));
			}
		}
		//bool optionsShown;
		//public void ShowOptions()
		//{
		//	if (optionsShown)
		//	{
		//		return;
		//	}
		//	optionsShown = true;
		//	OperationsView.AddChild(optionContainer);
		//}

		//public void HideOptions()
		//{
		//	if (!optionsShown)
		//	{
		//		return;
		//	}
		//	optionsShown = false;
		//	OperationsView.RemoveChild(optionContainer);
		//}

		public Transform GetActualTranformation()
		{
			return new Transform
			{
				Translation = new Point(sliderX.Value - midValue, sliderY.Value - midValue),
				RotateAngle = sliderRotate.Value,
				Scale = sliderScale.Value
			};
		}
	}
}
