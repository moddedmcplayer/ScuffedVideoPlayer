namespace ScuffedVideoPlayer.API
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using PluginAPI.Core;

    public class LoadedVideo
    {
        private readonly string _dir;
        private readonly string _framesDir;
        private Bitmap[]? _frames = null;

        public Bitmap[] Frames
        {
            get
            {
                return _frames ??= LoadVideo();
            }
        }
        public string? AudioFile { get; private set; }
        public int FramesPerSecond { get; private set; }

        private Bitmap[] LoadVideo()
        {
            List<Bitmap> frames = new List<Bitmap>();
            Dictionary<int, List<int>> secondToFrame = new Dictionary<int, List<int>>();
            foreach (var img in Directory.GetFiles(_framesDir, "*.jpeg", SearchOption.TopDirectoryOnly))
            {
                var fileName = Path.GetFileNameWithoutExtension(img);
                var split = fileName.Split('-');
                if (split.Length != 2)
                {
                    throw new VideoLoadingException("Invalid frame name (split)");
                }
                if (!int.TryParse(split[0], out var second))
                {
                    throw new VideoLoadingException("Invalid frame name (second)");
                }
                if (!int.TryParse(split[1], out var frame))
                {
                    throw new VideoLoadingException("Invalid frame name (frame)");
                }
                if (secondToFrame.TryGetValue(second, out var value))
                {
                    value.Add(frame);
                }
                else
                {
                    secondToFrame.Add(second, new List<int> { frame });
                }
                frames.Add(new Bitmap(img));
            }

            if (!secondToFrame.TryGetValue(0, out var firstFrames))
            {
                throw new VideoLoadingException("No frames for second 0");
            }
            int count = firstFrames.Count;
            for (var index = 1; index < secondToFrame.Count-1; index++)
            {
                var value = secondToFrame[index];
                if (value.Count != count && index != secondToFrame.Count - 1)
                {
                    Log.Debug($"index {index}, count-1 {secondToFrame.Count - 1}");
                    throw new VideoLoadingException($"Invalid frame count (second {index})");
                }
            }
            FramesPerSecond = count;

            return frames.ToArray();
        }

        public LoadedVideo(string dir)
        {
            _dir = dir;
            var audioPath = Path.Combine(dir, "audio.ogg");
            if (File.Exists(audioPath))
            {
                AudioFile = audioPath;
            }
            _framesDir = Path.Combine(dir, "frames");
            if (!Directory.Exists(_framesDir) || Directory.GetFiles(_framesDir).Length == 0)
            {
                throw new VideoLoadingException("No frames?");
            }
        }
    }
}