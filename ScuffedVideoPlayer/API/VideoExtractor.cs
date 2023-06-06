namespace ScuffedVideoPlayer.API
{
    using System.IO.Compression;

    public static class VideoExtractor
    {
        public static void ExtractFiles(string file, string outdir)
        {
            using (var zip = ZipFile.OpenRead(file))
            {
                zip.ExtractToDirectory(outdir);
            }
        }
    }
}