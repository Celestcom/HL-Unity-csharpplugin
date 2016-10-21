using NullSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSLoaderWrapper
{
	class Program
	{
		static void Main(string[] args)
		{
			NSLoader loader = new NSLoader(@"C:\Users\NullSpace Team\Documents\API_Environment\Assets\StreamingAssets");
				for (int i = 0; i < 20; i++)
				{
				loader.PlayPattern("ns.body_jolt", 0);
				System.Threading.Thread.Sleep(1000);

				}

			//	loader.Dispose();
		}
	}
}
