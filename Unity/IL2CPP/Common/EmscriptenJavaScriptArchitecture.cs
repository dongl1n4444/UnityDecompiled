namespace Unity.IL2CPP.Common
{
    using System;
    using System.Runtime.CompilerServices;

    public class EmscriptenJavaScriptArchitecture : Unity.IL2CPP.Common.Architecture
    {
        public override int Bits =>
            0x20;

        public override string Name =>
            "EmscriptenJavaScript";
    }
}

