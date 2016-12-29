namespace Unity.PackageManager
{
    using System;
    using System.Net;
    using System.Runtime.CompilerServices;

    public static class WebRequestExtensions
    {
        public static WebResponse GetResponseWithoutException(this WebRequest request)
        {
            try
            {
                return request.GetResponse();
            }
            catch (WebException exception)
            {
                return exception.Response;
            }
        }
    }
}

