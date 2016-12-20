namespace UnityEditor.RestService
{
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class RestRequest
    {
        [CompilerGenerated]
        private static AsyncCallback <>f__mg$cache0;

        private static void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            WebResponse response = ((WebRequest) asynchronousResult.AsyncState).EndGetResponse(asynchronousResult);
            try
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                reader.ReadToEnd();
                reader.Close();
                responseStream.Close();
            }
            finally
            {
                response.Close();
            }
        }

        public static bool Send(string endpoint, string payload, int timeout)
        {
            if (ScriptEditorSettings.ServerURL == null)
            {
                return false;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(payload);
            WebRequest state = WebRequest.Create(ScriptEditorSettings.ServerURL + endpoint);
            state.Timeout = timeout;
            state.Method = "POST";
            state.ContentType = "application/json";
            state.ContentLength = bytes.Length;
            try
            {
                Stream requestStream = state.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return false;
            }
            try
            {
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new AsyncCallback(RestRequest.GetResponseCallback);
                }
                state.BeginGetResponse(<>f__mg$cache0, state);
            }
            catch (Exception exception2)
            {
                Logger.Log(exception2);
                return false;
            }
            return true;
        }
    }
}

