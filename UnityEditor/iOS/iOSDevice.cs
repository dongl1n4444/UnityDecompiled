namespace UnityEditor.iOS
{
    using System;
    using UnityEditor.Modules;
    using UnityEngine;

    [Serializable]
    internal class iOSDevice : IDevice
    {
        [SerializeField]
        private string id;
        [SerializeField]
        private int playerPort = -1;
        [SerializeField]
        private int remotePort = -1;

        public iOSDevice(string id)
        {
            this.id = id;
        }

        private RemoteAddress StartIosProxy(ushort devicePort)
        {
            int num = 0x2774;
            if (num > 0xffff)
            {
                throw new ApplicationException();
            }
            for (ushort i = 0x2710; i < num; i = (ushort) (i + 1))
            {
                if (Usbmuxd.StartIosProxy(i, devicePort, this.id))
                {
                    return new RemoteAddress("127.0.0.1", i);
                }
            }
            throw new ApplicationException("Couldn't start proxy for device");
        }

        public RemoteAddress StartPlayerConnectionSupport()
        {
            RemoteAddress address = this.StartIosProxy(0xd6d8);
            this.playerPort = (ushort) address.port;
            return address;
        }

        public RemoteAddress StartRemoteSupport()
        {
            RemoteAddress address = this.StartIosProxy(0x1c21);
            this.remotePort = (ushort) address.port;
            return address;
        }

        public void StopPlayerConnectionSupport()
        {
            if (this.playerPort != -1)
            {
                Usbmuxd.StopIosProxy((ushort) this.playerPort);
            }
            this.playerPort = -1;
        }

        public void StopRemoteSupport()
        {
            if (this.remotePort != -1)
            {
                Usbmuxd.StopIosProxy((ushort) this.remotePort);
            }
            this.remotePort = -1;
        }

        public string Id =>
            this.id;
    }
}

