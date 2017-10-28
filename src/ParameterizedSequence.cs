using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hardlight.SDK
{
	[Serializable]
	public class ParameterizedSequence : HapticElementBaseClass
	{
		[UnityEngine.SerializeField]
		private int _sequenceKey = 0;
		[UnityEngine.SerializeField]
		private SequenceSO _sequence;

		public bool UsingGenerator = false;
		[UnityEngine.SerializeField]
		private GeneratorLocation _generator;
		[UnityEngine.SerializeField]
		private AreaFlagLocation _areaLoc;

		public SequenceSO Sequence
		{
			get
			{
				return _sequence;
			}

			set
			{
				_sequence = value;
			}
		}
		public int SequenceKey
		{
			get
			{
				return _sequenceKey;
			}

			set
			{
				_sequenceKey = value;
			}
		}

		public GeneratorLocation Generator
		{
			get
			{
				if (_generator == null)
				{
					_generator = new GeneratorLocation();
				}
				return _generator;
			}

			set
			{
				if (_generator == null)
				{
					_generator = new GeneratorLocation();
				}
				_generator = value;
			}
		}
		//This could do something like talk to the generator?
		public AreaFlag Area
		{
			get
			{
				if (_areaLoc == null)
				{
					_areaLoc = new AreaFlagLocation();
				}
				return _areaLoc.Area;
			}

			set
			{
				if (_areaLoc == null)
				{
					_areaLoc = new AreaFlagLocation();
				}
				_areaLoc.Area = value;
			}
		}

		public ParameterizedSequence(SequenceSO sequence, AreaFlag area, float time = 0.0f, float strength = 1.0f)
		{
			Sequence = sequence;
			Area = area;
			Strength = strength;
			Time = time;
		}

		internal EventList Generate(float strength, float timeOffset)
		{
			EventList events = new EventList();
			var effects = Sequence.Effects;
			foreach (var effect in effects)
			{
				if (effect != null)
				{
					float finalStrength = strength * effect.Strength;
					float finalTime = timeOffset + effect.Time;

					var newApiRegions = AreaFlagToRegion.GetRegions(Area);

					events.AddEvent(new BasicHapticEvent(
						finalTime,
						finalStrength,
						(float)effect.Duration,
						newApiRegions,
						effect.Effect
					));
				}
			}
			return events;
		}

		/// <summary>
		/// Create an independent copy of this ParameterizedSequence
		/// </summary>
		/// <returns>A copy</returns>
		public ParameterizedSequence Clone()
		{
			return new ParameterizedSequence(Sequence, Area, Time, Strength);
		}

		public override string ToString()
		{
			return string.Format("On area {0}: \n{1}", Area, Sequence.ToString());
		}

	}
}
