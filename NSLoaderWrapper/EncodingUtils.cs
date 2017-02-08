using NullSpace.HapticFiles;
using System;

using FlatBuffers;
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

}
