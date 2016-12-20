namespace UnityEditor.Android.PostProcessor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEngine;

    internal class PostProcessRunner
    {
        private float _progressValue;
        private List<IPostProcessorTask> _tasks = new List<IPostProcessorTask>();

        public void AddNextTask(IPostProcessorTask task)
        {
            task.OnProgress += new ProgressHandler(this.ShowProgress);
            this._tasks.Add(task);
        }

        public void RunAllTasks(PostProcessorContext context)
        {
            int count = this._tasks.Count;
            int num2 = 1;
            try
            {
                foreach (IPostProcessorTask task in this._tasks)
                {
                    this._progressValue = ((float) num2++) / ((float) count);
                    task.Execute(context);
                }
            }
            catch (CommandInvokationFailure failure)
            {
                foreach (string str in failure.Errors)
                {
                    Debug.LogError(str + "\n");
                }
                CancelPostProcess.AbortBuild("Build failure", failure.HighLevelMessage + CancelPostProcess.ConsoleMessage, failure);
            }
            catch (ProcessAbortedException exception)
            {
                Debug.LogWarning("Build process aborted (" + exception.Message + ")\n");
            }
        }

        private void ShowProgress(IPostProcessorTask task, string message)
        {
            if (EditorUtility.DisplayCancelableProgressBar(task.Name, message, this._progressValue))
            {
                throw new ProcessAbortedException(task.Name);
            }
        }
    }
}

