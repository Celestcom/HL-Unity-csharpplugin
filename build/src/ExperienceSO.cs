using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hardlight.SDK.FileUtilities;

namespace Hardlight.SDK
{
	[CreateAssetMenu(menuName = "Hardlight/Experience")]
	public class ExperienceSO : ScriptableObjectHaptic
	{
		[SerializeField]
		private List<ParameterizedPattern> _patterns;

		public List<ParameterizedPattern> Patterns
		{
			get { return _patterns; }
			set { _patterns = value; }
		}

		/// <summary>
		/// Add a HapticPattern to this HapticExperience with a given time offset and default strength of 1.0
		/// </summary>
		/// <param name="time">Time offset (fractional seconds)</param>
		/// <param name="area">AreaFlag on which to play the HapticSequence</param>
		/// <param name="sequence">The HapticSequence to be added</param>
		public ExperienceSO AddPattern(double time, ParameterizedPattern pattern)
		{
			Debug.LogError("unfinished\n", this);
			//ParameterizedPattern clone = new ParameterizedPattern(pattern.Clone());
			//_children.Add(new CommonArgs<ParameterizedPattern>((float)time, 1f, clone));
			return this;
		}

		/// <summary>
		/// Add a HapticPattern to this HapticExperience with a given time offset and strength.
		/// </summary>
		/// <param name="time">Time offset (fractional seconds)</param>
		/// <param name="area">AreaFlag on which to play the HapticSequence</param>
		/// <param name="strength">Strength of the HapticSequence (0.0 - 1.0)</param>
		/// <param name="sequence">The HapticSequence to be added</param>
		public ExperienceSO AddPattern(double time, double strength, ParameterizedPattern pattern)
		{
			Debug.LogError("unfinished\n", this);
			//ParameterizedPattern clone = new ParameterizedPattern(pattern.Clone());
			//_children.Add(new CommonArgs<ParameterizedPattern>((float)time, (float)strength, clone));
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
		public ExperienceSO Clone()
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

		public static ExperienceSO LoadFromAsset(string resourcesPath)
		{
			var exp = Resources.Load<ExperienceSO>(resourcesPath);
			if (exp != null)
			{
				return exp;
			}

			Debug.LogError("Attempted to load ExperienceSO from path [" + resourcesPath + "] but failed.\n\rReturned a newly created and empty instance.\n");

			return CreateInstance<ExperienceSO>();
		}
		public static ExperienceSO LoadFromAssetBundle(string experienceAssetName, AssetBundle bundle)
		{
			var exp = bundle.LoadAsset<ExperienceSO>(experienceAssetName);

			if (exp != null)
			{
				return exp;
			}

			Debug.LogError("Attempted to load ExperienceSO from asset bundle [" + bundle.name + "] with asset name [" + experienceAssetName + "] but failed.\n\rReturned a newly created and empty instance.\n");

			return CreateInstance<ExperienceSO>();
		}
	}
}
