﻿namespace ScuffedVideoPlayer.Playback
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using ScuffedVideoPlayer.API;

    public class TextPlayback : IPlayback<string>
    {
        public IEnumerator<string> Play(LoadedVideo video)
        {
            foreach (var frame in video.Frames)
            {
                yield return ImageToText(frame);
            }
        }

        public static string ImageToText(Bitmap bitmap)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bitmap.Height; i++)
            {
                sb.Append("<size=33%><line-height=75%>");
                for (int y = 0; y < bitmap.Width; y++)
                {
                    var pixel = bitmap.GetPixel(y, i);
                    sb.Append($"<color={ToHex(pixel)}>█</color>");
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        public static string ToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    }
}