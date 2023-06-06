namespace ScuffedVideoPlayer.API
{
    using System;

    public class VideoLoadingException : Exception
    {
        public VideoLoadingException(string message) : base(message)
        {
        }
    }
}