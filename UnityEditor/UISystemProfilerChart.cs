namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class UISystemProfilerChart : ProfilerChart
    {
        private string[] m_MarkerNames;
        private EventMarker[] m_Markers;

        public UISystemProfilerChart(Chart.ChartType type, float dataScale, int seriesCount) : base(ProfilerArea.UIDetails, type, dataScale, seriesCount)
        {
            base.m_Chart = new UISystemChart();
        }

        public override int DoChartGUI(int currentFrame, ProfilerArea currentArea, out Chart.ChartAction action)
        {
            int num = base.DoChartGUI(currentFrame, currentArea, out action);
            if ((this.m_Markers != null) && this.showMarkers)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();
                lastRect.xMin += 170f;
                for (int i = 0; i < this.m_Markers.Length; i++)
                {
                    EventMarker marker = this.m_Markers[i];
                    Color color = ProfilerColors.colors[(int) ((IntPtr) (((ulong) marker.objectInstanceId) % ((long) ProfilerColors.colors.Length)))];
                    Color introduced6 = color.AlphaMultiplied(0.3f);
                    Chart.DrawVerticalLine(marker.frame, base.m_Data, lastRect, introduced6, color.AlphaMultiplied(0.4f), 1f);
                }
                this.DrawMarkerLabels(base.m_Data, lastRect, this.m_Markers, this.m_MarkerNames);
            }
            return num;
        }

        private void DrawMarkerLabels(ChartData cdata, Rect r, EventMarker[] markers, string[] markerNames)
        {
            Color contentColor = GUI.contentColor;
            int numberOfFrames = cdata.NumberOfFrames;
            float num2 = r.width / ((float) numberOfFrames);
            int num3 = (int) (r.height / 12f);
            if (num3 != 0)
            {
                Dictionary<int, int> dictionary = new Dictionary<int, int>();
                for (int i = 0; i < markers.Length; i++)
                {
                    int num5;
                    int frame = markers[i].frame;
                    if ((!dictionary.TryGetValue(markers[i].nameOffset, out num5) || (num5 != (frame - 1))) || (num5 < cdata.firstFrame))
                    {
                        frame -= cdata.firstFrame;
                        if (frame >= 0)
                        {
                            float num7 = r.x + (num2 * frame);
                            Color color2 = ProfilerColors.colors[(int) ((IntPtr) (((ulong) markers[i].objectInstanceId) % ((long) ProfilerColors.colors.Length)))];
                            GUI.contentColor = (Color) ((color2 + Color.white) * 0.5f);
                            Chart.DoLabel(num7 + -1f, (r.y + r.height) - (((i % num3) + 1) * 12), markerNames[i], 0f);
                        }
                    }
                    dictionary[markers[i].nameOffset] = markers[i].frame;
                }
            }
            GUI.contentColor = contentColor;
        }

        public void Update(int firstFrame, int historyLength)
        {
            int uISystemEventMarkersCount = ProfilerDriver.GetUISystemEventMarkersCount(firstFrame, historyLength);
            if (uISystemEventMarkersCount != 0)
            {
                this.m_Markers = new EventMarker[uISystemEventMarkersCount];
                this.m_MarkerNames = new string[uISystemEventMarkersCount];
                ProfilerDriver.GetUISystemEventMarkersBatch(firstFrame, historyLength, this.m_Markers, this.m_MarkerNames);
            }
        }

        public bool showMarkers =>
            ((UISystemChart) base.m_Chart).showMarkers;
    }
}

