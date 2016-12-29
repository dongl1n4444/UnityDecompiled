namespace UnityEngine.Analytics
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal sealed class UnityAnalyticsHandler : IDisposable
    {
        [NonSerialized]
        internal IntPtr m_Ptr;
        public UnityAnalyticsHandler()
        {
            this.InternalCreate();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void InternalCreate();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
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

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern AnalyticsResult FlushEvents();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern AnalyticsResult SetUserId(string userId);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern AnalyticsResult SetUserGender(Gender gender);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern AnalyticsResult SetUserBirthYear(int birthYear);
        [MethodImpl(MethodImplOptions.InternalCall)]
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

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern AnalyticsResult InternalTransaction(string productId, double amount, string currency, string receiptPurchaseData, string signature, bool usingIAPService);
        public AnalyticsResult CustomEvent(string customEventName) => 
            this.SendCustomEventName(customEventName);

        public AnalyticsResult CustomEvent(CustomEventData eventData) => 
            this.SendCustomEvent(eventData);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern AnalyticsResult SendCustomEventName(string customEventName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern AnalyticsResult SendCustomEvent(CustomEventData eventData);
    }
}

