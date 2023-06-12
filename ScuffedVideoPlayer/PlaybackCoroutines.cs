namespace ScuffedVideoPlayer
{
    using System.Collections.Generic;
    using System.Linq;
    using MEC;
    using PluginAPI.Core;
    using ScuffedVideoPlayer.API;
    using ScuffedVideoPlayer.Audio;
    using ScuffedVideoPlayer.Output;
    using ScuffedVideoPlayer.Output.Displays;
    using ScuffedVideoPlayer.Playback;
    using UnityEngine;
    using VoiceChat;

    public static class PlaybackCoroutines
    {
        public static IEnumerator<float> StandardPlaybackCoroutine(IDisplay display, LoadedVideo video, AudioNpc? audioNpc, bool destroyNpcOnFinish = true)
        {
            yield return Timing.WaitForSeconds(1f); // wait for audio
            var frames = video.Frames; // load video so fps is not 0
            float delay = 1.0f / video.FramesPerSecond;
            if (display.Paused)
                yield return Timing.WaitUntilFalse(() => display.Paused);
            if (audioNpc != null && video.AudioFile != null)
            {
                audioNpc.Play(video.AudioFile);
            }
            else
            {
                audioNpc = null;
                Log.Warning($"Playing track {Plugin.Videos.FirstOrDefault(x => x.Value == video).Key ?? "null"} without audio");
            }

            Color?[,]? lastFrame = null;
            foreach (var frame in frames)
            {
                if (display.Paused)
                {
                    if (audioNpc == null)
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        yield return Timing.WaitUntilFalse(() => display.Paused);
                    }
                    else
                    {
                        audioNpc.IsPaused = true;
                        // ReSharper disable once AccessToDisposedClosure
                        yield return Timing.WaitUntilFalse(() => display.Paused);
                        audioNpc.IsPaused = false;
                    }
                }

                if (display is ITextDisplay textDisplay)
                {
                    var text = TextPlayback.ImageToText(frame);
                    textDisplay.SetText(text);
                }

                if (display is PrimitiveDisplay primitiveDisplay)
                {
#pragma warning disable CS8602
                    var colorArr = PrimitivePlayback.ImageToColor(frame, lastFrame);
                    lastFrame = colorArr;
                    primitiveDisplay.SetColor(colorArr);
#pragma warning restore CS8602
                }

                yield return Timing.WaitForSeconds(delay);
            }
            display.Clear();

            if (audioNpc is { IsPlaying: true })
                yield return Timing.WaitUntilFalse(() => audioNpc.IsPlaying);
            if (destroyNpcOnFinish)
                audioNpc?.Destroy();
        }

        public static IEnumerator<float> PlayerPlaybackCoroutine(PlayerDisplay display, LoadedVideo video, AudioNpc? audioNpc, bool destroyNpcOnFinish = true)
        {
            yield return Timing.WaitForSeconds(1f); // wait for audio
            var frames = video.Frames; // load video so fps is not 0
            float delay = 1.0f / video.FramesPerSecond;
            if (display.Paused)
                yield return Timing.WaitUntilFalse(() => display.Paused);
            if (audioNpc != null && video.AudioFile != null)
            {
                audioNpc.Play(video.AudioFile, VoiceChatChannel.RoundSummary);
            }
            else
            {
                audioNpc = null;
                Log.Warning($"Player playing track {Plugin.Videos.FirstOrDefault(x => x.Value == video).Key ?? "null"} without audio");
            }

            foreach (var frame in frames)
            {
                if (display.Paused)
                {
                    if (audioNpc == null)
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        yield return Timing.WaitUntilFalse(() => display.Paused);
                    }
                    else
                    {
                        audioNpc.IsPaused = true;
                        // ReSharper disable once AccessToDisposedClosure
                        yield return Timing.WaitUntilFalse(() => display.Paused);
                        audioNpc.IsPaused = false;
                    }
                }

                var text = TextPlayback.ImageToText(frame);
                display.SetText(text);
                yield return Timing.WaitForSeconds(delay);
            }
            display.Clear();

            if (audioNpc is { IsPlaying: true })
                yield return Timing.WaitUntilFalse(() => audioNpc.IsPlaying);
            if (destroyNpcOnFinish)
                audioNpc?.Destroy();
        }
    }
}