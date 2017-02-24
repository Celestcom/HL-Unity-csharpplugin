using NullSpace.HapticFiles;
using System;

using FlatBuffers;
using NullSpace.Events;
using System.Linq;
using System.Collections.Generic;

namespace NullSpace.SDK
{
	internal static class EncodingUtils
	{

		
		internal static byte[] Encode(IGeneratable effects, UInt32 handle)
		{
			var builder = new FlatBuffers.FlatBufferBuilder(128);
			var packet = effects.Generate(builder);
			


			var name = builder.CreateString("Code generated Haptic Node");
			HapticPacket.StartHapticPacket(builder);
			HapticPacket.AddHandle(builder, handle);
			HapticPacket.AddPacketType(builder, FileType.Node);
			HapticPacket.AddName(builder, name);
			HapticPacket.AddPacket(builder, packet.Value);
			var rootTable = HapticPacket.EndHapticPacket(builder);
			builder.Finish(rootTable.Value);
			return builder.SizedByteArray();

		}
		internal delegate Offset<Node> BuffEncoder(FlatBufferBuilder builder);

		internal static byte[] EncodeDel(BuffEncoder encoder, UInt32 handle)
		{
			var builder = new FlatBuffers.FlatBufferBuilder(128);
			var packet = encoder(builder);



			var name = builder.CreateString("Code generated Haptic Node");
			HapticPacket.StartHapticPacket(builder);
			HapticPacket.AddHandle(builder, handle);
			HapticPacket.AddPacketType(builder, FileType.Node);
			HapticPacket.AddName(builder, name);
			HapticPacket.AddPacket(builder, packet.Value);
			var rootTable = HapticPacket.EndHapticPacket(builder);
			builder.Finish(rootTable.Value);
			return builder.SizedByteArray();
		}



		public interface IHapticGenerator<Input, Output>
		{
			Output Generate(Input input);
		}

		static void Traverse(Func<float, float> prevTransform, CodeSequence parent, EventList builder)
		{
			float[] propogatedStrengths = parent.Effects.Select(effect => prevTransform((float)(effect.Strength * parent.Strength))).ToArray();

			float[] propogatedTimes = parent.Effects.Select(effect => (float)(effect.Time + parent.Time)).ToArray();

			for (int i = 0; i < parent.Effects.Count; i++)
			{
				builder.AddEvent(new BasicHapticEvent(
					propogatedTimes[i],
					propogatedStrengths[i],
					(float) parent.Effects[i].Duration,
					(uint) parent.Area,
					parent.Effects[i].Effect
				));
			}
		}

		static void Traverse(Func<float, float> prevTransform, CodePattern parent, EventList builder)
		{
			float[] propogatedStrengths = PropogateFrom(parent.Sequences, 
				(seq) => { return (float)seq.Time + (float) parent.Time;
			});

			float[] propogatedTimes = PropogateFrom(parent.Sequences,
				(seq) => { return (float)seq.Strength * (float) parent.Strength;
			});


			for (int i = 0; i < parent.Sequences.Count; i++)
			{

			}
			

		}

		
		
		internal static float[] PropogateFrom<T>(IList<T> items, Func<T, float> transform)
		{
			return items.Select(item => transform(item)).ToArray();
		}
		/*
		public class RandomGenerator : IHapticGenerator<CodeSequence, CodePattern>
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
			public CodePattern Generate(CodeSequence s)
			{
				CodePattern p = new CodePattern();
				p.AddChild(0f, _currentArea, s);
				return p;
			}
			public CodePattern GenerateNext(CodeSequence s)
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

	internal class CodeHapticEncoder : IEncoder
	{
		EventList _events;
		public CodeHapticEncoder()
		{
			_events = new EventList();
		}

		public CodeHapticEncoder Flatten(CodePattern p)
		{
			CreateEventList(p);
			return this;
		}
		
		public CodeHapticEncoder Flatten(CodeSequence s)
		{
			CreateEventList(s, 0.0, 1.0);
			return this;
		}

		private void CreateEventList(CodeSequence s, double parentTime, double parentStrength) 
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

		private void CreateEventList(CodePattern p)
		{
			EventList events = new EventList();

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
