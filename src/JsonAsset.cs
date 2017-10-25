using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hardlight.SDK.FileUtilities
{ 
	[Serializable]
	public class JsonAsset : UnityEngine.ScriptableObject
	{
		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		private string json;

		public string GetJson()
		{
			return json;
		}

		public void SetJson(string json)
		{
			this.json = json;
		}
	}
}
