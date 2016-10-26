using System;
namespace NullSpace.Loader
{
	public class NSLoader : IDisposable
	{
		private IntPtr _ptr;

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

		public Interop.Quaternion PollTracking()
		{
			Interop.Quaternion q = new Interop.Quaternion();
			Interop.TestClass_PollTracking(_ptr, ref q);
			return q;
		}
		
		public void PlaySequence(string name, int location)
		{
			
			Interop.TestClass_PlaySequence(_ptr, name, location);
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
