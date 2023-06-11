namespace ScuffedVideoPlayer.Output
{
    using System;
    using MEC;
    using PluginAPI.Core;
    using ScuffedVideoPlayer.Audio;

    public class PlaybackHandle : IDisposable
    {
        public CoroutineHandle CoroutineHandle { get; }
        public AudioNpc? AudioNpc { get; }

        public bool IsPlaying => Timing.IsRunning(CoroutineHandle) || (AudioNpc?.IsPlaying ?? false);

        public void Dispose()
        {
            try
            {
                AudioNpc?.Destroy();
            }
            catch (Exception e)
            {
                Log.Debug($"Failed to destroy audio npc: {e}");
            }
            Timing.KillCoroutines(CoroutineHandle);
        }

        public PlaybackHandle(CoroutineHandle coroutineHandle, AudioNpc audioNpc)
        {
            CoroutineHandle = coroutineHandle;
            AudioNpc = audioNpc;
        }
    }
}