namespace UnityEditor.EditorTestsRunner
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Text;
    using UnityEditor.EditorTests;
    using UnityEngine;

    internal class XmlResultWriter
    {
        private const string k_NUnitVersion = "2.6.4-Unity";
        private int m_Indend;
        private string m_Platform;
        private readonly ITestResult[] m_Results;
        private readonly StringBuilder m_ResultWriter = new StringBuilder();
        private readonly string m_SuiteName;

        public XmlResultWriter(string suiteName, string platform, ITestResult[] results)
        {
            this.m_SuiteName = suiteName;
            this.m_Results = results;
            this.m_Platform = platform;
        }

        private static string EnvironmentGetCurrentDirectory() => 
            Environment.CurrentDirectory;

        private static string GetEnvironmentMachineName() => 
            Environment.MachineName;

        private static string GetEnvironmentOSVersion() => 
            Environment.OSVersion.ToString();

        private static string GetEnvironmentOSVersionPlatform() => 
            Environment.OSVersion.Platform.ToString();

        private static string GetEnvironmentUserDomainName() => 
            Environment.UserDomainName;

        private static string GetEnvironmentUserName() => 
            Environment.UserName;

        private static string GetEnvironmentVersion() => 
            Environment.Version.ToString();

        public string GetTestResult()
        {
            this.InitializeXmlFile(this.m_SuiteName, new ResultSummarizer(this.m_Results));
            foreach (ITestResult result in this.m_Results)
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
                    summaryResults.testsRun.ToString()
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
                    summaryResults.skipped.ToString()
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

        private void StartTestElement(ITestResult result)
        {
            string str;
            Dictionary<string, string> attributes = new Dictionary<string, string> {
                { 
                    "name",
                    result.fullName
                },
                { 
                    "executed",
                    result.executed.ToString()
                }
            };
            if (result.resultState == TestResultState.Cancelled)
            {
                str = TestResultState.Failure.ToString();
            }
            else
            {
                str = result.resultState.ToString();
            }
            attributes.Add("result", str);
            if (result.executed)
            {
                attributes.Add("success", result.isSuccess.ToString());
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
            if (text.Length != 0)
            {
                this.m_ResultWriter.AppendFormat("<![CDATA[{0}]]>", text);
                this.m_ResultWriter.AppendLine();
            }
        }

        private void WriteClosingElement(string elementName)
        {
            this.m_Indend--;
            this.WriteIndend();
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

        private void WriteFailureElement(ITestResult result)
        {
            this.WriteOpeningElement("failure");
            this.WriteOpeningElement("message");
            this.WriteCData(result.message);
            this.WriteClosingElement("message");
            this.WriteOpeningElement("stack-trace");
            if (result.stackTrace != null)
            {
                this.WriteCData(StackTraceFilter.Filter(result.stackTrace));
            }
            this.WriteClosingElement("stack-trace");
            this.WriteClosingElement("failure");
        }

        private void WriteHeader()
        {
            this.m_ResultWriter.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            this.m_ResultWriter.AppendLine("<!--This file represents the results of running a test suite-->");
        }

        private void WriteIndend()
        {
            for (int i = 0; i < this.m_Indend; i++)
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
            this.WriteIndend();
            this.m_Indend++;
            this.m_ResultWriter.Append("<");
            this.m_ResultWriter.Append(elementName);
            foreach (KeyValuePair<string, string> pair in attributes)
            {
                this.m_ResultWriter.AppendFormat(" {0}=\"{1}\"", pair.Key, SecurityElement.Escape(pair.Value));
            }
            if (closeImmediatelly)
            {
                this.m_ResultWriter.Append(" /");
                this.m_Indend--;
            }
            this.m_ResultWriter.AppendLine(">");
        }

        private void WriteReasonElement(ITestResult result)
        {
            this.WriteOpeningElement("reason");
            this.WriteOpeningElement("message");
            this.WriteCData(result.message);
            this.WriteClosingElement("message");
            this.WriteClosingElement("reason");
        }

        private void WriteResultElement(ITestResult result)
        {
            this.StartTestElement(result);
            switch (result.resultState)
            {
                case TestResultState.Inconclusive:
                case TestResultState.Success:
                    if (result.message != null)
                    {
                        this.WriteReasonElement(result);
                    }
                    break;

                case TestResultState.NotRunnable:
                case TestResultState.Skipped:
                case TestResultState.Ignored:
                    this.WriteReasonElement(result);
                    break;

                case TestResultState.Failure:
                case TestResultState.Error:
                case TestResultState.Cancelled:
                    this.WriteFailureElement(result);
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

