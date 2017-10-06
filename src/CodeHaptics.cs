using System.Collections.Generic;
using System.Text;
using Hardlight.SDK.FileUtilities;
using System;
using System.Diagnostics;

namespace Hardlight.SDK
{
	/// <summary>
	/// HapticEffects are the base building blocks of more complex effects. They can be strung together, repeated over a duration, and given strengths and time offsets.
	/// </summary>
	public sealed class HapticEffect
	{
		private Effect _effect;
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
		/// Construct a HapticEffect with a given Effect, and default duration of 0.0
		/// </summary>
		/// <param name="effect"></param>
		public HapticEffect(Effect effect)
		{
			_effect = effect;
			_duration = 0f;
		}

		/// <summary>
		/// Construct a HapticEffect with a given Effect and duration
		/// </summary>
		/// <param name="effect"></param>
		/// <param name="duration">Effect duration (fractional seconds)</param>
		public HapticEffect(Effect effect, double duration)
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
			HapticEffect clone = new HapticEffect(_effect);
			clone.Duration = this._duration;
			return clone;
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

	/// <summary>
	/// <para>CodeSequences are haptic effects which play on a given area on the suit. The area is specified with an AreaFlag, which can represent anything from one location to the entire suit.</para>
	/// <para>A HapticSequence is composed of one or more HapticEffects with time offsets.</para>
	/// </summary>
	public sealed class HapticSequence : SerializableHaptic
	{
		private IList<CommonArgs<HapticEffect>> _children;

		internal IList<CommonArgs<HapticEffect>> Effects
		{
			get { return _children; }
			set { _children = value; }
		}

		/// <summary>
		/// Construct an empty HapticSequence
		/// </summary>
		public HapticSequence() : base("sequence")
		{
			_children = new List<CommonArgs<HapticEffect>>();
		}
		public HapticSequence(string lazyLoadAssetPath) : base("sequence")
		{
			LoadedAssetName = lazyLoadAssetPath;
			_children = new List<CommonArgs<HapticEffect>>();
		}

		/// <summary>
		/// Add a HapticEffect with a given time offset
		/// </summary>
		/// <param name="time">Time offset (fractional seconds)</param>
		/// <param name="effect">The HapticEffect to add</param>
		public HapticSequence AddEffect(double time, HapticEffect effect)
		{
			Effects.Add(new CommonArgs<HapticEffect>((float)time, 1f, effect.Clone()));
			return this;
		}

		/// <summary>
		/// Add a HapticEffect with a given time offset and strength 
		/// </summary>
		/// <param name="time">Time offset (fractional seconds)</param>
		/// <param name="strength">Strength (0.0-1.0)</param>
		/// <param name="effect">The HapticEffect to add</param>
		/// <returns></returns>
		public HapticSequence AddEffect(double time, double strength, HapticEffect effect)
		{
			Effects.Add(new CommonArgs<HapticEffect>((float)time, (float)strength, effect.Clone()));
			return this;
		}

		/// <summary>
		/// Create a HapticHandle from this HapticSequence, specifying an AreaFlag to play on.
		/// </summary>
		/// <param name="area">The AreaFlag where this HapticSequence should play</param>
		/// <returns>A new HapticHandle bound to this effect playing on the given area</returns>
		public HapticHandle CreateHandle(AreaFlag area)
		{
			HandleLazyAssetLoading();

			EventList e = new ParameterizedSequence(this, area).Generate(1f, 0f);

			HapticHandle.CommandWithHandle creator = delegate (IntPtr handle)
			{
				e.Transmit(handle);
			};

			HapticHandle h = new HapticHandle(creator);
			return h;
		}

		/// <summary>
		/// Create a HapticHandle for this HapticSequence, specifying an AreaFlag and a strength.
		/// </summary>
		/// <param name="area">The AreaFlag where this HapticSequence should play</param>
		/// <param name="strength">The strength of this HapticSequence (0.0-1.0)</param>
		/// <returns>A new HapticHandle bound to this effect playing on the given area</returns>
		public HapticHandle CreateHandle(AreaFlag area, double strength)
		{
			HandleLazyAssetLoading();

			EventList e = new ParameterizedSequence(this, area).Generate((float)strength, 0f);
			HapticHandle.CommandWithHandle creator = delegate (IntPtr handle)
			{
				e.Transmit(handle);
			};

			return new HapticHandle(creator);
		}

		/// <summary>
		/// <para>If you want to play a sequence but don't care about controlling playback, use this method. It will automatically clean up resources.</para>
		/// <para>Synonymous with someSequence.CreateHandle(area).Play().Release() </para>
		/// </summary>
		/// <param name="area">The area on which to play this sequence</param>
		/// <returns>A new HapticHandle bound to this effect playing on the given area</returns>
		public void Play(AreaFlag area)
		{
			CreateHandle(area).Play().Dispose();
		}

		/// <summary>
		/// <para>A helper which calls Play on a newly created HapticHandle.</para>
		/// <para>Synonymous with someSequence.CreateHandle(area, strength).Play()</para>
		/// </summary>
		/// <param name="area">The area on which to play this sequence</param>
		/// <param name="strength">The strength with which to play this sequence</param>
		/// <returns>A new HapticHandle bound to this effect playing on the given area</returns>
		public HapticHandle Play(AreaFlag area, double strength)
		{
			return CreateHandle(area, strength).Play();
		}

		/// <summary>
		/// Internal use: turns an HDF into a sequence
		/// </summary>
		/// <param name="hdf"></param>
		internal override void doLoadFromHDF(string key, HapticDefinitionFile hdf)
		{
			var sequence_def_array = hdf.sequence_definitions[key];
			foreach (var effect in sequence_def_array)
			{
				Effect e = FileEffectToCodeEffect.TryParse(effect.effect, Effect.Click);
				this.AddEffect(effect.time, effect.strength, new HapticEffect(e, effect.duration));
			}
		}

		/// <summary>
		/// Create an independent copy of this HapticSequence
		/// </summary>
		/// <returns></returns>
		public HapticSequence Clone()
		{
			var clone = new HapticSequence(LoadedAssetName);
			clone.Effects = new List<CommonArgs<HapticEffect>>(_children);
			return clone;
		}
		/// <summary>
		/// Returns a string representation of this HapticSequence for debugging purposes, including all child effects
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("Sequence of {0} HapticEffects:\n", this.Effects.Count));
			foreach (var child in this.Effects)
			{
				sb.Append(string.Format("[{0}]\n", child.ToString()));
			}
			sb.Append("\n");
			return sb.ToString();
		}
	}
	/// <summary>
	/// HapticPatterns are used to combine one or more HapticSequences into a single, playable effect. Each HapticSequence added to the HapticPattern will have a time offset and optional strength, as well as a specified area.
	/// </summary>
	public sealed class HapticPattern : SerializableHaptic
	{
		private IList<CommonArgs<ParameterizedSequence>> _children;

		internal IList<CommonArgs<ParameterizedSequence>> Sequences
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
		/// Construct an empty HapticPattern
		/// </summary>
		public HapticPattern() : base("pattern")
		{
			_children = new List<CommonArgs<ParameterizedSequence>>();
		}

		public HapticPattern(string lazyLoadAssetPath) : base("pattern")
		{
			LoadedAssetName = lazyLoadAssetPath;
			_children = new List<CommonArgs<ParameterizedSequence>>();
		}/// <summary>
		 /// Add a HapticSequence to this HapticPattern with a given time offset and AreaFlag, and default strength of 1.0
		 /// </summary>
		 /// <param name="time">Time offset (fractional seconds)</param>
		 /// <param name="area">AreaFlag on which to play the HapticSequence</param>
		 /// <param name="sequence">The HapticSequence to be added</param>
		public HapticPattern AddSequence(double time, AreaFlag area, HapticSequence sequence)
		{
			ParameterizedSequence clone = new ParameterizedSequence(sequence.Clone(), area);
			_children.Add(new CommonArgs<ParameterizedSequence>((float)time, 1f, clone));
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
			ParameterizedSequence clone = new ParameterizedSequence(sequence.Clone(), area);
			_children.Add(new CommonArgs<ParameterizedSequence>((float)time, (float)strength, clone));
			return this;
		}

		/// <summary>
		/// Create a HapticHandle from this HapticPattern, which can be used to manipulate the effect. 
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public HapticHandle CreateHandle()
		{
			HandleLazyAssetLoading();

			EventList e = new ParameterizedPattern(this).Generate(1f, 0f);

			HapticHandle.CommandWithHandle creator = delegate (IntPtr handle)
			{
				e.Transmit(handle);
			};

			return new HapticHandle(creator);
		}

		/// <summary>
		/// Create a HapticHandle from this HapticPattern, passing in a given strength. 
		/// </summary>
		/// <param name="strength"></param>
		/// <returns>A new HapticHandle</returns>
		public HapticHandle CreateHandle(double strength)
		{
			HandleLazyAssetLoading();

			EventList e = new ParameterizedPattern(this).Generate((float)strength, 0f);

			HapticHandle.CommandWithHandle creator = delegate (IntPtr handle)
			{
				e.Transmit(handle);

			};

			return new HapticHandle(creator);
		}

		/// <summary>
		/// <para>If you want to play a pattern but don't care about controlling playback, use this method. It will automatically clean up resources.</para>
		/// <para>Synonymous with somePattern.CreateHandle().Play().Release()</para>
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public void Play()
		{
			CreateHandle().Play().Dispose();
		}

		/// <summary>
		/// <para>If you want to play a pattern but don't care about controlling playback, use this method. It will automatically clean up resources.</para>
		/// <para>Synonymous with somePattern.CreateHandle(strength).Play().Release()</para>
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public void Play(double strength)
		{
			CreateHandle(strength).Play().Dispose();
		}

		/// <summary>
		/// Internal use: turns an HDF into a pattern
		/// </summary>
		/// <param name="hdf"></param>
		internal override void doLoadFromHDF(string key, HapticDefinitionFile hdf)
		{
			var pattern_def_array = hdf.pattern_definitions[key];
			foreach (var seq in pattern_def_array)
			{
				AreaFlag area = new AreaParser(seq.area).GetArea();
				HapticSequence thisSeq = new HapticSequence();
				thisSeq.doLoadFromHDF(seq.sequence, hdf);
				AddSequence(seq.time, area, thisSeq);
			}
		}

		/// <summary>
		/// Create an independent copy of this HapticPattern
		/// </summary>
		/// <returns></returns>
		public HapticPattern Clone()
		{
			var clone = new HapticPattern(LoadedAssetName);
			clone.Sequences = new List<CommonArgs<ParameterizedSequence>>(_children);
			return clone;
		}
		/// <summary>
		/// Returns a string representation of this HapticPattern for debugging purposes, including all child sequences
		/// </summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("Pattern of {0} sequences: \n", this.Sequences.Count));

			foreach (var child in this.Sequences)
			{
				sb.Append(string.Format("{0}\n", child.ToString()));
			}
			sb.Append("\n");
			return sb.ToString();
		}
	}

	/// <summary>
	/// HapticExperiences are containers for one or more HapticPatterns.
	/// </summary>
	public sealed class HapticExperience : SerializableHaptic
	{
		private IList<CommonArgs<ParameterizedPattern>> _children;

		internal IList<CommonArgs<ParameterizedPattern>> Patterns
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
		/// Construct an empty HapticExperience
		/// </summary>
		public HapticExperience() : base("experience")
		{
			_children = new List<CommonArgs<ParameterizedPattern>>();
		}

		public HapticExperience(string lazyLoadAssetPath) : base("experience")
		{
			LoadedAssetName = lazyLoadAssetPath;
			_children = new List<CommonArgs<ParameterizedPattern>>();
		}
		/// <summary>
		/// Add a HapticPattern to this HapticExperience with a given time offset and default strength of 1.0
		/// </summary>
		/// <param name="time">Time offset (fractional seconds)</param>
		/// <param name="area">AreaFlag on which to play the HapticSequence</param>
		/// <param name="sequence">The HapticSequence to be added</param>
		public HapticExperience AddPattern(double time, HapticPattern pattern)
		{
			ParameterizedPattern clone = new ParameterizedPattern(pattern.Clone());
			_children.Add(new CommonArgs<ParameterizedPattern>((float)time, 1f, clone));
			return this;
		}

		/// <summary>
		/// Add a HapticPattern to this HapticExperience with a given time offset and strength.
		/// </summary>
		/// <param name="time">Time offset (fractional seconds)</param>
		/// <param name="area">AreaFlag on which to play the HapticSequence</param>
		/// <param name="strength">Strength of the HapticSequence (0.0 - 1.0)</param>
		/// <param name="sequence">The HapticSequence to be added</param>
		public HapticExperience AddPattern(double time, double strength, HapticPattern pattern)
		{
			ParameterizedPattern clone = new ParameterizedPattern(pattern.Clone());
			_children.Add(new CommonArgs<ParameterizedPattern>((float)time, (float)strength, clone));
			return this;
		}

		/// <summary>
		/// Create a HapticHandle from this HapticExperience, which can be used to manipulate the effect. 
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public HapticHandle CreateHandle()
		{
			HandleLazyAssetLoading();

			EventList e = new ParameterizedExperience(this).Generate(1f, 0f);

			HapticHandle.CommandWithHandle creator = delegate (IntPtr handle)
			{
				e.Transmit(handle);
			};

			return new HapticHandle(creator);
		}

		/// <summary>
		/// Create a HapticHandle from this HapticExperience, passing in a given strength. 
		/// </summary>
		/// <param name="strength"></param>
		/// <returns>A new HapticHandle</returns>
		public HapticHandle CreateHandle(double strength)
		{
			HandleLazyAssetLoading();

			EventList e = new ParameterizedExperience(this).Generate((float)strength, 0f);

			HapticHandle.CommandWithHandle creator = delegate (IntPtr handle)
			{
				e.Transmit(handle);
			};

			return new HapticHandle(creator);
		}

		/// <summary>
		/// <para>If you want to play an experience but don't care about controlling playback, use this method. It will automatically clean up resources.</para>
		/// <para>Synonymous with someExperience.CreateHandle().Play()</para>
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public void Play()
		{
			CreateHandle().Play().Dispose();
		}

		/// <summary>
		/// <para>If you want to play an experience but don't care about controlling playback, use this method. It will automatically clean up resources.</para>
		/// <para>Synonymous with someExperience.CreateHandle(strength).Play().Release()</para>
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public void Play(double strength)
		{
			CreateHandle(strength).Play().Dispose();
		}

		/// <summary>
		/// Internal use: turns an HDF into an experience
		/// </summary>
		/// <param name="hdf"></param>
		internal override void doLoadFromHDF(string key, HapticDefinitionFile hdf)
		{
			var experience_def_array = hdf.experience_definitions[key];
			foreach (var pat in experience_def_array)
			{
				HapticPattern p = new HapticPattern();
				p.doLoadFromHDF(pat.pattern, hdf);
				AddPattern(pat.time, p);
			}
		}

		/// <summary>
		/// Create an independent copy of this HapticExperience
		/// </summary>
		/// <returns></returns>
		public HapticExperience Clone()
		{
			var clone = new HapticExperience(LoadedAssetName);
			clone.Patterns = new List<CommonArgs<ParameterizedPattern>>(_children);
			return clone;
		}
		/// <summary>
		/// Returns a representation of this HapticExperience for debugging purposes, including the representation of child patterns
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("Experience of {0} patterns: \n", this.Patterns.Count));

			foreach (var child in this.Patterns)
			{
				sb.Append(string.Format("{0}\n", child.ToString()));
			}
			sb.Append("\n");
			return sb.ToString();
		}
	}
}