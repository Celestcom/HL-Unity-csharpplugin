using System;

using NullSpace.SDK;
using NullSpace.SDK.Internal;
using System.ServiceProcess;

namespace NSLoaderWrapper
{

	public unsafe class Program
	{

		static unsafe NSVR_System* systemPtr;

		public static int Main()
		{

		

			fixed (NSVR_System** system_ptr = &systemPtr)
			{
				if (Interop.NSVR_FAILURE(Interop.NSVR_System_Create(system_ptr))) {
					Console.WriteLine("Failed to create nsvr system");
				}
			}
			//NSVR.NSVR_Plugin loader = new NSVR.NSVR_Plugin();
			
			IntPtr eventPtr = IntPtr.Zero;
			Interop.NSVR_Event_Create(ref eventPtr, Interop.NSVR_EventType.Basic_Haptic_Event);
			Interop.NSVR_Event_SetInteger(eventPtr, "effect", 666);
			Interop.NSVR_Event_SetFloat(eventPtr, "duration", 0.5f);
			Interop.NSVR_Event_SetInteger(eventPtr, "area",(int)AreaFlag.All_Areas);

			Interop.NSVR_Event_SetFloat(eventPtr, "time", 0.1f);

			IntPtr timelinePtr = IntPtr.Zero;
			Interop.NSVR_Timeline_Create(ref timelinePtr, systemPtr);

		
			Interop.NSVR_Timeline_AddEvent(timelinePtr, eventPtr);
			Interop.NSVR_Event_SetFloat(eventPtr, "time", 0.8f);
			Interop.NSVR_Timeline_AddEvent(timelinePtr, eventPtr);

			Interop.NSVR_Event_SetFloat(eventPtr, "time", 2.0f);
			Interop.NSVR_Timeline_AddEvent(timelinePtr, eventPtr);


			IntPtr handlePtr = IntPtr.Zero;
			Interop.NSVR_PlaybackHandle_Create(ref handlePtr);
			Interop.NSVR_Timeline_Transmit(timelinePtr, handlePtr);
			Interop.NSVR_Timeline_Release(ref timelinePtr);

			Interop.NSVR_PlaybackHandle_Command(handlePtr, Interop.NSVR_PlaybackCommand.Play);
		//	Interop.NSVR_System_Haptics_Destroy(systemPtr);

			//	fixed (NSVR_System** system_ptr = &systemPtr)
			//	//{
			//Interop.NSVR_System_Release(system_ptr);
			//	}
			Console.WriteLine("hi");
			while (true)
			{
				int i = 3;
			}
			return 0;
		}

	}
}
