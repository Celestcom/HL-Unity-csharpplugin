
using NullSpace.SDK.FileUtilities;
using System.IO;
using UnityEngine;
namespace NullSpace.SDK
{
	public class SerializableHaptic
	{
		private string _type;

		internal SerializableHaptic(string type)
		{
			_type = type;
		}

		public void LoadFromAsset(string assetPath)
		{
			var file = Resources.Load<JsonAsset>(assetPath);

			if (file == null)
			{
				Debug.LogWarning("Unable to load haptic resource at path " + assetPath);
				return;
			}

			HapticDefinitionFile hdf = new HapticDefinitionFile();
			hdf.Deserialize(file.GetJson());

			if (hdf.rootEffect.type != _type)
			{
				Debug.LogWarning(string.Format("File type mismatch: file is a {0}, but this is a {1}", hdf.rootEffect.type, _type));
				return;
			}

			doLoadFromHDF(hdf.rootEffect.name, hdf);
		}

		internal virtual void doLoadFromHDF(string key, HapticDefinitionFile file) { }


	}
}
