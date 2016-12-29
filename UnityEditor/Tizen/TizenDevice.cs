namespace UnityEditor.Tizen
{
    using System;

    public class TizenDevice : IDisposable
    {
        private readonly string _deviceId;

        public TizenDevice(string deviceId)
        {
            this._deviceId = deviceId;
        }

        public void Dispose()
        {
        }

        public string Id =>
            this._deviceId;
    }
}

