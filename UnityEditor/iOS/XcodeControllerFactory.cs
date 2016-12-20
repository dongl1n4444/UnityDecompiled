namespace UnityEditor.iOS
{
    using System;

    public class XcodeControllerFactory
    {
        public static IXcodeController CreateXcodeController()
        {
            if (XCodeUtils.GetXcodeMajorVersionNumber() < 8)
            {
                return new XcodeController();
            }
            return new XcodeScriptingController();
        }
    }
}

