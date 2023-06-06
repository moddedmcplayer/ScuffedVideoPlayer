namespace ScuffedVideoConverter
{
    using System;
    using System.Drawing;
    using System.IO;
    using MediaToolkit;
    using MediaToolkit.Model;
    using MediaToolkit.Options;

    public class VideoConverter : IDisposable
    {
        public readonly string FolderPath;
        public readonly string FramesPath;
        public readonly MediaFile MediaFile;
        private readonly Engine _engine;

        public void ExtractAudio()
        {
            _engine.CustomCommand($"-i \"{MediaFile.Filename}\" -ac 1 -c:a pcm_s16le -ar 48000 -vn -acodec libvorbis \"{Path.Combine(FolderPath, "audio.ogg")}\"");
        }

        public void SetResolution(int width, int height)
        {
            if (!OperatingSystem.IsWindows())
            {
                throw new Exception("Windows only lol");
            }

            int i = 0;
            foreach (var file in Directory.GetFiles(FramesPath))
            {
                var img = Image.FromFile(file);
                var resized = Stackoverflow.ResizeImage(img, width, height);
                img.Dispose();
                resized.Save(file);
            }
        }

        public VideoConverter(Engine engine, string path, int fps = 10)
        {
            _engine = engine;
            MediaFile = new MediaFile { Filename = path };

            _engine.GetMetadata(MediaFile);

            string folder = Guid.NewGuid().ToString();
            string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "temp");
            FolderPath = Path.Combine(tempFolder, folder);
            FramesPath = Path.Combine(FolderPath, "frames");
            Directory.CreateDirectory(FramesPath);
            double increment = (double)1/fps;
            for (int i = 0; i < MediaFile.Metadata.Duration.TotalSeconds; i++)
            {
                for (double j = 0; j < fps; j++)
                {
                    var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(i+(increment*j)) };
                    var outputFile = new MediaFile { Filename = Path.Combine(FramesPath, $"{i}-{j}.jpeg")};
                    _engine.GetThumbnail(MediaFile, outputFile, options);
                }
            }
        }

        public void Dispose()
        {
            Directory.Delete(FolderPath, true);
        }
    }
}