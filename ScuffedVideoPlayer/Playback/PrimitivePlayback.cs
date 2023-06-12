namespace ScuffedVideoPlayer.Playback
{
    using System.Collections.Generic;
    using System.Drawing;
    using ScuffedVideoPlayer.API;
    using Color = UnityEngine.Color;

    public class PrimitivePlayback
    {
        public static IEnumerator<Color?[,]> Play(LoadedVideo video)
        {
            Color?[,]? lastFrame = null;
            foreach (var frame in video.Frames)
            {
                var newFrame = ImageToColor(frame, lastFrame);
                lastFrame = newFrame;
                yield return newFrame;
            }
        }

        public static Color?[,] ImageToColor(Bitmap bitmap, Color?[,]? prevFrame = null)
        {
            var frame = new Color?[bitmap.Height, bitmap.Width];
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int y = 0; y < bitmap.Width; y++)
                {
                    var pixel = bitmap.GetPixel(y, i);
                    // ReSharper disable PossibleLossOfFraction
                    var pixelUnity = new Color(pixel.R / 255f, pixel.G / 255f, pixel.B / 255f, pixel == System.Drawing.Color.Transparent ? 0 : 1);
                    // ReSharper restore PossibleLossOfFraction

                    if (prevFrame != null && prevFrame[y, i] == pixelUnity)
                    {
                        frame[i, y] = null;
                        continue;
                    }
                    frame[i, y] = pixelUnity;
                }
            }
            return frame;
        }
    }
}