using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatBuffers;
using NullSpace.Events;

namespace NullSpace.SDK
{
	internal abstract class SuitEvent
	{
		private SuitEventType _eventType;
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
	}

	internal class BasicHapticEvent : SuitEvent
	{
		private float _time;
		private float _strength;
		private float _duration;
		private UInt32 _area;
		private string _effect;

		protected override int _generate(FlatBufferBuilder b)
		{
			var effect = b.CreateString(_effect);
			Events.BasicHapticEvent.StartBasicHapticEvent(b);
			Events.BasicHapticEvent.AddArea(b, _area);
			Events.BasicHapticEvent.AddStrength(b, _strength);
			Events.BasicHapticEvent.AddDuration(b, _duration);
			Events.BasicHapticEvent.AddTime(b, _time);
			Events.BasicHapticEvent.AddEffect(b, effect);
			return Events.BasicHapticEvent.EndBasicHapticEvent(b).Value;
		}
		public BasicHapticEvent(float time, float strength, float duration, UInt32 area, string effect)
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
	
		private IList<SuitEvent> _events;
		public EventList()
		{
			_events = new List<SuitEvent>();
		}

		public void AddEvent(SuitEvent e)
		{
			_events.Add(e);
		}

		public byte[] Generate()
		{
			FlatBufferBuilder builder = new FlatBufferBuilder(1);
			
		
			Offset<Events.SuitEvent>[] generatedEvents = _events.Select(x => x.Generate(builder)).ToArray();
			var offsetGeneratedEvents = Events.SuitEventList.CreateEventsVector(builder, generatedEvents);
			Events.SuitEventList.StartSuitEventList(builder);
			Events.SuitEventList.AddEvents(builder, offsetGeneratedEvents);
			var list = Events.SuitEventList.EndSuitEventList(builder);

			builder.Finish(list.Value);


			return builder.SizedByteArray();
			


		}


	}
}
