using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	/// <summary>
	///   <para>Script interface for particle systems (Shuriken).</para>
	/// </summary>
	public sealed class ParticleSystem : Component
	{
		/// <summary>
		///   <para>Script interface for a Burst.</para>
		/// </summary>
		public struct Burst
		{
			private float m_Time;

			private short m_MinCount;

			private short m_MaxCount;

			/// <summary>
			///   <para>The time that each burst occurs.</para>
			/// </summary>
			public float time
			{
				get
				{
					return this.m_Time;
				}
				set
				{
					this.m_Time = value;
				}
			}

			/// <summary>
			///   <para>Minimum number of bursts to be emitted.</para>
			/// </summary>
			public short minCount
			{
				get
				{
					return this.m_MinCount;
				}
				set
				{
					this.m_MinCount = value;
				}
			}

			/// <summary>
			///   <para>Maximum number of bursts to be emitted.</para>
			/// </summary>
			public short maxCount
			{
				get
				{
					return this.m_MaxCount;
				}
				set
				{
					this.m_MaxCount = value;
				}
			}

			/// <summary>
			///   <para>Construct a new Burst with a time and count.</para>
			/// </summary>
			/// <param name="_time">Time to emit the burst.</param>
			/// <param name="_minCount">Minimum number of particles to emit.</param>
			/// <param name="_maxCount">Maximum number of particles to emit.</param>
			/// <param name="_count"></param>
			public Burst(float _time, short _count)
			{
				this.m_Time = _time;
				this.m_MinCount = _count;
				this.m_MaxCount = _count;
			}

			public Burst(float _time, short _minCount, short _maxCount)
			{
				this.m_Time = _time;
				this.m_MinCount = _minCount;
				this.m_MaxCount = _maxCount;
			}
		}

		/// <summary>
		///   <para>Script interface for a Min-Max Curve.</para>
		/// </summary>
		public struct MinMaxCurve
		{
			private ParticleSystemCurveMode m_Mode;

			private float m_CurveScalar;

			private AnimationCurve m_CurveMin;

			private AnimationCurve m_CurveMax;

			private float m_ConstantMin;

			private float m_ConstantMax;

			/// <summary>
			///   <para>Set the mode that the min-max curve will use to evaluate values.</para>
			/// </summary>
			public ParticleSystemCurveMode mode
			{
				get
				{
					return this.m_Mode;
				}
				set
				{
					this.m_Mode = value;
				}
			}

			/// <summary>
			///   <para>Set a multiplier to be applied to the curves.</para>
			/// </summary>
			public float curveScalar
			{
				get
				{
					return this.m_CurveScalar;
				}
				set
				{
					this.m_CurveScalar = value;
				}
			}

			/// <summary>
			///   <para>Set a curve for the upper bound.</para>
			/// </summary>
			public AnimationCurve curveMax
			{
				get
				{
					return this.m_CurveMax;
				}
				set
				{
					this.m_CurveMax = value;
				}
			}

			/// <summary>
			///   <para>Set a curve for the lower bound.</para>
			/// </summary>
			public AnimationCurve curveMin
			{
				get
				{
					return this.m_CurveMin;
				}
				set
				{
					this.m_CurveMin = value;
				}
			}

			/// <summary>
			///   <para>Set a constant for the upper bound.</para>
			/// </summary>
			public float constantMax
			{
				get
				{
					return this.m_ConstantMax;
				}
				set
				{
					this.m_ConstantMax = value;
				}
			}

			/// <summary>
			///   <para>Set a constant for the lower bound.</para>
			/// </summary>
			public float constantMin
			{
				get
				{
					return this.m_ConstantMin;
				}
				set
				{
					this.m_ConstantMin = value;
				}
			}

			/// <summary>
			///   <para>A single constant value for the entire curve.</para>
			/// </summary>
			/// <param name="constant">Constant value.</param>
			public MinMaxCurve(float constant)
			{
				this.m_Mode = ParticleSystemCurveMode.Constant;
				this.m_CurveScalar = 0f;
				this.m_CurveMin = null;
				this.m_CurveMax = null;
				this.m_ConstantMin = 0f;
				this.m_ConstantMax = constant;
			}

			/// <summary>
			///   <para>Use one curve when evaluating numbers along this Min-Max curve.</para>
			/// </summary>
			/// <param name="scalar">A multiplier to be applied to the curve.</param>
			/// <param name="curve">A single curve for evaluating against.</param>
			public MinMaxCurve(float scalar, AnimationCurve curve)
			{
				this.m_Mode = ParticleSystemCurveMode.Curve;
				this.m_CurveScalar = scalar;
				this.m_CurveMin = null;
				this.m_CurveMax = curve;
				this.m_ConstantMin = 0f;
				this.m_ConstantMax = 0f;
			}

			/// <summary>
			///   <para>Randomly select values based on the interval between the minimum and maximum curves.</para>
			/// </summary>
			/// <param name="scalar">A multiplier to be applied to the curves.</param>
			/// <param name="min">The curve describing the minimum values to be evaluated.</param>
			/// <param name="max">The curve describing the maximum values to be evaluated.</param>
			public MinMaxCurve(float scalar, AnimationCurve min, AnimationCurve max)
			{
				this.m_Mode = ParticleSystemCurveMode.TwoCurves;
				this.m_CurveScalar = scalar;
				this.m_CurveMin = min;
				this.m_CurveMax = max;
				this.m_ConstantMin = 0f;
				this.m_ConstantMax = 0f;
			}

			/// <summary>
			///   <para>Randomly select values based on the interval between the minimum and maximum constants.</para>
			/// </summary>
			/// <param name="min">The constant describing the minimum values to be evaluated.</param>
			/// <param name="max">The constant describing the maximum values to be evaluated.</param>
			public MinMaxCurve(float min, float max)
			{
				this.m_Mode = ParticleSystemCurveMode.TwoConstants;
				this.m_CurveScalar = 0f;
				this.m_CurveMin = null;
				this.m_CurveMax = null;
				this.m_ConstantMin = min;
				this.m_ConstantMax = max;
			}
		}

		/// <summary>
		///   <para>Script interface for a Min-Max Gradient.</para>
		/// </summary>
		public struct MinMaxGradient
		{
			private ParticleSystemGradientMode m_Mode;

			private Gradient m_GradientMin;

			private Gradient m_GradientMax;

			private Color m_ColorMin;

			private Color m_ColorMax;

			/// <summary>
			///   <para>Set the mode that the min-max gradient will use to evaluate colors.</para>
			/// </summary>
			public ParticleSystemGradientMode mode
			{
				get
				{
					return this.m_Mode;
				}
				set
				{
					this.m_Mode = value;
				}
			}

			/// <summary>
			///   <para>Set a gradient for the upper bound.</para>
			/// </summary>
			public Gradient gradientMax
			{
				get
				{
					return this.m_GradientMax;
				}
				set
				{
					this.m_GradientMax = value;
				}
			}

			/// <summary>
			///   <para>Set a gradient for the lower bound.</para>
			/// </summary>
			public Gradient gradientMin
			{
				get
				{
					return this.m_GradientMin;
				}
				set
				{
					this.m_GradientMin = value;
				}
			}

			/// <summary>
			///   <para>Set a constant color for the upper bound.</para>
			/// </summary>
			public Color colorMax
			{
				get
				{
					return this.m_ColorMax;
				}
				set
				{
					this.m_ColorMax = value;
				}
			}

			/// <summary>
			///   <para>Set a constant color for the lower bound.</para>
			/// </summary>
			public Color colorMin
			{
				get
				{
					return this.m_ColorMin;
				}
				set
				{
					this.m_ColorMin = value;
				}
			}

			/// <summary>
			///   <para>A single constant color for the entire gradient.</para>
			/// </summary>
			/// <param name="color">Constant color.</param>
			public MinMaxGradient(Color color)
			{
				this.m_Mode = ParticleSystemGradientMode.Color;
				this.m_GradientMin = null;
				this.m_GradientMax = null;
				this.m_ColorMin = Color.black;
				this.m_ColorMax = color;
			}

			/// <summary>
			///   <para>Use one gradient when evaluating numbers along this Min-Max gradient.</para>
			/// </summary>
			/// <param name="gradient">A single gradient for evaluating against.</param>
			public MinMaxGradient(Gradient gradient)
			{
				this.m_Mode = ParticleSystemGradientMode.Gradient;
				this.m_GradientMin = null;
				this.m_GradientMax = gradient;
				this.m_ColorMin = Color.black;
				this.m_ColorMax = Color.black;
			}

			/// <summary>
			///   <para>Randomly select colors based on the interval between the minimum and maximum constants.</para>
			/// </summary>
			/// <param name="min">The constant color describing the minimum colors to be evaluated.</param>
			/// <param name="max">The constant color describing the maximum colors to be evaluated.</param>
			public MinMaxGradient(Color min, Color max)
			{
				this.m_Mode = ParticleSystemGradientMode.TwoColors;
				this.m_GradientMin = null;
				this.m_GradientMax = null;
				this.m_ColorMin = min;
				this.m_ColorMax = max;
			}

			/// <summary>
			///   <para>Randomly select colors based on the interval between the minimum and maximum gradients.</para>
			/// </summary>
			/// <param name="min">The gradient describing the minimum colors to be evaluated.</param>
			/// <param name="max">The gradient describing the maximum colors to be evaluated.</param>
			public MinMaxGradient(Gradient min, Gradient max)
			{
				this.m_Mode = ParticleSystemGradientMode.TwoGradients;
				this.m_GradientMin = min;
				this.m_GradientMax = max;
				this.m_ColorMin = Color.black;
				this.m_ColorMax = Color.black;
			}
		}

		/// <summary>
		///   <para>Script interface for the Emission module.</para>
		/// </summary>
		public struct EmissionModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the Emission module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.EmissionModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.EmissionModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The rate at which new particles are spawned.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve rate
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.EmissionModule.GetRate(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.EmissionModule.SetRate(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>The emission type.</para>
			/// </summary>
			public ParticleSystemEmissionType type
			{
				get
				{
					return (ParticleSystemEmissionType)ParticleSystem.EmissionModule.GetType(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.EmissionModule.SetType(this.m_ParticleSystem, (int)value);
				}
			}

			/// <summary>
			///   <para>The current number of bursts.</para>
			/// </summary>
			public int burstCount
			{
				get
				{
					return ParticleSystem.EmissionModule.GetBurstCount(this.m_ParticleSystem);
				}
			}

			internal EmissionModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public void SetBursts(ParticleSystem.Burst[] bursts)
			{
				ParticleSystem.EmissionModule.SetBursts(this.m_ParticleSystem, bursts, bursts.Length);
			}

			public void SetBursts(ParticleSystem.Burst[] bursts, int size)
			{
				ParticleSystem.EmissionModule.SetBursts(this.m_ParticleSystem, bursts, size);
			}

			public int GetBursts(ParticleSystem.Burst[] bursts)
			{
				return ParticleSystem.EmissionModule.GetBursts(this.m_ParticleSystem, bursts);
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetType(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetType(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetBurstCount(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRate(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRate(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetBursts(ParticleSystem system, ParticleSystem.Burst[] bursts, int size);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetBursts(ParticleSystem system, ParticleSystem.Burst[] bursts);
		}

		/// <summary>
		///   <para>Script interface for the Shape module.</para>
		/// </summary>
		public struct ShapeModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the Shape module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.ShapeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Type of shape to emit particles from.</para>
			/// </summary>
			public ParticleSystemShapeType shapeType
			{
				get
				{
					return (ParticleSystemShapeType)ParticleSystem.ShapeModule.GetShapeType(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetShapeType(this.m_ParticleSystem, (int)value);
				}
			}

			/// <summary>
			///   <para>Randomizes the starting direction of particles.</para>
			/// </summary>
			public bool randomDirection
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRandomDirection(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRandomDirection(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Radius of the shape.</para>
			/// </summary>
			public float radius
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRadius(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRadius(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Angle of the cone.</para>
			/// </summary>
			public float angle
			{
				get
				{
					return ParticleSystem.ShapeModule.GetAngle(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetAngle(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Length of the cone.</para>
			/// </summary>
			public float length
			{
				get
				{
					return ParticleSystem.ShapeModule.GetLength(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetLength(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Scale of the box.</para>
			/// </summary>
			public Vector3 box
			{
				get
				{
					return ParticleSystem.ShapeModule.GetBox(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetBox(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Where on the mesh to emit particles from.</para>
			/// </summary>
			public ParticleSystemMeshShapeType meshShapeType
			{
				get
				{
					return (ParticleSystemMeshShapeType)ParticleSystem.ShapeModule.GetMeshShapeType(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetMeshShapeType(this.m_ParticleSystem, (int)value);
				}
			}

			/// <summary>
			///   <para>Mesh to emit particles from.</para>
			/// </summary>
			public Mesh mesh
			{
				get
				{
					return ParticleSystem.ShapeModule.GetMesh(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetMesh(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>MeshRenderer to emit particles from.</para>
			/// </summary>
			public MeshRenderer meshRenderer
			{
				get
				{
					return ParticleSystem.ShapeModule.GetMeshRenderer(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetMeshRenderer(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>SkinnedMeshRenderer to emit particles from.</para>
			/// </summary>
			public SkinnedMeshRenderer skinnedMeshRenderer
			{
				get
				{
					return ParticleSystem.ShapeModule.GetSkinnedMeshRenderer(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetSkinnedMeshRenderer(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Emit from a single material, or the whole mesh.</para>
			/// </summary>
			public bool useMeshMaterialIndex
			{
				get
				{
					return ParticleSystem.ShapeModule.GetUseMeshMaterialIndex(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetUseMeshMaterialIndex(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Emit particles from a single material of a mesh.</para>
			/// </summary>
			public int meshMaterialIndex
			{
				get
				{
					return ParticleSystem.ShapeModule.GetMeshMaterialIndex(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetMeshMaterialIndex(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Modulate the particle colours with the vertex colors, or the material color if no vertex colors exist.</para>
			/// </summary>
			public bool useMeshColors
			{
				get
				{
					return ParticleSystem.ShapeModule.GetUseMeshColors(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetUseMeshColors(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Move particles away from the surface of the source mesh.</para>
			/// </summary>
			public float normalOffset
			{
				get
				{
					return ParticleSystem.ShapeModule.GetNormalOffset(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetNormalOffset(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Circle arc angle.</para>
			/// </summary>
			public float arc
			{
				get
				{
					return ParticleSystem.ShapeModule.GetArc(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetArc(this.m_ParticleSystem, value);
				}
			}

			internal ShapeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetShapeType(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetShapeType(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRandomDirection(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetRandomDirection(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadius(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadius(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetAngle(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetAngle(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLength(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetLength(ParticleSystem system);

			private static void SetBox(ParticleSystem system, Vector3 value)
			{
				ParticleSystem.ShapeModule.INTERNAL_CALL_SetBox(system, ref value);
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetBox(ParticleSystem system, ref Vector3 value);

			private static Vector3 GetBox(ParticleSystem system)
			{
				Vector3 result;
				ParticleSystem.ShapeModule.INTERNAL_CALL_GetBox(system, out result);
				return result;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetBox(ParticleSystem system, out Vector3 value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMeshShapeType(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMeshShapeType(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMesh(ParticleSystem system, Mesh value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Mesh GetMesh(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMeshRenderer(ParticleSystem system, MeshRenderer value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern MeshRenderer GetMeshRenderer(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSkinnedMeshRenderer(ParticleSystem system, SkinnedMeshRenderer value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern SkinnedMeshRenderer GetSkinnedMeshRenderer(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseMeshMaterialIndex(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseMeshMaterialIndex(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMeshMaterialIndex(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMeshMaterialIndex(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseMeshColors(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseMeshColors(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetNormalOffset(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetNormalOffset(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetArc(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetArc(ParticleSystem system);
		}

		/// <summary>
		///   <para>Script interface for the Velocity Over Lifetime module.</para>
		/// </summary>
		public struct VelocityOverLifetimeModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the Velocity Over Lifetime module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Curve to control particle speed based on lifetime, on the X axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Curve to control particle speed based on lifetime, on the Y axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Curve to control particle speed based on lifetime, on the Z axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Specifies if the velocities are in local space (rotated with the transform) or world space.</para>
			/// </summary>
			public ParticleSystemSimulationSpace space
			{
				get
				{
					return (!ParticleSystem.VelocityOverLifetimeModule.GetWorldSpace(this.m_ParticleSystem)) ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetWorldSpace(this.m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
				}
			}

			internal VelocityOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWorldSpace(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetWorldSpace(ParticleSystem system);
		}

		/// <summary>
		///   <para>Script interface for the Limit Velocity Over Lifetime module.</para>
		/// </summary>
		public struct LimitVelocityOverLifetimeModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the Limit Force Over Lifetime module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Maximum velocity curve for the X axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve limitX
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LimitVelocityOverLifetimeModule.GetX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Maximum velocity curve for the Y axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve limitY
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LimitVelocityOverLifetimeModule.GetY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Maximum velocity curve for the Z axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve limitZ
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LimitVelocityOverLifetimeModule.GetZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Maximum velocity curve, when not using one curve per axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve limit
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LimitVelocityOverLifetimeModule.GetMagnitude(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetMagnitude(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Controls how much the velocity that exceeds the velocity limit should be dampened.</para>
			/// </summary>
			public float dampen
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetDampen(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetDampen(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Set the velocity limit on each axis separately.</para>
			/// </summary>
			public bool separateAxes
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetSeparateAxes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetSeparateAxes(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Specifies if the velocity limits are in local space (rotated with the transform) or world space.</para>
			/// </summary>
			public ParticleSystemSimulationSpace space
			{
				get
				{
					return (!ParticleSystem.LimitVelocityOverLifetimeModule.GetWorldSpace(this.m_ParticleSystem)) ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetWorldSpace(this.m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
				}
			}

			internal LimitVelocityOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMagnitude(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetMagnitude(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDampen(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetDampen(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWorldSpace(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetWorldSpace(ParticleSystem system);
		}

		/// <summary>
		///   <para>The Inherit Velocity Module controls how the velocity of the emitter is transferred to the particles as they are emitted.</para>
		/// </summary>
		public struct InheritVelocityModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the InheritVelocity module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.InheritVelocityModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.InheritVelocityModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>How to apply emitter velocity to particles.</para>
			/// </summary>
			public ParticleSystemInheritVelocityMode mode
			{
				get
				{
					return (ParticleSystemInheritVelocityMode)ParticleSystem.InheritVelocityModule.GetMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.InheritVelocityModule.SetMode(this.m_ParticleSystem, (int)value);
				}
			}

			/// <summary>
			///   <para>Curve to define how much emitter velocity is applied during the lifetime of a particle.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve curve
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.InheritVelocityModule.GetCurve(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.InheritVelocityModule.SetCurve(this.m_ParticleSystem, ref value);
				}
			}

			internal InheritVelocityModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMode(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMode(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCurve(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetCurve(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
		}

		/// <summary>
		///   <para>Script interface for the Force Over Lifetime module.</para>
		/// </summary>
		public struct ForceOverLifetimeModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the Force Over Lifetime module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.ForceOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The curve defining particle forces in the X axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.ForceOverLifetimeModule.GetX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>The curve defining particle forces in the Y axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.ForceOverLifetimeModule.GetY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>The curve defining particle forces in the Z axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.ForceOverLifetimeModule.GetZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Are the forces being applied in local or world space?</para>
			/// </summary>
			public ParticleSystemSimulationSpace space
			{
				get
				{
					return (!ParticleSystem.ForceOverLifetimeModule.GetWorldSpace(this.m_ParticleSystem)) ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World;
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetWorldSpace(this.m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
				}
			}

			/// <summary>
			///   <para>When randomly selecting values between two curves or constants, this flag will cause a new random force to be chosen on each frame.</para>
			/// </summary>
			public bool randomized
			{
				get
				{
					return ParticleSystem.ForceOverLifetimeModule.GetRandomized(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetRandomized(this.m_ParticleSystem, value);
				}
			}

			internal ForceOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWorldSpace(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetWorldSpace(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRandomized(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetRandomized(ParticleSystem system);
		}

		/// <summary>
		///   <para>Script interface for the Color Over Lifetime module.</para>
		/// </summary>
		public struct ColorOverLifetimeModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the Color Over Lifetime module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.ColorOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ColorOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The curve controlling the particle colors.</para>
			/// </summary>
			public ParticleSystem.MinMaxGradient color
			{
				get
				{
					ParticleSystem.MinMaxGradient result = default(ParticleSystem.MinMaxGradient);
					ParticleSystem.ColorOverLifetimeModule.GetColor(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.ColorOverLifetimeModule.SetColor(this.m_ParticleSystem, ref value);
				}
			}

			internal ColorOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
		}

		/// <summary>
		///   <para>Script interface for the Color By Speed module.</para>
		/// </summary>
		public struct ColorBySpeedModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the Color By Speed module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.ColorBySpeedModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ColorBySpeedModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The curve controlling the particle colors.</para>
			/// </summary>
			public ParticleSystem.MinMaxGradient color
			{
				get
				{
					ParticleSystem.MinMaxGradient result = default(ParticleSystem.MinMaxGradient);
					ParticleSystem.ColorBySpeedModule.GetColor(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.ColorBySpeedModule.SetColor(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Apply the color gradient between these minimum and maximum speeds.</para>
			/// </summary>
			public Vector2 range
			{
				get
				{
					return ParticleSystem.ColorBySpeedModule.GetRange(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ColorBySpeedModule.SetRange(this.m_ParticleSystem, value);
				}
			}

			internal ColorBySpeedModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			private static void SetRange(ParticleSystem system, Vector2 value)
			{
				ParticleSystem.ColorBySpeedModule.INTERNAL_CALL_SetRange(system, ref value);
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);

			private static Vector2 GetRange(ParticleSystem system)
			{
				Vector2 result;
				ParticleSystem.ColorBySpeedModule.INTERNAL_CALL_GetRange(system, out result);
				return result;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);
		}

		/// <summary>
		///   <para>Script interface for the Size Over Lifetime module.</para>
		/// </summary>
		public struct SizeOverLifetimeModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the Size Over Lifetime module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.SizeOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Curve to control particle size based on lifetime.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve size
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeOverLifetimeModule.GetSize(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetSize(this.m_ParticleSystem, ref value);
				}
			}

			internal SizeOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSize(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetSize(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
		}

		/// <summary>
		///   <para>Script interface for the Size By Speed module.</para>
		/// </summary>
		public struct SizeBySpeedModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the Size By Speed module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Curve to control particle size based on speed.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve size
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeBySpeedModule.GetSize(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetSize(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Apply the size curve between these minimum and maximum speeds.</para>
			/// </summary>
			public Vector2 range
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetRange(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetRange(this.m_ParticleSystem, value);
				}
			}

			internal SizeBySpeedModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSize(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetSize(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			private static void SetRange(ParticleSystem system, Vector2 value)
			{
				ParticleSystem.SizeBySpeedModule.INTERNAL_CALL_SetRange(system, ref value);
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);

			private static Vector2 GetRange(ParticleSystem system)
			{
				Vector2 result;
				ParticleSystem.SizeBySpeedModule.INTERNAL_CALL_GetRange(system, out result);
				return result;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);
		}

		/// <summary>
		///   <para>Script interface for the Rotation Over Lifetime module.</para>
		/// </summary>
		public struct RotationOverLifetimeModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the Rotation Over Lifetime module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.RotationOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Rotation over lifetime curve for the X axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationOverLifetimeModule.GetX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Rotation over lifetime curve for the Y axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationOverLifetimeModule.GetY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Rotation over lifetime curve for the Z axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationOverLifetimeModule.GetZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Set the rotation over lifetime on each axis separately.</para>
			/// </summary>
			public bool separateAxes
			{
				get
				{
					return ParticleSystem.RotationOverLifetimeModule.GetSeparateAxes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetSeparateAxes(this.m_ParticleSystem, value);
				}
			}

			internal RotationOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);
		}

		/// <summary>
		///   <para>Script interface for the Rotation By Speed module.</para>
		/// </summary>
		public struct RotationBySpeedModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the Rotation By Speed module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.RotationBySpeedModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Rotation by speed curve for the X axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationBySpeedModule.GetX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Rotation by speed curve for the Y axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationBySpeedModule.GetY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Rotation by speed curve for the Z axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationBySpeedModule.GetZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Set the rotation by speed on each axis separately.</para>
			/// </summary>
			public bool separateAxes
			{
				get
				{
					return ParticleSystem.RotationBySpeedModule.GetSeparateAxes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetSeparateAxes(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Apply the rotation curve between these minimum and maximum speeds.</para>
			/// </summary>
			public Vector2 range
			{
				get
				{
					return ParticleSystem.RotationBySpeedModule.GetRange(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetRange(this.m_ParticleSystem, value);
				}
			}

			internal RotationBySpeedModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);

			private static void SetRange(ParticleSystem system, Vector2 value)
			{
				ParticleSystem.RotationBySpeedModule.INTERNAL_CALL_SetRange(system, ref value);
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);

			private static Vector2 GetRange(ParticleSystem system)
			{
				Vector2 result;
				ParticleSystem.RotationBySpeedModule.INTERNAL_CALL_GetRange(system, out result);
				return result;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);
		}

		/// <summary>
		///   <para>Script interface for the External Forces module.</para>
		/// </summary>
		public struct ExternalForcesModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the External Forces module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.ExternalForcesModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ExternalForcesModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Multiplies the magnitude of applied external forces.</para>
			/// </summary>
			public float multiplier
			{
				get
				{
					return ParticleSystem.ExternalForcesModule.GetMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ExternalForcesModule.SetMultiplier(this.m_ParticleSystem, value);
				}
			}

			internal ExternalForcesModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMultiplier(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMultiplier(ParticleSystem system);
		}

		/// <summary>
		///   <para>Script interface for the Collision module.</para>
		/// </summary>
		public struct CollisionModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the Collision module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.CollisionModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The type of particle collision to perform.</para>
			/// </summary>
			public ParticleSystemCollisionType type
			{
				get
				{
					return (ParticleSystemCollisionType)ParticleSystem.CollisionModule.GetType(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetType(this.m_ParticleSystem, (int)value);
				}
			}

			/// <summary>
			///   <para>Choose between 2D and 3D world collisions.</para>
			/// </summary>
			public ParticleSystemCollisionMode mode
			{
				get
				{
					return (ParticleSystemCollisionMode)ParticleSystem.CollisionModule.GetMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetMode(this.m_ParticleSystem, (int)value);
				}
			}

			/// <summary>
			///   <para>How much speed is lost from each particle after a collision.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve dampen
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.CollisionModule.GetDampen(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.CollisionModule.SetDampen(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>How much force is applied to each particle after a collision.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve bounce
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.CollisionModule.GetBounce(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.CollisionModule.SetBounce(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>How much a particle's lifetime is reduced after a collision.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve lifetimeLoss
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.CollisionModule.GetEnergyLoss(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.CollisionModule.SetEnergyLoss(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Kill particles whose speed falls below this threshold, after a collision.</para>
			/// </summary>
			public float minKillSpeed
			{
				get
				{
					return ParticleSystem.CollisionModule.GetMinKillSpeed(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetMinKillSpeed(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Control which layers this particle system collides with.</para>
			/// </summary>
			public LayerMask collidesWith
			{
				get
				{
					return ParticleSystem.CollisionModule.GetCollidesWith(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetCollidesWith(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Allow particles to collide with dynamic colliders when using world collision mode.</para>
			/// </summary>
			public bool enableDynamicColliders
			{
				get
				{
					return ParticleSystem.CollisionModule.GetEnableDynamicColliders(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetEnableDynamicColliders(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Allow particles to collide when inside colliders.</para>
			/// </summary>
			public bool enableInteriorCollisions
			{
				get
				{
					return ParticleSystem.CollisionModule.GetEnableInteriorCollisions(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetEnableInteriorCollisions(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The maximum number of collision shapes that will be considered for particle collisions. Excess shapes will be ignored. Terrains take priority.</para>
			/// </summary>
			public int maxCollisionShapes
			{
				get
				{
					return ParticleSystem.CollisionModule.GetMaxCollisionShapes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetMaxCollisionShapes(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Specifies the accuracy of particle collisions against colliders in the scene.</para>
			/// </summary>
			public ParticleSystemCollisionQuality quality
			{
				get
				{
					return (ParticleSystemCollisionQuality)ParticleSystem.CollisionModule.GetQuality(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetQuality(this.m_ParticleSystem, (int)value);
				}
			}

			/// <summary>
			///   <para>Size of voxels in the collision cache.</para>
			/// </summary>
			public float voxelSize
			{
				get
				{
					return ParticleSystem.CollisionModule.GetVoxelSize(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetVoxelSize(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>A multiplier applied to the size of each particle before collisions are processed.</para>
			/// </summary>
			public float radiusScale
			{
				get
				{
					return ParticleSystem.CollisionModule.GetRadiusScale(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetRadiusScale(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Send collision callback messages.</para>
			/// </summary>
			public bool sendCollisionMessages
			{
				get
				{
					return ParticleSystem.CollisionModule.GetUsesCollisionMessages(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetUsesCollisionMessages(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The maximum number of planes it is possible to set as colliders.</para>
			/// </summary>
			public int maxPlaneCount
			{
				get
				{
					return ParticleSystem.CollisionModule.GetMaxPlaneCount(this.m_ParticleSystem);
				}
			}

			internal CollisionModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Set a collision plane to be used with this particle system.</para>
			/// </summary>
			/// <param name="index">Specifies which plane to set.</param>
			/// <param name="transform">The plane to set.</param>
			public void SetPlane(int index, Transform transform)
			{
				ParticleSystem.CollisionModule.SetPlane(this.m_ParticleSystem, index, transform);
			}

			/// <summary>
			///   <para>Get a collision plane associated with this particle system.</para>
			/// </summary>
			/// <param name="index">Specifies which plane to access.</param>
			/// <returns>
			///   <para>The plane.</para>
			/// </returns>
			public Transform GetPlane(int index)
			{
				return ParticleSystem.CollisionModule.GetPlane(this.m_ParticleSystem, index);
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetType(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetType(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMode(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMode(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDampen(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetDampen(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetBounce(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetBounce(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnergyLoss(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetEnergyLoss(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMinKillSpeed(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMinKillSpeed(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCollidesWith(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetCollidesWith(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnableDynamicColliders(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnableDynamicColliders(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnableInteriorCollisions(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnableInteriorCollisions(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMaxCollisionShapes(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMaxCollisionShapes(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetQuality(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetQuality(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetVoxelSize(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetVoxelSize(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusScale(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadiusScale(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUsesCollisionMessages(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUsesCollisionMessages(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetPlane(ParticleSystem system, int index, Transform transform);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Transform GetPlane(ParticleSystem system, int index);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMaxPlaneCount(ParticleSystem system);
		}

		/// <summary>
		///   <para>Script interface for the Sub Emitters module.</para>
		/// </summary>
		public struct SubEmittersModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the Sub Emitters module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Sub particle system to spawn on birth of the parent system's particles.</para>
			/// </summary>
			public ParticleSystem birth0
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetBirth(this.m_ParticleSystem, 0);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetBirth(this.m_ParticleSystem, 0, value);
				}
			}

			/// <summary>
			///   <para>Sub particle system to spawn on birth of the parent system's particles.</para>
			/// </summary>
			public ParticleSystem birth1
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetBirth(this.m_ParticleSystem, 1);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetBirth(this.m_ParticleSystem, 1, value);
				}
			}

			/// <summary>
			///   <para>Sub particle system to spawn on collision of the parent system's particles.</para>
			/// </summary>
			public ParticleSystem collision0
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetCollision(this.m_ParticleSystem, 0);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetCollision(this.m_ParticleSystem, 0, value);
				}
			}

			/// <summary>
			///   <para>Sub particle system to spawn on collision of the parent system's particles.</para>
			/// </summary>
			public ParticleSystem collision1
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetCollision(this.m_ParticleSystem, 1);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetCollision(this.m_ParticleSystem, 1, value);
				}
			}

			/// <summary>
			///   <para>Sub particle system to spawn on death of the parent system's particles.</para>
			/// </summary>
			public ParticleSystem death0
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetDeath(this.m_ParticleSystem, 0);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetDeath(this.m_ParticleSystem, 0, value);
				}
			}

			/// <summary>
			///   <para>Sub particle system to spawn on death of the parent system's particles.</para>
			/// </summary>
			public ParticleSystem death1
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetDeath(this.m_ParticleSystem, 1);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetDeath(this.m_ParticleSystem, 1, value);
				}
			}

			internal SubEmittersModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetBirth(ParticleSystem system, int index, ParticleSystem value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystem GetBirth(ParticleSystem system, int index);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCollision(ParticleSystem system, int index, ParticleSystem value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystem GetCollision(ParticleSystem system, int index);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDeath(ParticleSystem system, int index, ParticleSystem value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystem GetDeath(ParticleSystem system, int index);
		}

		/// <summary>
		///   <para>Script interface for the Texture Sheet Animation module.</para>
		/// </summary>
		public struct TextureSheetAnimationModule
		{
			private ParticleSystem m_ParticleSystem;

			/// <summary>
			///   <para>Enable/disable the Texture Sheet Animation module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Defines the tiling of the texture in the X axis.</para>
			/// </summary>
			public int numTilesX
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetNumTilesX(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetNumTilesX(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Defines the tiling of the texture in the Y axis.</para>
			/// </summary>
			public int numTilesY
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetNumTilesY(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetNumTilesY(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Specifies the animation type.</para>
			/// </summary>
			public ParticleSystemAnimationType animation
			{
				get
				{
					return (ParticleSystemAnimationType)ParticleSystem.TextureSheetAnimationModule.GetAnimationType(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetAnimationType(this.m_ParticleSystem, (int)value);
				}
			}

			/// <summary>
			///   <para>Use a random row of the texture sheet for each particle emitted.</para>
			/// </summary>
			public bool useRandomRow
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetUseRandomRow(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetUseRandomRow(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Curve to control which frame of the texture sheet animation to play.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve frameOverTime
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.TextureSheetAnimationModule.GetFrameOverTime(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetFrameOverTime(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Specifies how many times the animation will loop during the lifetime of the particle.</para>
			/// </summary>
			public int cycleCount
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetCycleCount(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetCycleCount(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Explicitly select which row of the texture sheet is used, when ParticleSystem.TextureSheetAnimationModule.useRandomRow is set to false.</para>
			/// </summary>
			public int rowIndex
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetRowIndex(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetRowIndex(this.m_ParticleSystem, value);
				}
			}

			internal TextureSheetAnimationModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetNumTilesX(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetNumTilesX(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetNumTilesY(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetNumTilesY(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetAnimationType(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetAnimationType(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseRandomRow(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseRandomRow(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetFrameOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetFrameOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCycleCount(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetCycleCount(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRowIndex(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetRowIndex(ParticleSystem system);
		}

		/// <summary>
		///   <para>Script interface for a Particle.</para>
		/// </summary>
		public struct Particle
		{
			private Vector3 m_Position;

			private Vector3 m_Velocity;

			private Vector3 m_AnimatedVelocity;

			private Vector3 m_InitialVelocity;

			private Vector3 m_AxisOfRotation;

			private Vector3 m_Rotation;

			private Vector3 m_AngularVelocity;

			private float m_StartSize;

			private Color32 m_StartColor;

			private uint m_RandomSeed;

			private float m_Lifetime;

			private float m_StartLifetime;

			private float m_EmitAccumulator0;

			private float m_EmitAccumulator1;

			/// <summary>
			///   <para>The position of the particle.</para>
			/// </summary>
			public Vector3 position
			{
				get
				{
					return this.m_Position;
				}
				set
				{
					this.m_Position = value;
				}
			}

			/// <summary>
			///   <para>The velocity of the particle.</para>
			/// </summary>
			public Vector3 velocity
			{
				get
				{
					return this.m_Velocity;
				}
				set
				{
					this.m_Velocity = value;
				}
			}

			/// <summary>
			///   <para>The lifetime of the particle.</para>
			/// </summary>
			public float lifetime
			{
				get
				{
					return this.m_Lifetime;
				}
				set
				{
					this.m_Lifetime = value;
				}
			}

			/// <summary>
			///   <para>The starting lifetime of the particle.</para>
			/// </summary>
			public float startLifetime
			{
				get
				{
					return this.m_StartLifetime;
				}
				set
				{
					this.m_StartLifetime = value;
				}
			}

			/// <summary>
			///   <para>The initial size of the particle. The current size of the particle is calculated procedurally based on this value and the active size modules.</para>
			/// </summary>
			public float startSize
			{
				get
				{
					return this.m_StartSize;
				}
				set
				{
					this.m_StartSize = value;
				}
			}

			public Vector3 axisOfRotation
			{
				get
				{
					return this.m_AxisOfRotation;
				}
				set
				{
					this.m_AxisOfRotation = value;
				}
			}

			/// <summary>
			///   <para>The rotation of the particle.</para>
			/// </summary>
			public float rotation
			{
				get
				{
					return this.m_Rotation.z * 57.29578f;
				}
				set
				{
					this.m_Rotation = new Vector3(0f, 0f, value * 0.0174532924f);
				}
			}

			/// <summary>
			///   <para>The 3D rotation of the particle.</para>
			/// </summary>
			public Vector3 rotation3D
			{
				get
				{
					return this.m_Rotation * 57.29578f;
				}
				set
				{
					this.m_Rotation = value * 0.0174532924f;
				}
			}

			/// <summary>
			///   <para>The angular velocity of the particle.</para>
			/// </summary>
			public float angularVelocity
			{
				get
				{
					return this.m_AngularVelocity.z * 57.29578f;
				}
				set
				{
					this.m_AngularVelocity.z = value * 0.0174532924f;
				}
			}

			/// <summary>
			///   <para>The 3D angular velocity of the particle.</para>
			/// </summary>
			public Vector3 angularVelocity3D
			{
				get
				{
					return this.m_AngularVelocity * 57.29578f;
				}
				set
				{
					this.m_AngularVelocity = value * 0.0174532924f;
				}
			}

			/// <summary>
			///   <para>The initial color of the particle. The current color of the particle is calculated procedurally based on this value and the active color modules.</para>
			/// </summary>
			public Color32 startColor
			{
				get
				{
					return this.m_StartColor;
				}
				set
				{
					this.m_StartColor = value;
				}
			}

			/// <summary>
			///   <para>The random value of the particle.</para>
			/// </summary>
			[Obsolete("randomValue property is deprecated. Use randomSeed instead to control random behavior of particles.")]
			public float randomValue
			{
				get
				{
					return BitConverter.ToSingle(BitConverter.GetBytes(this.m_RandomSeed), 0);
				}
				set
				{
					this.m_RandomSeed = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
				}
			}

			/// <summary>
			///   <para>The random seed of the particle.</para>
			/// </summary>
			public uint randomSeed
			{
				get
				{
					return this.m_RandomSeed;
				}
				set
				{
					this.m_RandomSeed = value;
				}
			}

			[Obsolete("size property is deprecated. Use startSize or GetCurrentSize() instead.")]
			public float size
			{
				get
				{
					return this.m_StartSize;
				}
				set
				{
					this.m_StartSize = value;
				}
			}

			[Obsolete("color property is deprecated. Use startColor or GetCurrentColor() instead.")]
			public Color32 color
			{
				get
				{
					return this.m_StartColor;
				}
				set
				{
					this.m_StartColor = value;
				}
			}

			/// <summary>
			///   <para>Calculate the current size of the particle by applying the relevant curves to its startSize property.</para>
			/// </summary>
			/// <param name="system">The particle system from which this particle was emitted.</param>
			/// <returns>
			///   <para>Current size.</para>
			/// </returns>
			public float GetCurrentSize(ParticleSystem system)
			{
				return ParticleSystem.Particle.GetCurrentSize(system, ref this);
			}

			/// <summary>
			///   <para>Calculate the current color of the particle by applying the relevant curves to its startColor property.</para>
			/// </summary>
			/// <param name="system">The particle system from which this particle was emitted.</param>
			/// <returns>
			///   <para>Current color.</para>
			/// </returns>
			public Color32 GetCurrentColor(ParticleSystem system)
			{
				return ParticleSystem.Particle.GetCurrentColor(system, ref this);
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetCurrentSize(ParticleSystem system, ref ParticleSystem.Particle particle);

			private static Color32 GetCurrentColor(ParticleSystem system, ref ParticleSystem.Particle particle)
			{
				Color32 result;
				ParticleSystem.Particle.INTERNAL_CALL_GetCurrentColor(system, ref particle, out result);
				return result;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetCurrentColor(ParticleSystem system, ref ParticleSystem.Particle particle, out Color32 value);
		}

		/// <summary>
		///   <para>Script interface for particle emission parameters.</para>
		/// </summary>
		public struct EmitParams
		{
			internal ParticleSystem.Particle m_Particle;

			internal bool m_PositionSet;

			internal bool m_VelocitySet;

			internal bool m_AxisOfRotationSet;

			internal bool m_RotationSet;

			internal bool m_AngularVelocitySet;

			internal bool m_StartSizeSet;

			internal bool m_StartColorSet;

			internal bool m_RandomSeedSet;

			internal bool m_StartLifetimeSet;

			/// <summary>
			///   <para>Override the position of emitted particles.</para>
			/// </summary>
			public Vector3 position
			{
				get
				{
					return this.m_Particle.position;
				}
				set
				{
					this.m_Particle.position = value;
					this.m_PositionSet = true;
				}
			}

			/// <summary>
			///   <para>Override the velocity of emitted particles.</para>
			/// </summary>
			public Vector3 velocity
			{
				get
				{
					return this.m_Particle.velocity;
				}
				set
				{
					this.m_Particle.velocity = value;
					this.m_VelocitySet = true;
				}
			}

			/// <summary>
			///   <para>Override the lifetime of emitted particles.</para>
			/// </summary>
			public float startLifetime
			{
				get
				{
					return this.m_Particle.startLifetime;
				}
				set
				{
					this.m_Particle.startLifetime = value;
					this.m_StartLifetimeSet = true;
				}
			}

			/// <summary>
			///   <para>Override the initial size of emitted particles.</para>
			/// </summary>
			public float startSize
			{
				get
				{
					return this.m_Particle.startSize;
				}
				set
				{
					this.m_Particle.startSize = value;
					this.m_StartSizeSet = true;
				}
			}

			/// <summary>
			///   <para>Override the axis of rotation of emitted particles.</para>
			/// </summary>
			public Vector3 axisOfRotation
			{
				get
				{
					return this.m_Particle.axisOfRotation;
				}
				set
				{
					this.m_Particle.axisOfRotation = value;
					this.m_AxisOfRotationSet = true;
				}
			}

			/// <summary>
			///   <para>Override the rotation of emitted particles.</para>
			/// </summary>
			public float rotation
			{
				get
				{
					return this.m_Particle.rotation;
				}
				set
				{
					this.m_Particle.rotation = value;
					this.m_RotationSet = true;
				}
			}

			/// <summary>
			///   <para>Override the 3D rotation of emitted particles.</para>
			/// </summary>
			public Vector3 rotation3D
			{
				get
				{
					return this.m_Particle.rotation3D;
				}
				set
				{
					this.m_Particle.rotation3D = value;
					this.m_RotationSet = true;
				}
			}

			/// <summary>
			///   <para>Override the angular velocity of emitted particles.</para>
			/// </summary>
			public float angularVelocity
			{
				get
				{
					return this.m_Particle.angularVelocity;
				}
				set
				{
					this.m_Particle.angularVelocity = value;
					this.m_AngularVelocitySet = true;
				}
			}

			/// <summary>
			///   <para>Override the 3D angular velocity of emitted particles.</para>
			/// </summary>
			public Vector3 angularVelocity3D
			{
				get
				{
					return this.m_Particle.angularVelocity3D;
				}
				set
				{
					this.m_Particle.angularVelocity3D = value;
					this.m_AngularVelocitySet = true;
				}
			}

			/// <summary>
			///   <para>Override the initial color of emitted particles.</para>
			/// </summary>
			public Color32 startColor
			{
				get
				{
					return this.m_Particle.startColor;
				}
				set
				{
					this.m_Particle.startColor = value;
					this.m_StartColorSet = true;
				}
			}

			/// <summary>
			///   <para>Override the random seed of emitted particles.</para>
			/// </summary>
			public uint randomSeed
			{
				get
				{
					return this.m_Particle.randomSeed;
				}
				set
				{
					this.m_Particle.randomSeed = value;
					this.m_RandomSeedSet = true;
				}
			}

			/// <summary>
			///   <para>Revert the position back to the value specified in the inspector.</para>
			/// </summary>
			public void ResetPosition()
			{
				this.m_PositionSet = false;
			}

			/// <summary>
			///   <para>Revert the velocity back to the value specified in the inspector.</para>
			/// </summary>
			public void ResetVelocity()
			{
				this.m_VelocitySet = false;
			}

			/// <summary>
			///   <para>Revert the axis of rotation back to the value specified in the inspector.</para>
			/// </summary>
			public void ResetAxisOfRotation()
			{
				this.m_AxisOfRotationSet = false;
			}

			/// <summary>
			///   <para>Reverts rotation and rotation3D back to the values specified in the inspector.</para>
			/// </summary>
			public void ResetRotation()
			{
				this.m_RotationSet = false;
			}

			/// <summary>
			///   <para>Reverts angularVelocity and angularVelocity3D back to the values specified in the inspector.</para>
			/// </summary>
			public void ResetAngularVelocity()
			{
				this.m_AngularVelocitySet = false;
			}

			/// <summary>
			///   <para>Revert the initial size back to the value specified in the inspector.</para>
			/// </summary>
			public void ResetStartSize()
			{
				this.m_StartSizeSet = false;
			}

			/// <summary>
			///   <para>Revert the initial color back to the value specified in the inspector.</para>
			/// </summary>
			public void ResetStartColor()
			{
				this.m_StartColorSet = false;
			}

			/// <summary>
			///   <para>Revert the random seed back to the value specified in the inspector.</para>
			/// </summary>
			public void ResetRandomSeed()
			{
				this.m_RandomSeedSet = false;
			}

			/// <summary>
			///   <para>Revert the lifetime back to the value specified in the inspector.</para>
			/// </summary>
			public void ResetStartLifetime()
			{
				this.m_StartLifetimeSet = false;
			}
		}

		[Obsolete("ParticleSystem.CollisionEvent has been deprecated. Use ParticleCollisionEvent instead (UnityUpgradable) -> ParticleCollisionEvent", true)]
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CollisionEvent
		{
			public Vector3 intersection
			{
				get
				{
					return default(Vector3);
				}
			}

			public Vector3 normal
			{
				get
				{
					return default(Vector3);
				}
			}

			public Vector3 velocity
			{
				get
				{
					return default(Vector3);
				}
			}

			public Collider collider
			{
				get
				{
					return null;
				}
			}
		}

		internal delegate bool IteratorDelegate(ParticleSystem ps);

		/// <summary>
		///   <para>Start delay in seconds.</para>
		/// </summary>
		public extern float startDelay
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Is the particle system playing right now ?</para>
		/// </summary>
		public extern bool isPlaying
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Is the particle system stopped right now ?</para>
		/// </summary>
		public extern bool isStopped
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Is the particle system paused right now ?</para>
		/// </summary>
		public extern bool isPaused
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Is the particle system looping?</para>
		/// </summary>
		public extern bool loop
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>If set to true, the particle system will automatically start playing on startup.</para>
		/// </summary>
		public extern bool playOnAwake
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Playback position in seconds.</para>
		/// </summary>
		public extern float time
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The duration of the particle system in seconds (Read Only).</para>
		/// </summary>
		public extern float duration
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The playback speed of the particle system. 1 is normal playback speed.</para>
		/// </summary>
		public extern float playbackSpeed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The current number of particles (Read Only).</para>
		/// </summary>
		public extern int particleCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>When set to false, the particle system will not emit particles.</para>
		/// </summary>
		[Obsolete("enableEmission property is deprecated. Use emission.enable instead.")]
		public extern bool enableEmission
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The rate of emission.</para>
		/// </summary>
		[Obsolete("emissionRate property is deprecated. Use emission.rate instead.")]
		public extern float emissionRate
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The initial speed of particles when emitted. When using curves, this values acts as a scale on the curve.</para>
		/// </summary>
		public extern float startSpeed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The initial size of particles when emitted. When using curves, this values acts as a scale on the curve.</para>
		/// </summary>
		public extern float startSize
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The initial color of particles when emitted.</para>
		/// </summary>
		public Color startColor
		{
			get
			{
				Color result;
				this.INTERNAL_get_startColor(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_startColor(ref value);
			}
		}

		/// <summary>
		///   <para>The initial rotation of particles when emitted. When using curves, this values acts as a scale on the curve.</para>
		/// </summary>
		public extern float startRotation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The initial 3D rotation of particles when emitted. When using curves, this values acts as a scale on the curves.</para>
		/// </summary>
		public Vector3 startRotation3D
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_startRotation3D(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_startRotation3D(ref value);
			}
		}

		/// <summary>
		///   <para>The total lifetime in seconds that particles will have when emitted. When using curves, this values acts as a scale on the curve. This value is set in the particle when it is create by the particle system.</para>
		/// </summary>
		public extern float startLifetime
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Scale being applied to the gravity defined by Physics.gravity.</para>
		/// </summary>
		public extern float gravityModifier
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The maximum number of particles to emit.</para>
		/// </summary>
		public extern int maxParticles
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>This selects the space in which to simulate particles. It can be either world or local space.</para>
		/// </summary>
		public extern ParticleSystemSimulationSpace simulationSpace
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The scaling mode applied to particle sizes and positions.</para>
		/// </summary>
		public extern ParticleSystemScalingMode scalingMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Random seed used for the particle system emission. If set to 0, it will be assigned a random value on awake.</para>
		/// </summary>
		public extern uint randomSeed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Access the particle system emission module.</para>
		/// </summary>
		public ParticleSystem.EmissionModule emission
		{
			get
			{
				return new ParticleSystem.EmissionModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system shape module.</para>
		/// </summary>
		public ParticleSystem.ShapeModule shape
		{
			get
			{
				return new ParticleSystem.ShapeModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system velocity over lifetime module.</para>
		/// </summary>
		public ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime
		{
			get
			{
				return new ParticleSystem.VelocityOverLifetimeModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system limit velocity over lifetime module.</para>
		/// </summary>
		public ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetime
		{
			get
			{
				return new ParticleSystem.LimitVelocityOverLifetimeModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system velocity inheritance module.</para>
		/// </summary>
		public ParticleSystem.InheritVelocityModule inheritVelocity
		{
			get
			{
				return new ParticleSystem.InheritVelocityModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system force over lifetime module.</para>
		/// </summary>
		public ParticleSystem.ForceOverLifetimeModule forceOverLifetime
		{
			get
			{
				return new ParticleSystem.ForceOverLifetimeModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system color over lifetime module.</para>
		/// </summary>
		public ParticleSystem.ColorOverLifetimeModule colorOverLifetime
		{
			get
			{
				return new ParticleSystem.ColorOverLifetimeModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system color by lifetime module.</para>
		/// </summary>
		public ParticleSystem.ColorBySpeedModule colorBySpeed
		{
			get
			{
				return new ParticleSystem.ColorBySpeedModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system size over lifetime module.</para>
		/// </summary>
		public ParticleSystem.SizeOverLifetimeModule sizeOverLifetime
		{
			get
			{
				return new ParticleSystem.SizeOverLifetimeModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system size by speed module.</para>
		/// </summary>
		public ParticleSystem.SizeBySpeedModule sizeBySpeed
		{
			get
			{
				return new ParticleSystem.SizeBySpeedModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system rotation over lifetime module.</para>
		/// </summary>
		public ParticleSystem.RotationOverLifetimeModule rotationOverLifetime
		{
			get
			{
				return new ParticleSystem.RotationOverLifetimeModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system rotation by speed  module.</para>
		/// </summary>
		public ParticleSystem.RotationBySpeedModule rotationBySpeed
		{
			get
			{
				return new ParticleSystem.RotationBySpeedModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system external forces module.</para>
		/// </summary>
		public ParticleSystem.ExternalForcesModule externalForces
		{
			get
			{
				return new ParticleSystem.ExternalForcesModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system collision module.</para>
		/// </summary>
		public ParticleSystem.CollisionModule collision
		{
			get
			{
				return new ParticleSystem.CollisionModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system sub emitters module.</para>
		/// </summary>
		public ParticleSystem.SubEmittersModule subEmitters
		{
			get
			{
				return new ParticleSystem.SubEmittersModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system texture sheet animation module.</para>
		/// </summary>
		public ParticleSystem.TextureSheetAnimationModule textureSheetAnimation
		{
			get
			{
				return new ParticleSystem.TextureSheetAnimationModule(this);
			}
		}

		[Obsolete("safeCollisionEventSize has been deprecated. Use GetSafeCollisionEventSize() instead (UnityUpgradable) -> ParticlePhysicsExtensions.GetSafeCollisionEventSize(UnityEngine.ParticleSystem)", false)]
		public int safeCollisionEventSize
		{
			get
			{
				return ParticleSystemExtensionsImpl.GetSafeCollisionEventSize(this);
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_startColor(out Color value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_startColor(ref Color value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_startRotation3D(out Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_startRotation3D(ref Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetParticles(ParticleSystem.Particle[] particles, int size);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetParticles(ParticleSystem.Particle[] particles);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Simulate(ParticleSystem self, float t, bool restart);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Play(ParticleSystem self);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Stop(ParticleSystem self);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Pause(ParticleSystem self);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Clear(ParticleSystem self);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_IsAlive(ParticleSystem self);

		[ExcludeFromDocs]
		public void Simulate(float t, bool withChildren)
		{
			bool restart = true;
			this.Simulate(t, withChildren, restart);
		}

		[ExcludeFromDocs]
		public void Simulate(float t)
		{
			bool restart = true;
			bool withChildren = true;
			this.Simulate(t, withChildren, restart);
		}

		/// <summary>
		///   <para>Fastforwards the particle system by simulating particles over given period of time, then pauses it.</para>
		/// </summary>
		/// <param name="t">Time to fastforward the particle system.</param>
		/// <param name="withChildren">Fastforward all child particle systems as well.</param>
		/// <param name="restart">Restart and start from the beginning.</param>
		public void Simulate(float t, [DefaultValue("true")] bool withChildren, [DefaultValue("true")] bool restart)
		{
			this.IterateParticleSystems(withChildren, (ParticleSystem ps) => ParticleSystem.Internal_Simulate(ps, t, restart));
		}

		[ExcludeFromDocs]
		public void Play()
		{
			bool withChildren = true;
			this.Play(withChildren);
		}

		/// <summary>
		///   <para>Plays the particle system.</para>
		/// </summary>
		/// <param name="withChildren">Play all child particle systems as well.</param>
		public void Play([DefaultValue("true")] bool withChildren)
		{
			this.IterateParticleSystems(withChildren, (ParticleSystem ps) => ParticleSystem.Internal_Play(ps));
		}

		[ExcludeFromDocs]
		public void Stop()
		{
			bool withChildren = true;
			this.Stop(withChildren);
		}

		/// <summary>
		///   <para>Stops playing the particle system.</para>
		/// </summary>
		/// <param name="withChildren">Stop all child particle systems as well.</param>
		public void Stop([DefaultValue("true")] bool withChildren)
		{
			this.IterateParticleSystems(withChildren, (ParticleSystem ps) => ParticleSystem.Internal_Stop(ps));
		}

		[ExcludeFromDocs]
		public void Pause()
		{
			bool withChildren = true;
			this.Pause(withChildren);
		}

		/// <summary>
		///   <para>Pauses playing the particle system.</para>
		/// </summary>
		/// <param name="withChildren">Pause all child particle systems as well.</param>
		public void Pause([DefaultValue("true")] bool withChildren)
		{
			this.IterateParticleSystems(withChildren, (ParticleSystem ps) => ParticleSystem.Internal_Pause(ps));
		}

		[ExcludeFromDocs]
		public void Clear()
		{
			bool withChildren = true;
			this.Clear(withChildren);
		}

		/// <summary>
		///   <para>Remove all particles in the particle system.</para>
		/// </summary>
		/// <param name="withChildren">Clear all child particle systems as well.</param>
		public void Clear([DefaultValue("true")] bool withChildren)
		{
			this.IterateParticleSystems(withChildren, (ParticleSystem ps) => ParticleSystem.Internal_Clear(ps));
		}

		[ExcludeFromDocs]
		public bool IsAlive()
		{
			bool withChildren = true;
			return this.IsAlive(withChildren);
		}

		/// <summary>
		///   <para>Does the system have any live particles (or will produce more)?</para>
		/// </summary>
		/// <param name="withChildren">Check all child particle systems as well.</param>
		/// <returns>
		///   <para>True if the particle system is still "alive", false if the particle system is done emitting particles and all particles are dead.</para>
		/// </returns>
		public bool IsAlive([DefaultValue("true")] bool withChildren)
		{
			return this.IterateParticleSystems(withChildren, (ParticleSystem ps) => ParticleSystem.Internal_IsAlive(ps));
		}

		/// <summary>
		///   <para>Emit count particles immediately.</para>
		/// </summary>
		/// <param name="count">Number of particles to emit.</param>
		public void Emit(int count)
		{
			ParticleSystem.INTERNAL_CALL_Emit(this, count);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Emit(ParticleSystem self, int count);

		/// <summary>
		///   <para></para>
		/// </summary>
		/// <param name="position"></param>
		/// <param name="velocity"></param>
		/// <param name="size"></param>
		/// <param name="lifetime"></param>
		/// <param name="color"></param>
		[Obsolete("Emit with specific parameters is deprecated. Pass a ParticleSystem.EmitParams parameter instead, which allows you to override some/all of the emission properties")]
		public void Emit(Vector3 position, Vector3 velocity, float size, float lifetime, Color32 color)
		{
			ParticleSystem.Particle particle = default(ParticleSystem.Particle);
			particle.position = position;
			particle.velocity = velocity;
			particle.lifetime = lifetime;
			particle.startLifetime = lifetime;
			particle.startSize = size;
			particle.rotation3D = Vector3.zero;
			particle.angularVelocity3D = Vector3.zero;
			particle.startColor = color;
			particle.randomSeed = 5u;
			this.Internal_EmitOld(ref particle);
		}

		[Obsolete("Emit with a single particle structure is deprecated. Pass a ParticleSystem.EmitParams parameter instead, which allows you to override some/all of the emission properties")]
		public void Emit(ParticleSystem.Particle particle)
		{
			this.Internal_EmitOld(ref particle);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_EmitOld(ref ParticleSystem.Particle particle);

		public void Emit(ParticleSystem.EmitParams emitParams, int count)
		{
			this.Internal_Emit(ref emitParams, count);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_Emit(ref ParticleSystem.EmitParams emitParams, int count);

		internal bool IterateParticleSystems(bool recurse, ParticleSystem.IteratorDelegate func)
		{
			bool flag = func(this);
			if (recurse)
			{
				flag |= ParticleSystem.IterateParticleSystemsRecursive(base.transform, func);
			}
			return flag;
		}

		private static bool IterateParticleSystemsRecursive(Transform transform, ParticleSystem.IteratorDelegate func)
		{
			bool flag = false;
			foreach (Transform transform2 in transform)
			{
				ParticleSystem component = transform2.gameObject.GetComponent<ParticleSystem>();
				if (component != null)
				{
					flag = func(component);
					if (flag)
					{
						break;
					}
					ParticleSystem.IterateParticleSystemsRecursive(transform2, func);
				}
			}
			return flag;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetupDefaultType(int type);
	}
}
