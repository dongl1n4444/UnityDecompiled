namespace UnityEngine.Purchasing
{
    using System;
    using System.IO;
    using System.Text;
    using UnityEngine;

    internal class TransactionLog
    {
        private readonly ILogger logger;
        private readonly string persistentDataPath;

        public TransactionLog(ILogger logger, string persistentDataPath)
        {
            this.logger = logger;
            if (!string.IsNullOrEmpty(persistentDataPath))
            {
                this.persistentDataPath = Path.Combine(Path.Combine(persistentDataPath, "Unity"), "UnityPurchasing");
            }
        }

        public void Clear()
        {
            Directory.Delete(this.persistentDataPath, true);
        }

        internal static string ComputeHash(string transactionID)
        {
            ulong num = 0x2aaaaaaaaaaaab67L;
            for (int i = 0; i < transactionID.Length; i++)
            {
                num += transactionID[i];
                num *= (ulong) 0x2aaaaaaaaaaaab6fL;
            }
            StringBuilder builder = new StringBuilder(0x10);
            foreach (byte num3 in BitConverter.GetBytes(num))
            {
                builder.AppendFormat("{0:X2}", num3);
            }
            return builder.ToString();
        }

        private string GetRecordPath(string transactionID)
        {
            return Path.Combine(this.persistentDataPath, ComputeHash(transactionID));
        }

        public bool HasRecordOf(string transactionID)
        {
            if (string.IsNullOrEmpty(transactionID) || string.IsNullOrEmpty(this.persistentDataPath))
            {
                return false;
            }
            return Directory.Exists(this.GetRecordPath(transactionID));
        }

        public void Record(string transactionID)
        {
            if (!string.IsNullOrEmpty(transactionID) && !string.IsNullOrEmpty(this.persistentDataPath))
            {
                string recordPath = this.GetRecordPath(transactionID);
                try
                {
                    Directory.CreateDirectory(recordPath);
                }
                catch (Exception exception)
                {
                    this.logger.Log(exception.Message);
                }
            }
        }
    }
}

