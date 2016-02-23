using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.Experimental.Networking
{
	/// <summary>
	///   <para>A general-purpose UploadHandler subclass, using a native-code memory buffer.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public sealed class UploadHandlerRaw : UploadHandler
	{
		/// <summary>
		///   <para>General constructor. Contents of the input argument are copied into a native buffer.</para>
		/// </summary>
		/// <param name="data">Raw data to transmit to the remote server.</param>
		public UploadHandlerRaw(byte[] data)
		{
			base.InternalCreateRaw(data);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string InternalGetContentType();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetContentType(string newContentType);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern byte[] InternalGetData();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float InternalGetProgress();

		internal override string GetContentType()
		{
			return this.InternalGetContentType();
		}

		internal override void SetContentType(string newContentType)
		{
			this.InternalSetContentType(newContentType);
		}

		internal override byte[] GetData()
		{
			return this.InternalGetData();
		}

		internal override float GetProgress()
		{
			return this.InternalGetProgress();
		}
	}
}
