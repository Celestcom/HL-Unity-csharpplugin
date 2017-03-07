using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NullSpace.SDK
{
	internal class ParameterizedSequence
	{
		private HapticSequence _sequence;
		private AreaFlag _area;
		public ParameterizedSequence(HapticSequence sequence, AreaFlag area)
		{
			_sequence = sequence;
			_area = area;
		}

		public EventList Generate(float strength, float timeOffset)
		{
			EventList events = new EventList();
			foreach (var effect in _sequence.Effects)
			{
				float finalStrength = strength * effect.Strength;
				float finalTime = timeOffset + effect.Time;

				events.AddEvent(new BasicHapticEvent(
					finalTime,
					finalStrength,
					(float)effect.Item.Duration,
					(uint)_area,
					effect.Item.Effect	
				));
			}
			return events;
		}

	}
}
