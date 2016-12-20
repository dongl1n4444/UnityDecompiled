namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal abstract class CameraController
    {
        protected CameraController()
        {
        }

        public abstract void Update(CameraState cameraState, Camera cam);
    }
}

