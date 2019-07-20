using LiteForms;
using LiteForms.Cocoa;
using System.Collections.Generic;

namespace BasicGraphics.Cocoa
{
	class Motion
	{
		public Transform Initial { get; set; }
		public Transform Animate { get; set; }
	}

	public enum TransitionType
	{
		Spring
	}

	public enum TransitionBehaviour
	{
		BeforeChildren
	}

	public class Svg
	{
		public string Xmlns { get; set; }
		public string ViewBox { get; set; }

		public List<SvgPath> Paths { get; set; }
	}

	public class SvgPath
	{
		public string Data { get; set; }
		public string Initial { get; set; }
		public string Animate { get; set; }

		public Dictionary<string, Transition> Transitions = new Dictionary<string, Transition>();
	}

	public abstract class Transition
	{
		public string Name { get; set; }
		public float Duration { get; set; }
	}

	public class EaseEffectTransition : Transition
	{
		public string EaseEffect { get; set; }
	}

	public class EaseValuesTransition : Transition
	{
		public float[] Values { get; set; }
	}

	//public class Transition
	//{
	//	public TransitionType TransitionType { get; set; }
	//	public float Stiffness { get; set; }
	//	public float Damping { get; set; }

	//	public TransitionBehaviour Behaviour { get; set; }
	//	public float StaggerChildren { get; set; }
	//}

}

