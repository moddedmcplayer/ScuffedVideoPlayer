namespace ScuffedVideoPlayer.Commands.Displays.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommandSystem;
    using NWAPIPermissionSystem;
    using PluginAPI.Core;
    using RemoteAdmin;
    using ScuffedVideoPlayer.Output.Displays;
    using Utils;

    public class CreatePlayerCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("videoplayer.display.create.player"))
            {
                response = "You do not have permission to run this command (videoplayer.display.create.player).";
                return false;
            }

            List<Player> targets = new();
            if (arguments.Count > 0)
            {
                var list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out var newArgs);
                targets = list.Select(Player.Get).ToList();
            }
            else if (sender is PlayerCommandSender playerCommandSender)
            {
                targets.Add(Player.Get(playerCommandSender));
            }
            else
            {
                response = "You must specify a player to create a display for.";
                return false;
            }

            foreach (var target in targets)
            {
                if (PlayerDisplay.Instances.TryGetValue(target, out var value))
                {
                    response = $"Player display already exists for {target.Nickname} (display id: {value.Id}).";
                    return false;
                }
            }

            var display = new PlayerDisplay(targets);

            response = $"Created display with Id: {display.Id}.";
            return true;
        }

        public string Command { get; } = "player";
        public string[] Aliases { get; } = { "pl" };
        public string Description { get; } = "Creates a player display.";
    }
}