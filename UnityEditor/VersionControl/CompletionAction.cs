namespace UnityEditor.VersionControl
{
    using System;

    /// <summary>
    /// <para>Different actions a version control task can do upon completion.</para>
    /// </summary>
    public enum CompletionAction
    {
        /// <summary>
        /// <para>Refresh windows upon task completion.</para>
        /// </summary>
        OnAddedChangeWindow = 7,
        /// <summary>
        /// <para>Update the content of a pending changeset with the result of the task upon completion.</para>
        /// </summary>
        OnChangeContentsPendingWindow = 2,
        /// <summary>
        /// <para>Update the pending changesets with the result of the task upon completion.</para>
        /// </summary>
        OnChangeSetsPendingWindow = 4,
        /// <summary>
        /// <para>Show or update the checkout failure window.</para>
        /// </summary>
        OnCheckoutCompleted = 8,
        /// <summary>
        /// <para>Refreshes the incoming and pensing changes window upon task completion.</para>
        /// </summary>
        OnGotLatestPendingWindow = 5,
        /// <summary>
        /// <para>Update incoming changes window with the result of the task upon completion.</para>
        /// </summary>
        OnIncomingPendingWindow = 3,
        /// <summary>
        /// <para>Refresh the submit window with the result of the task upon completion.</para>
        /// </summary>
        OnSubmittedChangeWindow = 6,
        /// <summary>
        /// <para>Update the list of pending changes when a task completes.</para>
        /// </summary>
        UpdatePendingWindow = 1
    }
}

