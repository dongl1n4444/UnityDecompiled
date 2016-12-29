namespace UnityEngine.Networking
{
    using System;
    using System.Text;

    /// <summary>
    /// <para>A helper object for form sections containing generic, non-file data.</para>
    /// </summary>
    public class MultipartFormDataSection : IMultipartFormSection
    {
        private string content;
        private byte[] data;
        private string name;

        /// <summary>
        /// <para>Raw data section, unnamed and no Content-Type header.</para>
        /// </summary>
        /// <param name="data">Data payload of this section.</param>
        public MultipartFormDataSection(byte[] data) : this(null, data)
        {
        }

        /// <summary>
        /// <para>An anonymous raw data section whose payload is derived from a UTF8 string, with a default Content-Type.</para>
        /// </summary>
        /// <param name="data">String data payload for this section.</param>
        public MultipartFormDataSection(string data) : this(null, data)
        {
        }

        /// <summary>
        /// <para>Raw data section with a section name, no Content-Type header.</para>
        /// </summary>
        /// <param name="name">Section name.</param>
        /// <param name="data">Data payload of this section.</param>
        public MultipartFormDataSection(string name, byte[] data) : this(name, data, null)
        {
        }

        /// <summary>
        /// <para>A names raw data section whose payload is derived from a UTF8 string, with a default Content-Type.</para>
        /// </summary>
        /// <param name="name">Section name.</param>
        /// <param name="data">String data payload for this section.</param>
        public MultipartFormDataSection(string name, string data) : this(name, data, "text/plain")
        {
        }

        /// <summary>
        /// <para>A raw data section with a section name and a Content-Type header.</para>
        /// </summary>
        /// <param name="name">Section name.</param>
        /// <param name="data">Data payload of this section.</param>
        /// <param name="contentType">The value for this section's Content-Type header.</param>
        public MultipartFormDataSection(string name, byte[] data, string contentType)
        {
            if ((data == null) || (data.Length < 1))
            {
                throw new ArgumentException("Cannot create a multipart form data section without body data");
            }
            this.name = name;
            this.data = data;
            this.content = contentType;
        }

        /// <summary>
        /// <para>A named raw data section whose payload is derived from a UTF8 string, with a Content-Type header.</para>
        /// </summary>
        /// <param name="name">Section name.</param>
        /// <param name="data">String data payload for this section.</param>
        /// <param name="contentType">C.</param>
        public MultipartFormDataSection(string name, string data, string contentType) : this(name, data, Encoding.UTF8, contentType)
        {
        }

        /// <summary>
        /// <para>A named raw data section whose payload is derived from a string, with a Content-Type header.</para>
        /// </summary>
        /// <param name="name">Section name.</param>
        /// <param name="data">String data payload for this section.</param>
        /// <param name="contentType">The value for this section's Content-Type header.</param>
        /// <param name="encoding">An encoding to marshal data to or from raw bytes.</param>
        public MultipartFormDataSection(string name, string data, Encoding encoding, string contentType)
        {
            if ((data == null) || (data.Length < 1))
            {
                throw new ArgumentException("Cannot create a multipart form data section without body data");
            }
            byte[] bytes = encoding.GetBytes(data);
            this.name = name;
            this.data = bytes;
            if ((contentType != null) && !contentType.Contains("encoding="))
            {
                contentType = contentType.Trim() + "; encoding=" + encoding.WebName;
            }
            this.content = contentType;
        }

        /// <summary>
        /// <para>Returns the value to use in this section's Content-Type header.</para>
        /// </summary>
        /// <returns>
        /// <para>The Content-Type header for this section, or null.</para>
        /// </returns>
        public string contentType =>
            this.content;

        /// <summary>
        /// <para>Returns a string denoting the desired filename of this section on the destination server.</para>
        /// </summary>
        /// <returns>
        /// <para>The desired file name of this section, or null if this is not a file section.</para>
        /// </returns>
        public string fileName =>
            null;

        /// <summary>
        /// <para>Returns the raw binary data contained in this section. Will not return null or a zero-length array.</para>
        /// </summary>
        /// <returns>
        /// <para>The raw binary data contained in this section. Will not be null or empty.</para>
        /// </returns>
        public byte[] sectionData =>
            this.data;

        /// <summary>
        /// <para>Returns the name of this section, if any.</para>
        /// </summary>
        /// <returns>
        /// <para>The section's name, or null.</para>
        /// </returns>
        public string sectionName =>
            this.name;
    }
}

