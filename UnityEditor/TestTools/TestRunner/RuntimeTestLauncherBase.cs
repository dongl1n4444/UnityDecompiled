namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.TestTools.TestRunner;

    internal abstract class RuntimeTestLauncherBase : TestLauncherBase
    {
        protected RuntimeTestLauncherBase()
        {
        }

        protected Scene CreateBootstrapScene(string sceneName, Action<PlaymodeTestsController> runnerSetup)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PlaymodeTestsController controller = new GameObject("Code-based tests runner").AddComponent<PlaymodeTestsController>();
            runnerSetup(controller);
            controller.settings.bootstrapScene = sceneName;
            EditorSceneManager.MarkSceneDirty(scene);
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(scene, sceneName, false);
            return scene;
        }

        public string CreateSceneName() => 
            ("Assets/InitTestScene" + DateTime.Now.Ticks + ".unity");
    }
}

