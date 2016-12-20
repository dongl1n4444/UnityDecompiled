namespace UnityEngine
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Webplayer security related class. Not supported from 5.4.0 onwards.</para>
    /// </summary>
    public sealed class Security
    {
        private static readonly string kSignatureExtension = ".signature";

        /// <summary>
        /// <para>Loads an assembly and checks that it is allowed to be used in the webplayer. (Web Player is no Longer Supported).</para>
        /// </summary>
        /// <param name="assemblyData">Assembly to verify.</param>
        /// <param name="authorizationKey">Public key used to verify assembly.</param>
        /// <returns>
        /// <para>Loaded, verified, assembly, or null if the assembly cannot be verfied.</para>
        /// </returns>
        [Obsolete("This was an internal method which is no longer used", true)]
        public static Assembly LoadAndVerifyAssembly(byte[] assemblyData)
        {
            return null;
        }

        /// <summary>
        /// <para>Loads an assembly and checks that it is allowed to be used in the webplayer. (Web Player is no Longer Supported).</para>
        /// </summary>
        /// <param name="assemblyData">Assembly to verify.</param>
        /// <param name="authorizationKey">Public key used to verify assembly.</param>
        /// <returns>
        /// <para>Loaded, verified, assembly, or null if the assembly cannot be verfied.</para>
        /// </returns>
        [Obsolete("This was an internal method which is no longer used", true)]
        public static Assembly LoadAndVerifyAssembly(byte[] assemblyData, string authorizationKey)
        {
            return null;
        }

        /// <summary>
        /// <para>Prefetch the webplayer socket security policy from a non-default port number.</para>
        /// </summary>
        /// <param name="ip">IP address of server.</param>
        /// <param name="atPort">Port from where socket policy is read.</param>
        /// <param name="timeout">Time to wait for response.</param>
        [Obsolete("Security.PrefetchSocketPolicy is no longer supported, since the Unity Web Player is no longer supported by Unity."), ExcludeFromDocs]
        public static bool PrefetchSocketPolicy(string ip, int atPort)
        {
            int timeout = 0xbb8;
            return PrefetchSocketPolicy(ip, atPort, timeout);
        }

        /// <summary>
        /// <para>Prefetch the webplayer socket security policy from a non-default port number.</para>
        /// </summary>
        /// <param name="ip">IP address of server.</param>
        /// <param name="atPort">Port from where socket policy is read.</param>
        /// <param name="timeout">Time to wait for response.</param>
        [Obsolete("Security.PrefetchSocketPolicy is no longer supported, since the Unity Web Player is no longer supported by Unity.")]
        public static bool PrefetchSocketPolicy(string ip, int atPort, [DefaultValue("3000")] int timeout)
        {
            return false;
        }

        [RequiredByNativeCode]
        internal static bool VerifySignature(string file, byte[] publicKey)
        {
            try
            {
                string path = file + kSignatureExtension;
                if (!File.Exists(path))
                {
                    return false;
                }
                using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
                {
                    provider.ImportCspBlob(publicKey);
                    using (SHA1CryptoServiceProvider provider2 = new SHA1CryptoServiceProvider())
                    {
                        return provider.VerifyData(File.ReadAllBytes(file), provider2, File.ReadAllBytes(path));
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            return false;
        }
    }
}

