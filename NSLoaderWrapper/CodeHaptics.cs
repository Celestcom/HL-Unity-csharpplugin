using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatBuffers;
using NullSpace.HapticFiles;
using NullSpace.HapticFiles.Mixed;
using static NullSpace.SDK.Internal.Interop;
using NullSpace.SDK.Internal;

namespace NullSpace.SDK
{
	
	public class CodeSequence : Playable, IPlayable
	{
		private string _name;
		public string Name
		{
			get { return _name; }
		}
		private Offset<HapticEffect>[] _offset;
		private List<CodeSequenceItem> _items;

		public List<CodeSequenceItem> Effects
		{
			get { return _items; }
		}
		public CodeSequence(string name)
		{
			_name = name;
			_items = new List<CodeSequenceItem>();
			
		}
		public void Add(CodeSequenceItem s)
		{
			_items.Add(s);
		}

		private byte[] load(uint location)
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
			return dataRef;
		}
		private CommandWithHandle _create(uint location)
		{

			var dataRef = load(location);
		
			return new CommandWithHandle(handle => Interop.NSVR_CreateCodeSequence(NSVR.NSVR_Plugin.Ptr, handle, dataRef, (uint)dataRef.Length));
		}


		public HapticHandle CreateHandle(AreaFlag location)
		{

			return new HapticHandle(_Play(), _Pause(), _create((uint)location), _Reset(), _name);

		}

		public void Load(string id)
		{
			_items.Clear();
			_name = id;
			bool loaded = Interop.NSVR_LoadSequence(NSVR.NSVR_Plugin.Ptr, _name);
			if (!loaded)
			{
				throw new HapticsLoadingException(NSVR.GetError());
			}
		}
	
		public CodeSequence Clone()
		{
			var temp = new CodeSequence(_name);
			temp._items = new List<CodeSequenceItem>(_items);
			return temp;
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



	public class CodePattern : Playable, IPlayable
	{
		static void Assert(bool assertPassed, string codeExecuted, string actualValue,
					string expectedValue)
		{
			if (assertPassed == false)
			{
				Console.WriteLine("Assert failed! " + codeExecuted + " (" + actualValue +
					") was not equal to " + expectedValue + ".");
				System.Environment.Exit(1);
			}
		}
		private string _name;
		private Offset<HapticFiles.Mixed.HapticFrame>[] _offset;
		private List<CodePatternItem> _codeItems;
		private List<PatternItem> _staticItems;
		public CodePattern(string name)
		{
			_name = name;
			_codeItems = new List<CodePatternItem>();
			_staticItems = new List<PatternItem>();
		}
		public void Add(CodePatternItem patternItem)
		{
			_codeItems.Add(patternItem);
		}
		public void Add(PatternItem item)
		{
			_staticItems.Add(item);
		}
		private byte[] load()
		{
			var builder = new FlatBufferBuilder(1);
			_offset = new Offset<HapticFiles.Mixed.HapticFrame>[_codeItems.Count+_staticItems.Count];
			for (int i = 0; i < _codeItems.Count; i++)
			{
				CodePatternItem item = _codeItems[i];
				var o = new Offset<HapticEffect>[item.Sequence.Effects.Count];

				for (int j = 0; j < item.Sequence.Effects.Count; j++)
				{
					CodeSequenceItem seqItem = item.Sequence.Effects[j];
					
					o[j] = HapticFiles.HapticEffect.CreateHapticEffect(builder, seqItem.Time, builder.CreateString(seqItem.Effect), seqItem.Strength, seqItem.Duration);
				}
				var listItems = HapticFiles.Mixed.ListOfHapticEffects.CreateItemsVector(builder, o);
				var efs = HapticFiles.Mixed.ListOfHapticEffects.CreateListOfHapticEffects(builder, listItems);
				var finishedSeq = HapticFiles.Mixed.Sequence.CreateSequence(builder, (uint)item.Area, EffectValuesOrNameReference.ListOfHapticEffects, efs.Value);

				_offset[i] = HapticFiles.Mixed.HapticFrame.CreateHapticFrame(builder, item.Time, finishedSeq, (uint)item.Area);

			}
			int offsetIndex = _codeItems.Count;
			for (int i = 0; i < _staticItems.Count; i++)
			{
				PatternItem item = _staticItems[i];
				var seqRef = HapticFiles.Mixed.SeqRef.CreateSeqRef(builder, builder.CreateString(item.Sequence.Name));
				var finishedSeq = HapticFiles.Mixed.Sequence.CreateSequence(builder, (uint)item.Area, EffectValuesOrNameReference.SeqRef, seqRef.Value);
				_offset[offsetIndex+i] = HapticFiles.Mixed.HapticFrame.CreateHapticFrame(builder, item.Time, finishedSeq, (uint)item.Area);
			}

			var frames = HapticFiles.Mixed.Pattern.CreateItemsVector(builder, _offset);
			var finishedPattern = HapticFiles.Mixed.Pattern.CreatePattern(builder, frames);
			builder.Finish(finishedPattern.Value);
			var dataRef = builder.SizedByteArray();
			return dataRef;
		}
		private CommandWithHandle _create()
		{

			var dataRef = load();

			return new CommandWithHandle(handle => Interop.NSVR_CreateCodePattern(NSVR.NSVR_Plugin.Ptr, handle, dataRef, (uint)dataRef.Length));
		}

		public HapticHandle CreateHandle()
		{

			return new HapticHandle(_Play(), _Pause(), _create(), _Reset(), _name);

		}

		public void Load(string id)
		{
			throw new NotImplementedException();
		}
	}
	public class PatternItem
	{
		public float Time;
		public Sequence Sequence;
		public AreaFlag Area;
		public PatternItem(float time, Sequence sequence, AreaFlag area)
		{
			Time = time;
			Sequence = sequence;
			Area = area;
		}
	}
	public class CodePatternItem
	{
		public float Time;
		public CodeSequence Sequence;
		public AreaFlag Area;
		public CodePatternItem(float time, CodeSequence sequence, AreaFlag area)
		{
			Time = time;
			Sequence = sequence;
			Area = area;
		}

	}
}
