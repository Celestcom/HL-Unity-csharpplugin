using System;
using System.Collections.Generic;

using System.IO;
using UnityEngine;
using UnityEditor;
namespace Hardlight.SDK.FileUtilities
{
	public static class HapticResources
	{
		private static string TryToFindAvailableFileName(string desiredPathAndName)
		{
			string enginePath = Application.dataPath;
			enginePath = Application.dataPath.Remove(enginePath.Length - 6, 6);
			bool exists = File.Exists(enginePath + desiredPathAndName);

			Debug.Log("Desired Path: " + desiredPathAndName + "\n" + enginePath + "\n" + enginePath + desiredPathAndName);

			bool succeeded = true;
			int extraChecks = 2;
			if (exists)
			{
				succeeded = false;
				for (int i = 0; i < extraChecks; i++)
				{
					exists = File.Exists(enginePath + desiredPathAndName + " " + i);
					Debug.Log("Trying to find file: " + desiredPathAndName + " " + i + "\n\t[" + exists + "]");
					if (!exists)
					{
						i = int.MaxValue;
						succeeded = true;
						desiredPathAndName = desiredPathAndName + i;
					}
				}
			}

			if (!succeeded)
			{
				throw new HapticsAssetException("Could not create asset for sequence [" + desiredPathAndName + "]\nToo many similarly named files (checked " + extraChecks + " files)...\n\tSuggestion: clean up some excessive names and try again.");
			}

			return desiredPathAndName;
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

		public static bool IsSequence(string key)
		{
			return key.Contains(".sequence");
		}
		public static bool IsPattern(string key)
		{
			return key.Contains(".pattern");
		}
		public static bool IsExperience(string key)
		{
			return key.Contains(".experience");
		}
		public static HapticSequence SequenceExists(string key)
		{
			string assetName = GetHapticFileName(key);

			return HapticSequence.LoadFromAsset(key);
		}

		public static string GetHapticFileName(string key)
		{
			return key.Replace('.', '_') + ".asset";
		}

		private static HapticDefinitionFile LoadHDFFromJson(string jsonPath)
		{
			var hdf = HapticAssetTool.GetHapticDefinitionFile(jsonPath);

			if (hdf == null)
			{
				Debug.LogWarning("Unable to load haptic resource at path " + jsonPath);
			}
			return hdf;
			//var file = Resources.Load<JsonAsset>(path);

			//if (file == null)
			//{
			//	Debug.LogWarning("Unable to load haptic resource at path " + path);
			//	return null;
			//}

			//HapticDefinitionFile hdf = new HapticDefinitionFile();
			//hdf.Deserialize(file.GetJson());
			//return hdf;
		}
		public static HapticSequence LoadSequenceFromJson(string jsonPath)
		{
			HapticDefinitionFile hdf = LoadHDFFromJson(jsonPath);

			if (hdf.root_effect.type == "sequence")
			{
				var seq = CodeHapticFactory.CreateSequence(hdf.root_effect.name, hdf);
				return seq;
			}
			else
			{
				throw new InvalidOperationException("Unable to load " + hdf.root_effect.name + " as a HapticSequence because it is a " + hdf.root_effect.type);
			}
		}
		public static HapticPattern LoadPatternFromJson(string jsonPath)
		{
			HapticDefinitionFile hdf = LoadHDFFromJson(jsonPath);
			if (hdf.root_effect.type == "pattern")
			{
				Debug.Log("Valid HDF. creating pattern elements\n");
				var pat = CodeHapticFactory.CreatePattern(hdf.root_effect.name, hdf);
				return pat;
			}
			else
			{
				throw new InvalidOperationException("Unable to load " + hdf.root_effect.name + " as a HapticPattern because it is a " + hdf.root_effect.type);
			}
		}

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
				seq.name = fileName;
			}
			else if (isPat)
			{
				Debug.LogError("Attempted to run a HapticResources.CreatePattern while providing a pattern at path: " + jsonPath + "\n\t");
			}
			else if (isPat)
			{
				Debug.LogError("Attempted to run a HapticResources.CreateSequence while providing a experience at path: " + jsonPath + "\n\t");
			}

			SaveSequence(fileName, seq);
			return seq;
		}
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
				pat.name = fileName;
			}
			else if (isSeq)
			{
				Debug.LogError("Attempted to run a HapticPattern.CreateAsset while providing a sequence at path: " + jsonPath + "\n\t");
			}
			else if (isPat)
			{
				Debug.LogError("Attempted to run a HapticPattern.CreateAsset while providing a experience at path: " + jsonPath + "\n\t");
			}

			//SavePattern(fileName, pat);
			return pat;
		}

		public static void SavePattern(string name, HapticPattern pattern)
		{
			string assetPath = "Assets/Resources/Haptics/";
			name = GetHapticFileName(name);

			string finalizedPath = TryToFindAvailableFileName(assetPath + name);
			Debug.Log(finalizedPath + "\n\t" + name + "\n");
			if (pattern != null)
			{
				HashSet<HapticSequence> keys = new HashSet<HapticSequence>();
				for (int i = 0; i < pattern.Sequences.Count; i++)
				{
					if (!keys.Contains(pattern.Sequences[i].Sequence))
					{
						SaveSequence(pattern.Sequences[i].Sequence.name, pattern.Sequences[i].Sequence);
						keys.Add(pattern.Sequences[i].Sequence);
					}
				}

				AssetDatabase.CreateAsset(pattern, finalizedPath);
				AssetDatabase.SaveAssets();
				Selection.activeObject = pattern;
			}
			else
				Debug.LogError("Attempted to save an invalid asset [" + name + "]\n");
		}
		public static void SaveSequence(string name, HapticSequence sequence)
		{
			string assetPath = "Assets/Resources/Haptics/";
			name = GetHapticFileName(name);

			string finalizedPath = TryToFindAvailableFileName(assetPath + name);

			if (sequence != null)
			{
				AssetDatabase.CreateAsset(sequence, finalizedPath);
				AssetDatabase.SaveAssets();
				Selection.activeObject = sequence;
			}
			else
				Debug.LogError("Attempted to save an invalid asset [" + name + "]\n");
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
