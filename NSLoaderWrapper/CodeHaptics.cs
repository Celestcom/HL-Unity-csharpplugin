using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatBuffers;
using NullSpace.HapticFiles;
using NullSpace.SDK.Internal;

namespace NullSpace.SDK
{

	public class CodeEffect  : IGeneratable
	{
		public float Time;
		public string Effect;
		public float Duration;
		public float Strength;
		public CodeEffect(float time, string effect,float duration, double strength = 1.0)
		{
			Time = time;
			Effect = effect;
			Duration = duration;
			Strength = (float)strength;
		}
		public CodeEffect(string effect, float duration, double strength = 1.0)
		{
			Effect = effect;
			Duration = duration;
			Strength = (float)strength;
		}
		Offset<Node> IGeneratable.Generate(FlatBufferBuilder builder) 
		{
			var effect = builder.CreateString(Effect);
			Node.StartNode(builder);
			Node.AddType(builder, NodeType.Effect);
			Node.AddTime(builder, Time);
			Node.AddEffect(builder, effect);
			Node.AddStrength(builder, Strength);
			Node.AddDuration(builder, Duration);
			var root = Node.EndNode(builder);
			return root;
		}

		public CodeEffect Clone()
		{
			return new CodeEffect(this.Time, this.Effect, this.Duration, this.Strength);
		}
	}

	public class CodeSequence :Playable, IGeneratable
	{
		public AreaFlag Area;
		public float Strength;
		public IList<CodeEffect> Children;
		public float Time;
		public CodeSequence(AreaFlag area, float strength)
		{
			Area = area;
			Strength = strength;
			Children = new List<CodeEffect>();

		}
		
		public CodeSequence()
		{
			Area = AreaFlag.None;
			Strength = 1f;
			Children = new List<CodeEffect>();
		}
		public CodeSequence Clone()
		{
			var clone = new CodeSequence();
			clone.Area = this.Area;
			clone.Strength = this.Strength;
			clone.Time = this.Time;
			clone.Children = new List<CodeEffect>(this.Children);
			return clone;
		}
		public void AddChild(CodeEffect e)
		{
			Children.Add(e);
		}
		public void AddChild(float time, CodeEffect e)
		{
			var clone = e.Clone();
			clone.Time = time;
			Children.Add(clone);
		}
		Offset<Node> IGeneratable.Generate(FlatBufferBuilder builder) {
			//var children = new Offset<Node>[Children.Count];
			var children = Node.CreateChildrenVector(builder,
				Children.Select(child => ((IGeneratable)child).Generate(builder)).ToArray());
			Node.StartNode(builder);
			Node.AddType(builder, NodeType.Sequence);
			Node.AddArea(builder, (uint)Area);
			Node.AddStrength(builder, Strength);
			Node.AddTime(builder, Time);
			Node.AddChildren(builder, children);
			var root = Node.EndNode(builder);
			return root;
		}
		private Interop.CommandWithHandle _create(AreaFlag location)
		{
			return new Interop.CommandWithHandle(handle => {
				Area = location;
				var bytes = EncodingUtils.Encode(this, handle);
				Interop.NSVR_CreateHaptic(NSVR.NSVR_Plugin.Ptr, handle, bytes, (uint)bytes.Length);
			});
			

		}
		public HapticHandle CreateHandle(AreaFlag area)
		{
			return new HapticHandle(_Play(), _Pause(), _create(area), _Reset());
		}
		public HapticHandle Play(AreaFlag area)
		{
			var handle = CreateHandle(area);
			handle.Play();
			return handle;
		}
	}
	internal interface IGeneratable
	{
		Offset<Node> Generate(FlatBufferBuilder builder);
	}
	public class CodePattern : Playable, IGeneratable
	{
		public float Strength;
		public float Time;
		public IList<CodeSequence> Children;

		public CodePattern()
		{
			Children = new List<CodeSequence>();
			Time = 0f;
			Strength = 1f;
		}
		public void AddChild(CodeSequence e)
		{
			Children.Add(e);
		}

		public void AddChild(float time, AreaFlag area, CodeSequence e)
		{
			var clone = e.Clone();
			clone.Time = time;
			clone.Area = area;
			Children.Add(clone);
		}
		Offset<Node> IGeneratable.Generate(FlatBufferBuilder builder)
		{
			//var children = new Offset<Node>[Children.Count];
			var children = Node.CreateChildrenVector(builder,
				Children.Select(child => ((IGeneratable)child).Generate(builder)).ToArray());
			Node.StartNode(builder);
			Node.AddType(builder, NodeType.Pattern);
			Node.AddTime(builder, Time);
			Node.AddStrength(builder, Strength);
			Node.AddChildren(builder, children);
			var root = Node.EndNode(builder);
			return root;
		}

		private void _create(uint handle)
		{
			var bytes = EncodingUtils.Encode(this, handle);
			Interop.NSVR_CreateHaptic(NSVR.NSVR_Plugin.Ptr, handle, bytes, (uint)bytes.Length);

		}
		public HapticHandle CreateHandle()
		{
			return new HapticHandle(_Play(), _Pause(), _create, _Reset());
		}

		public HapticHandle Play()
		{
			var handle = CreateHandle();
			handle.Play();
			return handle;
		}

	}
	/// <summary>
	/// Todo: implement this. It would gather any handles and dispose of them for you
	/// </summary>
	public class HandleCollector
	{
		private IList<HapticHandle> _handles;
		public HandleCollector()
		{
			_handles = new List<HapticHandle>();
		}
		public void Take(HapticHandle h)
		{
			_handles.Add(h);
		}

		public void Cleanup()
		{
			foreach (var handle in _handles)
			{
				handle.Dispose();
			}

			_handles.Clear();
		}
	}
	
}
