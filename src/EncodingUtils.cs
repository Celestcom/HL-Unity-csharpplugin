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
	/// 4. Get a ScriptableObjectHaptic out of the code haptic factory.
	/// 
	/// In practice, this is all done automatically by the plugin. 
	/// </summary>
	public class CodeHapticFactory
	{
		/// <summary>
		/// A class which exists as the Value in LoadedSequences dictionary to prevent duplicate HapticSequence creation from ones that already exist.
		/// </summary>
		public class SequenceImportData
		{
			public string SequenceKey;
			public HapticSequence Sequence;
			public bool saved = false;

			public SequenceImportData(HapticSequence seq, string key)
			{
				Sequence = seq;
				saved = false;
				SequenceKey = key;
			}
		}
		/// <summary>
		/// A class which exists as the Value in LoadedPatterns dictionary to prevent duplicate HapticPattern creation from ones that already exist.
		/// </summary>
		public class PatternImportData
		{
			public string PatternKey;
			public HapticPattern Pattern;
			public bool saved = false;

			public PatternImportData(HapticPattern pat, string key)
			{
				Pattern = pat;
				saved = false;
				PatternKey = key;
			}
		}

		private static Dictionary<string, SequenceImportData> _loadedSequences = new Dictionary<string, SequenceImportData>();
		private static Dictionary<string, SequenceImportData> LoadedSequences
		{
			get
			{
				if (_loadedSequences == null)
					_loadedSequences = new Dictionary<string, SequenceImportData>();
				return _loadedSequences;
			}

			set
			{
				_loadedSequences = value;
			}
		}

		private static Dictionary<string, PatternImportData> _loadedPatterns = new Dictionary<string, PatternImportData>();
		private static Dictionary<string, PatternImportData> LoadedPatterns
		{
			get
			{
				if (_loadedPatterns == null)
					_loadedPatterns = new Dictionary<string, PatternImportData>();
				return _loadedPatterns;
			}

			set
			{
				_loadedPatterns = value;
			}
		}

		public static void EnsureSequenceIsRemembered(string key, HapticSequence sequence)
		{
			key = HapticResources.CleanName(key);
			if (!LoadedSequences.ContainsKey(key) && sequence != null)
			{
				LoadedSequences.Add(key, new SequenceImportData(sequence, key));
			}
		}
		public static void EnsurePatternIsRemembered(string key, HapticPattern pattern)
		{
			key = HapticResources.CleanName(key);
			if (!LoadedPatterns.ContainsKey(key) && pattern != null)
			{
				LoadedPatterns.Add(key, new PatternImportData(pattern, key));
			}
		}

		public static SequenceImportData GetRememberedSequence(string key)
		{
			return LoadedSequences[key];
		}
		public static PatternImportData GetRememberedPattern(string key)
		{
			return LoadedPatterns[key];
		}

		public static bool SequenceExists(string key)
		{
			if (LoadedSequences.ContainsKey(key) && LoadedSequences[key].Sequence != null)
			{
				return true;
			}
			LoadedSequences.Remove(key);
			return false;
		}
		public static bool PatternExists(string key)
		{
			if (LoadedPatterns.ContainsKey(key) && LoadedPatterns[key].Pattern != null)
			{
				return true;
			}
			LoadedPatterns.Remove(key);
			return false;
		}

		/// <summary>
		/// Create a HapticSequence from a HapticDefinitionFile
		/// </summary>
		/// <param name="key">Name of the root effect</param>
		/// <param name="hdf">A HapticDefinitionFile containing the root effect</param>
		/// <returns></returns>
		public static HapticSequence CreateSequenceFromHDF(string key, HapticDefinitionFile hdf)
		{
			string cleanedKey = HapticResources.CleanName(key);
			if (LoadedSequences.ContainsKey(cleanedKey))
			{
				//Debug.Log("Sequence: " + cleanedKey + " already exists, returning it instead of needless reconstruction\n");
				return LoadedSequences[cleanedKey].Sequence;
			}
			//Debug.Log("Sequence: " + cleanedKey + " DOES NOT exist, creating a new one\n");

			HapticSequence seq = ScriptableObject.CreateInstance<HapticSequence>();
			var sequence_def_array = hdf.sequence_definitions[key];
			foreach (var effect in sequence_def_array)
			{
				seq.AddEffect(new HapticEffect(effect.ParseEffect(), effect.time, effect.duration, effect.strength));
			}
			EnsureSequenceIsRemembered(cleanedKey, seq);
			return seq;
		}

		/// <summary>
		/// Create a HapticPattern from a HapticDefinitionFile
		/// </summary>
		/// <param name="key">Name of the root effect</param>
		/// <param name="hdf">A HapticDefinitionFile containing the root effect</param>
		/// <returns></returns>
		public static HapticPattern CreatePatternFromHDF(string key, HapticDefinitionFile hdf)
		{
			string cleanedKey = HapticResources.CleanName(key);
			if (LoadedPatterns.ContainsKey(cleanedKey))
			{
				//Debug.Log("Pattern: " + cleanedKey + " already exists, returning it instead of needless reconstruction\n");
				return LoadedPatterns[cleanedKey].Pattern;
			}

			HapticPattern pat = ScriptableObject.CreateInstance<HapticPattern>();
			var pattern_def_array = hdf.pattern_definitions[key];

			foreach (var element in pattern_def_array)
			{
				//Debug.Log("Pattern Def Array: " + key + "  " + element.sequence + "\n");
				HapticSequence thisSeq = CreateSequenceFromHDF(element.sequence, hdf);
				thisSeq.name = element.sequence;

				ParameterizedSequence paraSeq = new ParameterizedSequence(thisSeq, element.ParseAreaFlag(), element.time, element.strength);
				pat.AddSequence(paraSeq);
			}

			EnsurePatternIsRemembered(cleanedKey, pat);
			return pat;
		}

		/// <summary>
		/// Create a HapticExperience from a HapticDefinitionFile
		/// </summary>
		/// <param name="key"></param>
		/// <param name="hdf"></param>
		/// <returns></returns>
		public static HapticExperience CreateExperienceFromHDF(string key, HapticDefinitionFile hdf)
		{
			string cleanedKey = HapticResources.CleanName(key);

			HapticExperience exp = ScriptableObject.CreateInstance<HapticExperience>();
			var experience_def_array = hdf.experience_definitions[key];

			foreach (var element in experience_def_array)
			{
				HapticPattern thisPat = CreatePatternFromHDF(element.pattern, hdf);
				thisPat.name = element.pattern;

				ParameterizedPattern paraPat = new ParameterizedPattern(thisPat, element.time, element.strength);
				exp.AddPattern(paraPat);
			}

			return exp;
		}
	}
}