using System;

using NullSpace.SDK;
using System.Collections.Generic;

namespace NSLoaderWrapper
{

	public class Program
	{

		

		public static int Main()
		{

			NSVR.NSVR_Plugin loader = new NSVR.NSVR_Plugin(@"C:\Users\NullSpace Team\Documents\NullSpace SDK 0.1.1\Assets\StreamingAssets\Haptics");

			
			while (true)
			{

				Console.WriteLine(loader.PollStatus());
				System.Threading.Thread.Sleep(400);
			}
		}

	}
}
