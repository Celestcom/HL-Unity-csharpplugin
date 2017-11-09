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

		public static void testMechStompSerialize()
		{

			
				//string rootPath = "D:/Users/Projects/Unity-SDK/Assets/StreamingAssets/Haptics/";

				//AssetTool tool = new AssetTool();
				//tool.SetRootHapticsFolder(rootPath);

				//string mechStompPath = "D:/Users/Projects/Unity-SDK/Assets/StreamingAssets/Haptics/NS Demos/experiences/mech_stomp.experience";

				//var mechHDF = tool.GetHapticDefinitionFile(mechStompPath);
				//var mechHDFstring = tool.GetHapticDefinitionFileJson(mechStompPath);

				//string mechStompJson = System.IO.File.ReadAllText(mechStompPath);
				//HapticDefinitionFile file = new HapticDefinitionFile();

				//Console.WriteLine("Deserializing\n");
				//file.Deserialize(mechHDFstring);

				//Console.WriteLine("Serializing\n");
				//var serialized = file.Serialize();

				////Console.WriteLine("\nHDF String\n\t[" + mechHDFstring + "]");
				//Console.WriteLine("Raw Json\n\t[" + mechHDFstring + "]");
				//Console.WriteLine("Deserialized Json to HDF to JSON\n\t[" + serialized + "]");
			}

		public static void testParsing()
		{
			string json = "{'pattern' : [{'time': 1.0, 'sequence': 'test', 'area' : 'random', 'params' : {'count' : 6, 'area-set' : [1,2, 3]}}]}".Replace("'", "\"");

			try
			{




				ScriptablePatternData model = ScriptablePatternParser.Parse(json);
				Console.WriteLine("Okay");

			
		
				



			} catch(ParsingError error)
			{
				Console.WriteLine(error.Message);
			}
		}
		public static int Main()
		{

			//HLVR.HLVR_Plugin plugin = new HLVR.HLVR_Plugin();

			const string userRoot = "HKEY_LOCAL_MACHINE";
			const string subkey = "SOFTWARE\\WOW6432Node\\NullSpace VR\\AssetTool";
			const string keyName = userRoot + "\\" + subkey;
			try
			{
				string path = (string)Microsoft.Win32.Registry.GetValue(keyName, "InstallPath", "unknown");
				Console.WriteLine("Path: " + path + "\n");

			}
			catch (Exception e)
			{
				Console.WriteLine("Exception e: " + e.Message + "\n");
			}


			//testParsing();

			Console.ReadLine();

			return 0;
		}




	}
}
