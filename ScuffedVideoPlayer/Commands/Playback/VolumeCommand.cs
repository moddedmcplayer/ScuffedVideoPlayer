namespace ScuffedVideoPlayer.Commands.Playback
{
    using System;
    using System.Globalization;
    using CommandSystem;
    using PluginAPI.Core;

    public class VolumeCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!SelectCommand.SelectedDisplays.TryGetValue(Player.Get(sender).UserId, out var display) || display == null)
            {
                response = "You must select a display first (vp select <id>).";
                return false;
            }

            var audioPlayerBase = display.PlaybackHandle?.AudioNpc?.AudioPlayerBase;
            if (audioPlayerBase == null)
            {
                response = "No audio player.";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = $"Current volume: {audioPlayerBase.Volume}.";
                return false;
            }

            if (!float.TryParse(arguments.At(0), NumberStyles.Float, CultureInfo.InvariantCulture, out var volume))
            {
                response = "You must specify a valid float.";
                return false;
            }

            audioPlayerBase.Volume = volume;

            response = $"Set volume to: {volume}.";
            return true;
        }

        public string Command { get; } = "volume";
        public string[] Aliases { get; } = { "v" };
        public string Description { get; } = "Sets playback volume.";
    }
}