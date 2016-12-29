namespace Unity.PackageManager.Ivy
{
    using System;
    using System.Collections;
    using System.Reflection;
    using Unity.DataContract;

    public static class Cloner
    {
        private static BindingFlags bindingFlags = (BindingFlags.Public | BindingFlags.Instance);

        private static void CloneFields(object origin, Type originType, object target, Type targetType)
        {
            foreach (FieldInfo info in originType.GetFields(bindingFlags))
            {
                object originValue = info.GetValue(origin);
                Type fieldType = info.FieldType;
                FieldInfo field = targetType.GetField(info.Name, bindingFlags);
                if (field != null)
                {
                    Type targetValueType = field.FieldType;
                    CloneThing(field, target, originValue, fieldType, targetValueType);
                }
                else
                {
                    PropertyInfo property = targetType.GetProperty(info.Name, bindingFlags);
                    if (property != null)
                    {
                        Type propertyType = property.PropertyType;
                        CloneThing(property, target, originValue, fieldType, propertyType);
                    }
                }
            }
        }

        public static ToType CloneObject<ToType>(object origin) where ToType: new()
        {
            Type originType = origin.GetType();
            ToType target = Activator.CreateInstance<ToType>();
            Type type = target.GetType();
            CloneProperties(origin, originType, target, type);
            CloneFields(origin, originType, target, type);
            return target;
        }

        public static object CloneObject(object origin, Type targetType)
        {
            Type originType = origin.GetType();
            object target = null;
            ConstructorInfo constructor = null;
            try
            {
                target = Activator.CreateInstance(targetType);
            }
            catch
            {
            }
            if ((target == null) && (originType != typeof(string)))
            {
                Type[] types = new Type[] { originType };
                constructor = targetType.GetConstructor(types);
                if (constructor != null)
                {
                    object[] parameters = new object[] { origin };
                    target = constructor.Invoke(parameters);
                }
                if (target == null)
                {
                    Type[] typeArray2 = new Type[] { typeof(string) };
                    constructor = targetType.GetConstructor(typeArray2);
                }
                if (constructor != null)
                {
                    object[] objArray2 = new object[] { origin.ToString() };
                    target = constructor.Invoke(objArray2);
                }
            }
            if (target == null)
            {
                return null;
            }
            if (constructor == null)
            {
                CloneProperties(origin, originType, target, targetType);
                CloneFields(origin, originType, target, targetType);
            }
            return target;
        }

        private static void CloneProperties(object origin, Type originType, object target, Type targetType)
        {
            foreach (PropertyInfo info in originType.GetProperties(bindingFlags))
            {
                object originValue = info.GetValue(origin);
                Type propertyType = info.PropertyType;
                PropertyInfo property = targetType.GetProperty(info.Name, bindingFlags);
                if (property != null)
                {
                    Type targetValueType = property.PropertyType;
                    CloneThing(property, target, originValue, propertyType, targetValueType);
                }
                else
                {
                    FieldInfo field = targetType.GetField(info.Name, bindingFlags);
                    if (field != null)
                    {
                        Type fieldType = field.FieldType;
                        CloneThing(field, target, originValue, propertyType, fieldType);
                    }
                }
            }
        }

        private static void CloneThing(MemberInfo thing, object target, object originValue, Type originValueType, Type targetValueType)
        {
            if ((!thing.CanWrite() || ((originValue != null) && !targetValueType.IsAssignableFrom(originValueType))) && typeof(IList).IsAssignableFrom(originValueType))
            {
                IList list = thing.GetValue(target) as IList;
                if ((list == null) && thing.CanWrite())
                {
                    list = Activator.CreateInstance(targetValueType) as IList;
                    thing.SetValue(target, list);
                }
                if (list != null)
                {
                    IEnumerator enumerator = (originValue as IList).GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            object current = enumerator.Current;
                            if (current != null)
                            {
                                object obj3 = CloneObject(current, GetElementType(list.GetType()));
                                list.Add(obj3);
                            }
                        }
                    }
                    finally
                    {
                        IDisposable disposable = enumerator as IDisposable;
                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }
                    return;
                }
            }
            if (thing.CanWrite())
            {
                if ((originValue != null) && !targetValueType.IsAssignableFrom(originValueType))
                {
                    if (targetValueType.IsAssignableFrom(typeof(Uri)) && originValueType.IsAssignableFrom(typeof(string)))
                    {
                        Uri result = null;
                        if (Uri.TryCreate((string) originValue, UriKind.RelativeOrAbsolute, out result))
                        {
                            thing.SetValue(target, result);
                        }
                    }
                    else if (targetValueType.IsAssignableFrom(typeof(string)) && originValueType.IsAssignableFrom(typeof(Uri)))
                    {
                        string str = string.Empty;
                        if (originValue != null)
                        {
                            str = originValue.ToString();
                        }
                        thing.SetValue(target, str);
                    }
                    else if (targetValueType.IsAssignableFrom(typeof(PackageVersion)) && originValueType.IsAssignableFrom(typeof(string)))
                    {
                        try
                        {
                            PackageVersion version = new PackageVersion((string) originValue);
                            thing.SetValue(target, version);
                        }
                        catch
                        {
                        }
                    }
                    else if (targetValueType.IsAssignableFrom(typeof(string)) && originValueType.IsAssignableFrom(typeof(PackageVersion)))
                    {
                        string str2 = string.Empty;
                        if (originValue != null)
                        {
                            str2 = originValue.ToString();
                        }
                        thing.SetValue(target, str2);
                    }
                    else if (targetValueType.IsClass)
                    {
                        object obj4 = CloneObject(originValue, targetValueType);
                        if (obj4 == null)
                        {
                            obj4 = thing.GetValue(target);
                        }
                        if (obj4 != null)
                        {
                            thing.SetValue(target, obj4);
                        }
                        else
                        {
                            thing.SetValue(target, originValue);
                        }
                    }
                    else if (targetValueType.IsEnum)
                    {
                        thing.SetValue(target, (int) originValue);
                    }
                }
                else if (originValue == null)
                {
                    thing.SetValue(target, originValue);
                }
                else if (targetValueType.IsClass && (targetValueType != typeof(string)))
                {
                    object obj5 = CloneObject(originValue, targetValueType);
                    if (obj5 == null)
                    {
                        obj5 = thing.GetValue(target);
                    }
                    if (obj5 != null)
                    {
                        thing.SetValue(target, obj5);
                    }
                    else
                    {
                        thing.SetValue(target, originValue);
                    }
                }
                else
                {
                    thing.SetValue(target, originValue);
                }
            }
        }

        private static Type GetElementType(Type type)
        {
            Type elementType = type.GetElementType();
            if (elementType == null)
            {
                if (type.IsGenericType)
                {
                    Type[] genericArguments = type.GetGenericArguments();
                    if (genericArguments.Length > 0)
                    {
                        elementType = genericArguments[0];
                    }
                    return elementType;
                }
                if (type.BaseType != null)
                {
                    elementType = GetElementType(type.BaseType);
                }
            }
            return elementType;
        }
    }
}

