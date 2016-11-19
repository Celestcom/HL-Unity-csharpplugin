﻿using System;
using NullSpace.SDK.Internal;
using static NullSpace.SDK.Internal.Interop;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using NullSpace.HapticFiles;
using FlatBuffers;
using NullSpace.HapticFiles.Mixed;

namespace NullSpace.SDK
{

	public class HapticsLoadingException : System.Exception
	{
		public HapticsLoadingException() : base() { }
		public HapticsLoadingException(string message) : base(message) { }
		public HapticsLoadingException(string message, System.Exception inner) : base(message, inner) { }

	
		protected HapticsLoadingException(System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
		{ }
	}


	public static class NSVR
	{
		internal static IntPtr _ptr;
		internal static bool _created = false;
		internal static string GetError()
		{
			IntPtr ptr = Interop.NSVR_GetError(NSVR.NSVR_Plugin.Ptr);
			string s = Marshal.PtrToStringAnsi(ptr);
			Interop.NSVR_FreeString(ptr);
			return s;
		}

		/// <summary>
		/// Retrieves a reference to a resource (will be loaded from disk if not yet loaded.)
		/// Call CreateHandle() on resource to play and modify haptic (see HapticHandle.)
		/// Can also be inserted into code created haptics. Ex: when creating a CodePatternItem, provide a
		/// HapticRef as the second argument
		/// </summary>
		/// <typeparam name="T">Sequence, Pattern, or Experience</typeparam>
		/// <param name="id">Name of the resource. Ex: ns.click</param>
		/// <returns>Reference to the resource</returns>
		public static T HapticRef<T>(string id) where T : IPlayable, new()
		{
			T haptic = new T();
			haptic.Load(id);
			haptic.Name = id;
			return haptic;
		}

		

		public sealed class NSVR_Plugin : IDisposable
		{

			internal static IntPtr Ptr
			{
				get
				{
					if (_created)
					{

						return _ptr;
					}
					else
					{
						throw new ArgumentException("[NSVR] You may not initialize Sequences, Patterns, and Experiences at their point of declaration (please do it inside of Start(), Awake(), or other runtime methods)");

					}

				}
			}
			public NSVR_Plugin(string path)
			{
				if (_created)
				{
					Debug.LogWarning("[NSVR] NSVR_Plugin should only be created by the NullSpace SDK");
				
				} 
				_ptr = Interop.NSVR_Create(path);
				_created = true;

			}

			


			public int PollStatus()
			{
				return Interop.NSVR_PollStatus(_ptr);
			}

			public void SetTrackingEnabled(bool wantTracking)
			{
				Interop.NSVR_SetTrackingEnabled(_ptr, wantTracking);
			}

			public TrackingUpdate PollTracking()
			{
				TrackingUpdate t = new TrackingUpdate();
				Interop.NSVR_PollTracking(_ptr, ref t);
				return t;
			}

			#region IDisposable Support
			private bool disposedValue = false; // To detect redundant calls

			void Dispose(bool disposing)
			{
				if (!disposedValue)
				{
					if (disposing)
					{
						// TODO: dispose managed state (managed objects).
					}

					// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
					// TODO: set large fields to null.
					Interop.NSVR_Delete(_ptr);

					disposedValue = true;
				}
			}

			// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
			~NSVR_Plugin()
			{
				//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
				Dispose(false);
			}

			// This code added to correctly implement the disposable pattern.
			public void Dispose()
			{
				// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
				Dispose(true);
				// TODO: uncomment the following line if the finalizer is overridden above.
				GC.SuppressFinalize(this);
			}
			#endregion


		}
	}
	
	public sealed class HapticHandle : IDisposable
	{
		/// <summary>
		/// Retain the effect name, so that someone calling ToString() can get useful information
		/// </summary>
		private string _effectName;
		/// <summary>
		/// Handle is used to interact with the engine
		/// </summary>
		private uint _handle;
		/// <summary>
		/// _playDelegate contains knowledge of how to play the effect
		/// </summary>
		private CommandWithHandle _playDelegate;
		/// <summary>
		/// _pauseDelegate contains knowledege of how to pause the effect
		/// </summary>
		private CommandWithHandle _pauseDelegate;
		/// <summary>
		/// _resetDelegate contains knowledge of how to reset the effect
		/// </summary>
		private CommandWithHandle _resetDelegate;

		internal HapticHandle(CommandWithHandle play, CommandWithHandle pause, CommandWithHandle create, CommandWithHandle reset, string name)
		{
			_effectName = name;
			_playDelegate = play;
			_pauseDelegate = pause;
			_resetDelegate = reset;

			//Grab a handle from the DLL
			_handle = Interop.NSVR_GenHandle(NSVR._ptr);
			//This is a network call to the engine, to load the effect up
			create(_handle);
		}

		
		public HapticHandle Play()
		{
			_playDelegate(_handle);
			return this;
		}

		public HapticHandle Reset()
		{
			_resetDelegate(_handle);
			return this;
		}

		public HapticHandle Pause()
		{
			_pauseDelegate(_handle);
			return this;
		}

		public override string ToString()
		{
			return string.Format("Handle ID {0} playing effect {1}", _handle, _effectName);
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}
				Interop.NSVR_HandleCommand(NSVR.NSVR_Plugin.Ptr, _handle, (short)Interop.Command.RELEASE);
				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		 ~HapticHandle() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			  Dispose(false);
		 }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			 GC.SuppressFinalize(this);
		}
		#endregion
	}
	
	public abstract class Playable
	{

		internal Playable() { }
		internal static CommandWithHandle GenerateCommandDelegate(Interop.Command c)
		{
			return new CommandWithHandle(x => Interop.NSVR_HandleCommand(NSVR.NSVR_Plugin.Ptr, x, (short)c));
		}

		internal CommandWithHandle _Play()
		{
			return GenerateCommandDelegate(Interop.Command.PLAY);
		}

		internal CommandWithHandle _Reset()
		{
			return GenerateCommandDelegate(Interop.Command.RESET);
		}

		internal CommandWithHandle _Pause()
		{
			return GenerateCommandDelegate(Interop.Command.PAUSE);
		}


	}

	public interface IPlayable
	{
		void Load(string id);

	}
	public interface ISaveable
	{
		void SaveAs(string id);
	}
	public class Sequence : AbstractSequence, IPlayable
	{
		private string _name;
		
		public Sequence(string name)
		{
			_name = name;
			
			bool loaded = Interop.NSVR_LoadSequence(NSVR.NSVR_Plugin.Ptr, name);
			
			if (!loaded)
			{
				throw new HapticsLoadingException(NSVR.GetError());
					
			}
		}
		override
		internal Offset<HapticFiles.Mixed.Sequence> Encode(FlatBufferBuilder builder, AreaFlag area)
		{
			var seqRef = HapticFiles.Mixed.SeqRef.CreateSeqRef(builder, builder.CreateString(_name));
			var finishedSeq = HapticFiles.Mixed.Sequence.CreateSequence(builder, (uint)area, EffectValuesOrNameReference.SeqRef, seqRef.Value);
			return finishedSeq;
		}
		public Sequence()
		{

		}
		private CommandWithHandle _create(uint location)
		{
			return new CommandWithHandle(handle => Interop.NSVR_CreateSequence(NSVR.NSVR_Plugin.Ptr, handle, _name, location));
		}

		public HapticHandle CreateHandle(AreaFlag location)
		{

			return new HapticHandle(_Play(), _Pause(), _create((uint)location), _Reset(), _name);

		}


		public void Load(string id)
		{
			_name = id;

			bool loaded = Interop.NSVR_LoadSequence(NSVR.NSVR_Plugin.Ptr, _name);

			if (!loaded)
			{
				throw new HapticsLoadingException(NSVR.GetError());

			}
		}

		
	}

	public class Pattern : Playable
	{
		private string _name;
		public Pattern(string name)
		{
			_name = name;
		
			bool loaded = Interop.NSVR_LoadPattern(NSVR.NSVR_Plugin.Ptr, name);
			if (!loaded)
			{
				throw new HapticsLoadingException(NSVR.GetError());

			}
		}

		private CommandWithHandle _create()
		{
			return new CommandWithHandle(handle => Interop.NSVR_CreatePattern(NSVR.NSVR_Plugin.Ptr, handle, _name));
		}

		public HapticHandle CreateHandle()
		{
			return new HapticHandle(_Play(), _Pause(), _create(), _Reset(), _name);
		}

		public void LoadFromExisting(string id)
		{
			throw new NotImplementedException();
		}
	}

	public class Experience : Playable
	{
		private string _name;
		public Experience(string name)
		{
			_name = name;

			bool loaded = Interop.NSVR_LoadExperience(NSVR.NSVR_Plugin.Ptr, name);
			if (!loaded)
			{
				throw new HapticsLoadingException(NSVR.GetError());

			}
		}

		private CommandWithHandle _create()
		{
			return new CommandWithHandle(handle => Interop.NSVR_CreateExperience(NSVR.NSVR_Plugin.Ptr, handle, _name));
		}
		public HapticHandle CreateHandle()
		{
			return new HapticHandle(_Play(), _Pause(), _create(), _Reset(), _name);
		}

		public void LoadFromExisting(string id)
		{
			throw new NotImplementedException();
		}
	}


	



}
