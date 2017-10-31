using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hardlight.SDK
{
	/// <summary>
	/// <para>HapticEffects are the base building blocks of more complex effects. They can be strung together, repeated over a duration, and given strengths and time offsets.
	/// </para>
	/// </summary>
	[Serializable]
	public class HapticEffect : HapticElementBaseClass
	{
		[UnityEngine.SerializeField]
		private Effect _effect;
		[UnityEngine.SerializeField]
		private float _duration;

		/// <summary>
		/// Retrieve the associated Effect
		/// </summary>
		public Effect Effect
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
		/// Construct a HapticEffect with the given parameters
		/// </summary>
		/// <param name="effect">Which effect family should be used</param>
		/// <param name="duration">Effect duration (fractional seconds)</param>
		/// <param name="time">How long from the start of the parent does this begin</param>
		/// <param name="strength">How strong this effect should be (between 0 and 1.0f)</param>
		public HapticEffect(Effect effect = Effect.Click, float time = 0.0f, double duration = 0.0f, float strength = 1.0f) : base(time, strength)
		{
			_effect = effect;
			Duration = duration;
		}

		/// <summary>
		/// Create an independent copy of this HapticEffect
		/// </summary>
		/// <returns>A copy</returns>
		public HapticEffect Clone()
		{
			return new HapticEffect(_effect, Time, Duration, Strength);
		}

		/// <summary>
		/// Returns a string representation of this HapticEffect, including effect name and duration 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0} for {1} seconds", this.Effect.ToString(), _duration);
		}
	}
}
