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


			NSVR.NSVR_Plugin plugin = new NSVR.NSVR_Plugin();

			EventList e = new EventList();
			e.AddEvent(new BasicHapticEvent(0, 1, 0, (uint)AreaFlag.Back_Both, Effect.Bump));
		
			IntPtr handle = IntPtr.Zero;

			Interop.NSVR_PlaybackHandle_Create(ref handle);
			Console.ReadLine();

			e.Transmit(handle);

			Interop.NSVR_EffectInfo info = new Interop.NSVR_EffectInfo();
			Interop.NSVR_PlaybackHandle_GetInfo(handle, ref info);
			Console.WriteLine("Effect duration:" + info.Duration);
			Console.ReadLine();
			//if (handle != IntPtr.Zero)
			//{
			//	Console.WriteLine("Something wrong");
			//}
		//	HapticSequence s = new HapticSequence();
		//	s.AddEffect(0, new HapticEffect(Effect.Bump));

			Console.ReadLine();

		//	var handle = s.CreateHandle(AreaFlag.All_Areas);

			//handle.Play();
		//	int x = 3;

			Console.ReadLine();


			
			//while (true)
			//{
			//	Interop.NSVR_HandleInfo info = new Interop.NSVR_HandleInfo();
			//	Interop.NSVR_PlaybackHandle_GetInfo(handlePtr, ref info);
			//	Console.WriteLine(info.Elapsed);
			//}



			//		Interop.NSVR_PlaybackHandle_Command(handlePtr, Interop.NSVR_PlaybackCommand.Play);

			//		System.Threading.Thread.Sleep(500);


			//	}
			//	bool dir =true;
			//	ushort strength = 1;
			//	while (true)
			//	{
			//		Console.WriteLine(strength);
			//		ushort[] strengths = new ushort[16];
			//		for (int i = 0; i < 16; i++) { strengths[i] = strength; }

			//		var areaslist = new List<AreaFlag>{ AreaFlag.Chest_Left, AreaFlag.Chest_Right, AreaFlag.Upper_Ab_Left, AreaFlag.Upper_Ab_Both };
			//		uint[] areas = new uint[4];

			//		for (int i = 0; i < 4; i++) { areas[i] = ((uint)areaslist[i]); }
			//		Interop.NSVR_Immediate_Set(systemPtr, strengths, areas, 4);
			//		if (dir)
			//		{
			//			strength++;
			//		} else
			//		{
			//			strength--;
			//		}
			//		if (strength > 254 || strength < 1)
			//		{
			//			dir = !dir;
			//		}
			//		Thread.Sleep(100);
			//	}
			//	return 0;
			//}
			return 0;
		}




	}
}
