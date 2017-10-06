using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Hardlight.SDK
{

	unsafe struct HLVR_System { }
	unsafe struct HLVR_Event { }
	unsafe struct HLVR_Timeline { }
	unsafe struct HLVR_PlaybackHandle { }
	namespace Internal
	{
		internal static class Interop
		{
			internal const int SUBREGION_BLOCK_SIZE = 1000000;

			public static bool HLVR_SUCCESS(int result)
			{
				return result >= 0;
			}

			public static bool HLVR_FAILURE(int result)
			{
				return !HLVR_SUCCESS(result);
			}

			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			public struct HLVR_Quaternion
			{
				public float w;
				public float x;
				public float y;
				public float z;
			}

			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			public struct HLVR_Color
			{
				public float r;
				public float g;
				public float b;
				public float a;
			}

			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			public struct HLVR_TrackingUpdate
			{
				public HLVR_Quaternion chest;
				public HLVR_Quaternion left_upper_arm;
				public HLVR_Quaternion left_forearm;
				public HLVR_Quaternion right_upper_arm;
				public HLVR_Quaternion right_forearm;
			}

		


			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			public struct HLVR_ServiceInfo
			{
				uint ServiceMajor;
				uint ServiceMinor;
			};


			public enum HLVR_EventType
			{
				Basic_Haptic_Event = 1,
				HLVR_EventType_MAX = 65535
			};

			public enum HLVR_PlaybackCommand
			{
				Play = 0,
				Pause,
				Reset
			}

			public enum HLVR_DeviceStatus
			{
				Unknown = 0,
				Connected = 1,
				Disconnected = 2
			}

			public enum HLVR_DeviceConcept
			{
				Unknown = 0,
				Suit,
				Controller,
				Headwear,
				Gun,
				Sword
			} 

			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			public struct HLVR_DeviceInfo
			{
				public UInt32 Id;
				[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
				public char[] Name;
				public HLVR_DeviceConcept Concept;
				public HLVR_DeviceStatus Status;
			};


			public struct HLVR_DeviceInfo_Iter
			{
				public IntPtr _internal;
				public HLVR_DeviceInfo DeviceInfo;
			}
			public enum HLVR_EffectInfo_State
			{
				HLVR_EffectInfo_State_Playing,
				HLVR_EffectInfo_State_Paused,
				HLVR_EffectInfo_State_Idle
			}

			
			[StructLayout(LayoutKind.Sequential)]
			public struct HLVR_EffectInfo
			{
				public float Duration;
				public float Elapsed;
				HLVR_EffectInfo_State PlaybackState;
			};


			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_Create(HLVR_System** systemPtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe void HLVR_System_Release(HLVR_System** value);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern uint HLVR_Version_Get();

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_Version_IsCompatibleDLL();

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_GetServiceInfo(HLVR_System* systemPtr, ref HLVR_ServiceInfo infoPtr);

			/* Haptics Engine */

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static unsafe extern int HLVR_System_Haptics_Suspend(HLVR_System* systemPtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_Haptics_Resume(HLVR_System* systemPtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_Haptics_Destroy(HLVR_System* systemPtr);

			/* Devices */

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_DeviceInfo_Iter_Init(ref HLVR_DeviceInfo_Iter iter);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe bool HLVR_DeviceInfo_Iter_Next(ref HLVR_DeviceInfo_Iter iter, HLVR_System* system);

			/* Tracking */
			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_Tracking_Poll(HLVR_System* systemPtr, ref HLVR_TrackingUpdate updatePtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_Tracking_Enable(HLVR_System* ptr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_Tracking_Disable(HLVR_System* ptr);


			/* Timeline API */

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_Event_Create(ref IntPtr eventPtr, HLVR_EventType type);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern void HLVR_Event_Release(ref IntPtr eventPtr);

			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_Event_SetFloat(IntPtr eventPtr, string key, float value);

			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_Event_SetInt(IntPtr eventPtr, string key, int value);


			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_Event_SetUInt32s(IntPtr eventPtr, string key, [In, Out] UInt32[] values, uint length);
			/* Timelines */
			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Timeline_Create(ref IntPtr eventListPtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern void HLVR_Timeline_Release(ref IntPtr listPtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_Timeline_AddEvent(IntPtr list, IntPtr eventPtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Timeline_Transmit(IntPtr timeline, HLVR_System* systemPtr, IntPtr handlePr);

			/* Playback */
			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_PlaybackHandle_Create(ref IntPtr handlePtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_PlaybackHandle_Command(IntPtr handlePtr, HLVR_PlaybackCommand command);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern void HLVR_PlaybackHandle_Release(ref IntPtr handlePtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_PlaybackHandle_GetInfo(IntPtr handlePtr, ref HLVR_EffectInfo info);

			/* Sampling */
			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Immediate_Sample(HLVR_System* systemPtr, [In, Out] UInt16[] intensities,  [In, Out] UInt32[] areas, [In, Out] UInt32[] families, int length, ref uint resultCount);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Immediate_Set(HLVR_System* systemPtr, [In, Out] UInt16[] intensities, [In, Out] UInt32[] areas, int length);


			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern  int HLVR_BodyView_Create(ref IntPtr body);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern  int HLVR_BodyView_Release(ref IntPtr body);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_BodyView_Poll(IntPtr body, HLVR_System* system);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_BodyView_GetNodeCount(IntPtr body, ref UInt32 outNodeCount);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern  int HLVR_BodyView_GetNodeType(IntPtr body, UInt32 nodeIndex, ref UInt32 outType);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_BodyView_GetNodeRegion(IntPtr body, UInt32 nodeIndex, ref UInt32 outRegion);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			//only valid if nodeType == intensity
			public static extern int HLVR_BodyView_GetIntensity(IntPtr body, UInt32 nodeIndex, ref float outIntensity);

			[DllImport("NSLoader", CallingConvention = CallingConvention.Cdecl)]
			//only valid if nodeType == color
			public static extern int HLVR_BodyView_GetColor(IntPtr body, UInt32 nodeIndex, ref HLVR_Color outColor);

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
		Lower_Arm_Left = 1 << 0,
		Forearm_Left = 1 << 0,
		Upper_Arm_Left = 1 << 1,
		Shoulder_Left = 1 << 2,
		Back_Left = 1 << 3,
		Chest_Left = 1 << 4,
		Upper_Ab_Left = 1 << 5,
		Mid_Ab_Left = 1 << 6,
		Lower_Ab_Left = 1 << 7,

		Forearm_Right = 1 << 16,
		Lower_Arm_Right = 1 << 16,
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

	

	public enum Region : int
	{
		unknown = 0,
		body = 1 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		torso = 2 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		torso_front = 3 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		chest_left = 4 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		chest_right = 5 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		upper_ab_left = 6 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		middle_ab_left = 7 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		lower_ab_left = 8 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		upper_ab_right = 9 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		middle_ab_right = 10 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		lower_ab_right = 11 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		torso_back = 12 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		torso_left = 13 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		torso_right = 14 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		upper_back_left = 15 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		upper_back_right = 16 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		upper_arm_left = 17 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		lower_arm_left = 18 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		upper_arm_right = 19 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		lower_arm_right = 20 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		shoulder_left = 21 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		shoulder_right = 22 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		upper_leg_left = 23 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		lower_leg_left = 24 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		upper_leg_right = 25 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		lower_leg_right = 26 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		head = 27 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		palm_left = 28 * Internal.Interop.SUBREGION_BLOCK_SIZE,
		palm_right = 29 * Internal.Interop.SUBREGION_BLOCK_SIZE
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

	
	}

	
}
