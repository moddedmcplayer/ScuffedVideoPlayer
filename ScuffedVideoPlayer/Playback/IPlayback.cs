namespace ScuffedVideoPlayer.Playback
{
    using System.Collections.Generic;
    using ScuffedVideoPlayer.API;

    public interface IPlayback<T>
    {
        public IEnumerator<T> Play(LoadedVideo video);
    }
}