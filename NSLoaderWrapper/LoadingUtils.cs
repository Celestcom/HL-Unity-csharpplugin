using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LitJson;
namespace NullSpace.SDK
{
	internal static class LoadingUtils
	{
		public class RootEffect
		{
			public string name { get; set; }
			public string type { get; set; }
			
		}

		public class JsonEffectAtom
		{
			public string effect;
			public float duration;
			public float strength;
			public float time;
			public JsonEffectAtom()
			{

			}
		}

		public class JsonSequenceAtom
		{
			public string sequence;
			public string area;
			public float strength;
			public float time;
			public JsonSequenceAtom(){}
		}

		public class JsonPatternAtom
		{
			public string pattern;
			public float strength;
			public float time;
			public JsonPatternAtom() { }
		}
		public class PatternDefinitions
		{
			public IDictionary<string, IList<JsonSequenceAtom>> items;
		}
		public class SequenceDefinitions
		{
			public IDictionary<string, IList<JsonSequenceAtom>> items;
		}
		public class ExperienceDefinitions
		{
			public IDictionary<string, IList<JsonSequenceAtom>> items;
		}
		public class HapticDefinitionFile
		{
			public RootEffect root_effect;
			SequenceDefinitions sequence_definitions;
			PatternDefinitions pattern_definitions;
			ExperienceDefinitions experience_definitions;

		}
		internal static void LoadAsset(string path)
		{
			try
			{

				var text = File.ReadAllText(path);
				var jsonData = JsonMapper.ToObject(text);

				var json = jsonData.IsObject;
				var root = jsonData["root_effect"];
				var x = JsonMapper.ToObject(new JsonReader(root.ToJson()));
			//	RootEffect e = JsonMapper.ToObject<RootEffect>(what);

				 
				int y = 3;
				
				
			} catch (IOException e)
			{
				throw new HapticsLoadingException("Couldn't read the haptics asset at " + path, e);
			}
		}


	}
}
