using System;

using NullSpace.SDK;
using NullSpace.SDK.Internal;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
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

			NSVR.NSVR_Plugin loader = new NSVR.NSVR_Plugin(@"C:\Users\NullSpace Team\Documents\NullSpace SDK 0.1.1\Assets\StreamingAssets\Haptics");


			EventList list = new EventList();
			list.AddEvent(new BasicHapticEvent(0f, 1f, 0f,(uint) AreaFlag.Back_Left, "click"));
			list.AddEvent(new BasicHapticEvent(0.5f, 1f, 0f, (uint)AreaFlag.Back_Right, "click"));
			list.AddEvent(new BasicHapticEvent(0.8f, 1f, 0f, (uint)AreaFlag.All_Areas, "click"));

			list.AddEvent(new BasicHapticEvent(1.8f, 1f, 0f, (uint)AreaFlag.All_Areas, "click"));

			list.AddEvent(new BasicHapticEvent(1.9f, 1f, 0f, (uint)AreaFlag.All_Areas, "click"));

			list.AddEvent(new BasicHapticEvent(3f, 1f, 1.0f, 12, "click"));

			list.AddEvent(new BasicHapticEvent(4.1f, 1f, 0f, 12, "hum"));

			list.AddEvent(new BasicHapticEvent(5f, 1f, 0f, 12, "hum"));

		//	var bytes = list.Generate();

			var a = new CodeSequence();
			a.AddEffect(0.0, new CodeEffect("hum", 0.3, 1.0));
			a.AddEffect(1.5, new CodeEffect("buzz"));
			a.CreateHandle(AreaFlag.All_Areas).Play();
		//	CodeHapticEncoder encoder = new CodeHapticEncoder();
		//	encoder.Flatten(a);
		//	var bytes = encoder.Encode();
		//	uint handle = Interop.NSVR_TransmitEvents(NSVR.NSVR_Plugin.Ptr, bytes, (UInt32) bytes.Length);
		//	Interop.NSVR_HandleCommand(NSVR.NSVR_Plugin.Ptr, handle, 0);
			while (true)
			{

			}
			return 0;
		}

	}
}
