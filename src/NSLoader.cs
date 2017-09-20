using System;
using UnityEngine;
using System.Runtime.InteropServices;
using NullSpace.SDK.Internal;
using System.ServiceProcess;
using System.Runtime.Remoting.Messaging;
using System.Collections.Generic;

namespace NullSpace.SDK
{

	/// <summary>
	/// Represents version information, containing a major and minor version
	/// </summary>
	public struct VersionInfo
	{
		public uint Major;
		public uint Minor;

		/// <summary>
		/// Returns Major.Minor
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0}.{1}", Major, Minor);
		}
	}

	/// <summary>
	/// Internal testing tool; do not depend upon this. May change at any time.
	/// </summary>
	public struct EffectSampleInfo
	{
		public UInt16 Strength;
		public UInt32 Family;
		public Region Area;
		public EffectSampleInfo(UInt16 strength, UInt32 family, Region area)
		{
			Strength = strength;
			Family = family;
			Area = area;
		}
	}

	public class HapticsLoadingException : System.Exception
	{
		public HapticsLoadingException() : base() { }
		public HapticsLoadingException(string message) : base(message) { }
		public HapticsLoadingException(string message, System.Exception inner) : base(message, inner) { }


		protected HapticsLoadingException(System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
		{ }
	}

	public class Device
	{
		public string Name { get; }
		public bool Connected { get; }
		internal Device(string name, bool connected)
		{
			Name = name;
			Connected = connected;
		}
	}

	/// <summary>
	/// Wrapper around the main access point of the plugin, NSVR_Plugin
	/// </summary>
	public static class NSVR
	{
		internal static unsafe NSVR_System* _ptr;
		internal static bool _created = false;

	
		/// <summary>
		/// Main point of access to the plugin, implements IDisposable
		/// </summary>
		public unsafe sealed class NSVR_Plugin : IDisposable
		{
			internal static bool _disposed = false;
			private IntPtr _bodyView = IntPtr.Zero;

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
						throw new MemberAccessException("[NSVR] You must have a NS Manager prefab in your scene!\n");

					}

				}
			}

			public NSVR_Plugin()
			{
				_disposed = false;
				if (_created)
				{
					Debug.LogWarning("[NSVR] NSVR_Plugin should only be created by the NullSpace SDK\n");
					return;
				}

				fixed (NSVR_System** system_ptr = &_ptr)
				{
					if (Interop.NSVR_SUCCESS(Interop.NSVR_System_Create(system_ptr)))
					{
						_created = true;

						int result = Interop.NSVR_BodyView_Create(ref _bodyView);
					}
					else
					{
						Debug.LogError("[NSVR] NSVR_Plugin could not be instantiated\n");

					}
				}
				

			}

			
	
			
			public Dictionary<Region, EffectSampleInfo> PollBodyView()
			{
				Dictionary<Region, EffectSampleInfo> result = new Dictionary<Region, EffectSampleInfo>();

				Interop.NSVR_BodyView_Poll(_bodyView, Ptr);

				uint numNodes = 0;
				Interop.NSVR_BodyView_GetNodeCount(_bodyView, ref numNodes);

				for (uint i = 0; i < numNodes; i++)
				{
					uint nodeType = 0;
					Interop.NSVR_BodyView_GetNodeType(_bodyView, i, ref nodeType);

					uint region = 0;

					Region outRegion = Region.unknown;

					if (region <= int.MaxValue)
					{
						outRegion = (Region)region;
					}
					else
					{
						Debug.LogError(string.Format("Warning: You must be a time traveler from the future. The region returned by the API [{0}] is too large to fit in your int32. It will most likely overflow and be meaningless.\nWe are returning Region.unknown instead.", region));
					}

					Interop.NSVR_BodyView_GetNodeRegion(_bodyView, i, ref region);


					float intensity = 0;
					Interop.NSVR_BodyView_GetIntensity(_bodyView, i, ref intensity);
					result[(Region)region] = new EffectSampleInfo((ushort)(intensity*255), 0, outRegion);




				}

				return result;
			}

			internal static double Clamp(double value, double min, double max)
			{
				return (value < min) ? min : (value > max) ? max : value;
			}

			/// <summary>
			/// Control the volume of an area directly. 
			/// </summary>
			/// <param name="singleArea">An AreaFlag representing a single area</param>
			/// <param name="strength">Strength to play, from 0.0 - 1.0</param>
			public void ControlDirectly(AreaFlag singleArea, double strength)
			{
	
					
				ushort[] intensities = new ushort[1];
				UInt32[] areas = new UInt32[1];
				areas[0] = (uint)singleArea;
				intensities[0] = (ushort)(255 * Clamp(strength, 0.0, 1.0));
				Interop.NSVR_Immediate_Set(Ptr, intensities, areas, 1);
			}

			/// <summary>
			/// Control the volume of multiple areas directly. 
			/// </summary>
			/// <param name="singleAreas">List of AreaFlags, each representing a single area</param>
			/// <param name="strengths">Strength to play, from 0-255</param>
			public void ControlDirectly(AreaFlag[] singleAreas, ushort[] strengths)
			{
				if (singleAreas.Length != strengths.Length)
				{
					Debug.LogWarning("You may not pass an area array and strength array of different lengths");
					return;
				}

				UInt32[] areas = new UInt32[singleAreas.Length];
				for (int i = 0; i < singleAreas.Length; i++)
				{
					areas[i] = (UInt32)(singleAreas[i]);
				}

				Interop.NSVR_Immediate_Set(Ptr, strengths, areas, areas.Length);

			}


			/** END INTERNAL **/
			/// <summary>
			/// Pause all currently active effects
			/// </summary>
			public void PauseAll()
			{
				Interop.NSVR_System_Haptics_Suspend(Ptr);
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
			/// Return the plugin version
			/// </summary>
			/// <returns></returns>
			public static VersionInfo GetPluginVersion()
			{
				uint version = Interop.NSVR_Version_Get();
				VersionInfo v = new VersionInfo();
				v.Minor = version & 0xFFFF;
				v.Major = (version & 0xFFFF0000) >> 16;
				return v;
			}



			public List<Device> GetKnownDevices()
			{
				List<Device> devices = new List<Device>();

				Interop.NSVR_DeviceInfo_Iter iter = new Interop.NSVR_DeviceInfo_Iter();
				Interop.NSVR_DeviceInfo_Iter_Init(ref iter);

				while (Interop.NSVR_DeviceInfo_Iter_Next(ref iter, Ptr))
				{
					devices.Add(new Device(new string(iter.DeviceInfo.Name), iter.DeviceInfo.Status == Interop.NSVR_DeviceStatus.Connected));
				}

				return devices;
			}


			public ServiceConnectionStatus IsConnectedToService()
			{
				Interop.NSVR_ServiceInfo serviceInfo = new Interop.NSVR_ServiceInfo();
				int value = Interop.NSVR_System_GetServiceInfo(Ptr, ref serviceInfo);

				//	Debug.Log(string.Format("Value is {0}", value));
				if (Interop.NSVR_SUCCESS(value))
				{
					return ServiceConnectionStatus.Connected;
				} else
				{
					return ServiceConnectionStatus.Disconnected;
				}
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

					Interop.NSVR_BodyView_Release(ref _bodyView);

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

			/// <summary>
			/// Disposes the plugin. After calling dispose, the plugin cannot be used again.
			/// </summary>
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
	public sealed class HapticHandle : IDisposable
	{
		private IntPtr _handle = IntPtr.Zero;
		private CommandWithHandle _creator;
		private float _duration;

		internal delegate void CommandWithHandle(IntPtr handle);

		public float EffectDuration
		{
			get { return _duration; }
		}

		/// <summary>
		/// If we want to construct the handle for the very first time, we have no duration info present.
		/// So we end up calculating it in the constructor, then caching it. Subsequent Clones() will copy the duration
		///
		/// </summary>
		/// <param name="creator"></param>
		internal HapticHandle(CommandWithHandle creator)
		{
			
			init(creator);
			Interop.NSVR_EffectInfo info = new Interop.NSVR_EffectInfo();
			int result = Interop.NSVR_PlaybackHandle_GetInfo(_handle, ref info);
			if (Interop.NSVR_SUCCESS(result))
			{
				_duration = info.Duration;
			} else
			{
				UnityEngine.Debug.LogError(string.Format("Failed to fetch information about haptic handle {0}! This handle is no longer usable and has likely been disposed.", _handle));
			}
			
		}

		/// <summary>
		/// Clones will call this constructor, grabbing the duration
		/// </summary>
		/// <param name="creator"></param>
		/// <param name="duration"></param>
		internal HapticHandle(CommandWithHandle creator, float duration)
		{
			init(creator);
			Debug.Assert(_handle != IntPtr.Zero);

			_duration = duration;
		}

		internal void init(CommandWithHandle creator)
		{
			Debug.Assert(creator != null);
			
			_creator = creator;

			Interop.NSVR_PlaybackHandle_Create(ref _handle);

			//Debug.Log("Inside init. Is creator null?!" + (creator == null));
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
		/// Cause the associated effect to immediately play from the beginning.
		/// Identical to Stop().Play()
		/// </summary>
		/// <returns></returns>
		public HapticHandle Replay()
		{
			return this.Stop().Play();
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
		/// Returns true if the effect has completed playback
		/// </summary>
		/// <returns></returns>
		public bool IsFinishedPlaying()
		{
			return GetElapsedTime() >= _duration;
		}

		/// <summary>
		/// Clone this HapticHandle, allowing an identical effect to be controlled independently 
		/// </summary>
		/// <returns></returns>
		public HapticHandle Clone()
		{
			HapticHandle newHandle = new HapticHandle(this._creator, _duration);
			return newHandle;
		}

		/// <summary>
		/// Returns how far this effect has advanced in fractional seconds. 
		/// Returns a value less than 0 on failure.
		/// </summary>
		/// <returns></returns>
		public float GetElapsedTime()
		{
			Interop.NSVR_EffectInfo info = new Interop.NSVR_EffectInfo();
			if (Interop.NSVR_SUCCESS(Interop.NSVR_PlaybackHandle_GetInfo(_handle, ref info)))
			{
				return info.Elapsed;
			} else
			{
				return -1.0f;
			}
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

				if (!NSVR.NSVR_Plugin._disposed)
				{
					Interop.NSVR_PlaybackHandle_Release(ref _handle);
				}

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		~HapticHandle() {
		   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		   Dispose(false);
		}

		/// <summary>
		/// Dispose the handle, releasing its resources from the plugin. After disposing a handle, it cannot be used again.
		/// </summary>
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



