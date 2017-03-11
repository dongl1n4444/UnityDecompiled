namespace UnityEngineInternal
{
    using System;
    using System.Text.RegularExpressions;
    using UnityEngine.Scripting;

    internal static class WebRequestUtils
    {
        private static Regex domainRegex = new Regex(@"^\s*\w+(?:\.\w+)+\s*$");

        internal static string MakeInitialUrl(string targetUrl, string localUrl)
        {
            Uri baseUri = new Uri(localUrl);
            if (targetUrl.StartsWith("//"))
            {
                targetUrl = baseUri.Scheme + ":" + targetUrl;
            }
            if (targetUrl.StartsWith("/"))
            {
                targetUrl = baseUri.Scheme + "://" + baseUri.Host + targetUrl;
            }
            if (domainRegex.IsMatch(targetUrl))
            {
                targetUrl = baseUri.Scheme + "://" + targetUrl;
            }
            Uri uri2 = null;
            try
            {
                uri2 = new Uri(targetUrl);
            }
            catch (FormatException exception)
            {
                try
                {
                    uri2 = new Uri(baseUri, targetUrl);
                }
                catch (FormatException)
                {
                    throw exception;
                }
            }
            return (!targetUrl.Contains("%") ? uri2.AbsoluteUri : uri2.OriginalString);
        }

        [RequiredByNativeCode]
        internal static string RedirectTo(string baseUri, string redirectUri)
        {
            Uri uri;
            if (redirectUri[0] == '/')
            {
                uri = new Uri(redirectUri, UriKind.Relative);
            }
            else
            {
                uri = new Uri(redirectUri, UriKind.RelativeOrAbsolute);
            }
            if (uri.IsAbsoluteUri)
            {
                return redirectUri;
            }
            Uri uri2 = new Uri(baseUri, UriKind.Absolute);
            Uri uri3 = new Uri(uri2, uri);
            return uri3.AbsoluteUri;
        }
    }
}

