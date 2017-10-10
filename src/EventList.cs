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
	internal abstract class HardlightEvent : IComparable, ITimeIndexed
	{
		private SuitEventType _eventType;

		public abstract float Time { get; }

		protected abstract unsafe void  _generateEvent(HLVR_Timeline* timeline);

		protected HardlightEvent(SuitEventType eventType)
		{
			this._eventType = eventType;
		}
		public unsafe void Generate(HLVR_Timeline* timeline)
		{
			_generateEvent(timeline);
		}

		public abstract int CompareTo(object obj);
	}

	internal class BasicHapticEvent : HardlightEvent, ITimeIndexed
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

		protected override unsafe void _generateEvent(HLVR_Timeline* timelinePtr)
		{
			Debug.Assert(timelinePtr != null);

			
			HLVR_Event* eventPtr = null;
			Interop.HLVR_Event_Create(&eventPtr, Interop.HLVR_EventType.SimpleHaptic);
		
			Debug.Assert(eventPtr != null);

			Interop.HLVR_Event_SetFloat(eventPtr, Interop.HLVR_EventKey.SimpleHaptic_Duration_Float, _duration);
			Interop.HLVR_Event_SetUInt32s(eventPtr, Interop.HLVR_EventKey.SimpleHaptic_Region_UInt32s, _area, (uint)_area.Length);
			Interop.HLVR_Event_SetFloat(eventPtr, Interop.HLVR_EventKey.SimpleHaptic_Strength_Float, _strength);
			Interop.HLVR_Event_SetFloat(eventPtr, Interop.HLVR_EventKey.Time_Float, _time);
			Interop.HLVR_Event_SetInt(eventPtr, Interop.HLVR_EventKey.SimpleHaptic_Effect_Int, (int)_effect);

			Interop.HLVR_Timeline_AddEvent(timelinePtr, eventPtr);

			Interop.HLVR_Event_Destroy(&eventPtr);
			Debug.Assert(eventPtr == null);
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
	
		private List<HardlightEvent> _events;
		public EventList()
		{
			_events = new List<HardlightEvent>();
		}

		public void AddEvent(HardlightEvent e)
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
		public unsafe void Transmit(HLVR_Effect* effect)
		{
			HLVR_Timeline* timelinePtr = null;
			Interop.HLVR_Timeline_Create(&timelinePtr);
		
			for (int i = 0; i < _events.Count; i++)
			{
				_events[i].Generate(timelinePtr);
			}

			
			Interop.HLVR_Timeline_Transmit(timelinePtr, HLVR_Plugin.Ptr, effect);
			

			Interop.HLVR_Timeline_Destroy(&timelinePtr);
			Debug.Assert(timelinePtr == null);
		}


	}
}
