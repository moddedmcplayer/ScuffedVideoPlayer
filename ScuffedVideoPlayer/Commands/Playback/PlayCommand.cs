namespace ScuffedVideoPlayer.Commands.Playback
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
    using UnityEngine;
    using Plugin = ScuffedVideoPlayer.Plugin;

    public class PlayCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("videoplayer.play"))
            {
                response = "You do not have permission to use this command (videoplayer.play).";
                return false;
            }

            if (!SelectCommand.SelectedDisplays.TryGetValue(Player.Get(sender).UserId, out var display) || display == null)
            {
                response = "You must select a display first (vp select <id>).";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "Usage: playvideo <video name>";
                return false;
            }

            var videoNameOrId = arguments.At(0);
            LoadedVideo videoResult;
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

                if (!Plugin.Videos.TryGetValue(videoNameOrId, out videoResult))
                {
                    response = $"Video with name \"{videoNameOrId}\" not found.";
                    return false;
                }
            }

            if (display.PlaybackHandle?.IsPlaying ?? false)
            {
                response = "The display is already playing a video.";
                return false;
            }

            if (display is PrimitiveDisplay primitiveDisplay)
            {
                var firstFrame = videoResult.Frames.First();
                if (firstFrame.Width != primitiveDisplay.Resolution.Item1 || firstFrame.Height != primitiveDisplay.Resolution.Item2)
                {
                    response = $"Resolution mismatch! ({firstFrame.Width}x{firstFrame.Height} vs {primitiveDisplay.Resolution.Item1}x{primitiveDisplay.Resolution.Item2})";
                    return false;
                }
            }

            AudioNpc audioNpc;
            if (display is not PlayerDisplay playerDisplay)
            {
                audioNpc = AudioNpc.Create(display.Position);
                    display.PlaybackHandle = new PlaybackHandle(
                        Timing.RunCoroutine(PlaybackCoroutines.StandardPlaybackCoroutine(display, videoResult, audioNpc))
                        , audioNpc);
            }
            else
            {
                audioNpc = AudioNpc.Create(Vector3.zero);
                    display.PlaybackHandle = new PlaybackHandle(
                        Timing.RunCoroutine(PlaybackCoroutines.PlayerPlaybackCoroutine(playerDisplay, videoResult, audioNpc))
                        , audioNpc);
            }

            response = "Done.";
            return true;
        }

        public string Command { get; } = "play";
        public string[] Aliases { get; } = { "p" };
        public string Description { get; } = "Plays a video.";
    }
}