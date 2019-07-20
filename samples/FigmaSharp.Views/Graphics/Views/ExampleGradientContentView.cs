using System;
using LiteForms;
using LiteForms.Cocoa;
using LiteForms.Graphics.Mac;

namespace BasicGraphics.Cocoa
{
	public class ExampleGradientContentView : ExampleContentView
	{
		public ExtendedView ContentView { get; private set; }
		public StackView OperationsView { get; private set; }

		public ExampleGradientContentView()
		{
			ContentView = new ExtendedView()
			{
				MovableByWindowBackground = true,
				BorderColor = new Color(1, 1, 1, 0.08f)
			};
			ContentView.SetGradientLayer(
				new Color(r: 1.00f, g: 0.00f, b: 0.53f),
				new Color(r: 0.87f, g: 0.00f, b: 0.93f)
				);
		
			AddChild(ContentView);

			OperationsView = new StackView() { Orientation = LayoutOrientation.Horizontal };
			AddChild(OperationsView);
		}

		float contentPercent = 0.59f;
		public float ContentPercent
		{
			get => contentPercent;
			set
			{
				contentPercent = Math.Max(0, Math.Min(1, value));
			}
		}

		public override void OnChangeFrameSize(Size newSize)
		{
			var operationHeight = newSize.Height * ContentPercent;
			ContentView.Allocation = new Rectangle(0, 0, newSize.Width, operationHeight);
			OperationsView.Allocation = new Rectangle(0, operationHeight, newSize.Width, newSize.Height - operationHeight);
		}
	}
}
