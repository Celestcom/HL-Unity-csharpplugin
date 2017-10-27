using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Hardlight.SDK.FileUtilities;

namespace Hardlight.SDK
{
	public class ScriptableObjectHaptic : ScriptableObject
	{
		protected bool Loaded = false;
		protected string LoadedAssetName = "";

		public virtual void Sort()
		{ }
	}
}
