using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NullSpace.SDK
{

	unsafe struct NSVR_System { }
	unsafe struct NSVR_Event { }
	unsafe struct NSVR_Timeline { }
	unsafe struct NSVR_PlaybackHandle { }
	namespace Internal
	{
		internal static class Interop
		{
			public static bool NSVR_SUCCESS(int result)
			{
				return result >= 0;
			}

			public static bool NSVR_FAILURE(int result)
			{
				return !NSVR_SUCCESS(result);
			}

			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			public struct NSVR_Quaternion
			{
				public float w;
				public float x;
				public float y;
				public float z;
			}

			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			public struct NSVR_TrackingUpdate
			{
				public NSVR_Quaternion chest;
				public NSVR_Quaternion left_upper_arm;
				public NSVR_Quaternion left_forearm;
				public NSVR_Quaternion right_upper_arm;
				public NSVR_Quaternion right_forearm;
			}

		


			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			public struct NSVR_ServiceInfo
			{
				uint ServiceMajor;
				uint ServiceMinor;
			};


			public enum NSVR_EventType
			{
				Basic_Haptic_Event = 1,
				NSVR_EventType_MAX = 65535
			};

			public enum NSVR_PlaybackCommand
			{
				Play = 0,
				Pause,
				Reset
			}

			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			public struct NSVR_DeviceInfo
			{
				[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
				public char[] ProductName;
				public short FirmwareMajor;
				public short FirmwareMinor;
				//tracking capabilities?
			};

			public enum NSVR_EffectInfo_State
			{
				Playing = 0,
				Paused,
				Idle
			}
			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			public struct NSVR_EffectInfo
			{
				public float Duration;
				public float Elapsed;
				public NSVR_EffectInfo_State PlaybackState;
			};


			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern unsafe int NSVR_System_Create(NSVR_System** systemPtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern unsafe void NSVR_System_Release(NSVR_System** value);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern uint NSVR_Version_Get();

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_Version_IsCompatibleDLL();

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern unsafe int NSVR_System_GetServiceInfo(NSVR_System* systemPtr, ref NSVR_ServiceInfo infoPtr);

			/* Haptics Engine */

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static unsafe extern int NSVR_System_Haptics_Pause(NSVR_System* systemPtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern unsafe int NSVR_System_Haptics_Resume(NSVR_System* systemPtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern unsafe int NSVR_System_Haptics_Destroy(NSVR_System* systemPtr);

			/* Devices */
			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern unsafe int NSVR_System_GetDeviceInfo(NSVR_System* systemPtr, ref NSVR_DeviceInfo infoPtr);

			/* Tracking */
			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern unsafe int NSVR_System_Tracking_Poll(NSVR_System* systemPtr, ref NSVR_TrackingUpdate updatePtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern unsafe int NSVR_System_Tracking_Enable(NSVR_System* ptr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern unsafe int NSVR_System_Tracking_Disable(NSVR_System* ptr);


			/* Timeline API */

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_Event_Create(ref IntPtr eventPtr, NSVR_EventType type);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern void NSVR_Event_Release(ref IntPtr eventPtr);

			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_Event_SetFloat(IntPtr eventPtr, string key, float value);

			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_Event_SetInteger(IntPtr eventPtr, string key, int value);


			/* Timelines */
			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern unsafe int NSVR_Timeline_Create(ref IntPtr eventListPtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern void NSVR_Timeline_Release(ref IntPtr listPtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_Timeline_AddEvent(IntPtr list, IntPtr eventPtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern unsafe int NSVR_Timeline_Transmit(IntPtr timeline, NSVR_System* systemPtr, IntPtr handlePr);

			/* Playback */
			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_PlaybackHandle_Create(ref IntPtr handlePtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_PlaybackHandle_Command(IntPtr handlePtr, NSVR_PlaybackCommand command);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern void NSVR_PlaybackHandle_Release(ref IntPtr handlePtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_PlaybackHandle_GetInfo(IntPtr handlePtr, ref NSVR_EffectInfo info);

			/* Sampling */
			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern unsafe int NSVR_Immediate_Sample(NSVR_System* systemPtr, [In, Out] UInt16[] intensities,  [In, Out] UInt32[] areas, [In, Out] UInt32[] families, int length, ref uint resultCount);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern unsafe int NSVR_Immediate_Set(NSVR_System* systemPtr, [In, Out] UInt16[] intensities, [In, Out] UInt32[] areas, int length);

		}


	}
	public enum ServiceConnectionStatus
	{
		Disconnected = 0,
		Connected = 1
	}


	public enum DeviceConnectionStatus
	{
		Disconnected = 0,
		Connected = 1
	}
	public enum Imu
	{
		Chest = 0,
		Right_Upper_Arm = 1,
		Left_Upper_Arm = 2,
		Right_Forearm = 3,
		Left_Forearm = 4
	};

	[Flags]
	public enum AreaFlag
	{
		None,
		Forearm_Left = 1 << 0,
		Upper_Arm_Left = 1 << 1,
		Shoulder_Left = 1 << 2,
		Back_Left = 1 << 3,
		Chest_Left = 1 << 4,
		Upper_Ab_Left = 1 << 5,
		Mid_Ab_Left = 1 << 6,
		Lower_Ab_Left = 1 << 7,

		Forearm_Right = 1 << 16,
		Upper_Arm_Right = 1 << 17,
		Shoulder_Right = 1 << 18,
		Back_Right = 1 << 19,
		Chest_Right = 1 << 20,
		Upper_Ab_Right = 1 << 21,
		Mid_Ab_Right = 1 << 22,
		Lower_Ab_Right = 1 << 23,
		Forearm_Both = Forearm_Left | Forearm_Right,
		Upper_Arm_Both = Upper_Arm_Left | Upper_Arm_Right,
		Shoulder_Both = Shoulder_Left | Shoulder_Right,
		Back_Both = Back_Left | Back_Right,
		Chest_Both = Chest_Left | Chest_Right,
		Upper_Ab_Both = Upper_Ab_Left | Upper_Ab_Right,
		Mid_Ab_Both = Mid_Ab_Left | Mid_Ab_Right,
		Lower_Ab_Both = Lower_Ab_Left | Lower_Ab_Right,
		Left_All = 0x000000FF,
		Right_All = 0x00FF0000,
		All_Areas = Left_All | Right_All,
	};
	public static class AreaFlagExtensions
	{
		/// <summary>
		/// Checks if an AreaFlag contains another AreaFlag
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns>True of the AreaFlag contains the other, else false</returns>
		public static bool ContainsArea(this AreaFlag lhs, AreaFlag rhs)
		{
			if ((lhs & rhs) == rhs)
			{
				return true;
			} else
			{
				return false;
			}
		}

		/// <summary>
		/// Adds the given area. Make sure to use the return value! Equivalent to area1 | area2
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static AreaFlag AddArea(this AreaFlag lhs, AreaFlag other)
		{
			return lhs = lhs | other;
		}

		/// <summary>
		/// Removes the given area. Make sure to use the return value! Equivalent to area1 & ~area2
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static AreaFlag RemoveArea(this AreaFlag lhs, AreaFlag other)
		{
			return lhs & ~other;
		}


		public static bool IsSingleArea(this AreaFlag baseFlag)
		{
			return baseFlag.NumberOfAreas() == 1;
		}
		/// <summary>
		/// For getting the number of set AreaFlags contained in the flag it is called on.
		/// </summary>
		/// <param name="baseFlag">I wonder how many flags are in this. Let's find out!</param>
		/// <returns>0 to 16 pads (depending on how many are in baseFlag)</returns>
		public static int NumberOfAreas(this AreaFlag baseFlag)
		{
			//This is credited as the Hamming Weight, Popcount or Sideways Addition.
			//Source: Stack Overflow
			//https://stackoverflow.com/questions/109023/how-to-count-the-number-of-set-bits-in-a-32-bit-integer

			//Really cool way to count the number of flags.

			int i = (int)baseFlag;
			// Java: use >>> instead of >>
			// C or C++: use uint32_t
			i = i - ((i >> 1) & 0x55555555);
			i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
			return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
		}
		/// <summary>
		/// A chainable function for adding flags together.
		/// Functionally identical to (baseFlag = baseFlag | added)
		/// </summary>
		/// <param name="baseFlag">The flag we want to add more flags to</param>
		/// <param name="added">The flags to add to the baseFlag</param>
		/// <returns>The resulted added flag.</returns>
		public static AreaFlag AddFlag(this AreaFlag baseFlag, AreaFlag added)
		{
			baseFlag = baseFlag | added;
			return baseFlag;
		}

		/// <summary>
		/// Check if the checked flag is set inside of baseFlag.
		/// </summary>
		/// <param name="baseFlag">The flag that might contain checkFlag</param>
		/// <param name="checkFlag">The flag(s) that we want to look for, can accept complex flags</param>
		/// <returns>Whether or not the base flag has ALL of the flags in checkFlag</returns>
		public static bool HasFlag(this AreaFlag baseFlag, AreaFlag checkFlag)
		{
			return HasFlag(baseFlag, (int)checkFlag);
		}

		/// <summary>
		/// An overload to use numerical values to see if we have the the requested flag(s)
		/// </summary>
		/// <param name="baseFlag">The flag that might contain the int equivalent (flag)</param>
		/// <param name="flag">A value between 0 and 16711936</param>
		/// <returns>Whether or not the base flag has ALL of the flags in flag (converted to an AreaFlag)</returns>
		public static bool HasFlag(this AreaFlag baseFlag, int flag)
		{
			if (((int)baseFlag & (flag)) == flag)
			{
				return true;
			}
			return false;
		}

		public static AreaFlag[] StaticAreaFlag =
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

		/// <summary>
		/// This function does NOT break apart the AreaFlag it is called on. For that, call ToArray()
		/// Returns an array of ALL single AreaFlags in the enum.
		/// Does not include Boths, Alls or None
		/// </summary>
		/// <param name="baseFlag">This value does not matter at all. You cannot append extension methods to Enums.</param>
		/// <returns>Returns an array of each single area in the enum. (No Boths, Alls or None AreaFlags)</returns>
		public static AreaFlag[] AllSingleAreasInEnum(this AreaFlag baseFlag)
		{
			return StaticAreaFlag;
		}
		/// <summary>
		/// Breaks a multiple AreaFlag into a list of Single AreaFlags
		/// Ex: Multi Area Flag = AreaFlag.Forearm_Left|AreaFlag.Forearm_Right
		/// Return: {AreaFlag.Forearm_Left, AreaFlag.Forearm_Right
		/// </summary>
		/// <param name="baseFlag">The flag to be evaluated.</param>
		/// <returns>Retusn an array of single AreaFlags. Will not include None, Boths or All AreaFlag values.</returns>
		public static AreaFlag[] ToArray(this AreaFlag baseFlag)
		{
			AreaFlag[] values = baseFlag.AllSingleAreasInEnum();

			List<AreaFlag> has = new List<AreaFlag>();
			for (int i = 0; i < values.Length; i++)
			{
				if (baseFlag.HasFlag(values[i]))
				{
					has.Add(values[i]);
				}
			}

			//
			//if (has.Count < 1)
			//{
			//	has.Add(AreaFlag.None);
			//}

			return has.ToArray();
		}
	}

	
}
