namespace UnityEngine.Analytics
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>Unity Analytics provides insight into your game users e.g. DAU, MAU.</para>
    /// </summary>
    public static class Analytics
    {
        private static UnityAnalyticsHandler s_UnityAnalyticsHandler;

        /// <summary>
        /// <para>Custom Events (optional).</para>
        /// </summary>
        /// <param name="customEventName"></param>
        public static AnalyticsResult CustomEvent(string customEventName)
        {
            if (string.IsNullOrEmpty(customEventName))
            {
                throw new ArgumentException("Cannot set custom event name to an empty or null string");
            }
            return GetUnityAnalyticsHandler()?.CustomEvent(customEventName);
        }

        public static AnalyticsResult CustomEvent(string customEventName, IDictionary<string, object> eventData)
        {
            if (string.IsNullOrEmpty(customEventName))
            {
                throw new ArgumentException("Cannot set custom event name to an empty or null string");
            }
            UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
            if (unityAnalyticsHandler == null)
            {
                return AnalyticsResult.NotInitialized;
            }
            if (eventData == null)
            {
                return unityAnalyticsHandler.CustomEvent(customEventName);
            }
            CustomEventData data = new CustomEventData(customEventName);
            data.Add(eventData);
            return unityAnalyticsHandler.CustomEvent(data);
        }

        /// <summary>
        /// <para>Custom Events (optional).</para>
        /// </summary>
        /// <param name="customEventName"></param>
        /// <param name="position"></param>
        public static AnalyticsResult CustomEvent(string customEventName, Vector3 position)
        {
            if (string.IsNullOrEmpty(customEventName))
            {
                throw new ArgumentException("Cannot set custom event name to an empty or null string");
            }
            UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
            if (unityAnalyticsHandler == null)
            {
                return AnalyticsResult.NotInitialized;
            }
            CustomEventData eventData = new CustomEventData(customEventName);
            eventData.Add("x", (double) Convert.ToDecimal(position.x));
            eventData.Add("y", (double) Convert.ToDecimal(position.y));
            eventData.Add("z", (double) Convert.ToDecimal(position.z));
            return unityAnalyticsHandler.CustomEvent(eventData);
        }

        /// <summary>
        /// <para>Attempts to flush immediately all queued analytics events to the network and filesystem cache if possible (optional).</para>
        /// </summary>
        public static AnalyticsResult FlushEvents() => 
            GetUnityAnalyticsHandler()?.FlushEvents();

        internal static UnityAnalyticsHandler GetUnityAnalyticsHandler()
        {
            if (s_UnityAnalyticsHandler == null)
            {
                s_UnityAnalyticsHandler = new UnityAnalyticsHandler();
            }
            return s_UnityAnalyticsHandler;
        }

        /// <summary>
        /// <para>User Demographics (optional).</para>
        /// </summary>
        /// <param name="birthYear">Birth year of user. Must be 4-digit year format, only.</param>
        public static AnalyticsResult SetUserBirthYear(int birthYear)
        {
            UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
            if (s_UnityAnalyticsHandler == null)
            {
                return AnalyticsResult.NotInitialized;
            }
            return unityAnalyticsHandler.SetUserBirthYear(birthYear);
        }

        /// <summary>
        /// <para>User Demographics (optional).</para>
        /// </summary>
        /// <param name="gender">Gender of user can be "Female", "Male", or "Unknown".</param>
        public static AnalyticsResult SetUserGender(Gender gender) => 
            GetUnityAnalyticsHandler()?.SetUserGender(gender);

        /// <summary>
        /// <para>User Demographics (optional).</para>
        /// </summary>
        /// <param name="userId">User id.</param>
        public static AnalyticsResult SetUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Cannot set userId to an empty or null string");
            }
            return GetUnityAnalyticsHandler()?.SetUserId(userId);
        }

        /// <summary>
        /// <para>Tracking Monetization (optional).</para>
        /// </summary>
        /// <param name="productId">The id of the purchased item.</param>
        /// <param name="amount">The price of the item.</param>
        /// <param name="currency">Abbreviation of the currency used for the transaction. For example “USD” (United States Dollars). See http:en.wikipedia.orgwikiISO_4217 for a standardized list of currency abbreviations.</param>
        /// <param name="receiptPurchaseData">Receipt data (iOS)  receipt ID (android)  for in-app purchases to verify purchases with Apple iTunes / Google Play. Use null in the absence of receipts.</param>
        /// <param name="signature">Android receipt signature. If using native Android use the INAPP_DATA_SIGNATURE string containing the signature of the purchase data that was signed with the private key of the developer. The data signature uses the RSASSA-PKCS1-v1_5 scheme. Pass in null in absence of a signature.</param>
        public static AnalyticsResult Transaction(string productId, decimal amount, string currency) => 
            GetUnityAnalyticsHandler()?.Transaction(productId, Convert.ToDouble(amount), currency, null, null);

        /// <summary>
        /// <para>Tracking Monetization (optional).</para>
        /// </summary>
        /// <param name="productId">The id of the purchased item.</param>
        /// <param name="amount">The price of the item.</param>
        /// <param name="currency">Abbreviation of the currency used for the transaction. For example “USD” (United States Dollars). See http:en.wikipedia.orgwikiISO_4217 for a standardized list of currency abbreviations.</param>
        /// <param name="receiptPurchaseData">Receipt data (iOS)  receipt ID (android)  for in-app purchases to verify purchases with Apple iTunes / Google Play. Use null in the absence of receipts.</param>
        /// <param name="signature">Android receipt signature. If using native Android use the INAPP_DATA_SIGNATURE string containing the signature of the purchase data that was signed with the private key of the developer. The data signature uses the RSASSA-PKCS1-v1_5 scheme. Pass in null in absence of a signature.</param>
        public static AnalyticsResult Transaction(string productId, decimal amount, string currency, string receiptPurchaseData, string signature) => 
            GetUnityAnalyticsHandler()?.Transaction(productId, Convert.ToDouble(amount), currency, receiptPurchaseData, signature);

        internal static AnalyticsResult Transaction(string productId, decimal amount, string currency, string receiptPurchaseData, string signature, bool usingIAPService) => 
            GetUnityAnalyticsHandler()?.Transaction(productId, Convert.ToDouble(amount), currency, receiptPurchaseData, signature, usingIAPService);
    }
}

