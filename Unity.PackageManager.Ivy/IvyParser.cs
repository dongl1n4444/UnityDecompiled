using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Unity.PackageManager.IvyInternal;

namespace Unity.PackageManager.Ivy
{
	public static class IvyParser
	{
		public static XmlSerializerNamespaces Namespaces;

		public static bool HasErrors;

		public static Exception ErrorException;

		public static string ErrorMessage;

		static IvyParser()
		{
			IvyParser.Namespaces = new XmlSerializerNamespaces();
			IvyParser.Namespaces.Add("e", "http://ant.apache.org/ivy/extra");
		}

		public static T ParseFile<T>(string path) where T : class
		{
			IvyParser.HasErrors = false;
			IvyParser.ErrorMessage = null;
			IvyParser.ErrorException = null;
			T result;
			if (!File.Exists(path))
			{
				IvyParser.HasErrors = true;
				IvyParser.ErrorMessage = string.Format("File does not exist: {0}", path);
				result = (T)((object)null);
			}
			else
			{
				object obj = IvyParser.Parse<T>(File.ReadAllText(path, Encoding.UTF8));
				if (obj != null && typeof(T) == typeof(IvyModule))
				{
					((IvyModule)obj).BasePath = Path.GetDirectoryName(path);
					((IvyModule)obj).IvyFile = Path.GetFileName(path);
				}
				result = (obj as T);
			}
			return result;
		}

		public static T Parse<T>(string xml) where T : class
		{
			object obj = IvyParser.Deserialize<T>(xml);
			if (obj != null && typeof(T) == typeof(IvyModule))
			{
				((IvyModule)obj).IvyFile = "ivy.xml";
			}
			return obj as T;
		}

		public static string Serialize(ModuleRepository repo)
		{
			IvyParser.HasErrors = false;
			IvyParser.ErrorMessage = null;
			IvyParser.ErrorException = null;
			Unity.PackageManager.IvyInternal.ModuleRepository repository = Cloner.CloneObject<Unity.PackageManager.IvyInternal.ModuleRepository>(repo);
			IvyRoot ivyRoot = new IvyRoot();
			ivyRoot.Repository = repository;
			StringBuilder stringBuilder = new StringBuilder();
			using (UTF8StringWriter uTF8StringWriter = new UTF8StringWriter(stringBuilder))
			{
				XmlSerializer serializer = XmlSerializable.GetSerializer(ivyRoot.GetType());
				serializer.Serialize(uTF8StringWriter, ivyRoot, IvyParser.Namespaces);
			}
			return stringBuilder.ToString().Replace("<root>", "").Replace("</root>", "");
		}

		public static string Serialize(IvyModule module)
		{
			IvyParser.HasErrors = false;
			IvyParser.ErrorMessage = null;
			IvyParser.ErrorException = null;
			Unity.PackageManager.IvyInternal.IvyModule ivyModule = Cloner.CloneObject<Unity.PackageManager.IvyInternal.IvyModule>(module);
			IvyRoot ivyRoot = new IvyRoot();
			ivyRoot.Module = ivyModule;
			StringBuilder stringBuilder = new StringBuilder();
			using (UTF8StringWriter uTF8StringWriter = new UTF8StringWriter(stringBuilder))
			{
				XmlSerializer serializer = XmlSerializable.GetSerializer(ivyModule.GetType());
				serializer.Serialize(uTF8StringWriter, ivyModule, IvyParser.Namespaces);
			}
			return stringBuilder.ToString().Replace("<root>", "").Replace("</root>", "");
		}

		public static T Deserialize<T>(string xml) where T : class
		{
			IvyParser.HasErrors = false;
			IvyParser.ErrorMessage = null;
			IvyParser.ErrorException = null;
			T result;
			if (xml.Length <= 0)
			{
				IvyParser.HasErrors = true;
				IvyParser.ErrorMessage = "Cannot deserialize empty xml document.";
				result = (T)((object)null);
			}
			else
			{
				Type typeFromHandle = typeof(T);
				int startIndex = 0;
				int num = xml.IndexOf("<ivy-module");
				int num2 = xml.IndexOf("<ivy-repository");
				if (typeof(T) == typeof(IvyModule))
				{
					if (num < 0)
					{
						result = (T)((object)null);
						return result;
					}
					startIndex = num;
					typeFromHandle = typeof(IvyModule);
				}
				else if (typeof(T) == typeof(ModuleRepository))
				{
					if (num2 < 0)
					{
						result = (T)((object)null);
						return result;
					}
					startIndex = num2;
					typeFromHandle = typeof(ModuleRepository);
				}
				else if (typeof(T) == typeof(object))
				{
					if (num2 >= 0)
					{
						startIndex = num2;
						typeFromHandle = typeof(ModuleRepository);
					}
					else if (num >= 0)
					{
						startIndex = num;
						typeFromHandle = typeof(IvyModule);
					}
				}
				xml = xml.Insert(startIndex, "<root>") + "</root>";
				try
				{
					XmlTextReader xmlReader = new XmlTextReader(xml, XmlNodeType.Document, null);
					XmlSerializer serializer = XmlSerializable.GetSerializer(typeof(IvyRoot));
					IvyRoot ivyRoot = serializer.Deserialize(xmlReader) as IvyRoot;
					if (typeFromHandle == typeof(IvyModule))
					{
						result = (Cloner.CloneObject(ivyRoot.Module, typeFromHandle) as T);
						return result;
					}
					result = (Cloner.CloneObject(ivyRoot.Repository, typeFromHandle) as T);
					return result;
				}
				catch (Exception errorException)
				{
					IvyParser.HasErrors = true;
					IvyParser.ErrorMessage = "Deserialization failed.";
					IvyParser.ErrorException = errorException;
				}
				result = (T)((object)null);
			}
			return result;
		}
	}
}
