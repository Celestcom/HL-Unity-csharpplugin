using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static NullSpace.SDK.FileUtilities.ParsingUtils;

namespace NullSpace.SDK.FileUtilities
{
	
	[Serializable]
	public class HapticDefinitionFile : UnityEngine.ScriptableObject, ParsingUtils.IJsonDeserializable
	{
		[SerializeField]
		public RootEffect rootEffect;

		[SerializeField]

		public StringToPatternAtom experienceDefinitions;
		//	public IDictionary<string, IList<JsonPatternAtom>> experienceDefinitions;
		//public IDictionary<string, IList<JsonSequenceAtom>> patternDefinitions;

		[SerializeField]

		public StringToSequenceAtom patternDefinitions;

		//public IDictionary<string, IList<JsonEffectAtom>> sequenceDefinitions;

		[SerializeField]

		public StringToEffectAtom sequenceDefinitions;

		public HapticDefinitionFile()
		{
			//this.rootEffect = new RootEffect();
		//	this.sequenceDefinitions = new StringToEffectAtom();
			//this.patternDefinitions = new StringToSequenceAtom();
		//	this.experienceDefinitions = new StringToPatternAtom();


		}

		public void OnEnable()
		{
			if (this.rootEffect == null)
			{
				this.rootEffect = CreateInstance();
			}
		}

		public void Load(HapticDefinitionFile other)
		{
			this.rootEffect = other.rootEffect;
			this.patternDefinitions = other.patternDefinitions;
			this.sequenceDefinitions = other.sequenceDefinitions;
			this.experienceDefinitions = other.experienceDefinitions;
		}
		public void Deserialize(IDictionary<string, object> dict)
		{
			rootEffect.Deserialize(dict["root_effect"] as IDictionary<string, object>);

			//Generics = worthless at this point. Thanks unity!
			//Should probably redesign a better native format for unity, parsed from the hdf
			patternDefinitions = parseDefinitionsDict<StringToSequenceAtom, JsonSequenceAtom, JsonSequenceList>(
				dict["pattern_definitions"] as IDictionary<string, object>
			);


			sequenceDefinitions = parseDefinitionsDict<StringToEffectAtom, JsonEffectAtom, JsonEffectList>(
				dict["sequence_definitions"] as IDictionary<string, object>
			);
			experienceDefinitions = parseDefinitionsDict<StringToPatternAtom, JsonPatternAtom, JsonPatternList>(
				dict["experience_definitions"] as IDictionary<string, object>
			);
		}
	}

}
