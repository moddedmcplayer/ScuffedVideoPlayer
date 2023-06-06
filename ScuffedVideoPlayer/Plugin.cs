namespace ScuffedVideoPlayer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using HarmonyLib;
    using MEC;
    using PluginAPI.Core;
    using PluginAPI.Core.Attributes;
    using PluginAPI.Helpers;
    using ScuffedVideoPlayer.API;
    using ScuffedVideoPlayer.Audio;
    using ScuffedVideoPlayer.Output;
    using ScuffedVideoPlayer.Playback;

    public class Plugin
    {
        public static readonly Dictionary<string, LoadedVideo> Videos = new Dictionary<string, LoadedVideo>();
        public static string PluginFolder = Path.Combine(Paths.LocalPlugins.Plugins, "ScuffedVideoPlayer");

        public static IEnumerator<float> PlaybackCoroutine(IDisplay display, LoadedVideo video, AudioNpc? audioNpc, bool destroyNpcOnFinish = true)
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
                Log.Warning($"Playing track {Videos.FirstOrDefault(x => x.Value == video).Key ?? "null"} without audio");
            }
            if (display is ITextDisplay textDisplay)
            {
                foreach (var frame in frames)
                {
                    if (display.Paused)
                    {
                        if (audioNpc == null)
                        {
                            yield return Timing.WaitUntilFalse(() => textDisplay.Paused);
                        }
                        else
                        {
                            audioNpc.IsPaused = true;
                            yield return Timing.WaitUntilFalse(() => textDisplay.Paused);
                            audioNpc.IsPaused = false;
                        }
                    }
                    var text = TextPlayback.ImageToText(frame);
                    textDisplay.SetText(text);
                    yield return Timing.WaitForSeconds(delay);
                }
                textDisplay.Clear();
            }

            if (audioNpc is { IsPlaying: true })
                yield return Timing.WaitUntilFalse(() => audioNpc.IsPlaying);
            audioNpc?.Destroy();
        }

        [PluginConfig]
        public Config Config;

        private Harmony? _harmony;

        [PluginEntryPoint("ScuffedVideoPlayer", "1.0.0", "scuffed video player", "moddedmcplayer")]
        private void OnEnabled()
        {
            _harmony = new Harmony("moddedmcplayer.scuffedvideoplayer");
            _harmony.PatchAll();
            SCPSLAudioApi.Startup.SetupDependencies();
            if (!Directory.Exists(Config.VideoFolder))
                Directory.CreateDirectory(Config.VideoFolder);
            foreach (var zip in Directory.GetFiles(Config.VideoFolder, "*.zip", SearchOption.TopDirectoryOnly))
            {
                VideoExtractor.ExtractFiles(zip, Path.Combine(Config.VideoFolder, Path.GetFileNameWithoutExtension(zip)));
                File.Delete(zip);
            }

            foreach (var videoDir in Directory.GetDirectories(Config.VideoFolder))
            {
                var dirName = videoDir.Split(Path.DirectorySeparatorChar).Last();
                try
                {
                    Videos.Add(dirName, new LoadedVideo(videoDir));
                }
                catch (VideoLoadingException e)
                {
                    Log.Error($"Video is invalid: {dirName}, reason: {e.Message}");
                }
            }
        }

        [PluginUnload]
        private void OnDisabled()
        {
            _harmony?.UnpatchAll(_harmony.Id);
            _harmony = null;
        }
    }
}