
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
				Debug.LogError(string.Format("Unable to load haptic resource at path [{0}]:\n\t file is null", assetPath));
				return;
			}

			HapticDefinitionFile hdf = new HapticDefinitionFile();

			var json = file.GetJson();
			if (json.Length == 0)
			{
				Debug.LogError(string.Format("Unable to load haptic resource at path [{0}]:\n\t file length is 0", assetPath));
				return;
			}

			try
			{
				hdf.Deserialize(json);
			} catch (HapticsAssetException e)
			{
				Debug.LogError(string.Format("Unable to load haptic resource at path [{0}]:\n\t {1}", assetPath, e.Message));
				return;
			}

			if (hdf.rootEffect.type != _type)
			{
				Debug.LogError(string.Format("File type mismatch at path [{0}]:\n\t file is a {1}, but this is a {2}", assetPath, hdf.rootEffect.type, _type));
				return;
			}

			doLoadFromHDF(hdf.rootEffect.name, hdf);
		}

		internal virtual void doLoadFromHDF(string key, HapticDefinitionFile file) { }


	}
}
