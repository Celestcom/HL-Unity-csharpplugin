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
		float Time;
		string Effect;
		float Duration;
		float Strength;
		AreaFlag Area;
		public CodeEffect(float time, string effect, float duration, float strength, AreaFlag area)
		{
			Time = time;
			Effect = effect;
			Duration = duration;
			Strength = strength;
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
			Node.AddChildren(builder, children);
			Node.AddTime(builder, 0);
			var root = Node.EndNode(builder);
			return root;
			/*
			 * var weaponOneName = builder.CreateString("Sword");
var weaponOneDamage = 3;
var weaponTwoName = builder.CreateString("Axe");
var weaponTwoDamage = 5;
// Use the `CreateWeapon()` helper function to create the weapons, since we set every field.
var sword = Weapon.CreateWeapon(builder, weaponOneName, (short)weaponOneDamage);
var axe = Weapon.CreateWeapon(builder, weaponTwoName, (short)weaponTwoDamage);
*/
		}
	}
}
