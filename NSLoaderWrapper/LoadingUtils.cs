using System;
using System.Collections.Generic;

using System.IO;
using UnityEngine;
using UnityEditor;
namespace NullSpace.SDK.FileUtilities
{
	public static class HapticResources
	{
		public static object Load<T>(string path)
		{
			var file = Resources.Load<HapticDefinitionFile>(path);

			if (file == null)
			{
				Debug.LogWarning("Unable to load haptic resource at path " + path);
				return null;
			}

			Type fileType = typeof(T);
			
			if (fileType == typeof(CodeSequence))
			{
				if (file.rootEffect.type != "sequence")
				{
					Debug.LogException(new InvalidOperationException("Could not load a CodeSequence from the file at path " + path + ": the file is a " + file.rootEffect.type));
					return null;
				}

				return FileToCodeHaptic.CreateSequence(file.rootEffect.name, file);
			}

			else if (fileType == typeof(CodePattern))
			{
				if (file.rootEffect.type != "pattern")
				{
					Debug.LogException(new InvalidOperationException("Could not load a CodePattern from the file at path " + path + ": the file is a " + file.rootEffect.type));
				}

				return FileToCodeHaptic.CreatePattern(file.rootEffect.type, file);
			}

			else
			{
				throw new NotImplementedException("Loading " + fileType + " is not implemented yet.");
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
		internal static T tryParse<T>(object potentialFloat, T defaultValue)
		{
			try
			{
				//double intermediate = (double)potentialFloat;
				return (T)potentialFloat;
			} catch (System.InvalidCastException e)
			{
				return defaultValue;
			}
		}

		//This is actually terrible now that we have to deal with Unity serialization. 
		//Should probably just make overloads at this point?
		/// <summary>
		/// Parse a json object into a list of atoms (smallest unit that describes a sequence, pattern, or experience)
		/// </summary>
		/// <typeparam name="T">The json atom type</typeparam>
		/// <param name="dict">The raw json object</param>
		/// <returns>A dictionary representing the list of haptic effect IDs and their associated atoms</returns>
		public static TResult parseDefinitionsDict<TResult, TAtomType, TListType>(IDictionary<string, object> dict) 
			where TAtomType: IJsonDeserializable, new() 
			where TResult: SerializableDictionary<string, TListType>, new()
			where TListType: List<TAtomType>, new()
		{
			//setup a dictionary from string -> list of atoms for our result
			TResult resultDict = new TResult();

			foreach (var kvp in dict)
			{
				IList<object> atoms = kvp.Value as IList<object>;
				//make sure to instantiate the list for this key
				resultDict.Add(kvp.Key, new TListType());

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
	
		[Serializable]
		public class JsonEffectAtom : IJsonDeserializable
		{
			[SerializeField]
			public string effect;
			[SerializeField]

			public float duration;
			[SerializeField]

			public float strength;
			[SerializeField]

			public float time;

			public void Deserialize(IDictionary<string, object> dict)
			{
				this.effect = dict["effect"] as string;
				this.duration = tryParse((double)dict["duration"], 0f);
				this.strength = tryParse((double)dict["strength"], 1f);
				this.time = tryParse((double)dict["time"], 0f);
			}
		}


	
		public static T LoadHapticAsset<T>(string resourceID) where T: CodeSequence
		{
			Debug.Log("Trying to LoadHapticAsset with path " + resourceID);
			var hdf = AssetDatabase.LoadAssetAtPath(resourceID, typeof(HapticDefinitionFile)) as HapticDefinitionFile;
			if (hdf == null)
			{
				throw new ArgumentException("Couldn't find an asset with name " + resourceID);
			}
			Type typeParam = typeof(T);

			if (typeParam == typeof(CodeSequence))
			{
				if (hdf.rootEffect.type != "sequence")
				{
					throw new InvalidOperationException("A " + typeParam.ToString() + " was requested, but the haptic asset is a " + hdf.rootEffect.type);
				}

				object a = FileToCodeHaptic.CreateSequence(hdf.rootEffect.name, hdf);
				return (T)a;
			} else
			{
				throw new InvalidOperationException("Could not deal with type " + typeParam.ToString());
			}
			

		}

		[Serializable]
		public class JsonSequenceAtom : IJsonDeserializable
		{
			[SerializeField]

			public string sequence;
			[SerializeField]

			public string area;
			[SerializeField]

			public float strength;
			[SerializeField]

			public float time;


			public void Deserialize(IDictionary<string, object> dict)
			{
				this.sequence = dict["sequence"] as string;
				this.area = dict["area"] as string;
				this.strength = tryParse((double)dict["strength"], 1f);
				this.time = tryParse((double)dict["time"], 0f);
			}
		}

		[Serializable]
		public class JsonPatternAtom : IJsonDeserializable
		{
			[SerializeField]

			public string pattern;
			[SerializeField]

			public float strength;
			[SerializeField]

			public float time;
			public JsonPatternAtom() { }

			public void Deserialize(IDictionary<string, object> dict)
			{
				this.pattern = dict["pattern"] as string;
				this.strength = tryParse((double)dict["strength"], 1f);
				this.time = tryParse((double)dict["time"], 0f);
			
			}
		}


		[Serializable]
		public class JsonEffectList : List<JsonEffectAtom> { }
		[Serializable]
		public class StringToEffectAtom : SerializableDictionary<string, JsonEffectList> { }

		[Serializable]
		public class JsonSequenceList : List<JsonSequenceAtom> { }
		[Serializable]
		public class StringToSequenceAtom: SerializableDictionary<string, JsonSequenceList> { }


		[Serializable]
		public class JsonPatternList : List<JsonPatternAtom> { }
		[Serializable]
		public class StringToPatternAtom : SerializableDictionary<string, JsonPatternList> { }
		

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
