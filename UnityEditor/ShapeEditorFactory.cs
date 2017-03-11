namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.U2D.Interface;

    internal class ShapeEditorFactory : IShapeEditorFactory
    {
        public ShapeEditor CreateShapeEditor() => 
            new ShapeEditor(new GUIUtilitySystem(), new EventSystem());
    }
}

