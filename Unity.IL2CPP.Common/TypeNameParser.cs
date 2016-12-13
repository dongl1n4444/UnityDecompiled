using System;

namespace Unity.IL2CPP.Common
{
	public class TypeNameParser
	{
		private int _p;

		private readonly int _end;

		private readonly char[] _data;

		private readonly bool _isNested;

		private bool _acceptAssemblyName;

		private TypeNameParseInfo _info;

		private bool IsEOL
		{
			get
			{
				return this._p >= this._end;
			}
		}

		private TypeNameParser(char[] data, int begin, int end, TypeNameParseInfo info, bool isNested)
		{
			this._data = data;
			this._p = begin;
			this._end = end;
			this._info = info;
			this._isNested = isNested;
			this._acceptAssemblyName = true;
		}

		public static TypeNameParseInfo Parse(string name)
		{
			TypeNameParseInfo typeNameParseInfo = new TypeNameParseInfo();
			char[] array = name.ToCharArray();
			TypeNameParser typeNameParser = new TypeNameParser(array, 0, array.Length, typeNameParseInfo, false);
			return typeNameParser.Parse(true) ? typeNameParseInfo : null;
		}

		private bool CurrentIs(char v)
		{
			return !this.IsEOL && this._data[this._p] == v;
		}

		private bool Next(bool skipWhites = false)
		{
			this._p++;
			if (skipWhites)
			{
				this.SkipWhites();
			}
			return !this.IsEOL;
		}

		private bool NextWillBe(char v, bool skipWhites = false)
		{
			bool result;
			if (this.IsEOL)
			{
				result = false;
			}
			else
			{
				int num = 1;
				if (this._p + num >= this._end)
				{
					result = false;
				}
				else if (!skipWhites)
				{
					result = (this._data[this._p + num] == v);
				}
				else
				{
					while (this._data[this._p + num] == ' ' || this._data[this._p + num] == '\t')
					{
						num++;
						if (this._p + num >= this._end)
						{
							result = false;
							return result;
						}
					}
					result = (this._data[this._p + num] == v);
				}
			}
			return result;
		}

		private void InitializeParser()
		{
			this.SkipWhites();
		}

		private void SkipWhites()
		{
			while ((this.CurrentIs(' ') || this.CurrentIs('\t')) && !this.IsEOL)
			{
				this._p++;
			}
		}

		private void ConsumeIdentifier()
		{
			while (true)
			{
				char c = this._data[this._p];
				switch (c)
				{
				case '*':
				case '+':
				case ',':
				case '.':
					return;
				case '-':
					IL_2D:
					switch (c)
					{
					case '[':
					case ']':
					case '`':
						return;
					case '\\':
						this.Next(false);
						goto IL_7D;
					case '^':
					case '_':
						IL_4E:
						if (c != '"' && c != '&' && c != '=')
						{
							goto IL_7D;
						}
						return;
					}
					goto IL_4E;
					IL_7D:
					if (!this.Next(false))
					{
						return;
					}
					continue;
				}
				goto IL_2D;
			}
		}

		private void ConsumeAssemblyIdentifier()
		{
			do
			{
				char c = this._data[this._p];
				switch (c)
				{
				case '[':
				case ']':
				case '`':
					return;
				case '\\':
					this.Next(false);
					goto IL_75;
				case '^':
				case '_':
					IL_31:
					switch (c)
					{
					case '*':
					case '+':
					case ',':
						return;
					default:
						if (c != '"' && c != '&' && c != '=')
						{
							goto IL_75;
						}
						return;
					}
					break;
				}
				goto IL_31;
				IL_75:;
			}
			while (this.Next(false));
		}

		private void ConsumePropertyIdentifier()
		{
			do
			{
				char c = this._data[this._p];
				if (c == '=')
				{
					break;
				}
			}
			while (this.Next(false));
		}

		private void ConsumePropertyValue()
		{
			do
			{
				char c = this._data[this._p];
				if (c == ',' || c == ']')
				{
					break;
				}
			}
			while (this.Next(false));
		}

		private bool ConsumeNumber(ref int value)
		{
			bool result;
			if (!char.IsDigit(this._data[this._p]))
			{
				result = false;
			}
			else
			{
				int p = this._p;
				while (char.IsDigit(this._data[this._p]))
				{
					if (!this.Next(false))
					{
						break;
					}
				}
				string s = new string(this._data, p, this._p - p);
				value = int.Parse(s);
				result = true;
			}
			return result;
		}

		private bool ParseTypeName(ref int arity)
		{
			int p = this._p;
			int num = this._end;
			while (true)
			{
				this.ConsumeIdentifier();
				if (!this.CurrentIs('.'))
				{
					break;
				}
				num = this._p;
				if (!this.Next(false))
				{
					goto Block_2;
				}
			}
			bool result;
			if (this.CurrentIs('`'))
			{
				if (!this.Next(false))
				{
					result = false;
					return result;
				}
				if (!this.ConsumeNumber(ref arity))
				{
					result = false;
					return result;
				}
			}
			if (num == this._end)
			{
				this._info.Name = new string(this._data, p, this._p - p);
			}
			else
			{
				this._info.Namespace = new string(this._data, p, num - p);
				this._info.Name = new string(this._data, num + 1, this._p - num - 1);
			}
			this._info.Arities.Add(arity);
			result = true;
			return result;
			Block_2:
			result = false;
			return result;
		}

		private bool ParseNestedTypeOptional(ref int arity)
		{
			bool result;
			while (this.CurrentIs('+'))
			{
				if (this.Next(false))
				{
					int num = 0;
					int p = this._p;
					this.ConsumeIdentifier();
					if (this.CurrentIs('`'))
					{
						if (!this.Next(false))
						{
							result = false;
							return result;
						}
						if (!this.ConsumeNumber(ref num))
						{
							result = false;
							return result;
						}
						arity += num;
					}
					this._info.Nested.Add(new string(this._data, p, this._p - p));
					this._info.Arities.Add(num);
					continue;
				}
				result = false;
				return result;
			}
			result = true;
			return result;
		}

		private bool ParseTypeArgumentsOptional(ref int arity)
		{
			this.SkipWhites();
			bool result;
			if (this.IsEOL)
			{
				result = true;
			}
			else if (!this.CurrentIs('['))
			{
				result = true;
			}
			else if (this.NextWillBe(']', true) || this.NextWillBe(',', true) || this.NextWillBe('*', true))
			{
				result = true;
			}
			else if (!this.Next(true))
			{
				result = false;
			}
			else
			{
				this._info.TypeArguments.Capacity = arity;
				while (true)
				{
					bool flag = false;
					if (this.CurrentIs('['))
					{
						flag = true;
						if (!this.Next(true))
						{
							break;
						}
					}
					TypeNameParseInfo typeNameParseInfo = new TypeNameParseInfo();
					TypeNameParser typeNameParser = new TypeNameParser(this._data, this._p, this._end, typeNameParseInfo, true);
					if (!typeNameParser.Parse(flag))
					{
						goto Block_8;
					}
					this._p = typeNameParser._p;
					this._info.TypeArguments.Add(typeNameParseInfo);
					this.SkipWhites();
					if (this.IsEOL)
					{
						goto Block_9;
					}
					if (flag)
					{
						if (!this.CurrentIs(']'))
						{
							goto Block_11;
						}
						if (!this.Next(true))
						{
							goto Block_12;
						}
					}
					if (this.CurrentIs(']'))
					{
						goto Block_13;
					}
					if (!this.CurrentIs(','))
					{
						goto IL_17B;
					}
					if (!this.Next(true))
					{
						goto Block_15;
					}
				}
				result = false;
				return result;
				Block_8:
				result = false;
				return result;
				Block_9:
				result = false;
				return result;
				Block_11:
				result = false;
				return result;
				Block_12:
				result = false;
				return result;
				Block_13:
				if (this._info.TypeArguments.Count != arity)
				{
					result = false;
					return result;
				}
				this.Next(true);
				result = true;
				return result;
				Block_15:
				result = false;
				return result;
				IL_17B:
				result = false;
			}
			return result;
		}

		private bool ParseAssemblyNameOptional()
		{
			bool result;
			if (!this._acceptAssemblyName)
			{
				result = true;
			}
			else if (!this.CurrentIs(','))
			{
				result = true;
			}
			else if (!this.Next(false))
			{
				result = false;
			}
			else
			{
				this.SkipWhites();
				result = this.ParseAssemblyName();
			}
			return result;
		}

		private bool ParseAssemblyName()
		{
			int p = this._p;
			this.ConsumeAssemblyIdentifier();
			this._info.Assembly.Name = new string(this._data, p, this._p - p);
			this.SkipWhites();
			return this.ParsePropertiesOptional();
		}

		private bool ParsePropertiesOptional()
		{
			bool result;
			while (this.CurrentIs(','))
			{
				if (!this.Next(true))
				{
					result = false;
				}
				else
				{
					int p = this._p;
					this.ConsumePropertyIdentifier();
					string text = new string(this._data, p, this._p - p);
					if (!this.CurrentIs('='))
					{
						result = false;
					}
					else if (!this.Next(false))
					{
						result = false;
					}
					else
					{
						p = this._p;
						this.ConsumePropertyValue();
						string text2 = new string(this._data, p, this._p - p);
						if (text != null)
						{
							if (!(text == "Version"))
							{
								if (!(text == "PublicKey"))
								{
									if (!(text == "PublicKeyToken"))
									{
										if (!(text == "Culture"))
										{
											goto IL_17E;
										}
										this._info.Assembly.Culture = text2;
									}
									else if (text2 != "null")
									{
										if (text2.Length != 16)
										{
											result = false;
											return result;
										}
										char[] array = text2.ToCharArray();
										Array.Copy(array, this._info.Assembly.PublicKeyToken, Math.Min(17, array.Length));
									}
								}
								else if (text2 != "null")
								{
									this._info.Assembly.PublicKey = text2;
								}
							}
							else if (!TypeNameParser.ParseVersion(text2, ref this._info))
							{
								result = false;
								return result;
							}
							continue;
						}
						IL_17E:
						result = false;
					}
				}
				return result;
			}
			result = true;
			return result;
		}

		private bool ParseArrayModifierOptional()
		{
			this.SkipWhites();
			bool result;
			if (this.IsEOL)
			{
				result = true;
			}
			else if (!this.CurrentIs('['))
			{
				result = true;
			}
			else if (!this.NextWillBe(']', true) && !this.NextWillBe(',', true) && !this.NextWillBe('*', true))
			{
				result = true;
			}
			else if (!this.Next(true))
			{
				result = false;
			}
			else
			{
				int num = 1;
				while (!this.CurrentIs(']'))
				{
					if (this.CurrentIs(','))
					{
						num++;
						if (!this.Next(true))
						{
							result = false;
							return result;
						}
					}
					else
					{
						if (!this.CurrentIs('*'))
						{
							result = false;
							return result;
						}
						this._info.Modifiers.Add(-2);
						if (!this.Next(true))
						{
							result = false;
							return result;
						}
					}
				}
				this.Next(true);
				this._info.Modifiers.Add(num);
				result = this.ParseArrayModifierOptional();
			}
			return result;
		}

		private bool ParsePointerModifiersOptional()
		{
			bool result;
			if (this.IsEOL)
			{
				result = true;
			}
			else
			{
				while (this.CurrentIs('*'))
				{
					this._info.Modifiers.Add(-1);
					if (!this.Next(true))
					{
						break;
					}
				}
				result = true;
			}
			return result;
		}

		private bool ParseByRefModifiersOptional()
		{
			bool result;
			if (this.IsEOL)
			{
				result = true;
			}
			else if (!this.CurrentIs('&'))
			{
				result = true;
			}
			else if (this._info.Modifiers.Contains(0))
			{
				result = false;
			}
			else
			{
				this._info.Modifiers.Add(0);
				this.Next(true);
				result = true;
			}
			return result;
		}

		private static bool ParseVersion(string version, out ushort major, out ushort minor, out ushort build, out ushort revision)
		{
			major = 0;
			minor = 0;
			build = 0;
			revision = 0;
			int num = 0;
			int num2 = version.IndexOf('.');
			bool result;
			if (num2 == -1)
			{
				result = false;
			}
			else
			{
				major = ushort.Parse(version.Substring(num, num2 - num));
				num = num2 + 1;
				num2 = version.IndexOf('.', num);
				if (num2 == -1)
				{
					result = false;
				}
				else
				{
					minor = ushort.Parse(version.Substring(num, num2 - num));
					num = num2 + 1;
					num2 = version.IndexOf('.', num);
					if (num2 == -1)
					{
						result = false;
					}
					else
					{
						build = ushort.Parse(version.Substring(num, num2 - num));
						num = num2 + 1;
						revision = ushort.Parse(version.Substring(num, version.Length - num));
						result = true;
					}
				}
			}
			return result;
		}

		private static bool ParseVersion(string version, ref TypeNameParseInfo info)
		{
			ushort major;
			ushort minor;
			ushort build;
			ushort revision;
			bool result;
			if (!TypeNameParser.ParseVersion(version, out major, out minor, out build, out revision))
			{
				result = false;
			}
			else
			{
				info.Assembly.Major = major;
				info.Assembly.Minor = minor;
				info.Assembly.Build = build;
				info.Assembly.Revision = revision;
				result = true;
			}
			return result;
		}

		private bool Parse(bool acceptAssemblyName = true)
		{
			this._acceptAssemblyName = acceptAssemblyName;
			int num = 0;
			this.InitializeParser();
			return !this.IsEOL && this.ParseTypeName(ref num) && this.ParseNestedTypeOptional(ref num) && this.ParseTypeArgumentsOptional(ref num) && this.ParsePointerModifiersOptional() && this.ParseArrayModifierOptional() && this.ParseByRefModifiersOptional() && this.ParseAssemblyNameOptional() && (this._p == this._end || this._isNested);
		}
	}
}
