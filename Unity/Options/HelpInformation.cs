namespace Unity.Options
{
    using System;
    using System.Reflection;

    public class HelpInformation
    {
        public string CustomValueDescription;
        public System.Reflection.FieldInfo FieldInfo;
        public string Summary;

        public bool HasCustomValueDescription
        {
            get
            {
                return !string.IsNullOrEmpty(this.CustomValueDescription);
            }
        }

        public bool HasSummary
        {
            get
            {
                return !string.IsNullOrEmpty(this.Summary);
            }
        }
    }
}

