using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hardlight.SDK.FileUtilities;

namespace Hardlight.SDK
{
	/// <summary>
	/// Don't use HapticSequence seq = new HapticSequence
	/// Use HapticSequence.CreateNew()
	/// </summary>
	[Serializable]
	[CreateAssetMenu(menuName = "Hardlight/Sequence")]
	public class HapticSequence : ScriptableObjectHaptic
	{
		/// <summary>
		/// DO NOT use new HapticSequence(). It will not work.
		/// Use HapticSequence.CreateNew()
		/// </summary>
		public HapticSequence()
		{

		}

		[SerializeField]
		private List<HapticEffect> _effects = new List<HapticEffect>();
		public List<HapticEffect> Effects
		{
			get { return _effects; }
			set
			{
				if (value != null)
				{
					_effects = value;
				}
			}
		}

		/// <summary>
		/// Construct an empty HapticSequence
		/// </summary>
		public void OnEnable()
		{
		}

		/// <summary>
		/// Adds a HapticEffect (these parameters go right to it)
		/// Returns self for a builder pattern
		/// </summary>
		/// <param name="effect"></param>
		/// <param name="time"></param>
		/// <param name="duration"></param>
		/// <param name="strength"></param>
		public HapticSequence AddEffect(Effect effect, float time = 0.0f, double duration = 0.0f, float strength = 1.0f)
		{
			Effects.Add(new HapticEffect(effect, time, duration, strength));
			return this;
		}

		/// <summary>
		/// Add a preconstructed HapticEffect
		/// Returns self for a builder pattern
		/// </summary>
		/// <param name="effect">The HapticEffect to add</param>
		public HapticSequence AddEffect(HapticEffect effect)
		{
			Effects.Add(effect.Clone());
			return this;
		}

		public HapticSequence RemoveEffect(int index)
		{
			if (Effects.Count > index)
			{
				Effects.RemoveAt(index);
			}
			return this;
		}

		public HapticSequence RemoveEffect(HapticEffect eff)
		{
			if (Effects.Contains(eff))
			{
				Effects.Remove(eff);
			}
			return this;
		}

		/// <summary>
		/// Create a HapticHandle from this HapticSequence, specifying an AreaFlag to play on.
		/// </summary>
		/// <param name="area">The AreaFlag where this HapticSequence should play</param>
		/// <returns>A new HapticHandle bound to this effect playing on the given area</returns>
		public unsafe HapticHandle CreateHandle(AreaFlag area)
		{
			return CreateHandle(area, 1.0f);
		}

		/// <summary>
		/// Create a HapticHandle for this HapticSequence, specifying an AreaFlag and a strength.
		/// </summary>
		/// <param name="area">The AreaFlag where this HapticSequence should play</param>
		/// <param name="strength">The strength of this HapticSequence (0.0-1.0)</param>
		/// <returns>A new HapticHandle bound to this effect playing on the given area</returns>
		public unsafe HapticHandle CreateHandle(AreaFlag area, double strength)
		{
			EventList e = new ParameterizedSequence(this, area).Generate((float)strength, 0f);
			//Debug.Log("Event List: " + e.ToString() + "\n");
			HapticHandle.CommandWithHandle creator = delegate (HLVR_Effect* handle)
			{
				e.Transmit(handle);
			};

			return new HapticHandle(creator);
		}

		/// <summary>
		/// <para>If you want to play a sequence but don't care about controlling playback, use this method. It will automatically clean up resources.</para>
		/// <para>Synonymous with someSequence.CreateHandle(area).Play().Release() </para>
		/// </summary>
		/// <param name="area">The area on which to play this sequence</param>
		/// <returns>A new HapticHandle bound to this effect playing on the given area</returns>
		public void Play(AreaFlag area)
		{
			CreateHandle(area).Play().Dispose();
		}

		/// <summary>
		/// <para>A helper which calls Play on a newly created HapticHandle.</para>
		/// <para>Synonymous with someSequence.CreateHandle(area, strength).Play()</para>
		/// </summary>
		/// <param name="area">The area on which to play this sequence</param>
		/// <param name="strength">The strength with which to play this sequence</param>
		/// <returns>A new HapticHandle bound to this effect playing on the given area</returns>
		public HapticHandle Play(AreaFlag area, double strength)
		{
			return CreateHandle(area, strength).Play();
		}

		public override void Sort()
		{
			Effects = Effects.OrderBy(x => x.Time).ToList();
		}

		/// Returns a string representation of this HapticSequence for debugging purposes, including all child effects
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("Sequence of {0} HapticEffects:\n", this.Effects.Count));
			foreach (var child in this.Effects)
			{
				sb.Append(string.Format("[{0}]\n", child.ToString()));
			}
			sb.Append("\n");
			return sb.ToString();
		}

		/// <summary>
		/// Use this instead of new HapticSequence()
		/// </summary>
		/// <param name="name">Assigns the new SO's name</param>
		/// <returns></returns>
		public static HapticSequence CreateNew(string name = "Empty Sequence")
		{
			var newSeq = CreateInstance<HapticSequence>();
			newSeq.name = name;
			return newSeq;
		}
		/// <summary>
		/// Attempts to load an existing HapticSequence asset file
		/// If the path does not exist, it will return CreateNew()
		/// </summary>
		/// <param name="resourcesPath">Files should be located in a Resources/Haptics folder. This does not append the Haptics folder (include it on your end)</param>
		/// <returns></returns>
		public static HapticSequence LoadFromAsset(string resourcesPath)
		{
			var seq = Resources.Load<HapticSequence>(resourcesPath);
			if (seq != null)
			{
				return seq;
			}

			Debug.LogError("Attempted to load HapticSequence from path [" + resourcesPath + "] but failed.\n\rReturned a newly created and empty instance.\n");

			return CreateNew();
		}

		/// <summary>
		/// Attempts to load an existing HapticSequence asset file from the provided asset bundle
		/// If the path does not exist, it will return CreateNew()
		/// </summary>
		/// <param name="sequenceAssetName"></param>
		/// <param name="bundle"></param>
		/// <returns></returns>
		public static HapticSequence LoadFromAssetBundle(string sequenceAssetName, AssetBundle bundle)
		{
			var seq = bundle.LoadAsset<HapticSequence>(sequenceAssetName);

			if (seq != null)
			{
				return seq;
			}

			Debug.LogError("Attempted to load HapticSequence from asset bundle [" + bundle.name + "] with asset name [" + sequenceAssetName + "] but failed.\n\rReturned a newly created and empty instance.\n");

			return CreateNew();
		}

		public static HapticSequence LoadFromJson(string jsonPath)
		{
			return HapticResources.CreateSequence(jsonPath);
		}

		///// <summary>
		///// Only available in UnityEditor
		///// </summary>
		///// <param name="fileNameWithoutExtension"></param>
		///// <param name="sequence"></param>
		//public static void SaveAsset(string fileNameWithoutExtension, HapticSequence sequence)
		//{
		//	HapticResources.SaveSequence(fileNameWithoutExtension, sequence);
		//}
	}
}
