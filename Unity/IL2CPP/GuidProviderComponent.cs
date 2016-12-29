namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    internal class GuidProviderComponent : IGuidProvider
    {
        [CompilerGenerated]
        private static Func<CustomAttribute, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<CustomAttribute, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<TypeReference, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<FieldDefinition, TypeReference> <>f__am$cache4;
        private static readonly byte[] kParameterizedNamespaceGuid = new byte[] { 0x11, 0xf4, 0x7a, 0xd5, 0x7b, 0x73, 0x42, 0xc0, 0xab, 0xae, 0x87, 0x8b, 30, 0x16, 0xad, 0xee };
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public Guid GuidFor(TypeReference type)
        {
            GenericInstanceType type2 = type as GenericInstanceType;
            if (type2 != null)
            {
                return ParameterizedGuidFromTypeIdentifier(IdentifierFor(type2));
            }
            if ((type is TypeSpecification) || (type is GenericParameter))
            {
                throw new InvalidOperationException($"Cannot retrieve GUID for {type.FullName}");
            }
            TypeDefinition definition = type.Resolve();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<CustomAttribute, bool>(null, (IntPtr) <GuidFor>m__0);
            }
            CustomAttribute attribute = definition.CustomAttributes.SingleOrDefault<CustomAttribute>(<>f__am$cache0);
            if (attribute != null)
            {
                CustomAttributeArgument argument = attribute.ConstructorArguments[0];
                return new Guid((string) argument.Value);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<CustomAttribute, bool>(null, (IntPtr) <GuidFor>m__1);
            }
            attribute = definition.CustomAttributes.SingleOrDefault<CustomAttribute>(<>f__am$cache1);
            if (attribute == null)
            {
                throw new InvalidOperationException($"'{type.FullName}' doesn't have a GUID.");
            }
            CustomAttributeArgument argument2 = attribute.ConstructorArguments[0];
            CustomAttributeArgument argument3 = attribute.ConstructorArguments[1];
            CustomAttributeArgument argument4 = attribute.ConstructorArguments[2];
            CustomAttributeArgument argument5 = attribute.ConstructorArguments[3];
            CustomAttributeArgument argument6 = attribute.ConstructorArguments[4];
            CustomAttributeArgument argument7 = attribute.ConstructorArguments[5];
            CustomAttributeArgument argument8 = attribute.ConstructorArguments[6];
            CustomAttributeArgument argument9 = attribute.ConstructorArguments[7];
            CustomAttributeArgument argument10 = attribute.ConstructorArguments[8];
            CustomAttributeArgument argument11 = attribute.ConstructorArguments[9];
            CustomAttributeArgument argument12 = attribute.ConstructorArguments[10];
            return new Guid((uint) argument2.Value, (ushort) argument3.Value, (ushort) argument4.Value, (byte) argument5.Value, (byte) argument6.Value, (byte) argument7.Value, (byte) argument8.Value, (byte) argument9.Value, (byte) argument10.Value, (byte) argument11.Value, (byte) argument12.Value);
        }

        private static string IdentifierFor(TypeReference type)
        {
            switch (type.MetadataType)
            {
                case MetadataType.Boolean:
                    return "b1";

                case MetadataType.Char:
                    return "c2";

                case MetadataType.Byte:
                    return "u1";

                case MetadataType.Int16:
                    return "i2";

                case MetadataType.UInt16:
                    return "u2";

                case MetadataType.Int32:
                    return "i4";

                case MetadataType.UInt32:
                    return "u4";

                case MetadataType.Int64:
                    return "i8";

                case MetadataType.UInt64:
                    return "u8";

                case MetadataType.Single:
                    return "f4";

                case MetadataType.Double:
                    return "f8";

                case MetadataType.String:
                    return "string";

                case MetadataType.ValueType:
                    if (type.FullName != "System.Guid")
                    {
                        break;
                    }
                    return "g16";

                case MetadataType.Object:
                    return "cinterface(IInspectable)";
            }
            TypeDefinition definition = WindowsRuntimeProjections.ProjectToWindowsRuntime(type.Resolve()).Resolve();
            if (((type.MetadataType != MetadataType.Class) && (type.MetadataType != MetadataType.ValueType)) && (type.MetadataType != MetadataType.GenericInstance))
            {
                throw new InvalidOperationException($"Cannot compute type identifier for {type.FullName}, as its metadata type is not supported: {type.MetadataType}.");
            }
            if (!definition.IsWindowsRuntime)
            {
                throw new InvalidOperationException($"Cannot compute type identifier for {type.FullName}, as it is not a Windows Runtime type.");
            }
            GenericInstanceType type3 = type as GenericInstanceType;
            if (type3 != null)
            {
                return $"pinterface({{{definition.GetGuid().ToString()}}};{IdentifierFor(type3.GenericArguments)})";
            }
            if (definition.MetadataType == MetadataType.ValueType)
            {
                if (definition.IsEnum())
                {
                    return $"enum({definition.FullName};{IdentifierFor(definition.GetUnderlyingEnumType())})";
                }
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = new Func<FieldDefinition, bool>(null, (IntPtr) <IdentifierFor>m__3);
                }
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = new Func<FieldDefinition, TypeReference>(null, (IntPtr) <IdentifierFor>m__4);
                }
                IEnumerable<TypeReference> nameElements = definition.Fields.Where<FieldDefinition>(<>f__am$cache3).Select<FieldDefinition, TypeReference>(<>f__am$cache4);
                return $"struct({definition.FullName};{IdentifierFor(nameElements)})";
            }
            if (definition.IsInterface)
            {
                return $"{{{definition.GetGuid().ToString()}}}";
            }
            if (definition.IsDelegate())
            {
                return $"delegate({{{definition.GetGuid().ToString()}}})";
            }
            TypeReference reference = definition.ExtractDefaultInterface();
            GenericInstanceType type4 = reference as GenericInstanceType;
            if (type4 != null)
            {
                return $"rc({definition.FullName};{IdentifierFor(type4)})";
            }
            return $"rc({definition.FullName};{{{reference.GetGuid().ToString()}}})";
        }

        public static string IdentifierFor(IEnumerable<TypeReference> nameElements)
        {
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<TypeReference, string>(null, (IntPtr) <IdentifierFor>m__2);
            }
            return nameElements.Select<TypeReference, string>(<>f__am$cache2).AggregateWith(";");
        }

        private static Guid ParameterizedGuidFromTypeIdentifier(string typeIdentifier)
        {
            byte[] buffer;
            List<byte> list = new List<byte>();
            list.AddRange(kParameterizedNamespaceGuid);
            list.AddRange(Encoding.UTF8.GetBytes(typeIdentifier));
            using (SHA1Managed managed = new SHA1Managed())
            {
                buffer = managed.ComputeHash(list.ToArray());
            }
            int a = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, 0));
            short b = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, 4));
            short c = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, 6));
            byte[] d = buffer.Skip<byte>(8).Take<byte>(8).ToArray<byte>();
            c = (short) (c & 0xfff);
            c = (short) (c | 0x5000);
            d[0] = (byte) (d[0] & 0x3f);
            d[0] = (byte) (d[0] | 0x80);
            return new Guid(a, b, c, d);
        }
    }
}

