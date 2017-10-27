using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Hardlight.SDK
{
	[Serializable]
	internal class ParameterizedExperience : HapticElementBaseClass
	{
		private ExperienceSO _experience;
		public ExperienceSO Experience
		{
			get
			{
				return _experience;
			}

			set
			{
				_experience = value;
			}
		}

		public ParameterizedExperience(ExperienceSO experience, float time = 0.0f, float strength = 1.0f)
		{
			Experience = experience;
			Strength = strength;
			Time = time;
		}

		public EventList Generate(float strength, float timeOffset)
		{
			EventList events = new EventList();
			foreach (var pattern in Experience.Patterns)
			{
				if (pattern != null)
				{
					float finalStrength = strength * pattern.Strength;
					float finalTime = timeOffset + pattern.Time;

					EventList subEvents = pattern.Generate(finalStrength, finalTime);
					events.AddAll(subEvents);
				}
			}
			return events;
		}

		/// <summary>
		/// Create an independent copy of this ParameterizedExperience
		/// </summary>
		/// <returns>A copy</returns>
		public ParameterizedExperience Clone()
		{
			return new ParameterizedExperience(Experience, Time, Strength);
		}

		public override string ToString()
		{
			return string.Format("{0}", Experience.ToString());
		}
	}
}
