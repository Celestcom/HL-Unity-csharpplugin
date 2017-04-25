using System;

using NullSpace.SDK;
using NullSpace.SDK.Internal;
using System.ServiceProcess;

namespace NSLoaderWrapper
{

	public class Program
	{

		

		public static int Main()
		{

			NSVR.NSVR_Plugin loader = new NSVR.NSVR_Plugin();

			while (true)
			{
				var status = loader.TestServiceConnection();
				Console.WriteLine(status);
			}
			return 0;
		}

	}
}
