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
		public class PackageInfo : LoadingUtils.IJsonDeserializable
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

		public class PackageInfoList
		{
			public List<PackageInfo> Packages { get; set; }
		}

		private string _toolName;
		private string _toolLocation;
		private Process _process;
		private string _rootPath;
		private IDictionary<string, string> _arguments;

		//the HapticAssetTool must be in your path
		public AssetTool()
		{
		//	_toolLocation = @"C:\Users\NullSpace Team\Documents\Visual Studio 2015\Projects\HapticAssetTools\Debug\";
			_toolName = "HapticAssetTools.exe";
			_process = new Process();
			_process.StartInfo.RedirectStandardOutput = true;
			_process.StartInfo.UseShellExecute = false;

			_process.StartInfo.CreateNoWindow = true;
			_process.StartInfo.FileName =  _toolName;
			_arguments = new Dictionary<string, string>();
			_rootPath = @"C:\Users\NullSpace Team\Documents\NullSpace SDK 0.1.1\Assets\StreamingAssets\Haptics";

		

		}

	

		/// <summary>
		/// Set the root haptics directory
		/// </summary>
		/// <param name="path"></param>
		public void SetRootHapticsFolder(string path)
		{
			_rootPath = path;
		}
		public List<PackageInfo> TryGetPackageInfo()
		{

			var result = executeToolWithArgs(
				new Dictionary<string, string>() {
						{"root-path", _rootPath },
						{"list-packages", "" },
						{"json", "" }
				}
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

		private string executeToolWithArgs(IDictionary<string, string> args)
		{
			var argString = createArgumentString(args);
			_process.StartInfo.Arguments = argString;
			_process.Start();
			string output = _process.StandardOutput.ReadToEnd();
			_process.WaitForExit();
			return output;
		}

		public LoadingUtils.HapticDefinitionFile GetHapticDefinitionFile(string path)
		{
	
		
			var result = executeToolWithArgs(
				new Dictionary<string, string>() {
					{ "root-path", _rootPath },
					{ "generate-asset", path },
					{"json", "" }
				}
			);

			LoadingUtils.HapticDefinitionFile hdf = new LoadingUtils.HapticDefinitionFile();
			hdf.Deserialize(MiniJSON.Json.Deserialize(result) as IDictionary<string, object>);
			return hdf;
		
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

		private string createArgumentString(IDictionary<string, string> arguments)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var arg in arguments)
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
