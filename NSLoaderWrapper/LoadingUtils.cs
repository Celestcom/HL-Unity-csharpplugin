using System.Collections.Generic;

using System.IO;


namespace NullSpace.SDK.FileUtilities
{


	public static class LoadingUtils
	{
		internal static float parseFloat(object f)
		{
			double a = (double)f;
			return (float)a;
		}

		public static IDictionary<string, IList<T>> parseDict<T>(IDictionary<string, object> dict) where T: IJsonDeserializable, new()
		{
			IDictionary<string, IList<T>> resultDict = new Dictionary<string, IList<T>>();
			foreach (var kvp in dict)
			{
				//items.Add(kvp.Key, )
				IList<object> items = kvp.Value as IList<object>;
				resultDict.Add(kvp.Key, new List<T>());
				foreach (var which in items)
				{
					T a = new T();
					a.Deserialize(which as IDictionary<string, object>);
					resultDict[kvp.Key].Add(a);
				}
			}

			return resultDict;
		}
		public interface IJsonDeserializable
		{
			void Deserialize(IDictionary<string, object> dict);
		}
		public class RootEffect : IJsonDeserializable
		{
			public string name { get; set; }
			public string type { get; set; }
		

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
				this.duration = parseFloat(dict["duration"]);
				this.strength = parseFloat(dict["strength"]);
				this.time = parseFloat(dict["time"]);
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
				this.strength = parseFloat(dict["strength"]);
				this.time = parseFloat(dict["time"]);
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
				this.strength = parseFloat(dict["strength"]);
				this.time = parseFloat(dict["time"]);
			
			}
		}
	
		public class HapticDefinitionFile : IJsonDeserializable
		{
			public RootEffect root_effect;
			
			public IDictionary<string, IList<JsonPatternAtom>> experience_definitions;
			public IDictionary<string, IList<JsonSequenceAtom>> pattern_definitions;
			public IDictionary<string, IList<JsonEffectAtom>> sequence_definitions;

			public HapticDefinitionFile()
			{
				this.root_effect = new RootEffect();
				
			}
			public void Deserialize(IDictionary<string, object> dict)
			{
				root_effect.Deserialize(dict["root_effect"] as IDictionary<string, object>);

				pattern_definitions = parseDict<JsonSequenceAtom>(
					dict["pattern_definitions"] as IDictionary<string, object>
				);
				sequence_definitions = parseDict<JsonEffectAtom>(
					dict["sequence_definitions"] as IDictionary<string, object>
				);
				experience_definitions = parseDict<JsonPatternAtom>(
					dict["experience_definitions"] as IDictionary<string, object>
				);
			}
		}


	public static HapticDefinitionFile LoadAsset(string path)
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
