namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    public sealed class UnityEventQueueSystem
    {
        public static string GenerateEventIdForPayload(string eventPayloadName)
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return $"REGISTER_EVENT_ID(0x{buffer[0]:X2}{buffer[1]:X2}{buffer[2]:X2}{buffer[3]:X2}{buffer[4]:X2}{buffer[5]:X2}{buffer[6]:X2}{buffer[7]:X2}ULL,0x{buffer[8]:X2}{buffer[9]:X2}{buffer[10]:X2}{buffer[11]:X2}{buffer[12]:X2}{buffer[13]:X2}{buffer[14]:X2}{buffer[15]:X2}ULL,{eventPayloadName})";
        }

        public static IntPtr GetGlobalEventQueue()
        {
            IntPtr ptr;
            INTERNAL_CALL_GetGlobalEventQueue(out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetGlobalEventQueue(out IntPtr value);
    }
}

