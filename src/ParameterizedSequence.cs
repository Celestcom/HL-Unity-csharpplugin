using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hardlight.SDK
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

				var newApiRegions = AreaFlagToRegion.GetRegions(_area);

				events.AddEvent(new BasicHapticEvent(
					finalTime,
					finalStrength,
					(float)effect.Item.Duration,
					newApiRegions,
					effect.Item.Effect	
				));
			}
			return events;
		}

		public override string ToString()
		{
			return string.Format("On area {0}: \n{1}", _area,  _sequence.ToString());
		}

	}
}
