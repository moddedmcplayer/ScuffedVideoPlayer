namespace ScuffedVideoPlayer
{
    using System;
    using CommandSystem;
    using MEC;
    using NWAPIPermissionSystem;

    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class PlayCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("playvideo"))
            {
                response = "You do not have permission to use this command (playvideo).";
                return false;
            }

            if (Plugin.PlayCoroutineHandle.IsValid && Plugin.PlayCoroutineHandle.IsRunning)
            {
                response = "A video is already playing.";
                return false;
            }

            Plugin.PlayCoroutineHandle = Timing.RunCoroutine(Plugin.PlayCoroutine());
            response = "Done.";
            return true;
        }

        public string Command { get; } = "playvideo";
        public string[] Aliases { get; } = { "playv" };
        public string Description { get; } = "Plays a video on the intercom.";
    }
}