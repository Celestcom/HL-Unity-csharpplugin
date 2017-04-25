using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static NullSpace.SDK.FileUtilities.ParsingUtils;

namespace NullSpace.SDK.FileUtilities
{
	public class HapticsAssetException : Exception {
		public HapticsAssetException(string message) : base(message)
		{

		}

		public HapticsAssetException(string message, Exception inner):base(message, inner) { }
	}

 	public class DefDictionary<TAtomType> : Dictionary<string, IList<TAtomType>> { }

	public class HapticDefinitionFile :  ParsingUtils.IJsonDeserializable
	{
		public RootEffect rootEffect;


		public DefDictionary<JsonPatternAtom> experienceDefinitions;
		public DefDictionary<JsonSequenceAtom> patternDefinitions;
		public DefDictionary<JsonEffectAtom> sequenceDefinitions;



		public HapticDefinitionFile()
		{
			this.rootEffect = new RootEffect();
			this.sequenceDefinitions = new DefDictionary<JsonEffectAtom>();
			this.patternDefinitions = new DefDictionary<JsonSequenceAtom>();
			this.experienceDefinitions = new DefDictionary<JsonPatternAtom>();


		}

		

		public void Load(HapticDefinitionFile other)
		{
			this.rootEffect = other.rootEffect;
			this.patternDefinitions = other.patternDefinitions;
			this.sequenceDefinitions = other.sequenceDefinitions;
			this.experienceDefinitions = other.experienceDefinitions;
		}
		public void Deserialize(string json)
		{
			var dict = MiniJSON.Json.Deserialize(json) as IDictionary<string, object>;
			if (dict == null)
			{
				throw new HapticsAssetException("Couldn't parse the haptic asset; it doesn't look like json");
			} else {
				this.Deserialize(dict);
			}
		}
		public void Deserialize(IDictionary<string, object> dict)
		{
			try
			{
				rootEffect.Deserialize(dict["root_effect"] as IDictionary<string, object>);

				//Generics = worthless at this point. Thanks unity!
				//Should probably redesign a better native format for unity, parsed from the hdf

				try
				{
					patternDefinitions = parseDefinitionsDict<JsonSequenceAtom>(
					dict["pattern_definitions"] as IDictionary<string, object>
				);

				} catch (KeyNotFoundException e)
				{
					Debug.LogError("In pats: " + e.Message);
				}

				try { 
				sequenceDefinitions = parseDefinitionsDict<JsonEffectAtom>(
					dict["sequence_definitions"] as IDictionary<string, object>
				);
				}
				catch (KeyNotFoundException e)
				{
					Debug.LogError("In pats: " + e.Message);
				}
				try { 
				experienceDefinitions = parseDefinitionsDict<JsonPatternAtom>(
					dict["experience_definitions"] as IDictionary<string, object>
				);
				}
				catch (KeyNotFoundException e)
				{
					Debug.LogError("In pats: " + e.Message);
				}
			} catch (Exception e)
			{
				var exep = new HapticsAssetException("Couldn't parse the haptic asset", e);

				Debug.LogException(exep);
			}
		}
	}

}
