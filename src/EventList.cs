using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NullSpace.Events;
using NullSpace.SDK.Internal;
using System.Diagnostics;
using static NullSpace.SDK.NSVR;

namespace NullSpace.SDK
{
	internal interface ITimeIndexed
	{
		float Time { get; }
	}
	internal abstract class SuitEvent : IComparable, ITimeIndexed
	{
		private SuitEventType _eventType;

		public abstract float Time { get; }

		protected abstract void  _generateEvent(IntPtr timelinePtr);

		protected SuitEvent(SuitEventType eventType)
		{
			this._eventType = eventType;
		}
		public void Generate(IntPtr timelinePtr)
		{
			_generateEvent(timelinePtr);
		}

		public abstract int CompareTo(object obj);
	}

	internal class BasicHapticEvent : SuitEvent, ITimeIndexed
	{
		private float _time;
		private float _strength;
		private float _duration;
		private UInt32[] _area;
		private Effect _effect;

	

		public override float Time
		{
			get
			{
				return _time;
			}
		}

		protected override void _generateEvent(IntPtr timelinePtr)
		{
			Debug.Assert(timelinePtr != IntPtr.Zero);

			IntPtr eventPtr = IntPtr.Zero;
			Interop.NSVR_Event_Create(ref eventPtr, Interop.NSVR_EventType.SimpleHaptic);
			Debug.Assert(eventPtr != IntPtr.Zero);

			Interop.NSVR_Event_SetFloat(eventPtr, Interop.NSVR_EventKey.SimpleHaptic_Duration_Float, _duration);
			//could be unsigned problem?!? Nah. Context: something is generating events without area information

			Interop.NSVR_Event_SetUInt32s(eventPtr, Interop.NSVR_EventKey.SimpleHaptic_Region_UInt32s, _area, (uint)_area.Length);
			Interop.NSVR_Event_SetFloat(eventPtr, Interop.NSVR_EventKey.SimpleHaptic_Strength_Float, _strength);

			Interop.NSVR_Event_SetFloat(eventPtr, Interop.NSVR_EventKey.Time_Float, _time);
			Interop.NSVR_Event_SetInt(eventPtr, Interop.NSVR_EventKey.SimpleHaptic_Effect_Int, (int)_effect);

			Interop.NSVR_Timeline_AddEvent(timelinePtr, eventPtr);

			Interop.NSVR_Event_Release(ref eventPtr);
			Debug.Assert(eventPtr == IntPtr.Zero);
		}

		public override int CompareTo(object obj)
		{
			ITimeIndexed timed = (ITimeIndexed)obj;
			return _time.CompareTo(timed.Time);

		}

		public BasicHapticEvent(float time, float strength, float duration, UInt32[] area, Effect effect)
			:base(SuitEventType.BasicHapticEvent)
		{
			
			this._time = time;
			this._strength = strength;
			this._duration = duration;
			this._area = area;
			this._effect = effect;
		}
		
	}
	internal class EventList { 
	
		private List<SuitEvent> _events;
		public EventList()
		{
			_events = new List<SuitEvent>();
		}

		public void AddEvent(SuitEvent e)
		{
			_events.Add(e);
		}


		public void AddAll(EventList other)
		{
			foreach (var e in other._events)
			{
				_events.Add(e);
			}
		}

		public override string ToString()
		{
			return string.Format("Event list made up of " + _events.Count + " events");
		}
		public void Transmit(IntPtr playbackHandle)
		{
			IntPtr timelinePtr = IntPtr.Zero;

			unsafe
			{
				Interop.NSVR_Timeline_Create(ref timelinePtr);
			}

			for (int i = 0; i < _events.Count; i++)
			{
				_events[i].Generate(timelinePtr);
			}

			unsafe
			{
				Interop.NSVR_Timeline_Transmit(timelinePtr, NSVR_Plugin.Ptr, playbackHandle);
			}

			Interop.NSVR_Timeline_Release(ref timelinePtr);
		}


	}
}
