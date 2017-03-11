namespace UnityEditor.RestService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEditorInternal;

    internal abstract class Handler
    {
        [CompilerGenerated]
        private static Func<string, JSONValue> <>f__am$cache0;

        protected Handler()
        {
        }

        private static void CallSafely(Request request, string payload, Response writeResponse, Func<Request, JSONValue, JSONValue> method)
        {
            RestRequestException exception;
            try
            {
                JSONValue value2 = 0;
                if (payload.Trim().Length == 0)
                {
                    value2 = new JSONValue();
                }
                else
                {
                    try
                    {
                        value2 = new JSONParser(request.Payload).Parse();
                    }
                    catch (JSONParseException)
                    {
                        ThrowInvalidJSONException();
                    }
                }
                writeResponse.SimpleResponse(HttpStatusCode.Ok, method(request, value2).ToString());
            }
            catch (JSONTypeException)
            {
                ThrowInvalidJSONException();
            }
            catch (KeyNotFoundException)
            {
                exception = new RestRequestException {
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
                RespondWithException(writeResponse, exception);
            }
            catch (RestRequestException exception2)
            {
                RespondWithException(writeResponse, exception2);
            }
            catch (Exception exception3)
            {
                exception = new RestRequestException {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    RestErrorString = "InternalServerError",
                    RestErrorDescription = "Caught exception while fulfilling request: " + exception3
                };
                RespondWithException(writeResponse, exception);
            }
        }

        protected virtual JSONValue HandleDelete(Request request, JSONValue payload)
        {
            RestRequestException exception = new RestRequestException {
                HttpStatusCode = HttpStatusCode.MethodNotAllowed,
                RestErrorString = "MethodNotAllowed",
                RestErrorDescription = "This endpoint does not support the DELETE verb."
            };
            throw exception;
        }

        protected virtual JSONValue HandleGet(Request request, JSONValue payload)
        {
            RestRequestException exception = new RestRequestException {
                HttpStatusCode = HttpStatusCode.MethodNotAllowed,
                RestErrorString = "MethodNotAllowed",
                RestErrorDescription = "This endpoint does not support the GET verb."
            };
            throw exception;
        }

        protected virtual JSONValue HandlePost(Request request, JSONValue payload)
        {
            RestRequestException exception = new RestRequestException {
                HttpStatusCode = HttpStatusCode.MethodNotAllowed,
                RestErrorString = "MethodNotAllowed",
                RestErrorDescription = "This endpoint does not support the POST verb."
            };
            throw exception;
        }

        private void InvokeDelete(Request request, string payload, Response writeResponse)
        {
            CallSafely(request, payload, writeResponse, new Func<Request, JSONValue, JSONValue>(this.HandleDelete));
        }

        private void InvokeGet(Request request, string payload, Response writeResponse)
        {
            CallSafely(request, payload, writeResponse, new Func<Request, JSONValue, JSONValue>(this.HandleGet));
        }

        private void InvokePost(Request request, string payload, Response writeResponse)
        {
            CallSafely(request, payload, writeResponse, new Func<Request, JSONValue, JSONValue>(this.HandlePost));
        }

        private static void RespondWithException(Response writeResponse, RestRequestException rre)
        {
            StringBuilder builder = new StringBuilder("{");
            if (rre.RestErrorString != null)
            {
                builder.AppendFormat("\"error\":\"{0}\",", rre.RestErrorString);
            }
            if (rre.RestErrorDescription != null)
            {
                builder.AppendFormat("\"errordescription\":\"{0}\"", rre.RestErrorDescription);
            }
            builder.Append("}");
            writeResponse.SimpleResponse(rre.HttpStatusCode, builder.ToString());
        }

        private static void ThrowInvalidJSONException()
        {
            RestRequestException exception = new RestRequestException {
                HttpStatusCode = HttpStatusCode.BadRequest,
                RestErrorString = "Invalid JSON"
            };
            throw exception;
        }

        protected static JSONValue ToJSON(IEnumerable<string> strings)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = s => new JSONValue(s);
            }
            return new JSONValue(Enumerable.Select<string, JSONValue>(strings, <>f__am$cache0).ToList<JSONValue>());
        }
    }
}

