using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Hardlight.SDK
{
	internal class ParameterizedExperience
	{
		private HapticExperience _experience;
		public ParameterizedExperience(HapticExperience experience)
		{
			_experience = experience;
		}

		public EventList Generate(float strength, float timeOffset)
		{
			EventList events = new EventList();
			foreach (var pattern in _experience.Patterns)
			{
				float finalStrength = strength * pattern.Strength;
				float finalTime = timeOffset + pattern.Time;

				EventList subEvents = pattern.Item.Generate(finalStrength, finalTime);
				events.AddAll(subEvents);
			}
			return events;
		}

		public override string ToString()
		{
			return string.Format("{0}", _experience.ToString());
		}
	}
}
