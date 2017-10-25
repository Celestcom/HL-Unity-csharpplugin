using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hardlight.SDK
{
	public class ParameterizedPattern
	{
		private HapticPattern _pattern;
		public ParameterizedPattern(HapticPattern pattern)
		{
			_pattern = pattern;
		}

		internal EventList Generate(float strength, float timeOffset)
		{
			EventList events = new EventList();
			foreach (var sequence in _pattern.Sequences)
			{
				float finalStrength = strength * sequence.Strength;
				float finalTime = timeOffset + sequence.Time;

				EventList subEvents = sequence.Item.Generate(finalStrength, finalTime);
				events.AddAll(subEvents);
			}
			return events;
		}

	}
}
