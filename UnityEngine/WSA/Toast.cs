namespace UnityEngine.WSA
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Represents a toast notification in Windows Store Apps.
    /// </para>
    /// </summary>
    public sealed class Toast
    {
        private int m_ToastId;

        private Toast(int id)
        {
            this.m_ToastId = id;
        }

        /// <summary>
        /// <para>Create toast notification.</para>
        /// </summary>
        /// <param name="xml">XML document with tile data.</param>
        /// <param name="image">Uri to image to show on a toast, can be empty, in that case text-only notification will be shown.</param>
        /// <param name="text">A text to display on a toast notification.</param>
        /// <returns>
        /// <para>A toast object for further work with created notification or null, if creation of toast failed.</para>
        /// </returns>
        public static Toast Create(string xml)
        {
            int id = CreateToastXml(xml);
            if (id < 0)
            {
                return null;
            }
            return new Toast(id);
        }

        /// <summary>
        /// <para>Create toast notification.</para>
        /// </summary>
        /// <param name="xml">XML document with tile data.</param>
        /// <param name="image">Uri to image to show on a toast, can be empty, in that case text-only notification will be shown.</param>
        /// <param name="text">A text to display on a toast notification.</param>
        /// <returns>
        /// <para>A toast object for further work with created notification or null, if creation of toast failed.</para>
        /// </returns>
        public static Toast Create(string image, string text)
        {
            int id = CreateToastImageAndText(image, text);
            if (id < 0)
            {
                return null;
            }
            return new Toast(id);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int CreateToastImageAndText(string image, string text);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int CreateToastXml(string xml);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool GetActivated(int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string GetArguments(int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool GetDismissed(int id, bool byUser);
        /// <summary>
        /// <para>Get template XML for toast notification.
        /// </para>
        /// </summary>
        /// <param name="templ">A template identifier.</param>
        /// <returns>
        /// <para>string, which is an empty XML document to be filled and used for toast notification.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetTemplate(ToastTemplate templ);
        /// <summary>
        /// <para>Hide displayed toast notification.</para>
        /// </summary>
        public void Hide()
        {
            Hide(this.m_ToastId);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Hide(int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetArguments(int id, string args);
        /// <summary>
        /// <para>Show toast notification.</para>
        /// </summary>
        public void Show()
        {
            Show(this.m_ToastId);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Show(int id);

        /// <summary>
        /// <para>true if toast was activated by user.</para>
        /// </summary>
        public bool activated
        {
            get
            {
                return GetActivated(this.m_ToastId);
            }
        }

        /// <summary>
        /// <para>Arguments to be passed for application when toast notification is activated.</para>
        /// </summary>
        public string arguments
        {
            get
            {
                return GetArguments(this.m_ToastId);
            }
            set
            {
                SetArguments(this.m_ToastId, value);
            }
        }

        /// <summary>
        /// <para>true if toast notification was dismissed (for any reason).</para>
        /// </summary>
        public bool dismissed
        {
            get
            {
                return GetDismissed(this.m_ToastId, false);
            }
        }

        /// <summary>
        /// <para>true if toast notification was explicitly dismissed by user.</para>
        /// </summary>
        public bool dismissedByUser
        {
            get
            {
                return GetDismissed(this.m_ToastId, true);
            }
        }
    }
}

