namespace UnityScript.MonoDevelop.ProjectModel
{
    using MonoDevelop.Core.Serialization;
    using MonoDevelop.Projects;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UnityScriptCompilationParameters : ConfigurationParameters
    {
        private List<string> defines = new List<string>();
        private static string DefineSeparator = ";";

        [ItemProperty]
        public string DefineConstants
        {
            get => 
                string.Join(DefineSeparator, this.defines);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.defines = new List<string>();
                }
                else
                {
                    this.defines = new List<string>(value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Distinct<string>().ToArray<string>());
                }
            }
        }

        public List<string> DefineSymbols =>
            this.defines;
    }
}

