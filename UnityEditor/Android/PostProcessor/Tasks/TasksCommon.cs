namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Utils;

    internal class TasksCommon
    {
        public static string Exec(string command, string args, string workingdir, string errorMsg, int retriesOnFailure = 0)
        {
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = command,
                Arguments = args,
                WorkingDirectory = workingdir,
                CreateNoWindow = true
            };
            while (true)
            {
                try
                {
                    return Command.Run(psi, null, errorMsg);
                }
                catch (CommandInvokationFailure)
                {
                    if (retriesOnFailure == 0)
                    {
                        throw;
                    }
                }
                retriesOnFailure--;
            }
        }

        public static string GetClassDirectory(PostProcessorContext context)
        {
            string[] components = new string[] { GetVariationsDirectory(context), "Classes" };
            return Paths.Combine(components);
        }

        public static string GetLibsDirectory(PostProcessorContext context)
        {
            string[] components = new string[] { GetVariationsDirectory(context), "Libs" };
            return Paths.Combine(components);
        }

        public static string GetMD5HashOfEOCD(string fileName)
        {
            long num = 0x10016L;
            FileStream inputStream = new FileStream(fileName, System.IO.FileMode.Open);
            inputStream.Seek(inputStream.Length - Math.Min(inputStream.Length, num), SeekOrigin.Begin);
            byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(inputStream);
            inputStream.Close();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append(buffer[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static string GetSymbolsDirectory(PostProcessorContext context)
        {
            string[] components = new string[] { GetVariationsDirectory(context), "Symbols" };
            return Paths.Combine(components);
        }

        private static string GetVariationsDirectory(PostProcessorContext context)
        {
            ScriptingImplementation implementation = context.Get<ScriptingImplementation>("ScriptingBackend");
            string str = context.Get<string>("PlayerPackage");
            string str2 = context.Get<string>("Variation");
            string[] components = new string[] { str, "Variations", (implementation != ScriptingImplementation.IL2CPP) ? "mono" : "il2cpp", str2 };
            return Paths.Combine(components);
        }

        public static string SDKTool(PostProcessorContext context, string[] command, string workingdir, string errorMsg) => 
            context.Get<AndroidSDKTools>("SDKTools").RunCommand(command, workingdir, null, errorMsg);
    }
}

