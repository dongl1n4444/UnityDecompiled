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
        public virtual float GetAxisRaw(string axisName)
        {
            return Input.GetAxisRaw(axisName);
        }

        /// <summary>
        /// <para>Interface to Input.GetButtonDown. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        /// <param name="buttonName"></param>
        public virtual bool GetButtonDown(string buttonName)
        {
            return Input.GetButtonDown(buttonName);
        }

        /// <summary>
        /// <para>Interface to Input.GetMouseButton. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        /// <param name="button"></param>
        public virtual bool GetMouseButton(int button)
        {
            return Input.GetMouseButton(button);
        }

        /// <summary>
        /// <para>Interface to Input.GetMouseButtonDown. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        /// <param name="button"></param>
        public virtual bool GetMouseButtonDown(int button)
        {
            return Input.GetMouseButtonDown(button);
        }

        /// <summary>
        /// <para>Interface to Input.GetMouseButtonUp. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        /// <param name="button"></param>
        public virtual bool GetMouseButtonUp(int button)
        {
            return Input.GetMouseButtonUp(button);
        }

        /// <summary>
        /// <para>Interface to Input.GetTouch. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        /// <param name="index"></param>
        public virtual Touch GetTouch(int index)
        {
            return Input.GetTouch(index);
        }

        /// <summary>
        /// <para>Interface to Input.compositionCursorPos. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual Vector2 compositionCursorPos
        {
            get
            {
                return Input.compositionCursorPos;
            }
            set
            {
                Input.compositionCursorPos = value;
            }
        }

        /// <summary>
        /// <para>Interface to Input.compositionString. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual string compositionString
        {
            get
            {
                return Input.compositionString;
            }
        }

        /// <summary>
        /// <para>Interface to Input.imeCompositionMode. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual IMECompositionMode imeCompositionMode
        {
            get
            {
                return Input.imeCompositionMode;
            }
            set
            {
                Input.imeCompositionMode = value;
            }
        }

        /// <summary>
        /// <para>Interface to Input.mousePosition. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual Vector2 mousePosition
        {
            get
            {
                return Input.mousePosition;
            }
        }

        /// <summary>
        /// <para>Interface to Input.mousePresent. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual bool mousePresent
        {
            get
            {
                return Input.mousePresent;
            }
        }

        /// <summary>
        /// <para>Interface to Input.mouseScrollDelta. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual Vector2 mouseScrollDelta
        {
            get
            {
                return Input.mouseScrollDelta;
            }
        }

        /// <summary>
        /// <para>Interface to Input.touchCount. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual int touchCount
        {
            get
            {
                return Input.touchCount;
            }
        }

        /// <summary>
        /// <para>Interface to Input.touchSupported. Can be overridden to provide custom input instead of using the Input class.</para>
        /// </summary>
        public virtual bool touchSupported
        {
            get
            {
                return Input.touchSupported;
            }
        }
    }
}

