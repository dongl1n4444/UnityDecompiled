using System;
using System.Collections.Generic;

namespace UnityEngine
{
	/// <summary>
	///   <para>A component can be designed drive a RectTransform. The DrivenRectTransformTracker struct is used to specify which RectTransforms it is driving.</para>
	/// </summary>
	public struct DrivenRectTransformTracker
	{
		private List<RectTransform> m_Tracked;

		/// <summary>
		///   <para>Add a RectTransform to be driven.</para>
		/// </summary>
		/// <param name="driver">The object to drive properties.</param>
		/// <param name="rectTransform">The RectTransform to be driven.</param>
		/// <param name="drivenProperties">The properties to be driven.</param>
		public void Add(Object driver, RectTransform rectTransform, DrivenTransformProperties drivenProperties)
		{
			if (this.m_Tracked == null)
			{
				this.m_Tracked = new List<RectTransform>();
			}
			if (!Application.isPlaying)
			{
				RuntimeUndo.RecordObject(rectTransform, "Driving RectTransform");
			}
			this.m_Tracked.Add(rectTransform);
			rectTransform.drivenByObject = driver;
			rectTransform.drivenProperties |= drivenProperties;
		}

		/// <summary>
		///   <para>Clear the list of RectTransforms being driven.</para>
		/// </summary>
		public void Clear()
		{
			if (this.m_Tracked != null)
			{
				for (int i = 0; i < this.m_Tracked.Count; i++)
				{
					if (this.m_Tracked[i] != null)
					{
						if (!Application.isPlaying)
						{
							RuntimeUndo.RecordObject(this.m_Tracked[i], "Driving RectTransform");
						}
						this.m_Tracked[i].drivenByObject = null;
					}
				}
				this.m_Tracked.Clear();
			}
		}
	}
}
