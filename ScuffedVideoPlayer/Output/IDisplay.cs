namespace ScuffedVideoPlayer.Output
{
    using UnityEngine;

    public interface IDisplay
    {
        bool Paused { get; set; }
        Vector3 Position { get; }
        PlaybackHandle? PlaybackHandle { get; set; }
    }
}