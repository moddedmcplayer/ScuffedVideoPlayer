namespace ScuffedVideoPlayer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AdminToys;
    using HarmonyLib;
    using MEC;
    using Mirror;
    using PluginAPI.Core;
    using PluginAPI.Core.Attributes;
    using PluginAPI.Enums;
    using PluginAPI.Events;
    using PluginAPI.Helpers;
    using ScuffedVideoPlayer.API;
    using ScuffedVideoPlayer.Audio;
    using ScuffedVideoPlayer.Commands;
    using ScuffedVideoPlayer.Commands.Playback;
    using ScuffedVideoPlayer.Output;
    using ScuffedVideoPlayer.Output.Displays;
    using ScuffedVideoPlayer.Playback;
    using UnityEngine;
    using VoiceChat;

    public class Plugin
    {
        public static readonly Dictionary<string, LoadedVideo> Videos = new Dictionary<string, LoadedVideo>();
        public static string PluginFolder = Path.Combine(Paths.LocalPlugins.Plugins, "ScuffedVideoPlayer");
        public static Dictionary<int, IDisplay> Displays = new Dictionary<int, IDisplay>();
        internal static Queue<int> FreeDisplayIds = new Queue<int>();
        public static int GetDisplayId()
        {
            if (FreeDisplayIds.Count > 0)
                return FreeDisplayIds.Dequeue();
            for (int i = 0; i < 10000; i++)
            {
                if (!Displays.ContainsKey(i))
                {
                    return i;
                }
            }
            return -1;
        }

        [PluginConfig]
        public Config Config;

        public static Plugin? Instance { get; private set; }
        private Harmony? _harmony;

        [PluginEntryPoint("ScuffedVideoPlayer", "1.0.3", "scuffed video player", "moddedmcplayer")]
        private void OnEnabled()
        {
            Instance = this;
            _harmony = new Harmony("moddedmcplayer.scuffedvideoplayer");
            _harmony.PatchAll();
            EventManager.RegisterEvents(this);
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
            EventManager.UnregisterEvents(this);
            _harmony?.UnpatchAll(_harmony.Id);
            _harmony = null;
            Instance = null;
        }

        [PluginEvent(ServerEventType.WaitingForPlayers)]
        public void OnWaitingForPlayers()
        {
            Displays.Clear();
            PrimitiveDisplay.Instances.Clear();
            SelectCommand.SelectedDisplays.Clear();
            Displays.Add(0, IntercomDisplay.Instance);
            foreach (var go in NetworkClient.prefabs.Values)
            {
                if (go.TryGetComponent(out PrimitiveObjectToy component))
                {
                    PrimitiveDisplay.PrimitiveObjectToy = component;
                    break;
                }
            }
        }
    }
}