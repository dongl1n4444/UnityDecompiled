namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Scripting;

    internal class AttributeHelper
    {
        [DebuggerHidden]
        internal static IEnumerable<T> CallMethodsWithAttribute<T>(Type attributeType, params object[] arguments) => 
            new <CallMethodsWithAttribute>c__Iterator0<T> { 
                attributeType = attributeType,
                arguments = arguments,
                $PC = -2
            };

        [RequiredByNativeCode]
        private static MonoCreateAssetItem[] ExtractCreateAssetMenuItems(Assembly assembly)
        {
            List<MonoCreateAssetItem> list = new List<MonoCreateAssetItem>();
            Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
            foreach (Type type in typesFromAssembly)
            {
                CreateAssetMenuAttribute customAttribute = (CreateAssetMenuAttribute) Attribute.GetCustomAttribute(type, typeof(CreateAssetMenuAttribute));
                if (customAttribute != null)
                {
                    if (!type.IsSubclassOf(typeof(ScriptableObject)))
                    {
                        object[] args = new object[] { type.FullName };
                        Debug.LogWarningFormat("CreateAssetMenu attribute on {0} will be ignored as {0} is not derived from ScriptableObject.", args);
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
            Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
            foreach (Type type in typesFromAssembly)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                for (int i = 0; i < methods.GetLength(0); i++)
                {
                    MethodInfo info = methods[i];
                    object[] customAttributes = info.GetCustomAttributes(typeof(DrawGizmo), false);
                    foreach (DrawGizmo gizmo in customAttributes)
                    {
                        ParameterInfo[] parameters = info.GetParameters();
                        if (parameters.Length != 2)
                        {
                            Debug.LogWarning($"Method {info.DeclaringType.FullName}.{info.Name} is marked with the DrawGizmo attribute but does not take parameters (ComponentType, GizmoType) so will be ignored.");
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
                                Debug.LogWarning($"Method {info.DeclaringType.FullName}.{info.Name} is marked with the DrawGizmo attribute but the component type it applies to could not be determined.");
                                goto Label_0198;
                            }
                            if ((parameters[1].ParameterType != typeof(GizmoType)) && (parameters[1].ParameterType != typeof(int)))
                            {
                                Debug.LogWarning($"Method {info.DeclaringType.FullName}.{info.Name} is marked with the DrawGizmo attribute but does not take a second parameter of type GizmoType so will be ignored.");
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

        internal static bool GameObjectContainsAttribute(GameObject go, Type attributeType)
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
        private static string GetComponentMenuName(Type type)
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
        private static int GetComponentMenuOrdering(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(AddComponentMenu), false);
            if (customAttributes.Length > 0)
            {
                AddComponentMenu menu = (AddComponentMenu) customAttributes[0];
                return menu.componentOrder;
            }
            return 0;
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

        [CompilerGenerated]
        private sealed class <CallMethodsWithAttribute>c__Iterator0<T> : IEnumerable, IEnumerable<T>, IEnumerator, IDisposable, IEnumerator<T>
        {
            internal T $current;
            internal bool $disposing;
            internal Assembly[] $locvar0;
            internal int $locvar1;
            internal Type[] $locvar2;
            internal int $locvar3;
            internal MethodInfo[] $locvar4;
            internal int $locvar5;
            internal int $PC;
            internal Assembly <assembly>__1;
            internal MethodInfo <method>__3;
            internal Type <type>__2;
            internal object[] arguments;
            internal Type attributeType;

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
                            this.<assembly>__1 = this.$locvar0[this.$locvar1];
                            this.$locvar2 = this.<assembly>__1.GetTypes();
                            this.$locvar3 = 0;
                            while (this.$locvar3 < this.$locvar2.Length)
                            {
                                this.<type>__2 = this.$locvar2[this.$locvar3];
                                this.$locvar4 = this.<type>__2.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                                this.$locvar5 = 0;
                                while (this.$locvar5 < this.$locvar4.Length)
                                {
                                    this.<method>__3 = this.$locvar4[this.$locvar5];
                                    if (this.<method>__3.GetCustomAttributes(this.attributeType, false).Length > 0)
                                    {
                                        this.$current = (T) this.<method>__3.Invoke(null, this.arguments);
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

        [StructLayout(LayoutKind.Sequential)]
        private struct MonoCreateAssetItem
        {
            public string menuItem;
            public string fileName;
            public int order;
            public Type type;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MonoGizmoMethod
        {
            public MethodInfo drawGizmo;
            public Type drawnType;
            public int options;
        }
    }
}

