namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>The PreferenceItem attribute allows you to add preferences sections to the Preferences Window.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PreferenceItem : Attribute
    {
        public string name;

        /// <summary>
        /// <para>Creates a section in the Preferences Window called name and invokes the static function following it for the section's GUI.</para>
        /// </summary>
        /// <param name="name"></param>
        public PreferenceItem(string name)
        {
            this.name = name;
        }
    }
}

