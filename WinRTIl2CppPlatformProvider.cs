using System;
using UnityEditorInternal;

internal class WinRTIl2CppPlatformProvider : BaseIl2CppPlatformProvider
{
    public WinRTIl2CppPlatformProvider() : base(BuildTarget.WSAPlayer, "")
    {
    }

    public override bool enableStackTraces
    {
        get
        {
            return false;
        }
    }

    public override bool loadSymbols
    {
        get
        {
            return true;
        }
    }
}

