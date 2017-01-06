using System;

using NullSpace.SDK;
using NullSpace.SDK.Internal;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace NSLoaderWrapper
{
	public static class Extensions
	{
		public static byte[] ReadAllBytes(this BinaryReader reader)
		{
			const int bufferSize = 256;
			using (var ms = new MemoryStream())
			{
				byte[] buffer = new byte[bufferSize];
				int count;
				while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
					ms.Write(buffer, 0, count);
				return ms.ToArray();
			}

		}
	}
	public class Program
	{

	

		public static int Main()
		{
			//	Stopwatch sw = new Stopwatch();


			//	loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
			//loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
			//bool toggle = true;
				NSVR.NSVR_Plugin loader = new NSVR.NSVR_Plugin(@"C:\Users\NullSpace Team\Documents\NullSpace SDK 0.1.1\Assets\StreamingAssets\Haptics");

			CodePattern startupRoutine = new CodePattern();
			CodeSequence effect = new CodeSequence();
			effect.AddChild(new CodeEffect("click", 0f));
			startupRoutine.AddChild(0.0f, AreaFlag.Forearm_Left, effect);
			startupRoutine.AddChild(0.1f, AreaFlag.Upper_Arm_Left, effect);
			startupRoutine.AddChild(0.2f, AreaFlag.Shoulder_Left, effect);
			startupRoutine.AddChild(0.3f, AreaFlag.Back_Left, effect);
			startupRoutine.AddChild(0.4f, AreaFlag.Chest_Left, effect);
			startupRoutine.AddChild(0.5f, AreaFlag.Upper_Ab_Left, effect);
			startupRoutine.AddChild(0.6f, AreaFlag.Mid_Ab_Left, effect);
			startupRoutine.AddChild(0.7f, AreaFlag.Lower_Ab_Left, effect);
			startupRoutine.AddChild(0.8f, AreaFlag.Forearm_Right, effect);
			startupRoutine.AddChild(0.9f, AreaFlag.Upper_Arm_Right, effect);
			startupRoutine.AddChild(1.0f, AreaFlag.Shoulder_Right, effect);
			startupRoutine.AddChild(1.1f, AreaFlag.Back_Right, effect);
			startupRoutine.AddChild(1.2f, AreaFlag.Chest_Right, effect);
			startupRoutine.AddChild(1.3f, AreaFlag.Upper_Ab_Right, effect);
			startupRoutine.AddChild(1.4f, AreaFlag.Mid_Ab_Right, effect);
			startupRoutine.AddChild(1.5f, AreaFlag.Lower_Ab_Right, effect);


			//Picking a handle near uint max
			using (BinaryWriter writer = new BinaryWriter(File.Open("test.bin", FileMode.Create))) {
				var bytes = EncodingUtils.Encode(startupRoutine, 4294967290);

				writer.Write(bytes, 0, bytes.Length);
			}
			

			using (BinaryReader reader = new BinaryReader(File.Open("test.bin", FileMode.Open)))
			{
				byte[] bytes = reader.ReadAllBytes();
				Interop.NSVR_CreateHaptic(NSVR.NSVR_Plugin.Ptr, 4294967290, bytes, (uint)bytes.Length);
				Interop.NSVR_HandleCommand(NSVR.NSVR_Plugin.Ptr, 4294967290, 0);
			}

			Console.ReadLine();
			//	Sequence s = new Sequence("ns.basic.click_click_click");
			//s.CreateHandle(AreaFlag.All_Areas).Play();
			//DefaultTimeProvider p = new DefaultTimeProvider();
			//RandomGenerator g = new RandomGenerator();

			//	g.GenerateNext(new CodeEffect(0f, "click", 0f, 1f, AreaFlag.None)).Play();

			//	CodePattern pa = new CodePattern();
		
/*
			CodeSequence seq = new CodeSequence();
			seq.AddChild(new CodeEffect("click", 0f));
			
			var randomGen = new RandomGenerator();

			for (int i = 0; i < 100; i++)
			{
				randomGen.GenerateNext(seq).Play();
				System.Threading.Thread.Sleep(50);
			}
			*/
			
		/*	CutaneousRabbit rabbit = new CutaneousRabbit(AreaFlag.Forearm_Left, AreaFlag.Upper_Arm_Left);
			rabbit.SetParam("strength", 1.0);
			rabbit.SetParam("frequency", 2.4);
			GenericStrengthShader shader = new GenericStrengthShader();
			ShaderProgram program = new ShaderProgram(rabbit, shader);
			program.UseTimeProvider(p);
			program.Execute();
			

			Console.ReadLine();

			program.Destroy();
		*/
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
