using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hardlight.Events;
using Hardlight.SDK.Internal;
using System.Diagnostics;
using static Hardlight.SDK.HLVR;

namespace Hardlight.SDK
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
			Interop.HLVR_Event_Create(ref eventPtr, Interop.HLVR_EventType.Basic_Haptic_Event);
			Debug.Assert(eventPtr != IntPtr.Zero);

			Interop.HLVR_Event_SetFloat(eventPtr, "duration", _duration);
			//could be unsigned problem?!? Nah. Context: something is generating events without area information

			Interop.HLVR_Event_SetUInt32s(eventPtr, "area", _area, (uint)_area.Length);
			Interop.HLVR_Event_SetFloat(eventPtr, "strength", _strength);

			Interop.HLVR_Event_SetFloat(eventPtr, "time", _time);
			Interop.HLVR_Event_SetInt(eventPtr, "effect", (int)_effect);

			Interop.HLVR_Timeline_AddEvent(timelinePtr, eventPtr);

			Interop.HLVR_Event_Release(ref eventPtr);
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
				Interop.HLVR_Timeline_Create(ref timelinePtr);
			}

			for (int i = 0; i < _events.Count; i++)
			{
				_events[i].Generate(timelinePtr);
			}

			unsafe
			{
				Interop.HLVR_Timeline_Transmit(timelinePtr, HLVR_Plugin.Ptr, playbackHandle);
			}

			Interop.HLVR_Timeline_Release(ref timelinePtr);
		}


	}
}
