namespace UnityEngine.SocialPlatforms
{
    using System;

    public interface IAchievement
    {
        void ReportProgress(Action<bool> callback);

        /// <summary>
        /// <para>Set to true when percentCompleted is 100.0.</para>
        /// </summary>
        bool completed { get; }

        /// <summary>
        /// <para>This achievement is currently hidden from the user.</para>
        /// </summary>
        bool hidden { get; }

        /// <summary>
        /// <para>The unique identifier of this achievement.</para>
        /// </summary>
        string id { get; set; }

        /// <summary>
        /// <para>Set by server when percentCompleted is updated.</para>
        /// </summary>
        DateTime lastReportedDate { get; }

        /// <summary>
        /// <para>Progress for this achievement.</para>
        /// </summary>
        double percentCompleted { get; set; }
    }
}

