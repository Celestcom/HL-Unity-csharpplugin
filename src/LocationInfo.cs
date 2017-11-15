using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hardlight.SDK
{
	public interface ILocationInfo
	{

	}

	/// <summary>
	/// An incomplete stub class for handling the future feature of generators (such as randomization of which location to pick)
	/// </summary>
	public class GeneratorLocation : ILocationInfo
	{
		private string generator = "Derp";

		public string Generator
		{
			get
			{
				return generator;
			}

			set
			{
				generator = value;
			}
		}
	}
	public class AreaFlagLocation : ILocationInfo
	{
		private AreaFlag _area;
		public AreaFlag Area
		{
			get
			{
				return _area;
			}

			set
			{
				_area = value;
			}
		}
	}
}