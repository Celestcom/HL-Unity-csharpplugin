using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NullSpace.SDK
{

	public enum Effect
	{
		Bump = 1,
		Buzz = 2,
		Click = 3,
		Fuzz = 5,
		Hum = 6,
		Pulse = 8,
		Tick = 11,
		Double_Click = 4,
		Triple_Click = 16
	}

	/// <summary>
	/// Used to map the effect written in a .hdf into the enum based version. 
	/// Inside the engine this is translated back into a string, so this pipeline should be
	/// fixed up. We need to be backwards compatible. 
	/// </summary>
	internal static class FileEffectToCodeEffect
	{
		private static Dictionary<string, Effect> _effects;

		static FileEffectToCodeEffect()
		{
			_effects = new Dictionary<string, Effect>()
					{
						{"bump", Effect.Bump},
						{"buzz", Effect.Buzz },
						{"click", Effect.Click },
						{"sharp_click", Effect.Click},
						{"fuzz", Effect.Fuzz},
						{"hum", Effect.Hum},
						{"long_double_sharp_tick", Effect.Double_Click},
						{"pulse", Effect.Pulse},
						{"pulse_sharp", Effect.Pulse},
						{"sharp_tick", Effect.Tick },
						{"short_double_click", Effect.Double_Click},
						{"short_double_sharp_tick", Effect.Double_Click},
						{"transition_click", Effect.Click},
						{"transition_hum", Effect.Hum},
						{"triple_click", Effect.Triple_Click }
					};
		}

		/// <summary>
		/// Attempt to parse a string into an Effect, returning defaultEffect if fail
		/// </summary>
		/// <param name="effect">A potential effect</param>
		/// <param name="defaultEffect">The default to return if parsing fails</param>
		/// <returns></returns>
		public static Effect TryParse(string effect, Effect defaultEffect)
		{
		

			if (_effects.ContainsKey(effect))
			{
				return _effects[effect];
			}
			else
			{
				return defaultEffect;
			}
		}
	}
}
