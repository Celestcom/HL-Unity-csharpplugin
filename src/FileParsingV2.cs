using System;

using Hardlight.Events;
using System.Linq;
using System.Collections.Generic;
using Hardlight.SDK.FileUtilities;
using UnityEngine;

namespace Hardlight.SDK.FileUtilities
{
	/// <summary>
	/// Represents a discriminated union of all possible area types
	/// Probably overkill for now..
	/// </summary>
	public class Area
	{
		public enum AreaType
		{
			Unknown,
			Region,
			AreaFlag
		}

		private AreaType tag;
		private List<int> regions;
		private AreaFlag flag;

		public AreaType Tag
		{
			get { return tag; }
		}

		/// <summary>
		/// Default construct an Area with value AreaFlag.None
		/// </summary>
		public Area()
		{
			tag = AreaType.AreaFlag;
			flag = AreaFlag.None;
		}

		/// <summary>
		/// Construct an Area from a given AreaFlag
		/// </summary>
		/// <param name="areaFlag"></param>
		public Area(AreaFlag areaFlag)
		{
			tag = AreaType.AreaFlag;
			flag = areaFlag;
		}

		/// <summary>
		/// Construct an Area from a given list of Regions
		/// </summary>
		/// <param name="regions"></param>
		public Area(List<int> regions)
		{
			tag = AreaType.Region;
			this.regions = new List<int>(regions);
		}

		/// <summary>
		/// Retrieve the value of the Area
		/// </summary>
		/// <param name="areaFlag"></param>
		/// <param name="regions"></param>
		public void Get(Action<AreaFlag> areaFlag, Action<List<int>> regions)
		{
			switch (tag)
			{
				case AreaType.AreaFlag:
					areaFlag(flag);
					break;
				case AreaType.Region:
					regions(this.regions);
					break;
				default:
					break;
			}
		}
	}

	public interface IGenerator
	{
		List<long> Generate();
	}




	/// <summary>
	/// How I think ScriptableGenerator will look like
	/// </summary>
	public class RandomGenerator : IGenerator
	{

		//[SerializeField] probably goes here, with some UI constraints for min and max
		public int Count;

		//[SerializeField] probably goes here..
		public List<long> Regions;


		private System.Random random;


		public RandomGenerator(IDictionary<string, object> arguments)
		{
			random = new System.Random();

			
			Regions = new JsonParameter<long>("area-set").GetList(arguments, Enumerable.Range(1, 16).ToList().Select(x => (long)x).ToList());
			Count = (int) new JsonParameter<long>("count").Get(arguments, 1);
			
		}

		public RandomGenerator() : this(null)
		{
		}

		public List<long> Generate()
		{
			var result = new List<long>();
			while (result.Count < Count)
			{
				var randomIndex = random.Next(0, Regions.Count);
				result.Add(Regions[randomIndex]);
			}
			return result;
		}
	}

	public class ParsingError : Exception
	{
		public ParsingError(string message) : base(message) { }
		public ParsingError(string message, Exception inner) : base(message, inner) { }
	}

	/// <summary>
	/// First representation of the generic json data within a pattern. Leaves the arguments for a generator
	/// for later parsing. 
	/// </summary>
	public class RawEntry
	{
		public double Time;
		public string Sequence;
		public string AreaGenerator;
		public IDictionary<string, object> AreaGeneratorArgs;
	}

	/// <summary>
	/// Second representation of json data within a pattern. Arguments for generator have been parsed, submitted to generator,
	/// and result in a list of Regions. 
	/// </summary>
	public class PatternEntry
	{
		public double Time;
		public string Sequence;
		public IGenerator Generator;

		public PatternEntry(RawEntry raw, IGenerator gen)
		{
			Time = raw.Time;
			Sequence = raw.Sequence;
			Generator = gen;
		}
	}


	public class ScriptablePatternData
	{
		public List<PatternEntry> Entries;
	}

	public class JsonParameter<T>
	{
		private string key;

		public JsonParameter(string key)
		{
			this.key = key;
		}

		public List<T> GetList(IDictionary<string, object> arguments, List<T> defaultVal)
		{

			if (arguments == null || !arguments.ContainsKey(key))
			{
				return defaultVal;
			}

			try
			{
				ScriptablePatternParser.GetList(arguments, key, out defaultVal);
				return defaultVal;
			}
			catch (ParsingError p)
			{
				throw new ArgumentException(string.Format("Parameter '{0}' parameter must be a {1}", key, typeof(T).FullName), p);
			}
		}

	
		public T Get(IDictionary<string, object> arguments, T defaultVal)
		{
			if (arguments == null || !arguments.ContainsKey(key))
			{
				return defaultVal;
			}

			try
			{
				ScriptablePatternParser.Get(arguments, key, out defaultVal);
				return defaultVal;
			}
			catch (ParsingError p)
			{
				throw new ArgumentException(string.Format("Parameter '{0}' parameter must be a {1}", key, typeof(T).FullName), p);
			}
		}
	}

	/// <summary>
	/// Responsible for turning a raw JSON representation of a ScriptablePattern into an intermediate format
	/// suitable for creating a real ScriptablePattern
	/// </summary>
	public class ScriptablePatternParser
	{
		internal delegate IGenerator IGeneratorFactory(IDictionary<string, object> arguments);

		internal static Dictionary<string, IGeneratorFactory> generators = new Dictionary<string, IGeneratorFactory>()
		{
			//Add new generator factory functions here
			{"random", (args) => { return new RandomGenerator(args); } }
		};

		internal static ScriptablePatternData Parse(IList<object> list)
		{
			ScriptablePatternData model = new ScriptablePatternData();
			model.Entries = new List<PatternEntry>();

			for (int i = 0; i < list.Count; i++)
			{
				try
				{
					model.Entries.Add(ParseEntry(list[i] as IDictionary<string, object>));
				}
				catch (ParsingError error)
				{
					throw new ParsingError(string.Format("In entry [{0}], while parsing: {1} ", i, error.Message), error);
				}
			}
			return model;
		}


		internal static PatternEntry ParseEntry(IDictionary<string, object> rawData)
		{
			if (rawData == null)
			{
				throw new ParsingError("Malformed json");
			}


			RawEntry rawEntry = ParseRawEntry(rawData);

			try
			{
				var generator = MakeGenerator(rawEntry.AreaGenerator, rawEntry.AreaGeneratorArgs);
				return new PatternEntry(rawEntry, generator);
			}
			catch (ArgumentException error)
			{
				throw new ParsingError(string.Format("While constructing generator: {0}", error.Message), error);
			}
		}


		internal static RawEntry ParseRawEntry(IDictionary<string, object> rawEntry)
		{
			RawEntry entry = new RawEntry();
			Get(rawEntry, "time", out entry.Time);
			Get(rawEntry, "sequence", out entry.Sequence);
			Get(rawEntry, "area", out entry.AreaGenerator);
			Get(rawEntry, "params", out entry.AreaGeneratorArgs);
			return entry;
		}


		internal static IGenerator MakeGenerator(string generatorName, IDictionary<string, object> arguments)
		{

			if (!generators.ContainsKey(generatorName))
			{
				throw new ArgumentException(string.Format("Could not find any generators of the name '{0}'", generatorName));
			}

			return generators[generatorName](arguments);
		}

		internal static object GetRawObject(IDictionary<string, object> arguments, string key)
		{
			object outVal = new object();
			if (!arguments.TryGetValue(key, out outVal))
			{
				throw new ParsingError(string.Format("Could not find required parameter '{0}'", key));
			}

			if (outVal == null)
			{
				throw new ParsingError(string.Format("Parameter '{0}' was null, malformed json?", key));
			}

			return outVal;
		}

		internal static void GetList<T>(IDictionary<string, object> dict, string key, out List<T> value)
		{
			object outVal = GetRawObject(dict, key);
			try
			{
				List<object> temp = (List<object>)outVal;
				value = temp.Select(x => (T)x).ToList();
			}
			catch (InvalidCastException)
			{
				throw new ParsingError(string.Format("Parameter '{0}' was not of type {1} as expected", key, typeof(T).FullName));
			}
		}

		internal static void Get<T>(IDictionary<string, object> dict, string key, out T value)
		{
			object outVal = GetRawObject(dict, key);
			try
			{
				value = (T)outVal;
			}
			catch (InvalidCastException)
			{
				throw new ParsingError(string.Format("Parameter '{0}' was not of type {1} as expected", key, typeof(T).FullName));
			}
		}

		public static ScriptablePatternData Parse(string json)
		{
			var dict = MiniJSON.Json.Deserialize(json) as IDictionary<string, object>;
			object pattern = null;

			if (!dict.TryGetValue("pattern", out pattern))
			{
				throw new ParsingError("Couldn't find required key 'pattern'");
			}

			return Parse(pattern as IList<object>);
		}
	}
}
