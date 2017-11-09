using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hardlight.SDK
{
	/// <summary>
	/// Combine a pattern with inherent time & strength info
	/// Used for generating the timeline to play an experience.
	/// </summary>
	[Serializable]
	public class ParameterizedPattern : ParameterizedHapticElement
	{
		[UnityEngine.SerializeField]
		private HapticPattern _pattern;
		public HapticPattern Pattern
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

		public ParameterizedPattern(HapticPattern pattern, float time = 0.0f, float strength = 1.0f) : base(time, strength)
		{
			Pattern = pattern;
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
