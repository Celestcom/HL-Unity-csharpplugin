using System;
namespace NullSpace.Loader
{
	public class NSLoader : IDisposable
	{
		private static IntPtr _ptr;
		
		public delegate void CommandWithHandle(uint handle);
		
		public abstract class Playable
		{
			internal Playable() { }
			private static CommandWithHandle GenerateCommandDelegate(Interop.Command c)
			{
				return new CommandWithHandle(x => Interop.NSVR_HandleCommand(_ptr, x, (short)c));
			}

			protected CommandWithHandle _Play()
			{
				return GenerateCommandDelegate(Interop.Command.PLAY);
			}

			protected CommandWithHandle _Reset()
			{
				return GenerateCommandDelegate(Interop.Command.RESET);
			}

			protected CommandWithHandle _Pause()
			{
				return GenerateCommandDelegate(Interop.Command.PAUSE);
			}
		}
		public class HapticHandle
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

			public HapticHandle(CommandWithHandle play, CommandWithHandle pause, CommandWithHandle create, CommandWithHandle reset, string name)
			{
				_effectName = name;
				_playDelegate = play;
				_pauseDelegate = pause;
				_resetDelegate = reset;

				//Grab a handle from the DLL
				_handle = Interop.NSVR_GenHandle(_ptr);
				//This is a network call to the engine, to load the effect up
				create(_handle);
			}

			~HapticHandle()
			{
				//release this handle, network call to engine
			//	Interop.NSVR_HandleCommand(_ptr, _handle, (short)Interop.Command.RESET);

				Interop.NSVR_HandleCommand(_ptr, _handle, (short)Interop.Command.RELEASE);
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

			public override string ToString() {
				return string.Format("Handle ID {0} playing effect {1}", _handle, _effectName);
			}
		}
		public class Sequence : Playable
		{
			private string _name;
			public Sequence(string name)
			{
				_name = name;
				bool loaded = Interop.NSVR_LoadSequence(_ptr, name);
				if (!loaded)
				{
					throw new System.IO.FileNotFoundException("Could not find sequence " + name);
				}
			}
			private CommandWithHandle _create(uint location)
			{
				return new CommandWithHandle(handle => Interop.NSVR_CreateSequence(_ptr, handle, _name, location));
			}
			
			public HapticHandle CreateHandle(Interop.AreaFlag location)
			{

				return new HapticHandle(_Play(), _Pause(), _create((uint)location), _Reset(), _name);
				
			}
		}

		public class Pattern : Playable
		{
			private string _name;
			public Pattern(string name)
			{
				_name = name;
				bool loaded = Interop.NSVR_LoadPattern(_ptr, name);
				if (!loaded)
				{
					throw new System.IO.FileNotFoundException("Could not find pattern " + name);
				}
			}

			private CommandWithHandle _create()
			{
				return new CommandWithHandle(handle => Interop.NSVR_CreatePattern(_ptr, handle, _name));
			}

			public HapticHandle CreateHandle()
			{
				return new HapticHandle(_Play(), _Pause(), _create(), _Reset(), _name);
			}
		}


		public NSLoader(string assetPath)
		{
			_ptr = Interop.NSVR_Create(assetPath);

		}
		

		public int PollStatus()
		{
			return Interop.NSVR_PollStatus(_ptr);
		}

		public void SetTrackingEnabled(bool wantTracking)
		{
			Interop.NSVR_SetTrackingEnabled(_ptr, wantTracking);
		}

		public Interop.TrackingUpdate PollTracking()
		{
			Interop.TrackingUpdate t = new Interop.TrackingUpdate();
			Interop.NSVR_PollTracking(_ptr, ref t);
			return t;
		}
		
	

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				
					// TODO: dispose managed state (managed objects).
				}

				Interop.NSVR_Delete(_ptr);

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		 ~NSLoader() {
		   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
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
