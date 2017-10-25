
using Hardlight.SDK.FileUtilities;
using System.IO;
using UnityEngine;
namespace Hardlight.SDK
{
	public class SerializableHaptic
	{
		protected bool Loaded = false;
		private string _type;
		protected string LoadedAssetName = "";

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

			if (hdf.root_effect.type != _type)
			{
				Debug.LogError(string.Format("File type mismatch at path [{0}]:\n\t file is a {1}, but this is a {2}", assetPath, hdf.root_effect.type, _type));
				return;
			}

			LoadedAssetName = assetPath;
			Loaded = true;
			doLoadFromHDF(hdf.root_effect.name, hdf);
		}

		public void HandleLazyAssetLoading()
		{
			if (!Loaded && LoadedAssetName.Length > 0)
			{
				LoadFromAsset(LoadedAssetName);
			}
		}

		internal virtual void doLoadFromHDF(string key, HapticDefinitionFile file) { }

		////Cull before and after
		//public virtual SerializableHaptic CullEvents(bool cullBeforeThreshold, float timeThreshold)
		//{ return this; }
		////Cull before and after
		//public virtual SerializableHaptic CullEventsWithinRange(Vector2 cullRange)
		//{ return this; }
		////Clears all elements
		//public virtual SerializableHaptic ClearAllEvents()
		//{ return this; }
		////Inserts an offset for all elements after the time of offset
		//public virtual SerializableHaptic InsertOffset(float offsetDuration, float timeOfOffset = 0.0f)
		//{ return this; }
		////Sets all elements within a range to a specified strength
		//public virtual SerializableHaptic SetStrengthOfElementsWithinRange(float newStrength, Vector2 range)
		//{ return this; }
		////Multiplies all elements within a range by specific strength
		//public virtual SerializableHaptic MultiplyStrengthOfElementsWithinRange(float strengthMultiplier, Vector2 range)
		//{ return this; }
		////Offsets all elements within a range by a new value (add 5 seconds to elements in the first minute)
		//public virtual SerializableHaptic OffsetElementsWithinRange(float offsetToApply, Vector2 elementTimeRange)
		//{ return this; }
		////Splitting CodeHaptics into two separate code haptic elements.
		//public virtual SerializableHaptic MoveElementsIntoANewCodeHaptic(Vector2 elementTimeRange)
		//{ return this; }
		//public virtual SerializableHaptic Clone()
		//{ return this; }
		////For each element, reflect it's location L/R Area flags
		//public virtual SerializableHaptic MirrorAreaFlags()
		//{ return this; }
		//public virtual SerializableHaptic MirrorAreaFlagsWithinRange(Vector2 elementTimeRange)
		//{ return this; }
		//public virtual int GetElementCount()
		//{ return 0; }
	}
}
