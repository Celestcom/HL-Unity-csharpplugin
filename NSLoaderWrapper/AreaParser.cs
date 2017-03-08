using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NullSpace.SDK
{
	internal class AreaParser
	{
		private AreaFlag _area;
		public AreaParser(string tokens)
		{
			this._area = AreaFlag.None;
			parse(tokens);
		}
		public AreaFlag GetArea()
		{
			return _area;
		}
		private void parse(string areaString)
		{
			string[] tokens = areaString.Split('|');
			foreach (var possibleArea in tokens)
			{
				var cleanedUpArea = possibleArea.Trim();
				try
				{
					var a = (AreaFlag)Enum.Parse(typeof(AreaFlag), cleanedUpArea);
					_area |= a;
				}
				catch (ArgumentException e)
				{
					//do not add this unrecognized area flag
				}
			}
		}
	}
}
