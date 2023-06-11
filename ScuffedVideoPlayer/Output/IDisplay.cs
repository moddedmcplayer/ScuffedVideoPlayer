namespace ScuffedVideoPlayer.Output
{
    using System;
    using UnityEngine;

    public interface IDisplay : IDisposable
    {
        bool Paused { get; set; }
        Vector3 Position { get; }
        PlaybackHandle? PlaybackHandle { get; set; }
        int Id { get; }
        void Clear();
    }
}