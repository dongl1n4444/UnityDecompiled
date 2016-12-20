namespace UnityEngine.UI
{
    using UnityEngine;

    public interface IMaterialModifier
    {
        /// <summary>
        /// <para>Perform material modification in this function.</para>
        /// </summary>
        /// <param name="baseMaterial">Configured Material.</param>
        /// <returns>
        /// <para>Modified material.</para>
        /// </returns>
        Material GetModifiedMaterial(Material baseMaterial);
    }
}

