using NullSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace NSLoaderWrapper
{
	class Program
	{
		static void Main(string[] args)
		{
			Stopwatch sw = new Stopwatch();

			NSLoader loader = new NSLoader(@"C:\Users\NullSpace Team\Documents\API_Environment\Assets\StreamingAssets");

				while(true)
			{
				int stat = loader.PollStatus();
				Console.WriteLine("Status: " + stat);

				//	loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
			//	loader.PlayPattern("ns.body_jolt", 1);
				
			System.Threading.Thread.Sleep(10);

			}

			//	loader.Dispose();
		}
	}
}
