namespace ScuffedVideoPlayer.Commands
{
    using System;
    using System.Linq;
    using CommandSystem;
    using MEC;
    using NWAPIPermissionSystem;
    using PluginAPI.Core;
    using ScuffedVideoPlayer.API;
    using ScuffedVideoPlayer.Audio;
    using ScuffedVideoPlayer.Output;
    using ScuffedVideoPlayer.Output.Displays;
    using Plugin = ScuffedVideoPlayer.Plugin;

    public class PlayCommand : ICommand
    {
        private static CoroutineHandle handle;
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("videoplayer.play"))
            {
                response = "You do not have permission to use this command (videoplayer.play).";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "Usage: playvideo <video name>";
                return false;
            }

            var videoNameOrId = arguments.At(0);
            LoadedVideo videoResult = null!;
            if (int.TryParse(videoNameOrId, out var id))
            {
                if (id < 1 || id > Plugin.Videos.Count)
                {
                    response = $"Video with id {id} not found.";
                    return false;
                }
                videoResult = Plugin.Videos.Values.ElementAt(id-1);
            }
            else
            {
                if (videoNameOrId.StartsWith("\"") && arguments.Count > 1)
                {
                    int closingIndex;
                    if (arguments.Last().EndsWith("\""))
                        closingIndex = arguments.Count - 1;
                    else
                    {
                        closingIndex = arguments.ToList().FindIndex(1, s => s.EndsWith("\""));
                        if (closingIndex == -1)
                        {
                            response = "Missing closing quotes";
                            return false;
                        }
                    }

                    videoNameOrId = videoNameOrId.Substring(1) + " " +
                                    string.Join(" ", arguments.Skip(1).TakeWhile((_, i) => i <= closingIndex));
                    videoNameOrId = videoNameOrId.Substring(0, videoNameOrId.Length - 1);
                }

                if (!Plugin.Videos.TryGetValue(videoNameOrId, out var video))
                {
                    response = $"Video with name \"{videoNameOrId}\" not found.";
                    return false;
                }
            }
            if (IntercomDisplay.Instance.PlaybackHandle?.IsPlaying ?? false)
            {
                response = "The intercom is already playing a video.";
                return false;
            }
            var audioNpc = AudioNpc.Create(IntercomDisplay.Instance.Position);
            IntercomDisplay.Instance.PlaybackHandle = new PlaybackHandle(
                Timing.RunCoroutine(Plugin.PlaybackCoroutine(IntercomDisplay.Instance, videoResult, audioNpc))
                , audioNpc);

            response = "Done.";
            return true;
        }

        public string Command { get; } = "play";
        public string[] Aliases { get; } = { "p" };
        public string Description { get; } = "Plays a video on the intercom.";
    }
}