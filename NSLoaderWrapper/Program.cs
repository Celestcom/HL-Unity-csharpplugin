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

			int stat = loader.PollStatus();
				for (int i = 0; i < 20; i++)
			{
			//	loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
					loader.PlayPattern("ns.body_jolt", 0);
				
			System.Threading.Thread.Sleep(1000);

			}

			//	loader.Dispose();
		}
	}
}
