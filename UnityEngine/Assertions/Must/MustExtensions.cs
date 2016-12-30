namespace UnityEngine.Assertions.Must
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine.Assertions;

    /// <summary>
    /// <para>An extension class that serves as a wrapper for the Assert class.</para>
    /// </summary>
    [Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead"), DebuggerStepThrough]
    public static class MustExtensions
    {
        /// <summary>
        /// <para>An extension method for Assertions.Assert.AreApproximatelyEqual.</para>
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="message"></param>
        /// <param name="tolerance"></param>
        [Conditional("UNITY_ASSERTIONS"), Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead")]
        public static void MustBeApproximatelyEqual(this float actual, float expected)
        {
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(actual, expected);
        }

        /// <summary>
        /// <para>An extension method for Assertions.Assert.AreApproximatelyEqual.</para>
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="message"></param>
        /// <param name="tolerance"></param>
        [Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead"), Conditional("UNITY_ASSERTIONS")]
        public static void MustBeApproximatelyEqual(this float actual, float expected, float tolerance)
        {
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(actual, expected, tolerance);
        }

        /// <summary>
        /// <para>An extension method for Assertions.Assert.AreApproximatelyEqual.</para>
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="message"></param>
        /// <param name="tolerance"></param>
        [Conditional("UNITY_ASSERTIONS"), Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead")]
        public static void MustBeApproximatelyEqual(this float actual, float expected, string message)
        {
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(actual, expected, message);
        }

        /// <summary>
        /// <para>An extension method for Assertions.Assert.AreApproximatelyEqual.</para>
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="message"></param>
        /// <param name="tolerance"></param>
        [Conditional("UNITY_ASSERTIONS"), Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead")]
        public static void MustBeApproximatelyEqual(this float actual, float expected, float tolerance, string message)
        {
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(expected, actual, tolerance, message);
        }

        [Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead"), Conditional("UNITY_ASSERTIONS")]
        public static void MustBeEqual<T>(this T actual, T expected)
        {
            UnityEngine.Assertions.Assert.AreEqual<T>(actual, expected);
        }

        [Conditional("UNITY_ASSERTIONS"), Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead")]
        public static void MustBeEqual<T>(this T actual, T expected, string message)
        {
            UnityEngine.Assertions.Assert.AreEqual<T>(expected, actual, message);
        }

        /// <summary>
        /// <para>An extension method for Assertions.Assert.IsFalse.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        [Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead"), Conditional("UNITY_ASSERTIONS")]
        public static void MustBeFalse(this bool value)
        {
            UnityEngine.Assertions.Assert.IsFalse(value);
        }

        /// <summary>
        /// <para>An extension method for Assertions.Assert.IsFalse.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        [Conditional("UNITY_ASSERTIONS"), Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead")]
        public static void MustBeFalse(this bool value, string message)
        {
            UnityEngine.Assertions.Assert.IsFalse(value, message);
        }

        [Conditional("UNITY_ASSERTIONS"), Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead")]
        public static void MustBeNull<T>(this T expected) where T: class
        {
            UnityEngine.Assertions.Assert.IsNull<T>(expected);
        }

        [Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead"), Conditional("UNITY_ASSERTIONS")]
        public static void MustBeNull<T>(this T expected, string message) where T: class
        {
            UnityEngine.Assertions.Assert.IsNull<T>(expected, message);
        }

        /// <summary>
        /// <para>An extension method for Assertions.Assert.IsTrue.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        [Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead"), Conditional("UNITY_ASSERTIONS")]
        public static void MustBeTrue(this bool value)
        {
            UnityEngine.Assertions.Assert.IsTrue(value);
        }

        /// <summary>
        /// <para>An extension method for Assertions.Assert.IsTrue.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        [Conditional("UNITY_ASSERTIONS"), Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead")]
        public static void MustBeTrue(this bool value, string message)
        {
            UnityEngine.Assertions.Assert.IsTrue(value, message);
        }

        /// <summary>
        /// <para>An extension method for Assertions.Assert.AreNotApproximatelyEqual.</para>
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="message"></param>
        /// <param name="tolerance"></param>
        [Conditional("UNITY_ASSERTIONS"), Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead")]
        public static void MustNotBeApproximatelyEqual(this float actual, float expected)
        {
            UnityEngine.Assertions.Assert.AreNotApproximatelyEqual(expected, actual);
        }

        /// <summary>
        /// <para>An extension method for Assertions.Assert.AreNotApproximatelyEqual.</para>
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="message"></param>
        /// <param name="tolerance"></param>
        [Conditional("UNITY_ASSERTIONS"), Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead")]
        public static void MustNotBeApproximatelyEqual(this float actual, float expected, float tolerance)
        {
            UnityEngine.Assertions.Assert.AreNotApproximatelyEqual(expected, actual, tolerance);
        }

        /// <summary>
        /// <para>An extension method for Assertions.Assert.AreNotApproximatelyEqual.</para>
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="message"></param>
        /// <param name="tolerance"></param>
        [Conditional("UNITY_ASSERTIONS"), Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead")]
        public static void MustNotBeApproximatelyEqual(this float actual, float expected, string message)
        {
            UnityEngine.Assertions.Assert.AreNotApproximatelyEqual(expected, actual, message);
        }

        /// <summary>
        /// <para>An extension method for Assertions.Assert.AreNotApproximatelyEqual.</para>
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="message"></param>
        /// <param name="tolerance"></param>
        [Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead"), Conditional("UNITY_ASSERTIONS")]
        public static void MustNotBeApproximatelyEqual(this float actual, float expected, float tolerance, string message)
        {
            UnityEngine.Assertions.Assert.AreNotApproximatelyEqual(expected, actual, tolerance, message);
        }

        [Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead"), Conditional("UNITY_ASSERTIONS")]
        public static void MustNotBeEqual<T>(this T actual, T expected)
        {
            UnityEngine.Assertions.Assert.AreNotEqual<T>(actual, expected);
        }

        [Conditional("UNITY_ASSERTIONS"), Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead")]
        public static void MustNotBeEqual<T>(this T actual, T expected, string message)
        {
            UnityEngine.Assertions.Assert.AreNotEqual<T>(expected, actual, message);
        }

        [Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead"), Conditional("UNITY_ASSERTIONS")]
        public static void MustNotBeNull<T>(this T expected) where T: class
        {
            UnityEngine.Assertions.Assert.IsNotNull<T>(expected);
        }

        [Conditional("UNITY_ASSERTIONS"), Obsolete("Must extensions are deprecated. Use UnityEngine.Assertions.Assert instead")]
        public static void MustNotBeNull<T>(this T expected, string message) where T: class
        {
            UnityEngine.Assertions.Assert.IsNotNull<T>(expected, message);
        }
    }
}

