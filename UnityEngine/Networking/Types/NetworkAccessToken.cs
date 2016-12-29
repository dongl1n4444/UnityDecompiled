namespace UnityEngine.Networking.Types
{
    using System;

    /// <summary>
    /// <para>Access token used to authenticate a client session for the purposes of allowing or disallowing match operations requested by that client.</para>
    /// </summary>
    public class NetworkAccessToken
    {
        /// <summary>
        /// <para>Binary field for the actual token.</para>
        /// </summary>
        public byte[] array;
        private const int NETWORK_ACCESS_TOKEN_SIZE = 0x40;

        public NetworkAccessToken()
        {
            this.array = new byte[0x40];
        }

        public NetworkAccessToken(byte[] array)
        {
            this.array = array;
        }

        public NetworkAccessToken(string strArray)
        {
            try
            {
                this.array = Convert.FromBase64String(strArray);
            }
            catch (Exception)
            {
                this.array = new byte[0x40];
            }
        }

        /// <summary>
        /// <para>Accessor to get an encoded string from the m_array data.</para>
        /// </summary>
        public string GetByteString() => 
            Convert.ToBase64String(this.array);

        /// <summary>
        /// <para>Checks if the token is a valid set of data with respect to default values (returns true if the values are not default, does not validate the token is a current legitimate token with respect to the server's auth framework).</para>
        /// </summary>
        public bool IsValid()
        {
            if ((this.array == null) || (this.array.Length != 0x40))
            {
                return false;
            }
            foreach (byte num in this.array)
            {
                if (num != 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

