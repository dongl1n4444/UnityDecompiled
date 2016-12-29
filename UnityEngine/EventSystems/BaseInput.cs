namespace UnityEngine.EventSystems
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Interface to the Input system used by the BaseInputModule. With this it is possible to bypass the Input system with your own but still use the same InputModule. For example this can be used to feed fake input into the UI or interface with a different input system.</para>
    /// </summary>
    public class BaseInput : UIBehaviour
    {
        /// <summary>
        /// <para>Interface to Input.GetAxisRaw. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        /// <param name="axisName"></param>
        public virtual float GetAxisRaw(string axisName) => 
            Input.GetAxisRaw(axisName);

        /// <summary>
        /// <para>Interface to Input.GetButtonDown. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        /// <param name="buttonName"></param>
        public virtual bool GetButtonDown(string buttonName) => 
            Input.GetButtonDown(buttonName);

        /// <summary>
        /// <para>Interface to Input.GetMouseButton. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        /// <param name="button"></param>
        public virtual bool GetMouseButton(int button) => 
            Input.GetMouseButton(button);

        /// <summary>
        /// <para>Interface to Input.GetMouseButtonDown. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        /// <param name="button"></param>
        public virtual bool GetMouseButtonDown(int button) => 
            Input.GetMouseButtonDown(button);

        /// <summary>
        /// <para>Interface to Input.GetMouseButtonUp. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        /// <param name="button"></param>
        public virtual bool GetMouseButtonUp(int button) => 
            Input.GetMouseButtonUp(button);

        /// <summary>
        /// <para>Interface to Input.GetTouch. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        /// <param name="index"></param>
        public virtual Touch GetTouch(int index) => 
            Input.GetTouch(index);

        /// <summary>
        /// <para>Interface to Input.compositionCursorPos. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual Vector2 compositionCursorPos
        {
            get => 
                Input.compositionCursorPos;
            set
            {
                Input.compositionCursorPos = value;
            }
        }

        /// <summary>
        /// <para>Interface to Input.compositionString. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual string compositionString =>
            Input.compositionString;

        /// <summary>
        /// <para>Interface to Input.imeCompositionMode. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual IMECompositionMode imeCompositionMode
        {
            get => 
                Input.imeCompositionMode;
            set
            {
                Input.imeCompositionMode = value;
            }
        }

        /// <summary>
        /// <para>Interface to Input.mousePosition. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual Vector2 mousePosition =>
            Input.mousePosition;

        /// <summary>
        /// <para>Interface to Input.mousePresent. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual bool mousePresent =>
            Input.mousePresent;

        /// <summary>
        /// <para>Interface to Input.mouseScrollDelta. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual Vector2 mouseScrollDelta =>
            Input.mouseScrollDelta;

        /// <summary>
        /// <para>Interface to Input.touchCount. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual int touchCount =>
            Input.touchCount;

        /// <summary>
        /// <para>Interface to Input.touchSupported. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual bool touchSupported =>
            Input.touchSupported;
    }
}

