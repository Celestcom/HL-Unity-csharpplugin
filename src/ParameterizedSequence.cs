using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hardlight.SDK
{
	/// <summary>
	/// Combine sequence & location information for presence in a Pattern's Sequences array.
	/// Parameterized also means it inherently has time & strength
	/// </summary>
	[Serializable]
	public class ParameterizedSequence : ParameterizedHapticElement
	{
		//[UnityEngine.SerializeField]
		//private int _sequenceKey = 0;
		[UnityEngine.SerializeField]
		private HapticSequence _sequence;

		public bool UsingGenerator = false;
		/// <summary>
		/// [Incomplete] This exists for the future capability to add Generators to Parameterized sequences (allowing for randomization of which area to play a haptic on)
		/// </summary>
		[UnityEngine.SerializeField]
		private GeneratorLocation _generator;
		[UnityEngine.SerializeField]
		private AreaFlagLocation _areaLoc;

		//This exists because Unity is a huge pain when it comes to serialization.
		[UnityEngine.SerializeField]
		private AreaFlag _areaFlag;

		public HapticSequence Sequence
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
		//public int SequenceKey
		//{
		//	get
		//	{
		//		return _sequenceKey;
		//	}

		//	set
		//	{
		//		_sequenceKey = value;
		//	}
		//}

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
				return _areaFlag;
				//if (_areaLoc == null)
				//{
				//	_areaLoc = new AreaFlagLocation();
				//}
				//return _areaLoc.Area;
			}

			set
			{
				//if (value == AreaFlag.None)
				//	UnityEngine.Debug.LogError("Assigned empty area to a parameterized sequence\n");
				_areaFlag = value;
				//if (_areaLoc == null)
				//{
				//	_areaLoc = new AreaFlagLocation();
				//}
				//_areaLoc.Area = value;
			}
		}

		public ParameterizedSequence(HapticSequence sequence, AreaFlag area, float time = 0.0f, float strength = 1.0f) : base(time, strength)
		{
			Sequence = sequence;
			Area = area;
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

					//UnityEngine.Debug.Log(String.Format("{0} {1} {2} {3}", finalTime,
					//	finalStrength,
					//	(float)effect.Duration,
					//	effect.Effect) + "\n");

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
