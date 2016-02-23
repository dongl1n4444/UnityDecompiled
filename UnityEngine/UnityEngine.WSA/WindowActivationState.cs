using System;

namespace UnityEngine.WSA
{
	/// <summary>
	///   <para>Specifies the set of reasons that a windowActivated event was raised.</para>
	/// </summary>
	public enum WindowActivationState
	{
		/// <summary>
		///   <para>The window was activated.</para>
		/// </summary>
		CodeActivated,
		/// <summary>
		///   <para>The window was deactivated.</para>
		/// </summary>
		Deactivated,
		/// <summary>
		///   <para>The window was activated by pointer interaction.</para>
		/// </summary>
		PointerActivated
	}
}
