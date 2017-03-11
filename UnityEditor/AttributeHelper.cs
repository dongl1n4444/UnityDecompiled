namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Scripting;

    internal class AttributeHelper
    {
        [DebuggerHidden]
        internal static IEnumerable<T> CallMethodsWithAttribute<T>(System.Type attributeType, params object[] arguments) => 
            new <CallMethodsWithAttribute>c__Iterator0<T> { 
                attributeType = attributeType,
                arguments = arguments,
                $PC = -2
            };

        [RequiredByNativeCode]
        private static MonoMenuItem[] ExtractContextMenu(System.Type type)
        {
            Dictionary<string, MonoMenuItem> dictionary = new Dictionary<string, MonoMenuItem>();
            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < methods.GetLength(0); i++)
            {
                MethodInfo mi = methods[i];
                object[] customAttributes = mi.GetCustomAttributes(typeof(ContextMenu), false);
                foreach (ContextMenu menu in customAttributes)
                {
                    MonoMenuItem item = !dictionary.ContainsKey(menu.menuItem) ? new MonoMenuItem() : dictionary[menu.menuItem];
                    if (!ValidateMethodForMenuCommand(mi, true))
                    {
                        break;
                    }
                    item.menuItem = menu.menuItem;
                    if (menu.validate)
                    {
                        item.validateType = type;
                        item.validateMethod = mi;
                        item.validateName = mi.Name;
                    }
                    else
                    {
                        item.index = i;
                        item.priority = menu.priority;
                        item.executeType = type;
                        item.executeMethod = mi;
                        item.executeName = mi.Name;
                    }
                    dictionary[menu.menuItem] = item;
                }
            }
            MonoMenuItem[] array = dictionary.Values.ToArray<MonoMenuItem>();
            Array.Sort(array, new CompareMenuIndex());
            return array;
        }

        [RequiredByNativeCode]
        private static MonoCreateAssetItem[] ExtractCreateAssetMenuItems(Assembly assembly)
        {
            List<MonoCreateAssetItem> list = new List<MonoCreateAssetItem>();
            System.Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
            foreach (System.Type type in typesFromAssembly)
            {
                CreateAssetMenuAttribute customAttribute = (CreateAssetMenuAttribute) Attribute.GetCustomAttribute(type, typeof(CreateAssetMenuAttribute));
                if (customAttribute != null)
                {
                    if (!type.IsSubclassOf(typeof(ScriptableObject)))
                    {
                        object[] args = new object[] { type.FullName };
                        UnityEngine.Debug.LogWarningFormat("CreateAssetMenu attribute on {0} will be ignored as {0} is not derived from ScriptableObject.", args);
                    }
                    else
                    {
                        string str = !string.IsNullOrEmpty(customAttribute.menuName) ? customAttribute.menuName : ObjectNames.NicifyVariableName(type.Name);
                        string path = !string.IsNullOrEmpty(customAttribute.fileName) ? customAttribute.fileName : ("New " + ObjectNames.NicifyVariableName(type.Name) + ".asset");
                        if (!Path.HasExtension(path))
                        {
                            path = path + ".asset";
                        }
                        MonoCreateAssetItem item = new MonoCreateAssetItem {
                            menuItem = str,
                            fileName = path,
                            order = customAttribute.order,
                            type = type
                        };
                        list.Add(item);
                    }
                }
            }
            return list.ToArray();
        }

        [RequiredByNativeCode]
        private static MonoGizmoMethod[] ExtractGizmos(Assembly assembly)
        {
            List<MonoGizmoMethod> list = new List<MonoGizmoMethod>();
            System.Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
            foreach (System.Type type in typesFromAssembly)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                for (int i = 0; i < methods.GetLength(0); i++)
                {
                    MethodInfo info = methods[i];
                    object[] customAttributes = info.GetCustomAttributes(typeof(DrawGizmo), false);
                    foreach (DrawGizmo gizmo in customAttributes)
                    {
                        System.Reflection.ParameterInfo[] parameters = info.GetParameters();
                        if (parameters.Length != 2)
                        {
                            UnityEngine.Debug.LogWarning($"Method {info.DeclaringType.FullName}.{info.Name} is marked with the DrawGizmo attribute but does not take parameters (ComponentType, GizmoType) so will be ignored.");
                        }
                        else
                        {
                            MonoGizmoMethod item = new MonoGizmoMethod();
                            if (gizmo.drawnType == null)
                            {
                                item.drawnType = parameters[0].ParameterType;
                            }
                            else if (parameters[0].ParameterType.IsAssignableFrom(gizmo.drawnType))
                            {
                                item.drawnType = gizmo.drawnType;
                            }
                            else
                            {
                                UnityEngine.Debug.LogWarning($"Method {info.DeclaringType.FullName}.{info.Name} is marked with the DrawGizmo attribute but the component type it applies to could not be determined.");
                                goto Label_0198;
                            }
                            if ((parameters[1].ParameterType != typeof(GizmoType)) && (parameters[1].ParameterType != typeof(int)))
                            {
                                UnityEngine.Debug.LogWarning($"Method {info.DeclaringType.FullName}.{info.Name} is marked with the DrawGizmo attribute but does not take a second parameter of type GizmoType so will be ignored.");
                            }
                            else
                            {
                                item.drawGizmo = info;
                                item.options = (int) gizmo.drawOptions;
                                list.Add(item);
                            }
                        Label_0198:;
                        }
                    }
                }
            }
            return list.ToArray();
        }

        [RequiredByNativeCode]
        private static MonoMenuItem[] ExtractMenuCommands(Assembly assembly, bool modifiedSinceLastReload)
        {
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            if (modifiedSinceLastReload)
            {
                bindingAttr |= BindingFlags.Instance;
            }
            bool @bool = EditorPrefs.GetBool("InternalMode", false);
            Dictionary<string, MonoMenuItem> dictionary = new Dictionary<string, MonoMenuItem>();
            System.Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
            foreach (System.Type type in typesFromAssembly)
            {
                MethodInfo[] methods = type.GetMethods(bindingAttr);
                for (int i = 0; i < methods.GetLength(0); i++)
                {
                    MethodInfo mi = methods[i];
                    object[] customAttributes = mi.GetCustomAttributes(typeof(UnityEditor.MenuItem), false);
                    if ((customAttributes.Length > 0) && type.IsGenericTypeDefinition)
                    {
                        object[] args = new object[] { type.Name, mi.Name };
                        UnityEngine.Debug.LogWarningFormat("Method {0}.{1} cannot be used for menu commands because class {0} is an open generic type.", args);
                    }
                    else
                    {
                        foreach (UnityEditor.MenuItem item in customAttributes)
                        {
                            MonoMenuItem item2 = !dictionary.ContainsKey(item.menuItem) ? new MonoMenuItem() : dictionary[item.menuItem];
                            if (!ValidateMethodForMenuCommand(mi, false))
                            {
                                break;
                            }
                            if (item.menuItem.StartsWith("internal:", StringComparison.Ordinal))
                            {
                                if (!@bool)
                                {
                                    continue;
                                }
                                item2.menuItem = item.menuItem.Substring(9);
                            }
                            else
                            {
                                item2.menuItem = item.menuItem;
                            }
                            if (item.validate)
                            {
                                item2.validateType = type;
                                item2.validateMethod = mi;
                                item2.validateName = mi.Name;
                            }
                            else
                            {
                                item2.index = i;
                                item2.priority = item.priority;
                                item2.executeType = type;
                                item2.executeMethod = mi;
                                item2.executeName = mi.Name;
                            }
                            dictionary[item.menuItem] = item2;
                        }
                    }
                }
            }
            MonoMenuItem[] array = dictionary.Values.ToArray<MonoMenuItem>();
            Array.Sort(array, new CompareMenuIndex());
            return array;
        }

        internal static bool GameObjectContainsAttribute(GameObject go, System.Type attributeType)
        {
            foreach (Component component in go.GetComponents(typeof(Component)))
            {
                if ((component != null) && (component.GetType().GetCustomAttributes(attributeType, true).Length > 0))
                {
                    return true;
                }
            }
            return false;
        }

        [RequiredByNativeCode]
        private static string GetComponentMenuName(System.Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(AddComponentMenu), false);
            if (customAttributes.Length > 0)
            {
                AddComponentMenu menu = (AddComponentMenu) customAttributes[0];
                return menu.componentMenu;
            }
            return null;
        }

        [RequiredByNativeCode]
        private static int GetComponentMenuOrdering(System.Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(AddComponentMenu), false);
            if (customAttributes.Length > 0)
            {
                AddComponentMenu menu = (AddComponentMenu) customAttributes[0];
                return menu.componentOrder;
            }
            return 0;
        }

        [RequiredByNativeCode]
        internal static string GetHelpURLFromAttribute(System.Type objectType)
        {
            HelpURLAttribute customAttribute = (HelpURLAttribute) Attribute.GetCustomAttribute(objectType, typeof(HelpURLAttribute));
            return customAttribute?.URL;
        }

        internal static object InvokeMemberIfAvailable(object target, string methodName, object[] args)
        {
            MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (method != null)
            {
                return method.Invoke(target, args);
            }
            return null;
        }

        private static bool ValidateMethodForMenuCommand(MethodInfo mi, bool contextMenu)
        {
            if (contextMenu)
            {
                if (mi.IsStatic)
                {
                    object[] args = new object[] { mi.DeclaringType.FullName, mi.Name };
                    UnityEngine.Debug.LogWarningFormat("Method {0}.{1} is static and cannot be used for context menu commands.", args);
                    return false;
                }
            }
            else if (!mi.IsStatic)
            {
                object[] objArray2 = new object[] { mi.DeclaringType.FullName, mi.Name };
                UnityEngine.Debug.LogWarningFormat("Method {0}.{1} is not static and cannot be used for menu commands.", objArray2);
                return false;
            }
            if (mi.IsGenericMethod)
            {
                object[] objArray3 = new object[] { mi.DeclaringType.FullName, mi.Name };
                UnityEngine.Debug.LogWarningFormat("Method {0}.{1} is generic and cannot be used for menu commands.", objArray3);
                return false;
            }
            if ((mi.GetParameters().Length > 1) || ((mi.GetParameters().Length == 1) && (mi.GetParameters()[0].ParameterType != typeof(MenuCommand))))
            {
                object[] objArray4 = new object[] { mi.DeclaringType.FullName, mi.Name };
                UnityEngine.Debug.LogWarningFormat("Method {0}.{1} has invalid parameters. MenuCommand is the only optional supported parameter.", objArray4);
                return false;
            }
            return true;
        }

        [CompilerGenerated]
        private sealed class <CallMethodsWithAttribute>c__Iterator0<T> : IEnumerable, IEnumerable<T>, IEnumerator, IDisposable, IEnumerator<T>
        {
            internal T $current;
            internal bool $disposing;
            internal Assembly[] $locvar0;
            internal int $locvar1;
            internal System.Type[] $locvar2;
            internal int $locvar3;
            internal MethodInfo[] $locvar4;
            internal int $locvar5;
            internal int $PC;
            internal Assembly <assembly>__0;
            internal MethodInfo <method>__2;
            internal System.Type <type>__1;
            internal object[] arguments;
            internal System.Type attributeType;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$locvar0 = EditorAssemblies.loadedAssemblies;
                        this.$locvar1 = 0;
                        while (this.$locvar1 < this.$locvar0.Length)
                        {
                            this.<assembly>__0 = this.$locvar0[this.$locvar1];
                            this.$locvar2 = AssemblyHelper.GetTypesFromAssembly(this.<assembly>__0);
                            this.$locvar3 = 0;
                            while (this.$locvar3 < this.$locvar2.Length)
                            {
                                this.<type>__1 = this.$locvar2[this.$locvar3];
                                this.$locvar4 = this.<type>__1.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                                this.$locvar5 = 0;
                                while (this.$locvar5 < this.$locvar4.Length)
                                {
                                    this.<method>__2 = this.$locvar4[this.$locvar5];
                                    if (this.<method>__2.GetCustomAttributes(this.attributeType, false).Length > 0)
                                    {
                                        this.$current = (T) this.<method>__2.Invoke(null, this.arguments);
                                        if (!this.$disposing)
                                        {
                                            this.$PC = 1;
                                        }
                                        return true;
                                    }
                                Label_0101:
                                    this.$locvar5++;
                                }
                                this.$locvar3++;
                            }
                            this.$locvar1++;
                        }
                        this.$PC = -1;
                        break;

                    case 1:
                        goto Label_0101;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AttributeHelper.<CallMethodsWithAttribute>c__Iterator0<T> { 
                    attributeType = this.attributeType,
                    arguments = this.arguments
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();

            T IEnumerator<T>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        internal class CompareMenuIndex : IComparer
        {
            int IComparer.Compare(object xo, object yo)
            {
                AttributeHelper.MonoMenuItem item = (AttributeHelper.MonoMenuItem) xo;
                AttributeHelper.MonoMenuItem item2 = (AttributeHelper.MonoMenuItem) yo;
                if (item.priority != item2.priority)
                {
                    return item.priority.CompareTo(item2.priority);
                }
                return item.index.CompareTo(item2.index);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MonoCreateAssetItem
        {
            public string menuItem;
            public string fileName;
            public int order;
            public System.Type type;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MonoGizmoMethod
        {
            public MethodInfo drawGizmo;
            public System.Type drawnType;
            public int options;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MonoMenuItem
        {
            public string menuItem;
            public int index;
            public int priority;
            public System.Type executeType;
            public MethodInfo executeMethod;
            public string executeName;
            public System.Type validateType;
            public MethodInfo validateMethod;
            public string validateName;
        }
    }
}

