namespace UnityEngine.SocialPlatforms
{
    using System;

    public interface ISocialPlatform
    {
        void Authenticate(ILocalUser user, Action<bool> callback);
        void Authenticate(ILocalUser user, Action<bool, string> callback);
        /// <summary>
        /// <para>See Social.CreateAchievement..</para>
        /// </summary>
        IAchievement CreateAchievement();
        /// <summary>
        /// <para>See Social.CreateLeaderboard.</para>
        /// </summary>
        ILeaderboard CreateLeaderboard();
        bool GetLoading(ILeaderboard board);
        void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback);
        void LoadAchievements(Action<IAchievement[]> callback);
        void LoadFriends(ILocalUser user, Action<bool> callback);
        void LoadScores(string leaderboardID, Action<IScore[]> callback);
        void LoadScores(ILeaderboard board, Action<bool> callback);
        void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback);
        void ReportProgress(string achievementID, double progress, Action<bool> callback);
        void ReportScore(long score, string board, Action<bool> callback);
        /// <summary>
        /// <para>See Social.ShowAchievementsUI.</para>
        /// </summary>
        void ShowAchievementsUI();
        /// <summary>
        /// <para>See Social.ShowLeaderboardUI.</para>
        /// </summary>
        void ShowLeaderboardUI();

        /// <summary>
        /// <para>See Social.localUser.</para>
        /// </summary>
        ILocalUser localUser { get; }
    }
}

