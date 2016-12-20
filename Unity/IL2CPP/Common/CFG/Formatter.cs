namespace Unity.IL2CPP.Common.CFG
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;

    public static class Formatter
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map0;

        public static string FormatInstruction(Instruction instruction)
        {
            StringWriter writer = new StringWriter();
            WriteInstruction(writer, instruction);
            return writer.ToString();
        }

        private static string FormatLabel(int offset)
        {
            string str = "000" + offset.ToString("x");
            return ("IL_" + str.Substring(str.Length - 4));
        }

        public static string FormatMethodBody(MethodDefinition method)
        {
            StringWriter writer = new StringWriter();
            WriteMethodBody(writer, method);
            return writer.ToString();
        }

        public static string FormatTypeReference(TypeReference type)
        {
            string fullName = type.FullName;
            if (fullName != null)
            {
                int num;
                if (<>f__switch$map0 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(7) {
                        { 
                            "System.Void",
                            0
                        },
                        { 
                            "System.String",
                            1
                        },
                        { 
                            "System.Int32",
                            2
                        },
                        { 
                            "System.Long",
                            3
                        },
                        { 
                            "System.Boolean",
                            4
                        },
                        { 
                            "System.Single",
                            5
                        },
                        { 
                            "System.Double",
                            6
                        }
                    };
                    <>f__switch$map0 = dictionary;
                }
                if (<>f__switch$map0.TryGetValue(fullName, out num))
                {
                    switch (num)
                    {
                        case 0:
                            return "void";

                        case 1:
                            return "string";

                        case 2:
                            return "int32";

                        case 3:
                            return "int64";

                        case 4:
                            return "bool";

                        case 5:
                            return "float32";

                        case 6:
                            return "float64";
                    }
                }
            }
            return fullName;
        }

        public static string ToInvariantCultureString(object value)
        {
            IConvertible convertible = value as IConvertible;
            return ((convertible == null) ? value.ToString() : convertible.ToString(CultureInfo.InvariantCulture));
        }

        public static void WriteInstruction(TextWriter writer, Instruction instruction)
        {
            writer.Write(FormatLabel(instruction.Offset));
            writer.Write(": ");
            writer.Write(instruction.OpCode.Name);
            if (instruction.Operand != null)
            {
                writer.Write(' ');
                WriteOperand(writer, instruction.Operand);
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
                writer.Write(FormatLabel(instructions[i].Offset));
            }
            writer.Write(")");
        }

        public static void WriteMethodBody(TextWriter writer, MethodDefinition method)
        {
            writer.WriteLine(method);
            foreach (Instruction instruction in method.Body.Instructions)
            {
                writer.Write('\t');
                WriteInstruction(writer, instruction);
                writer.WriteLine();
            }
        }

        private static void WriteMethodReference(TextWriter writer, MethodReference method)
        {
            writer.Write(FormatTypeReference(method.ReturnType));
            writer.Write(' ');
            writer.Write(FormatTypeReference(method.DeclaringType));
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
                writer.Write(FormatTypeReference(parameters[i].ParameterType));
            }
            writer.Write(")");
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
                writer.Write(FormatLabel(instruction.Offset));
            }
            else
            {
                Instruction[] instructions = operand as Instruction[];
                if (instructions != null)
                {
                    WriteLabelList(writer, instructions);
                }
                else
                {
                    VariableReference reference = operand as VariableReference;
                    if (reference != null)
                    {
                        writer.Write(reference.Index.ToString());
                    }
                    else
                    {
                        MethodReference method = operand as MethodReference;
                        if (method != null)
                        {
                            WriteMethodReference(writer, method);
                        }
                        else
                        {
                            string str = operand as string;
                            if (str != null)
                            {
                                writer.Write("\"" + str + "\"");
                            }
                            else
                            {
                                str = ToInvariantCultureString(operand);
                                writer.Write(str);
                            }
                        }
                    }
                }
            }
        }
    }
}

