using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Globalization;
using System.IO;

namespace Unity.IL2CPP.Common.CFG
{
	public static class Formatter
	{
		public static string FormatInstruction(Instruction instruction)
		{
			StringWriter stringWriter = new StringWriter();
			Formatter.WriteInstruction(stringWriter, instruction);
			return stringWriter.ToString();
		}

		public static string FormatMethodBody(MethodDefinition method)
		{
			StringWriter stringWriter = new StringWriter();
			Formatter.WriteMethodBody(stringWriter, method);
			return stringWriter.ToString();
		}

		public static void WriteMethodBody(TextWriter writer, MethodDefinition method)
		{
			writer.WriteLine(method);
			foreach (Instruction current in method.Body.Instructions)
			{
				writer.Write('\t');
				Formatter.WriteInstruction(writer, current);
				writer.WriteLine();
			}
		}

		public static void WriteInstruction(TextWriter writer, Instruction instruction)
		{
			writer.Write(Formatter.FormatLabel(instruction.Offset));
			writer.Write(": ");
			writer.Write(instruction.OpCode.Name);
			if (instruction.Operand != null)
			{
				writer.Write(' ');
				Formatter.WriteOperand(writer, instruction.Operand);
			}
		}

		private static string FormatLabel(int offset)
		{
			string text = "000" + offset.ToString("x");
			return "IL_" + text.Substring(text.Length - 4);
		}

		private static void WriteOperand(TextWriter writer, object operand)
		{
			if (operand == null)
			{
				throw new ArgumentNullException("operand");
			}
			Instruction instruction = operand as Instruction;
			if (instruction != null)
			{
				writer.Write(Formatter.FormatLabel(instruction.Offset));
			}
			else
			{
				Instruction[] array = operand as Instruction[];
				if (array != null)
				{
					Formatter.WriteLabelList(writer, array);
				}
				else
				{
					VariableReference variableReference = operand as VariableReference;
					if (variableReference != null)
					{
						writer.Write(variableReference.Index.ToString());
					}
					else
					{
						MethodReference methodReference = operand as MethodReference;
						if (methodReference != null)
						{
							Formatter.WriteMethodReference(writer, methodReference);
						}
						else
						{
							string text = operand as string;
							if (text != null)
							{
								writer.Write("\"" + text + "\"");
							}
							else
							{
								text = Formatter.ToInvariantCultureString(operand);
								writer.Write(text);
							}
						}
					}
				}
			}
		}

		private static void WriteLabelList(TextWriter writer, Instruction[] instructions)
		{
			writer.Write("(");
			for (int i = 0; i < instructions.Length; i++)
			{
				if (i != 0)
				{
					writer.Write(", ");
				}
				writer.Write(Formatter.FormatLabel(instructions[i].Offset));
			}
			writer.Write(")");
		}

		public static string ToInvariantCultureString(object value)
		{
			IConvertible convertible = value as IConvertible;
			return (convertible == null) ? value.ToString() : convertible.ToString(CultureInfo.InvariantCulture);
		}

		private static void WriteMethodReference(TextWriter writer, MethodReference method)
		{
			writer.Write(Formatter.FormatTypeReference(method.ReturnType));
			writer.Write(' ');
			writer.Write(Formatter.FormatTypeReference(method.DeclaringType));
			writer.Write("::");
			writer.Write(method.Name);
			writer.Write("(");
			Collection<ParameterDefinition> parameters = method.Parameters;
			for (int i = 0; i < parameters.Count; i++)
			{
				if (i > 0)
				{
					writer.Write(", ");
				}
				writer.Write(Formatter.FormatTypeReference(parameters[i].ParameterType));
			}
			writer.Write(")");
		}

		public static string FormatTypeReference(TypeReference type)
		{
			string fullName = type.FullName;
			string result;
			switch (fullName)
			{
			case "System.Void":
				result = "void";
				return result;
			case "System.String":
				result = "string";
				return result;
			case "System.Int32":
				result = "int32";
				return result;
			case "System.Long":
				result = "int64";
				return result;
			case "System.Boolean":
				result = "bool";
				return result;
			case "System.Single":
				result = "float32";
				return result;
			case "System.Double":
				result = "float64";
				return result;
			}
			result = fullName;
			return result;
		}
	}
}
