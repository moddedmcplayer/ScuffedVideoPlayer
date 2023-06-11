namespace ScuffedVideoPlayer.Commands.Displays
{
    using System;
    using System.Text;
    using CommandSystem;
    using NorthwoodLib.Pools;
    using ScuffedVideoPlayer.Commands.Displays.Creation;

    public class DisplayCommand : ParentCommand
    {
        public static DisplayCommand Create()
        {
            var command = new DisplayCommand();
            command.LoadGeneratedCommands();
            return command;
        }

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new DestroyCommand());
            RegisterCommand(new BringCommand());

            RegisterCommand(CreateCommand.Create());
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

        public override string Command { get; } = "display";
        public override string[] Aliases { get; } = { "d" };
        public override string Description { get; } = "Display management parent command.";
    }
}