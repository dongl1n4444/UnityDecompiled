namespace UnityEditor.PlaymodeTestsRunner
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Text;
    using UnityEngine;
    using UnityEngine.PlaymodeTestsRunner;

    internal class XmlResultWriter
    {
        private const string k_NUnitVersion = "2.6.4-Unity";
        private int m_Indent;
        private string m_Platform;
        private readonly TestResult[] m_Results;
        private readonly StringBuilder m_ResultWriter = new StringBuilder();
        private readonly string m_SuiteName;

        public XmlResultWriter(string suiteName, string platform, TestResult[] results)
        {
            this.m_SuiteName = suiteName;
            this.m_Results = results;
            this.m_Platform = platform;
        }

        private static string EnvironmentGetCurrentDirectory()
        {
            return Environment.CurrentDirectory;
        }

        private static string GetEnvironmentMachineName()
        {
            return Environment.MachineName;
        }

        private static string GetEnvironmentOSVersion()
        {
            return Environment.OSVersion.ToString();
        }

        private static string GetEnvironmentOSVersionPlatform()
        {
            return Environment.OSVersion.Platform.ToString();
        }

        private static string GetEnvironmentUserDomainName()
        {
            return Environment.UserDomainName;
        }

        private static string GetEnvironmentUserName()
        {
            return Environment.UserName;
        }

        private static string GetEnvironmentVersion()
        {
            return Environment.Version.ToString();
        }

        public string GetTestResult()
        {
            this.InitializeXmlFile(this.m_SuiteName, new ResultSummarizer(this.m_Results));
            foreach (TestResult result in this.m_Results)
            {
                this.WriteResultElement(result);
            }
            this.TerminateXmlFile();
            return this.m_ResultWriter.ToString();
        }

        private void InitializeXmlFile(string resultsName, ResultSummarizer summaryResults)
        {
            this.WriteHeader();
            DateTime now = DateTime.Now;
            Dictionary<string, string> attributes = new Dictionary<string, string> {
                { 
                    "name",
                    "Unity Tests"
                },
                { 
                    "total",
                    summaryResults.TestsRun.ToString()
                },
                { 
                    "errors",
                    summaryResults.errors.ToString()
                },
                { 
                    "failures",
                    summaryResults.failures.ToString()
                },
                { 
                    "not-run",
                    summaryResults.testsNotRun.ToString()
                },
                { 
                    "inconclusive",
                    summaryResults.inconclusive.ToString()
                },
                { 
                    "ignored",
                    summaryResults.ignored.ToString()
                },
                { 
                    "skipped",
                    summaryResults.Skipped.ToString()
                },
                { 
                    "invalid",
                    summaryResults.notRunnable.ToString()
                },
                { 
                    "date",
                    now.ToString("yyyy-MM-dd")
                },
                { 
                    "time",
                    now.ToString("HH:mm:ss")
                }
            };
            this.WriteOpeningElement("test-results", attributes);
            this.WriteEnvironment(this.m_Platform);
            this.WriteCultureInfo();
            this.WriteTestSuite(resultsName, summaryResults);
            this.WriteOpeningElement("results");
        }

        private void StartTestElement(TestResult result)
        {
            string str;
            Dictionary<string, string> attributes = new Dictionary<string, string> {
                { 
                    "name",
                    result.fullName
                },
                { 
                    "executed",
                    (result.resultType == TestResult.ResultType.NotRun) ? "false" : "true"
                }
            };
            TestResult.ResultType resultType = result.resultType;
            if (resultType == TestResult.ResultType.Success)
            {
                str = "Success";
            }
            else if (resultType == TestResult.ResultType.Failed)
            {
                str = "Failure";
            }
            else
            {
                str = "Inconclusive";
            }
            attributes.Add("result", str);
            if (result.resultType != TestResult.ResultType.NotRun)
            {
                attributes.Add("success", (result.resultType == TestResult.ResultType.Success).ToString());
                attributes.Add("time", result.duration.ToString("#####0.000", NumberFormatInfo.InvariantInfo));
            }
            this.WriteOpeningElement("test-case", attributes);
        }

        private void TerminateXmlFile()
        {
            this.WriteClosingElement("results");
            this.WriteClosingElement("test-suite");
            this.WriteClosingElement("test-results");
        }

        private void WriteCData(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                this.m_ResultWriter.AppendFormat("<![CDATA[{0}]]>", text);
                this.m_ResultWriter.AppendLine();
            }
        }

        private void WriteClosingElement(string elementName)
        {
            this.m_Indent--;
            this.WriteIndent();
            this.m_ResultWriter.AppendLine("</" + elementName + ">");
        }

        private void WriteCultureInfo()
        {
            Dictionary<string, string> attributes = new Dictionary<string, string> {
                { 
                    "current-culture",
                    CultureInfo.CurrentCulture.ToString()
                },
                { 
                    "current-uiculture",
                    CultureInfo.CurrentUICulture.ToString()
                }
            };
            this.WriteOpeningElement("culture-info", attributes, true);
        }

        private void WriteEnvironment(string targetPlatform)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string> {
                { 
                    "nunit-version",
                    "2.6.4-Unity"
                },
                { 
                    "clr-version",
                    GetEnvironmentVersion()
                },
                { 
                    "os-version",
                    GetEnvironmentOSVersion()
                },
                { 
                    "platform",
                    GetEnvironmentOSVersionPlatform()
                },
                { 
                    "cwd",
                    EnvironmentGetCurrentDirectory()
                },
                { 
                    "machine-name",
                    GetEnvironmentMachineName()
                },
                { 
                    "user",
                    GetEnvironmentUserName()
                },
                { 
                    "user-domain",
                    GetEnvironmentUserDomainName()
                },
                { 
                    "unity-version",
                    Application.unityVersion
                },
                { 
                    "unity-platform",
                    targetPlatform
                }
            };
            this.WriteOpeningElement("environment", attributes, true);
        }

        private void WriteFailureElement(TestResult result)
        {
            this.WriteOpeningElement("failure");
            this.WriteOpeningElement("message");
            this.WriteCData(result.messages);
            this.WriteClosingElement("message");
            this.WriteOpeningElement("stack-trace");
            if (result.stacktrace != null)
            {
                this.WriteCData(StackTraceFilter.Filter(result.stacktrace));
            }
            this.WriteClosingElement("stack-trace");
            this.WriteClosingElement("failure");
        }

        private void WriteHeader()
        {
            this.m_ResultWriter.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            this.m_ResultWriter.AppendLine("<!--This file represents the results of running a test suite-->");
        }

        private void WriteIndent()
        {
            for (int i = 0; i < this.m_Indent; i++)
            {
                this.m_ResultWriter.Append("  ");
            }
        }

        private void WriteOpeningElement(string elementName)
        {
            this.WriteOpeningElement(elementName, new Dictionary<string, string>());
        }

        private void WriteOpeningElement(string elementName, Dictionary<string, string> attributes)
        {
            this.WriteOpeningElement(elementName, attributes, false);
        }

        private void WriteOpeningElement(string elementName, Dictionary<string, string> attributes, bool closeImmediatelly)
        {
            this.WriteIndent();
            this.m_Indent++;
            this.m_ResultWriter.Append("<");
            this.m_ResultWriter.Append(elementName);
            foreach (KeyValuePair<string, string> pair in attributes)
            {
                this.m_ResultWriter.AppendFormat(" {0}=\"{1}\"", pair.Key, SecurityElement.Escape(pair.Value));
            }
            if (closeImmediatelly)
            {
                this.m_ResultWriter.Append(" /");
                this.m_Indent--;
            }
            this.m_ResultWriter.AppendLine(">");
        }

        private void WriteReasonElement(TestResult result)
        {
            this.WriteOpeningElement("reason");
            this.WriteOpeningElement("message");
            this.WriteCData(result.messages);
            this.WriteClosingElement("message");
            this.WriteClosingElement("reason");
        }

        private void WriteResultElement(TestResult result)
        {
            this.StartTestElement(result);
            switch (result.resultType)
            {
                case TestResult.ResultType.NotRun:
                    this.WriteReasonElement(result);
                    break;

                case TestResult.ResultType.Failed:
                    this.WriteFailureElement(result);
                    break;

                case TestResult.ResultType.Success:
                    if (result.messages != null)
                    {
                        this.WriteReasonElement(result);
                    }
                    break;
            }
            this.WriteClosingElement("test-case");
        }

        private void WriteTestSuite(string resultsName, ResultSummarizer summaryResults)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string> {
                { 
                    "name",
                    resultsName
                },
                { 
                    "type",
                    "Assembly"
                },
                { 
                    "executed",
                    "True"
                },
                { 
                    "result",
                    !summaryResults.success ? "Failure" : "Success"
                },
                { 
                    "success",
                    !summaryResults.success ? "False" : "True"
                },
                { 
                    "time",
                    summaryResults.duration.ToString("#####0.000", NumberFormatInfo.InvariantInfo)
                }
            };
            this.WriteOpeningElement("test-suite", attributes);
        }

        public void WriteToFile(string resultDestiantion, string resultFileName)
        {
            try
            {
                string path = Path.Combine(resultDestiantion, resultFileName);
                Debug.Log("Saving results in " + path);
                File.WriteAllText(path, this.GetTestResult(), Encoding.UTF8);
            }
            catch (Exception exception)
            {
                Debug.LogError("Error while opening file");
                Debug.LogException(exception);
            }
        }
    }
}

