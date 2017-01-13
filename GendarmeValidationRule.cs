using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Scripting;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Utils;

internal abstract class GendarmeValidationRule : IValidationRule
{
    private readonly string _gendarmeExePath;
    [CompilerGenerated]
    private static Func<string, string, string> <>f__am$cache0;

    protected GendarmeValidationRule(string gendarmeExePath)
    {
        this._gendarmeExePath = gendarmeExePath;
    }

    protected string BuildGendarmeCommandLineArguments(IEnumerable<string> userAssemblies)
    {
        GendarmeOptions options = this.ConfigureGendarme(userAssemblies);
        if ((options.UserAssemblies == null) || (options.UserAssemblies.Length == 0))
        {
            return null;
        }
        List<string> list = new List<string> {
            "--config " + options.ConfigFilePath,
            "--set " + options.RuleSet
        };
        list.AddRange(options.UserAssemblies);
        if (<>f__am$cache0 == null)
        {
            <>f__am$cache0 = (agg, i) => agg + " " + i;
        }
        return Enumerable.Aggregate<string>(list, <>f__am$cache0);
    }

    protected abstract GendarmeOptions ConfigureGendarme(IEnumerable<string> userAssemblies);
    private static ManagedProgram ManagedProgramFor(string exe, string arguments) => 
        new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), null, exe, arguments, false, null);

    private static bool StartManagedProgram(string exe, string arguments, CompilerOutputParserBase parser, ref IEnumerable<CompilerMessage> compilerMessages)
    {
        using (ManagedProgram program = ManagedProgramFor(exe, arguments))
        {
            program.LogProcessStartInfo();
            try
            {
                program.Start();
            }
            catch
            {
                throw new Exception("Could not start " + exe);
            }
            program.WaitForExit();
            if (program.ExitCode == 0)
            {
                return true;
            }
            compilerMessages = parser.Parse(program.GetErrorOutput(), program.GetStandardOutput(), true);
        }
        return false;
    }

    public ValidationResult Validate(IEnumerable<string> userAssemblies, params object[] options)
    {
        string arguments = this.BuildGendarmeCommandLineArguments(userAssemblies);
        ValidationResult result = new ValidationResult {
            Success = true,
            Rule = this,
            CompilerMessages = null
        };
        try
        {
            result.Success = StartManagedProgram(this._gendarmeExePath, arguments, new GendarmeOutputParser(), ref result.CompilerMessages);
        }
        catch (Exception exception)
        {
            result.Success = false;
            CompilerMessage[] messageArray1 = new CompilerMessage[1];
            CompilerMessage message = new CompilerMessage {
                file = "Exception",
                message = exception.Message,
                line = 0,
                column = 0,
                type = CompilerMessageType.Error
            };
            messageArray1[0] = message;
            result.CompilerMessages = messageArray1;
        }
        return result;
    }
}

