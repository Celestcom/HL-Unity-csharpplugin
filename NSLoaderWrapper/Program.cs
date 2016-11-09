using NullSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace NSLoaderWrapper
{
	class Program
	{
		static void Main(string[] args)
		{
		//	Stopwatch sw = new Stopwatch();

			NSLoader loader = new NSLoader(@"C:\Users\NullSpace Team\Documents\API_Environment\Assets\StreamingAssets");

			//	loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
			//loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
			//bool toggle = true;

			NSLoader.Sequence s = new NSLoader.Sequence("ns.basic.click_click_click");
			var forearms = Interop.AreaFlag.Left_All | Interop.AreaFlag.Chest_Right;
			var handle1 = s.CreateHandle(forearms);
			Console.WriteLine(handle1);
			while (true)
			{
				//Console.WriteLine("HI");
				handle1.Reset();
			Console.ReadLine();

				handle1.Play();


				//loader.SetTrackingEnabled(true);

				//	loader.SetTrackingEnabled(toggle);
				//	toggle = !toggle;
				//loader.PlaySequence("ns.basic.click_click_click", 0);
				//	int stat = loader.PollStatus();
				//	Console.WriteLine("Status: " + stat);
				//	Interop.Quaternion q = loader.PollTracking();
				//	Console.WriteLine("Quat: " + q.x + ", " + q.y + ", " + q.z + "," + q.w);

				//loader.PlayPattern("ns.body_jolt", 0);


			}

			//	loader.Dispose();
		}
	}
}
