namespace UnityEditor.RestService
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal class RestRequestException : Exception
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UnityEditor.RestService.HttpStatusCode <HttpStatusCode>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <RestErrorDescription>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <RestErrorString>k__BackingField;

        public RestRequestException()
        {
        }

        public RestRequestException(UnityEditor.RestService.HttpStatusCode httpStatusCode, string restErrorString) : this(httpStatusCode, restErrorString, null)
        {
        }

        public RestRequestException(UnityEditor.RestService.HttpStatusCode httpStatusCode, string restErrorString, string restErrorDescription)
        {
            this.HttpStatusCode = httpStatusCode;
            this.RestErrorString = restErrorString;
            this.RestErrorDescription = restErrorDescription;
        }

        public UnityEditor.RestService.HttpStatusCode HttpStatusCode { get; set; }

        public string RestErrorDescription { get; set; }

        public string RestErrorString { get; set; }
    }
}

