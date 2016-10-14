using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP
{
	internal class Labeler
	{
		private readonly MethodDefinition _methodDefinition;

		private readonly Dictionary<Instruction, List<Instruction>> _jumpMap = new Dictionary<Instruction, List<Instruction>>();

		public Labeler(MethodDefinition methodDefinition)
		{
			this._methodDefinition = methodDefinition;
			this.BuildLabelMap(methodDefinition);
		}

		public bool NeedsLabel(Instruction ins)
		{
			return this._jumpMap.ContainsKey(ins);
		}

		public string ForJump(Instruction targetInstruction)
		{
			return string.Format("goto {0};", this.FormatOffset(targetInstruction));
		}

		public string ForJump(int offset)
		{
			return string.Format("goto {0};", this.FormatOffset(offset));
		}

		public string ForLabel(Instruction ins)
		{
			return this.FormatOffset(ins) + ":";
		}

		private void BuildLabelMap(MethodDefinition methodDefinition)
		{
			foreach (Instruction current in methodDefinition.Body.Instructions)
			{
				Instruction instruction = current.Operand as Instruction;
				if (instruction != null)
				{
					this.AddJumpLabel(current, instruction);
				}
				else
				{
					Instruction[] array = current.Operand as Instruction[];
					if (array != null)
					{
						Instruction[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							Instruction targetInstruction = array2[i];
							this.AddJumpLabel(current, targetInstruction);
						}
					}
				}
			}
			foreach (ExceptionHandler current2 in methodDefinition.Body.ExceptionHandlers)
			{
				this.AddJumpLabel(null, current2.HandlerStart);
			}
		}

		private void AddJumpLabel(Instruction ins, Instruction targetInstruction)
		{
			List<Instruction> list;
			if (!this._jumpMap.TryGetValue(targetInstruction, out list))
			{
				this._jumpMap.Add(targetInstruction, list = new List<Instruction>());
			}
			list.Add(ins);
		}

		public string FormatOffset(Instruction ins)
		{
			return this.FormatOffset(ins.Offset);
		}

		private string FormatOffset(int offset)
		{
			string arg = "IL";
			foreach (ExceptionHandler current in this._methodDefinition.Body.ExceptionHandlers)
			{
				if (current.HandlerStart.Offset == offset)
				{
					switch (current.HandlerType)
					{
					case ExceptionHandlerType.Catch:
						arg = "CATCH";
						break;
					case ExceptionHandlerType.Filter:
						arg = "FILTER";
						break;
					case ExceptionHandlerType.Finally:
						arg = "FINALLY";
						break;
					case ExceptionHandlerType.Fault:
						arg = "FAULT";
						break;
					}
				}
			}
			return string.Format("{0}_{1:x4}", arg, offset);
		}
	}
}
