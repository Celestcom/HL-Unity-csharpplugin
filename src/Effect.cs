using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Hardlight.SDK
{

	public enum Effect
	{
		Bump = 1,
		Buzz = 2,
		Click = 3,
		Fuzz = 5,
		Hum = 6,
		Pulse = 8,
		Tick = 11,
		Double_Click = 4,
		Triple_Click = 16
	}

	public static class AreaFlagToRegion
	{
		private static Dictionary<AreaFlag, Region> _regions;

		static AreaFlagToRegion()
		{
			_regions = new Dictionary<AreaFlag, Region>()
			{
				{AreaFlag.Chest_Left, Region.chest_left},
				{AreaFlag.Upper_Ab_Left, Region.upper_ab_left },
				{AreaFlag.Mid_Ab_Left, Region.middle_ab_left },
				{AreaFlag.Lower_Ab_Left, Region.lower_ab_left },
				{AreaFlag.Shoulder_Left, Region.shoulder_left },
				{AreaFlag.Upper_Arm_Left, Region.upper_arm_left },
				{AreaFlag.Lower_Arm_Left, Region.lower_arm_left },
				{AreaFlag.Back_Left, Region.upper_back_left },
				{AreaFlag.Chest_Right, Region.chest_right},
				{AreaFlag.Upper_Ab_Right, Region.upper_ab_right },
				{AreaFlag.Mid_Ab_Right, Region.middle_ab_right },
				{AreaFlag.Lower_Ab_Right, Region.lower_ab_right },
				{AreaFlag.Shoulder_Right, Region.shoulder_right },
				{AreaFlag.Upper_Arm_Right, Region.upper_arm_right },
				{AreaFlag.Lower_Arm_Right, Region.lower_arm_right },
				{AreaFlag.Back_Right, Region.upper_back_right }
			};
		}
		private static AreaFlag[] StaticAreaFlag =
		{
			AreaFlag.Forearm_Left,
			AreaFlag.Upper_Arm_Left,
			AreaFlag.Shoulder_Left,
			AreaFlag.Back_Left,
			AreaFlag.Chest_Left,
			AreaFlag.Upper_Ab_Left,
			AreaFlag.Mid_Ab_Left,
			AreaFlag.Lower_Ab_Left,
			AreaFlag.Forearm_Right,
			AreaFlag.Upper_Arm_Right,
			AreaFlag.Shoulder_Right,
			AreaFlag.Back_Right,
			AreaFlag.Chest_Right,
			AreaFlag.Upper_Ab_Right,
			AreaFlag.Mid_Ab_Right,
			AreaFlag.Lower_Ab_Right
		};

		public static UInt32[] GetRegions(AreaFlag area)
		{

			List<UInt32> results = new List<uint>();
			foreach (AreaFlag areaEnum in StaticAreaFlag)
			{
				if (area.ContainsArea(areaEnum))
				{
					if (_regions.ContainsKey(areaEnum))
					{
						Region translated = _regions[areaEnum];
						results.Add((UInt32)(translated));
					}
					else
					{
						Debug.Log("Couldn't find the region corresponding to area " + areaEnum);
					}

				}
			}
			return results.ToArray();
		}
	}

	public static class RegionToAreaFlag
	{
		private static Dictionary<Region, AreaFlag> _lookup;

		static RegionToAreaFlag()
		{
			_lookup = new Dictionary<Region, AreaFlag>()
			{
				{Region.chest_left, AreaFlag.Chest_Left},
				{Region.upper_ab_left, AreaFlag.Upper_Ab_Left},
				{Region.middle_ab_left, AreaFlag.Mid_Ab_Left},
				{Region.lower_ab_left, AreaFlag.Lower_Ab_Left},
				{Region.shoulder_left, AreaFlag.Shoulder_Left},
				{Region.upper_arm_left, AreaFlag.Upper_Arm_Left},
				{Region.lower_arm_left, AreaFlag.Lower_Arm_Left},
				{Region.upper_back_left, AreaFlag.Back_Left},
				{Region.chest_right, AreaFlag.Chest_Right},
				{Region.upper_ab_right, AreaFlag.Upper_Ab_Right},
				{Region.middle_ab_right, AreaFlag.Mid_Ab_Right},
				{Region.lower_ab_right, AreaFlag.Lower_Ab_Right},
				{Region.shoulder_right, AreaFlag.Shoulder_Right},
				{Region.upper_arm_right, AreaFlag.Upper_Arm_Right},
				{Region.lower_arm_right, AreaFlag.Lower_Arm_Right},
				{Region.upper_back_right, AreaFlag.Back_Right}
			};
		}
		#region Regions
		private static Region[] StaticRegions =
		{
			Region.chest_left,
			Region.upper_ab_left,
			Region.middle_ab_left,
			Region.lower_ab_left,
			Region.shoulder_left,
			Region.upper_arm_left,
			Region.lower_arm_left,
			Region.upper_back_left,
			Region.chest_right,
			Region.upper_ab_right,
			Region.middle_ab_right,
			Region.lower_ab_right,
			Region.shoulder_right,
			Region.upper_arm_right,
			Region.lower_arm_right,
			Region.upper_back_right
		};
		#endregion
		#region Area Flags
		private static AreaFlag[] StaticAreaFlag =
		{
			AreaFlag.Forearm_Left,
			AreaFlag.Upper_Arm_Left,
			AreaFlag.Shoulder_Left,
			AreaFlag.Back_Left,
			AreaFlag.Chest_Left,
			AreaFlag.Upper_Ab_Left,
			AreaFlag.Mid_Ab_Left,
			AreaFlag.Lower_Ab_Left,
			AreaFlag.Forearm_Right,
			AreaFlag.Upper_Arm_Right,
			AreaFlag.Shoulder_Right,
			AreaFlag.Back_Right,
			AreaFlag.Chest_Right,
			AreaFlag.Upper_Ab_Right,
			AreaFlag.Mid_Ab_Right,
			AreaFlag.Lower_Ab_Right
		}; 
		#endregion

		public static AreaFlag GetAreaFlag(Region region)
		{
			if (_lookup.ContainsKey(region))
			{
				return _lookup[region];
			}
			return AreaFlag.None;
			//List<UInt32> results = new List<uint>();
			//foreach (AreaFlag areaEnum in StaticRegions)
			//{
			//	if (area.ContainsArea(areaEnum))
			//	{
			//		if (_lookup.ContainsKey(areaEnum))
			//		{
			//			Region translated = _lookup[areaEnum];
			//			results.Add((UInt32)(translated));
			//		}
			//		else
			//		{
			//			Debug.Log("Couldn't find the region corresponding to area " + areaEnum);
			//		}

			//	}
			//}
			//return results.ToArray();
		}
	}


	/// <summary>
	/// Used to map the effect written in a .hdf into the enum based version. 
	/// Inside the engine this is translated back into a string, so this pipeline should be
	/// fixed up. We need to be backwards compatible. 
	/// </summary>
	internal static class FileEffectToCodeEffect
	{
		private static Dictionary<string, Effect> _effects;

		static FileEffectToCodeEffect()
		{
			_effects = new Dictionary<string, Effect>()
					{
						{"bump", Effect.Bump},
						{"buzz", Effect.Buzz },
						{"click", Effect.Click },
						{"sharp_click", Effect.Click},
						{"fuzz", Effect.Fuzz},
						{"hum", Effect.Hum},
						{"long_double_sharp_tick", Effect.Double_Click},
						{"pulse", Effect.Pulse},
						{"pulse_sharp", Effect.Pulse},
						{"sharp_tick", Effect.Tick },
						{"short_double_click", Effect.Double_Click},
						{"short_double_sharp_tick", Effect.Double_Click},
						{"transition_click", Effect.Click},
						{"transition_hum", Effect.Hum},
						{"triple_click", Effect.Triple_Click },
						{"double_click" , Effect.Double_Click}
					};
		}

		/// <summary>
		/// Attempt to parse a string into an Effect, returning defaultEffect if fail
		/// </summary>
		/// <param name="effect">A potential effect</param>
		/// <param name="defaultEffect">The default to return if parsing fails</param>
		/// <returns></returns>
		public static Effect TryParse(string effect, Effect defaultEffect)
		{


			if (_effects.ContainsKey(effect))
			{
				return _effects[effect];
			}
			else
			{
				return defaultEffect;
			}
		}
	}
}
