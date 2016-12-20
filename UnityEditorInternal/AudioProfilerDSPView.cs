namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    internal class AudioProfilerDSPView
    {
        private const int AUDIOPROFILER_DSPFLAGS_ACTIVE = 1;
        private const int AUDIOPROFILER_DSPFLAGS_BYPASS = 2;

        private void DrawRectClipped(Rect r, Color col, string name, Rect c, float zoomFactor)
        {
            Rect rect = new Rect(r.x * zoomFactor, r.y * zoomFactor, r.width * zoomFactor, r.height * zoomFactor);
            float x = rect.x;
            float a = rect.x + rect.width;
            float y = rect.y;
            float num4 = rect.y + rect.height;
            float b = c.x;
            float num6 = c.x + c.width;
            float num7 = c.y;
            float num8 = c.y + c.height;
            float num9 = Mathf.Max(x, b);
            float num10 = Mathf.Max(y, num7);
            float num11 = Mathf.Min(a, num6);
            float num12 = Mathf.Min(num4, num8);
            if ((num9 < num11) && (num10 < num12))
            {
                if (name == null)
                {
                    EditorGUI.DrawRect(rect, col);
                }
                else
                {
                    GUI.color = col;
                    GUI.Button(rect, name);
                }
            }
        }

        private static int GetOutCode(Vector3 p, Rect c)
        {
            int num = 0;
            if (p.x < c.x)
            {
                num |= 1;
            }
            if (p.x > (c.x + c.width))
            {
                num |= 2;
            }
            if (p.y < c.y)
            {
                num |= 4;
            }
            if (p.y > (c.y + c.height))
            {
                num |= 8;
            }
            return num;
        }

        public void OnGUI(Rect clippingRect, ProfilerProperty property, bool showInactiveDSPChains, bool highlightAudibleDSPChains, ref float zoomFactor, ref Vector2 scrollPos)
        {
            if (((Event.current.type == EventType.ScrollWheel) && clippingRect.Contains(Event.current.mousePosition)) && Event.current.shift)
            {
                float num = 1.05f;
                float num2 = zoomFactor;
                zoomFactor *= (Event.current.delta.y <= 0f) ? (1f / num) : num;
                scrollPos = (Vector2) (scrollPos + ((Event.current.mousePosition - scrollPos) * (zoomFactor - num2)));
                Event.current.Use();
            }
            else if (Event.current.type == EventType.Repaint)
            {
                int num3 = 0x40;
                int num4 = 0x10;
                int num5 = 140;
                int num6 = 30;
                int num7 = num3 + 10;
                int num8 = 5;
                Dictionary<int, AudioProfilerDSPNode> dictionary = new Dictionary<int, AudioProfilerDSPNode>();
                List<AudioProfilerDSPWire> list = new List<AudioProfilerDSPWire>();
                AudioProfilerDSPInfo[] audioProfilerDSPInfo = property.GetAudioProfilerDSPInfo();
                if (audioProfilerDSPInfo != null)
                {
                    bool flag = true;
                    foreach (AudioProfilerDSPInfo info in audioProfilerDSPInfo)
                    {
                        if (showInactiveDSPChains || ((info.flags & 1) != 0))
                        {
                            if (!dictionary.ContainsKey(info.id))
                            {
                                AudioProfilerDSPNode firstTarget = !dictionary.ContainsKey(info.target) ? null : dictionary[info.target];
                                if (firstTarget != null)
                                {
                                    dictionary[info.id] = new AudioProfilerDSPNode(firstTarget, info, (firstTarget.x + num5) + num7, firstTarget.maxY, firstTarget.level + 1);
                                    firstTarget.maxY += num6 + num8;
                                    for (AudioProfilerDSPNode node2 = firstTarget; node2 != null; node2 = node2.firstTarget)
                                    {
                                        node2.maxY = Mathf.Max(node2.maxY, firstTarget.maxY);
                                    }
                                }
                                else if (flag)
                                {
                                    flag = false;
                                    dictionary[info.id] = new AudioProfilerDSPNode(firstTarget, info, 10 + (num5 / 2), 10 + (num6 / 2), 1);
                                }
                                if (firstTarget != null)
                                {
                                    list.Add(new AudioProfilerDSPWire(dictionary[info.id], firstTarget, info));
                                }
                            }
                            else
                            {
                                list.Add(new AudioProfilerDSPWire(dictionary[info.id], dictionary[info.target], info));
                            }
                        }
                    }
                    foreach (KeyValuePair<int, AudioProfilerDSPNode> pair in dictionary)
                    {
                        AudioProfilerDSPNode node3 = pair.Value;
                        node3.y += ((node3.maxY != node3.y) ? (node3.maxY - node3.y) : (num6 + num8)) / 2;
                    }
                    foreach (AudioProfilerDSPWire wire in list)
                    {
                        float num10 = 4f;
                        AudioProfilerDSPNode source = wire.source;
                        AudioProfilerDSPNode target = wire.target;
                        AudioProfilerDSPInfo info2 = wire.info;
                        Vector3 p = new Vector3((source.x - (num5 * 0.5f)) * zoomFactor, source.y * zoomFactor, 0f);
                        Vector3 vector3 = new Vector3((target.x + (num5 * 0.5f)) * zoomFactor, (target.y + (wire.targetPort * num10)) * zoomFactor, 0f);
                        int outCode = GetOutCode(p, clippingRect);
                        int num12 = GetOutCode(vector3, clippingRect);
                        if ((outCode & num12) == 0)
                        {
                            float width = 3f;
                            Handles.color = new Color(info2.weight, 0f, 0f, (highlightAudibleDSPChains && !source.audible) ? 0.4f : 1f);
                            Vector3[] points = new Vector3[] { p, vector3 };
                            Handles.DrawAAPolyLine(width, 2, points);
                        }
                    }
                    foreach (AudioProfilerDSPWire wire2 in list)
                    {
                        AudioProfilerDSPNode node6 = wire2.source;
                        AudioProfilerDSPNode node7 = wire2.target;
                        AudioProfilerDSPInfo info3 = wire2.info;
                        if (info3.weight != 1f)
                        {
                            int num14 = node6.x - ((num7 + num5) / 2);
                            int num15 = (node7 == null) ? node7.y : (node7.y + ((int) ((((num14 - node7.x) - (num5 * 0.5f)) * (node6.y - node7.y)) / ((float) ((node6.x - node7.x) - num5)))));
                            this.DrawRectClipped(new Rect((float) (num14 - (num3 / 2)), (float) (num15 - (num4 / 2)), (float) num3, (float) num4), new Color(1f, 0.3f, 0.2f, (highlightAudibleDSPChains && !node6.audible) ? 0.4f : 1f), string.Format("{0:0.00}%", 100f * info3.weight), clippingRect, zoomFactor);
                        }
                    }
                    foreach (KeyValuePair<int, AudioProfilerDSPNode> pair2 in dictionary)
                    {
                        AudioProfilerDSPNode node8 = pair2.Value;
                        AudioProfilerDSPInfo info4 = node8.info;
                        if (dictionary.ContainsKey(info4.target) && (node8.firstTarget == dictionary[info4.target]))
                        {
                            string audioProfilerNameByOffset = property.GetAudioProfilerNameByOffset(info4.nameOffset);
                            float num16 = 0.01f * info4.cpuLoad;
                            float num17 = 0.1f;
                            bool flag2 = (info4.flags & 1) != 0;
                            bool flag3 = (info4.flags & 2) != 0;
                            Color col = new Color((flag2 && !flag3) ? Mathf.Clamp((float) ((2f * num17) * num16), (float) 0f, (float) 1f) : 0.5f, (flag2 && !flag3) ? Mathf.Clamp((float) (2f - ((2f * num17) * num16)), (float) 0f, (float) 1f) : 0.5f, !flag3 ? (!flag2 ? 0.5f : 0f) : 1f, (highlightAudibleDSPChains && !node8.audible) ? 0.4f : 1f);
                            audioProfilerNameByOffset = audioProfilerNameByOffset.Replace("ChannelGroup", "Group").Replace("FMOD Channel", "Channel").Replace("FMOD WaveTable Unit", "Wavetable").Replace("FMOD Resampler Unit", "Resampler").Replace("FMOD Channel DSPHead Unit", "Channel DSP").Replace("FMOD Channel DSPHead Unit", "Channel DSP") + string.Format(" ({0:0.00}%)", num16);
                            this.DrawRectClipped(new Rect(node8.x - (num5 * 0.5f), node8.y - (num6 * 0.5f), (float) num5, (float) num6), col, audioProfilerNameByOffset, clippingRect, zoomFactor);
                            if (node8.audible)
                            {
                                if (info4.numLevels >= 1)
                                {
                                    float height = (num6 - 6) * Mathf.Clamp(info4.level1, 0f, 1f);
                                    this.DrawRectClipped(new Rect((node8.x - (num5 * 0.5f)) + 3f, (node8.y - (num6 * 0.5f)) + 3f, 4f, (float) (num6 - 6)), Color.black, null, clippingRect, zoomFactor);
                                    this.DrawRectClipped(new Rect((node8.x - (num5 * 0.5f)) + 3f, (((node8.y - (num6 * 0.5f)) - 3f) + num6) - height, 4f, height), Color.red, null, clippingRect, zoomFactor);
                                }
                                if (info4.numLevels >= 2)
                                {
                                    float num19 = (num6 - 6) * Mathf.Clamp(info4.level2, 0f, 1f);
                                    this.DrawRectClipped(new Rect((node8.x - (num5 * 0.5f)) + 8f, (node8.y - (num6 * 0.5f)) + 3f, 4f, (float) (num6 - 6)), Color.black, null, clippingRect, zoomFactor);
                                    this.DrawRectClipped(new Rect((node8.x - (num5 * 0.5f)) + 8f, (((node8.y - (num6 * 0.5f)) - 3f) + num6) - num19, 4f, num19), Color.red, null, clippingRect, zoomFactor);
                                }
                            }
                        }
                    }
                }
            }
        }

        private class AudioProfilerDSPNode
        {
            public bool audible;
            public AudioProfilerDSPView.AudioProfilerDSPNode firstTarget;
            public AudioProfilerDSPInfo info;
            public int level;
            public int maxY;
            public int targetPort;
            public int x;
            public int y;

            public AudioProfilerDSPNode(AudioProfilerDSPView.AudioProfilerDSPNode firstTarget, AudioProfilerDSPInfo info, int x, int y, int level)
            {
                this.firstTarget = firstTarget;
                this.info = info;
                this.x = x;
                this.y = y;
                this.level = level;
                this.maxY = y;
                this.audible = ((info.flags & 1) != 0) && ((info.flags & 2) == 0);
                if (firstTarget != null)
                {
                    this.audible &= firstTarget.audible;
                }
            }
        }

        private class AudioProfilerDSPWire
        {
            public AudioProfilerDSPInfo info;
            public AudioProfilerDSPView.AudioProfilerDSPNode source;
            public AudioProfilerDSPView.AudioProfilerDSPNode target;
            public int targetPort;

            public AudioProfilerDSPWire(AudioProfilerDSPView.AudioProfilerDSPNode source, AudioProfilerDSPView.AudioProfilerDSPNode target, AudioProfilerDSPInfo info)
            {
                this.source = source;
                this.target = target;
                this.info = info;
                this.targetPort = target.targetPort;
            }
        }
    }
}

