using NullSpace.HapticFiles;
using System;

using FlatBuffers;
using NullSpace.Events;
using System.Linq;
using System.Collections.Generic;
using NullSpace.SDK.FileUtilities;
using UnityEngine;

namespace NullSpace.SDK.FileUtilities
{
	internal static class EncodingUtils
	{

	
	/*
		public class RandomGenerator : IHapticGenerator<HapticSequence, HapticPattern>
		{
			private System.Random _random;
			private IList<AreaFlag> _areas;
			private AreaFlag _currentArea;
			public RandomGenerator()
			{
				var stuff = Enum.GetValues(typeof(AreaFlag));
				AreaFlag[] a = (AreaFlag[])stuff;
				_areas = a.Where(flag => flag.ToString().Contains("_Left") || flag.ToString().Contains("_Right")).ToList();

				_random = new System.Random();
				Next();
			}

			public RandomGenerator Next()
			{

				int which = _random.Next(0, _areas.Count);
				_currentArea = _areas[which];
				return this;
			}
			public HapticPattern Generate(HapticSequence s)
			{
				HapticPattern p = new HapticPattern();
				p.AddChild(0f, _currentArea, s);
				return p;
			}
			public HapticPattern GenerateNext(HapticSequence s)
			{
				return Next().Generate(s);
			}

		}
		*/
		//their code
	}
	
	internal interface IEncoder
	{
		byte[] Encode();
	}
	
	public class FileToCodeHaptic
	{

		
		
		public static HapticSequence CreateSequence(string key,HapticDefinitionFile hdf)
		{
		
			HapticSequence s = new HapticSequence();

			var sequence_def_array = hdf.sequenceDefinitions[key];
			foreach (var effect in sequence_def_array)
			{
				s.AddEffect(effect.time, new HapticEffect(effect.effect, effect.duration, effect.strength));
			}

			return s;
		}

		public static HapticPattern CreatePattern(string key, HapticDefinitionFile hdf)
		{
			HapticPattern p = new HapticPattern();
			var pattern_def_array = hdf.patternDefinitions[key];
			foreach (var seq in pattern_def_array)
			{
				AreaFlag area = new AreaParser(seq.area).GetArea();
				HapticSequence thisSeq = CreateSequence(seq.sequence, hdf);
				p.AddSequence(seq.time, area, thisSeq);
			}
			return p;
		}

		
	}
	
	public class CodeToFileHaptic
	{
		public CodeToFileHaptic()
		{

		}
		//in progress
		public HapticDefinitionFile Encode(string id, HapticSequence sequence)
		{
			HapticDefinitionFile hdf = new HapticDefinitionFile();
			hdf.sequenceDefinitions[id] = new List<ParsingUtils.JsonEffectAtom>();
			foreach (var effect in sequence.Effects)
			{
				ParsingUtils.JsonEffectAtom atom = new ParsingUtils.JsonEffectAtom();
				atom.duration = (float)effect.Duration;
				atom.strength = (float) effect.Strength;
				atom.effect = effect.Effect;
				atom.time = effect.Time;

				hdf.sequenceDefinitions[id].Add(atom);
			}

			hdf.rootEffect.name = id;
			hdf.rootEffect.type = "sequence";

			return hdf;
			
		}

	
	}
	public class CodeHapticEncoder : IEncoder
	{
		EventList _events;
		public CodeHapticEncoder()
		{
			_events = new EventList();
		}

		public CodeHapticEncoder Flatten(HapticPattern p)
		{
			
			CreateEventList(p);
			return this;
		}
		
		public CodeHapticEncoder Flatten(HapticSequence s)
		{
			CreateEventList(s, 0.0, 1.0);
			return this;
		}

		private void CreateEventList(HapticSequence s, double parentTime, double parentStrength) 
		{
			
			var strength = parentStrength * s.Strength;
			var time = parentTime + s.Time;
			foreach (var eff in s.Effects)
			{
				float finalStrength = (float)strength * (float)eff.Strength;
				float finalTime = (float) time+ eff.Time;
				_events.AddEvent(new BasicHapticEvent(
					finalTime,
					finalStrength,
					(float)eff.Duration,
					(uint)s.Area,
					eff.Effect
				));

			}

		}

		private void CreateEventList(HapticPattern p)
		{

			double baseStrength = p.Strength;
			double baseTime = p.Time;

			foreach (var seq in p.Sequences)
			{
				double newStrength = baseStrength * seq.Strength;
				double newTime = baseTime + seq.Time;


				CreateEventList(seq, baseTime, baseStrength);
				
			}

		}
		

		public byte[] Encode()
		{
			return  _events.Generate();
		}
	}
}
