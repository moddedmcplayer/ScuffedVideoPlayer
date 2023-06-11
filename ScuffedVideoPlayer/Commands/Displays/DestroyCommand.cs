namespace ScuffedVideoPlayer.Commands.Displays
{
    using System;
    using CommandSystem;
    using NWAPIPermissionSystem;
    using ScuffedVideoPlayer.Output.Displays;
    using Plugin = ScuffedVideoPlayer.Plugin;

    public class DestroyCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("videoplayer.display.destroy"))
            {
                response = "You do not have permission to run this command (videoplayer.display.bring).";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "You must specify a display to destroy.";
                return false;
            }

            if (!int.TryParse(arguments.At(0), out var id))
            {
                response = "You must specify a int.";
                return false;
            }

            if (!Plugin.Displays.TryGetValue(id, out var display))
            {
                response = $"Display with id {id} not found.";
                return false;
            }

            if (display is not PrimitiveDisplay primitiveDisplay)
            {
                response = "You cannot destroy this display (try using the stop command instead).";
                return false;
            }

            primitiveDisplay.Dispose();
            response = $"Destroyed display {id}.";
            return true;
        }

        public string Command { get; } = "destroy";
        public string[] Aliases { get; } = { "d" };
        public string Description { get; } = "Destroys a display.";
    }
}