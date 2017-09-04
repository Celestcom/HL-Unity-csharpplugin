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


		public static int Main()
		{

			Console.WriteLine("Hit");
			NSVR.NSVR_Plugin plugin = new NSVR.NSVR_Plugin();

			Console.ReadLine();



			HapticSequence s = new HapticSequence();
			s.AddEffect(0.0, new HapticEffect(Effect.Click));
			s.AddEffect(1.0, new HapticEffect(Effect.Click));

			var handle = s.CreateHandle(AreaFlag.Chest_Left);
			handle.Play();
			Console.WriteLine("Playing");

			Console.ReadLine();


		
			return 0;
		}




	}
}
