namespace UnityEditor.RestService
{
    using System;
    using UnityEditor;
    using UnityEditorInternal;

    internal class PlayModeRestHandler : Handler
    {
        internal string CurrentState()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return "stopped";
            }
            return (!EditorApplication.isPaused ? "playing" : "paused");
        }

        protected override JSONValue HandleGet(Request request, JSONValue payload)
        {
            JSONValue value2 = new JSONValue();
            value2["state"] = this.CurrentState();
            return value2;
        }

        protected override JSONValue HandlePost(Request request, JSONValue payload)
        {
            string str = payload.Get("action").AsString();
            string str2 = this.CurrentState();
            switch (str)
            {
                case "play":
                    EditorApplication.isPlaying = true;
                    EditorApplication.isPaused = false;
                    break;

                case "pause":
                    EditorApplication.isPaused = true;
                    break;

                case "stop":
                    EditorApplication.isPlaying = false;
                    break;

                default:
                {
                    RestRequestException exception = new RestRequestException {
                        HttpStatusCode = HttpStatusCode.BadRequest,
                        RestErrorString = "Invalid action: " + str
                    };
                    throw exception;
                }
            }
            JSONValue value3 = new JSONValue();
            value3["oldstate"] = str2;
            value3["newstate"] = this.CurrentState();
            return value3;
        }

        internal static void Register()
        {
            Router.RegisterHandler("/unity/playmode", new PlayModeRestHandler());
        }
    }
}

