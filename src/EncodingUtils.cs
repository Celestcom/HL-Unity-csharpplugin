using System;

using Hardlight.Events;
using System.Linq;
using System.Collections.Generic;
using Hardlight.SDK.FileUtilities;
using UnityEngine;

namespace Hardlight.SDK.FileUtilities
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
			HapticSequence s = ScriptableObject.CreateInstance<HapticSequence>();
			var sequence_def_array = hdf.sequence_definitions[key];
			foreach (var effect in sequence_def_array)
			{
				s.AddEffect(new HapticEffect(effect.ParseEffect(), effect.time, effect.duration, effect.strength));
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
			HapticPattern p = ScriptableObject.CreateInstance<HapticPattern>();
			var pattern_def_array = hdf.pattern_definitions[key];
			Dictionary<string, HapticSequence> usedSequences = new Dictionary<string, HapticSequence>();
			foreach (var element in pattern_def_array)
			{
				HapticSequence thisSeq = null;
				if (usedSequences.ContainsKey(element.sequence))
				{
					thisSeq = usedSequences[element.sequence];
				}

				if (thisSeq == null)
				{
					thisSeq = CreateSequence(element.sequence, hdf);
					thisSeq.name = element.sequence;

					usedSequences.Add(element.sequence, thisSeq);
				}

				ParameterizedSequence paraSeq = new ParameterizedSequence(thisSeq, element.ParseAreaFlag(), element.time, element.strength);
				p.AddSequence(paraSeq);
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
			HapticExperience e = ScriptableObject.CreateInstance<HapticExperience>();
			var experience_def_array = hdf.experience_definitions[key];
			foreach (var pat in experience_def_array)
			{
				HapticPattern thisPat = CreatePattern(pat.pattern, hdf);

				ParameterizedPattern paraPat = new ParameterizedPattern(thisPat, pat.time, pat.strength);
				e.AddPattern(paraPat);
			}
			return e;
		}
	}
}
