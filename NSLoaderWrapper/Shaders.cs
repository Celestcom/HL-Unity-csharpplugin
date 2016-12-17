using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading;
using NullSpace.HapticFiles;
namespace NullSpace.SDK
{
	
	public class Area
	{
		public IList<AreaFlag> Areas
		{
			get { return _areas; }
		}
		private IList<AreaFlag> _areas;
		public Area(AreaFlag f)
		{
			_areas = new List<AreaFlag>();
			_areas.Add(f);
		}

		public Area(IList<AreaFlag> list)
		{
			_areas = list;
		}
		public Area(params AreaFlag[] list)
		{
			_areas = new List<AreaFlag>();
			for (int i = 0; i < list.Length; i++)
			{
				_areas.Add(list[i]);
			}
		}

		public static Area None = new Area(AreaFlag.None);

	}

	
	public abstract class HapticPrimitiveGenerator
	{
		private IDictionary<string, object> _parameters;
		public void SetParams(IDictionary<string, object> inputs)
		{
			_parameters = inputs;
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
			} catch (KeyNotFoundException)
			{
				Debug.Log("The parameter named " + key + " was not found in the shader");
				return default(TVal);
			}
		}

		abstract public Area Compute(float time);
	}


	public class FanWindGenerator : HapticPrimitiveGenerator
	{
		public FanWindGenerator()
		{

		}
		public override Area Compute(float time)
		{
			bool enabled = GetParam<bool>("enabled");
			if (!enabled)
			{
				return Area.None;
			}

			AreaFlag closest = AreaFlag.Chest_Both;
			return new Area(closest);
		}
	}
	public abstract class AreaShader
	{
		private IDictionary<string, object> _parameters;

		public AreaShader SetInputs(IDictionary<string, object> inputs)
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

		public abstract CodeEffect Shade();
	}
	public class GenericStrengthShader : AreaShader
	{
		
		public GenericStrengthShader()
		{

		}

		public override CodeEffect Shade()
		{
			float strength = GetParam<float>("strength");
			//internal engine flag
			AreaFlag area = GetParam<AreaFlag>("area");
			string effect = GetParam<string>("effect");
			return new CodeEffect(0, effect, 0, strength, area);

		}
	}


	public class ShaderProgram
	{
		HapticPrimitiveGenerator _generator;
		AreaShader _shader;
		Thread _shaderThread;
		float _currentTime;
		bool _shouldStop;
		public ShaderProgram(HapticPrimitiveGenerator generator, AreaShader shader)
		{
			_generator = generator;
			_shader = shader;
			_shaderThread = new Thread(new ThreadStart(this.Main));
			_shouldStop = false;


		}
		public void Execute()
		{
			_shaderThread.Start();
		}
		public void Destroy()
		{
			_shouldStop = true;
			_shaderThread.Join(50);
			_shaderThread.Abort();
		}

		private void Main()
		{
			while (!_shouldStop)
			{
				Evaluate();
			}
		}
		private void Evaluate()
		{
			_currentTime = Time.realtimeSinceStartup;
			Debug.Log("Time is " + _currentTime);

			Area areas = _generator.Compute(_currentTime);

			//our *pixels*

			List<CodeEffect> effects = areas.Areas.Select(
				area => _shader.SetInputs(new Dictionary<string, object>()
				{
					{ "strength", 1.0},
					{ "area", area},
					{ "effect", "click" }

				}).Shade())
			.ToList();

			var bytes = Encode(effects);
			var handle = NullSpace.SDK.Internal.Interop.NSVR_GenHandle(NSVR.NSVR_Plugin.Ptr);
			NullSpace.SDK.Internal.Interop.NSVR_CreateHaptic(NSVR.NSVR_Plugin.Ptr, handle, bytes, (uint)bytes.Length);
		}

		private byte[] Encode(IList<CodeEffect> effects)
		{
			var builder = new FlatBuffers.FlatBufferBuilder(128);
			var offsets = effects.Select(codeEffect => codeEffect.Generate(builder)).ToArray();


			var children = Node.CreateChildrenVector(builder, offsets);
			Node.StartNode(builder);
			Node.AddType(builder, NodeType.Pattern);
			Node.AddTime(builder, 0);
			Node.AddChildren(builder, children);
			var rootNode = Node.EndNode(builder);
			builder.Finish(rootNode.Value);
			return builder.SizedByteArray();

		}
	}
}
