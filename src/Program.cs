using System;

using Hardlight.SDK;
using Hardlight.SDK.Internal;
using System.ServiceProcess;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using Hardlight.SDK.FileUtilities;

namespace NSLoaderWrapper
{

	public unsafe class Program
	{


		public static int Main()
		{

			HLVR.HLVR_Plugin plugin = new HLVR.HLVR_Plugin();
			string rootPath = "D:/Users/Projects/Unity-SDK/Assets/StreamingAssets/Haptics/";

			AssetTool tool = new AssetTool();
			tool.SetRootHapticsFolder(rootPath);

			string mechStompPath = "D:/Users/Projects/Unity-SDK/Assets/StreamingAssets/Haptics/NS Demos/experiences/mech_stomp.experience";

			var mechHDF = tool.GetHapticDefinitionFile(mechStompPath);
			var mechHDFstring = tool.GetHapticDefinitionFileJson(mechStompPath);

			string mechStompJson = System.IO.File.ReadAllText(mechStompPath);
			HapticDefinitionFile file = new HapticDefinitionFile();

			Console.WriteLine("Deserializing\n");
			file.Deserialize(mechHDFstring);

			Console.WriteLine("Serializing\n");
			var serialized = file.Serialize();

			//Console.WriteLine("\nHDF String\n\t[" + mechHDFstring + "]");
			Console.WriteLine("Raw Json\n\t[" + mechHDFstring + "]");
			Console.WriteLine("Deserialized Json to HDF to JSON\n\t[" + serialized + "]");
			//HapticSequence seq = new HapticSequence();
			//seq.AddEffect(0.0f, new HapticEffect(Effect.Click, .1f));
			//seq.AddEffect(0.25f, new HapticEffect(Effect.Click, .1f));
			//seq.AddEffect(0.50f, new HapticEffect(Effect.Click, .1f));
			//seq.AddEffect(0.75f, new HapticEffect(Effect.Click, .1f));

			//Creat the HDF & it's root effect (which defines the file name to my understanding).
			//ParsingUtils.RootEffect root = new ParsingUtils.RootEffect("click", "sequence");
			//HapticDefinitionFile hdf = new HapticDefinitionFile(root);

			////Create a json effect atom for us to use.
			//ParsingUtils.JsonEffectAtom jsonAtom = new ParsingUtils.JsonEffectAtom(Effect.Click, 0.0f, .25f);
			////Make a list to add it to (to represent all of the effects in the HDF)
			//var atomList = new List<ParsingUtils.JsonEffectAtom>();
			//atomList.Add(jsonAtom);

			////Probably need to create a JsonSequenceAtom that uses one of the atoms.

			//hdf.sequence_definitions.Add("click", atomList);

			//Make sure that HDF root effect is assigned
			//Console.WriteLine(hdf.root_effect.name + "  " + hdf.root_effect.type + "\n");

			//Attempt to serialize the HDF.
			//string serialized = hdf.Serialize();

			//Print out the serialization
			//Console.WriteLine("\nJSON\n\t[" + serialized + "]");

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
