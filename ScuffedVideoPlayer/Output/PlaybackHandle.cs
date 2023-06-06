namespace ScuffedVideoPlayer.Output
{
    using System;
    using MEC;
    using ScuffedVideoPlayer.Audio;

    public class PlaybackHandle : IDisposable
    {
        public CoroutineHandle CoroutineHandle { get; }
        public AudioNpc AudioNpc { get; }

        public bool IsPlaying => Timing.IsRunning(CoroutineHandle) || AudioNpc.IsPlaying;

        public void Dispose()
        {
            AudioNpc.Destroy();
            Timing.KillCoroutines(CoroutineHandle);
        }

        public PlaybackHandle(CoroutineHandle coroutineHandle, AudioNpc audioNpc)
        {
            CoroutineHandle = coroutineHandle;
            AudioNpc = audioNpc;
        }
    }
}