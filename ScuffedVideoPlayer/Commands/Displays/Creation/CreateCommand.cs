namespace ScuffedVideoPlayer.Commands.Displays.Creation
{
    using System;
    using System.Text;
    using CommandSystem;
    using NorthwoodLib.Pools;

    public class CreateCommand : ParentCommand
    {
        public static CreateCommand Create()
        {
            var command = new CreateCommand();
            command.LoadGeneratedCommands();
            return command;
        }

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CreatePrimitiveCommand());
            RegisterCommand(new CreatePlayerCommand());
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

        public override string Command { get; } = "create";
        public override string[] Aliases { get; } = { "c" };
        public override string Description { get; } = "Display creation parent command.";
    }
}