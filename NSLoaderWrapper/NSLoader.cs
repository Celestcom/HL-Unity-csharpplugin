using System;
namespace NullSpace.Loader
{
	public class NSLoader : IDisposable
	{
		private static IntPtr _ptr;
		
		public delegate void Play();
		public delegate void CommandWithHandle(uint handle);
		public class HapticHandle
		{
			private uint _handle;
			private CommandWithHandle _playDelegate;
			private CommandWithHandle _pauseDelegate;
			public HapticHandle(CommandWithHandle p, CommandWithHandle ph, CommandWithHandle createDel)
			{
				_playDelegate = p;
				_pauseDelegate = ph;
				_handle = Interop.TestClass_GenHandle(_ptr);
				createDel(_handle);
			}
			public void Play()
			{
				_playDelegate(_handle);
			}

			public void Pause()
			{
				_pauseDelegate(_handle);
			}
			
		}
		public class Sequence
		{
			private string _name;
			public Sequence(string name)
			{
				_name = name;
				bool loaded = Interop.TestClass_LoadSequence(_ptr, name);
				if (!loaded)
				{
					throw new System.IO.FileNotFoundException("Could not find sequence " + name);
				}
			}
			private CommandWithHandle _create(int location)
			{
				return new CommandWithHandle(x => Interop.TestClass_PlaySequence(_ptr, x, _name, location));
			}
		
			private CommandWithHandle _pause()
			{
				return new CommandWithHandle(x => Interop.TestClass_HandleCommand(_ptr, x, (short)Interop.Command.PAUSE));
			}
			private CommandWithHandle _play()
			{
				return new CommandWithHandle(x => Interop.TestClass_HandleCommand(_ptr, x, (short)Interop.Command.PLAY));

			}
			public HapticHandle CreateHandle(int location)
			{
				
				var h = new HapticHandle(_play(), _pause(), _create(location));
				return h;
			}
			
		}



		public NSLoader(string assetPath)
		{
			_ptr = Interop.TestClass_Create(assetPath);

		}
		public void PlayEffect(int effect, int location, float duration=0.0f, float time=0.0f, uint priority=1)
		{
			Interop.TestClass_PlayEffect(_ptr, effect, location, duration, time, priority);
		}

		public int PollStatus()
		{
			return Interop.TestClass_PollStatus(_ptr);
		}

		public void SetTrackingEnabled(bool wantTracking)
		{
			Interop.TestClass_SetTrackingEnabled(_ptr, wantTracking);
		}

		public Interop.TrackingUpdate PollTracking()
		{
			Interop.TrackingUpdate t = new Interop.TrackingUpdate();
			Interop.TestClass_PollTracking(_ptr, ref t);
			return t;
		}
		
		public void PlaySequence(string name, int location)
		{
			
			Interop.TestClass_PlaySequence(_ptr, (uint)0, name, location);
		}

		public void PlayPattern(string name, int side)
		{

			Interop.TestClass_PlayPattern(_ptr, name, side);
		}

		public void PlayExperience(string name, int side)
		{

			Interop.TestClass_PlayExperience(_ptr, name, side);
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

				Interop.TestClass_Delete(_ptr);

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
