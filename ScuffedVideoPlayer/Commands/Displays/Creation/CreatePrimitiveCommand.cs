namespace ScuffedVideoPlayer.Commands.Displays.Creation
{
    using System;
    using CommandSystem;
    using NWAPIPermissionSystem;
    using RemoteAdmin;
    using ScuffedVideoPlayer.Output.Displays;

    public class CreatePrimitiveCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("videoplayer.display.create.primitive"))
            {
                response = "You do not have permission to run this command (videoplayer.display.create.primitive).";
                return false;
            }

            int width = 45;
            int height = 45;
            if (arguments.Count > 0)
            {
                if (!int.TryParse(arguments.At(0), out width) || width < 1)
                {
                    response = "You must specify a valid non 0 width.";
                    return false;
                }

                if (arguments.Count > 1 && (!int.TryParse(arguments.At(1), out height) || height < 1))
                {
                    response = "You must specify a valid non 0 height.";
                    return false;
                }
            }

            double scale = 1;
            if (arguments.Count > 2 && !double.TryParse(arguments.At(2), out scale))
            {
                response = "You must specify a valid scale.";
                return false;
            }

            var display = new PrimitiveDisplay(width, height, scale);
            if (sender is PlayerCommandSender playerCommandSender)
            {
                var transform = playerCommandSender.ReferenceHub.transform;
                display.Position = transform.position;
                display.ParentGameObject.transform.rotation = transform.rotation;
            }
            response = $"Created display with Id: {display.Id}.";
            return true;
        }

        public string Command { get; } = "primitive";
        public string[] Aliases { get; } = { "p" };
        public string Description { get; } = "Creates a primitive display.";
    }
}