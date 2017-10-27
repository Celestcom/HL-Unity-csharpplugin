using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hardlight.SDK.FileUtilities;

namespace Hardlight.SDK
{
	[Serializable]
	[CreateAssetMenu(menuName = "Hardlight/Sequence")]
	public class SequenceSO : ScriptableObjectHaptic
	{
		[SerializeField]
		private List<HapticEffect> _effects = new List<HapticEffect>();
		//[SerializeField]
		//public List<CommonArgs<HapticEffect>> _moreOtherChildren;
		//[SerializeField]
		//public IList<CommonArgs<HapticEffect>> _children;

		//internal IList<CommonArgs<HapticEffect>> Effects
		//{
		//	get { return _children; }
		//	set { _children = value; }
		//}
		public List<HapticEffect> Effects
		{
			get { return _effects; }
			set { _effects = value; }
		}

		/// <summary>
		/// Construct an empty HapticSequence
		/// </summary>
		public void OnEnable()
		{
		}

		/// <summary>
		/// Add a HapticEffect with a given time offset
		/// </summary>
		/// <param name="time">Time offset (fractional seconds)</param>
		/// <param name="effect">The HapticEffect to add</param>
		public SequenceSO AddEffect(HapticEffect effect)
		{
			Effects.Add(effect.Clone());
			return this;
		}

		public SequenceSO RemoveEffect(int index)
		{
			if (Effects.Count > index)
			{
				Effects.RemoveAt(index);
			}
			return this;
		}

		public SequenceSO RemoveEffect(HapticEffect eff)
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

		/// <summary>
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

		public static SequenceSO LoadFromAsset(string resourcesPath)
		{
			var seq = Resources.Load<SequenceSO>(resourcesPath);
			if (seq != null)
			{
				return seq;
			}

			Debug.LogError("Attempted to load SequenceSO from path [" + resourcesPath + "] but failed.\n\rReturned a newly created and empty instance.\n");

			return CreateInstance<SequenceSO>();
		}
		public static SequenceSO LoadFromAssetBundle(string sequenceAssetName, AssetBundle bundle)
		{
			var seq = bundle.LoadAsset<SequenceSO>(sequenceAssetName);

			if (seq != null)
			{
				return seq;
			}

			Debug.LogError("Attempted to load SequenceSO from asset bundle [" + bundle.name + "] with asset name [" + sequenceAssetName + "] but failed.\n\rReturned a newly created and empty instance.\n");

			return CreateInstance<SequenceSO>();
		}
	}
}
