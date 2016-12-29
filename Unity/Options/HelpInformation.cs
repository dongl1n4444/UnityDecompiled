namespace Unity.Options
{
    using System;
    using System.Reflection;

    public class HelpInformation
    {
        public string CustomValueDescription;
        public System.Reflection.FieldInfo FieldInfo;
        public string Summary;

        public bool HasCustomValueDescription =>
            !string.IsNullOrEmpty(this.CustomValueDescription);

        public bool HasSummary =>
            !string.IsNullOrEmpty(this.Summary);
    }
}

