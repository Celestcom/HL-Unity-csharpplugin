using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading;
using NullSpace.HapticFiles;
using NullSpace.SDK.Internal;
using FlatBuffers;

namespace NullSpace.SDK
{

	public class Area
	{
		public IList<AreaAttribute> Areas
		{
			get { return _areas; }
		}
		private IList<AreaAttribute> _areas;
		public Area(AreaAttribute f)
		{
			_areas = new List<AreaAttribute>();
			_areas.Add(f);
		}


		public Area(params AreaAttribute[] list)
		{
			_areas = new List<AreaAttribute>();
			for (int i = 0; i < list.Length; i++)
			{
				_areas.Add(list[i]);
			}
		}

		public static Area None
		{
			get
			{
				return new Area();
			}
		}
	}
	public class AreaAttribute
	{
		public AreaFlag Area
		{
			get { return _flag; }
		}
		private AreaFlag _flag;
		private IDictionary<string, object> _parameters;
		public AreaAttribute(AreaFlag flag, IDictionary<string, object> paras)
		{
			_parameters = paras;
			_flag = flag;
		}
		public TVal GetParam<TVal>(string key)
		{
			try
			{
				return (TVal)_parameters[key];
			}
			catch (KeyNotFoundException)
			{
				Debug.Log("The parameter named " + key + " was not found in the shader");
				return default(TVal);
			}
		}
	}

	public abstract class AreaGenerator
	{
		private IDictionary<string, object> _parameters;
		public void SetParams(IDictionary<string, object> inputs)
		{
			_parameters = inputs;
		}

		public double SquareWave(double hz, double time)
		{
			double T = 1.0 / hz;
			double t = 0.2;
			double accum = 0.0;

			for (int i = 1; i < 10; i++)
			{
				double eval = (2.0 / (i * Math.PI)) * Mathf.Sin((float)((Math.PI * i * t) / T))
					* Mathf.Cos((float)(((Math.PI * 2.0 * i) / T) * time));
				accum += eval;
			}

			double res = (t / T) + accum;
			if (res > .7)
			{
				return 1.0;
			}
			else
			{
				return 0.0;
			}

		}
		public void SetParam(string key, object value)
		{
			if (_parameters == null)
			{
				_parameters = new Dictionary<string, object>();
			}

			_parameters[key] = value;
		}

		protected TVal GetParam<TVal>(string key)
		{
			try
			{
				return (TVal)_parameters[key];
			}
			catch (KeyNotFoundException)
			{
				Debug.Log("The parameter named " + key + " was not found in the shader");
				return default(TVal);
			}
		}

		abstract public Area Compute(double time);
	}

	public static class Extensions
	{
		public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
		{
			if (val.CompareTo(min) < 0) return min;
			else if (val.CompareTo(max) > 0) return max;
			else return val;
		}
	}
	/*
	public class FanWindGenerator : AreaGenerator
	{
		public AreaAttribute[] Mix(AreaFlag a, AreaFlag b, double t)
		{
			t = Extensions.Clamp(t, 0.0, 1.0);
			if (t == 1.0)
			{
				return new AreaAttribute[] { new AreaAttribute(b, new Dictionary<string, object>() { { "strength", 1.0 } }) };
			}
			if (t == 0.0)
			{
				return new AreaAttribute[] { new AreaAttribute(a, new Dictionary<string, object>() { { "strength", 1.0 } }) };

			}
			else
			{
				return new AreaAttribute[] {
					new AreaAttribute(a, new Dictionary<string, object>() { { "strength", 1.0 - t } }),
					new AreaAttribute(b, new Dictionary<string, object>() { { "strength", t } })
				};
			}
		}


		public FanWindGenerator()
		{

		}
		public override Area Compute(double time)
		{
			bool enabled = GetParam<bool>("enabled");
			if (!enabled)
			{
				return Area.None;
			}
			//cwb 4308
			var buckets = new AreaFlag[] { AreaFlag.Chest_Left, AreaFlag.Chest_Both, AreaFlag.Chest_Both | AreaFlag.Mid_Ab_Both, AreaFlag.Chest_Both | AreaFlag.Mid_Ab_Both | AreaFlag.Lower_Ab_Both };
			//intersect(User.Torso, vec3 windDirection, vec3 origin);

			//var outParams = new Dictionary<string, object>() { { "strength", Math.Sin(time*.5) } };
			return new Area(Mix(AreaFlag.Chest_Left, AreaFlag.Chest_Right, (Math.Sin(time) + 1.0) / 2.0));
		}
	}
	*/
	/*
	public class CutaneousRabbit : AreaGenerator
	{
		private AreaFlag _nodeA;
		private AreaFlag _nodeB;
		public CutaneousRabbit(AreaFlag a, AreaFlag b)
		{
			_nodeA = a;
			_nodeB = b;
		}
		public override Area Compute(double time)
		{
			double frequency = GetParam<double>("frequency");
			double value1 = SquareWave(frequency, time);
			double value2 = SquareWave(frequency, time + (1.0 / frequency / 2.0));
			Area a = Area.None;
			if (value1 == 1.0)
			{

				a.Areas.Add(new AreaAttribute(_nodeA, new Dictionary<string, object>() { { "strength", 1.0 } }));


			}
			if (value2 == 1.0)
			{
				a.Areas.Add(new AreaAttribute(_nodeB, new Dictionary<string, object>() { { "strength", 1.0 } }));
			}

			return a;
		}
	}
	*/
	public abstract class HapticShader
	{
		private IDictionary<string, object> _parameters;

		internal HapticShader SetInputs(IDictionary<string, object> inputs)
		{
			_parameters = inputs;
			return this;
		}

		protected TVal GetParam<TVal>(string key)
		{
			try
			{
				return (TVal)_parameters[key];
			}
			catch (KeyNotFoundException)
			{
				Debug.Log("The parameter named " + key + " was not found in the shader");
				return default(TVal);
			}
		}

		internal abstract CodeEffect Shade();
	}
	/*
	public class GenericStrengthShader : HapticShader
	{

		public GenericStrengthShader()
		{

		}

		internal override CodeEffect Shade()
		{
			double strength = GetParam<double>("strength");
			//internal engine flag
			AreaFlag area = GetParam<AreaFlag>("area");
			string effect = GetParam<string>("effect");

			//todo:fix
			//return new CodeEffect(0, effect, 0.0f, strength, area);
			return new CodeEffect(0f, "click", 0f);
		}
	}

	*/

	public interface ITimeProvider
	{
		double GetTimeSinceStartup();
	}
	internal class DefaultTimeProvider : ITimeProvider
	{
		private System.Diagnostics.Stopwatch _watch;
		public DefaultTimeProvider()
		{
			_watch = new System.Diagnostics.Stopwatch();
			_watch.Start();
		}

		public double GetTimeSinceStartup()
		{

			return _watch.Elapsed.TotalSeconds;
		}
	}
	public class ShaderProgram
	{

		AreaGenerator _generator;
		HapticShader _shader;
		Thread _shaderThread;
		bool _shouldStop;
		ITimeProvider _time;
		uint _prevHandle;
		public ShaderProgram(AreaGenerator generator, HapticShader shader)
		{
			_generator = generator;
			_shader = shader;
			_shouldStop = false;
			_shaderThread = new Thread(new ThreadStart(this.ThreadMain));
			_shaderThread.IsBackground = true;

			_shaderThread.Name = "Shader thread";

		}
		public void UseTimeProvider(ITimeProvider p)
		{
			_time = p;
		}
		public void Execute()
		{

			_shaderThread.Start();

		}
		public void Execute(float t)
		{

		}
		public void Destroy()
		{
			_shouldStop = true;
			_shaderThread.Join(50);
			_shaderThread.Abort();
		}
		public void Destroy(float t)
		{

		}
		internal void ThreadMain()
		{
			while (!_shouldStop)
			{
				Interop.NSVR_HandleCommand(NSVR.NSVR_Plugin.Ptr, _prevHandle, (short)Command.PAUSE);
				Interop.NSVR_HandleCommand(NSVR.NSVR_Plugin.Ptr, _prevHandle, (short)Command.RELEASE);
				Evaluate();
				System.Threading.Thread.Sleep(50);

			}
			Interop.NSVR_HandleCommand(NSVR.NSVR_Plugin.Ptr, _prevHandle, (short)Command.PAUSE);
			Interop.NSVR_HandleCommand(NSVR.NSVR_Plugin.Ptr, _prevHandle, (short)Command.RELEASE);

		}

		private void Evaluate()
		{
			//Console.WriteLine("Time is " + _time.GetTimeSinceStartup());


			Area areas = _generator.Compute(_time.GetTimeSinceStartup());

			//our *pixels*

			List<CodeEffect> effects = areas.Areas.Select(
				area => _shader.SetInputs(new Dictionary<string, object>()
				{
					{ "strength", area.GetParam<double>("strength")},
					{ "area", area.Area},
					{ "effect", "click" }

				}).Shade())
			.ToList();
			//todo: Fix shader implementation now that have CodeSequence, CodePattern etc.
			/*
			if (effects.Count > 0)
			{
				var handle = Interop.NSVR_GenHandle(NSVR.NSVR_Plugin.Ptr);

				var bytes = EncodingUtils.Encode(effects, handle);
				Interop.NSVR_CreateHaptic(NSVR.NSVR_Plugin.Ptr, handle, bytes, (uint)bytes.Length);
				Interop.NSVR_HandleCommand(NSVR.NSVR_Plugin.Ptr, handle, (short)Command.PLAY);
				_prevHandle = handle;
			}
			*/
		}


	}

}

