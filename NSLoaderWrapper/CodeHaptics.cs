using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatBuffers;
using NullSpace.HapticFiles;
using static NullSpace.SDK.Internal.Interop;
using NullSpace.SDK.Internal;

namespace NullSpace.SDK
{
	public class CodeSequence : Playable
	{
		private string _name;
		private Offset<HapticEffect>[] _offset;
		private List<CodeSequenceItem> _items;
		public CodeSequence(string name)
		{
			_name = name;
			_items = new List<CodeSequenceItem>();
			
		}
		public void Add(CodeSequenceItem s)
		{
			_items.Add(s);
		}
		private CommandWithHandle _create(uint location)
		{

			var builder = new FlatBufferBuilder(1);
			_offset = new Offset<HapticEffect>[_items.Count];
			for (int i = 0; i < _items.Count; i++)
			{
				var item = _items[i];
				_offset[i] = HapticEffect.CreateHapticEffect(builder, item.Time, builder.CreateString(item.Effect), item.Strength, item.Duration);

			}
			var itemsVec = NullSpace.HapticFiles.Sequence.CreateItemsVector(builder, _offset);
			var sequence = NullSpace.HapticFiles.Sequence.CreateSequence(builder, location, itemsVec);
			builder.Finish(sequence.Value);

			var dataRef = builder.SizedByteArray();
		
			return new CommandWithHandle(handle => Interop.NSVR_CreateCodeSequence(Wrapper.NSVR_Plugin.Ptr, handle, dataRef, (uint)dataRef.Length));
		}


		public HapticHandle CreateHandle(AreaFlag location)
		{

			return new HapticHandle(_Play(), _Pause(), _create((uint)location), _Reset(), _name);

		}

	}
	public class CodeSequenceItem
	{
		public float Time;
		public string Effect;
		public float Strength;
		public float Duration;
		public CodeSequenceItem(float time, string effect, float strength = 1.0f, float duration = 0.0f)
		{
			Time = time;
			Effect = effect;
			Strength = strength;
			Duration = duration;
		}


	}
}
