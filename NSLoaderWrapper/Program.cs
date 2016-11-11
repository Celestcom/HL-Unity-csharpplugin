using System;

using NullSpace.SDK;
using static NullSpace.SDK.NSVR_Plugin;

namespace NSLoaderWrapper
{
	class Program
	{
		static void Main(string[] args)
		{
		//	Stopwatch sw = new Stopwatch();

		 var loader = new NSVR_Plugin(@"C:\Users\NullSpace Team\Documents\API_Environment\Assets\StreamingAssets");

			//	loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
			//loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
			//bool toggle = true;
		
			Sequence s = new Sequence("ns.basic.click_click_click");
			Pattern pat = new Pattern("ns.basic.test");
			var forearms = AreaFlag.All_Areas;
			var handle1 = s.CreateHandle(forearms);
			var handle2 = pat.CreateHandle();
			
			while (true)
			{
			//	//handle1.Reset();
				//handle1.Play();

				Console.ReadLine();
				handle1.Reset();
				handle1.Play();
				//Console.WriteLine("HI");
				//Console.ReadLine();
			//	handle1.Play();


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
