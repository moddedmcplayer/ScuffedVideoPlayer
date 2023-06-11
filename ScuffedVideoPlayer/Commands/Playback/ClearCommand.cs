namespace ScuffedVideoPlayer.Commands.Playback
{
    using System;
    using CommandSystem;
    using PluginAPI.Core;

    public class ClearCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!SelectCommand.SelectedDisplays.TryGetValue(Player.Get(sender).UserId, out var display) || display == null)
            {
                response = "You must select a display first (vp select <id>).";
                return false;
            }

            display.Clear();
            response = "Display cleared.";
            return true;
        }

        public string Command { get; } = "clear";
        public string[] Aliases { get; } = { "c" };
        public string Description { get; } = "Clears the display.";
    }
}