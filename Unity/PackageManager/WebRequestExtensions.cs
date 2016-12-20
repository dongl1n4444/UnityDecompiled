namespace Unity.PackageManager
{
    using System;
    using System.Net;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class WebRequestExtensions
    {
        [Extension]
        public static WebResponse GetResponseWithoutException(WebRequest request)
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

