using System;
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
			Interop.NSVR_FreeError(ptr);
			return s;
		}





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
			public NSVR_Plugin(string path)
			{
				_disposed = false;
				if (_created)
				{
					Debug.LogWarning("[NSVR] NSVR_Plugin should only be created by the NullSpace SDK");
					return;
				}
				_ptr = Interop.NSVR_Create();
				//New plugin doesn't do FS stuff?
				//Interop.NSVR_InitializeFromFilesystem(_ptr, path);
				_created = true;

			}


			public void PauseAll()
			{
				Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.PAUSE_ALL);
			}

			public void ResumeAll()
			{
				Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.PLAY_ALL);
			}

			public void ClearAll()
			{
				Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.CLEAR_ALL);
			}



			public SuitStatus PollStatus()
			{
				return (SuitStatus)Interop.NSVR_PollStatus(Ptr);
			}

			public void SetTrackingEnabled(bool wantTracking)
			{
				if (wantTracking)
				{
					Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.ENABLE_TRACKING);
				}
				else
				{
					Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.DISABLE_TRACKING);

				}
			}

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

	public struct TrackingUpdate
	{
		public UnityEngine.Quaternion Chest;
		public UnityEngine.Quaternion LeftUpperArm;
		public UnityEngine.Quaternion LeftForearm;
		public UnityEngine.Quaternion RightUpperArm;
		public UnityEngine.Quaternion RightForearm;
	}

	public sealed class HapticHandle 
	{
		private uint _handle;
		private CommandWithHandle _creator; 

		internal delegate void CommandWithHandle(uint handle);

		internal HapticHandle(CommandWithHandle creator)
		{
			_creator = creator;


			_handle = Interop.NSVR_GenHandle(NSVR.NSVR_Plugin.Ptr);

			_creator(_handle);
		}

		public HapticHandle Play()
		{
			Interop.NSVR_DoHandleCommand(NSVR.NSVR_Plugin.Ptr, _handle, (short)Interop.HandleCommand.PLAY);
			return this;
		}
		public HapticHandle Pause()
		{
			Interop.NSVR_DoHandleCommand(NSVR.NSVR_Plugin.Ptr, _handle, (short)Interop.HandleCommand.PAUSE);
			return this;
		}
		public HapticHandle Stop()
		{
			Interop.NSVR_DoHandleCommand(NSVR.NSVR_Plugin.Ptr, _handle, (short)Interop.HandleCommand.RESET);
			return this;
		}



	}
	
	
}



