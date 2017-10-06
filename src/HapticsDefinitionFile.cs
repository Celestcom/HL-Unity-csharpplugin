using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static NullSpace.SDK.FileUtilities.ParsingUtils;

namespace NullSpace.SDK.FileUtilities
{
	public class HapticsAssetException : Exception
	{
		public HapticsAssetException(string message) : base(message)
		{

		}

		public HapticsAssetException(string message, Exception inner) : base(message, inner) { }
	}

	public class DefDictionary<TAtomType> : Dictionary<string, IList<TAtomType>> { }

	public class HapticDefinitionFile : IJsonDeserializable
	{
		[SerializeField]
		public RootEffect root_effect;

		[SerializeField]
		public DefDictionary<JsonPatternAtom> experience_definitions;
		[SerializeField]
		public DefDictionary<JsonSequenceAtom> pattern_definitions;
		[SerializeField]
		public DefDictionary<JsonEffectAtom> sequence_definitions;

		public HapticDefinitionFile()
		{
			this.root_effect = new RootEffect();
			this.sequence_definitions = new DefDictionary<JsonEffectAtom>();
			this.pattern_definitions = new DefDictionary<JsonSequenceAtom>();
			this.experience_definitions = new DefDictionary<JsonPatternAtom>();
		}
		public HapticDefinitionFile(RootEffect root)
		{
			this.root_effect = root;
			this.sequence_definitions = new DefDictionary<JsonEffectAtom>();
			this.pattern_definitions = new DefDictionary<JsonSequenceAtom>();
			this.experience_definitions = new DefDictionary<JsonPatternAtom>();
		}

		public void Load(HapticDefinitionFile other)
		{
			this.root_effect = other.root_effect;
			this.pattern_definitions = other.pattern_definitions;
			this.sequence_definitions = other.sequence_definitions;
			this.experience_definitions = other.experience_definitions;
		}
		public void Deserialize(string json)
		{
			var dict = MiniJSON.Json.Deserialize(json) as IDictionary<string, object>;
			if (dict == null)
			{
				throw new HapticsAssetException("Couldn't parse the haptic asset; it doesn't look like json");
			}
			else
			{
				this.Deserialize(dict);
			}

		}
		public void Deserialize(IDictionary<string, object> dict)
		{
			try
			{
				root_effect.Deserialize(dict["root_effect"] as IDictionary<string, object>);

				//Generics = worthless at this point. Thanks unity!
				//Should probably redesign a better native format for unity, parsed from the hdf

				try
				{
					pattern_definitions = parseDefinitionsDict<JsonSequenceAtom>(
						dict["pattern_definitions"] as IDictionary<string, object>
				);

				}
				catch (KeyNotFoundException e)
				{
					Debug.LogError("In pats: " + e.Message + "\n");
				}

				try
				{
					sequence_definitions = parseDefinitionsDict<JsonEffectAtom>(
						dict["sequence_definitions"] as IDictionary<string, object>
					);
				}
				catch (KeyNotFoundException e)
				{
					Debug.LogError("In seqs: " + e.Message + "\n");
				}
				try
				{
					experience_definitions = parseDefinitionsDict<JsonPatternAtom>(
						dict["experience_definitions"] as IDictionary<string, object>
					);
				}
				catch (KeyNotFoundException e)
				{
					Debug.LogError("In exps: " + e.Message + "\n");
				}
			}
			catch (Exception e)
			{
				var exep = new HapticsAssetException("Couldn't parse the haptic asset", e);

				Debug.LogException(exep);
			}
		}
		public string Serialize()
		{
			IDictionary<string, object> dict = new Dictionary<string, object>();
			try
			{
				#region Root Effect
				if (root_effect != null)
				{
					var result = root_effect.Serialize();
					if (result != null && result.Count > 0)
					{
						dict.Add("root_effect", result);
					}
				}
				#endregion

				#region Sequences
				try
				{
					IDictionary<string, object> output = encodeDefinitionsDict<JsonEffectAtom>(sequence_definitions);

					if (output.Count > 0)
					{
						dict.Add("sequence_definitions", output);
					}
				}
				catch (KeyNotFoundException e)
				{
					Debug.LogError("In seqs: " + e.Message + "\n");
				}
				#endregion

				#region Patterns
				try
				{
					//Console.WriteLine("Attempting to encode pattern definitions");
					IDictionary<string, object> output = encodeDefinitionsDict<JsonSequenceAtom>(pattern_definitions);

					if (output.Count > 0)
					{
						dict.Add("pattern_definitions", output);
					}
					//Console.WriteLine("pattern definitions: " + dict.Count);
					foreach (var kvp in output)
					{
						Console.WriteLine(kvp.Key.ToString() + "   " + kvp.Value.ToString() + "\n");
					}

					//pattern_definitions = encodeDefinitionsDict<JsonSequenceAtom>(
					//	dict["pattern_definitions"] as IDictionary<string, object>);
				}
				catch (KeyNotFoundException e)
				{
					Debug.LogError("In pats: " + e.Message + "\n");
				}
				#endregion

				#region Experiences
				try
				{
					IDictionary<string, object> output = encodeDefinitionsDict<JsonPatternAtom>(experience_definitions);

					if (output.Count > 0)
					{
						dict.Add("experience_definitions", output);
					}
					//experience_definitions = parseDefinitionsDict<JsonPatternAtom>(
					//	dict["experience_definitions"] as IDictionary<string, object>);
				}
				catch (KeyNotFoundException e)
				{
					Debug.LogError("In exps: " + e.Message + "\n");
				}
				#endregion
			}

			#region Catch Exception
			catch (Exception e)
			{
				var exep = new HapticsAssetException("Couldn't parse the haptic asset", e);

#if UNITY_EDITOR
				Debug.LogException(exep);
#else
				Console.WriteLine("Failed to encode\n\t" + e.Message);
#endif
			} 
			#endregion

			string json = MiniJSON.Json.Serialize(dict);


			return json;
		}
	}

	public class JsonHDF
	{
		public string root_effect;
		public DefDictionary<JsonSequenceAtom> sequence_definitions;
	}
}
