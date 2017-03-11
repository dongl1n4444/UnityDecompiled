namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using UnityEditor.VersionControl;
    using UnityEditorInternal;
    using UnityEditorInternal.VersionControl;
    using UnityEngine;

    internal class AssetModificationProcessorInternal
    {
        private static IEnumerable<System.Type> assetModificationProcessors = null;
        internal static MethodInfo[] isOpenForEditMethods = null;

        private static bool CheckArguments(object[] args, MethodInfo method)
        {
            System.Type[] types = new System.Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                types[i] = args[i].GetType();
            }
            return CheckArgumentTypes(types, method);
        }

        private static bool CheckArgumentsAndReturnType(object[] args, MethodInfo method, System.Type returnType)
        {
            System.Type[] types = new System.Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                types[i] = args[i].GetType();
            }
            return CheckArgumentTypesAndReturnType(types, method, returnType);
        }

        private static bool CheckArgumentTypes(System.Type[] types, MethodInfo method)
        {
            System.Reflection.ParameterInfo[] parameters = method.GetParameters();
            if (types.Length != parameters.Length)
            {
                string[] textArray1 = new string[] { "Parameter count did not match. Expected: ", types.Length.ToString(), " Got: ", parameters.Length.ToString(), " in ", method.DeclaringType.ToString(), ".", method.Name };
                Debug.LogWarning(string.Concat(textArray1));
                return false;
            }
            int index = 0;
            foreach (System.Type type in types)
            {
                System.Reflection.ParameterInfo info = parameters[index];
                if (type != info.ParameterType)
                {
                    Debug.LogWarning(string.Concat(new object[] { "Parameter type mismatch at parameter ", index, ". Expected: ", type.ToString(), " Got: ", info.ParameterType.ToString(), " in ", method.DeclaringType.ToString(), ".", method.Name }));
                    return false;
                }
                index++;
            }
            return true;
        }

        private static bool CheckArgumentTypesAndReturnType(System.Type[] types, MethodInfo method, System.Type returnType)
        {
            if (returnType != method.ReturnType)
            {
                Debug.LogWarning("Return type mismatch. Expected: " + returnType.ToString() + " Got: " + method.ReturnType.ToString() + " in " + method.DeclaringType.ToString() + "." + method.Name);
                return false;
            }
            return CheckArgumentTypes(types, method);
        }

        private static void FileModeChanged(string[] assets, UnityEditor.VersionControl.FileMode mode)
        {
            if (Provider.enabled && Provider.PromptAndCheckoutIfNeeded(assets, ""))
            {
                Provider.SetFileMode(assets, mode);
            }
        }

        internal static MethodInfo[] GetIsOpenForEditMethods()
        {
            if (isOpenForEditMethods == null)
            {
                List<MethodInfo> list = new List<MethodInfo>();
                foreach (System.Type type in AssetModificationProcessors)
                {
                    MethodInfo method = type.GetMethod("IsOpenForEdit", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                    if (method != null)
                    {
                        RequireTeamLicense();
                        string str = "";
                        bool flag = false;
                        System.Type[] types = new System.Type[] { str.GetType(), str.GetType().MakeByRefType() };
                        if (CheckArgumentTypesAndReturnType(types, method, flag.GetType()))
                        {
                            list.Add(method);
                        }
                    }
                }
                isOpenForEditMethods = list.ToArray();
            }
            return isOpenForEditMethods;
        }

        internal static bool IsOpenForEdit(string assetPath, out string message, StatusQueryOptions statusOptions)
        {
            message = "";
            if (string.IsNullOrEmpty(assetPath))
            {
                return true;
            }
            bool flag2 = AssetModificationHook.IsOpenForEdit(assetPath, out message, statusOptions);
            foreach (MethodInfo info in GetIsOpenForEditMethods())
            {
                object[] parameters = new object[] { assetPath, message };
                if (!((bool) info.Invoke(null, parameters)))
                {
                    message = parameters[1] as string;
                    return false;
                }
            }
            return flag2;
        }

        internal static void OnStatusUpdated()
        {
            WindowPending.OnStatusUpdated();
            foreach (System.Type type in AssetModificationProcessors)
            {
                MethodInfo method = type.GetMethod("OnStatusUpdated", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (method != null)
                {
                    RequireTeamLicense();
                    object[] args = new object[0];
                    if (CheckArgumentsAndReturnType(args, method, typeof(void)))
                    {
                        method.Invoke(null, args);
                    }
                }
            }
        }

        private static void OnWillCreateAsset(string path)
        {
            foreach (System.Type type in AssetModificationProcessors)
            {
                MethodInfo method = type.GetMethod("OnWillCreateAsset", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (method != null)
                {
                    object[] args = new object[] { path };
                    if (CheckArguments(args, method))
                    {
                        method.Invoke(null, args);
                    }
                }
            }
        }

        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            AssetDeleteResult didNotDelete = AssetDeleteResult.DidNotDelete;
            if (!InternalEditorUtility.HasTeamLicense())
            {
                return didNotDelete;
            }
            foreach (System.Type type in AssetModificationProcessors)
            {
                MethodInfo method = type.GetMethod("OnWillDeleteAsset", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (method != null)
                {
                    RequireTeamLicense();
                    object[] args = new object[] { assetPath, options };
                    if (CheckArgumentsAndReturnType(args, method, didNotDelete.GetType()))
                    {
                        didNotDelete |= (AssetDeleteResult) method.Invoke(null, args);
                    }
                }
            }
            if (didNotDelete != AssetDeleteResult.DidNotDelete)
            {
                return didNotDelete;
            }
            return AssetModificationHook.OnWillDeleteAsset(assetPath, options);
        }

        private static AssetMoveResult OnWillMoveAsset(string fromPath, string toPath, string[] newPaths, string[] NewMetaPaths)
        {
            AssetMoveResult didNotMove = AssetMoveResult.DidNotMove;
            if (InternalEditorUtility.HasTeamLicense())
            {
                didNotMove = AssetModificationHook.OnWillMoveAsset(fromPath, toPath);
                foreach (System.Type type in AssetModificationProcessors)
                {
                    MethodInfo method = type.GetMethod("OnWillMoveAsset", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                    if (method != null)
                    {
                        RequireTeamLicense();
                        object[] args = new object[] { fromPath, toPath };
                        if (CheckArgumentsAndReturnType(args, method, didNotMove.GetType()))
                        {
                            didNotMove |= (AssetMoveResult) method.Invoke(null, args);
                        }
                    }
                }
            }
            return didNotMove;
        }

        private static void OnWillSaveAssets(string[] assets, out string[] assetsThatShouldBeSaved, out string[] assetsThatShouldBeReverted, int explicitlySaveAsset)
        {
            assetsThatShouldBeReverted = new string[0];
            assetsThatShouldBeSaved = assets;
            bool flag = ((assets.Length > 0) && EditorPrefs.GetBool("VerifySavingAssets", false)) && InternalEditorUtility.isHumanControllingUs;
            if (((explicitlySaveAsset != 0) && (assets.Length == 1)) && (assets[0].EndsWith(".unity") || assets[0].EndsWith(".prefab")))
            {
                flag = false;
            }
            if (flag)
            {
                AssetSaveDialog.ShowWindow(assets, out assetsThatShouldBeSaved);
            }
            else
            {
                assetsThatShouldBeSaved = assets;
            }
            foreach (System.Type type in AssetModificationProcessors)
            {
                MethodInfo method = type.GetMethod("OnWillSaveAssets", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (method != null)
                {
                    object[] args = new object[] { assetsThatShouldBeSaved };
                    if (CheckArguments(args, method))
                    {
                        string[] strArray = (string[]) method.Invoke(null, args);
                        if (strArray != null)
                        {
                            assetsThatShouldBeSaved = strArray;
                        }
                    }
                }
            }
            if (assetsThatShouldBeSaved != null)
            {
                List<string> list = new List<string>();
                foreach (string str in assetsThatShouldBeSaved)
                {
                    if (!AssetDatabase.IsOpenForEdit(str, StatusQueryOptions.ForceUpdate))
                    {
                        list.Add(str);
                    }
                }
                assets = list.ToArray();
                if ((assets.Length != 0) && !Provider.PromptAndCheckoutIfNeeded(assets, ""))
                {
                    Debug.LogError("Could not check out the following files in version control before saving: " + string.Join(", ", assets));
                    assetsThatShouldBeSaved = new string[0];
                }
            }
        }

        private static void RequireTeamLicense()
        {
            if (!InternalEditorUtility.HasTeamLicense())
            {
                throw new MethodAccessException("Requires team license");
            }
        }

        private static IEnumerable<System.Type> AssetModificationProcessors
        {
            get
            {
                if (assetModificationProcessors == null)
                {
                    List<System.Type> list = new List<System.Type>();
                    list.AddRange(EditorAssemblies.SubclassesOf(typeof(UnityEditor.AssetModificationProcessor)));
                    list.AddRange(EditorAssemblies.SubclassesOf(typeof(AssetModificationProcessor)));
                    assetModificationProcessors = list.ToArray();
                }
                return assetModificationProcessors;
            }
        }

        private enum FileMode
        {
            Binary,
            Text
        }
    }
}

