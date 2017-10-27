using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hardlight.SDK
{
	
	[Serializable]
	public class CommonArgs<THapticType>
	{
		private THapticType _item;
		float _time;
		float _strength;

		public float Time { get { return _time; } }
		public float Strength { get { return _strength; } }
		public THapticType Item { get { return _item; } }

		public CommonArgs(float time, float strength, THapticType item) {
			_item = item;
			_time = time;
			_strength = strength;
		}

		public override string ToString()
		{
			return string.Format("Time: {0}, Strength: {1}, Item: {2}", _time, _strength, _item.ToString());
		}
	}

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
