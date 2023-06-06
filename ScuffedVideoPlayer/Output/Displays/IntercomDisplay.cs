namespace ScuffedVideoPlayer.Output.Displays
{
    using UnityEngine;

    public class IntercomDisplay : ITextDisplay
    {
        public static IntercomDisplay Instance { get; } = new IntercomDisplay();
        public bool Paused { get; set; } = false;
        public Vector3 Position { get; } = PlayerRoles.Voice.IntercomDisplay._singleton.transform.position;
        public PlaybackHandle? PlaybackHandle { get; set; }

        public void SetText(string text)
        {
            PlayerRoles.Voice.IntercomDisplay.TrySetDisplay(text);
        }

        public void Clear()
        {
            PlayerRoles.Voice.IntercomDisplay.TrySetDisplay(null);
        }
    }
}