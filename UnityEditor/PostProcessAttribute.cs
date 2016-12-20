﻿namespace UnityEditor
{
    using System;
    using UnityEngine.Scripting;

    [Obsolete("PostProcessAttribute has been renamed to CallbackOrderAttribute."), RequiredByNativeCode]
    public abstract class PostProcessAttribute : CallbackOrderAttribute
    {
        [Obsolete("PostProcessAttribute has been renamed. Use m_CallbackOrder of CallbackOrderAttribute.")]
        protected int m_PostprocessOrder;

        protected PostProcessAttribute()
        {
        }

        [Obsolete("PostProcessAttribute has been renamed. Use callbackOrder of CallbackOrderAttribute.")]
        internal int GetPostprocessOrder
        {
            get
            {
                return this.m_PostprocessOrder;
            }
        }
    }
}

