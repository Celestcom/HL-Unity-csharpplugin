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
		internal static unsafe HLVR_Agent* _ptr;
		internal static bool _created = false;

	
		/// <summary>
		/// Main point of access to the plugin, implements IDisposable
		/// </summary>
		public unsafe sealed class NSVR_Plugin : IDisposable
		{
			internal static bool _disposed = false;
			private IntPtr _bodyView = IntPtr.Zero;

			internal static unsafe HLVR_Agent* Ptr
			{
				get
				{
					if (_created && !_disposed && _ptr != null)
					{

						return _ptr;
					}
					else
					{
						throw new MemberAccessException("[HLVR] You must have a NS Manager prefab in your scene!\n");

					}

				}
			}

			public NSVR_Plugin()
			{
				_disposed = false;
				if (_created)
				{
					Debug.LogWarning("[HLVR] NSVR_Plugin should only be created by the NullSpace SDK\n");
					return;
				}

				fixed (HLVR_Agent** system_ptr = &_ptr)
				{
					if (Interop.OK(Interop.HLVR_Agent_Create(system_ptr)))
					{
						_created = true;

						int result = Interop.HLVR_BodyView_Create(ref _bodyView);
					}
					else
					{
						Debug.LogError("[HLVR] HLVR_Plugin could not be instantiated\n");

					}
				}
				

			}

			
	
			
			public Dictionary<Region, EffectSampleInfo> PollBodyView()
			{
				Dictionary<Region, EffectSampleInfo> result = new Dictionary<Region, EffectSampleInfo>();

				Interop.HLVR_BodyView_Poll(_bodyView, Ptr);

				uint numNodes = 0;
				Interop.HLVR_BodyView_GetNodeCount(_bodyView, ref numNodes);

				for (uint i = 0; i < numNodes; i++)
				{
					uint nodeType = 0;
					Interop.HLVR_BodyView_GetNodeType(_bodyView, i, ref nodeType);

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

					Interop.HLVR_BodyView_GetNodeRegion(_bodyView, i, ref region);


					float intensity = 0;
					Interop.HLVR_BodyView_GetIntensity(_bodyView, i, ref intensity);
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
				Interop.HLVR_Immediate_Set(Ptr, intensities, areas, 1);
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

				Interop.HLVR_Immediate_Set(Ptr, strengths, areas, areas.Length);

			}


			/** END INTERNAL **/
			/// <summary>
			/// Pause all currently active effects
			/// </summary>
			public void PauseAll()
			{
				Interop.HLVR_Agent_SuspendEffects(Ptr);
			}


			/// <summary>
			/// Resume all effects that were paused with a call to PauseAll()
			/// </summary>
			public void ResumeAll()
			{
				Interop.HLVR_Agent_ResumeEffects(Ptr);
			}

			/// <summary>
			/// Destroy all effects (invalidates any HapticHandles)
			/// </summary>
			public void ClearAll()
			{
				Interop.HLVR_Agent_CancelEffects(Ptr);
			}

			/// <summary>
			/// Return the plugin version
			/// </summary>
			/// <returns></returns>
			public static VersionInfo GetPluginVersion()
			{
				uint version = Interop.HLVR_Version_Get();
				VersionInfo v = new VersionInfo();
				v.Minor = version & 0xFFFF;
				v.Major = (version & 0xFFFF0000) >> 16;
				return v;
			}



			public List<Device> GetKnownDevices()
			{
				List<Device> devices = new List<Device>();

				Interop.HLVR_DeviceIterator iter = new Interop.HLVR_DeviceIterator();
				Interop.HLVR_DeviceIterator_Init(ref iter);

				while (Interop.OK(Interop.HLVR_DeviceIterator_Next(ref iter, Ptr)))
				{
					devices.Add(new Device(new string(iter.DeviceInfo.Name), iter.DeviceInfo.Status == Interop.HLVR_DeviceStatus.Connected));
				}

				return devices;
			}


			public ServiceConnectionStatus IsConnectedToService()
			{
				Interop.HLVR_PlatformInfo serviceInfo = new Interop.HLVR_PlatformInfo();
				int value = Interop.HLVR_Agent_GetPlatformInfo(Ptr, ref serviceInfo);

				//	Debug.Log(string.Format("Value is {0}", value));
				if (Interop.OK(value))
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
				Interop.HLVR_Agent_EnableTracking(Ptr);

			}

			/// <summary>
			/// Disable tracking on the suit 
			/// </summary>
			public void DisableTracking()
			{
				Interop.HLVR_Agent_DisableTracking(Ptr);
			}

			/// <summary>
			/// Enable or disable tracking
			/// </summary>
			/// <param name="enableTracking">If true, enables tracking. Else disables tracking.</param>
			public void SetTrackingEnabled(bool enableTracking)
			{
				if (enableTracking)
				{
					EnableTracking();
				}
				else
				{
					DisableTracking();
				}
			}

			/// <summary>
			/// Poll the suit for the latest tracking data
			/// </summary>
			/// <returns>A data structure containing all valid quaternion data</returns>
			public TrackingUpdate PollTracking()
			{
				Interop.HLVR_TrackingUpdate t = new Interop.HLVR_TrackingUpdate();
				Interop.HLVR_Agent_PollTracking(Ptr, ref t);

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

					Interop.HLVR_BodyView_Release(ref _bodyView);

					fixed (HLVR_Agent** ptr = &_ptr)
					{
						
						Interop.HLVR_Agent_Destroy(ptr);
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
		private unsafe HLVR_Effect* _handle;
		private CommandWithHandle _creator;
		private float _duration;

		internal unsafe delegate void CommandWithHandle(HLVR_Effect* effect);

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

		
			Interop.HLVR_EffectInfo info = new Interop.HLVR_EffectInfo();

			//Failure by default
			int result = -1;

			unsafe
			{
				result = Interop.HLVR_Effect_GetInfo(_handle, ref info);
			}

			if (Interop.OK(result))
			{
				_duration = info.Duration;
			} else
			{
				Debug.LogError(string.Format("Failed to fetch information about haptic handle; the handle has been disposed and is no longer usable."));
			}
			
		}

		/// <summary>
		/// Clones will call this constructor, grabbing the duration
		/// </summary>
		/// <param name="creator"></param>
		/// <param name="duration"></param>
		internal unsafe HapticHandle(CommandWithHandle creator, float duration)
		{
			init(creator);
			Debug.Assert(_handle != null);

			_duration = duration;
		}

		internal unsafe void init(CommandWithHandle creator)
		{
			Debug.Assert(creator != null);
			
			_creator = creator;

			fixed (HLVR_Effect** effect_ptr = &_handle)
			{
				if (Interop.FAIL(Interop.HLVR_Effect_Create(effect_ptr)))
				{
					Debug.LogError("[HLVR] Failed to create new Haptic Handle!");
				}
			}

			//Debug.Log("Inside init. Is creator null?!" + (creator == null));
			_creator(_handle);
		}
		/// <summary>
		/// Cause the associated effect to play. If paused, play will resume where it left off. If stopped, play will resume from the beginning. 
		/// </summary>
		/// <returns>Reference to this HapticHandle</returns>
		public unsafe HapticHandle Play()
		{
			Interop.HLVR_Effect_Play(_handle);
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
		public unsafe HapticHandle Pause()
		{
			Interop.HLVR_Effect_Pause(_handle);
			return this;
		}

		/// <summary>
		/// Cause the associated effect to stop. Will reset the effect to the beginning in a paused state. 
		/// </summary>
		/// <returns>Reference to this HapticHandle</returns>
		public unsafe HapticHandle Stop()
		{
			Interop.HLVR_Effect_Reset(_handle);
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
		public unsafe float GetElapsedTime()
		{
			Interop.HLVR_EffectInfo info = new Interop.HLVR_EffectInfo();
			if (Interop.OK(Interop.HLVR_Effect_GetInfo(_handle, ref info)))
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
					unsafe
					{
						fixed (HLVR_Effect** ptr = &_handle)
						{
							Interop.HLVR_Effect_Destroy(ptr);
						}
					}

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



