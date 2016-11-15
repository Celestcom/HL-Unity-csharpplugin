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
			public static extern IntPtr NSVR_Create([MarshalAs(UnmanagedType.LPStr)] string s);





			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_PollStatus(IntPtr value);

			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern void NSVR_SetTrackingEnabled(IntPtr value, bool wantTracking);

			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern uint NSVR_GenHandle(IntPtr value);

			#region Sequence
			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			[return: MarshalAs(UnmanagedType.I1)]

			public static extern bool NSVR_LoadSequence(IntPtr value, String s);

			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			[return: MarshalAs(UnmanagedType.I1)]

			public static extern bool NSVR_CreateSequence(IntPtr value, uint handle, [MarshalAs(UnmanagedType.LPStr)] string s, uint location);
			#endregion

			#region Pattern
			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			[return: MarshalAs(UnmanagedType.I1)]
			public static extern bool NSVR_LoadPattern(IntPtr value, [MarshalAs(UnmanagedType.LPStr)] string s);
			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			[return: MarshalAs(UnmanagedType.I1)]
			public static extern bool NSVR_CreatePattern(IntPtr value, uint handle, [MarshalAs(UnmanagedType.LPStr)] string s);
			#endregion

			#region Experience
			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			[return: MarshalAs(UnmanagedType.I1)]
			public static extern bool NSVR_LoadExperience(IntPtr value, [MarshalAs(UnmanagedType.LPStr)]string s);
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

			[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
			public static extern int NSVR_PollTracking(IntPtr value, ref TrackingUpdate q);


			[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
			public static extern void NSVR_Delete(IntPtr value);




		}
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

		Left_All = 0x000000FF,
		Right_All = 0x00FF0000,
		All_Areas = Left_All | Right_All,
	};


	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Quaternion
	{
		public float w;
		public float x;
		public float y;
		public float z;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TrackingUpdate
	{
		public Quaternion chest;
		public Quaternion left_upper_arm;
		public Quaternion left_forearm;
		public Quaternion right_upper_arm;
		public Quaternion right_forearm;
	}
	
}
