using System;
using System.Runtime.InteropServices;

namespace NullSpace.Loader
{
	public static class Interop
	{
		[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr TestClass_Create([MarshalAs(UnmanagedType.LPStr)] String s);

		[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern int TestClass_PlaySequence(IntPtr value, [MarshalAs(UnmanagedType.LPStr)] String s, int location);


		[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern int TestClass_PlayPattern(IntPtr value, [MarshalAs(UnmanagedType.LPStr)] String s, int side);

		[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern int TestClass_PlayExperience(IntPtr value, [MarshalAs(UnmanagedType.LPStr)] String s, int side);

		[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern int TestClass_PlayEffect(IntPtr value, int effect, int location, float duration, float time, uint priority);


		[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern int TestClass_PollStatus(IntPtr value);

		[StructLayout(LayoutKind.Sequential,Pack=1)]
		public struct Quaternion
		{
			public float w;
			public float x;
			public float y;
			public float z;
		}

		[StructLayout(LayoutKind.Sequential, Pack =1)]
		public struct TrackingUpdate
		{
			public Quaternion chest;
			public Quaternion left_upper_arm;
			public Quaternion left_forearm;
			public Quaternion right_upper_arm;
			public Quaternion right_forearm;
		}
		[DllImport("NSLoader", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern int TestClass_PollTracking(IntPtr value,  ref TrackingUpdate q);


		[DllImport("NSLoader", CallingConvention = CallingConvention.StdCall)]
		public static extern void TestClass_Delete(IntPtr value);
	}
}
