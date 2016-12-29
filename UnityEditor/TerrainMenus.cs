namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TerrainMenus
    {
        [MenuItem("GameObject/3D Object/Terrain", false, 0xbb8)]
        private static void CreateTerrain(MenuCommand menuCommand)
        {
            TerrainData asset = new TerrainData {
                heightmapResolution = 0x401,
                size = new Vector3(1000f, 600f, 1000f),
                heightmapResolution = 0x200,
                baseMapResolution = 0x400
            };
            asset.SetDetailResolution(0x400, asset.detailResolutionPerPatch);
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath("Assets/New Terrain.asset"));
            GameObject context = menuCommand.context as GameObject;
            string uniqueNameForSibling = GameObjectUtility.GetUniqueNameForSibling(context?.transform, "Terrain");
            GameObject child = Terrain.CreateTerrainGameObject(asset);
            child.name = uniqueNameForSibling;
            GameObjectUtility.SetParentAndAlign(child, context);
            Selection.activeObject = child;
            Undo.RegisterCreatedObjectUndo(child, "Create terrain");
        }

        internal static void ExportHeightmapRaw()
        {
            TerrainWizard.DisplayTerrainWizard<ExportRawHeightmap>("Export Heightmap", "Export").InitializeDefaults(GetActiveTerrain());
        }

        internal static void Flatten()
        {
            TerrainWizard.DisplayTerrainWizard<FlattenHeightmap>("Flatten Heightmap", "Flatten").InitializeDefaults(GetActiveTerrain());
        }

        private static void FlushHeightmapModification()
        {
            GetActiveTerrain().Flush();
        }

        private static Terrain GetActiveTerrain()
        {
            Object[] filtered = Selection.GetFiltered(typeof(Terrain), SelectionMode.Editable);
            if (filtered.Length != 0)
            {
                return (filtered[0] as Terrain);
            }
            return Terrain.activeTerrain;
        }

        private static TerrainData GetActiveTerrainData()
        {
            if (GetActiveTerrain() != null)
            {
                return GetActiveTerrain().terrainData;
            }
            return null;
        }

        internal static void ImportRaw()
        {
            string path = EditorUtility.OpenFilePanel("Import Raw Heightmap", "", "raw");
            if (path != "")
            {
                TerrainWizard.DisplayTerrainWizard<ImportRawHeightmap>("Import Heightmap", "Import").InitializeImportRaw(GetActiveTerrain(), path);
            }
        }

        internal static void MassPlaceTrees()
        {
            TerrainWizard.DisplayTerrainWizard<PlaceTreeWizard>("Place Trees", "Place").InitializeDefaults(GetActiveTerrain());
        }

        internal static void RefreshPrototypes()
        {
            GetActiveTerrainData().RefreshPrototypes();
            GetActiveTerrain().Flush();
            EditorApplication.SetSceneRepaintDirty();
        }
    }
}

