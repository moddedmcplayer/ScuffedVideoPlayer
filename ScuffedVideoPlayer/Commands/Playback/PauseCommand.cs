namespace ScuffedVideoPlayer.Commands.Playback
{
    using System;
    using CommandSystem;
    using PluginAPI.Core;

    public class PauseCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!SelectCommand.SelectedDisplays.TryGetValue(Player.Get(sender).UserId, out var display) || display == null)
            {
                response = "You must select a display first (vp select <id>).";
                return false;
            }

            var paused = (display.Paused = !display.Paused);
            response = $"Playback paused: {paused}";
            return true;
        }

        public string Command { get; } = "pause";
        public string[] Aliases { get; } = { "ps" };
        public string Description { get; } = "Pauses playback.";
    }
}