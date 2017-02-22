using System;
using System.Runtime.InteropServices;

namespace NullSpace.SDK
{
	namespace Internal
	{

		internal static class Interop
		{
			public delegate void CommandWithHandle(uint handle);

			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern IntPtr NSVR_Create();





			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_PollStatus(IntPtr value);

			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern void NSVR_SetTrackingEnabled(IntPtr value, bool wantTracking);

			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern uint NSVR_GenHandle(IntPtr value);

			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			[return: MarshalAs(UnmanagedType.I1)]

			public static extern bool NSVR_Load(IntPtr value, String s, int fileType);

			#region Sequence
			

			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			[return: MarshalAs(UnmanagedType.I1)]

			public static extern bool NSVR_CreateSequence(IntPtr value, uint handle, [MarshalAs(UnmanagedType.LPStr)] string s, uint location);

		
			#endregion

			#region Pattern
			
			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			[return: MarshalAs(UnmanagedType.I1)]
			public static extern bool NSVR_CreatePattern(IntPtr value, uint handle, [MarshalAs(UnmanagedType.LPStr)] string s);
			


			#endregion

			#region Experience
			
			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			[return: MarshalAs(UnmanagedType.I1)]
			public static extern bool NSVR_CreateExperience(IntPtr value, uint handle, [MarshalAs(UnmanagedType.LPStr)]string s);
			#endregion


			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern void NSVR_HandleCommand(IntPtr value, uint handle, short command);
			public enum Command
			{
				PLAY = 0, PAUSE, RESET, RELEASE
			}
			public enum EngineCommand
			{
				PLAY_ALL=1, PAUSE_ALL, CLEAR_ALL, ENABLE_TRACKING, DISABLE_TRACKING
			}
			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern bool NSVR_EngineCommand(IntPtr value, short command);
			
			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_PollTracking(IntPtr value, ref InteropTrackingUpdate q);


			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern void NSVR_Delete(IntPtr value);

			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern IntPtr NSVR_GetError(IntPtr value);

			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern void NSVR_FreeString(IntPtr value);


			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			[return: MarshalAs(UnmanagedType.I1)]

			public static extern bool NSVR_InitializeFromFilesystem(IntPtr value, [MarshalAs(UnmanagedType.LPStr)]string path);



			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern void NSVR_CreateHaptic(IntPtr value, uint handle, byte[] data, uint size);


			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern UInt32 NSVR_TransmitEvents(IntPtr value, byte[] data, uint size);
		}
	}
	
	public enum SuitStatus
	{
		Disconnected = 0,
		Connected = 2
	}
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

	public enum Imu
	{
		Chest = 0,
		Right_Upper_Arm = 1,
		Left_Upper_Arm = 2,
		Right_Forearm = 3,
		Left_Forearm = 4
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct Quaternion
	{
		public float w;
		public float x;
		public float y;
		public float z;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct InteropTrackingUpdate
	{
		public Quaternion chest;
		public Quaternion left_upper_arm;
		public Quaternion left_forearm;
		public Quaternion right_upper_arm;
		public Quaternion right_forearm;
	}
	
}
