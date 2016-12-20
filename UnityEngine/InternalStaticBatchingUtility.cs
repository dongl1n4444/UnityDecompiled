namespace UnityEngine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal class InternalStaticBatchingUtility
    {
        [CompilerGenerated]
        private static Func<Material, bool> <>f__am$cache0;
        private const string CombinedMeshPrefix = "Combined Mesh";
        private const int MaxVerticesInBatch = 0xfa00;

        public static void Combine(GameObject staticBatchRoot, bool combineOnlyStatic, bool isEditorPostprocessScene)
        {
            GameObject[] objArray = (GameObject[]) UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject obj2 in objArray)
            {
                if (((staticBatchRoot == null) || obj2.transform.IsChildOf(staticBatchRoot.transform)) && (!combineOnlyStatic || obj2.isStaticBatchable))
                {
                    list.Add(obj2);
                }
            }
            CombineGameObjects(list.ToArray(), staticBatchRoot, isEditorPostprocessScene);
        }

        public static void CombineGameObjects(GameObject[] gos, GameObject staticBatchRoot, bool isEditorPostprocessScene)
        {
            Matrix4x4 identity = Matrix4x4.identity;
            Transform staticBatchRootTransform = null;
            if (staticBatchRoot != null)
            {
                identity = staticBatchRoot.transform.worldToLocalMatrix;
                staticBatchRootTransform = staticBatchRoot.transform;
            }
            int batchIndex = 0;
            int num2 = 0;
            List<MeshSubsetCombineUtility.MeshContainer> meshes = new List<MeshSubsetCombineUtility.MeshContainer>();
            Array.Sort(gos, new SortGO());
            foreach (GameObject obj2 in gos)
            {
                MeshFilter component = obj2.GetComponent(typeof(MeshFilter)) as MeshFilter;
                if (component != null)
                {
                    Mesh sharedMesh = component.sharedMesh;
                    if ((sharedMesh != null) && (isEditorPostprocessScene || sharedMesh.canAccess))
                    {
                        Renderer context = component.GetComponent<Renderer>();
                        if (((context != null) && context.enabled) && (context.staticBatchIndex == 0))
                        {
                            Material[] sharedMaterials = context.sharedMaterials;
                            if (<>f__am$cache0 == null)
                            {
                                <>f__am$cache0 = new Func<Material, bool>(null, (IntPtr) <CombineGameObjects>m__0);
                            }
                            if (!Enumerable.Any<Material>(sharedMaterials, <>f__am$cache0))
                            {
                                int vertexCount = sharedMesh.vertexCount;
                                if (vertexCount != 0)
                                {
                                    MeshRenderer renderer2 = context as MeshRenderer;
                                    if (((renderer2 == null) || (renderer2.additionalVertexStreams == null)) || (vertexCount == renderer2.additionalVertexStreams.vertexCount))
                                    {
                                        if ((num2 + vertexCount) > 0xfa00)
                                        {
                                            MakeBatch(meshes, staticBatchRootTransform, batchIndex++);
                                            meshes.Clear();
                                            num2 = 0;
                                        }
                                        MeshSubsetCombineUtility.MeshInstance instance = new MeshSubsetCombineUtility.MeshInstance {
                                            meshInstanceID = sharedMesh.GetInstanceID(),
                                            rendererInstanceID = context.GetInstanceID()
                                        };
                                        if ((renderer2 != null) && (renderer2.additionalVertexStreams != null))
                                        {
                                            instance.additionalVertexStreamsMeshInstanceID = renderer2.additionalVertexStreams.GetInstanceID();
                                        }
                                        instance.transform = identity * component.transform.localToWorldMatrix;
                                        instance.lightmapScaleOffset = context.lightmapScaleOffset;
                                        instance.realtimeLightmapScaleOffset = context.realtimeLightmapScaleOffset;
                                        MeshSubsetCombineUtility.MeshContainer item = new MeshSubsetCombineUtility.MeshContainer {
                                            gameObject = obj2,
                                            instance = instance,
                                            subMeshInstances = new List<MeshSubsetCombineUtility.SubMeshInstance>()
                                        };
                                        meshes.Add(item);
                                        if (sharedMaterials.Length > sharedMesh.subMeshCount)
                                        {
                                            Debug.LogWarning(string.Concat(new object[] { "Mesh '", sharedMesh.name, "' has more materials (", sharedMaterials.Length, ") than subsets (", sharedMesh.subMeshCount, ")" }), context);
                                            Material[] materialArray2 = new Material[sharedMesh.subMeshCount];
                                            for (int j = 0; j < sharedMesh.subMeshCount; j++)
                                            {
                                                materialArray2[j] = context.sharedMaterials[j];
                                            }
                                            context.sharedMaterials = materialArray2;
                                            sharedMaterials = materialArray2;
                                        }
                                        for (int i = 0; i < Math.Min(sharedMaterials.Length, sharedMesh.subMeshCount); i++)
                                        {
                                            MeshSubsetCombineUtility.SubMeshInstance instance2 = new MeshSubsetCombineUtility.SubMeshInstance {
                                                meshInstanceID = component.sharedMesh.GetInstanceID(),
                                                vertexOffset = num2,
                                                subMeshIndex = i,
                                                gameObjectInstanceID = obj2.GetInstanceID(),
                                                transform = instance.transform
                                            };
                                            item.subMeshInstances.Add(instance2);
                                        }
                                        num2 += sharedMesh.vertexCount;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            MakeBatch(meshes, staticBatchRootTransform, batchIndex);
        }

        public static void CombineRoot(GameObject staticBatchRoot)
        {
            Combine(staticBatchRoot, false, false);
        }

        private static void MakeBatch(List<MeshSubsetCombineUtility.MeshContainer> meshes, Transform staticBatchRootTransform, int batchIndex)
        {
            if (meshes.Count >= 2)
            {
                List<MeshSubsetCombineUtility.MeshInstance> list = new List<MeshSubsetCombineUtility.MeshInstance>();
                List<MeshSubsetCombineUtility.SubMeshInstance> list2 = new List<MeshSubsetCombineUtility.SubMeshInstance>();
                foreach (MeshSubsetCombineUtility.MeshContainer container in meshes)
                {
                    list.Add(container.instance);
                    list2.AddRange(container.subMeshInstances);
                }
                string meshName = "Combined Mesh";
                meshName = meshName + " (root: " + ((staticBatchRootTransform == null) ? "scene" : staticBatchRootTransform.name) + ")";
                if (batchIndex > 0)
                {
                    meshName = meshName + " " + (batchIndex + 1);
                }
                Mesh combinedMesh = StaticBatchingUtility.InternalCombineVertices(list.ToArray(), meshName);
                StaticBatchingUtility.InternalCombineIndices(list2.ToArray(), combinedMesh);
                int firstSubMesh = 0;
                foreach (MeshSubsetCombineUtility.MeshContainer container2 in meshes)
                {
                    MeshFilter component = (MeshFilter) container2.gameObject.GetComponent(typeof(MeshFilter));
                    component.sharedMesh = combinedMesh;
                    int subMeshCount = Enumerable.Count<MeshSubsetCombineUtility.SubMeshInstance>(container2.subMeshInstances);
                    Renderer renderer = container2.gameObject.GetComponent<Renderer>();
                    renderer.SetStaticBatchInfo(firstSubMesh, subMeshCount);
                    renderer.staticBatchRootTransform = staticBatchRootTransform;
                    renderer.enabled = false;
                    renderer.enabled = true;
                    MeshRenderer renderer2 = renderer as MeshRenderer;
                    if (renderer2 != null)
                    {
                        renderer2.additionalVertexStreams = null;
                    }
                    firstSubMesh += subMeshCount;
                }
            }
        }

        internal class SortGO : IComparer
        {
            private static int GetLightmapIndex(Renderer renderer)
            {
                if (renderer == null)
                {
                    return -1;
                }
                return renderer.lightmapIndex;
            }

            private static int GetMaterialId(Renderer renderer)
            {
                if ((renderer == null) || (renderer.sharedMaterial == null))
                {
                    return 0;
                }
                return renderer.sharedMaterial.GetInstanceID();
            }

            private static Renderer GetRenderer(GameObject go)
            {
                if (go == null)
                {
                    return null;
                }
                MeshFilter component = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
                if (component == null)
                {
                    return null;
                }
                return component.GetComponent<Renderer>();
            }

            int IComparer.Compare(object a, object b)
            {
                if (a == b)
                {
                    return 0;
                }
                Renderer renderer = GetRenderer(a as GameObject);
                Renderer renderer2 = GetRenderer(b as GameObject);
                int num2 = GetMaterialId(renderer).CompareTo(GetMaterialId(renderer2));
                if (num2 == 0)
                {
                    num2 = GetLightmapIndex(renderer).CompareTo(GetLightmapIndex(renderer2));
                }
                return num2;
            }
        }
    }
}

