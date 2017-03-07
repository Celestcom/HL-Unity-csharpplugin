using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NullSpace.SDK
{
	
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

		
	}
}
