namespace ScuffedVideoPlayer.Audio
{
    using System.Collections.Generic;
    using System.Linq;
    using MEC;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerStatsSystem;
    using PluginAPI.Core;
    using SCPSLAudioApi.AudioCore;
    using UnityEngine;
    using VoiceChat;

    public class AudioNpc
    {
        private static List<ReferenceHub> _npcs = new List<ReferenceHub>();
        public readonly ReferenceHub Hub;
        public readonly AudioPlayerBase AudioPlayerBase;

        public bool IsPlaying => AudioPlayerBase.VorbisReader is { IsEndOfStream: false };

        public bool IsPaused
        {
            get => AudioPlayerBase.ShouldPlay;
            set => AudioPlayerBase.ShouldPlay = value;
        }

        public void SetPosition(Vector3 position)
        {
            Hub.TryOverridePosition(position, Vector3.zero);
        }

        public void Play(string file, VoiceChatChannel channel = VoiceChatChannel.Proximity, float volume = 100f)
        {
            Stop();
            AudioPlayerBase.Volume = volume;
            AudioPlayerBase.BroadcastChannel = channel;
            AudioPlayerBase.Enqueue(file, 0);
            AudioPlayerBase.Play(0);
        }

        public void Queue(string file)
        {
            AudioPlayerBase.Enqueue(file, -1);
        }

        public void Stop()
        {
            AudioPlayerBase.Stoptrack(true);
        }

        public static AudioNpc Create(Vector3? pos = null)
        {
            var newPlayer = Object.Instantiate(NetworkManager.singleton.playerPrefab);
            var fakeConnection = new FakeConnection(new RecyclablePlayerId(true).Value);
            var hub = newPlayer.GetComponent<ReferenceHub>();
            NetworkServer.AddPlayerForConnection(fakeConnection, newPlayer);
            _npcs.Add(hub);
            Timing.CallDelayed(0.7f, () =>
            {
                hub.roleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.None, RoleSpawnFlags.None);
                hub.characterClassManager.GodMode = true;
                hub.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                foreach (var target in ReferenceHub.AllHubs.Where(x => x != ReferenceHub.HostHub))
                    NetworkServer.SendSpawnMessage(hub.networkIdentity, target.connectionToClient);
            });
            Timing.CallDelayed(1f, () =>
            {
                if (pos != null)
                    hub.TryOverridePosition(pos.Value, Vector3.zero);
            });
            return new AudioNpc(hub);
        }

        public void Destroy()
        {
            _npcs.Remove(Hub);
            NetworkServer.Destroy(Hub.gameObject);
        }

        public static bool IsNpc(ReferenceHub hub)
        {
            return _npcs.Contains(hub);
        }

        private AudioNpc(ReferenceHub hub)
        {
            Hub = hub;
            AudioPlayerBase = AudioPlayerBase.Get(Hub);
        }
    }
}