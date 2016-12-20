﻿namespace UnityEditor.RestService
{
    using System;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AssetRestHandler
    {
        internal static void Register()
        {
            Router.RegisterHandler("/unity/assets", new LibraryHandler());
            Router.RegisterHandler("/unity/assets/*", new AssetHandler());
        }

        internal class AssetHandler : Handler
        {
            internal void CreateAsset(string assetPath, string contents)
            {
                string fullPath = Path.GetFullPath(assetPath);
                try
                {
                    using (StreamWriter writer = new StreamWriter(File.OpenWrite(fullPath)))
                    {
                        writer.Write(contents);
                        writer.Close();
                    }
                }
                catch (Exception exception)
                {
                    throw new RestRequestException(HttpStatusCode.BadRequest, "FailedCreatingAsset", "Caught exception: " + exception);
                }
            }

            internal JSONValue GetAssetText(string assetPath)
            {
                Object obj2 = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
                if (obj2 == null)
                {
                    throw new RestRequestException(HttpStatusCode.BadRequest, "AssetNotFound");
                }
                JSONValue value2 = new JSONValue();
                value2["file"] = assetPath;
                value2["contents"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(obj2.ToString()));
                return value2;
            }

            protected override JSONValue HandleDelete(Request request, JSONValue payload)
            {
                if (!AssetDatabase.DeleteAsset(request.Url.Substring("/unity/".Length)))
                {
                    RestRequestException exception = new RestRequestException {
                        HttpStatusCode = HttpStatusCode.InternalServerError,
                        RestErrorString = "FailedDeletingAsset",
                        RestErrorDescription = "DeleteAsset() returned false"
                    };
                    throw exception;
                }
                return new JSONValue();
            }

            protected override JSONValue HandleGet(Request request, JSONValue payload)
            {
                int index = request.Url.ToLowerInvariant().IndexOf("/assets/");
                string assetPath = request.Url.ToLowerInvariant().Substring(index + 1);
                return this.GetAssetText(assetPath);
            }

            protected override JSONValue HandlePost(Request request, JSONValue payload)
            {
                string str = payload.Get("action").AsString();
                switch (str)
                {
                    case "move":
                    {
                        string from = request.Url.Substring("/unity/".Length);
                        string to = payload.Get("newpath").AsString();
                        this.MoveAsset(from, to);
                        break;
                    }
                    case "create":
                    {
                        string assetPath = request.Url.Substring("/unity/".Length);
                        byte[] bytes = Convert.FromBase64String(payload.Get("contents").AsString());
                        string contents = Encoding.UTF8.GetString(bytes);
                        this.CreateAsset(assetPath, contents);
                        break;
                    }
                    default:
                    {
                        RestRequestException exception = new RestRequestException {
                            HttpStatusCode = HttpStatusCode.BadRequest,
                            RestErrorString = "Uknown action: " + str
                        };
                        throw exception;
                    }
                }
                return new JSONValue();
            }

            internal bool MoveAsset(string from, string to)
            {
                string str = AssetDatabase.MoveAsset(from, to);
                if (str.Length > 0)
                {
                    throw new RestRequestException(HttpStatusCode.BadRequest, "MoveAsset failed with error: " + str);
                }
                return (str.Length == 0);
            }
        }

        internal class LibraryHandler : Handler
        {
            protected override JSONValue HandleGet(Request request, JSONValue payload)
            {
                JSONValue value2 = new JSONValue();
                string[] searchInFolders = new string[] { "Assets" };
                value2["assets"] = Handler.ToJSON(AssetDatabase.FindAssets("", searchInFolders));
                return value2;
            }
        }
    }
}

