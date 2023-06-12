namespace ScuffedVideoPlayer.Output.Displays
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AdminToys;
    using Mirror;
    using PluginAPI.Core;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class PrimitiveDisplay : IDisplay
    {
        public static PrimitiveObjectToy PrimitiveObjectToy;
        public static List<PrimitiveDisplay> Instances { get; } = new List<PrimitiveDisplay>();
        public bool Paused { get; set; } = false;

        public Vector3 Position
        {
            get => ParentGameObject.transform.position;
            set => ParentGameObject.transform.position = value;
        }
        public PlaybackHandle? PlaybackHandle { get; set; }
        public int Id { get; }

        public readonly GameObject ParentGameObject;
        public readonly PrimitiveObjectToy[][] GameObjects;
        public readonly Tuple<int, int> Resolution;

        public PrimitiveDisplay(int width = 45, int height = 45, double scale = 1)
        {
            Id = Plugin.GetDisplayId();
            Instances.Add(this);
            Plugin.Displays.Add(Id, this);

            Resolution = new Tuple<int, int>(width, height);
            ParentGameObject = new GameObject($"PrimitiveDisplay_{Id}");
            List<List<PrimitiveObjectToy>> objects = new List<List<PrimitiveObjectToy>>();
            var size = (float)(scale * 0.05);
            var centerDelta = (float)(scale * 0.05 * width / 2);
            for (int i = height; i > 0; i--)
            {
                var ypos = (float)(i*0.05*scale);
                var list = new List<PrimitiveObjectToy>();
                objects.Add(list);
                for (int y = width; y > 0; y--)
                {
                    var obj = Object.Instantiate(PrimitiveObjectToy.gameObject);
                    var comp = obj.GetComponent<PrimitiveObjectToy>();
                    comp.MovementSmoothing = 0;
                    comp.PrimitiveType = PrimitiveType.Cube;
                    var transform = comp.transform;
                    transform.localScale = new Vector3(size, size, size);
                    transform.localPosition = new Vector3((float)(y*0.05*scale - centerDelta), ypos, 0);
                    transform.SetParent(ParentGameObject.transform);
                    NetworkServer.Spawn(obj);
                    list.Add(comp);
                }
            }
            GameObjects = objects.Select(x => x.ToArray()).ToArray();
        }

        public void SetColor(Color?[,] newPixels)
        {
            for (int row = 0; row < newPixels.GetLength(0); row++)
            {
                for (int col = 0; col < newPixels.GetLength(1); col++)
                {
                    var color = newPixels[row,col];
                    if (color == null)
                        continue;
                    GameObjects[row][col].NetworkMaterialColor = color.Value;
                }
            }
        }

        public void Clear()
        {
            for (int row = 0; row < Resolution.Item1; row++)
            {
                for (int col = 0; col < Resolution.Item2; col++)
                {
                    GameObjects[row][col].NetworkMaterialColor = Color.white;
                }    
            }
            PlaybackHandle?.AudioNpc?.Destroy();
        }

        public void Dispose()
        {
            PlaybackHandle?.Dispose();
            Instances.Remove(this);
            Plugin.Displays.Remove(Id);
            foreach (var arr in GameObjects)
            {
                foreach (var comp in arr)
                {
                    NetworkServer.Destroy(comp.gameObject);
                }
            }
            Plugin.FreeDisplayIds.Enqueue(Id);
        }
    }
}