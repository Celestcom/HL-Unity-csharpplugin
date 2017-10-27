using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hardlight.SDK
{
	[Serializable]
	public class ParameterizedPattern : HapticElementBaseClass
	{
		[UnityEngine.SerializeField]
		private PatternSO _pattern;

		public PatternSO Pattern
		{
			get
			{
				return _pattern;
			}

			set
			{
				_pattern = value;
			}
		}

		public ParameterizedPattern(PatternSO pattern, float time = 0.0f, float strength = 1.0f)
		{
			Pattern = pattern;
			Time = time;
			Strength = strength;
		}

		internal EventList Generate(float strength, float timeOffset)
		{
			EventList events = new EventList();
			foreach (var sequence in Pattern.Sequences)
			{
				if (sequence != null)
				{
					float finalStrength = strength * sequence.Strength;
					float finalTime = timeOffset + sequence.Time;

					EventList subEvents = sequence.Generate(finalStrength, finalTime);
					events.AddAll(subEvents);
				}
			}
			return events;
		}

		/// <summary>
		/// Create an independent copy of this ParameterizedPattern
		/// </summary>
		/// <returns>A copy</returns>
		public ParameterizedPattern Clone()
		{
			return new ParameterizedPattern(Pattern, Time, Strength);
		}

	}
}
