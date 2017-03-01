using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatBuffers;
using NullSpace.HapticFiles;
using NullSpace.SDK.Internal;
using NullSpace.SDK.FileUtilities;
namespace NullSpace.SDK
{
	/// <summary>
	/// <para>CodeEffects are the base atoms used to create more complex haptics. See documentation for Effect Families
	/// to get a list of the currently available effects.</para><para> Effects can have time offsets, durations, and strengths, 
	/// and are combined together to create CodeSequences. </para>
	/// </summary>
	public class HapticEffect 
	{

		private float _time;
		private string _effect;
		private float _duration;
		private float _strength;

		/// <summary>
		/// Retrieve the time offset (fractional seconds)
		/// </summary>
		public float Time
		{
			get
			{
				return _time;
			}

			set
			{
				_time = value;
			}
		}

		/// <summary>
		/// Retrieve the effect name 
		/// </summary>
		public string Effect
		{
			get
			{
				return _effect;
			}

			set
			{
				_effect = value;
			}
		}

		/// <summary>
		/// Retrieve the duration
		/// </summary>
		public double Duration
		{
			get
			{
				return _duration;
			}

			set
			{
				_duration = (float)value;
			}
		}

		/// <summary>
		/// Retrieve the strength (0.0 - 1.0)
		/// </summary>
		public double Strength
		{
			get
			{
				return _strength;
			}

			set
			{
				_strength = (float)value;
			}
		}

	

		/// <summary>
		/// Construct a HapticEffect with a given effect family, default duration of 0, and strength of 1.
		/// </summary>
		/// <param name="effect">Effect name</param>
		public HapticEffect(string effect)
		{
			_effect = effect;
			_duration = 0f;
			_strength = 1f;
		}

		/// <summary>
		/// Construct a HapticEffect with a given effect family, duration, and default strength of 1
		/// </summary>
		/// <param name="effect">Effect name</param>
		/// <param name="duration">Effect duration (fractional seconds)</param>
		public HapticEffect(string effect, double duration)
		{
			_effect = effect;
			Duration = duration;
			_strength = 1f;
		}

		/// <summary>
		/// Construct a HapticEffect with a given effect family, duration, and strength
		/// </summary>
		/// <param name="effect">Effect name</param>
		/// <param name="duration">Effect duration (fractional seconds)</param>
		/// <param name="strength">Effect strength (0.0-1.0)</param>
		public HapticEffect(string effect, double duration, double strength)
		{
			_effect = effect;
			Duration = duration;
			Strength = strength;
		}


		
	
		/// <summary>
		/// Create an independent copy of this HapticEffect
		/// </summary>
		/// <returns>A copy</returns>
		public HapticEffect Clone()
		{
			return new HapticEffect(_effect, Duration, Strength);
		}

	
	}

	/// <summary>
	/// <para>CodeSequences are haptic effects which play on a given area on the suit. This area is specified with an AreaFlag, which can represent anything from one location to the entire suit.</para><para>A HapticSequence is composed of one or more CodeEffects with time offsets.</para>
	/// </summary>
	public class HapticSequence :Playable
	{
		private AreaFlag _area;
		private float _strength;
		private IList<HapticEffect> _children;
		private float _time;

		/// <summary>
		/// Retrieve the AreaFlag associated with this HapticSequence
		/// </summary>
		public AreaFlag Area
		{
			get
			{
				return _area;
			}

			set
			{
				_area = value;
			}
		}

		/// <summary>
		/// Retrieve the strength of this HapticSequence (0.0-1.0)
		/// </summary>
		public double Strength
		{
			get
			{
				return _strength;
			}

			set
			{
				_strength = (float)value;
			}
		}

		/// <summary>
		/// Retrieve the list of CodeEffects associated with this HapticSequence
		/// </summary>
		public IList<HapticEffect> Effects
		{
			get
			{
				return _children;
			}

			set
			{
				_children = value;
			}
		}

		/// <summary>
		/// Retrieve the time offset of this HapticSequence (fractional seconds)
		/// </summary>
		public double Time
		{
			get
			{
				return _time;
			}

			set
			{
				_time = (float) value;
			}
		}


		/// <summary>
		/// Construct an empty HapticSequence with a default area of AreaFlag.None, and strength of 1.
		/// </summary>
		public HapticSequence()
		{
			_area = AreaFlag.None;
			_strength = 1f;
			_children = new List<HapticEffect>();
		}

		public HapticSequence(string id)
		{
			
		}

		/// <summary>
		/// Create an independent copy of this HapticSequence
		/// </summary>
		/// <returns></returns>
		public HapticSequence Clone()
		{
			var clone = new HapticSequence();
			clone.Area = _area;
			clone.Strength = _strength;
			clone.Time = _time;
			clone.Effects = new List<HapticEffect>(_children);
			return clone;
		}
		
		/// <summary>
		/// Add a HapticEffect with a given time offset
		/// </summary>
		/// <param name="time">Time offset (fractional seconds)</param>
		/// <param name="e">The HapticEffect to add</param>
		public HapticSequence AddEffect(double time, HapticEffect effect)
		{
			var clone = effect.Clone();
			clone.Time = (float)time;
			Effects.Add(clone);
			return this;
		}

		
		
		private Interop.CommandWithHandle _create(AreaFlag location, double strength = 1.0)
		{
			return new Interop.CommandWithHandle(handle => {
				var oldArea = this.Area;
				this.Area = location;
				CodeHapticEncoder encoder = new CodeHapticEncoder();
				encoder.Flatten(this);
				var bytes = encoder.Encode();
				this.Area = oldArea;
			
				Interop.NSVR_TransmitEvents(NSVR.NSVR_Plugin.Ptr, handle, bytes, (UInt32)bytes.Length);
			});
			

		}

		/// <summary>
		/// Create a HapticHandle for this HapticSequence, specifying an AreaFlag to play on.
		/// </summary>
		/// <param name="area">The AreaFlag where this HapticSequence should play</param>
		/// <returns></returns>
		public HapticHandle CreateHandle(AreaFlag area)
		{
			return new HapticHandle(_Play(), _Pause(), _create(area), _Reset());
		}

		/// <summary>
		/// Create a HapticHandle for this HapticSequence, specifying an AreaFlag and a strength.
		/// </summary>
		/// <param name="area">The AreaFlag where this HapticSequence should play</param>
		/// <param name="strength">The strength of this HapticSequence (0.0-1.0)</param>
		/// <returns></returns>
		public HapticHandle CreateHandle(AreaFlag area, double strength)
		{
			return new HapticHandle(_Play(), _Pause(), _create(area, strength), _Reset());
		}

		/// <summary>
		/// <para>A helper which calls Play on a newly created HapticHandle.</para>
		/// <para>Synonymous with someSequence.CreateHandle(area).Play() </para>
		/// </summary>
		/// <param name="area"></param>
		/// <returns></returns>
		public HapticHandle Play(AreaFlag area)
		{
			var handle = CreateHandle(area);
			handle.Play();
			return handle;
		}

		/// <summary>
		/// <para>A helper which calls Play on a newly created HapticHandle.</para>
		/// <para>Synonymous with someSequence.CreateHandle(area, strength).Play()</para>
		/// </summary>
		/// <param name="area"></param>
		/// <param name="strength"></param>
		/// <returns></returns>
		public HapticHandle Play(AreaFlag area, double strength)
		{
			var handle = CreateHandle(area, strength);
			handle.Play();
			return handle;
		}
	}
	


	/// <summary>
	/// CodePatterns are used to combine one or more CodeSequences into a single, playable effect. Each HapticSequence added to the HapticPattern will have a time offset and optional strength. 
	/// </summary>
	public class HapticPattern : Playable
	{
		private float _strength;
		private float _time;
		private IList<HapticSequence> _children;

		/// <summary>
		/// Retrieve the strength of this HapticPattern (0.0 - 1.0)
		/// </summary>
		public double Strength
		{
			get
			{
				return _strength;
			}

			set
			{
				_strength = (float)value;
			}
		}

		/// <summary>
		/// Retrieve the time offset of this HapticPattern (fractional seconds)
		/// </summary>
		public double Time
		{
			get
			{
				return _time;
			}

			set
			{
				_time = (float)value;
			}
		}

		/// <summary>
		/// Retrieve the list of CodeSequences that make up this HapticPattern
		/// </summary>
		public IList<HapticSequence> Sequences
		{
			get
			{
				return _children;
			}

			set
			{
				_children = value;
			}
		}

	

		/// <summary>
		/// Construct an empty HapticPattern with a default strength of 1
		/// </summary>
		public HapticPattern()
		{
			_children = new List<HapticSequence>();
			_time = 0f;
			_strength = 1f;
		}
		
		/// <summary>
		/// Add a HapticSequence to this HapticPattern with a given time offset and AreaFlag, and default strength of 1
		/// </summary>
		/// <param name="time">Time offset (fractional seconds)</param>
		/// <param name="area">AreaFlag on which to play the HapticSequence</param>
		/// <param name="sequence">The HapticSequence to be added</param>
		public HapticPattern AddSequence(double time, AreaFlag area, HapticSequence sequence)
		{
			var clone = sequence.Clone();
			clone.Time = time;
			clone.Area = area;
			clone.Strength = 1.0;
			_children.Add(clone);
			return this;
		}

		/// <summary>
		/// Add a HapticSequence to this HapticPattern with a given time offset, AreaFlag, and strength.
		/// </summary>
		/// <param name="time">Time offset (fractional seconds)</param>
		/// <param name="area">AreaFlag on which to play the HapticSequence</param>
		/// <param name="strength">Strength of the HapticSequence (0.0 - 1.0)</param>
		/// <param name="sequence">The HapticSequence to be added</param>
		public HapticPattern AddSequence(double time, AreaFlag area, double strength, HapticSequence sequence)
		{
			var clone = sequence.Clone();
			clone.Strength = strength;
			clone.Time = time;
			clone.Area = area;
			_children.Add(clone);
			return this;
		}

		
		
		private Interop.CommandWithHandle _create(double strength = 1.0)
		{
			return new Interop.CommandWithHandle(handle => {

				CodeHapticEncoder encoder = new CodeHapticEncoder();
				encoder.Flatten(this);
				var bytes = encoder.Encode();

				Interop.NSVR_TransmitEvents(NSVR.NSVR_Plugin.Ptr, handle, bytes, (UInt32)bytes.Length);
			});
		

		}

		/// <summary>
		/// Create a HapticHandle from this HapticPattern, which can be used to manipulate the effect. 
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public HapticHandle CreateHandle()
		{
			return new HapticHandle(_Play(), _Pause(), _create(_strength), _Reset());
		}

		/// <summary>
		/// Create a HapticHandle from this HapticPattern, passing in a given strength. 
		/// </summary>
		/// <param name="strength"></param>
		/// <returns>A new HapticHandle</returns>
		public HapticHandle CreateHandle(double strength)
		{
			return new HapticHandle(_Play(), _Pause(), _create(strength), _Reset());
		}


		/// <summary>
		/// <para>Helper method which calls Play on a newly-created HapticHandle.</para>
		/// <para>Synonymous with somePattern.CreateHandle().Play()</para>
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public HapticHandle Play()
		{
			return CreateHandle().Play();
		}

		/// <summary>
		/// <para>Helper method which calls Play on a newly-created HapticHandle with a given strength</para>
		/// <para>Synonymous with somePattern.CreateHandle(strength).Play()</para>
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public HapticHandle Play(double strength)
		{
			return CreateHandle(strength).Play();
		}
	}
	

	
}
