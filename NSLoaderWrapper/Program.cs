using System;

using NullSpace.SDK;
using NullSpace.SDK.Internal;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;

namespace NSLoaderWrapper
{

	public class Program
	{

		static void WriteEffectToFile(IGeneratable effect, string filename)
		{
		
		
			try
			{
				var dataBytes = EncodingUtils.Encode(effect, 0xDEADBEEF);
				// Open file for reading
				System.IO.FileStream _FileStream =
				   new System.IO.FileStream(filename, System.IO.FileMode.Create,
											System.IO.FileAccess.Write);
				// Writes a block of bytes to this stream using data from
				// a byte array.
				_FileStream.Write(dataBytes, 0, dataBytes.Length);

				// close file stream
				_FileStream.Close();

				
			}
			catch (Exception _Exception)
			{
				// Error
				Console.WriteLine("Exception caught in process: {0}",
								  _Exception.ToString());
			}

		
		
	}

		public static int Main()
		{
			//	Stopwatch sw = new Stopwatch();


			//	loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
			//loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
			//bool toggle = true;
			NSVR.NSVR_Plugin loader = new NSVR.NSVR_Plugin(@"C:\Users\NullSpace Team\Documents\NullSpace SDK 0.1.1\Assets\StreamingAssets\Haptics");


			CodeSequence buzz = new CodeSequence();
			buzz.AddEffect(1.38 - 1.38, new CodeEffect("sharp_click", 0.0, 1.0));
			buzz.AddEffect(1.58 - 1.38, new CodeEffect("sharp_click", 0.0, 1.0));
			buzz.AddEffect(1.9 - 1.38, new CodeEffect("sharp_click", 0.0, 1.0));
			buzz.AddEffect(2.24 - 1.38, new CodeEffect("sharp_click", 0.0, 1.0));
			buzz.AddEffect(2.44 - 1.38, new CodeEffect("sharp_click", 0.0, 1.0));
			buzz.AddEffect(2.79 - 1.38, new CodeEffect("click", 0.0, 1.0));
			buzz.AddEffect(3.5 - 1.38, new CodeEffect("buzz", 0.0, 1.0));

			CodeSequence buzz2 = new CodeSequence();
			buzz2.AddEffect(0, new CodeEffect("buzz", 0.5));

			WriteEffectToFile(buzz, "mario.haptic");
			WriteEffectToFile(buzz2, "buzz.haptic");


			//System.Threading.Thread.Sleep(500);


			var left_areas = new List<AreaFlag>() {
				AreaFlag.Forearm_Left,
				AreaFlag.Upper_Arm_Left,
				AreaFlag.Shoulder_Left,
				AreaFlag.Back_Left,
				AreaFlag.Chest_Left,
				AreaFlag.Upper_Ab_Left,
				AreaFlag.Mid_Ab_Left,
				AreaFlag.Lower_Ab_Left,



			};
			var right_areas = new List<AreaFlag>() {
				AreaFlag.Forearm_Right,
				AreaFlag.Upper_Arm_Right,
				AreaFlag.Shoulder_Right,
				AreaFlag.Back_Right,
				AreaFlag.Chest_Right,
				AreaFlag.Upper_Ab_Right,
				AreaFlag.Mid_Ab_Right,
				AreaFlag.Lower_Ab_Right,



			};

			CodePattern p = new CodePattern();
			double time = 0;
			foreach (var area in left_areas)
			{
				p.AddSequence(time, area, buzz2);
				time += 0.7;
			}
			time += 0.7;
			foreach (var area in right_areas)
			{
				p.AddSequence(time, area, buzz2);
				time += 0.7;
			}
			p.Play();
			WriteEffectToFile(p, "test_all.haptic");

			foreach (var area in left_areas)
			{
				CodePattern p2 = new CodePattern();
				var seq = new CodeSequence();
				seq.AddEffect(0, new CodeEffect("buzz", 0.5));
				p2.AddSequence(0, area, seq);
				WriteEffectToFile(p2, area.ToString() + ".haptic");
			}
			foreach (var area in right_areas)
			{
				CodePattern p2 = new CodePattern();
				var seq = new CodeSequence();
				seq.AddEffect(0, new CodeEffect("buzz", 0.5));
				p2.AddSequence(0, area, seq);
				WriteEffectToFile(p2, area.ToString() + ".haptic");
			}
			//p.Play();
			Console.ReadLine();
			//some time later (< effect length), noop


			//some time later (>= effect length),
			//performs the equivalent of hh.Reset().Play()
			//hh.Play();


			//	Console.ReadLine();
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
