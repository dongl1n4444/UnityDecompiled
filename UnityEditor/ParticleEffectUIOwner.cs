namespace UnityEditor
{
    using System;

    internal interface ParticleEffectUIOwner
    {
        void Repaint();

        Editor customEditor { get; }
    }
}

