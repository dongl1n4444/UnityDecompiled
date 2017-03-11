namespace UnityEngine.Analytics
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential)]
    internal sealed class UnityAnalyticsHandler : IDisposable
    {
        [NonSerialized]
        internal IntPtr m_Ptr;
        public UnityAnalyticsHandler()
        {
            this.InternalCreate();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void InternalCreate();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        internal extern void InternalDestroy();
        ~UnityAnalyticsHandler()
        {
            this.InternalDestroy();
        }

        public void Dispose()
        {
            this.InternalDestroy();
            GC.SuppressFinalize(this);
        }

        public static bool limitUserTracking { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
        public static bool deviceStatsEnabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern AnalyticsResult FlushEvents();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern AnalyticsResult SetUserId(string userId);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern AnalyticsResult SetUserGender(Gender gender);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern AnalyticsResult SetUserBirthYear(int birthYear);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern AnalyticsResult Transaction(string productId, double amount, string currency);
        public AnalyticsResult Transaction(string productId, double amount, string currency, string receiptPurchaseData, string signature) => 
            this.Transaction(productId, amount, currency, receiptPurchaseData, signature, false);

        internal AnalyticsResult Transaction(string productId, double amount, string currency, string receiptPurchaseData, string signature, bool usingIAPService)
        {
            if (receiptPurchaseData == null)
            {
                receiptPurchaseData = string.Empty;
            }
            if (signature == null)
            {
                signature = string.Empty;
            }
            return this.InternalTransaction(productId, amount, currency, receiptPurchaseData, signature, usingIAPService);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern AnalyticsResult InternalTransaction(string productId, double amount, string currency, string receiptPurchaseData, string signature, bool usingIAPService);
        public AnalyticsResult CustomEvent(string customEventName) => 
            this.SendCustomEventName(customEventName);

        public AnalyticsResult CustomEvent(CustomEventData eventData) => 
            this.SendCustomEvent(eventData);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern AnalyticsResult SendCustomEventName(string customEventName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern AnalyticsResult SendCustomEvent(CustomEventData eventData);
    }
}

