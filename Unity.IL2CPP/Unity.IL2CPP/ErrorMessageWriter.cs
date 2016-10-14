using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Text;

namespace Unity.IL2CPP
{
	public static class ErrorMessageWriter
	{
		public static string FormatMessage(ErrorInformation errorInformation, string additionalInformation)
		{
			if (errorInformation == null)
			{
				throw new ArgumentNullException("errorInformation");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("IL2CPP error");
			if (errorInformation.Method != null)
			{
				stringBuilder.AppendFormat(" for method '{0}'", errorInformation.Method.FullName);
			}
			else if (errorInformation.Type != null)
			{
				stringBuilder.AppendFormat(" for type '{0}'", errorInformation.Type);
			}
			else
			{
				stringBuilder.Append(" (no further information about what managed code was being converted is available)");
			}
			bool flag = ErrorMessageWriter.AppendSourceCodeLocation(errorInformation, stringBuilder);
			if (!flag && errorInformation.Type != null && errorInformation.Type.Module != null)
			{
				stringBuilder.AppendFormat(" in assembly '{0}'", errorInformation.Type.Module.FullyQualifiedName);
			}
			if (!flag)
			{
				additionalInformation = string.Format("Build a development build for more information.{0}{1}", (!string.IsNullOrEmpty(additionalInformation)) ? " " : "", additionalInformation);
			}
			if (!string.IsNullOrEmpty(additionalInformation))
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("Additional information: {0}", additionalInformation);
			}
			return stringBuilder.ToString();
		}

		private static bool AppendSourceCodeLocation(ErrorInformation errorInformation, StringBuilder message)
		{
			string text = ErrorMessageWriter.FindSourceCodeLocationForInstruction(errorInformation.Instruction);
			if (string.IsNullOrEmpty(text))
			{
				text = ErrorMessageWriter.FindSourceCodeLocation(errorInformation.Method);
			}
			if (string.IsNullOrEmpty(text) && errorInformation.Type != null)
			{
				foreach (MethodDefinition current in errorInformation.Type.Methods)
				{
					text = ErrorMessageWriter.FindSourceCodeLocation(current);
					if (!string.IsNullOrEmpty(text))
					{
						break;
					}
				}
			}
			bool result;
			if (!string.IsNullOrEmpty(text))
			{
				message.AppendFormat(" in {0}", text);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private static string FindSourceCodeLocation(MethodDefinition method)
		{
			string text = string.Empty;
			if (method != null && method.HasBody)
			{
				foreach (Instruction current in method.Body.Instructions)
				{
					text = ErrorMessageWriter.FindSourceCodeLocationForInstruction(current);
					if (!string.IsNullOrEmpty(text))
					{
						break;
					}
				}
			}
			return text;
		}

		private static string FindSourceCodeLocationForInstruction(Instruction instruction)
		{
			string result = string.Empty;
			if (instruction != null && instruction.SequencePoint != null)
			{
				result = string.Format("{0}:{1}", instruction.SequencePoint.Document.Url, instruction.SequencePoint.StartLine);
			}
			return result;
		}
	}
}
