using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	/// <summary>
	///   <para>Class containing methods to ease debugging while developing a game.</para>
	/// </summary>
	public sealed class Debug
	{
		internal static Logger s_Logger = new Logger(new DebugLogHandler());

		/// <summary>
		///   <para>Get default debug logger.</para>
		/// </summary>
		public static ILogger logger
		{
			get
			{
				return Debug.s_Logger;
			}
		}

		/// <summary>
		///   <para>Opens or closes developer console.</para>
		/// </summary>
		public static extern bool developerConsoleVisible
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>In the Build Settings dialog there is a check box called "Development Build".</para>
		/// </summary>
		public static extern bool isDebugBuild
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Draws a line between specified start and end points.</para>
		/// </summary>
		/// <param name="start">Point in world space where the line should start.</param>
		/// <param name="end">Point in world space where the line should end.</param>
		/// <param name="color">Color of the line.</param>
		/// <param name="duration">How long the line should be visible for.</param>
		/// <param name="depthTest">Should the line be obscured by objects closer to the camera?</param>
		public static void DrawLine(Vector3 start, Vector3 end, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
		{
			Debug.INTERNAL_CALL_DrawLine(ref start, ref end, ref color, duration, depthTest);
		}

		/// <summary>
		///   <para>Draws a line between specified start and end points.</para>
		/// </summary>
		/// <param name="start">Point in world space where the line should start.</param>
		/// <param name="end">Point in world space where the line should end.</param>
		/// <param name="color">Color of the line.</param>
		/// <param name="duration">How long the line should be visible for.</param>
		/// <param name="depthTest">Should the line be obscured by objects closer to the camera?</param>
		[ExcludeFromDocs]
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
		{
			bool depthTest = true;
			Debug.INTERNAL_CALL_DrawLine(ref start, ref end, ref color, duration, depthTest);
		}

		/// <summary>
		///   <para>Draws a line between specified start and end points.</para>
		/// </summary>
		/// <param name="start">Point in world space where the line should start.</param>
		/// <param name="end">Point in world space where the line should end.</param>
		/// <param name="color">Color of the line.</param>
		/// <param name="duration">How long the line should be visible for.</param>
		/// <param name="depthTest">Should the line be obscured by objects closer to the camera?</param>
		[ExcludeFromDocs]
		public static void DrawLine(Vector3 start, Vector3 end, Color color)
		{
			bool depthTest = true;
			float duration = 0f;
			Debug.INTERNAL_CALL_DrawLine(ref start, ref end, ref color, duration, depthTest);
		}

		/// <summary>
		///   <para>Draws a line between specified start and end points.</para>
		/// </summary>
		/// <param name="start">Point in world space where the line should start.</param>
		/// <param name="end">Point in world space where the line should end.</param>
		/// <param name="color">Color of the line.</param>
		/// <param name="duration">How long the line should be visible for.</param>
		/// <param name="depthTest">Should the line be obscured by objects closer to the camera?</param>
		[ExcludeFromDocs]
		public static void DrawLine(Vector3 start, Vector3 end)
		{
			bool depthTest = true;
			float duration = 0f;
			Color white = Color.white;
			Debug.INTERNAL_CALL_DrawLine(ref start, ref end, ref white, duration, depthTest);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DrawLine(ref Vector3 start, ref Vector3 end, ref Color color, float duration, bool depthTest);

		/// <summary>
		///   <para>Draws a line from start to start + dir in world coordinates.</para>
		/// </summary>
		/// <param name="start">Point in world space where the ray should start.</param>
		/// <param name="dir">Direction and length of the ray.</param>
		/// <param name="color">Color of the drawn line.</param>
		/// <param name="duration">How long the line will be visible for (in seconds).</param>
		/// <param name="depthTest">Should the line be obscured by other objects closer to the camera?</param>
		[ExcludeFromDocs]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
		{
			bool depthTest = true;
			Debug.DrawRay(start, dir, color, duration, depthTest);
		}

		/// <summary>
		///   <para>Draws a line from start to start + dir in world coordinates.</para>
		/// </summary>
		/// <param name="start">Point in world space where the ray should start.</param>
		/// <param name="dir">Direction and length of the ray.</param>
		/// <param name="color">Color of the drawn line.</param>
		/// <param name="duration">How long the line will be visible for (in seconds).</param>
		/// <param name="depthTest">Should the line be obscured by other objects closer to the camera?</param>
		[ExcludeFromDocs]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color)
		{
			bool depthTest = true;
			float duration = 0f;
			Debug.DrawRay(start, dir, color, duration, depthTest);
		}

		/// <summary>
		///   <para>Draws a line from start to start + dir in world coordinates.</para>
		/// </summary>
		/// <param name="start">Point in world space where the ray should start.</param>
		/// <param name="dir">Direction and length of the ray.</param>
		/// <param name="color">Color of the drawn line.</param>
		/// <param name="duration">How long the line will be visible for (in seconds).</param>
		/// <param name="depthTest">Should the line be obscured by other objects closer to the camera?</param>
		[ExcludeFromDocs]
		public static void DrawRay(Vector3 start, Vector3 dir)
		{
			bool depthTest = true;
			float duration = 0f;
			Color white = Color.white;
			Debug.DrawRay(start, dir, white, duration, depthTest);
		}

		/// <summary>
		///   <para>Draws a line from start to start + dir in world coordinates.</para>
		/// </summary>
		/// <param name="start">Point in world space where the ray should start.</param>
		/// <param name="dir">Direction and length of the ray.</param>
		/// <param name="color">Color of the drawn line.</param>
		/// <param name="duration">How long the line will be visible for (in seconds).</param>
		/// <param name="depthTest">Should the line be obscured by other objects closer to the camera?</param>
		public static void DrawRay(Vector3 start, Vector3 dir, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
		{
			Debug.DrawLine(start, start + dir, color, duration, depthTest);
		}

		/// <summary>
		///   <para>Pauses the editor.</para>
		/// </summary>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Break();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DebugBreak();

		/// <summary>
		///   <para>Logs message to the Unity Console.</para>
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void Log(object message)
		{
			Debug.logger.Log(LogType.Log, message);
		}

		/// <summary>
		///   <para>Logs message to the Unity Console.</para>
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void Log(object message, Object context)
		{
			Debug.logger.Log(LogType.Log, message, context);
		}

		/// <summary>
		///   <para>Logs a formatted message to the Unity Console.</para>
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogFormat(string format, params object[] args)
		{
			Debug.logger.LogFormat(LogType.Log, format, args);
		}

		/// <summary>
		///   <para>Logs a formatted message to the Unity Console.</para>
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogFormat(Object context, string format, params object[] args)
		{
			Debug.logger.LogFormat(LogType.Log, context, format, args);
		}

		/// <summary>
		///   <para>A variant of Debug.Log that logs an error message to the console.</para>
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogError(object message)
		{
			Debug.logger.Log(LogType.Error, message);
		}

		/// <summary>
		///   <para>A variant of Debug.Log that logs an error message to the console.</para>
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogError(object message, Object context)
		{
			Debug.logger.Log(LogType.Error, message, context);
		}

		/// <summary>
		///   <para>Logs a formatted error message to the Unity console.</para>
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogErrorFormat(string format, params object[] args)
		{
			Debug.logger.LogFormat(LogType.Error, format, args);
		}

		/// <summary>
		///   <para>Logs a formatted error message to the Unity console.</para>
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogErrorFormat(Object context, string format, params object[] args)
		{
			Debug.logger.LogFormat(LogType.Error, context, format, args);
		}

		/// <summary>
		///   <para>Clears errors from the developer console.</para>
		/// </summary>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearDeveloperConsole();

		/// <summary>
		///   <para>A variant of Debug.Log that logs an error message to the console.</para>
		/// </summary>
		/// <param name="context">Object to which the message applies.</param>
		/// <param name="exception">Runtime Exception.</param>
		public static void LogException(Exception exception)
		{
			Debug.logger.LogException(exception, null);
		}

		/// <summary>
		///   <para>A variant of Debug.Log that logs an error message to the console.</para>
		/// </summary>
		/// <param name="context">Object to which the message applies.</param>
		/// <param name="exception">Runtime Exception.</param>
		public static void LogException(Exception exception, Object context)
		{
			Debug.logger.LogException(exception, context);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void LogPlayerBuildError(string message, string file, int line, int column);

		/// <summary>
		///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogWarning(object message)
		{
			Debug.logger.Log(LogType.Warning, message);
		}

		/// <summary>
		///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogWarning(object message, Object context)
		{
			Debug.logger.Log(LogType.Warning, message, context);
		}

		/// <summary>
		///   <para>Logs a formatted warning message to the Unity Console.</para>
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogWarningFormat(string format, params object[] args)
		{
			Debug.logger.LogFormat(LogType.Warning, format, args);
		}

		/// <summary>
		///   <para>Logs a formatted warning message to the Unity Console.</para>
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogWarningFormat(Object context, string format, params object[] args)
		{
			Debug.logger.LogFormat(LogType.Warning, context, format, args);
		}

		/// <summary>
		///   <para>Assert a condition and logs a formatted error message to the Unity console on failure.</para>
		/// </summary>
		/// <param name="condition">Condition you expect to be true.</param>
		/// <param name="context">Object to which the message applies.</param>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition)
		{
			if (!condition)
			{
				Debug.logger.Log(LogType.Assert, "Assertion failed");
			}
		}

		/// <summary>
		///   <para>Assert a condition and logs a formatted error message to the Unity console on failure.</para>
		/// </summary>
		/// <param name="condition">Condition you expect to be true.</param>
		/// <param name="context">Object to which the message applies.</param>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, Object context)
		{
			if (!condition)
			{
				Debug.logger.Log(LogType.Assert, "Assertion failed", context);
			}
		}

		/// <summary>
		///   <para>Assert a condition and logs a formatted error message to the Unity console on failure.</para>
		/// </summary>
		/// <param name="condition">Condition you expect to be true.</param>
		/// <param name="context">Object to which the message applies.</param>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, object message)
		{
			if (!condition)
			{
				Debug.logger.Log(LogType.Assert, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, string message)
		{
			if (!condition)
			{
				Debug.logger.Log(LogType.Assert, message);
			}
		}

		/// <summary>
		///   <para>Assert a condition and logs a formatted error message to the Unity console on failure.</para>
		/// </summary>
		/// <param name="condition">Condition you expect to be true.</param>
		/// <param name="context">Object to which the message applies.</param>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, object message, Object context)
		{
			if (!condition)
			{
				Debug.logger.Log(LogType.Assert, message, context);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, string message, Object context)
		{
			if (!condition)
			{
				Debug.logger.Log(LogType.Assert, message, context);
			}
		}

		/// <summary>
		///   <para>Assert a condition and logs a formatted error message to the Unity console on failure.</para>
		/// </summary>
		/// <param name="condition">Condition you expect to be true.</param>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertFormat(bool condition, string format, params object[] args)
		{
			if (!condition)
			{
				Debug.logger.LogFormat(LogType.Assert, format, args);
			}
		}

		/// <summary>
		///   <para>Assert a condition and logs a formatted error message to the Unity console on failure.</para>
		/// </summary>
		/// <param name="condition">Condition you expect to be true.</param>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertFormat(bool condition, Object context, string format, params object[] args)
		{
			if (!condition)
			{
				Debug.logger.LogFormat(LogType.Assert, context, format, args);
			}
		}

		/// <summary>
		///   <para>A variant of Debug.Log that logs an assertion message to the console.</para>
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertion(object message)
		{
			Debug.logger.Log(LogType.Assert, message);
		}

		/// <summary>
		///   <para>A variant of Debug.Log that logs an assertion message to the console.</para>
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertion(object message, Object context)
		{
			Debug.logger.Log(LogType.Assert, message, context);
		}

		/// <summary>
		///   <para>Logs a formatted assertion message to the Unity console.</para>
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertionFormat(string format, params object[] args)
		{
			Debug.logger.LogFormat(LogType.Assert, format, args);
		}

		/// <summary>
		///   <para>Logs a formatted assertion message to the Unity console.</para>
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertionFormat(Object context, string format, params object[] args)
		{
			Debug.logger.LogFormat(LogType.Assert, context, format, args);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OpenConsoleFile();

		[Conditional("UNITY_ASSERTIONS"), Obsolete("Assert(bool, string, params object[]) is obsolete. Use AssertFormat(bool, string, params object[]) (UnityUpgradable) -> AssertFormat(*)", true)]
		public static void Assert(bool condition, string format, params object[] args)
		{
			if (!condition)
			{
				Debug.logger.LogFormat(LogType.Assert, format, args);
			}
		}
	}
}
