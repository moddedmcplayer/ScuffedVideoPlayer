namespace ScuffedVideoPlayer.Commands
{
    using System;
    using System.Linq;
    using System.Text;
    using CommandSystem;
    using NWAPIPermissionSystem;
    using ScuffedVideoPlayer.Output;
    using ScuffedVideoPlayer.Output.Displays;
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
            bool videos = true;
            bool displays = true;
            if (arguments.Count > 0)
            {
                if (arguments.At(0) == "videos" || arguments.At(0) == "v")
                    displays = false;
                else if (arguments.At(0) == "displays" || arguments.At(0) == "d")
                    videos = false;
            }

            sb.Append('\n');
            if (videos)
            {
                sb.Append("Videos:\n");
                for (int i = 0; i < Plugin.Videos.Count; i++)
                {
                    sb.Append($"{i + 1} - \"{Plugin.Videos.Keys.ElementAt(i)}\"");
                    sb.Append('\n');
                }
            }

            if (displays)
            {
                sb.Append("Displays:\n");
                foreach (var kvp in Plugin.Displays)
                {
                    sb.Append($"{kvp.Key} - \"{GetName(kvp.Value)}\"");
                    if (kvp.Value is PrimitiveDisplay primitiveDisplay)
                        sb.Append($" ({primitiveDisplay.Resolution.Item1}x{primitiveDisplay.Resolution.Item2})");
                    if (kvp.Value.PlaybackHandle?.IsPlaying ?? false)
                        sb.Append(" (playing)");
                    if (kvp.Value.Paused)
                        sb.Append(" (paused)");
                    sb.Append('\n');
                }
            }

            response = sb.ToString();
            return true;
        }

        public static string GetName(IDisplay display)
        {
            if (display == IntercomDisplay.Instance)
                return "Intercom";
            if (display is PrimitiveDisplay _)
                return "Primitive";
            if (display is PlayerDisplay _)
                return "Player";
            if (display is ITextDisplay _)
                return "Unnamed text";
            return "unknown";
        }

        public string Command { get; } = "list";
        public string[] Aliases { get; } = { "l" };
        public string Description { get; } = "Lists all available videos.";
    }
}