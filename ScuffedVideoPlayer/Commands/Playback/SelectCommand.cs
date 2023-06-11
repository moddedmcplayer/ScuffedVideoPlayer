namespace ScuffedVideoPlayer.Commands.Playback
{
    using System;
    using System.Collections.Generic;
    using CommandSystem;
    using PluginAPI.Core;
    using ScuffedVideoPlayer.Output;

    public class SelectCommand : ICommand
    {
        public static Dictionary<string, IDisplay> SelectedDisplays { get; } = new Dictionary<string, IDisplay>();
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "You must specify a display to select.";
                return false;
            }

            if (!int.TryParse(arguments.At(0), out var @int))
            {
                response = "Invalid int.";
                return false;
            }

            if (!Plugin.Displays.TryGetValue(@int, out var display))
            {
                response = $"Display with id {@int} not found.";
                return false;
            }

            SelectedDisplays[Player.Get(sender).UserId] = display;
            response = $"Selected display {@int}.";
            return true;
        }

        public string Command { get; } = "select";
        public string[] Aliases { get; } = { "sl" };
        public string Description { get; } = "Selects the display to affect.";
    }
}