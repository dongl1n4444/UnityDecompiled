namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    public static class ErrorMessageWriter
    {
        [CompilerGenerated]
        private static Func<Instruction, MethodDefinition, SequencePoint> <>f__mg$cache0;

        private static bool AppendSourceCodeLocation(ErrorInformation errorInformation, StringBuilder message, Func<Instruction, MethodDefinition, SequencePoint> getSequencePoint)
        {
            string str = FindSourceCodeLocationForInstruction(errorInformation.Instruction, errorInformation.Method, getSequencePoint);
            if (string.IsNullOrEmpty(str))
            {
                str = FindSourceCodeLocation(errorInformation.Method, getSequencePoint);
            }
            if (string.IsNullOrEmpty(str) && (errorInformation.Type != null))
            {
                foreach (MethodDefinition definition in errorInformation.Type.Methods)
                {
                    str = FindSourceCodeLocation(definition, getSequencePoint);
                    if (!string.IsNullOrEmpty(str))
                    {
                        break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(str))
            {
                message.AppendFormat(" in {0}", str);
                return true;
            }
            return false;
        }

        private static string FindSourceCodeLocation(MethodDefinition method, Func<Instruction, MethodDefinition, SequencePoint> getSequencePoint)
        {
            string str = string.Empty;
            if ((method != null) && method.HasBody)
            {
                foreach (Instruction instruction in method.Body.Instructions)
                {
                    str = FindSourceCodeLocationForInstruction(instruction, method, getSequencePoint);
                    if (!string.IsNullOrEmpty(str))
                    {
                        return str;
                    }
                }
            }
            return str;
        }

        private static string FindSourceCodeLocationForInstruction(Instruction instruction, MethodDefinition method, Func<Instruction, MethodDefinition, SequencePoint> getSequencePoint)
        {
            if (instruction == null)
            {
                return string.Empty;
            }
            SequencePoint point = getSequencePoint(instruction, method);
            if (point == null)
            {
                return string.Empty;
            }
            return $"{point.Document.Url}:{point.StartLine}";
        }

        public static string FormatMessage(ErrorInformation errorInformation, string additionalInformation)
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<Instruction, MethodDefinition, SequencePoint>(ErrorMessageWriter.GetSequencePoint);
            }
            return FormatMessage(errorInformation, additionalInformation, <>f__mg$cache0);
        }

        public static string FormatMessage(ErrorInformation errorInformation, string additionalInformation, Func<Instruction, MethodDefinition, SequencePoint> getSequencePoint)
        {
            if (errorInformation == null)
            {
                throw new ArgumentNullException("errorInformation");
            }
            StringBuilder message = new StringBuilder();
            message.Append("IL2CPP error");
            if (errorInformation.Method != null)
            {
                message.AppendFormat(" for method '{0}'", errorInformation.Method.FullName);
            }
            else if (errorInformation.Type != null)
            {
                message.AppendFormat(" for type '{0}'", errorInformation.Type);
            }
            else
            {
                message.Append(" (no further information about what managed code was being converted is available)");
            }
            bool flag = AppendSourceCodeLocation(errorInformation, message, getSequencePoint);
            if ((!flag && (errorInformation.Type != null)) && (errorInformation.Type.Module != null))
            {
                if (errorInformation.Type.Module.FileName == null)
                {
                }
                message.AppendFormat(" in assembly '{0}'", errorInformation.Type.Module.Name);
            }
            if (!flag)
            {
                additionalInformation = $"Build a development build for more information.{!string.IsNullOrEmpty(additionalInformation) ? " " : ""}{additionalInformation}";
            }
            if (!string.IsNullOrEmpty(additionalInformation))
            {
                message.AppendLine();
                message.AppendFormat("Additional information: {0}", additionalInformation);
            }
            return message.ToString();
        }

        private static SequencePoint GetSequencePoint(Instruction ins, MethodDefinition method) => 
            ins.GetSequencePoint(method);
    }
}

