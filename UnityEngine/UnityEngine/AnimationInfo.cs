using System;
using System.ComponentModel;

namespace UnityEngine
{
	/// <summary>
	///   <para>Information about what animation clips is played and its weight.</para>
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use AnimatorClipInfo instead (UnityUpgradable) -> AnimatorClipInfo", true)]
	public struct AnimationInfo
	{
		/// <summary>
		///   <para>Animation clip that is played.</para>
		/// </summary>
		public AnimationClip clip
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		///   <para>The weight of the animation clip.</para>
		/// </summary>
		public float weight
		{
			get
			{
				return 0f;
			}
		}
	}
}
