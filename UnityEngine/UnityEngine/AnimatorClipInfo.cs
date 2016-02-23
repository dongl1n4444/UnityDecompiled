using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Information about clip been played and blended by the Animator.</para>
	/// </summary>
	[UsedByNativeCode]
	public struct AnimatorClipInfo
	{
		private int m_ClipInstanceID;

		private float m_Weight;

		/// <summary>
		///   <para>Returns the animation clip played by the Animator.</para>
		/// </summary>
		public AnimationClip clip
		{
			get
			{
				return (this.m_ClipInstanceID == 0) ? null : AnimatorClipInfo.ClipInstanceToScriptingObject(this.m_ClipInstanceID);
			}
		}

		/// <summary>
		///   <para>Returns the blending weight used by the Animator to blend this clip.</para>
		/// </summary>
		public float weight
		{
			get
			{
				return this.m_Weight;
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationClip ClipInstanceToScriptingObject(int instanceID);
	}
}
