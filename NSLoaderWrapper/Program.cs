using System;

using NullSpace.SDK;
using System.Collections.Generic;

namespace NSLoaderWrapper
{

	public class Program
	{

		

		public static int Main()
		{

			NSVR.NSVR_Plugin loader = new NSVR.NSVR_Plugin(@"C:\Users\NullSpace Team\Documents\NullSpace SDK 0.1.1\Assets\StreamingAssets\Haptics");


			var s = new HapticSequence();
			s.AddEffect(0.0, new HapticEffect("hum", 10.0));
			//s.Play(AreaFlag.All_Areas);d
			System.Threading.Thread.Sleep(500);
		
				loader.SetTrackingEnabled(true);

			
			//int counter = 0;
			//bool which = true;
			//while (true)
			//{
			//	counter++;
			//	if (counter > 100)
			//	{
			//		loader.SetTrackingEnabled(which);
			//		which = !which;
			//		counter = 0;
			//	}
			//	var trackingData = loader.PollTracking();
			//	Console.WriteLine(trackingData.Chest.x);
			//	System.Threading.Thread.Sleep(16);
			//}

			return 0;
		}

	}
}
