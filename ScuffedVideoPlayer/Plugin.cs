namespace ScuffedVideoPlayer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using MEC;
    using PlayerRoles;
    using PlayerRoles.Voice;
    using PluginAPI.Core;
    using PluginAPI.Core.Attributes;
    using PluginAPI.Events;
    using PluginAPI.Helpers;
    using SCPSLAudioApi.AudioCore;
    using UnityEngine;
    using VoiceChat;
    using Color = System.Drawing.Color;

    public class Plugin
    {
        [PluginConfig]
        public Config Config;

        [PluginEntryPoint("ScuffedVideoPlayer", "1.0.0", "scuffed video player", "moddedmcplayer")]
        private void OnEnabled()
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            SCPSLAudioApi.Startup.SetupDependencies();
            var frames = Path.Combine(folder, "frames");
            var fileNames = Directory.GetFiles(frames);
            framesArray = new Bitmap[fileNames.Length];
            for (var i = 0; i < fileNames.Length; i++)
            {
                var img = fileNames[i];
                using var stream = File.OpenRead(img);
                try
                {
                    framesArray[i] = new Bitmap(Image.FromStream(stream));
                }
                catch (Exception e)
                {
                    Log.Info($"image conversion error of type: {e.GetType()}");
                    // ignored
                } // Win32 error
            }
        }
        static Bitmap[] framesArray;
        static string folder = Path.Combine(Paths.LocalPlugins.Plugins, "scuffedvideoplayer");
        static string audioFile = Path.Combine(folder, "audio.ogg");

        public static CoroutineHandle PlayCoroutineHandle;
        public static IEnumerator<float> PlayCoroutine()
        {
            var host = Player.Get(ReferenceHub.HostHub);
            var hostaudio = AudioPlayerBase.Get(host.ReferenceHub);
            host.SetRole(RoleTypeId.Tutorial);
            host.IsGodModeEnabled = true;
            host.IsNoclipEnabled = true;
            host.Position = IntercomDisplay._singleton.transform.position - Vector3.up * 2;
            hostaudio.Stoptrack(true);
            hostaudio.BroadcastChannel = VoiceChatChannel.Proximity;
            hostaudio.Volume = 100f;
            hostaudio.Enqueue(audioFile, 0);
            hostaudio.Play(0);
            

            foreach (var frame in framesArray)
            {
                var text = ImageToText(frame);
                IntercomDisplay.TrySetDisplay(text);

                yield return Timing.WaitForSeconds(0.1f);
            }
            IntercomDisplay.TrySetDisplay(null);
        }

        public static string ImageToText(Bitmap bitmap)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bitmap.Height; i++)
            {
                sb.Append("<size=33%><line-height=75%>");
                for (int y = 0; y < bitmap.Width; y++)
                {
                    var pixel = bitmap.GetPixel(y, i);
                    sb.Append($"<color={ToHex(pixel)}>█</color>");
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        static string ToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    }
}