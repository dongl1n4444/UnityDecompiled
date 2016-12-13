using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnityEditor.Graphs
{
	public class TypeSelector
	{
		public enum TypeKind
		{
			Simple,
			List,
			Array
		}

		private static string[] s_TypeKindNames = new string[]
		{
			"Simple",
			"List",
			"Array"
		};

		private static string[] s_TypeNames = new string[]
		{
			"string (System.String)",
			"integer (System.Int32)",
			"float (System.Single)",
			"bool (System.Boolean)",
			"byte (System.Byte)",
			"char (System.Char)",
			"GameObject (UnityEngine.GameObject)",
			"Component (UnityEngine.Component)",
			"Material (UnityEngine.Material)",
			"Vector2 (UnityEngine.Vector2)",
			"Vector3 (UnityEngine.Vector3)",
			"Vector4 (UnityEngine.Vector4)",
			"Color (UnityEngine.Color)",
			"Quaternion (UnityEngine.Quaternion)",
			"Rectangle (UnityEngine.Rect)",
			"AnimationCurve (UnityEngine.AnimationCurve)",
			"",
			"Other..."
		};

		private static string[] s_ComponentTypeNames = new string[]
		{
			"Transform (UnityEngine.Transform)",
			"Rigidbody (UnityEngine.Rigidbody)",
			"Camera (UnityEngine.Camera)",
			"Light (UnityEngine.Light)",
			"Animation (UnityEngine.Animation)",
			"ConstantForce (UnityEngine.ConstantForce)",
			"Renderer (UnityEngine.Renderer)",
			"AudioSource (UnityEngine.AudioSource)",
			"GUIText (UnityEngine.GUIText)",
			"NetworkView (UnityEngine.NetworkView)",
			"GUITexture (UnityEngine.GUITexture)",
			"Collider (UnityEngine.Collider)",
			"HingeJoint (UnityEngine.HingeJoint)",
			"ParticleEmitter (UnityEngine.ParticleEmitter)",
			"",
			"Other..."
		};

		[SerializeField]
		private bool m_EditingOther;

		[SerializeField]
		private string m_OtherTypeName;

		[SerializeField]
		private string m_ShownError;

		[SerializeField]
		private string[] m_TypeNames;

		[SerializeField]
		private bool m_OnlyComponents;

		public Type selectedType = typeof(DummyNullType);

		public TypeSelector.TypeKind selectedTypeKind = TypeSelector.TypeKind.Simple;

		private static Texture2D m_warningIcon;

		private static Texture2D warningIcon
		{
			get
			{
				Texture2D arg_1D_0;
				if ((arg_1D_0 = TypeSelector.m_warningIcon) == null)
				{
					arg_1D_0 = (TypeSelector.m_warningIcon = EditorGUIUtility.LoadIcon("console.warnicon"));
				}
				return arg_1D_0;
			}
		}

		public TypeSelector()
		{
			this.Init(TypeSelector.GenericTypeSelectorCommonTypes(), false);
		}

		public TypeSelector(string[] types)
		{
			this.Init(types, false);
		}

		public TypeSelector(bool onlyComponents)
		{
			this.Init((!onlyComponents) ? TypeSelector.GenericTypeSelectorCommonTypes() : TypeSelector.s_ComponentTypeNames, onlyComponents);
		}

		public bool DoGUI()
		{
			return (this.selectedType != typeof(DummyNullType) && this.selectedType != null) ? this.DoTypeClear() : this.DoTypeSelector();
		}

		private void Init(string[] types, bool onlyComponents)
		{
			this.m_TypeNames = types;
			this.m_OnlyComponents = onlyComponents;
		}

		public bool DoTypeKindGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			this.selectedTypeKind = (TypeSelector.TypeKind)EditorGUILayout.Popup("Kind", (int)this.selectedTypeKind, TypeSelector.s_TypeKindNames, new GUILayoutOption[0]);
			bool changed2 = GUI.changed;
			GUI.enabled |= changed;
			return changed2;
		}

		public static Type GetFinalType(TypeSelector.TypeKind typeKind, Type baseType)
		{
			Type result;
			if (typeKind != TypeSelector.TypeKind.Simple)
			{
				if (typeKind != TypeSelector.TypeKind.Array)
				{
					if (typeKind != TypeSelector.TypeKind.List)
					{
						throw new ArgumentException("Internal error: got weird type kind");
					}
					result = typeof(List<>).MakeGenericType(new Type[]
					{
						baseType
					});
				}
				else
				{
					result = baseType.MakeArrayType();
				}
			}
			else
			{
				result = baseType;
			}
			return result;
		}

		public static Type GetBaseType(TypeSelector.TypeKind typeKind, Type finalType)
		{
			Type result;
			if (finalType == null)
			{
				result = null;
			}
			else if (typeKind != TypeSelector.TypeKind.Simple)
			{
				if (typeKind != TypeSelector.TypeKind.Array)
				{
					if (typeKind != TypeSelector.TypeKind.List)
					{
						throw new ArgumentException("Internal error: got weird type kind");
					}
					result = finalType.GetGenericArguments()[0];
				}
				else
				{
					result = finalType.GetElementType();
				}
			}
			else
			{
				result = finalType;
			}
			return result;
		}

		public static TypeSelector.TypeKind GetTypeKind(Type dataType)
		{
			TypeSelector.TypeKind result;
			if (dataType == null)
			{
				result = TypeSelector.TypeKind.Simple;
			}
			else if (dataType.IsArray)
			{
				result = TypeSelector.TypeKind.Array;
			}
			else if (dataType.IsGenericType)
			{
				result = TypeSelector.TypeKind.List;
			}
			else
			{
				result = TypeSelector.TypeKind.Simple;
			}
			return result;
		}

		private bool DoTypeClear()
		{
			bool result = false;
			EditorGUILayout.LabelField("Type", TypeSelector.DotNetTypeNiceName(this.selectedType), new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Reset", EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				this.selectedType = typeof(DummyNullType);
				result = true;
			}
			GUILayout.EndHorizontal();
			return result;
		}

		private bool DoTypeSelector()
		{
			bool result;
			if (this.m_EditingOther)
			{
				result = this.DoOtherEditing();
			}
			else
			{
				int num = EditorGUILayout.Popup("Choose a type", -1, this.m_TypeNames, new GUILayoutOption[0]);
				if (num != -1)
				{
					string text = this.m_TypeNames[num];
					if (text == "Other...")
					{
						this.m_EditingOther = true;
						this.m_OtherTypeName = string.Empty;
						result = false;
					}
					else
					{
						int num2 = text.IndexOf('(');
						if (num2 != -1)
						{
							text = text.Substring(num2 + 1, text.IndexOf(')') - num2 - 1);
						}
						this.selectedType = TypeSelector.FindType(text);
						result = true;
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private static Type FindType(string typeName)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				Type type = assembly.GetType(typeName);
				if (type != null)
				{
					return type;
				}
			}
			throw new ArgumentException("Type '" + typeName + "' was not found.");
		}

		private bool DoOtherEditing()
		{
			bool result = false;
			this.m_OtherTypeName = EditorGUILayout.TextField("Full type Name", this.m_OtherTypeName, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Set", EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				try
				{
					this.selectedType = TypeSelector.FindType(this.m_OtherTypeName);
					if (!this.m_OnlyComponents || typeof(Component).IsAssignableFrom(this.selectedType))
					{
						this.m_OtherTypeName = string.Empty;
						this.m_EditingOther = false;
						this.m_ShownError = string.Empty;
						result = true;
					}
					this.m_ShownError = "Type must be derived from 'Component'.";
				}
				catch
				{
					this.m_ShownError = "Could not find a type '" + this.m_OtherTypeName + "'";
				}
			}
			if (GUILayout.Button("Cancel", EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				this.m_OtherTypeName = string.Empty;
				this.m_EditingOther = false;
				this.m_ShownError = string.Empty;
			}
			GUILayout.EndHorizontal();
			if (!string.IsNullOrEmpty(this.m_ShownError))
			{
				TypeSelector.ShowError(this.m_ShownError);
			}
			return result;
		}

		private static void ShowError(string shownError)
		{
			GUIContent content = new GUIContent(shownError)
			{
				image = TypeSelector.warningIcon
			};
			GUILayout.Space(5f);
			GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
			GUILayout.Label(content, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
			GUILayout.EndVertical();
		}

		private static string[] GenericTypeSelectorCommonTypes()
		{
			return TypeSelector.s_TypeNames;
		}

		public static string DotNetTypeNiceName(Type t)
		{
			string result;
			if (t == null)
			{
				result = string.Empty;
			}
			else
			{
				string[] array = TypeSelector.s_TypeNames;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					if (text.IndexOf(t.FullName) != -1)
					{
						result = text;
						return result;
					}
				}
				result = t.FullName;
			}
			return result;
		}
	}
}
