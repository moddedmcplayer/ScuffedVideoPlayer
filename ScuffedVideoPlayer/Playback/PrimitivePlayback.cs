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
            Color[,] _lastFrame = null!;
            foreach (var frame in video.Frames)
            {
                var newFrame = ImageToColor(frame);
                var newPixels = new Color?[frame.Height, frame.Width]; 
                for (int i = 0; i < frame.Height; i++)
                {
                    for (int y = 0; y < frame.Width; y++)
                    {
                        if (_lastFrame == null || _lastFrame[i, y] != newFrame[i, y])
                        {
                            newPixels[i, y] = newFrame[i, y];
                        }
                    }
                }
                _lastFrame = newFrame;
                yield return newPixels;
            }
        }

        public static Color[,] ImageToColor(Bitmap bitmap)
        {
            var frame = new Color[bitmap.Height, bitmap.Width];
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int y = 0; y < bitmap.Width; y++)
                {
                    var pixel = bitmap.GetPixel(y, i);
                    frame[i, y] = new Color(pixel.R, pixel.G, pixel.B);
                }
            }
            return frame;
        }
    }
}