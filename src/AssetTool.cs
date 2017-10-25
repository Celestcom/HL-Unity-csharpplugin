using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.ComponentModel;

namespace Hardlight.SDK.FileUtilities
{
	/// <summary>
	/// A wrapper over the Haptic Asset Tool binary. Retrieves json responses from the tool, 
	/// allowing the retrieval of a package list or .hdf
	/// </summary>
	public class AssetTool
	{
		/// <summary>
		/// Holds relevant information about a haptic package
		/// </summary>
		public class PackageInfo : ParsingUtils.IJsonDeserializable
		{
			/// <summary>
			/// Version of the packages
			/// </summary>
			public string version;

			/// <summary>
			/// Studio that created the packages
			/// </summary>
			public string studio;

			/// <summary>
			/// Namespace of the packages
			/// </summary>
			public string @namespace;

			/// <summary>
			/// Absolute file path to the package's root directory
			/// </summary>
			public string path;

			/// <summary>
			/// Constructs an empty PackageInfo object
			/// </summary>
			public PackageInfo()
			{

			}

			/// <summary>
			/// Short description of this package
			/// </summary>
			public string description;

			/// <summary>
			/// Given a dictionary representing the json object of a package, deserialize into a PackageInfo
			/// </summary>
			/// <param name="dict">json object</param>
			void ParsingUtils.IJsonDeserializable.Deserialize(IDictionary<string, object> dict)
			{
				version = dict["version"] as string;
				studio = dict["studio"] as string;
				@namespace = dict["namespace"] as string;
				path = dict["path"] as string;
				description = dict["description"] as string;
			}
		}

		private class ArgList
		{
			private IDictionary<string, string> _args;
			public ArgList()
			{
				_args = new Dictionary<string, string>();
			}

			public ArgList Add(string key, string value)
			{
				_args[key] = value;
				return this;
			}

			public ArgList Add(string key)
			{
				_args[key] = "";
				return this;
			}

			public List<KeyValuePair<string, string>> Args { get { return this._args.ToList(); } }
		}

		/// <summary>
		/// This will deal with opening and running the asset tool binary
		/// </summary>
		private Process _process;

		/// <summary>
		/// The root path of the haptics folder
		/// </summary>
		private string _rootPath;

		/// <summary>
		/// Create a new AssetTool. Note that the asset tool MUST be in the user's registry, which means
		/// the service MUST be installed. 
		/// </summary>
		public AssetTool()
		{

			_process = new Process();
			_process.StartInfo.RedirectStandardOutput = true;
			_process.StartInfo.UseShellExecute = false;

			_process.StartInfo.CreateNoWindow = true;

			const string userRoot = "HKEY_CURRENT_USER";
			const string subkey = "SOFTWARE\\Hardlight VR\\AssetTool";
			const string keyName = userRoot + "\\" + subkey;
			try
			{
				string path = (string)Microsoft.Win32.Registry.GetValue(keyName, "InstallPath", "unknown");

				if (path == "unknown")
				{
#if UNITY_EDITOR
					UnityEngine.Debug.LogError("[HLVR] Failed to find the asset tool's install directory. Is the Service installed?\n");
#else
					Console.WriteLine("[HLVR] Failed to find the asset tool's install directory. Is the Service installed?\n");
#endif
				}
				else
				{
					_process.StartInfo.FileName = path;

				}
			}
			catch (ArgumentException)
			{
#if UNITY_EDITOR
				UnityEngine.Debug.LogError("[HLVR] Failed to find the asset tool's install directory. Is the Service installed?\n");
#else
				Console.WriteLine("[HLVR] Failed to find the asset tool's install directory. Is the Service installed?\n");
#endif
			}
			catch (IOException)
			{
#if UNITY_EDITOR
				UnityEngine.Debug.LogError("[HLVR] Failed to find the asset tool's install directory. Try reinstalling the service.\n");
#else
				Console.WriteLine("[HLVR] Failed to find the asset tool's install directory. Try reinstalling the service.\n");
#endif
			}
			catch (SecurityException)
			{
#if UNITY_EDITOR
				UnityEngine.Debug.LogError("[HLVR] Failed to find the asset tool's install directory, because I don't have permission to read the registry. Try running as administrator?\n");
#else
				Console.WriteLine("[HLVR] Failed to find the asset tool's install directory, because I don't have permission to read the registry. Try running as administrator?\n");
#endif
			}
		}



		/// <summary>
		/// Set the user's root haptics directory
		/// </summary>
		/// <param name="path">Absolute path to the root haptics directory</param>
		public void SetRootHapticsFolder(string path)
		{
			_rootPath = path;
		}

		/// <summary>
		/// Retrieve a list of the packages present in a haptics directory, along with associated info
		/// </summary>
		/// <exception cref="InvalidOperationException">If root haptics directory is not set</exception>
		/// <returns>List of PackageInfo objects</returns>
		public List<PackageInfo> TryGetPackageInfo()
		{

			if (_rootPath == null || _rootPath == "")
			{
				throw new InvalidOperationException("You must supply a non-empty root haptics path using SetRootHapticsFolder before calling this method");
			}

			var result = executeToolAndWaitForResult(
				new ArgList()
				.Add("root-path", _rootPath)
				.Add("list-packages")
				.Add("json")
			);
			//We need better error reporting for the json communication
			if (result.Contains("Malformed config"))
			{
				UnityEngine.Debug.LogError("[HLVR] Asset tool failed: " + result + "\n");
				return new List<PackageInfo>();
			}
			var a = MiniJSON.Json.Deserialize(result) as IList<object>;
			List<PackageInfo> packages = new List<PackageInfo>();
			foreach (var p in a)
			{
				PackageInfo package = new PackageInfo();
				((ParsingUtils.IJsonDeserializable)package).Deserialize(p as IDictionary<string, object>);
				packages.Add(package);
			}
			return packages;



		}

		private string executeToolAndWaitForResult(ArgList args)
		{

			var argString = createArgumentString(args);

			_process.StartInfo.Arguments = argString;
			_process.Start();
			string output = _process.StandardOutput.ReadToEnd();

			_process.WaitForExit(500);

			if (output.Contains("Error:"))
			{
				throw new HapticsAssetException(output.Substring(output.IndexOf("Error:")));
			}
			return output;

		}

		/// <summary>
		/// Given a path to the raw haptic asset, generate a HapticDefinitionFile 
		/// </summary>
		/// <param name="path">Path to haptic asset. Ex: C:\Haptics\my\patterns\test.pattern</param>
		/// <returns></returns>
		public HapticDefinitionFile GetHapticDefinitionFile(string path)
		{
			var result = executeToolAndWaitForResult(
				new ArgList()
				.Add("root-path", _rootPath)
				.Add("generate-asset", path)
				.Add("json")
			);

			HapticDefinitionFile hdf = new HapticDefinitionFile();

			try
			{
				object x = MiniJSON.Json.Deserialize(result);
				hdf.Deserialize(x as IDictionary<string, object>);
				return hdf;
			}
			catch (Exception e)
			{
#if UNITY_EDITOR
				UnityEngine.Debug.LogException(e);
#else
				Console.WriteLine(e);
#endif
				throw new HapticsLoadingException("[HLVR] Couldn't deserialize the json response for the request file path [" + path + "]");
			}


		}


		/// <summary>
		/// Given a path to the raw haptic asset, return the json representation of a HapticDefinitionFile
		/// </summary>
		/// <param name="path">Path to haptic asset. Ex: C:\Haptics\my\patterns\test.pattern</param>
		/// <returns>JSON string representing a HapticDefinitionFile</returns>
		public string GetHapticDefinitionFileJson(string path)
		{
			var result = executeToolAndWaitForResult(
							new ArgList()
							.Add("root-path", _rootPath)
							.Add("generate-asset", path)

						);

			return result;
		}

		/// <summary>
		/// Converts a haptic package into an HDF package, mirroring the standard haptic directory layout
		/// </summary>
		/// <param name="package">The package to convert</param>
		/// <returns>An error string, if any</returns>
		public string ConvertPackageToHDFs(PackageInfo package, string outDir)
		{
			var result = executeToolAndWaitForResult(
				new ArgList()
				.Add("root-path", _rootPath)
				.Add("convert-package", package.@namespace)
				.Add("hdf-out", outDir)
			);

			return result;
		}

		private string createArgumentString(ArgList arguments)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var arg in arguments.Args)
			{
				if (arg.Value == "")
				{
					sb.Append(string.Format("--{0} ", arg.Key));
				}
				else
				{
					sb.Append(string.Format("--{0}=\"{1}\" ", arg.Key, arg.Value));
				}
			}
			return sb.ToString();
		}
	}
}
