namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Text;

    public static class ErrorMessageWriter
    {
        private static bool AppendSourceCodeLocation(ErrorInformation errorInformation, StringBuilder message)
        {
            string str = FindSourceCodeLocationForInstruction(errorInformation.Instruction);
            if (string.IsNullOrEmpty(str))
            {
                str = FindSourceCodeLocation(errorInformation.Method);
            }
            if (string.IsNullOrEmpty(str) && (errorInformation.Type != null))
            {
                foreach (MethodDefinition definition in errorInformation.Type.Methods)
                {
                    str = FindSourceCodeLocation(definition);
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

        private static string FindSourceCodeLocation(MethodDefinition method)
        {
            string str = string.Empty;
            if ((method != null) && method.HasBody)
            {
                foreach (Instruction instruction in method.Body.Instructions)
                {
                    str = FindSourceCodeLocationForInstruction(instruction);
                    if (!string.IsNullOrEmpty(str))
                    {
                        return str;
                    }
                }
            }
            return str;
        }

        private static string FindSourceCodeLocationForInstruction(Instruction instruction)
        {
            string str = string.Empty;
            if ((instruction != null) && (instruction.SequencePoint != null))
            {
                str = string.Format("{0}:{1}", instruction.SequencePoint.Document.Url, instruction.SequencePoint.StartLine);
            }
            return str;
        }

        public static string FormatMessage(ErrorInformation errorInformation, string additionalInformation)
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
            bool flag = AppendSourceCodeLocation(errorInformation, message);
            if ((!flag && (errorInformation.Type != null)) && (errorInformation.Type.Module != null))
            {
                message.AppendFormat(" in assembly '{0}'", errorInformation.Type.Module.FullyQualifiedName);
            }
            if (!flag)
            {
                additionalInformation = string.Format("Build a development build for more information.{0}{1}", !string.IsNullOrEmpty(additionalInformation) ? " " : "", additionalInformation);
            }
            if (!string.IsNullOrEmpty(additionalInformation))
            {
                message.AppendLine();
                message.AppendFormat("Additional information: {0}", additionalInformation);
            }
            return message.ToString();
        }
    }
}

