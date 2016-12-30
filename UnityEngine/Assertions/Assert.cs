namespace UnityEngine.Assertions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using UnityEngine;
    using UnityEngine.Assertions.Comparers;

    /// <summary>
    /// <para>The Assert class contains assertion methods for setting invariants in the code.</para>
    /// </summary>
    [DebuggerStepThrough]
    public static class Assert
    {
        /// <summary>
        /// <para>Should an exception be thrown on a failure.</para>
        /// </summary>
        public static bool raiseExceptions;
        internal const string UNITY_ASSERTIONS = "UNITY_ASSERTIONS";

        /// <summary>
        /// <para>Asserts that the values are approximately equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.
        /// 
        /// Note: Every time you call the method with tolerance specified, a new instance of Assertions.Comparers.FloatComparer is created. For performance reasons you might want to instance your own comparer and pass it to the AreEqual method. If the tolerance is not specifies, a default comparer is used and the issue does not occur.</para>
        /// </summary>
        /// <param name="tolerance">Tolerance of approximation.</param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void AreApproximatelyEqual(float expected, float actual)
        {
            AreEqual<float>(expected, actual, null, FloatComparer.s_ComparerWithDefaultTolerance);
        }

        /// <summary>
        /// <para>Asserts that the values are approximately equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.
        /// 
        /// Note: Every time you call the method with tolerance specified, a new instance of Assertions.Comparers.FloatComparer is created. For performance reasons you might want to instance your own comparer and pass it to the AreEqual method. If the tolerance is not specifies, a default comparer is used and the issue does not occur.</para>
        /// </summary>
        /// <param name="tolerance">Tolerance of approximation.</param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void AreApproximatelyEqual(float expected, float actual, float tolerance)
        {
            AreApproximatelyEqual(expected, actual, tolerance, null);
        }

        /// <summary>
        /// <para>Asserts that the values are approximately equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.
        /// 
        /// Note: Every time you call the method with tolerance specified, a new instance of Assertions.Comparers.FloatComparer is created. For performance reasons you might want to instance your own comparer and pass it to the AreEqual method. If the tolerance is not specifies, a default comparer is used and the issue does not occur.</para>
        /// </summary>
        /// <param name="tolerance">Tolerance of approximation.</param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void AreApproximatelyEqual(float expected, float actual, string message)
        {
            AreEqual<float>(expected, actual, message, FloatComparer.s_ComparerWithDefaultTolerance);
        }

        /// <summary>
        /// <para>Asserts that the values are approximately equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.
        /// 
        /// Note: Every time you call the method with tolerance specified, a new instance of Assertions.Comparers.FloatComparer is created. For performance reasons you might want to instance your own comparer and pass it to the AreEqual method. If the tolerance is not specifies, a default comparer is used and the issue does not occur.</para>
        /// </summary>
        /// <param name="tolerance">Tolerance of approximation.</param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void AreApproximatelyEqual(float expected, float actual, float tolerance, string message)
        {
            AreEqual<float>(expected, actual, message, new FloatComparer(tolerance));
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreEqual<T>(T expected, T actual)
        {
            AreEqual<T>(expected, actual, null);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreEqual(UnityEngine.Object expected, UnityEngine.Object actual, string message)
        {
            if (actual != expected)
            {
                Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, true), message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreEqual<T>(T expected, T actual, string message)
        {
            AreEqual<T>(expected, actual, message, EqualityComparer<T>.Default);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreEqual<T>(T expected, T actual, string message, IEqualityComparer<T> comparer)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
            {
                AreEqual(expected as UnityEngine.Object, actual as UnityEngine.Object, message);
            }
            else if (!comparer.Equals(actual, expected))
            {
                Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, true), message);
            }
        }

        /// <summary>
        /// <para>Asserts that the values are approximately not equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.</para>
        /// </summary>
        /// <param name="tolerance">Tolerance of approximation.</param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotApproximatelyEqual(float expected, float actual)
        {
            AreNotEqual<float>(expected, actual, null, FloatComparer.s_ComparerWithDefaultTolerance);
        }

        /// <summary>
        /// <para>Asserts that the values are approximately not equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.</para>
        /// </summary>
        /// <param name="tolerance">Tolerance of approximation.</param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotApproximatelyEqual(float expected, float actual, float tolerance)
        {
            AreNotApproximatelyEqual(expected, actual, tolerance, null);
        }

        /// <summary>
        /// <para>Asserts that the values are approximately not equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.</para>
        /// </summary>
        /// <param name="tolerance">Tolerance of approximation.</param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotApproximatelyEqual(float expected, float actual, string message)
        {
            AreNotEqual<float>(expected, actual, message, FloatComparer.s_ComparerWithDefaultTolerance);
        }

        /// <summary>
        /// <para>Asserts that the values are approximately not equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.</para>
        /// </summary>
        /// <param name="tolerance">Tolerance of approximation.</param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotApproximatelyEqual(float expected, float actual, float tolerance, string message)
        {
            AreNotEqual<float>(expected, actual, message, new FloatComparer(tolerance));
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotEqual<T>(T expected, T actual)
        {
            AreNotEqual<T>(expected, actual, null);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotEqual(UnityEngine.Object expected, UnityEngine.Object actual, string message)
        {
            if (actual == expected)
            {
                Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, false), message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotEqual<T>(T expected, T actual, string message)
        {
            AreNotEqual<T>(expected, actual, message, EqualityComparer<T>.Default);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AreNotEqual<T>(T expected, T actual, string message, IEqualityComparer<T> comparer)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
            {
                AreNotEqual(expected as UnityEngine.Object, actual as UnityEngine.Object, message);
            }
            else if (comparer.Equals(actual, expected))
            {
                Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, false), message);
            }
        }

        [Obsolete("Assert.Equals should not be used for Assertions", true), EditorBrowsable(EditorBrowsableState.Never)]
        public static bool Equals(object obj1, object obj2)
        {
            throw new InvalidOperationException("Assert.Equals should not be used for Assertions");
        }

        private static void Fail(string message, string userMessage)
        {
            if (Debugger.IsAttached)
            {
                throw new AssertionException(message, userMessage);
            }
            if (raiseExceptions)
            {
                throw new AssertionException(message, userMessage);
            }
            if (message == null)
            {
                message = "Assertion has failed\n";
            }
            if (userMessage != null)
            {
                message = userMessage + '\n' + message;
            }
            UnityEngine.Debug.LogAssertion(message);
        }

        /// <summary>
        /// <para>Asserts that the condition is false.</para>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse(bool condition)
        {
            IsFalse(condition, null);
        }

        /// <summary>
        /// <para>Asserts that the condition is false.</para>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse(bool condition, string message)
        {
            if (condition)
            {
                Fail(AssertionMessageUtil.BooleanFailureMessage(false), message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>(T value) where T: class
        {
            IsNotNull<T>(value, null);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull(UnityEngine.Object value, string message)
        {
            if (value == null)
            {
                Fail(AssertionMessageUtil.NullFailureMessage(value, false), message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>(T value, string message) where T: class
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
            {
                IsNotNull(value as UnityEngine.Object, message);
            }
            else if (value == null)
            {
                Fail(AssertionMessageUtil.NullFailureMessage(value, false), message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T value) where T: class
        {
            IsNull<T>(value, null);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull(UnityEngine.Object value, string message)
        {
            if (value != null)
            {
                Fail(AssertionMessageUtil.NullFailureMessage(value, true), message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T value, string message) where T: class
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
            {
                IsNull(value as UnityEngine.Object, message);
            }
            else if (value != null)
            {
                Fail(AssertionMessageUtil.NullFailureMessage(value, true), message);
            }
        }

        /// <summary>
        /// <para>Asserts that the condition is true.</para>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue(bool condition)
        {
            IsTrue(condition, null);
        }

        /// <summary>
        /// <para>Asserts that the condition is true.</para>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue(bool condition, string message)
        {
            if (!condition)
            {
                Fail(AssertionMessageUtil.BooleanFailureMessage(true), message);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Assert.ReferenceEquals should not be used for Assertions", true)]
        public static bool ReferenceEquals(object obj1, object obj2)
        {
            throw new InvalidOperationException("Assert.ReferenceEquals should not be used for Assertions");
        }
    }
}

