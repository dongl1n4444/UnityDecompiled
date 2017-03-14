namespace Unity.IL2CPP.Running.Android
{
    using NiceIO;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.Common;

    internal class AndroidDevice
    {
        [CompilerGenerated]
        private static Func<AndroidDevice, bool> <>f__am$cache0;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string <Abi>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string <Id>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string <ProductId>k__BackingField;
        private const string DefaultDeviceProductId = "foster";

        public AndroidDevice(string id)
        {
            this.<Id>k__BackingField = id;
            this.<Abi>k__BackingField = this.GetAbi();
            this.<ProductId>k__BackingField = this.GetProductId();
        }

        public string ExecuteCommand(string command, bool throwOnError = true) => 
            AndroidDebugBridge.ExecuteCommand($"-s {this.Id} {command}", throwOnError);

        public string ExecuteExecOutCommand(string command, bool throwOnError = true) => 
            this.ExecuteCommand($"exec-out {command}", throwOnError);

        public string ExecuteShellCommand(string command, bool throwOnError = true) => 
            this.ExecuteCommand($"shell {command}", throwOnError);

        public void ForceStop(string package)
        {
            this.ExecuteShellCommand($"am force-stop {package}", false);
        }

        private string GetAbi()
        {
            try
            {
                string[] nonEmptyLines = AndroidDebugBridge.GetNonEmptyLines(this.ExecuteShellCommand("getprop ro.product.cpu.abi", true));
                string str2 = (nonEmptyLines.Length <= 0) ? null : nonEmptyLines[0].Trim();
                if (!string.IsNullOrEmpty(str2))
                {
                    return str2;
                }
            }
            catch
            {
            }
            return "unknown";
        }

        private static string GetAbi(Unity.IL2CPP.Common.Architecture architecture)
        {
            if (architecture is ARMv7Architecture)
            {
                return "arm";
            }
            if (!(architecture is x86Architecture))
            {
                throw new ArgumentException($"Unsupported architecture {architecture.Name}.", "architecture");
            }
            return "x86";
        }

        public static AndroidDevice GetDefault(Unity.IL2CPP.Common.Architecture architecture)
        {
            <GetDefault>c__AnonStorey0 storey = new <GetDefault>c__AnonStorey0 {
                abi = GetAbi(architecture).ToLowerInvariant()
            };
            AndroidDevice[] source = AndroidDebugBridge.GetAllDevices().Where<AndroidDevice>(new Func<AndroidDevice, bool>(storey.<>m__0)).ToArray<AndroidDevice>();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = d => d.ProductId == "foster";
            }
            AndroidDevice device = source.FirstOrDefault<AndroidDevice>(<>f__am$cache0);
            if (device != null)
            {
                return device;
            }
            if (source.Length == 0)
            {
                throw new Exception($"Android device with {architecture.Name} architecture not found.");
            }
            return source[0];
        }

        private string GetProductId()
        {
            try
            {
                string[] nonEmptyLines = AndroidDebugBridge.GetNonEmptyLines(this.ExecuteShellCommand("getprop ro.build.product", true));
                string str2 = (nonEmptyLines.Length <= 0) ? null : nonEmptyLines[0].Trim();
                if (!string.IsNullOrEmpty(str2))
                {
                    return str2;
                }
            }
            catch
            {
            }
            return "unknown";
        }

        public bool Install(NPath path) => 
            this.ExecuteCommand("install " + path.InQuotes(), true).Contains("Success");

        public bool Install(string path) => 
            this.Install(new NPath(path));

        public bool Run(string package, string activity, KeyValuePair<string, string>? extra = new KeyValuePair<string, string>?())
        {
            string command = $"am start -a android.intent.action.MAIN -n {package}/{activity}";
            if (extra.HasValue)
            {
                command = command + $" -e "{extra.Value.Key}" "{extra.Value.Value}"";
            }
            return this.ExecuteShellCommand(command, true).Contains("Starting");
        }

        public override string ToString() => 
            $"{this.Id} ({this.ProductId})";

        public bool Uninstall(string package) => 
            this.ExecuteCommand("uninstall " + package, false).Contains("Succees");

        public string Abi =>
            this.<Abi>k__BackingField;

        public string Id =>
            this.<Id>k__BackingField;

        public string ProductId =>
            this.<ProductId>k__BackingField;

        [CompilerGenerated]
        private sealed class <GetDefault>c__AnonStorey0
        {
            internal string abi;

            internal bool <>m__0(AndroidDevice d) => 
                d.Abi.ToLowerInvariant().Contains(this.abi);
        }
    }
}

