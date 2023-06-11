namespace ScuffedVideoPlayer.Commands.Displays
{
    using System;
    using CommandSystem;
    using NWAPIPermissionSystem;
    using PluginAPI.Core;
    using RemoteAdmin;
    using ScuffedVideoPlayer.Commands.Playback;
    using ScuffedVideoPlayer.Output;
    using ScuffedVideoPlayer.Output.Displays;
    using UnityEngine;
    using Plugin = ScuffedVideoPlayer.Plugin;

    public class BringCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("videoplayer.display.bring"))
            {
                response = "You do not have permission to run this command (videoplayer.display.bring).";
                return false;
            }

            if (sender is not PlayerCommandSender playerSender)
            {
                response = "You must be a player to run this command.";
                return false;
            }

            var ply = Player.Get(playerSender)!;

            IDisplay display;
            if (arguments.Count < 1)
            {
                if (!SelectCommand.SelectedDisplays.TryGetValue(ply.UserId, out display))
                {
                    response = "You must specify or select a display to bring.";
                    return false;
                }
            }
            else
            {
                if (!int.TryParse(arguments.At(0), out var id))
                {
                    response = "You must specify a valid int.";
                    return false;
                }

                if (!Plugin.Displays.TryGetValue(id, out display))
                {
                    response = $"Display with id {id} not found.";
                    return false;
                }
            }

            bool moveRotation = false;
            if (arguments.Count > 1)
            {
                if (!bool.TryParse(arguments.At(1), out moveRotation))
                {
                    response = "You must specify a valid bool.";
                    return false;
                }
            }

            if (display is not PrimitiveDisplay primitiveDisplay)
            {
                response = "You cannot move this display.";
                return false;
            }

            primitiveDisplay.ParentGameObject.transform.position = ply.Position;
            if (moveRotation)
            {
                primitiveDisplay.ParentGameObject.transform.rotation = Quaternion.Euler(ply.Rotation);
            }

            response = $"Brought display {display.Id}.";
            return true;
        }

        public string Command { get; } = "bring";
        public string[] Aliases { get; } = { "b" };
        public string Description { get; } = "Brings a display to your position.";
    }
}