namespace ScuffedVideoPlayer.Output.Displays
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using PluginAPI.Core;
    using UnityEngine;

    public class PlayerDisplay : ITextDisplay
    {
        public static Dictionary<Player, PlayerDisplay> Instances { get; } = new Dictionary<Player, PlayerDisplay>();
        public readonly ReadOnlyCollection<Player> Players;
        public bool Paused { get; set; } = false;
        public Vector3 Position => Vector3.negativeInfinity;
        public PlaybackHandle? PlaybackHandle { get; set; }
        public int Id { get; }

        public void SetText(string text)
        {
            foreach (var ply in Players)
            {
                ply.ReceiveHint(text, 1000f);
            }
        }

        public PlayerDisplay(Player player) : this(new[] { player }){}
        public PlayerDisplay(IEnumerable<Player> players)
        {
            var playerList = players.ToList();
            if (playerList.Count == 0)
            {
                throw new System.ArgumentException("Must have at least one player");
            }
            Players = new ReadOnlyCollection<Player>(playerList);
            Id = Plugin.GetDisplayId();
            foreach (var ply in playerList)
            {
                Instances.Add(ply, this);
            }
            Plugin.Displays.Add(Id, this);
        }

        public void Clear()
        {
            foreach (var ply in Players)
            {
                ply.ReceiveHint(null, 0.1f);
            }
            PlaybackHandle?.AudioNpc?.Destroy();
        }

        public void Dispose()
        {
            Clear();
            PlaybackHandle?.Dispose();
            foreach (var ply in Players)
            {
                Instances.Remove(ply);
            }
            Plugin.Displays.Remove(Id);
            Plugin.FreeDisplayIds.Enqueue(Id);
        }
    }
}