namespace Unity.TinyProfiling
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class GraphMaker
    {
        private const int _heightForSection = 15;
        private readonly Palette _palette = new Palette();
        [CompilerGenerated]
        private static Func<TinyProfiler.ThreadContext, IEnumerable<TinyProfiler.TimedSection>> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<TinyProfiler.TimedSection, double> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<TinyProfiler.ThreadContext, bool> <>f__am$cache2;
        private double millisecondsToPixels;
        private const int YOfTopSection = 20;
        private int YTopOfCurrentThread;

        private string Clean(string details)
        {
            if (details.Contains("<"))
            {
                details = details.Replace("<", "");
            }
            if (details.Contains(">"))
            {
                details = details.Replace(">", "");
            }
            return details;
        }

        private StringBuilder EmitSingleThread(TinyProfiler.ThreadContext threadContext)
        {
            List<TinyProfiler.TimedSection> sections = threadContext.Sections;
            int num = this.MaxParents(sections);
            int num2 = 20 + (15 * (num + 1));
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("<rect x=\"0.0\" y=\"{0}\" width=\"2000.0\" height=\"{1}\" fill=\"url(#background)\"  />", this.YTopOfCurrentThread, num2));
            builder.AppendLine(string.Format("<text text-anchor=\"left\" x=\"0\" y=\"{0}\" font-size=\"12\" font-family=\"Verdana\" fill=\"rgb(0,0,0)\"  >ThreadID: {1} {2}</text>", this.YTopOfCurrentThread + 12, threadContext.ThreadID, threadContext.ThreadName));
            for (int i = 0; i != sections.Count; i++)
            {
                builder.Append(this.SectionRect(sections, i));
            }
            this.YTopOfCurrentThread += num2 + 50;
            return builder;
        }

        private double FontSizeFor(double duration)
        {
            double num = this.Scale(duration);
            float num2 = 20f;
            float num3 = 2f;
            float num4 = 200f;
            float num5 = 10f;
            double num6 = (num - num2) / ((double) (num4 - num2));
            if (num6 > 1.0)
            {
                num6 = 1.0;
            }
            return (num3 + (num6 * (num5 - num3)));
        }

        private string Header(string title)
        {
            return string.Format("<?xml version=\"1.0\" standalone=\"no\"?>\n<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">\n<svg version=\"1.1\" width=\"2000\" height=\"800\" onload=\"init(evt)\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">\n<defs >\n    <linearGradient id=\"background\" y1=\"0\" y2=\"1\" x1=\"0\" x2=\"0\" >\n        <stop stop-color=\"#eeeeee\" offset=\"5%\" />\n        <stop stop-color=\"#eeeeb0\" offset=\"95%\" />\n    </linearGradient>\n</defs>\n<style type=\"text/css\">\n    .func_g:hover {{ fill:rgb(220,80,80); }}\n    .func_text {{ pointer-events:none; }}\n</style>\n<script type=\"text/ecmascript\">\n<![CDATA[\n    var details;\n    function init(evt) {{ details = document.getElementById(\"details\").firstChild; }}\n    function s(info) {{ details.nodeValue = info; }}\n    function c() {{ details.nodeValue = ' '; }}\n]]>\n</script>\n<script xlink:href=\"SVGPan.js\"/>\n\n<g id=\"viewport\" class=\"chart\" width=\"2000\">\n\n<text text-anchor=\"middle\" x=\"1000\" y=\"30\" font-size=\"17\" font-family=\"Verdana\" fill=\"rgb(0,0,0)\"  >{0}</text>\n", title);
        }

        public string MakeGraph(List<TinyProfiler.ThreadContext> threadContexts, string title)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<TinyProfiler.ThreadContext, IEnumerable<TinyProfiler.TimedSection>>(null, (IntPtr) <MakeGraph>m__0);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<TinyProfiler.TimedSection, double>(null, (IntPtr) <MakeGraph>m__1);
            }
            double num = Enumerable.Max<TinyProfiler.TimedSection>(Enumerable.SelectMany<TinyProfiler.ThreadContext, TinyProfiler.TimedSection>(threadContexts, <>f__am$cache0), <>f__am$cache1);
            this.millisecondsToPixels = 2000.0 / num;
            StringBuilder builder = new StringBuilder();
            object[] objArray1 = new object[] { title, " ", num, "ms" };
            builder.Append(this.Header(string.Concat(objArray1)));
            this.YTopOfCurrentThread = 80;
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<TinyProfiler.ThreadContext, bool>(null, (IntPtr) <MakeGraph>m__2);
            }
            foreach (TinyProfiler.ThreadContext context in Enumerable.Where<TinyProfiler.ThreadContext>(threadContexts, <>f__am$cache2))
            {
                builder.Append(this.EmitSingleThread(context));
            }
            builder.Append("</g>");
            builder.Append("<rect x=\"20\" y=\"20\" rx=\"5\" ry=\"5\" width=\"550\" height=\"30\" style=\"fill:white;stroke:black;stroke-width:2;\"/>");
            builder.Append("<text text-anchor=\"\" x=\"25\" y=\"40\" font-size=\"16\" font-family=\"Verdana\" fill=\"rgb(0,0,0)\" id=\"details\" > </text></svg>");
            return builder.ToString();
        }

        private int MaxParents(List<TinyProfiler.TimedSection> sections)
        {
            int num = 0;
            for (int i = 0; i != sections.Count; i++)
            {
                num = Math.Max(num, this.NumberOfParents(sections, i));
            }
            return num;
        }

        private int NumberOfParents(List<TinyProfiler.TimedSection> sections, int index)
        {
            TinyProfiler.TimedSection section = sections[index];
            int parent = section.Parent;
            if (parent == -1)
            {
                return 0;
            }
            return (this.NumberOfParents(sections, parent) + 1);
        }

        private double Scale(double milliseconds)
        {
            return (milliseconds * this.millisecondsToPixels);
        }

        private string SectionRect(List<TinyProfiler.TimedSection> sections, int index)
        {
            int num = (this.YTopOfCurrentThread + 20) + (this.NumberOfParents(sections, index) * 15);
            TinyProfiler.TimedSection section = sections[index];
            StringBuilder builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine(string.Format("<rect class=\"func_g\" onmouseover=\"s('{4}')\" onmouseout=\"c()\" x=\"{0}\" y=\"{1}\" width=\"{2}\" height=\"15.0\" fill=\"{3}\" rx=\"0\" ry=\"0\" />", new object[] { this.Scale(section.Start).ToString(CultureInfo.InvariantCulture), num, this.Scale(section.Duration).ToString(CultureInfo.InvariantCulture), this._palette.ColorFor(section.Label), this.Clean(section.Summary) }));
            double num4 = this.FontSizeFor(section.Duration);
            if (num4 > 2.0)
            {
                builder.AppendLine(string.Format("<text class=\"func_text\" text-anchor=\"top\" x=\"{0}\" y=\"{1}\" font-size=\"{2}\" font-family=\"Verdana\" fill=\"rgb(0,0,s0)\">{3}</text>", new object[] { this.Scale(section.Start).ToString(CultureInfo.InvariantCulture), num + 13, num4.ToString(CultureInfo.InvariantCulture), section.Label }));
            }
            return builder.ToString();
        }
    }
}

