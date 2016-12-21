using System;

using NullSpace.SDK;
using NullSpace.SDK.Internal;
using System.Runtime.InteropServices;
using System.Threading;

namespace NSLoaderWrapper
{
	
	public class Program
	{
	
		
		public class Alpha
		{

			// This method that will be called when the thread is started
			public void Beta()
			{
				while (true)
				{
					Console.WriteLine("Alpha.Beta is running in its own thread.");
				}
			}
		};
		public static int Main()
		{
			//	Stopwatch sw = new Stopwatch();


			//	loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
			//loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
			//bool toggle = true;
				NSVR.NSVR_Plugin loader = new NSVR.NSVR_Plugin(@"C:\Users\NullSpace Team\Documents\NullSpace SDK 0.1.1\Assets\StreamingAssets\Haptics");
			//	Sequence s = new Sequence("ns.basic.click_click_click");
			//s.CreateHandle(AreaFlag.All_Areas).Play();
			DefaultTimeProvider p = new DefaultTimeProvider();
			RandomGenerator g = new RandomGenerator();

			g.GenerateNext(new CodeEffect(0f, "click", 0f, 1f, AreaFlag.None)).Play();



			CutaneousRabbit rabbit = new CutaneousRabbit(AreaFlag.Forearm_Left, AreaFlag.Upper_Arm_Left);
			rabbit.SetParam("strength", 1.0);
			rabbit.SetParam("frequency", 2.4);
			GenericStrengthShader shader = new GenericStrengthShader();
			ShaderProgram program = new ShaderProgram(rabbit, shader);
			program.UseTimeProvider(p);
			program.Execute();
			

			Console.ReadLine();

			program.Destroy();
		
			//handle1.Play();
			//	Console.ReadLine();
			//	var ha = seq.CreateHandle(AreaFlag.Chest_Left).Play();
			//	Console.ReadLine();
			//ha.Pause();

			//	h.Play();
			//	h.Reset();				

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


			//	}
			//	loader.Dispose();
			//}
			return 0;
		}

	}
}
