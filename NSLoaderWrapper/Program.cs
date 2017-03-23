using System;

using NullSpace.SDK;
using System.Collections.Generic;
using NullSpace.SDK.FileUtilities;
namespace NSLoaderWrapper
{

	public class Program
	{

		

		public static int Main()
		{

			NSVR.NSVR_Plugin loader = new NSVR.NSVR_Plugin();


			//var s = new HapticSequence();
			//s.AddEffect(0.0, new HapticEffect(Effect.Click));
			//s.AddEffect(0.5, new HapticEffect(Effect.Click));

			//s.AddEffect(0.7, new HapticEffect(Effect.Click));

			//s.AddEffect(0.9, new HapticEffect(Effect.Click));

			////s.CreateHandle(AreaFlag.All_Areas).Play();
			////s.Play(AreaFlag.All_Areas);d

			//var p = new HapticPattern();
			//p.AddSequence(0.0, AreaFlag.All_Areas, s);
			//p.AddSequence(2.0, AreaFlag.All_Areas, s);

			//p.CreateHandle().Play();
			//System.Threading.Thread.Sleep(3300);

			//	loader.SetTrackingEnabled(true);



			AssetTool a = new AssetTool();
			a.SetRootHapticsFolder(@"C:\Users\NullSpace Team\Documents\NullSpace SDK 0.1.1\Assets\StreamingAssets\Haptics");
			

			var hdf = a.GetHapticDefinitionFile(@"C:\Users\NullSpace Team\Documents\NullSpace SDK 0.1.1\Assets\StreamingAssets\Haptics\NS Demos\patterns\beating_heart.pattern");

			HapticSequence s = new HapticSequence();		
			var pat = CodeHapticFactory.CreatePattern(hdf.rootEffect.name, hdf);
		//	Console.WriteLine(pat.ToString());
		

			AreaFlag myArea = AreaFlag.None;
			myArea = myArea.AddArea(AreaFlag.Chest_Left);

		

			Console.WriteLine(myArea.ToStringIncludedAreas());
			
			Console.ReadLine();





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
			//}

			return 0;
		}

	}
}
