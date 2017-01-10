using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatBuffers;
using NullSpace.HapticFiles;
using NullSpace.SDK.Internal;

namespace NullSpace.SDK
{
	/// <summary>
	/// hum, buzz, click, double_click, fuzz, long_double_sharp_tick, <para></para>pulse, pulse_sharp, sharp_click, sharp_tick, short_double_click, short_double_sharp_tick, transition_click, transition_hum, triple_click
	/// </summary>
	public class CodeEffect  : IGeneratable
	{

		private float _time;
		private string _effect;
		private float _duration;
		private float _strength;

		public float Time
		{
			get
			{
				return _time;
			}

			set
			{
				_time = value;
			}
		}

		public string Effect
		{
			get
			{
				return _effect;
			}

			set
			{
				_effect = value;
			}
		}

		public double Duration
		{
			get
			{
				return _duration;
			}

			set
			{
				_duration = (float)value;
			}
		}

		public double Strength
		{
			get
			{
				return _strength;
			}

			set
			{
				_strength = (float)value;
			}
		}

		public CodeEffect(string effect)
		{
			_effect = effect;
			_duration = 0f;
			_strength = 1f;
		}

		public CodeEffect(string effect, double duration)
		{
			_effect = effect;
			Duration = duration;
			_strength = 1f;
		}

		public CodeEffect(string effect, double duration, double strength)
		{
			_effect = effect;
			Duration = duration;
			Strength = strength;
		}
		Offset<Node> IGeneratable.Generate(FlatBufferBuilder builder) 
		{
			var effect = builder.CreateString(_effect);
			Node.StartNode(builder);
			Node.AddType(builder, NodeType.Effect);
			Node.AddTime(builder, _time);
			Node.AddEffect(builder, effect);
			Node.AddStrength(builder, _strength);
			Node.AddDuration(builder, _duration);
			var root = Node.EndNode(builder);
			return root;
		}
	
		public CodeEffect Clone()
		{
			return new CodeEffect(_effect, Duration, Strength);
		}
	}
	public class CodeSequence :Playable, IGeneratable
	{
		private AreaFlag _area;
		private float _strength;
		private IList<CodeEffect> _children;
		private float _time;
		public AreaFlag Area
		{
			get
			{
				return _area;
			}

			set
			{
				_area = value;
			}
		}

		public double Strength
		{
			get
			{
				return _strength;
			}

			set
			{
				_strength = (float)value;
			}
		}

		public IList<CodeEffect> Children
		{
			get
			{
				return _children;
			}

			set
			{
				_children = value;
			}
		}

		public double Time
		{
			get
			{
				return _time;
			}

			set
			{
				_time = (float) value;
			}
		}

		
		public CodeSequence()
		{
			_area = AreaFlag.None;
			_strength = 1f;
			_children = new List<CodeEffect>();
		}
		public CodeSequence Clone()
		{
			var clone = new CodeSequence();
			clone.Area = _area;
			clone.Strength = _strength;
			clone.Time = _time;
			clone.Children = new List<CodeEffect>(_children);
			return clone;
		}
		
		public void AddEffect(double time, CodeEffect e)
		{
			var clone = e.Clone();
			clone.Time = (float)time;
			Children.Add(clone);
		}
		Offset<Node> IGeneratable.Generate(FlatBufferBuilder builder) {
			return this.Bake(Area, _strength)(builder);
		}

		
		EncodingUtils.BuffEncoder Bake(AreaFlag area, double strength)
		{
			return delegate (FlatBufferBuilder builder)
			{
				var children = Node.CreateChildrenVector(builder,
				_children.Select(child => ((IGeneratable)child).Generate(builder)).ToArray());
				Node.StartNode(builder);
				Node.AddType(builder, NodeType.Sequence);
				Node.AddArea(builder, (uint)area);
				Node.AddStrength(builder, (float)strength);
				Node.AddTime(builder, _time);
				Node.AddChildren(builder, children);
				return Node.EndNode(builder);
			};
		}
		private Interop.CommandWithHandle _create(AreaFlag location, double strength = 1.0)
		{
			return new Interop.CommandWithHandle(handle => {
			
				
				var bytes = EncodingUtils.EncodeDel(this.Bake(location, strength), handle);
				Interop.NSVR_CreateHaptic(NSVR.NSVR_Plugin.Ptr, handle, bytes, (uint)bytes.Length);
			});
			

		}

		public HapticHandle CreateHandle(AreaFlag area)
		{
			return new HapticHandle(_Play(), _Pause(), _create(area), _Reset());
		}
		public HapticHandle CreateHandle(AreaFlag area, double strength)
		{
			return new HapticHandle(_Play(), _Pause(), _create(area, strength), _Reset());
		}
		public HapticHandle Play(AreaFlag area)
		{
			var handle = CreateHandle(area);
			handle.Play();
			return handle;
		}

		public HapticHandle Play(AreaFlag area, double strength)
		{
			var handle = CreateHandle(area, strength);
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
		private float _strength;
		private float _time;
		private IList<CodeSequence> _children;

		public double Strength
		{
			get
			{
				return _strength;
			}

			set
			{
				_strength = (float)value;
			}
		}

		public double Time
		{
			get
			{
				return _time;
			}

			set
			{
				_time = (float)value;
			}
		}

		public IList<CodeSequence> Children
		{
			get
			{
				return _children;
			}

			set
			{
				_children = value;
			}
		}

		public CodePattern()
		{
			_children = new List<CodeSequence>();
			_time = 0f;
			_strength = 1f;
		}
		
		public void AddSequence(double time, AreaFlag area, CodeSequence sequence)
		{
			var clone = sequence.Clone();
			clone.Time = time;
			clone.Area = area;
			clone.Strength = 1.0;
			_children.Add(clone);
		}

		public void AddSequence(double time, AreaFlag area, double strength, CodeSequence sequence)
		{
			var clone = sequence.Clone();
			clone.Strength = strength;
			clone.Time = time;
			clone.Area = area;
			_children.Add(clone);
		}

		Offset<Node> IGeneratable.Generate(FlatBufferBuilder builder)
		{
			return this.Bake(_strength)(builder);
			
		}
	
		EncodingUtils.BuffEncoder Bake(double strength)
		{
			return delegate (FlatBufferBuilder builder)
			{
				var children = Node.CreateChildrenVector(builder,
				_children.Select(child => ((IGeneratable)child).Generate(builder)).ToArray());
				Node.StartNode(builder);
				Node.AddType(builder, NodeType.Pattern);
				Node.AddTime(builder, _time);
				Node.AddStrength(builder, (float)strength);
				Node.AddChildren(builder, children);
				var root = Node.EndNode(builder);
				return root;
			};
		}
		private Interop.CommandWithHandle _create(double strength = 1.0)
		{
			return new Interop.CommandWithHandle(handle => {
				
				var bytes = EncodingUtils.EncodeDel(this.Bake(strength), handle);
				Interop.NSVR_CreateHaptic(NSVR.NSVR_Plugin.Ptr, handle, bytes, (uint)bytes.Length);
			});
		

		}
		public HapticHandle CreateHandle()
		{
			return new HapticHandle(_Play(), _Pause(), _create(_strength), _Reset());
		}
		public HapticHandle CreateHandle(double strength)
		{
			return new HapticHandle(_Play(), _Pause(), _create(strength), _Reset());
		}

		public HapticHandle Play()
		{
			var handle = CreateHandle();
			handle.Play();
			return handle;
		}

		public HapticHandle Play(double strength)
		{
			var handle = CreateHandle(strength);
			handle.Play();
			return handle;
		}
	}
	
	
}
