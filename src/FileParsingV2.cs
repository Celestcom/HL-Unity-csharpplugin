using System;

using Hardlight.Events;
using System.Linq;
using System.Collections.Generic;
using Hardlight.SDK.FileUtilities;
using UnityEngine;

namespace Hardlight.SDK.FileUtilities
{
	
	public class ParsingError : Exception
	{
		public ParsingError(string message) : base(message) { }
		public ParsingError(string message, Exception inner) : base(message, inner) { }
	} 

	public class RawEntry
	{
		public float Time;
		public string Sequence;
		public string AreaGenerator;
		public IDictionary<string, object> AreaGeneratorArgs; 
	}

	public class InputModel
	{
		public List<RawEntry> Entries;
	}


	public class InputModelParser
	{
		/// <summary>
		/// Represents a "generator" function that returns a list of areas given some user-defined parameters. 
		/// The generator is responsible for validating its own arguments and throwing ArgumentExceptions if necessary.
		/// </summary>
		/// <param name="arguments">Raw dictionary of parameters</param>
		/// <returns>List of areas</returns>
		public delegate List<UInt32> GeneratorDelegate(IDictionary<string, object> arguments);

		static Dictionary<string, GeneratorDelegate> generators = new Dictionary<string, GeneratorDelegate>()
		{
			{"random", new GeneratorDelegate(random_generator) }
		};

		public class ParameterConstraint<T>
		{
			string key;

			public ParameterConstraint(string key) {
				this.key = key;
			}

			public T Get(IDictionary<string, object> arguments)
			{
				try
				{
					return Get<T>(arguments, this.key);
				} catch (ParsingError p)
				{
					throw new ArgumentException(string.Format("Parameter '{0}' parameter must be a {1}", key, typeof(T).FullName), p);
				}
			}
		}

		public class ParameterConstraint<TContainer, TValue>
		{
			string key;
			public ParameterConstraint(string key)
			{
				this.key = key;
			}
		}

		public static List<UInt32> random_generator(IDictionary<string, object> arguments)
		{
			System.Random gen = new System.Random();

			List<uint> areaSet = new List<uint>();

			if (!arguments.ContainsKey("area-set"))
			{
				for (uint i = 0; i < 16; i++)
				{
					areaSet.Add(i);
				}

			} else
			{
				var areas = new ParameterConstraint<IList<double>>("area-set").Get(arguments);
				foreach (var area in areas)
				{
					areaSet.Add((uint)area);
				}
			}

			int count = 1;
			if (!arguments.ContainsKey("count"))
			{
				//do nothing, 1 is a good default
			} else {
				count = (int) new ParameterConstraint<double>("count").Get(arguments);
			}

			

			List<uint> result = new List<uint>();
			while (result.Count < count)
			{
				var pick = gen.Next((int)areaSet.Min(), (int)areaSet.Max());
				if (areaSet.Contains((uint)pick)) {
					result.Add((uint)pick);
				}
			}

			return result;
		}

	
		public static List<UInt32> Invoke(string generatorName, IDictionary<string, object> arguments)
		{
			if (!generators.ContainsKey(generatorName))
			{
				throw new ArgumentException(string.Format("Could not find any generators of the name '{0}'", generatorName));
			}

			return generators[generatorName](arguments);
		}

		public static T Get<T>(IDictionary<string, object> dict, string key)
		{
			object outVal = new object();
			if (!dict.TryGetValue(key, out outVal))
			{
				throw new ParsingError(string.Format("Could not find required parameter '{0}'", key));
			}

			if (outVal == null)
			{
				throw new ParsingError(string.Format("Parameter '{0}' was null", key));
			}

			try
			{
				T casted = (T)outVal;
				return casted;
			}
			catch (InvalidCastException)
			{
				throw new ParsingError(string.Format("Parameter '{0}' was not of type {1} as expected", key, typeof(T).FullName));
			}
		}

		public static InputModel Parse(string json)
		{
			var dict = MiniJSON.Json.Deserialize(json) as IDictionary<string, object>;
			object pattern = null;
			if (!dict.TryGetValue("pattern", out pattern))
			{
				throw new ParsingError("Couldn't find required key 'pattern'");
			}

			return Parse(pattern as IList<object>);
		}
		public static RawEntry Parse(IDictionary<string, object> rawEntry)
		{
			RawEntry entry = new RawEntry();
			entry.Time =(float) Get<double>(rawEntry, "time");
			entry.Sequence = Get<string>(rawEntry, "sequence");
			entry.AreaGenerator = Get<string>(rawEntry, "sequence");
			entry.AreaGeneratorArgs = Get<IDictionary<string, object>>(rawEntry, "params");

			if (entry.AreaGeneratorArgs == null)
			{
				throw new ParsingError("Parameters list was malformed json");
			}

			return entry;

		}
		private static InputModel Parse(IList<object> list)
		{
			InputModel model = new InputModel();
			model.Entries = new List<RawEntry>();

			for (int i = 0; i < list.Count; i++)
			{

				if (list[i] == null)
				{
					throw new ParsingError((string.Format("Entry [{0}] was malformed json", i)));
				}
				try
				{
					model.Entries.Add(Parse(list[i] as IDictionary<string, object>));
				} catch(ParsingError error)
				{
					throw new ParsingError(string.Format("In entry [{0}]: {1} ", i, error.Message), error);
				}

			}

			
			return model;
		}
	}
}
