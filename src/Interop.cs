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
	unsafe struct HLVR_Effect { }

	namespace Internal
	{
		internal static class Interop
		{
			internal const int SUBREGION_BLOCK_SIZE = 1000000;

			public static bool OK(int result)
			{
				return result >= 0;
			}

			public static bool FAIL(int result)
			{
				return !OK(result);
			}


			[StructLayout(LayoutKind.Sequential)]
			public struct HLVR_Quaternion
			{
				public float w;
				public float x;
				public float y;
				public float z;
			}
			[StructLayout(LayoutKind.Sequential)]
			public struct HLVR_Vector3f
			{
				public float x;
				public float y;
				public float z;
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct HLVR_TrackingUpdate
			{
				public HLVR_Quaternion chest;
				public HLVR_Quaternion left_upper_arm;
				public HLVR_Quaternion left_forearm;
				public HLVR_Quaternion right_upper_arm;
				public HLVR_Quaternion right_forearm;

				public HLVR_Vector3f chest_gravity;
				public HLVR_Vector3f chest_compass;
				public HLVR_Vector3f left_upper_arm_gravity;
				public HLVR_Vector3f left_upper_arm_compass;
				public HLVR_Vector3f left_forearm_gravity;
				public HLVR_Vector3f left_forearm_compass;
				public HLVR_Vector3f right_upper_arm_gravity;
				public HLVR_Vector3f right_upper_arm_compass;
				public HLVR_Vector3f right_forearm_gravity;
				public HLVR_Vector3f right_forearm_compass;
			}

	
			[StructLayout(LayoutKind.Sequential)]
			public struct HLVR_RuntimeInfo
			{
				public uint MajorVersion;
				public uint MinorVersion;
			};

			public enum HLVR_EventKey
			{
				Unknown = 0,
				/* Common Keys */
				Target_Regions_UInt32s,
				Target_Nodes_UInt32s,

				/* Event-Specific keys */
				DiscreteHaptic_Repetitions_UInt32 = 1000,
				DiscreteHaptic_Strength_Float,
				DiscreteHaptic_Waveform_Int,

				BufferedHaptic_Samples_Floats = 3000,
				BufferedHaptic_Frequency_Float,


			}


			public enum HLVR_EventType
			{
				Unknown = 0,
				DiscreteHaptic = 1,
				BufferedHaptic = 3,
				BeginAnalogAudio = 3,
				EndAnalogAudio = 5,

			};


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

			[StructLayout(LayoutKind.Sequential)]
			public struct HLVR_DeviceInfo
			{
				public UInt32 Id;
				[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
				public char[] Name;
				public HLVR_DeviceConcept Concept;
				public HLVR_DeviceStatus Status;
			};


			[StructLayout(LayoutKind.Sequential)]
			public struct HLVR_DeviceIterator
			{
				public IntPtr _internal;
				public HLVR_DeviceInfo DeviceInfo;
			}


			public enum HLVR_EffectInfo_State
			{
				Unknown = 0,
				Playing,
				Paused,
				Idle
			}

			
			[StructLayout(LayoutKind.Sequential)]
			public struct HLVR_EffectInfo
			{
				public float Duration;
				public float Elapsed;
				HLVR_EffectInfo_State PlaybackState;
			};

			/* Agent functions */
			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_Create(HLVR_System** agent);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe void HLVR_System_Destroy(HLVR_System* agent);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static unsafe extern int HLVR_System_SuspendEffects(HLVR_System* agent);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_ResumeEffects(HLVR_System* agent);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_CancelEffects(HLVR_System* agent);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_GetRuntimeInfo(HLVR_System* agent, ref HLVR_RuntimeInfo infoPtr);


			/* Versioning */
			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern uint HLVR_Version_Get();

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_Version_IsCompatibleDLL();

			
			/* Device enumeration */

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_DeviceIterator_Init(ref HLVR_DeviceIterator iter);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_DeviceIterator_Next(ref HLVR_DeviceIterator iter, HLVR_System* system);

			/* Events */

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Event_Create(HLVR_Event** eventData, HLVR_EventType type);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe void HLVR_Event_Destroy(HLVR_Event* eventData);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Event_SetFloat(HLVR_Event* eventData, HLVR_EventKey key, float value);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Event_SetInt(HLVR_Event* eventData, HLVR_EventKey key, int value);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Event_SetUInt32(HLVR_Event* eventData, HLVR_EventKey key, UInt32 value);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Event_SetUInt32s(HLVR_Event* eventData, HLVR_EventKey key, [In, Out] UInt32[] values, uint length);

			/* Timelines */
			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Timeline_Create(HLVR_Timeline** timeline);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe void HLVR_Timeline_Destroy(HLVR_Timeline* timeline);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Timeline_AddEvent(HLVR_Timeline* timeline, double timeOffsetSeconds, HLVR_Event* data);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Timeline_Transmit(HLVR_Timeline* timeline, HLVR_System* agent, HLVR_Effect* effect);

			/* Playback */
			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Effect_Create(HLVR_Effect** effect);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe void HLVR_Effect_Destroy(HLVR_Effect* effect);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Effect_Play(HLVR_Effect* effect);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Effect_Pause(HLVR_Effect* effect);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Effect_Reset(HLVR_Effect* effect);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Effect_GetInfo(HLVR_Effect* effect, ref HLVR_EffectInfo info);


			/* Experimental APIs */
		
			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_Immediate_Set(HLVR_System* agent, [In, Out] UInt16[] intensities, [In, Out] UInt32[] areas, int length);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern  int HLVR_BodyView_Create(ref IntPtr body);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern  int HLVR_BodyView_Release(ref IntPtr body);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_BodyView_Poll(IntPtr body, HLVR_System* system);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_BodyView_GetNodeCount(IntPtr body, ref UInt32 outNodeCount);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern  int HLVR_BodyView_GetNodeType(IntPtr body, UInt32 nodeIndex, ref UInt32 outType);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_BodyView_GetNodeRegion(IntPtr body, UInt32 nodeIndex, ref UInt32 outRegion);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern int HLVR_BodyView_GetIntensity(IntPtr body, UInt32 nodeIndex, ref float outIntensity);

			/* Tracking */
			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_Tracking_GetOrientation(HLVR_System* system, uint region, ref HLVR_Quaternion outOrientation);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_Tracking_GetCompass(HLVR_System* system, uint region, ref HLVR_Vector3f outCompass);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_Tracking_GetGravity(HLVR_System* system, uint region, ref HLVR_Vector3f outGravity);


			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_Tracking_Enable(HLVR_System* ptr, uint device_id);

			[DllImport("Hardlight", CallingConvention = CallingConvention.Cdecl)]
			public static extern unsafe int HLVR_System_Tracking_Disable(HLVR_System* ptr, uint device_id);
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
		middle_sternum = 3 * Internal.Interop.SUBREGION_BLOCK_SIZE + 1,
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
