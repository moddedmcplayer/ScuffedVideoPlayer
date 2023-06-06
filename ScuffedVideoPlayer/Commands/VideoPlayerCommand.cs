namespace ScuffedVideoPlayer.Commands
{
    using System;
    using System.Text;
    using CommandSystem;
    using NorthwoodLib.Pools;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class VideoPlayerCommand : ParentCommand
    {
        public VideoPlayerCommand() => LoadGeneratedCommands();
        public override string Command => "videoplayer";
        public override string[] Aliases { get; } = { "vp" };
        public override string Description => "Videoplayer parent command.";

        public sealed override void LoadGeneratedCommands()
        {
            RegisterCommand(new PlayCommand());
            RegisterCommand(new ListCommand());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
            stringBuilder.AppendLine("Please enter a valid subcommand! Available:");
            foreach (ICommand command in AllCommands)
            {
                stringBuilder.AppendLine(command.Aliases is { Length: > 0 }
                    ? $"{command.Command} | Aliases: {string.Join(", ", command.Aliases)}"
                    : command.Command);
            }

            response = StringBuilderPool.Shared.ToStringReturn(stringBuilder).TrimEnd();
            return false;
        }
    }
}