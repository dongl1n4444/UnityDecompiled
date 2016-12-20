namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>Helper class to generate form data to post to web servers using the WWW class.</para>
    /// </summary>
    public sealed class WWWForm
    {
        private byte[] boundary = new byte[40];
        private bool containsFiles = false;
        private List<string> fieldNames = new List<string>();
        private List<string> fileNames = new List<string>();
        private List<byte[]> formData = new List<byte[]>();
        private List<string> types = new List<string>();

        /// <summary>
        /// <para>Creates an empty WWWForm object.</para>
        /// </summary>
        public WWWForm()
        {
            for (int i = 0; i < 40; i++)
            {
                int num2 = UnityEngine.Random.Range(0x30, 110);
                if (num2 > 0x39)
                {
                    num2 += 7;
                }
                if (num2 > 90)
                {
                    num2 += 6;
                }
                this.boundary[i] = (byte) num2;
            }
        }

        /// <summary>
        /// <para>Add binary data to the form.</para>
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="contents"></param>
        /// <param name="fileName"></param>
        /// <param name="mimeType"></param>
        [ExcludeFromDocs]
        public void AddBinaryData(string fieldName, byte[] contents)
        {
            string mimeType = null;
            string fileName = null;
            this.AddBinaryData(fieldName, contents, fileName, mimeType);
        }

        /// <summary>
        /// <para>Add binary data to the form.</para>
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="contents"></param>
        /// <param name="fileName"></param>
        /// <param name="mimeType"></param>
        [ExcludeFromDocs]
        public void AddBinaryData(string fieldName, byte[] contents, string fileName)
        {
            string mimeType = null;
            this.AddBinaryData(fieldName, contents, fileName, mimeType);
        }

        /// <summary>
        /// <para>Add binary data to the form.</para>
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="contents"></param>
        /// <param name="fileName"></param>
        /// <param name="mimeType"></param>
        public void AddBinaryData(string fieldName, byte[] contents, [DefaultValue("null")] string fileName, [DefaultValue("null")] string mimeType)
        {
            this.containsFiles = true;
            bool flag = ((((contents.Length > 8) && (contents[0] == 0x89)) && ((contents[1] == 80) && (contents[2] == 0x4e))) && (((contents[3] == 0x47) && (contents[4] == 13)) && ((contents[5] == 10) && (contents[6] == 0x1a)))) && (contents[7] == 10);
            if (fileName == null)
            {
                fileName = fieldName + (!flag ? ".dat" : ".png");
            }
            if (mimeType == null)
            {
                if (flag)
                {
                    mimeType = "image/png";
                }
                else
                {
                    mimeType = "application/octet-stream";
                }
            }
            this.fieldNames.Add(fieldName);
            this.fileNames.Add(fileName);
            this.formData.Add(contents);
            this.types.Add(mimeType);
        }

        /// <summary>
        /// <para>Adds a simple field to the form.</para>
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="i"></param>
        public void AddField(string fieldName, int i)
        {
            this.AddField(fieldName, i.ToString());
        }

        /// <summary>
        /// <para>Add a simple field to the form.</para>
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <param name="e"></param>
        [ExcludeFromDocs]
        public void AddField(string fieldName, string value)
        {
            Encoding e = Encoding.UTF8;
            this.AddField(fieldName, value, e);
        }

        /// <summary>
        /// <para>Add a simple field to the form.</para>
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <param name="e"></param>
        public void AddField(string fieldName, string value, [DefaultValue("System.Text.Encoding.UTF8")] Encoding e)
        {
            this.fieldNames.Add(fieldName);
            this.fileNames.Add(null);
            this.formData.Add(e.GetBytes(value));
            this.types.Add("text/plain; charset=\"" + e.WebName + "\"");
        }

        /// <summary>
        /// <para>(Read Only) The raw data to pass as the POST request body when sending the form.</para>
        /// </summary>
        public byte[] data
        {
            get
            {
                if (this.containsFiles)
                {
                    byte[] buffer = WWW.DefaultEncoding.GetBytes("--");
                    byte[] buffer2 = WWW.DefaultEncoding.GetBytes("\r\n");
                    byte[] buffer3 = WWW.DefaultEncoding.GetBytes("Content-Type: ");
                    byte[] buffer4 = WWW.DefaultEncoding.GetBytes("Content-disposition: form-data; name=\"");
                    byte[] buffer5 = WWW.DefaultEncoding.GetBytes("\"");
                    byte[] buffer6 = WWW.DefaultEncoding.GetBytes("; filename=\"");
                    using (MemoryStream stream = new MemoryStream(0x400))
                    {
                        for (int i = 0; i < this.formData.Count; i++)
                        {
                            stream.Write(buffer2, 0, buffer2.Length);
                            stream.Write(buffer, 0, buffer.Length);
                            stream.Write(this.boundary, 0, this.boundary.Length);
                            stream.Write(buffer2, 0, buffer2.Length);
                            stream.Write(buffer3, 0, buffer3.Length);
                            byte[] buffer7 = Encoding.UTF8.GetBytes(this.types[i]);
                            stream.Write(buffer7, 0, buffer7.Length);
                            stream.Write(buffer2, 0, buffer2.Length);
                            stream.Write(buffer4, 0, buffer4.Length);
                            string str = "";
                            string s = this.fieldNames[i];
                            if (!WWWTranscoder.SevenBitClean(s, Encoding.UTF8) || (s.IndexOf("=?") > -1))
                            {
                                string[] textArray1 = new string[] { "=?", str, "?Q?", WWWTranscoder.QPEncode(s, Encoding.UTF8), "?=" };
                                s = string.Concat(textArray1);
                            }
                            byte[] buffer8 = Encoding.UTF8.GetBytes(s);
                            stream.Write(buffer8, 0, buffer8.Length);
                            stream.Write(buffer5, 0, buffer5.Length);
                            if (this.fileNames[i] != null)
                            {
                                string str3 = this.fileNames[i];
                                if (!WWWTranscoder.SevenBitClean(str3, Encoding.UTF8) || (str3.IndexOf("=?") > -1))
                                {
                                    string[] textArray2 = new string[] { "=?", str, "?Q?", WWWTranscoder.QPEncode(str3, Encoding.UTF8), "?=" };
                                    str3 = string.Concat(textArray2);
                                }
                                byte[] buffer9 = Encoding.UTF8.GetBytes(str3);
                                stream.Write(buffer6, 0, buffer6.Length);
                                stream.Write(buffer9, 0, buffer9.Length);
                                stream.Write(buffer5, 0, buffer5.Length);
                            }
                            stream.Write(buffer2, 0, buffer2.Length);
                            stream.Write(buffer2, 0, buffer2.Length);
                            byte[] buffer10 = this.formData[i];
                            stream.Write(buffer10, 0, buffer10.Length);
                        }
                        stream.Write(buffer2, 0, buffer2.Length);
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Write(this.boundary, 0, this.boundary.Length);
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Write(buffer2, 0, buffer2.Length);
                        return stream.ToArray();
                    }
                }
                byte[] bytes = WWW.DefaultEncoding.GetBytes("&");
                byte[] buffer13 = WWW.DefaultEncoding.GetBytes("=");
                using (MemoryStream stream2 = new MemoryStream(0x400))
                {
                    for (int j = 0; j < this.formData.Count; j++)
                    {
                        byte[] buffer14 = WWWTranscoder.URLEncode(Encoding.UTF8.GetBytes(this.fieldNames[j]));
                        byte[] toEncode = this.formData[j];
                        byte[] buffer16 = WWWTranscoder.URLEncode(toEncode);
                        if (j > 0)
                        {
                            stream2.Write(bytes, 0, bytes.Length);
                        }
                        stream2.Write(buffer14, 0, buffer14.Length);
                        stream2.Write(buffer13, 0, buffer13.Length);
                        stream2.Write(buffer16, 0, buffer16.Length);
                    }
                    return stream2.ToArray();
                }
            }
        }

        /// <summary>
        /// <para>(Read Only) Returns the correct request headers for posting the form using the WWW class.</para>
        /// </summary>
        public Dictionary<string, string> headers
        {
            get
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                if (this.containsFiles)
                {
                    dictionary["Content-Type"] = "multipart/form-data; boundary=\"" + Encoding.UTF8.GetString(this.boundary, 0, this.boundary.Length) + "\"";
                    return dictionary;
                }
                dictionary["Content-Type"] = "application/x-www-form-urlencoded";
                return dictionary;
            }
        }
    }
}

