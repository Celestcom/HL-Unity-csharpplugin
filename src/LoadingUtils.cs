using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Hardlight.SDK.FileUtilities
{
	public static class HapticResources
	{
		private static bool Exists(string enginePath, string desiredPathAndName)
		{
			bool normalExists = File.Exists((enginePath + desiredPathAndName));
			//bool lowerExists = File.Exists((enginePath + desiredPathAndName).ToLower());
			//Debug.Log("looking at " + desiredPathAndName + "\tNormalExists: " + normalExists + "\n\tLowerExists:" + lowerExists + "\n");
			return normalExists;
		}
		private static string GetEnginePath()
		{
			string enginePath = Application.dataPath;
			//this removes the Path/[Assets] from the Application.dataPath;
			enginePath = Application.dataPath.Remove(enginePath.Length - 6, 6);
			return enginePath;
		}
		private static string TryToFindAvailableFileName(string desiredPathAndName, string extension = ".asset")
		{
			string enginePath = GetEnginePath();
			bool exists = Exists(enginePath, desiredPathAndName + extension);

			desiredPathAndName = RemoveExtension(desiredPathAndName, extension);
			//Debug.Log("Desired Path: " + desiredPathAndName + " " + exists + "\n" + enginePath + "\n" + enginePath + desiredPathAndName);

			bool succeeded = true;
			int extraChecks = 2;
			if (exists)
			{
				succeeded = false;
				for (int i = 0; i < extraChecks; i++)
				{
					exists = File.Exists(enginePath + desiredPathAndName + " " + i + extension);
					//Debug.Log("Checking for: " + enginePath + desiredPathAndName + " " + i + extension + "\n" + exists);
					if (!exists)
					{
						desiredPathAndName = desiredPathAndName + " " + i;
						succeeded = true;
						break;
					}
				}
			}

			if (!succeeded)
			{
				throw new HapticsAssetException("Could not create asset for sequence [" + desiredPathAndName + "]\nToo many similarly named files (checked " + extraChecks + " files)...\n\tSuggestion: clean up some excessive names and try again.");
			}

			//Debug.Log("We have found a valid name: " + desiredPathAndName + extension + "\n");

			return desiredPathAndName + extension;
		}
		private static AssetTool assetTool;
		public static AssetTool HapticAssetTool
		{
			get
			{
				if (assetTool == null)
				{
					assetTool = new AssetTool();
					assetTool.SetRootHapticsFolder(UnityEngine.Application.streamingAssetsPath + "/Haptics/");
				}
				return assetTool;
			}
		}

		/// <summary>
		/// Returns whether or not the key has the '.sequence' file extension
		/// </summary>
		/// <param name="key"></param>
		public static bool IsSequence(string key)
		{
			return key.Contains(".sequence");
		}
		/// <summary>
		/// Returns whether or not the key has the '.pattern' file extension
		/// </summary>
		/// <param name="key"></param>
		public static bool IsPattern(string key)
		{
			return key.Contains(".pattern");
		}
		/// <summary>
		/// Returns whether or not the key has the '.experience' file extension
		/// </summary>
		/// <param name="key"></param>
		public static bool IsExperience(string key)
		{
			return key.Contains(".experience");
		}

		/// <summary>
		/// Attempts to create a haptic sequence from the provided json path
		/// It will create a HDF and then turn the hdf into a HapticSequence.
		/// </summary>
		/// <param name="jsonPath">Ex: StreamingAssets/Haptics/NS Demos/sequences/click.sequence</param>
		public static HapticSequence CreateSequence(string jsonPath)
		{
			var fileName = Path.GetFileNameWithoutExtension(jsonPath);

			////If we don't replace . with _, then Unity has serious trouble locating the file
			HapticSequence seq = null;

			bool isSeq = IsSequence(jsonPath);
			bool isPat = IsPattern(jsonPath);
			bool isExp = IsExperience(jsonPath);
			//Debug.Log("Attemtping haptic asset import: " + jsonPath + " " + isSeq + "\n" + fileName + "\n\n" + "\n");

			if (isSeq)
			{
				seq = LoadSequenceFromJson(jsonPath);
				seq.name = CleanName(fileName);
			}
			else if (isPat)
			{
				Debug.LogError("Attempted to run a HapticResources.CreatePattern while providing a pattern at path: " + jsonPath + "\n\t");
			}
			else if (isExp)
			{
				Debug.LogError("Attempted to run a HapticResources.CreateSequence while providing a experience at path: " + jsonPath + "\n\t");
			}

			return seq;
		}

		/// <summary>
		/// Attempts to create a haptic pattern from the provided json path
		/// It will create a HDF and then turn the hdf into a HapticPattern.
		/// </summary>
		/// <param name="jsonPath">Ex: StreamingAssets/Haptics/NS Demos/patterns/mech_stomp_left.pattern</param>
		public static HapticPattern CreatePattern(string jsonPath)
		{
			var fileName = Path.GetFileNameWithoutExtension(jsonPath);

			////If we don't replace . with _, then Unity has serious trouble locating the file
			HapticPattern pat = null;

			bool isSeq = IsSequence(jsonPath);
			bool isPat = IsPattern(jsonPath);
			bool isExp = IsExperience(jsonPath);

			if (isPat)
			{
				pat = LoadPatternFromJson(jsonPath);
				pat.name = CleanName(fileName);
			}
			else if (isSeq)
			{
				Debug.LogError("Attempted to run a HapticPattern.CreateAsset while providing a sequence at path: " + jsonPath + "\n\t");
			}
			else if (isExp)
			{
				Debug.LogError("Attempted to run a HapticPattern.CreateAsset while providing a experience at path: " + jsonPath + "\n\t");
			}

			return pat;
		}

		/// <summary>
		/// Attempts to create a haptic experience from the provided json path
		/// It will create a HDF and then turn the hdf into a HapticExperience.
		/// </summary>
		/// <param name="jsonPath">Ex: StreamingAssets/Haptics/NS Demos/experiences/mech_stomp.experience</param>
		public static HapticExperience CreateExperience(string jsonPath)
		{
			var fileName = Path.GetFileNameWithoutExtension(jsonPath);

			////If we don't replace . with _, then Unity has serious trouble locating the file
			HapticExperience exp = null;

			bool isSeq = IsSequence(jsonPath);
			bool isPat = IsPattern(jsonPath);
			bool isExp = IsExperience(jsonPath);

			if (isExp)
			{
				exp = LoadExperienceFromJson(jsonPath);
				exp.name = CleanName(fileName);
			}
			else if (isSeq)
			{
				Debug.LogError("Attempted to run a HapticPattern.CreateAsset while providing a sequence at path: " + jsonPath + "\n\t");
			}
			else if (isPat)
			{
				Debug.LogError("Attempted to run a HapticPattern.CreateAsset while providing a pattern at path: " + jsonPath + "\n\t");
			}

			return exp;
		}

		internal static HapticSequence LoadSequenceFromJson(string jsonPath)
		{
			HapticDefinitionFile hdf = LoadHDFFromJson(jsonPath);

			if (hdf.root_effect.type == "sequence")
			{
				var seq = CodeHapticFactory.CreateSequenceFromHDF(hdf.root_effect.name, hdf);
				return seq;
			}
			else
			{
				Debug.LogError("Error in LoadSequenceFromJson - likely an invalid path was provided\n\t" + hdf.root_effect.name + " is of type " + hdf.root_effect.type + "\n");
				return HapticSequence.CreateNew();
				//throw new InvalidOperationException("Unable to load " + hdf.root_effect.name + " as a HapticSequence because it is a " + hdf.root_effect.type);
			}
		}
		internal static HapticPattern LoadPatternFromJson(string jsonPath)
		{
			HapticDefinitionFile hdf = LoadHDFFromJson(jsonPath);
			if (hdf.root_effect.type == "pattern")
			{
				var pat = CodeHapticFactory.CreatePatternFromHDF(hdf.root_effect.name, hdf);
				return pat;
			}
			else
			{
				Debug.LogError("Error in LoadPatternFromJson - likely an invalid path was provided\n\t" + hdf.root_effect.name + " is of type " + hdf.root_effect.type + "\n");
				return HapticPattern.CreateNew();
				//throw new InvalidOperationException("Unable to load " + hdf.root_effect.name + " as a HapticPattern because it is a " + hdf.root_effect.type);
			}
		}
		internal static HapticExperience LoadExperienceFromJson(string jsonPath)
		{
			HapticDefinitionFile hdf = LoadHDFFromJson(jsonPath);
			if (hdf.root_effect.type == "experience")
			{
				var exp = CodeHapticFactory.CreateExperienceFromHDF(hdf.root_effect.name, hdf);
				return exp;
			}
			else
			{
				Debug.LogError("Error in LoadExperienceFromJson - likely an invalid path was provided\n\t" + hdf.root_effect.name + " is of type " + hdf.root_effect.type + "\n");
				return HapticExperience.CreateNew();
				//throw new InvalidOperationException("Unable to load " + hdf.root_effect.name + " as a HapticExperience because it is a " + hdf.root_effect.type);
			}
		}

		internal static HapticDefinitionFile LoadHDFFromJson(string jsonPath)
		{
			var hdf = HapticAssetTool.GetHapticDefinitionFile(jsonPath);

			if (hdf == null)
			{
				Debug.LogWarning("Unable to load haptic resource at path " + jsonPath);
			}
			return hdf;
		}

		public static void SaveSequence(string name, HapticSequence sequence)
		{
#if UNITY_EDITOR
			if (sequence != null)
			{
				name = CleanName(name);
				CodeHapticFactory.EnsureSequenceIsRemembered(name, sequence);
				if (CodeHapticFactory.SequenceExists(name))
				{
					var seqData = CodeHapticFactory.GetRememberedSequence(name);
					if (!seqData.saved)
					{
						string assetPath = GetDefaultSavePath();
						string finalizedPath = TryToFindAvailableFileName(assetPath + name, GetAssetExtension());

						//Debug.Log("Seq  " + name + " - Finalized Path: " + finalizedPath + "\n");

						if (!AssetDatabase.Contains(sequence))
						{
							seqData.saved = true;
							//Debug.Log("We are saving sequence: " + sequence.name + "\nAttempting: [" + finalizedPath + "]  " + Exists("", finalizedPath) + "\n");
							AssetDatabase.CreateAsset(sequence, finalizedPath);
							//Debug.Log("Asset was created - " + File.Exists(finalizedPath) + " does the lower case return true: " + File.Exists(finalizedPath.ToLower()) + "\n");
						}
						else
						{
							//Debug.LogError("Not saving: " + name + " sequence because it already exists?");
						}
						AssetDatabase.SetLabels(sequence, new string[] { "Haptics", "Sequence" });
						Selection.activeObject = sequence;
					}
					//else
					//	Debug.Log("Not saving: " + name + " sequence because we think it has been saved already");
				}
				//else
				//	Debug.LogError("Not saving: " + name + " sequence because it isn't in our sequence memory dictionary");
			}
			else
				Debug.LogError("Attempted to save a null sequence asset [" + name + "]\n");
#endif
		}
		public static void SavePattern(string name, HapticPattern pattern)
		{
#if UNITY_EDITOR
			if (pattern != null)
			{
				//Debug.Log(name + "  " + pattern.name);
				name = CleanName(name);
				CodeHapticFactory.EnsurePatternIsRemembered(name, pattern);
				//Debug.Log("Looking for: " + CleanName(pattern.name) + "\n");
				if (CodeHapticFactory.PatternExists(name))
				{
					var patData = CodeHapticFactory.GetRememberedPattern(name);
					if (!patData.saved)
					{
			#region Save Required Elements
						for (int i = 0; i < pattern.Sequences.Count; i++)
						{
							if (pattern.Sequences[i] != null)
							{
								string key = pattern.Sequences[i].Sequence.name;
								SaveSequence(key, pattern.Sequences[i].Sequence);
							}
							else
							{
								Debug.Log(name + " has null sequences at " + i + "\n");
							}
						}
			#endregion

			#region Save Self
						string assetPath = GetDefaultSavePath();
						string finalizedPath = TryToFindAvailableFileName(assetPath + name, GetAssetExtension());

						//Debug.Log("Pat  " + name + " - " + finalizedPath + "\n");

						if (!AssetDatabase.Contains(pattern))
						{
							patData.saved = true;
							AssetDatabase.CreateAsset(pattern, finalizedPath);
							//Debug.Log("Asset was created - " + finalizedPath + "  " + File.Exists(finalizedPath) + " does the lower case return true: " + File.Exists(finalizedPath.ToLower()) + "\n");
						}
						AssetDatabase.SetLabels(pattern, new string[] { "Haptics", "Pattern" });
						Selection.activeObject = pattern;
			#endregion
					}
				}
			}
			else
				Debug.LogError("Attempted to save a null pattern asset [" + name + "]\n");
#endif
		}
		public static void SaveExperience(string name, HapticExperience experience)
		{
#if UNITY_EDITOR
			if (experience != null)
			{
			#region Save Required Elements
				for (int i = 0; i < experience.Patterns.Count; i++)
				{
					string key = experience.Patterns[i].Pattern.name;
					SavePattern(key, experience.Patterns[i].Pattern);
				}
			#endregion

			#region Save Self
				string assetPath = GetDefaultSavePath();
				name = CleanName(name);

				string finalizedPath = TryToFindAvailableFileName(assetPath + name, GetAssetExtension());

				if (!AssetDatabase.Contains(experience))
				{
					AssetDatabase.CreateAsset(experience, finalizedPath);
				}
				AssetDatabase.SetLabels(experience, new string[] { "Haptics", "Experience" });
				Selection.activeObject = experience;
			#endregion
			}
			else
				Debug.LogError("Attempted to save a null experience asset [" + name + "]\n");
#endif
		}

		public static string CleanName(string name)
		{
			return StripNamespace(RemoveExtension(name));
		}
		private static string RemoveExtension(string filePath, string extension = ".asset")
		{
			string[] ext = new string[1];
			ext[0] = extension;
			if (filePath.Contains(extension))
			{
				return filePath.Split(ext, StringSplitOptions.RemoveEmptyEntries)[0];
			}
			return filePath;
		}
		private static string StripNamespace(string key)
		{
			//string output = "Dealing with key [" + key + "]\n";
			if (key.Contains("."))
			{
				var split = key.Split('.');
				//Debug.Log("split count: " + split.Length + "\n");
				//output += "Returning [" + split[split.Length - 1] + "]";
				//Debug.Log(output + "\n");
				return split[split.Length - 1];
			}
			return key;
		}
		private static string GetDefaultSavePath()
		{
			return "Assets/Resources/Haptics/";
		}
		private static string GetAssetExtension()
		{
			return ".asset";
		}
	}

	public static class ParsingUtils
	{
		public interface IJsonDeserializable
		{
			void Deserialize(IDictionary<string, object> dict);
		}
		public interface IJsonSerializable
		{
			IDictionary<string, object> SerializeToDictionary();
			string Serialize();
		}

		internal static float parseFloat(object f)
		{

			double a = (double)f;
			return (float)a;
		}

		/// <summary>
		/// Attempt to parse a float from a json object representing a double
		/// </summary>
		/// <param name="potentialFloat">The json object represented as a double</param>
		/// <param name="defaultValue">A default value if the parse fails</param>
		/// <returns></returns>
		internal static float tryParseFloatFromObject(IDictionary<string, object> map, string key, float defaultValue)
		{
			if (!map.ContainsKey(key))
			{
				return defaultValue;
			}

			try
			{

				double intermediate = (double)map[key];
				return (float)intermediate;
			}
			catch (System.InvalidCastException e)
			{
				Debug.LogException(e);
				return defaultValue;
			}
		}

		/// <summary>
		/// Parse a json object into a list of atoms (smallest unit that describes a sequence, pattern, or experience)
		/// </summary>
		/// <typeparam name="T">The json atom type</typeparam>
		/// <param name="dict">The raw json object</param>
		/// <returns>A dictionary representing the list of haptic effect IDs and their associated atoms</returns>
		public static DefDictionary<TAtomType> parseDefinitionsDict<TAtomType>(IDictionary<string, object> dict)
			where TAtomType : IJsonDeserializable, new()
		{
			//setup a dictionary from string -> list of atoms for our result
			DefDictionary<TAtomType> resultDict = new DefDictionary<TAtomType>();

			foreach (var kvp in dict)
			{
				IList<object> atoms = kvp.Value as IList<object>;
				//make sure to instantiate the list for this key
				resultDict.Add(kvp.Key, new List<TAtomType>());

				foreach (var atom in atoms)
				{
					TAtomType a = new TAtomType();
					a.Deserialize(atom as IDictionary<string, object>);
					resultDict[kvp.Key].Add(a);
				}
			}

			return resultDict;
		}

		public static IDictionary<string, object> encodeDefinitionsDict<TAtomType>(DefDictionary<TAtomType> dict)
			where TAtomType : IJsonSerializable, new()
		{
			IDictionary<string, object> returnDict = new Dictionary<string, object>();

#if !UNITY_EDITOR
			Console.WriteLine("Encode dict count: " + dict.Count + "   " + dict.GetType().ToString());
#endif

			foreach (var item in dict)
			{
				IList<object> atoms = new List<object>();
				//Console.WriteLine("Item list count: " + item.Value.Count);
				for (int i = 0; i < item.Value.Count; i++)
				{
#if !UNITY_EDITOR
					Console.WriteLine("\t\t " + item.Key + "  -  " + item.Value[i].ToString());
#endif
					atoms.Add(item.Value[i].Serialize());
				}

				returnDict.Add(item.Key, atoms);
			}

			return returnDict;
		}

		[Serializable]
		public class RootEffect : IJsonDeserializable, IJsonSerializable
		{
			[SerializeField]
			public string name;
			[SerializeField]
			public string type;

			public RootEffect()
			{ }

			public RootEffect(string myName, string myType)
			{
				name = myName;
				type = myType;
			}

			public void Deserialize(IDictionary<string, object> dict)
			{
				this.name = (string)dict["name"];
				this.type = (string)dict["type"];
			}
			public IDictionary<string, object> SerializeToDictionary()
			{
				Dictionary<string, object> dict = new Dictionary<string, object>();
				dict.Add("name", name);
				dict.Add("type", type);
				return dict;
			}
			public string Serialize()
			{
				return MiniJSON.Json.Serialize(SerializeToDictionary());
			}
		}

		public class JsonEffectAtom : IJsonDeserializable, IJsonSerializable
		{
			public string effect;
			public Effect ParseEffect()
			{
				return FileEffectToCodeEffect.TryParse(effect, Effect.Click);
			}

			public float duration;

			public float strength;

			public float time;

			public JsonEffectAtom()
			{ }

			public JsonEffectAtom(Effect effectEnum, float offset, float dur, float str = 1.0f)
			{
				effect = effectEnum.ToString();
				duration = dur;
				strength = str;
				time = offset;
			}

			public void Deserialize(IDictionary<string, object> dict)
			{
				this.effect = dict["effect"] as string;
				this.duration = tryParseFloatFromObject(dict, "duration", 0f);
				this.strength = tryParseFloatFromObject(dict, "strength", 1f);
				this.time = tryParseFloatFromObject(dict, "time", 0f);
			}
			public IDictionary<string, object> SerializeToDictionary()
			{
				Dictionary<string, object> dict = new Dictionary<string, object>();
				dict.Add("effect", effect);
				dict.Add("duration", duration);
				dict.Add("strength", strength);
				dict.Add("time", time);
				return dict;
			}
			public string Serialize()
			{
				return MiniJSON.Json.Serialize(SerializeToDictionary());
			}
		}

		public class JsonSequenceAtom : IJsonDeserializable, IJsonSerializable
		{

			public string sequence;

			public string area;
			public AreaFlag ParseAreaFlag()
			{
				return new AreaParser(area).GetArea();
			}

			public float strength;

			public float time;

			public JsonSequenceAtom(string sequenceName, AreaFlag where, float str, float offset)
			{
				sequence = sequenceName;
				area = where.ToString();
				strength = str;
				time = offset;
			}

			public JsonSequenceAtom()
			{ }

			public void Deserialize(IDictionary<string, object> dict)
			{
				this.sequence = dict["sequence"] as string;
				this.area = dict["area"] as string;
				this.strength = tryParseFloatFromObject(dict, "strength", 1f);
				this.time = tryParseFloatFromObject(dict, "time", 0f);
			}
			public IDictionary<string, object> SerializeToDictionary()
			{
				Dictionary<string, object> dict = new Dictionary<string, object>();
				dict.Add("sequence", sequence);
				dict.Add("area", area);
				dict.Add("strength", strength);
				dict.Add("time", time);
				return dict;
			}
			public string Serialize()
			{
				return MiniJSON.Json.Serialize(SerializeToDictionary());
			}
		}

		public class JsonPatternAtom : IJsonDeserializable, IJsonSerializable
		{
			public string pattern;

			public float strength;

			public float time;
			public JsonPatternAtom() { }

			public void Deserialize(IDictionary<string, object> dict)
			{
				this.pattern = dict["pattern"] as string;
				this.strength = tryParseFloatFromObject(dict, "strength", 1f);
				this.time = tryParseFloatFromObject(dict, "time", 0f);

			}
			public IDictionary<string, object> SerializeToDictionary()
			{
				Dictionary<string, object> dict = new Dictionary<string, object>();
				dict.Add("pattern", pattern);
				dict.Add("strength", strength);
				dict.Add("time", time);
				return dict;
			}
			public string Serialize()
			{
				return MiniJSON.Json.Serialize(SerializeToDictionary());
			}
		}

		public static HapticDefinitionFile ParseHDF(string path)
		{
			try
			{
				var json = File.ReadAllText(path);

				IDictionary<string, object> obj = MiniJSON.Json.Deserialize(json) as IDictionary<string, object>;

				HapticDefinitionFile file = new HapticDefinitionFile();
				file.Deserialize(obj);
				return file;



			}
			catch (IOException e)
			{
				throw new HapticsLoadingException("Couldn't read the haptics asset at " + path, e);
			}
		}
	}
}
