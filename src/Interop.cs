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
				char[] ProductName;
				short FirmwareMajor;
				short FirmwareMinor;
				//tracking capabilities?
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
			public static extern unsafe int NSVR_Timeline_Create(ref IntPtr eventListPtr, NSVR_System* systemPtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern void NSVR_Timeline_Release(ref IntPtr listPtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_Timeline_AddEvent(IntPtr list, IntPtr eventPtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_Timeline_Transmit(IntPtr timeline, IntPtr handlePr);

			/* Playback */
			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_PlaybackHandle_Create(ref IntPtr handlePtr);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_PlaybackHandle_Command(IntPtr handlePtr, NSVR_PlaybackCommand command);

			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern void NSVR_PlaybackHandle_Release(ref IntPtr handlePtr);

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

		/// <summary>
		/// Return a debug string containing all areas in this AreaFlag
		/// </summary>
		/// <param name="lhs"></param>
		/// <returns></returns>
		public static string ToStringIncludedAreas(this AreaFlag lhs)
		{
			if (lhs == AreaFlag.None)
			{
				return "None";
			}

			List<string> result = new List<string>();
			foreach (AreaFlag a in Enum.GetValues(typeof(AreaFlag)))
			{
				var areaString = a.ToString();
				if (a != AreaFlag.None && lhs.ContainsArea(a) && !areaString.Contains("Both") && !areaString.Contains("All"))
				{
					result.Add(a.ToString());
				}
			}
			return string.Join("|", result.ToArray());

		}
	}

	
}
