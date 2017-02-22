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

			NSVR.NSVR_Plugin loader = new NSVR.NSVR_Plugin(@"C:\Users\NullSpace Team\Documents\NullSpace SDK 0.1.1\Assets\StreamingAssets\Haptics");


			EventList list = new EventList();
			list.AddEvent(new BasicHapticEvent(0f, 1f, 1f, 12, "hum"));
			list.AddEvent(new BasicHapticEvent(0f, 1f, 1f, 12, "hum"));

			list.AddEvent(new BasicHapticEvent(0f, 1f, 1f, 12, "hum"));

			list.AddEvent(new BasicHapticEvent(0f, 1f, 1f, 12, "hum"));

			var bytes = list.Generate();
			Interop.NSVR_TransmitEvents(NSVR.NSVR_Plugin.Ptr, bytes, (UInt32) bytes.Length);
		
			return 0;
		}

	}
}
