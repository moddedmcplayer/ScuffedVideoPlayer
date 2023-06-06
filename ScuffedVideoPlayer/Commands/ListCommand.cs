namespace ScuffedVideoPlayer.Commands
{
    using System;
    using System.Linq;
    using System.Text;
    using CommandSystem;
    using NWAPIPermissionSystem;
    using Plugin = ScuffedVideoPlayer.Plugin;

    public class ListCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("videoplayer.list"))
            {
                response = "You do not have permission to use this command (videoplayer.list).";
                return false;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Plugin.Videos.Count; i++)
            {
                sb.Append($"{i+1} - \"{Plugin.Videos.Keys.ElementAt(i)}\"");
                if (i != Plugin.Videos.Count - 1)
                    sb.Append('\n');
            }
            response = sb.ToString();
            return true;
        }

        public string Command { get; } = "list";
        public string[] Aliases { get; } = { "l" };
        public string Description { get; } = "Lists all available videos.";
    }
}