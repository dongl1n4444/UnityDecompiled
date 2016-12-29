namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;

    public class ErrorTypeAndMethod
    {
        public const string NameOfErrorMethod = "_ErrorMethodForTestingOnly_";
        public const string NameOfErrorType = "_ErrorTypeForTestingOnly_";

        private static string FormatExceptionMessage(string typeOrMethod, string name) => 
            $"The managed {typeOrMethod} {name} always causes this exception when (--enable-error-message-test) is passed to il2cpp.exe. This exception is used only to test the il2cpp error reporting code.";

        public static void ThrowIfIsErrorMethod(MethodReference method)
        {
            VerifyRequiredCodeGenOptions();
            if (method.Name == "_ErrorMethodForTestingOnly_")
            {
                throw new NotSupportedException(FormatExceptionMessage("method", "_ErrorTypeForTestingOnly_"));
            }
        }

        public static void ThrowIfIsErrorType(TypeDefinition typeDefinition)
        {
            VerifyRequiredCodeGenOptions();
            if (typeDefinition.Name == "_ErrorTypeForTestingOnly_")
            {
                throw new NotSupportedException(FormatExceptionMessage("type", "_ErrorTypeForTestingOnly_"));
            }
        }

        private static void VerifyRequiredCodeGenOptions()
        {
            if (!CodeGenOptions.EnableErrorMessageTest)
            {
                throw new InvalidOperationException("This method should only be called when the EnableErrorMessageTest command line option for il2cpp is set.");
            }
        }
    }
}

