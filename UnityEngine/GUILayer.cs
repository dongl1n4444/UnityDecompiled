namespace UnityEngine
{
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Component added to a camera to make it render 2D GUI elements.</para>
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class GUILayer : Behaviour
    {
        /// <summary>
        /// <para>Get the GUI element at a specific screen position.</para>
        /// </summary>
        /// <param name="screenPosition"></param>
        public GUIElement HitTest(Vector3 screenPosition) => 
            INTERNAL_CALL_HitTest(this, ref screenPosition);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern GUIElement INTERNAL_CALL_HitTest(GUILayer self, ref Vector3 screenPosition);
    }
}

