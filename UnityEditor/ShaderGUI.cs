namespace UnityEditor
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Abstract class to derive from for defining custom GUI for shader properties and for extending the material preview.</para>
    /// </summary>
    public abstract class ShaderGUI
    {
        protected ShaderGUI()
        {
        }

        /// <summary>
        /// <para>This method is called when a new shader has been selected for a Material.</para>
        /// </summary>
        /// <param name="material">The material the newShader should be assigned to.</param>
        /// <param name="oldShader">Previous shader.</param>
        /// <param name="newShader">New shader to assign to the material.</param>
        public virtual void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            material.shader = newShader;
        }

        /// <summary>
        /// <para>Find shader properties.</para>
        /// </summary>
        /// <param name="propertyName">Name of the material property.</param>
        /// <param name="properties">The array of available properties.</param>
        /// <param name="propertyIsMandatory">If true then this method will throw an exception if a property with propertyName was not found.</param>
        /// <returns>
        /// <para>The material property found, otherwise null.</para>
        /// </returns>
        protected static MaterialProperty FindProperty(string propertyName, MaterialProperty[] properties)
        {
            return FindProperty(propertyName, properties, true);
        }

        /// <summary>
        /// <para>Find shader properties.</para>
        /// </summary>
        /// <param name="propertyName">Name of the material property.</param>
        /// <param name="properties">The array of available properties.</param>
        /// <param name="propertyIsMandatory">If true then this method will throw an exception if a property with propertyName was not found.</param>
        /// <returns>
        /// <para>The material property found, otherwise null.</para>
        /// </returns>
        protected static MaterialProperty FindProperty(string propertyName, MaterialProperty[] properties, bool propertyIsMandatory)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if ((properties[i] != null) && (properties[i].name == propertyName))
                {
                    return properties[i];
                }
            }
            if (propertyIsMandatory)
            {
                object[] objArray1 = new object[] { "Could not find MaterialProperty: '", propertyName, "', Num properties: ", properties.Length };
                throw new ArgumentException(string.Concat(objArray1));
            }
            return null;
        }

        /// <summary>
        /// <para>To define a custom shader GUI use the methods of materialEditor to render controls for the properties array.</para>
        /// </summary>
        /// <param name="materialEditor">The MaterialEditor that are calling this OnGUI (the 'owner').</param>
        /// <param name="properties">Material properties of the current selected shader.</param>
        public virtual void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            materialEditor.PropertiesDefaultGUI(properties);
        }

        public virtual void OnMaterialInteractivePreviewGUI(MaterialEditor materialEditor, Rect r, GUIStyle background)
        {
            materialEditor.DefaultPreviewGUI(r, background);
        }

        /// <summary>
        /// <para>Override for extending the rendering of the Preview area or completly replace the preview (by not calling base.OnMaterialPreviewGUI).</para>
        /// </summary>
        /// <param name="materialEditor">The MaterialEditor that are calling this method (the 'owner').</param>
        /// <param name="r">Preview rect.</param>
        /// <param name="background">Style for the background.</param>
        public virtual void OnMaterialPreviewGUI(MaterialEditor materialEditor, Rect r, GUIStyle background)
        {
            materialEditor.DefaultPreviewGUI(r, background);
        }

        /// <summary>
        /// <para>Override for extending the functionality of the toolbar of the preview area or completly replace the toolbar by not calling base.OnMaterialPreviewSettingsGUI.</para>
        /// </summary>
        /// <param name="materialEditor">The MaterialEditor that are calling this method (the 'owner').</param>
        public virtual void OnMaterialPreviewSettingsGUI(MaterialEditor materialEditor)
        {
            materialEditor.DefaultPreviewSettingsGUI();
        }
    }
}

