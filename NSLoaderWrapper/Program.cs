using System;

using NullSpace.SDK;
using System.Collections.Generic;

namespace NSLoaderWrapper
{

	public class Program
	{

		static void WriteEffectToFile(string effect, string filename)
		{
		
		
			try
			{
				//var dataBytes = EncodingUtils.Encode(effect, 0xDEADBEEF);
				// Open file for reading
				System.IO.FileStream _FileStream =
				   new System.IO.FileStream(filename, System.IO.FileMode.Create,
											System.IO.FileAccess.Write);
				// Writes a block of bytes to this stream using data from
				// a byte array.
			///	_FileStream.Write(dataBytes, 0, dataBytes.Length);

				// close file stream
			//	_FileStream.Close();

				
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

			NSVR.NSVR_Plugin loader = new NSVR.NSVR_Plugin(@"C:\Users\NullSpace Team\Documents\NullSpace SDK 0.1.1\Assets\StreamingAssets\Haptics");




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
			CodeSequence buzz2 = new CodeSequence();
			buzz2.AddEffect(0.0, new CodeEffect("hum", 0.1, 0.1));
			//buzz2.AddEffect(3.2, new CodeEffect("click"));


			CodePattern p = new CodePattern();
			double time = 0;
			foreach (var area in left_areas)
			{
				p.AddSequence(time, area, buzz2);
				time += 0.05;
			}
		//	time += 0.7;
			foreach (var area in right_areas)
			{
				p.AddSequence(time, area, buzz2);
				time += 0.05;
			}

			//	CodeHapticEncoder encoder = new CodeHapticEncoder();
			//	encoder.Flatten(a);
			//	var bytes = encoder.Encode();
			//	uint handle = Interop.NSVR_TransmitEvents(NSVR.NSVR_Plugin.Ptr, bytes, (UInt32) bytes.Length);
			//	Interop.NSVR_HandleCommand(NSVR.NSVR_Plugin.Ptr, handle, 0);
			//var hdf = LoadingUtils.LoadAsset(@"C:\Users\NullSpace Team\Documents\Visual Studio 2015\Projects//\NSLoaderWrapper\Debug\whatever.hdf");
			//AssetTool a = new AssetTool();
			
			//var packages = a.TryGetPackageInfo();
		//	var hdf = a.GetHapticDefinitionFile(@"C:\Users\NullSpace Team\Documents\NullSpace SDK 0.1.1\Assets\StreamingAssets\Haptics\NS Demos\patterns\beating_heart_very_fast.pattern");
			//CodePattern pat= FileToCodeHaptic.CreatePattern(hdf.root_effect.name, hdf);

			CodeSequence s = new CodeSequence();
			s.AddEffect(0.0, new CodeEffect("click"));
			CodePattern t = new CodePattern();
			t.AddSequence(0.0, AreaFlag.All_Areas, s);
			while (true)
			{
				
				t.Play();
				System.Threading.Thread.Sleep(1000);

			}
			return 0;
		}

	}
}
