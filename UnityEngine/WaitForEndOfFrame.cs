namespace UnityEngine
{
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Waits until the end of the frame after all cameras and GUI is rendered, just before displaying the frame on screen.</para>
    /// </summary>
    [RequiredByNativeCode]
    public sealed class WaitForEndOfFrame : YieldInstruction
    {
    }
}

