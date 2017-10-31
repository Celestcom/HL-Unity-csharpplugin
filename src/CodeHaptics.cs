using System.Collections.Generic;
using System.Text;
using Hardlight.SDK.FileUtilities;
using System;
using System.Diagnostics;

namespace Hardlight.SDK
{
	///// <summary>
	///// <para>CodeSequences are haptic effects which play on a given area on the suit. The area is specified with an AreaFlag, which can represent anything from one location to the entire suit.</para>
	///// <para>A HapticSequenceOld is composed of one or more HapticEffects with time offsets.</para>
	///// </summary>
	//public class HapticSequenceOld : SerializableHaptic
	//{
	//	public IList<CommonArgs<HapticEffect>> _children;

	//	internal IList<CommonArgs<HapticEffect>> Effects
	//	{
	//		get { return _children; }
	//		set { _children = value; }
	//	}

	//	/// <summary>
	//	/// Construct an empty HapticSequenceOld
	//	/// </summary>
	//	public HapticSequenceOld() : base("sequence")
	//	{
	//		_children = new List<CommonArgs<HapticEffect>>();
	//	}
	//	public HapticSequenceOld(string lazyLoadAssetPath) : base("sequence")
	//	{
	//		LoadedAssetName = lazyLoadAssetPath;
	//		_children = new List<CommonArgs<HapticEffect>>();
	//	}

	//	/// <summary>
	//	/// Add a HapticEffect with a given time offset
	//	/// </summary>
	//	/// <param name="time">Time offset (fractional seconds)</param>
	//	/// <param name="effect">The HapticEffect to add</param>
	//	public HapticSequenceOld AddEffect(double time, HapticEffect effect)
	//	{
	//		Effects.Add(new CommonArgs<HapticEffect>((float)time, 1f, effect.Clone()));
	//		return this;
	//	}

	//	/// <summary>
	//	/// Add a HapticEffect with a given time offset and strength 
	//	/// </summary>
	//	/// <param name="time">Time offset (fractional seconds)</param>
	//	/// <param name="strength">Strength (0.0-1.0)</param>
	//	/// <param name="effect">The HapticEffect to add</param>
	//	/// <returns></returns>
	//	public HapticSequenceOld AddEffect(double time, double strength, HapticEffect effect)
	//	{
	//		Effects.Add(new CommonArgs<HapticEffect>((float)time, (float)strength, effect.Clone()));
	//		return this;
	//	}

	//	/// <summary>
	//	/// Create a HapticHandle from this HapticSequenceOld, specifying an AreaFlag to play on.
	//	/// </summary>
	//	/// <param name="area">The AreaFlag where this HapticSequenceOld should play</param>
	//	/// <returns>A new HapticHandle bound to this effect playing on the given area</returns>
	//	public unsafe HapticHandle CreateHandle(AreaFlag area)
	//	{
	//		return CreateHandle(area, 1.0f);
	//	}

	//	/// <summary>
	//	/// Create a HapticHandle for this HapticSequenceOld, specifying an AreaFlag and a strength.
	//	/// </summary>
	//	/// <param name="area">The AreaFlag where this HapticSequenceOld should play</param>
	//	/// <param name="strength">The strength of this HapticSequenceOld (0.0-1.0)</param>
	//	/// <returns>A new HapticHandle bound to this effect playing on the given area</returns>
	//	public unsafe HapticHandle CreateHandle(AreaFlag area, double strength)
	//	{
	//		HandleLazyAssetLoading();

	//		UnityEngine.Debug.LogError("Broken\n");
	//		EventList e = new ParameterizedSequence(null, area).Generate((float)strength, 0f);
	//		HapticHandle.CommandWithHandle creator = delegate (HLVR_Effect* handle)
	//		{
	//			e.Transmit(handle);
	//		};

	//		return new HapticHandle(creator);
	//	}

	//	/// <summary>
	//	/// <para>If you want to play a sequence but don't care about controlling playback, use this method. It will automatically clean up resources.</para>
	//	/// <para>Synonymous with someSequence.CreateHandle(area).Play().Release() </para>
	//	/// </summary>
	//	/// <param name="area">The area on which to play this sequence</param>
	//	/// <returns>A new HapticHandle bound to this effect playing on the given area</returns>
	//	public void Play(AreaFlag area)
	//	{
	//		CreateHandle(area).Play().Dispose();
	//	}

	//	/// <summary>
	//	/// <para>A helper which calls Play on a newly created HapticHandle.</para>
	//	/// <para>Synonymous with someSequence.CreateHandle(area, strength).Play()</para>
	//	/// </summary>
	//	/// <param name="area">The area on which to play this sequence</param>
	//	/// <param name="strength">The strength with which to play this sequence</param>
	//	/// <returns>A new HapticHandle bound to this effect playing on the given area</returns>
	//	public HapticHandle Play(AreaFlag area, double strength)
	//	{
	//		return CreateHandle(area, strength).Play();
	//	}

	//	/// <summary>
	//	/// Internal use: turns an HDF into a sequence
	//	/// </summary>
	//	/// <param name="hdf"></param>
	//	internal override void doLoadFromHDF(string key, HapticDefinitionFile hdf)
	//	{
	//		var sequence_def_array = hdf.sequence_definitions[key];
	//		foreach (var effect in sequence_def_array)
	//		{
	//			Effect e = FileEffectToCodeEffect.TryParse(effect.effect, Effect.Click);
	//			this.AddEffect(effect.time, effect.strength, new HapticEffect(e, effect.duration));
	//		}
	//	}

	//	/// <summary>
	//	/// Create an independent copy of this HapticSequenceOld
	//	/// </summary>
	//	/// <returns></returns>
	//	public HapticSequenceOld Clone()
	//	{
	//		var clone = new HapticSequenceOld(LoadedAssetName);
	//		clone.Effects = new List<CommonArgs<HapticEffect>>(_children);
	//		return clone;
	//	}
	//	/// <summary>
	//	/// Returns a string representation of this HapticSequenceOld for debugging purposes, including all child effects
	//	/// </summary>
	//	/// <returns></returns>
	//	public override string ToString()
	//	{
	//		StringBuilder sb = new StringBuilder();
	//		sb.Append(string.Format("Sequence of {0} HapticEffects:\n", this.Effects.Count));
	//		foreach (var child in this.Effects)
	//		{
	//			sb.Append(string.Format("[{0}]\n", child.ToString()));
	//		}
	//		sb.Append("\n");
	//		return sb.ToString();
	//	}
	//}

	///// <summary>
	///// <para>HapticPatternOlds are used to combine one or more HapticSequenceOlds into a single, playable effect. Each HapticSequenceOld added to the HapticPatternOld will have a time offset and optional strength, as well as a specified area.
	///// </summary>
	//public class HapticPatternOld : SerializableHaptic
	//{
	//	public IList<CommonArgs<ParameterizedSequence>> _children;

	//	internal IList<CommonArgs<ParameterizedSequence>> Sequences
	//	{
	//		get
	//		{
	//			return _children;
	//		}

	//		set
	//		{
	//			_children = value;
	//		}
	//	}

	//	/// <summary>
	//	/// Construct an empty HapticPatternOld
	//	/// </summary>
	//	public HapticPatternOld() : base("pattern")
	//	{
	//		_children = new List<CommonArgs<ParameterizedSequence>>();
	//	}

	//	public HapticPatternOld(string lazyLoadAssetPath) : base("pattern")
	//	{
	//		LoadedAssetName = lazyLoadAssetPath;
	//		_children = new List<CommonArgs<ParameterizedSequence>>();
	//	}/// <summary>
	//	 /// Add a HapticSequenceOld to this HapticPatternOld with a given time offset and AreaFlag, and default strength of 1.0
	//	 /// </summary>
	//	 /// <param name="time">Time offset (fractional seconds)</param>
	//	 /// <param name="area">AreaFlag on which to play the HapticSequenceOld</param>
	//	 /// <param name="sequence">The HapticSequenceOld to be added</param>
	//	public HapticPatternOld AddSequence(double time, AreaFlag area, HapticSequenceOld sequence)
	//	{
	//		//ParameterizedSequence clone = new ParameterizedSequence(sequence.Clone(), area);
	//		//_children.Add(new CommonArgs<ParameterizedSequence>((float)time, 1f, clone));
	//		return this;
	//	}

	//	/// <summary>
	//	/// Add a HapticSequenceOld to this HapticPatternOld with a given time offset, AreaFlag, and strength.
	//	/// </summary>
	//	/// <param name="time">Time offset (fractional seconds)</param>
	//	/// <param name="area">AreaFlag on which to play the HapticSequenceOld</param>
	//	/// <param name="strength">Strength of the HapticSequenceOld (0.0 - 1.0)</param>
	//	/// <param name="sequence">The HapticSequenceOld to be added</param>
	//	public HapticPatternOld AddSequence(double time, AreaFlag area, double strength, HapticSequenceOld sequence)
	//	{
	//		//ParameterizedSequence clone = new ParameterizedSequence(sequence.Clone(), area);
	//		//_children.Add(new CommonArgs<ParameterizedSequence>((float)time, (float)strength, clone));
	//		return this;
	//	}

	//	/// <summary>
	//	/// Create a HapticHandle from this HapticPatternOld, which can be used to manipulate the effect. 
	//	/// </summary>
	//	/// <returns>A new HapticHandle</returns>
	//	public unsafe HapticHandle CreateHandle()
	//	{
	//		return CreateHandle(1.0f);
	//	}

	//	/// <summary>
	//	/// Create a HapticHandle from this HapticPatternOld, passing in a given strength. 
	//	/// </summary>
	//	/// <param name="strength"></param>
	//	/// <returns>A new HapticHandle</returns>
	//	public unsafe HapticHandle CreateHandle(double strength)
	//	{
	//		HandleLazyAssetLoading();

	//		UnityEngine.Debug.LogError("Switched parameter to null\n");
	//		EventList e = new ParameterizedPattern(null).Generate((float)strength, 0f);

	//		HapticHandle.CommandWithHandle creator = delegate (HLVR_Effect* handle)
	//		{
	//			e.Transmit(handle);

	//		};

	//		return new HapticHandle(creator);
	//	}

	//	/// <summary>
	//	/// <para>If you want to play a pattern but don't care about controlling playback, use this method. It will automatically clean up resources.</para>
	//	/// <para>Synonymous with somePattern.CreateHandle().Play().Release()</para>
	//	/// </summary>
	//	/// <returns>A new HapticHandle</returns>
	//	public void Play()
	//	{
	//		CreateHandle().Play().Dispose();
	//	}

	//	/// <summary>
	//	/// <para>If you want to play a pattern but don't care about controlling playback, use this method. It will automatically clean up resources.</para>
	//	/// <para>Synonymous with somePattern.CreateHandle(strength).Play().Release()</para>
	//	/// </summary>
	//	/// <returns>A new HapticHandle</returns>
	//	public void Play(double strength)
	//	{
	//		CreateHandle(strength).Play().Dispose();
	//	}

	//	/// <summary>
	//	/// Internal use: turns an HDF into a pattern
	//	/// </summary>
	//	/// <param name="hdf"></param>
	//	internal override void doLoadFromHDF(string key, HapticDefinitionFile hdf)
	//	{
	//		var pattern_def_array = hdf.pattern_definitions[key];
	//		foreach (var seq in pattern_def_array)
	//		{
	//			AreaFlag area = new AreaParser(seq.area).GetArea();
	//			HapticSequenceOld thisSeq = new HapticSequenceOld();
	//			thisSeq.doLoadFromHDF(seq.sequence, hdf);
	//			AddSequence(seq.time, area, thisSeq);
	//		}
	//	}

	//	/// <summary>
	//	/// Create an independent copy of this HapticPatternOld
	//	/// </summary>
	//	/// <returns></returns>
	//	public HapticPatternOld Clone()
	//	{
	//		var clone = new HapticPatternOld(LoadedAssetName);
	//		clone.Sequences = new List<CommonArgs<ParameterizedSequence>>(_children);
	//		return clone;
	//	}
	//	/// <summary>
	//	/// Returns a string representation of this HapticPatternOld for debugging purposes, including all child sequences
	//	/// </summary>
	//	public override string ToString()
	//	{
	//		StringBuilder sb = new StringBuilder();
	//		sb.Append(string.Format("Pattern of {0} sequences: \n", this.Sequences.Count));

	//		foreach (var child in this.Sequences)
	//		{
	//			sb.Append(string.Format("{0}\n", child.ToString()));
	//		}
	//		sb.Append("\n");
	//		return sb.ToString();
	//	}
	//}

	///// <summary>
	///// <para>HapticExperienceOlds are containers for one or more HapticPatternOlds.
	///// </summary>
	//public class HapticExperienceOld : SerializableHaptic
	//{
	//	public IList<CommonArgs<ParameterizedPattern>> _children;

	//	internal IList<CommonArgs<ParameterizedPattern>> Patterns
	//	{
	//		get
	//		{
	//			return _children;
	//		}

	//		set
	//		{
	//			_children = value;
	//		}
	//	}

	//	/// <summary>
	//	/// Construct an empty HapticExperienceOld
	//	/// </summary>
	//	public HapticExperienceOld() : base("experience")
	//	{
	//		_children = new List<CommonArgs<ParameterizedPattern>>();
	//	}

	//	public HapticExperienceOld(string lazyLoadAssetPath) : base("experience")
	//	{
	//		LoadedAssetName = lazyLoadAssetPath;
	//		_children = new List<CommonArgs<ParameterizedPattern>>();
	//	}
	//	/// <summary>
	//	/// Add a HapticPatternOld to this HapticExperienceOld with a given time offset and default strength of 1.0
	//	/// </summary>
	//	/// <param name="time">Time offset (fractional seconds)</param>
	//	/// <param name="area">AreaFlag on which to play the HapticSequenceOld</param>
	//	/// <param name="sequence">The HapticSequenceOld to be added</param>
	//	public HapticExperienceOld AddPattern(double time, HapticPatternOld pattern)
	//	{
	//		//ParameterizedPattern clone = new ParameterizedPattern(pattern.Clone());
	//		//_children.Add(new CommonArgs<ParameterizedPattern>((float)time, 1f, clone));
	//		return this;
	//	}

	//	/// <summary>
	//	/// Add a HapticPatternOld to this HapticExperienceOld with a given time offset and strength.
	//	/// </summary>
	//	/// <param name="time">Time offset (fractional seconds)</param>
	//	/// <param name="area">AreaFlag on which to play the HapticSequenceOld</param>
	//	/// <param name="strength">Strength of the HapticSequenceOld (0.0 - 1.0)</param>
	//	/// <param name="sequence">The HapticSequenceOld to be added</param>
	//	public HapticExperienceOld AddPattern(double time, double strength, HapticPatternOld pattern)
	//	{
	//		UnityEngine.Debug.LogError("Removed pattern adding\n");

	//		//ParameterizedPattern clone = new ParameterizedPattern(pattern.Clone());
	//		//_children.Add(new CommonArgs<ParameterizedPattern>((float)time, (float)strength, clone));
	//		return this;
	//	}

	//	/// <summary>
	//	/// Create a HapticHandle from this HapticExperienceOld, which can be used to manipulate the effect. 
	//	/// </summary>
	//	/// <returns>A new HapticHandle</returns>
	//	public unsafe HapticHandle CreateHandle()
	//	{
	//		return CreateHandle(1.0f);
	//	}

	//	/// <summary>
	//	/// Create a HapticHandle from this HapticExperienceOld, passing in a given strength. 
	//	/// </summary>
	//	/// <param name="strength"></param>
	//	/// <returns>A new HapticHandle</returns>
	//	public unsafe HapticHandle CreateHandle(double strength)
	//	{
	//		HandleLazyAssetLoading();

	//		UnityEngine.Debug.LogError("Switched parameter to null\n");
	//		EventList e = new ParameterizedExperience(null).Generate((float)strength, 0f);

	//		HapticHandle.CommandWithHandle creator = delegate (HLVR_Effect* handle)
	//		{
	//			e.Transmit(handle);
	//		};

	//		return new HapticHandle(creator);
	//	}

	//	/// <summary>
	//	/// <para>If you want to play an experience but don't care about controlling playback, use this method. It will automatically clean up resources.</para>
	//	/// <para>Synonymous with someExperience.CreateHandle().Play()</para>
	//	/// </summary>
	//	/// <returns>A new HapticHandle</returns>
	//	public void Play()
	//	{
	//		CreateHandle().Play().Dispose();
	//	}

	//	/// <summary>
	//	/// <para>If you want to play an experience but don't care about controlling playback, use this method. It will automatically clean up resources.</para>
	//	/// <para>Synonymous with someExperience.CreateHandle(strength).Play().Release()</para>
	//	/// </summary>
	//	/// <returns>A new HapticHandle</returns>
	//	public void Play(double strength)
	//	{
	//		CreateHandle(strength).Play().Dispose();
	//	}

	//	/// <summary>
	//	/// Internal use: turns an HDF into an experience
	//	/// </summary>
	//	/// <param name="hdf"></param>
	//	internal override void doLoadFromHDF(string key, HapticDefinitionFile hdf)
	//	{
	//		var experience_def_array = hdf.experience_definitions[key];
	//		foreach (var pat in experience_def_array)
	//		{
	//			HapticPatternOld p = new HapticPatternOld();
	//			p.doLoadFromHDF(pat.pattern, hdf);
	//			AddPattern(pat.time, p);
	//		}
	//	}

	//	/// <summary>
	//	/// Create an independent copy of this HapticExperienceOld
	//	/// </summary>
	//	/// <returns></returns>
	//	public HapticExperienceOld Clone()
	//	{
	//		var clone = new HapticExperienceOld(LoadedAssetName);
	//		clone.Patterns = new List<CommonArgs<ParameterizedPattern>>(_children);
	//		return clone;
	//	}
	//	/// <summary>
	//	/// Returns a representation of this HapticExperienceOld for debugging purposes, including the representation of child patterns
	//	/// </summary>
	//	/// <returns></returns>
	//	public override string ToString()
	//	{
	//		StringBuilder sb = new StringBuilder();
	//		sb.Append(string.Format("Experience of {0} patterns: \n", this.Patterns.Count));

	//		foreach (var child in this.Patterns)
	//		{
	//			sb.Append(string.Format("{0}\n", child.ToString()));
	//		}
	//		sb.Append("\n");
	//		return sb.ToString();
	//	}
	//}
}