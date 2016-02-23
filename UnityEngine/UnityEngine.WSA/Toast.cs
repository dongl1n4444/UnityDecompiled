using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.WSA
{
	/// <summary>
	///   <para>Represents a toast notification in Windows Store Apps.
	/// </para>
	/// </summary>
	public sealed class Toast
	{
		private int m_ToastId;

		/// <summary>
		///   <para>Arguments to be passed for application when toast notification is activated.</para>
		/// </summary>
		public string arguments
		{
			get
			{
				return Toast.GetArguments(this.m_ToastId);
			}
			set
			{
				Toast.SetArguments(this.m_ToastId, value);
			}
		}

		/// <summary>
		///   <para>true if toast was activated by user.</para>
		/// </summary>
		public bool activated
		{
			get
			{
				return Toast.GetActivated(this.m_ToastId);
			}
		}

		/// <summary>
		///   <para>true if toast notification was dismissed (for any reason).</para>
		/// </summary>
		public bool dismissed
		{
			get
			{
				return Toast.GetDismissed(this.m_ToastId, false);
			}
		}

		/// <summary>
		///   <para>true if toast notification was explicitly dismissed by user.</para>
		/// </summary>
		public bool dismissedByUser
		{
			get
			{
				return Toast.GetDismissed(this.m_ToastId, true);
			}
		}

		private Toast(int id)
		{
			this.m_ToastId = id;
		}

		/// <summary>
		///   <para>Get template XML for toast notification.
		/// </para>
		/// </summary>
		/// <param name="templ">A template identifier.</param>
		/// <returns>
		///   <para>string, which is an empty XML document to be filled and used for toast notification.</para>
		/// </returns>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetTemplate(ToastTemplate templ);

		/// <summary>
		///   <para>Create toast notification.</para>
		/// </summary>
		/// <param name="xml">XML document with tile data.</param>
		/// <param name="image">Uri to image to show on a toast, can be empty, in that case text-only notification will be shown.</param>
		/// <param name="text">A text to display on a toast notification.</param>
		/// <returns>
		///   <para>A toast object for further work with created notification or null, if creation of toast failed.</para>
		/// </returns>
		public static Toast Create(string xml)
		{
			int num = Toast.CreateToastXml(xml);
			if (num < 0)
			{
				return null;
			}
			return new Toast(num);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CreateToastXml(string xml);

		/// <summary>
		///   <para>Create toast notification.</para>
		/// </summary>
		/// <param name="xml">XML document with tile data.</param>
		/// <param name="image">Uri to image to show on a toast, can be empty, in that case text-only notification will be shown.</param>
		/// <param name="text">A text to display on a toast notification.</param>
		/// <returns>
		///   <para>A toast object for further work with created notification or null, if creation of toast failed.</para>
		/// </returns>
		public static Toast Create(string image, string text)
		{
			int num = Toast.CreateToastImageAndText(image, text);
			if (num < 0)
			{
				return null;
			}
			return new Toast(num);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CreateToastImageAndText(string image, string text);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetArguments(int id);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetArguments(int id, string args);

		/// <summary>
		///   <para>Show toast notification.</para>
		/// </summary>
		public void Show()
		{
			Toast.Show(this.m_ToastId);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Show(int id);

		/// <summary>
		///   <para>Hide displayed toast notification.</para>
		/// </summary>
		public void Hide()
		{
			Toast.Hide(this.m_ToastId);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Hide(int id);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetActivated(int id);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetDismissed(int id, bool byUser);
	}
}
