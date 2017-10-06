using System;

using NullSpace.SDK;
using NullSpace.SDK.Internal;
using System.ServiceProcess;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using NullSpace.SDK.FileUtilities;

namespace NSLoaderWrapper
{

	public unsafe class Program
	{


		public static int Main()
		{

			NSVR.NSVR_Plugin plugin = new NSVR.NSVR_Plugin();

			//HapticSequence seq = new HapticSequence();
			//seq.AddEffect(0.0f, new HapticEffect(Effect.Click, .1f));
			//seq.AddEffect(0.25f, new HapticEffect(Effect.Click, .1f));
			//seq.AddEffect(0.50f, new HapticEffect(Effect.Click, .1f));
			//seq.AddEffect(0.75f, new HapticEffect(Effect.Click, .1f));

			//Creat the HDF & it's root effect (which defines the file name to my understanding).
			ParsingUtils.RootEffect root = new ParsingUtils.RootEffect("click", "sequence");
			HapticDefinitionFile hdf = new HapticDefinitionFile(root);

			//Create a json effect atom for us to use.
			ParsingUtils.JsonEffectAtom jsonAtom = new ParsingUtils.JsonEffectAtom(Effect.Click, 0.0f, .25f);
			//Make a list to add it to (to represent all of the effects in the HDF)
			var atomList = new List<ParsingUtils.JsonEffectAtom>();
			atomList.Add(jsonAtom);

			//Probably need to create a JsonSequenceAtom that uses one of the atoms.

			hdf.sequence_definitions.Add("click", atomList);

			//Make sure that HDF root effect is assigned
			//Console.WriteLine(hdf.root_effect.name + "  " + hdf.root_effect.type + "\n");

			//Attempt to serialize the HDF.
			string serialized = hdf.Serialize();

			//Print out the serialization
			Console.WriteLine("\nJSON\n\t[" + serialized + "]");

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
