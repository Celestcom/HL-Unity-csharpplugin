using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace NullSpace.SDK.FileUtilities
{
	
	public class AssetTool
	{
		public class PackageInfo : ParsingUtils.IJsonDeserializable
		{
			public string version;
			public string studio;
			public string @namespace;
			public string path;

			public PackageInfo()
			{

			}

			public void Deserialize(IDictionary<string, object> dict)
			{
				version = dict["version"] as string;
				studio = dict["studio"] as string;
				@namespace = dict["namespace"] as string;
				path = dict["path"] as string;
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
		public class PackageInfoList
		{
			public List<PackageInfo> Packages { get; set; }
		}

		private string _toolName;
		private Process _process;
		private string _rootPath;

		//the HapticAssetTool must be in your path
		public AssetTool()
		{

			const string userRoot = "HKEY_CURRENT_USER";
			const string subkey = "SOFTWARE\\NullSpace VR\\AssetTool";
			const string keyName = userRoot + "\\" + subkey;

			string path = (string)Microsoft.Win32.Registry.GetValue(keyName, "InstallPath", "unknown");
			UnityEngine.Debug.Log("Path of the asset tool: " + path);
			_toolName = "HapticAssetTools.exe";
			_process = new Process();
			UnityEngine.Debug.Log("Is process null?" + (_process == null));
			_process.StartInfo.RedirectStandardOutput = true;
			_process.StartInfo.UseShellExecute = false;

			_process.StartInfo.CreateNoWindow = true;
			_process.StartInfo.FileName = path;
		}

	

		/// <summary>
		/// Set the root haptics directory
		/// </summary>
		/// <param name="path"></param>
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
				throw new InvalidOperationException("You must supply a non-empty root haptics path using the SetRootHapticsFolder(string path) before calling this");
			}
			UnityEngine.Debug.Log("Root path is?" + _rootPath) ;


			var result = executeToolAndWaitForResult(
				new ArgList()
				.Add("root-path", _rootPath)
				.Add("list-packages")
				.Add("json")
			);

			var a = MiniJSON.Json.Deserialize(result) as IList<object>;
			List<PackageInfo> packages = new List<PackageInfo>();
			foreach (var p in a)
			{
				PackageInfo package = new PackageInfo();
				package.Deserialize(p as IDictionary<string, object>);
				packages.Add(package);
			}
			return packages;
		


		}

		private string executeToolAndWaitForResult(ArgList args)
		{
			UnityEngine.Debug.Log("Inside execute tool" + args == null);

			var argString = createArgumentString(args);
			_process.StartInfo.Arguments = argString;
			_process.Start();
			string output = _process.StandardOutput.ReadToEnd();
			_process.WaitForExit();
			return output;
		}

		public HapticDefinitionFile GetHapticDefinitionFile(string path)
		{
	
		
		
			var result = executeToolAndWaitForResult(
				new ArgList()
				.Add("root-path", _rootPath)
				.Add("generate-asset", path)
				.Add("json")
			);

			HapticDefinitionFile hdf = new HapticDefinitionFile();
			hdf.Deserialize(MiniJSON.Json.Deserialize(result) as IDictionary<string, object>);
			return hdf;
		
		}

		public void GetHapticDefinitionFile(string path, HapticDefinitionFile inFile)
		{
			var result = executeToolAndWaitForResult(
							new ArgList()
							.Add("root-path", _rootPath)
							.Add("generate-asset", path)
							.Add("json")
						);

			inFile.Deserialize(MiniJSON.Json.Deserialize(result) as IDictionary<string, object>);

		}

		public string GetHapticDefinitionFileJson(string path)
		{
			var result = executeToolAndWaitForResult(
							new ArgList()
							.Add("root-path", _rootPath)
							.Add("generate-asset", path)
							.Add("json")
						);
			return result;
		}
		private string getHapticType(string path)
		{
			var ext = Path.GetExtension(path);
			if (ext == ".sequence" || ext == ".pattern" || ext == ".experience")
			{
				return ext.Substring(1);
			} else
			{
				throw new FileLoadException("Could not recognize file extension of " + path + ", should be one of: sequence, pattern, experience");
			}
		}

		private string getFileName(string path)
		{
			return Path.GetFileName(path);
		}

		private string getRootPath(string path)
		{
			return System.IO.Directory.GetParent(path).Parent.Parent.FullName;
		}

		private string createArgumentString(ArgList arguments)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var arg in arguments.Args)
			{
				if (arg.Value == "")
				{
					sb.Append(string.Format("--{0} ", arg.Key));
				} else
				{
					sb.Append(string.Format("--{0}=\"{1}\" ", arg.Key, arg.Value));
				}
			}
			return sb.ToString();
		}
	}
}
