using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Hardlight.SDK.FileUtilities
{
	[Serializable]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		//[SerializeField]
		//private TKey[] keys;

		[SerializeField]
		private List<TKey> keys = new List<TKey>();

		//[SerializeField]
		//private TValue[] values;

		[SerializeField]
		private List<TValue> values = new List<TValue>();

		// save the dictionary to lists
		public void OnBeforeSerialize()
		{

			//keys = new TKey[this.Keys.Count];
			//values = new TValue[this.Values.Count];

			keys.Clear();
			values.Clear();

			
			foreach (KeyValuePair<TKey, TValue> pair in this)
			{
				keys.Add(pair.Key);
				values.Add(pair.Value);
			}
		}

		// load dictionary from lists
		public void OnAfterDeserialize()
		{
			this.Clear();

			if (keys.Count!= values.Count)
				Debug.LogException(new Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable.")));
			Debug.Log("There are " + keys.Count + " keys, and " + values.Count + " values");
			for (int i = 0; i < keys.Count; i++)
				this.Add(keys[i], values[i]);

			Debug.Log("The new dict's key amount is " + this.Keys.Count);
		}
	}
}
