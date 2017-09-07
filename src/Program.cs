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

			Console.WriteLine("Hit");
			NSVR.NSVR_Plugin plugin = new NSVR.NSVR_Plugin();

			Console.ReadLine();


			while (true)
			{
				var devices = plugin.GetKnownDevices();
				Console.ReadLine();
			}


		
			return 0;
		}




	}
}
