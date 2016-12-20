﻿namespace UnityEngine.SocialPlatforms
{
    using System;

    public interface ILocalUser : IUserProfile
    {
        void Authenticate(Action<bool> callback);
        void Authenticate(Action<bool, string> callback);
        void LoadFriends(Action<bool> callback);

        /// <summary>
        /// <para>Checks if the current user has been authenticated.</para>
        /// </summary>
        bool authenticated { get; }

        /// <summary>
        /// <para>The users friends list.</para>
        /// </summary>
        IUserProfile[] friends { get; }

        /// <summary>
        /// <para>Is the user underage?</para>
        /// </summary>
        bool underage { get; }
    }
}

