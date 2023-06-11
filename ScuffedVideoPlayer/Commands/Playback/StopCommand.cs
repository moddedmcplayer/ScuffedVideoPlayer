namespace ScuffedVideoPlayer.Commands.Playback
{
    using System;
    using CommandSystem;
    using PluginAPI.Core;

    public class StopCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!SelectCommand.SelectedDisplays.TryGetValue(Player.Get(sender).UserId, out var display) || display == null)
            {
                response = "You must select a display first (vp select <id>).";
                return false;
            }

            if (!display.PlaybackHandle?.IsPlaying ?? true)
            {
                response = "Playback is not active.";
                return false;
            }

            display.Dispose();

            response = "Stopped playback.";
            return true;
        }

        public string Command { get; } = "stop";
        public string[] Aliases { get; } = { "s" };
        public string Description { get; } = "Stops playback and frees resources.";
    }
}