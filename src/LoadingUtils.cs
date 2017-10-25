using System;
using System.Collections.Generic;

using System.IO;
using UnityEngine;
using UnityEditor;
namespace Hardlight.SDK.FileUtilities
{
	public static class HapticResources
	{
		private static HapticDefinitionFile LoadHDF(string path)
		{
			var file = Resources.Load<JsonAsset>(path);

			if (file == null)
			{
				Debug.LogWarning("Unable to load haptic resource at path " + path);
				return null;
			}

			HapticDefinitionFile hdf = new HapticDefinitionFile();
			hdf.Deserialize(file.GetJson());
			return hdf;
		}
		public static HapticSequence LoadSequence(string path)
		{

			HapticDefinitionFile hdf = LoadHDF(path);

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
		public static HapticPattern LoadPattern(string path)
		{
			HapticDefinitionFile hdf = LoadHDF(path);
			if (hdf.root_effect.type == "pattern")
			{
				var pat = CodeHapticFactory.CreatePattern(hdf.root_effect.name, hdf);
				return pat;
			}
			else
			{
				throw new InvalidOperationException("Unable to load " + hdf.root_effect.name + " as a HapticPattern because it is a " + hdf.root_effect.type);
			}
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
