using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor.Utils;
using UnityEngine;

internal abstract class NativeCompiler : INativeCompiler
{
    [CompilerGenerated]
    private static Func<string, string, string> <>f__am$cache0;
    [CompilerGenerated]
    private static Func<string, string, string> <>f__am$cache1;
    [CompilerGenerated]
    private static Func<string, string, string> <>f__am$cache2;

    protected NativeCompiler()
    {
    }

    protected static string Aggregate(IEnumerable<string> items, string prefix, string suffix)
    {
        <Aggregate>c__AnonStorey0 storey = new <Aggregate>c__AnonStorey0 {
            prefix = prefix,
            suffix = suffix
        };
        return Enumerable.Aggregate<string, string>(items, "", new Func<string, string, string>(storey, (IntPtr) this.<>m__0));
    }

    protected internal static IEnumerable<string> AllSourceFilesIn(string directory)
    {
        return Enumerable.Concat<string>(Directory.GetFiles(directory, "*.cpp", SearchOption.AllDirectories), Directory.GetFiles(directory, "*.c", SearchOption.AllDirectories));
    }

    public abstract void CompileDynamicLibrary(string outFile, IEnumerable<string> sources, IEnumerable<string> includePaths, IEnumerable<string> libraries, IEnumerable<string> libraryPaths);
    protected void Execute(string arguments, string compilerPath)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo(compilerPath, arguments);
        this.SetupProcessStartInfo(startInfo);
        this.RunProgram(startInfo);
    }

    protected void ExecuteCommand(string command, params string[] arguments)
    {
        if (<>f__am$cache0 == null)
        {
            <>f__am$cache0 = new Func<string, string, string>(null, (IntPtr) <ExecuteCommand>m__0);
        }
        ProcessStartInfo startInfo = new ProcessStartInfo(command, Enumerable.Aggregate<string>(arguments, <>f__am$cache0));
        this.SetupProcessStartInfo(startInfo);
        this.RunProgram(startInfo);
    }

    protected internal static bool IsSourceFile(string source)
    {
        string extension = Path.GetExtension(source);
        return ((extension == "cpp") || (extension == "c"));
    }

    protected string ObjectFileFor(string source)
    {
        return Path.ChangeExtension(source, this.objectFileExtension);
    }

    internal static void ParallelFor<T>(T[] sources, Action<T> action)
    {
        <ParallelFor>c__AnonStorey1<T> storey = new <ParallelFor>c__AnonStorey1<T> {
            sources = sources,
            action = action
        };
        Thread[] threadArray = new Thread[Environment.ProcessorCount];
        Counter parameter = new Counter();
        for (int i = 0; i < threadArray.Length; i++)
        {
            threadArray[i] = new Thread(new ParameterizedThreadStart(storey.<>m__0));
        }
        foreach (Thread thread in threadArray)
        {
            thread.Start(parameter);
        }
        foreach (Thread thread2 in threadArray)
        {
            thread2.Join();
        }
    }

    private void RunProgram(ProcessStartInfo startInfo)
    {
        using (Program program = new Program(startInfo))
        {
            program.Start();
            while (!program.WaitForExit(100))
            {
            }
            string str = string.Empty;
            string[] standardOutput = program.GetStandardOutput();
            if (standardOutput.Length > 0)
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<string, string, string>(null, (IntPtr) <RunProgram>m__1);
                }
                str = Enumerable.Aggregate<string>(standardOutput, <>f__am$cache1);
            }
            string[] errorOutput = program.GetErrorOutput();
            if (errorOutput.Length > 0)
            {
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = new Func<string, string, string>(null, (IntPtr) <RunProgram>m__2);
                }
                str = str + Enumerable.Aggregate<string>(errorOutput, <>f__am$cache2);
            }
            if (program.ExitCode != 0)
            {
                Debug.LogError("Failed running " + startInfo.FileName + " " + startInfo.Arguments + "\n\n" + str);
                throw new Exception("IL2CPP compile failed.");
            }
        }
    }

    protected virtual void SetupProcessStartInfo(ProcessStartInfo startInfo)
    {
    }

    protected virtual string objectFileExtension
    {
        get
        {
            return "o";
        }
    }

    [CompilerGenerated]
    private sealed class <Aggregate>c__AnonStorey0
    {
        internal string prefix;
        internal string suffix;

        internal string <>m__0(string current, string additionalFile)
        {
            return (current + this.prefix + additionalFile + this.suffix);
        }
    }

    [CompilerGenerated]
    private sealed class <ParallelFor>c__AnonStorey1<T>
    {
        internal Action<T> action;
        internal T[] sources;

        internal void <>m__0(object obj)
        {
            int num;
            NativeCompiler.Counter counter = (NativeCompiler.Counter) obj;
            while ((num = Interlocked.Increment(ref counter.index)) <= this.sources.Length)
            {
                this.action(this.sources[num - 1]);
            }
        }
    }

    private class Counter
    {
        public int index;
    }
}

