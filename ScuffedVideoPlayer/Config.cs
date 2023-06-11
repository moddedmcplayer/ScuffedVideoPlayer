namespace ScuffedVideoPlayer
{
    using System.IO;

    public class Config
    {
        public string VideoFolder { get; set; } = Path.Combine(Plugin.PluginFolder, "videos");
        public bool PrimitiveDisplayCollisions { get; set; } = true;
    }
}