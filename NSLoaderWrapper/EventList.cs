using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatBuffers;
using NullSpace.Events;

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

		protected abstract int _generate(FlatBufferBuilder b);

		protected SuitEvent(SuitEventType eventType)
		{
			this._eventType = eventType;
		}
		public Offset<Events.SuitEvent> Generate(FlatBufferBuilder b)
		{
			int realValue = _generate(b);
			Events.SuitEvent.StartSuitEvent(b);
			Events.SuitEvent.AddEvent(b, realValue);
			Events.SuitEvent.AddEventType(b, _eventType);
			return Events.SuitEvent.EndSuitEvent(b);
		}

		public abstract int CompareTo(object obj);
	}

	internal class BasicHapticEvent : SuitEvent, ITimeIndexed
	{
		private float _time;
		private float _strength;
		private float _duration;
		private UInt32 _area;
		private Effect _effect;

	

		public override float Time
		{
			get
			{
				return _time;
			}
		}

		protected override int _generate(FlatBufferBuilder b)
		{
			Events.BasicHapticEvent.StartBasicHapticEvent(b);
			Events.BasicHapticEvent.AddArea(b, _area);
			Events.BasicHapticEvent.AddStrength(b, _strength);
			Events.BasicHapticEvent.AddDuration(b, _duration);
			Events.BasicHapticEvent.AddTime(b, _time);
			Events.BasicHapticEvent.AddEffect(b, (uint)_effect);
			return Events.BasicHapticEvent.EndBasicHapticEvent(b).Value;
		}

		public override int CompareTo(object obj)
		{
			ITimeIndexed timed = (ITimeIndexed)obj;
			return _time.CompareTo(timed.Time);

		}

		public BasicHapticEvent(float time, float strength, float duration, UInt32 area, Effect effect)
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
		public byte[] GetBytes()
		{
			_events.Sort();
			Console.WriteLine("WTF1");
			
			FlatBufferBuilder builder = new FlatBufferBuilder(1);
			
			Console.WriteLine("WTF2");
			Offset<Events.SuitEvent>[] generatedEvents = new Offset<Events.SuitEvent>[_events.Count];
			for (int i = 0; i < _events.Count; i++) {
				generatedEvents[i] = _events[i].Generate(builder);
			}
			
			
			var offsetGeneratedEvents = Events.SuitEventList.CreateEventsVector(builder, generatedEvents);
			Events.SuitEventList.StartSuitEventList(builder);
			Events.SuitEventList.AddEvents(builder, offsetGeneratedEvents);
			var list = Events.SuitEventList.EndSuitEventList(builder);

			builder.Finish(list.Value);


			return builder.SizedByteArray();
			
	

		}


	}
}
