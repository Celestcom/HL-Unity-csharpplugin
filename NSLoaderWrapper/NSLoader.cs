using System;
using UnityEngine;
using System.Runtime.InteropServices;
using NullSpace.SDK.Internal;
using System.ServiceProcess;
using System.Runtime.Remoting.Messaging;
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
		//public delegate void ServiceControllerCallback(bool success);
		//private delegate bool AsyncMethodCaller();

		//public static void TryServiceStartAsync(ServiceControllerCallback callback)
		//{
		//	AsyncMethodCaller caller = new AsyncMethodCaller(StartService);
		//	IAsyncResult asyncResult = caller.BeginInvoke(delegate (IAsyncResult iar) {

		//		AsyncResult result = (AsyncResult)iar;
		//		AsyncMethodCaller cbDelegate = (AsyncMethodCaller)result.AsyncDelegate;
		//		ServiceControllerCallback scCallback = (ServiceControllerCallback)iar.AsyncState;
		//		bool successfulStart = cbDelegate.EndInvoke(iar);
		//		scCallback(successfulStart);

		//	}, callback);
		//}


		//private static bool StartService()
		//{
		//	ServiceController sc = new ServiceController();
		//	sc.ServiceName = "NullSpace VR Runtime";
		//	sc.MachineName = 
		//	sc.Refresh();
		//	if (sc.Status == ServiceControllerStatus.Stopped)
		//	{
		//		try
		//		{
		//			sc.Start();
		//			sc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 5));
		//			return true;
		//		} catch (InvalidOperationException e)
		//		{
		//			Debug.LogWarning("Couldn't start the NullSpace VR Runtime service: " + e.Message);
		//		} catch(System.ServiceProcess.TimeoutException)
		//		{
		//			Debug.LogWarning("Couldn't start the NullSpace VR Runtime service in time. Runtime is stopped.");
		//		}
		//	}
		//	return false;
		//}

		internal static unsafe NSVR_System* _ptr;
		internal static bool _created = false;

		/// <summary>
		/// Retrieve the latest error from the plugin, freeing the string and returning a copy
		/// </summary>
		/// <returns></returns>
		//internal static string GetError()
		//{
		//	//Interop
		//	//IntPtr ptr = Interop.NSVR_GetError(NSVR_Plugin.Ptr);
		//	string s = Marshal.PtrToStringAnsi(ptr);
		//	Interop.NSVR_FreeError(ptr);
		//	return s;
		//}

		/// <summary>
		/// Main point of access to the plugin, implements IDisposable
		/// </summary>
		public unsafe sealed class NSVR_Plugin : IDisposable
		{
			internal static bool _disposed = false;

			internal static unsafe NSVR_System* Ptr
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

				fixed (NSVR_System** system_ptr = &_ptr)
				{
					if (Interop.NSVR_FAILURE(Interop.NSVR_System_Create(system_ptr)))
					{
						Debug.LogError("[NSVR] NSVR_Plugin could not be instantiated");

					}
					else
					{
						_created = true;
					}
				}
				

			}

			/// <summary>
			/// Pause all currently active effects
			/// </summary>
			public void PauseAll()
			{
				Interop.NSVR_System_Haptics_Pause(Ptr);
			}


			/// <summary>
			/// Resume all effects that were paused with a call to PauseAll()
			/// </summary>
			public void ResumeAll()
			{
				Interop.NSVR_System_Haptics_Resume(Ptr);
			}

			/// <summary>
			/// Destroy all effects (invalidates any HapticHandles)
			/// </summary>
			public void ClearAll()
			{
				Interop.NSVR_System_Haptics_Destroy(Ptr);
			}


			/// <summary>
			/// Poll the status of suit connection 
			/// </summary>
			/// <returns>Connected if the service is running and a suit is plugged in, else Disconnected</returns>
			public SuitStatus PollStatus()
			{

				Interop.NSVR_DeviceInfo deviceInfo = new Interop.NSVR_DeviceInfo();
				
				if (Interop.NSVR_SUCCESS(Interop.NSVR_System_GetDeviceInfo(Ptr, ref deviceInfo)))
				{
					return SuitStatus.Connected;
				}


				return SuitStatus.Disconnected;
			}


			/// <summary>
			/// Enable tracking on the suit
			/// </summary>
			public void EnableTracking()
			{
				Interop.NSVR_System_Tracking_Enable(Ptr);

			}

			/// <summary>
			/// Disable tracking on the suit 
			/// </summary>
			public void DisableTracking()
			{
				Interop.NSVR_System_Tracking_Disable(Ptr);
			}

			/// <summary>
			/// Enable or disable tracking
			/// </summary>
			/// <param name="enableTracking">If true, enables tracking. Else disables tracking.</param>
			public void SetTrackingEnabled(bool enableTracking)
			{
				if (enableTracking)
				{
					Interop.NSVR_System_Tracking_Enable(Ptr);
				}
				else
				{
					Interop.NSVR_System_Tracking_Disable(Ptr);

				}
			}

			/// <summary>
			/// Poll the suit for the latest tracking data
			/// </summary>
			/// <returns>A data structure containing all valid quaternion data</returns>
			public TrackingUpdate PollTracking()
			{
				Interop.NSVR_TrackingUpdate t = new Interop.NSVR_TrackingUpdate();
				Interop.NSVR_System_Tracking_Poll(Ptr, ref t);

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

					fixed (NSVR_System** ptr = &_ptr)
					{
						Interop.NSVR_System_Release(ptr);
					}

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
		private IntPtr _handle;
		private CommandWithHandle _creator; 

		internal delegate void CommandWithHandle(IntPtr handle);

		internal HapticHandle(CommandWithHandle creator)
		{
			//The reason we are storing the creator is so that people can clone the handle
			_creator = creator;
			Interop.NSVR_PlaybackHandle_Create(ref _handle);
			_creator(_handle);
		}

		/// <summary>
		/// Cause the associated effect to play. If paused, play will resume where it left off. If stopped, play will resume from the beginning. 
		/// </summary>
		/// <returns>Reference to this HapticHandle</returns>
		public HapticHandle Play()
		{
			Interop.NSVR_PlaybackHandle_Command(_handle, Interop.NSVR_PlaybackCommand.Play);
			return this;
		}

		/// <summary>
		/// Cause the associated effect to pause. 
		/// </summary>
		/// <returns>Reference to this HapticHandle</returns>
		public HapticHandle Pause()
		{
			Interop.NSVR_PlaybackHandle_Command(_handle, Interop.NSVR_PlaybackCommand.Pause);
			return this;
		}

		/// <summary>
		/// Cause the associated effect to stop. Will reset the effect to the beginning in a paused state. 
		/// </summary>
		/// <returns>Reference to this HapticHandle</returns>
		public HapticHandle Stop()
		{
			Interop.NSVR_PlaybackHandle_Command(_handle, Interop.NSVR_PlaybackCommand.Reset);
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

		//Release the resources associated with this handle. Use this if you do not intend to use this handle again.
		//Using the handle after releasing will have no effect. 
		public void Release()
		{
			Interop.NSVR_PlaybackHandle_Release(ref _handle);

		}

	}
	
	
}



