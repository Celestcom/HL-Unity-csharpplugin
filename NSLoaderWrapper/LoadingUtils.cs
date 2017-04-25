using System;
using System.Collections.Generic;

using System.IO;
using UnityEngine;
using UnityEditor;
namespace NullSpace.SDK.FileUtilities
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

			if (hdf.rootEffect.type == "sequence")
			{
				var seq = CodeHapticFactory.CreateSequence(hdf.rootEffect.name, hdf);
				return seq;
			}
			else
			{
				throw new InvalidOperationException("Unable to load " + hdf.rootEffect.name + " as a HapticSequence because it is a " + hdf.rootEffect.type);
			}
		}

		public static HapticPattern LoadPattern(string path)
		{
			HapticDefinitionFile hdf = LoadHDF(path);
			if (hdf.rootEffect.type == "pattern")
			{
				var pat = CodeHapticFactory.CreatePattern(hdf.rootEffect.name, hdf);
				return pat;
			} else
			{
				throw new InvalidOperationException("Unable to load " + hdf.rootEffect.name + " as a HapticPattern because it is a " + hdf.rootEffect.type);
			}
		}

	
	}

	public static class ParsingUtils
	{
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
			} catch (System.InvalidCastException e)
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
			where TAtomType: IJsonDeserializable, new() 
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

		public interface IJsonDeserializable
		{
			void Deserialize(IDictionary<string, object> dict);
		}

		[Serializable]
		public class RootEffect : IJsonDeserializable
		{
			[SerializeField]
			public string name;
			[SerializeField]
			public string type;
		

			public void Deserialize(IDictionary<string, object> dict)
			{
				this.name = (string)dict["name"];
				this.type = (string)dict["type"];
			}
		}
	
		public class JsonEffectAtom : IJsonDeserializable
		{
			public string effect;

			public float duration;

			public float strength;

			public float time;

			public void Deserialize(IDictionary<string, object> dict)
			{
				this.effect = dict["effect"] as string;
				this.duration = tryParseFloatFromObject(dict, "duration", 0f);
				this.strength = tryParseFloatFromObject(dict, "strength", 1f);
				this.time = tryParseFloatFromObject(dict, "time", 0f);
			}
		}


	
		

		public class JsonSequenceAtom : IJsonDeserializable
		{

			public string sequence;

			public string area;

			public float strength;

			public float time;


			public void Deserialize(IDictionary<string, object> dict)
			{
				this.sequence = dict["sequence"] as string;
				this.area = dict["area"] as string;
				this.strength = tryParseFloatFromObject(dict, "strength",  1f);
				this.time = tryParseFloatFromObject(dict, "time",  0f);
			}
		}

		public class JsonPatternAtom : IJsonDeserializable
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
