using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hardlight.SDK
{
	[Serializable]
	public abstract class HapticElementBaseClass
	{
		[UnityEngine.SerializeField]
		private float _time;
		[UnityEngine.SerializeField]
		private float _strength = 1;

		public float Time { get { return _time; } set { _time = value; } }
		public float Strength { get { return _strength; } set { _strength = value; } }

		public override string ToString()
		{
			return string.Format("Time: {0}, Strength: {1}", _time, _strength);
		}
	}
}
