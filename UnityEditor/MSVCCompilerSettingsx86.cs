﻿using System;

internal class MSVCCompilerSettingsx86 : ICompilerSettings
{
    private readonly string m_CompilerPath;
    private readonly string[] m_LibPaths;
    private readonly string m_LinkerPath;

    public MSVCCompilerSettingsx86()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VS100COMNTOOLS")))
        {
            throw new Exception("Environment variable 'VS100COMNTOOLS' is not set indicating Visual Studio is not properly installed.");
        }
        this.m_CompilerPath = Environment.ExpandEnvironmentVariables(@"%VS100COMNTOOLS%..\..\VC\bin\cl.exe");
        this.m_LinkerPath = Environment.ExpandEnvironmentVariables(@"%VS100COMNTOOLS%..\..\VC\bin\link.exe");
        this.m_LibPaths = new string[] { Environment.ExpandEnvironmentVariables(@"%VS100COMNTOOLS%..\..\VC\lib"), Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Lib") };
    }

    public string CompilerPath =>
        this.m_CompilerPath;

    public string[] LibPaths =>
        this.m_LibPaths;

    public string LinkerPath =>
        this.m_LinkerPath;

    public string MachineSpecification =>
        "X86";
}

