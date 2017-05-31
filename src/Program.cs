using System;

using NullSpace.SDK;
using NullSpace.SDK.Internal;
using System.ServiceProcess;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace NSLoaderWrapper
{

	public unsafe class Program
	{

		static unsafe NSVR_System* systemPtr;
		static bool running = true;
		public static void Monitor(object data)
		{
			UInt16[] strengths = new UInt16[16];
			UInt32[] areas = new UInt32[16];
			UInt32[] families = new UInt32[16];

			while (running)
			{

				uint resultCount = 0;
				Interop.NSVR_Immediate_Sample(systemPtr, strengths, areas, families, 16, ref resultCount);
				
			//	Console.WriteLine("Count: " + resultCount);

				for (int i = 0; i < resultCount; i++)
				{
					Console.WriteLine(string.Format("area {0} : str {1}, fam {2}", areas[i], strengths[i], families[i]));
				}

			//	System.Threading.Thread.Sleep(100);
			}
		}
		public static int Main()
		{

		

			fixed (NSVR_System** system_ptr = &systemPtr)
			{
				if (Interop.NSVR_FAILURE(Interop.NSVR_System_Create(system_ptr))) {
					Console.WriteLine("Failed to create nsvr system");
				}
			}

			Thread newThread = new Thread(Program.Monitor);
			//newThread.Start();
		

			IntPtr handlePtr = IntPtr.Zero;
			//	Interop.NSVR_PlaybackHandle_Create(ref handlePtr);
			////	while (true)
			////	{



			//		IntPtr timelinePtr = IntPtr.Zero;
			//		Interop.NSVR_Timeline_Create(ref timelinePtr, systemPtr);

			//		IntPtr eventPtr = IntPtr.Zero;
			//		Interop.NSVR_Event_Create(ref eventPtr, Interop.NSVR_EventType.Basic_Haptic_Event);

			//		Interop.NSVR_Event_SetInteger(eventPtr, "area", (int) AreaFlag.Chest_Left);
			//		Interop.NSVR_Event_SetInteger(eventPtr, "effect",(int)Effect.Click);
			//		Interop.NSVR_Event_SetFloat(eventPtr, "duration", 0.4f);
			//		Interop.NSVR_Event_SetFloat(eventPtr, "strength", 0.5f);

			//		Interop.NSVR_Timeline_AddEvent(timelinePtr, eventPtr);
			//		Interop.NSVR_Event_Release(ref eventPtr);

			//		Interop.NSVR_Timeline_Transmit(timelinePtr, handlePtr);
			//		Interop.NSVR_Timeline_Release(ref timelinePtr);




			//		Interop.NSVR_PlaybackHandle_Command(handlePtr, Interop.NSVR_PlaybackCommand.Play);

			//		System.Threading.Thread.Sleep(500);


			//	}
			bool dir =true;
			ushort strength = 1;
			while (true)
			{
				Console.WriteLine(strength);
				ushort[] strengths = new ushort[16];
				for (int i = 0; i < 16; i++) { strengths[i] = strength; }

				var areaslist = new List<AreaFlag>{ AreaFlag.Chest_Left, AreaFlag.Chest_Right, AreaFlag.Upper_Ab_Left, AreaFlag.Upper_Ab_Both };
				uint[] areas = new uint[4];

				for (int i = 0; i < 4; i++) { areas[i] = ((uint)areaslist[i]); }
				Interop.NSVR_Immediate_Set(systemPtr, strengths, areas, 4);
				if (dir)
				{
					strength++;
				} else
				{
					strength--;
				}
				if (strength > 254 || strength < 1)
				{
					dir = !dir;
				}
				Thread.Sleep(100);
			}
			return 0;
		}

	}
}
