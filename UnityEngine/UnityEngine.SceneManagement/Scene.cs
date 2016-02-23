using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.SceneManagement
{
	/// <summary>
	///   <para>Run-time data structure for *.unity file.</para>
	/// </summary>
	public struct Scene
	{
		private int m_Handle;

		internal int handle
		{
			get
			{
				return this.m_Handle;
			}
		}

		/// <summary>
		///   <para>Returns the relative path of the scene. Like: "AssetsMyScenesMyScene.unity".</para>
		/// </summary>
		public string path
		{
			get
			{
				return Scene.GetPathInternal(this.handle);
			}
		}

		/// <summary>
		///   <para>Returns the name of the scene.</para>
		/// </summary>
		public string name
		{
			get
			{
				return Scene.GetNameInternal(this.handle);
			}
		}

		/// <summary>
		///   <para>Returns true if the scene is loaded.</para>
		/// </summary>
		public bool isLoaded
		{
			get
			{
				return Scene.GetIsLoadedInternal(this.handle);
			}
		}

		/// <summary>
		///   <para>Returns the index of the scene in the Build Settings. Always returns -1 if the scene was loaded through an AssetBundle.</para>
		/// </summary>
		public int buildIndex
		{
			get
			{
				return Scene.GetBuildIndexInternal(this.handle);
			}
		}

		/// <summary>
		///   <para>Returns true if the scene is modifed.</para>
		/// </summary>
		public bool isDirty
		{
			get
			{
				return Scene.GetIsDirtyInternal(this.handle);
			}
		}

		/// <summary>
		///   <para>The number of root transforms of this scene.</para>
		/// </summary>
		public int rootCount
		{
			get
			{
				return Scene.GetRootCountInternal(this.handle);
			}
		}

		/// <summary>
		///   <para>Tells if the scene is valid.
		/// A scene can be invalid if you eg. tried to open a scene that does not exists in that case the scene returnen from EditorSceneManager.OpenScene would be invalid.</para>
		/// </summary>
		/// <returns>
		///   <para>Returns true if the scene is valid.</para>
		/// </returns>
		public bool IsValid()
		{
			return Scene.IsValidInternal(this.handle);
		}

		/// <summary>
		///   <para>Returns all the root game objects in the scene.</para>
		/// </summary>
		/// <returns>
		///   <para>An array of game objects.</para>
		/// </returns>
		public GameObject[] GetRootGameObjects()
		{
			List<GameObject> list = new List<GameObject>(this.rootCount);
			this.GetRootGameObjects(list);
			return list.ToArray();
		}

		public void GetRootGameObjects(List<GameObject> rootGameObjects)
		{
			if (rootGameObjects.Capacity < this.rootCount)
			{
				rootGameObjects.Capacity = this.rootCount;
			}
			rootGameObjects.Clear();
			if (!this.IsValid())
			{
				throw new ArgumentException("The scene is invalid.");
			}
			if (!this.isLoaded)
			{
				throw new ArgumentException("The scene is not loaded.");
			}
			if (this.rootCount == 0)
			{
				return;
			}
			Scene.GetRootGameObjectsInternal(this.handle, rootGameObjects);
		}

		public override int GetHashCode()
		{
			return this.m_Handle;
		}

		public override bool Equals(object other)
		{
			if (!(other is Scene))
			{
				return false;
			}
			Scene scene = (Scene)other;
			return this.handle == scene.handle;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValidInternal(int sceneHandle);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetPathInternal(int sceneHandle);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetNameInternal(int sceneHandle);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIsLoadedInternal(int sceneHandle);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIsDirtyInternal(int sceneHandle);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetBuildIndexInternal(int sceneHandle);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRootCountInternal(int sceneHandle);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRootGameObjectsInternal(int sceneHandle, object resultRootList);

		public static bool operator ==(Scene lhs, Scene rhs)
		{
			return lhs.handle == rhs.handle;
		}

		public static bool operator !=(Scene lhs, Scene rhs)
		{
			return lhs.handle != rhs.handle;
		}
	}
}
