using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatBuffers;
using NullSpace.HapticFiles;

namespace NullSpace.SDK
{
	public class CodeEffect
	{
		public float Time;
		public string Effect;
		public float Duration;
		public float Strength;
		public AreaFlag Area;
		public CodeEffect(float time, string effect,float duration,double strength, AreaFlag area)
		{
			Time = time;
			Effect = effect;
			Duration = duration;
			Strength = (float)strength;
			Area = area;
		}
		internal Offset<Node> Generate(FlatBufferBuilder builder) 
		{
			var effect = builder.CreateString(Effect);
			Node.StartNode(builder);
			Node.AddType(builder, NodeType.Effect);
			Node.AddTime(builder, Time);
			Node.AddEffect(builder, effect);
			Node.AddStrength(builder, Strength);
			Node.AddDuration(builder, Duration);
			var eff = Node.EndNode(builder);

			var childs = new Offset<Node>[1];
			childs[0] = eff;
			var children = Node.CreateChildrenVector(builder, childs);
			Node.StartNode(builder);
			Node.AddType(builder, NodeType.Sequence);
			Node.AddArea(builder, (uint)this.Area);
			Node.AddChildren(builder, children);
			Node.AddTime(builder, 0);
			var root = Node.EndNode(builder);
			return root;
		
		}
	}
}
