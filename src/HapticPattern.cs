using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hardlight.SDK.FileUtilities;

namespace Hardlight.SDK
{
	[Serializable]
	[CreateAssetMenu(menuName = "Hardlight/Pattern")]
	public class HapticPattern : ScriptableObjectHaptic
	{
		/// <summary>
		/// DO NOT use new HapticPattern(). It will not work.
		/// Use HapticPattern.CreateNew()
		/// </summary>
		public HapticPattern()
		{

		}

		//[SerializeField]
		//private List<HapticSequence> sequenceKeys = new List<HapticSequence>();
		//public List<HapticSequence> SequenceKeys
		//{
		//	get
		//	{
		//		return sequenceKeys;
		//	}

		//	set
		//	{
		//		if (value != null)
		//			sequenceKeys = value;
		//	}
		//}

		[SerializeField]
		private List<ParameterizedSequence> _sequences = new List<ParameterizedSequence>();
		public List<ParameterizedSequence> Sequences
		{
			get { return _sequences; }
			set
			{
				if (value != null)
					_sequences = value;
			}
		}

		/// <summary>
		/// Construct an empty HapticSequence
		/// </summary>
		public void OnEnable()
		{
		}
		/// <summary>
		/// Add a HapticSequence to this HapticPattern with a given time offset and AreaFlag, and default strength of 1.0
		/// </summary>
		/// <param name="time">Time offset (fractional seconds)</param>
		/// <param name="area">AreaFlag on which to play the HapticSequence</param>
		/// <param name="sequence">The HapticSequence to be added</param>
		public HapticPattern AddSequence(ParameterizedSequence sequence)
		{
			Sequences.Add(sequence.Clone());
			return this;
		}

		/// <summary>
		/// Add a HapticSequence to this HapticPattern with a given time offset, AreaFlag, and strength.
		/// </summary>
		/// <param name="time">Time offset (fractional seconds)</param>
		/// <param name="area">AreaFlag on which to play the HapticSequence</param>
		/// <param name="strength">Strength of the HapticSequence (0.0 - 1.0)</param>
		/// <param name="sequence">The HapticSequence to be added</param>
		public HapticPattern AddSequence(double time, AreaFlag area, double strength, ParameterizedSequence sequence)
		{
			//ParameterizedSequence clone = new ParameterizedSequence(sequence.Clone(), area);
			//_children.Add(new CommonArgs<ParameterizedSequence>((float)time, (float)strength, clone));
			return this;
		}

		/// <summary>
		/// Create a HapticHandle from this HapticPattern, which can be used to manipulate the effect. 
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public unsafe HapticHandle CreateHandle()
		{
			return CreateHandle(1.0f);
		}

		/// <summary>
		/// Create a HapticHandle from this HapticPattern, passing in a given strength. 
		/// </summary>
		/// <param name="strength"></param>
		/// <returns>A new HapticHandle</returns>
		public unsafe HapticHandle CreateHandle(double strength)
		{
			EventList e = new ParameterizedPattern(this).Generate((float)strength, 0f);

			HapticHandle.CommandWithHandle creator = delegate (HLVR_Effect* handle)
			{
				e.Transmit(handle);

			};

			return new HapticHandle(creator);
		}

		/// <summary>
		/// <para>If you want to play a pattern but don't care about controlling playback, use this method. It will automatically clean up resources.</para>
		/// <para>Synonymous with somePattern.CreateHandle().Play().Release()</para>
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public void Play()
		{
			CreateHandle().Play().Dispose();
		}

		/// <summary>
		/// <para>If you want to play a pattern but don't care about controlling playback, use this method. It will automatically clean up resources.</para>
		/// <para>Synonymous with somePattern.CreateHandle(strength).Play().Release()</para>
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public void Play(double strength)
		{
			CreateHandle(strength).Play().Dispose();
		}

		/// <summary>
		/// Create an independent copy of this HapticPattern
		/// </summary>
		/// <returns></returns>
		public HapticPattern Clone()
		{
			Debug.LogError("Unfinished Clone\n", this);
			//var clone = new HapticPattern(LoadedAssetName);
			//clone.Sequences = new List<CommonArgs<ParameterizedSequence>>(_children);
			//return clone;
			return null;
		}

		public override void Sort()
		{
			Sequences = Sequences.OrderBy(x => x.Time).ToList();
		}
		/// <summary>
		/// Returns a string representation of this HapticPattern for debugging purposes, including all child sequences
		/// </summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("Pattern of {0} sequences: \n", this.Sequences.Count));

			foreach (var child in this.Sequences)
			{
				sb.Append(string.Format("{0}\n", child.ToString()));
			}
			sb.Append("\n");
			return sb.ToString();
		}

		/// <summary>
		/// Use this instead of new HapticPattern()
		/// </summary>
		/// <returns></returns>
		public static HapticPattern CreateNew(string name = "Empty Sequence")
		{
			var newPat = CreateInstance<HapticPattern>();
			newPat.name = name;
			return newPat;
		}

		public static HapticPattern LoadFromJson(string jsonPath)
		{
			return HapticResources.CreatePattern(jsonPath);
		}

		///// <summary>
		///// Only available in UnityEditor
		///// </summary>
		///// <param name="fileNameWithoutExtension"></param>
		///// <param name="pattern"></param>
		//public static void SaveAsset(string fileNameWithoutExtension, HapticPattern pattern)
		//{
		//	HapticResources.SavePattern(fileNameWithoutExtension, pattern);
		//}

		/// <summary>
		/// Attempts to load an existing HapticPattern asset file
		/// If the path does not exist, it will return CreateNew()
		/// </summary>
		/// <param name="resourcesPath">Files should be located in a Resources/Haptics folder. This does not append the Haptics folder (include it on your end)</param>
		/// <returns></returns>
		public static HapticPattern LoadFromAsset(string resourcesPath)
		{
			var pat = Resources.Load<HapticPattern>(resourcesPath);
			if (pat != null)
			{
				return pat;
			}

			Debug.LogError("Attempted to load HapticPattern from path [" + resourcesPath + "] but failed.\n\rReturned a newly created and empty instance.\n");

			return CreateInstance<HapticPattern>();
		}

		/// <summary>
		/// Attempts to load an existing HapticPattern asset file from the provided asset bundle
		/// If the path does not exist, it will return CreateNew()
		/// </summary>
		/// <param name="patternAssetName"></param>
		/// <param name="bundle"></param>
		/// <returns></returns>
		public static HapticPattern LoadFromAssetBundle(string patternAssetName, AssetBundle bundle)
		{
			var pat = bundle.LoadAsset<HapticPattern>(patternAssetName);

			if (pat != null)
			{
				return pat;
			}

			Debug.LogError("Attempted to load HapticPattern from asset bundle [" + bundle.name + "] with asset name [" + patternAssetName + "] but failed.\n\rReturned a newly created and empty instance.\n");

			return CreateInstance<HapticPattern>();
		}
	}
}
