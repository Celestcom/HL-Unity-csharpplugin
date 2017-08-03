using System;

using NullSpace.SDK;
using NullSpace.SDK.Internal;
using System.ServiceProcess;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace NSLoaderWrapper
{

	public unsafe class Program
	{

		
		public static int Main()
		{


			NSVR.NSVR_Plugin plugin = new NSVR.NSVR_Plugin();

			for (int i = 0; i < 100; i++)
			{
				plugin.PollBodyView();
				System.Threading.Thread.Sleep(100);
			}

			Console.ReadLine();


		
			return 0;
		}




	}
}
