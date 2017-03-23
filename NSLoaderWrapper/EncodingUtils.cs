using NullSpace.HapticFiles;
using System;

using FlatBuffers;
using NullSpace.Events;
using System.Linq;
using System.Collections.Generic;
using NullSpace.SDK.FileUtilities;
using UnityEngine;

namespace NullSpace.SDK.FileUtilities
{
	/// <summary>
	/// Used to turn HapticDefinitionFiles into dynamic haptic effects
	/// Example workflow: 
	/// 1. create a .hdf using the asset tool binary
	/// 2. Deserialize the .hdf into a HapticDefinitionFile
	/// 3. Pass the HapticDefinitionFile to the CodeHapticFactory, along with the key of the root effect
	/// 4. Get a dynamic effect out of it
	/// 
	/// In practice, this is all done automatically by the plugin. 
	/// </summary>
	public class CodeHapticFactory
	{
		
		/// <summary>
		/// Create a HapticSequence from a HapticDefinitionFile
		/// </summary>
		/// <param name="key">Name of the root effect</param>
		/// <param name="hdf">A HapticDefinitionFile containing the root effect</param>
		/// <returns></returns>
		public static HapticSequence CreateSequence(string key, HapticDefinitionFile hdf)
		{
		
			HapticSequence s = new HapticSequence();
			var sequence_def_array = hdf.sequenceDefinitions[key];
			foreach (var effect in sequence_def_array)
			{
				Effect e = FileEffectToCodeEffect.TryParse(effect.effect, Effect.Click);
				s.AddEffect(effect.time, effect.strength, new HapticEffect(e, effect.duration));
			}
			return s;
		}

		/// <summary>
		/// Create a HapticPattern from a HapticDefinitionFile
		/// </summary>
		/// <param name="key">Name of the root effect</param>
		/// <param name="hdf">A HapticDefinitionFile containing the root effect</param>
		/// <returns></returns>
		public static HapticPattern CreatePattern(string key, HapticDefinitionFile hdf)
		{
			HapticPattern p = new HapticPattern();
			var pattern_def_array = hdf.patternDefinitions[key];
			foreach (var seq in pattern_def_array)
			{
				AreaFlag area = new AreaParser(seq.area).GetArea();
				HapticSequence thisSeq = CreateSequence(seq.sequence, hdf);
				p.AddSequence(seq.time, area, thisSeq);
			}
			return p;
		}

		/// <summary>
		/// Create a HapticExperience from a HapticDefinitionFile
		/// </summary>
		/// <param name="key"></param>
		/// <param name="hdf"></param>
		/// <returns></returns>
		public static HapticExperience CreateExperience(string key, HapticDefinitionFile hdf)
		{
			HapticExperience e = new HapticExperience();
			var experience_def_array = hdf.experienceDefinitions[key];
			foreach (var pat in experience_def_array)
			{
				HapticPattern thisPat = CreatePattern(pat.pattern, hdf);
				e.AddPattern(pat.time, thisPat);
			}
			return e;
		}
	}
}
