using System;
using UnityEngine;
using System.Runtime.InteropServices;
using NullSpace.SDK.Internal;

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

	/// <summary>
	/// Wrapper around the main access point of the plugin, NSVR_Plugin
	/// </summary>
	public static class NSVR
	{
		internal static IntPtr _ptr;
		internal static bool _created = false;

		/// <summary>
		/// Retrieve the latest error from the plugin, freeing the string and returning a copy
		/// </summary>
		/// <returns></returns>
		internal static string GetError()
		{
			IntPtr ptr = Interop.NSVR_GetError(NSVR_Plugin.Ptr);
			string s = Marshal.PtrToStringAnsi(ptr);
			Interop.NSVR_FreeError(ptr);
			return s;
		}

		/// <summary>
		/// Main point of access to the plugin, implements IDisposable
		/// </summary>
		public sealed class NSVR_Plugin : IDisposable
		{
			internal static bool _disposed = false;

			internal static IntPtr Ptr
			{
				get
				{
					if (_created && !_disposed && _ptr != null)
					{

						return _ptr;
					}
					else
					{
						throw new MemberAccessException("[NSVR] You must have a NS Manager prefab in your scene!");

					}

				}
			}

			public NSVR_Plugin()
			{
				_disposed = false;
				if (_created)
				{
					Debug.LogWarning("[NSVR] NSVR_Plugin should only be created by the NullSpace SDK");
					return;
				}
				_ptr = Interop.NSVR_Create();
				_created = true;

			}

			/// <summary>
			/// Pause all currently active effects
			/// </summary>
			public void PauseAll()
			{
				Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.PAUSE_ALL);
			}


			/// <summary>
			/// Resume all effects that were paused with a call to PauseAll()
			/// </summary>
			public void ResumeAll()
			{
				Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.PLAY_ALL);
			}

			/// <summary>
			/// Destroy all effects (invalidates any HapticHandles)
			/// </summary>
			public void ClearAll()
			{
				Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.CLEAR_ALL);
			}


			/// <summary>
			/// Poll the status of suit connection 
			/// </summary>
			/// <returns>Connected if the service is running and a suit is plugged in, else Disconnected</returns>
			public SuitStatus PollStatus()
			{
				return (SuitStatus)Interop.NSVR_PollStatus(Ptr);
			}

			/// <summary>
			/// Enable tracking on the suit
			/// </summary>
			public void EnableTracking()
			{
				Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.ENABLE_TRACKING);

			}

			/// <summary>
			/// Disable tracking on the suit 
			/// </summary>
			public void DisableTracking()
			{
				Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.DISABLE_TRACKING);

			}

			/// <summary>
			/// Enable or disable tracking
			/// </summary>
			/// <param name="enableTracking">If true, enables tracking. Else disables tracking.</param>
			public void SetTrackingEnabled(bool enableTracking)
			{
				if (enableTracking)
				{
					Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.ENABLE_TRACKING);
				}
				else
				{
					Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.DISABLE_TRACKING);

				}
			}

			/// <summary>
			/// Poll the suit for the latest tracking data
			/// </summary>
			/// <returns>A data structure containing all valid quaternion data</returns>
			public TrackingUpdate PollTracking()
			{
				InteropTrackingUpdate t = new InteropTrackingUpdate();
				Interop.NSVR_PollTracking(Ptr, ref t);

				TrackingUpdate update = new TrackingUpdate();
				update.Chest = new UnityEngine.Quaternion(t.chest.x, t.chest.y, t.chest.z, t.chest.w);
				update.LeftUpperArm = new UnityEngine.Quaternion(t.left_upper_arm.x, t.left_upper_arm.y, t.left_upper_arm.z, t.left_upper_arm.w);
				update.RightUpperArm = new UnityEngine.Quaternion(t.right_upper_arm.x, t.right_upper_arm.y, t.right_upper_arm.z, t.right_upper_arm.w);
				update.LeftForearm = new UnityEngine.Quaternion(t.left_forearm.x, t.left_forearm.y, t.left_forearm.z, t.left_forearm.w);
				update.RightForearm = new UnityEngine.Quaternion(t.right_forearm.x, t.right_forearm.y, t.right_forearm.z, t.right_forearm.w);
				return update;
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

					_created = false;
					Interop.NSVR_Delete(_ptr);

					disposedValue = true;
					_disposed = true;
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

	/// <summary>
	/// Able to hold tracking data for chest and arm IMUs
	/// </summary>
	public struct TrackingUpdate
	{
		public UnityEngine.Quaternion Chest;
		public UnityEngine.Quaternion LeftUpperArm;
		public UnityEngine.Quaternion LeftForearm;
		public UnityEngine.Quaternion RightUpperArm;
		public UnityEngine.Quaternion RightForearm;
	}

	/// <summary>
	/// Use a HapticHandle to Play, Pause, or Stop an effect. A HapticHandle represents a particular instance of an effect.
	/// </summary>
	public sealed class HapticHandle 
	{
		private uint _handle;
		private CommandWithHandle _creator; 

		internal delegate void CommandWithHandle(uint handle);

		internal HapticHandle(CommandWithHandle creator)
		{
			//The reason we are storing the creator is so that people can clone the handle
			_creator = creator;
			_handle = Interop.NSVR_GenHandle(NSVR.NSVR_Plugin.Ptr);
			_creator(_handle);
		}

		/// <summary>
		/// Cause the associated effect to play. If paused, play will resume where it left off. If stopped, play will resume from the beginning. 
		/// </summary>
		/// <returns>Reference to this HapticHandle</returns>
		public HapticHandle Play()
		{
			Interop.NSVR_DoHandleCommand(NSVR.NSVR_Plugin.Ptr, _handle, (short)Interop.HandleCommand.PLAY);
			return this;
		}

		/// <summary>
		/// Cause the associated effect to pause. 
		/// </summary>
		/// <returns>Reference to this HapticHandle</returns>
		public HapticHandle Pause()
		{
			Interop.NSVR_DoHandleCommand(NSVR.NSVR_Plugin.Ptr, _handle, (short)Interop.HandleCommand.PAUSE);
			return this;
		}

		/// <summary>
		/// Cause the associated effect to stop. Will reset the effect to the beginning in a paused state. 
		/// </summary>
		/// <returns>Reference to this HapticHandle</returns>
		public HapticHandle Stop()
		{
			Interop.NSVR_DoHandleCommand(NSVR.NSVR_Plugin.Ptr, _handle, (short)Interop.HandleCommand.RESET);
			return this;
		}


		/// <summary>
		/// Clone this HapticHandle, allowing an identical effect to be controlled independently 
		/// </summary>
		/// <returns></returns>
		public HapticHandle Clone()
		{
			HapticHandle newHandle = new HapticHandle(this._creator);
			return newHandle;
		}

	}
	
	
}



