using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	/// <summary>
	///   <para>Utility functions for working with JSON data.</para>
	/// </summary>
	public static class JsonUtility
	{
		/// <summary>
		///   <para>Generate a JSON representation of the public fields of an object.</para>
		/// </summary>
		/// <param name="obj">The object to convert to JSON form.</param>
		/// <param name="prettyPrint">If true, format the output for readability. If false, format the output for minimum size. Default is false.</param>
		/// <returns>
		///   <para>The object's data in JSON format.</para>
		/// </returns>
		public static string ToJson(object obj)
		{
			return JsonUtility.ToJson(obj, false);
		}

		/// <summary>
		///   <para>Generate a JSON representation of the public fields of an object.</para>
		/// </summary>
		/// <param name="obj">The object to convert to JSON form.</param>
		/// <param name="prettyPrint">If true, format the output for readability. If false, format the output for minimum size. Default is false.</param>
		/// <returns>
		///   <para>The object's data in JSON format.</para>
		/// </returns>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string ToJson(object obj, bool prettyPrint);

		public static T FromJson<T>(string json)
		{
			return (T)((object)JsonUtility.FromJson(json, typeof(T)));
		}

		/// <summary>
		///   <para>Create an object from its JSON representation.</para>
		/// </summary>
		/// <param name="json">The JSON representation of the object.</param>
		/// <param name="type">The type of object represented by the JSON.</param>
		/// <returns>
		///   <para>An instance of the object.</para>
		/// </returns>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern object FromJson(string json, Type type);

		/// <summary>
		///   <para>Overwrite data in an object by reading from its JSON representation.</para>
		/// </summary>
		/// <param name="json">The JSON representation of the object.</param>
		/// <param name="objectToOverwrite">The object that should be overwritten.</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FromJsonOverwrite(string json, object objectToOverwrite);
	}
}
