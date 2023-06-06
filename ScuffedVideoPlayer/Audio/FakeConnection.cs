namespace ScuffedVideoPlayer.Audio
{
    using System;
    using Mirror;

    public class FakeConnection : NetworkConnectionToClient
    {
        public override void Send(ArraySegment<byte> segment, int channelId = 0)
        {
        }

        public override string address => "127.0.0.1";
        public FakeConnection(int networkConnectionId) : base(networkConnectionId)
        {
        }
    }
}