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

			HapticSequence s = new HapticSequence();

			s.AddEffect(0.0, new HapticEffect(Effect.Click));
			s.AddEffect(0.2, new HapticEffect(Effect.Click));
			s.AddEffect(0.4, new HapticEffect(Effect.Click));

			s.Play(AreaFlag.All_Areas);

			Console.ReadLine();
			return 0;
		}

	}
}
