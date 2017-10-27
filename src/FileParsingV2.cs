using System;

using Hardlight.Events;
using System.Linq;
using System.Collections.Generic;
using Hardlight.SDK.FileUtilities;
using UnityEngine;

namespace Hardlight.SDK.FileUtilities
{
	
	public class Area
	{
		public List<int> Regions;
		public AreaFlag AreaFlag;
	}

	public class ParsingError : Exception
	{
		public ParsingError(string message) : base(message) { }
		public ParsingError(string message, Exception inner) : base(message, inner) { }
	} 

	public class RawEntry
	{
		public double Time;
		public string Sequence;
		public string AreaGenerator;
		public IDictionary<string, object> AreaGeneratorArgs; 
	}

	public class PatternEntry
	{
		public double Time;
		public string Sequence;
		public List<int> Area;
	}

	public class InputModel
	{
		public List<PatternEntry> Entries;
	}


	public class InputModelParser
	{
		/// <summary>
		/// Represents a "generator" function that returns a list of areas given some user-defined parameters. 
		/// The generator is responsible for validating its own arguments and throwing ArgumentExceptions if necessary.
		/// </summary>
		/// <param name="arguments">Raw dictionary of parameters</param>
		/// <returns>List of areas</returns>
		public delegate List<int> GeneratorDelegate(IDictionary<string, object> arguments);

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

			public T GetOr(IDictionary<string, object> arguments, T defaultVal)
			{
				if (!arguments.ContainsKey(key))
				{
					return defaultVal;
				}

				try
				{
					Get(arguments, key, out defaultVal);
					return defaultVal;
				} catch (ParsingError p)
				{
					throw new ArgumentException(string.Format("Parameter '{0}' parameter must be a {1}", key, typeof(T).FullName), p);
				}
			}
		}

	

		public static List<int> random_generator(IDictionary<string, object> arguments)
		{
			System.Random gen = new System.Random();


			var areas = new ParameterConstraint<List<long>>("area-set").GetOr(arguments, Enumerable.Range(1, 16).ToList().Select(x => (long)x).ToList());

			int count = (int) new ParameterConstraint<long>("count").GetOr(arguments, 1);
			
			List<int> result = new List<int>();
			while (result.Count < count)
			{
				var pick = gen.Next((int)areas.Min(), (int)areas.Max() + 1);
				if (areas.Contains(pick)) {
					result.Add(pick);
				}
			}

			return result;
		}

	
		public static List<int> Invoke(string generatorName, IDictionary<string, object> arguments)
		{
			if (!generators.ContainsKey(generatorName))
			{
				throw new ArgumentException(string.Format("Could not find any generators of the name '{0}'", generatorName));
			}

			return generators[generatorName](arguments);
		}



		public static void Get<T>(IDictionary<string, object> dict, string key, out T value) where T: new()
		{
			object outVal = new object();
			if (!dict.TryGetValue(key, out outVal))
			{
				throw new ParsingError(string.Format("Could not find required parameter '{0}'", key));
			}

			if (outVal == null)
			{
				throw new ParsingError(string.Format("Parameter '{0}' was null, malformed json?", key));
			}

			try
			{
				if (typeof(T).GetInterface("IList") != null)
				{
					List<object> temp = (List<object>)outVal;
					value = new T();

					foreach (var item in temp)
					{
						value.Add(item);
					}
				}
				else
				{
					value = (T)outVal;
				}
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
			Get(rawEntry, "time", out entry.Time);
			Get(rawEntry, "sequence", out entry.Sequence);
			Get(rawEntry, "area", out entry.AreaGenerator);
			Get(rawEntry, "params", out entry.AreaGeneratorArgs);

			//entry.Time =(float) Get<double>(rawEntry, "time");
			//entry.Sequence = Get<string>(rawEntry, "sequence");
			//entry.AreaGenerator = Get<string>(rawEntry, "area");
			//entry.AreaGeneratorArgs = Get<IDictionary<string, object>>(rawEntry, "params");

		
			return entry;

		}
		private static InputModel Parse(IList<object> list)
		{
			InputModel model = new InputModel();
			model.Entries = new List<PatternEntry>();

			for (int i = 0; i < list.Count; i++)
			{

				if (list[i] == null)
				{
					throw new ParsingError((string.Format("Entry [{0}] was malformed json", i)));
				}
				try
				{
					RawEntry raw = Parse(list[i] as IDictionary<string, object>);

				
					var areas = Invoke(raw.AreaGenerator, raw.AreaGeneratorArgs);

					PatternEntry entry = new PatternEntry();
					entry.Time = raw.Time;
					entry.Sequence = raw.Sequence;
					entry.Area = areas;
					model.Entries.Add(entry);

				} catch(ParsingError error)
				{
					throw new ParsingError(string.Format("In entry [{0}]: {1} ", i, error.Message), error);
				}
				catch (ArgumentException error)
				{
					throw new ParsingError(string.Format("In entry [{0}]: {1} ", i, error.Message), error);
				}


			}

			
			return model;
		}
	}
}
