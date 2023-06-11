namespace ScuffedVideoPlayer.Commands.Playback
{
    using System;
    using CommandSystem;
    using NWAPIPermissionSystem;
    using ScuffedVideoPlayer.Output.Displays;

    public class StopAllCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("videoplayer.stop"))
            {
                response = "You do not have permission to run this command (videoplayer.stop).";
                return false;
            }

            int removed = 0;
            foreach (var display in PrimitiveDisplay.Instances.ToArray())
            {
                display.Dispose();
                removed++;
            }

            if (IntercomDisplay.Instance.PlaybackHandle?.IsPlaying ?? false)
            {
                IntercomDisplay.Instance.Dispose();
                removed++;
            }

            response = $"Stopped {removed} videos.";
            return true;
        }

        public string Command { get; } = "stopall";
        public string[] Aliases { get; } = { "sa", "stopa" };
        public string Description { get; } = "Stops all videos.";
    }
}