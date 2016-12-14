using System;

using NullSpace.SDK;
using NullSpace.SDK.Internal;
using System.Runtime.InteropServices;

namespace NSLoaderWrapper
{
	class Program
	{
	
		static void test()
		{
			Sequence s = new Sequence("ns.basic.click_click_click");
			var handle1 = s.CreateHandle(AreaFlag.All_Areas);
			var handle2 = s.CreateHandle(AreaFlag.All_Areas);

			var handle3 = s.CreateHandle(AreaFlag.All_Areas);

			var handle4 = s.CreateHandle(AreaFlag.All_Areas);

			//s.CreateHandle(AreaFlag.All_Areas).Dispose();

			//s.CreateHandle(AreaFlag.All_Areas).Dispose();
		}
		static void Main(string[] args)
		{
			//	Stopwatch sw = new Stopwatch();

			
			//	loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
			//loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
			//bool toggle = true;
			NSVR.NSVR_Plugin loader = new NSVR.NSVR_Plugin(@"C:\Users\NullSpace Team\Documents\NullSpace SDK 0.1.1\Assets\StreamingAssets");
			//	Sequence s = new Sequence("ns.basic.click_click_click");
			//s.CreateHandle(AreaFlag.All_Areas).Play();
			Pattern p = new Pattern("ns.demos.intense");
		//	p.CreateHandle().Play();
			//	Sequence s = new Sequence("ns.demos.click_click_click");
			//s.CreateHandle(AreaFlag.All_Areas).Play();
			//loader.Dispose();
			//loader.Dispose();
			//loader.Dispose();
			//loader.ClearAll();
			//Console.ReadLine();
			//h.Pause();
			//Console.ReadLine();
			//h.Play();
			//Experience e = new Experience("ns.basic.test");
			//	Pattern e= new Pattern("ns.full_body_jolt");
			//	Sequence s = new Sequence("ns.click");
			//	var a = new Experience("ns.basic.test");
			//var h = e.CreateHandle();
			//CodeSequence seq = new CodeSequence("seq");
			//seq.Add(new CodeSequenceItem(0.0f, "hum", 1.0f, 3.0f));
			//seq.Add(new CodeSequenceItem(4.0f, "hum", 0.2f, 1.0f));
			//var a = NSVR.HapticRef<Sequence>("ns.click");
			//var codeSeq = new CodeSequence("myseq");
			//codeSeq.Add(new CodeSequenceItem(0.0f, "hum"));
			//codeSeq.CreateHandle(AreaFlag.All_Areas);

			//	var b = new CodePattern("test");
			//	b.Add(new PatternItem(0.0f, a, AreaFlag.All_Areas));
			//	b.Add(new PatternItem(0.0f, a, AreaFlag.All_Areas));
			//	b.Add(new PatternItem(0.0f, codeSeq, AreaFlag.Left_All));
			//	b.Add(new CodePatternItem(0.5f, codeSeq, AreaFlag.All_Areas));
			//b.Add(new CodePatternItem(0.5f, codeSeq, AreaFlag.All_Areas));

			//b.CreateHandle();
			while (true)
			{
				Console.WriteLine(loader.PollStatus());
			}
				//	//handle1.Reset();
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
		}

	}
}
