using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hardlight.SDK
{
	/// <summary>
	/// An abstract base class for the parameterized elements (which means that they have time and strength)
	/// At any level of haptics, you can adjust timing or strength of elements. This represents that.
	/// </summary>
	[Serializable]
	public abstract class ParameterizedHapticElement
	{
		[UnityEngine.SerializeField]
		private float _time;
		[UnityEngine.SerializeField]
		private float _strength = 1;

		public float Time { get { return _time; } set { _time = value; } }
		public float Strength { get { return _strength; } set { _strength = value; } }

		public ParameterizedHapticElement(float time = 0.0f, float strength = 1.0f)
		{
			Time = time;
			Strength = strength;
		}

		public override string ToString()
		{
			return string.Format("Time: {0}, Strength: {1}", _time, _strength);
		}
	}
}
