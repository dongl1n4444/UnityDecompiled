namespace Unity.Utilities
{
    using MonoDevelop.Components.Commands;
    using MonoDevelop.Core;
    using MonoDevelop.Ide;
    using MonoDevelop.Ide.Gui;
    using System;
    using System.IO;

    internal class SearchReferenceCommandHandler : CommandHandler
    {
        private string apiBase;
        private static string classReferencePage = "20_class_hierarchy.html";
        private static string onlineApiBase = "http://unity3d.com/support/documentation/ScriptReference";
        private static string searchPage = "30_search.html";
        private static char[] tokenBreakers = new char[] { ' ', '\t', '(', ')', '[', ']', '{', '}', ';', ':' };

        public SearchReferenceCommandHandler()
        {
            string[] strArray = new string[0];
            if (Platform.IsMac)
            {
                strArray = new string[] { "/Applications/Unity/Documentation/ScriptReference" };
            }
            else if (Platform.IsWindows)
            {
                strArray = new string[] { "C:/Program Files/Unity/Editor/Data/Documentation/Documentation/ScriptReference", "C:/Program Files (x86)/Unity/Editor/Data/Documentation/Documentation/ScriptReference" };
            }
            foreach (string str in strArray)
            {
                if (Directory.Exists(str))
                {
                    this.apiBase = str;
                    break;
                }
            }
            if (string.IsNullOrEmpty(this.apiBase))
            {
                this.apiBase = onlineApiBase;
            }
        }

        private static string GetBaseToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                int num = token.LastIndexOf('.');
                if ((0 <= num) && ((token.Length - 1) > num))
                {
                    return token.Substring(num + 1);
                }
            }
            return token;
        }

        private static string GetCurrentToken()
        {
            Document document = IdeApp.get_Workbench().get_ActiveDocument();
            if (null != document)
            {
                if (document.get_Editor().get_IsSomethingSelected())
                {
                    return document.get_Editor().get_SelectedText().Trim();
                }
                int startIndex = Math.Max(1, document.get_Editor().get_Caret().get_Column() - 1);
                string lineText = document.get_Editor().GetLineText(document.get_Editor().get_Caret().get_Line());
                if (3 < lineText.Length)
                {
                    int num2 = lineText.LastIndexOfAny(tokenBreakers, startIndex - 1);
                    int num3 = lineText.IndexOfAny(tokenBreakers, startIndex);
                    if (0 > num3)
                    {
                        num3 = lineText.Length;
                    }
                    int length = (num3 - num2) - 1;
                    if ((0 < length) && (lineText.Length >= ((num2 + 1) + length)))
                    {
                        return lineText.Substring(num2 + 1, length).Trim();
                    }
                }
            }
            return string.Empty;
        }

        protected override void Run()
        {
            string currentToken = GetCurrentToken();
            string baseToken = GetBaseToken(currentToken);
            if (string.IsNullOrEmpty(currentToken))
            {
                DesktopService.ShowUrl($"file://{this.apiBase}/{classReferencePage}");
            }
            else
            {
                string[] strArray = new string[] { currentToken, currentToken.Replace('.', '-'), baseToken };
                foreach (string str3 in strArray)
                {
                    string path = $"{this.apiBase}/{str3}.html";
                    if (File.Exists(path))
                    {
                        DesktopService.ShowUrl($"file://{path}");
                        return;
                    }
                }
                DesktopService.ShowUrl($"{onlineApiBase}/{searchPage}?q={baseToken}");
            }
        }

        protected override void Update(CommandInfo item)
        {
            bool flag;
            item.set_Enabled(flag = true);
            item.set_Visible(flag);
        }
    }
}

