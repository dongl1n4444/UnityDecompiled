namespace UnityEditor.PlaymodeTestsRunner.TestLauncher
{
    using System;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    internal class EditModeLauncherContextSettings : IDisposable
    {
        private bool m_RunInBackground;
        private int m_UndoGroup = -1;

        public EditModeLauncherContextSettings()
        {
            this.SetupProjectParameters();
        }

        private void CleanupProjectParameters()
        {
            PerformUndo(this.m_UndoGroup);
            Application.runInBackground = this.m_RunInBackground;
        }

        public void Dispose()
        {
            this.CleanupProjectParameters();
        }

        private Scene? OpenNewScene()
        {
            string operation = "Current scene is not saved. Do you want to save it?\n\n(You can disable this prompt in the Tests Runner's options)";
            if (string.IsNullOrEmpty(SceneManager.GetActiveScene().path) && ((operation != "") || !EditorSceneManager.EnsureUntitledSceneHasBeenSaved(operation)))
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                return null;
            }
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            return new Scene?(scene);
        }

        private static void PerformUndo(int undoGroup)
        {
            EditorUtility.DisplayProgressBar("Undo", "Reverting changes to the scene", 0f);
            DateTime now = DateTime.Now;
            Undo.RevertAllDownToGroup(undoGroup);
            TimeSpan span = (TimeSpan) (DateTime.Now - now);
            if (span.TotalSeconds > 1.0)
            {
                TimeSpan span2 = (TimeSpan) (DateTime.Now - now);
                Debug.LogWarning("Undo after editor test run took " + span2.Seconds + " seconds.");
            }
            EditorUtility.ClearProgressBar();
        }

        private static int RegisterUndo() => 
            Undo.GetCurrentGroup();

        private void SetupProjectParameters()
        {
            this.m_RunInBackground = Application.runInBackground;
            Application.runInBackground = true;
            this.m_UndoGroup = RegisterUndo();
        }
    }
}

