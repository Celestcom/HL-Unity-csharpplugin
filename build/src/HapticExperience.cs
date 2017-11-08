using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hardlight.SDK.FileUtilities;

namespace Hardlight.SDK
{
	[Serializable]
	[CreateAssetMenu(menuName = "Hardlight/Experience")]
	public class HapticExperience : ScriptableObjectHaptic
	{
		/// <summary>
		/// DO NOT use new HapticExperience(). It will not work.
		/// Use HapticExperience.CreateNew()
		/// </summary>
		public HapticExperience()
		{

		}

		[SerializeField]
		private List<ParameterizedPattern> _patterns = new List<ParameterizedPattern>();

		public List<ParameterizedPattern> Patterns
		{
			get { return _patterns; }
			set { _patterns = value; }
		}

		/// <summary>
		/// Adds a HapticPattern to this HapticExperience with a given time offset and default strength of 1.0
		/// </summary>
		/// <param name="pattern">The pattern with time/strength attributes</param>
		/// <param name="time">Time offset (fractional seconds)</param>
		/// <param name="strength">Strength of the Pattern (for switching which effects are used)</param>
		/// <returns></returns>
		public HapticExperience AddPattern(HapticPattern pattern, double time, double strength)
		{
			Patterns.Add(new ParameterizedPattern(pattern, (float)time, (float)strength));
			return this;
		}

		/// <summary>
		/// Add a HapticPattern to this HapticExperience with a given time offset and default strength of 1.0
		/// </summary>
		/// <param name="pattern">The pattern with time/strength attributes</param>
		public HapticExperience AddPattern(ParameterizedPattern pattern)
		{
			Patterns.Add(pattern);
			return this;
		}

		/// <summary>
		/// Create a HapticHandle from this HapticExperience, which can be used to manipulate the effect. 
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public unsafe HapticHandle CreateHandle()
		{
			return CreateHandle(1.0f);
		}

		/// <summary>
		/// Create a HapticHandle from this HapticExperience, passing in a given strength. 
		/// </summary>
		/// <param name="strength"></param>
		/// <returns>A new HapticHandle</returns>
		public unsafe HapticHandle CreateHandle(double strength)
		{
			EventList e = new ParameterizedExperience(this).Generate((float)strength, 0f);

			HapticHandle.CommandWithHandle creator = delegate (HLVR_Effect* handle)
			{
				e.Transmit(handle);
			};

			return new HapticHandle(creator);
		}

		/// <summary>
		/// <para>If you want to play an experience but don't care about controlling playback, use this method. It will automatically clean up resources.</para>
		/// <para>Synonymous with someExperience.CreateHandle().Play()</para>
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public void Play()
		{
			CreateHandle().Play().Dispose();
		}

		/// <summary>
		/// <para>If you want to play an experience but don't care about controlling playback, use this method. It will automatically clean up resources.</para>
		/// <para>Synonymous with someExperience.CreateHandle(strength).Play().Release()</para>
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public void Play(double strength)
		{
			CreateHandle(strength).Play().Dispose();
		}

		public override void Sort()
		{
			Patterns = Patterns.OrderBy(x => x.Time).ToList();
		}

		/// <summary>
		/// Create an independent copy of this HapticExperience
		/// </summary>
		/// <returns></returns>
		public HapticExperience Clone()
		{
			Debug.LogError("Unadded Clone\n", this);
			//var clone = new HapticExperience(LoadedAssetName);
			//clone.Patterns = new List<CommonArgs<ParameterizedPattern>>(_children);
			return null;
		}
		/// <summary>
		/// Returns a representation of this HapticExperience for debugging purposes, including the representation of child patterns
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("Experience of {0} patterns: \n", this.Patterns.Count));

			foreach (var child in this.Patterns)
			{
				sb.Append(string.Format("{0}\n", child.ToString()));
			}
			sb.Append("\n");
			return sb.ToString();
		}

		/// <summary>
		/// Use this instead of new HapticExperience()
		/// </summary>
		/// <returns></returns>
		public static HapticExperience CreateNew()
		{
			return CreateInstance<HapticExperience>();
		}

		public static HapticExperience LoadFromJson(string jsonPath)
		{
			return HapticResources.CreateExperience(jsonPath);
		}
		public static void SaveAsset(string fileNameWithoutExtension, HapticExperience experience)
		{
			HapticResources.SaveExperience(fileNameWithoutExtension, experience);
		}

		/// <summary>
		/// Attempts to load an existing HapticSequence asset file
		/// If the path does not exist, it will return CreateNew()
		/// </summary>
		/// <param name="resourcesPath">Files should be located in a Resources/Haptics folder. This does not append the Haptics folder (include it on your end)</param>
		/// <returns></returns>
		public static HapticExperience LoadFromAsset(string resourcesPath)
		{
			var exp = Resources.Load<HapticExperience>(resourcesPath);
			if (exp != null)
			{
				return exp;
			}

			Debug.LogError("Attempted to load HapticExperience from path [" + resourcesPath + "] but failed.\n\rReturned a newly created and empty instance.\n");

			return CreateInstance<HapticExperience>();
		}

		/// <summary>
		/// Attempts to load an existing HapticExperience asset file from the provided asset bundle
		/// If the path does not exist, it will return CreateNew()
		/// </summary>
		/// <param name="experienceAssetName"></param>
		/// <param name="bundle"></param>
		/// <returns></returns>
		public static HapticExperience LoadFromAssetBundle(string experienceAssetName, AssetBundle bundle)
		{
			var exp = bundle.LoadAsset<HapticExperience>(experienceAssetName);

			if (exp != null)
			{
				return exp;
			}

			Debug.LogError("Attempted to load HapticExperience from asset bundle [" + bundle.name + "] with asset name [" + experienceAssetName + "] but failed.\n\rReturned a newly created and empty instance.\n");

			return CreateInstance<HapticExperience>();
		}
	}
}
