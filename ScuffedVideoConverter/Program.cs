using System.IO.Compression;
using MediaToolkit;
using ScuffedVideoConverter;

if (args.Length == 0 || !File.Exists(args[0]))
{
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine("Drag a file on to the exe in order to convert it!");
    Console.Read();
}

string path = args[0];

int fps;
if (args.Any(x => x == "-fps"))
{
    fps = int.Parse(args[Array.IndexOf(args, "-fps") + 1]);
}
else
{
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("What fps? (default 10)");
    if (!int.TryParse(Console.ReadLine(), out fps))
        fps = 10;
}

int width;
int height;
if (args.Any(x => x == "-res"))
{
    var res = args[Array.IndexOf(args, "-res") + 1].Split('x');
    width = int.Parse(res[0]);
    height = int.Parse(res[1]);
}
else
{
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("What resolution? (default 45x45)");

    var res = Console.ReadLine()?.Split('x');
    if (!(res?.Length > 0 && int.TryParse(res[0], out width)))
        width = 45;
    if (!(res?.Length > 1 && int.TryParse(res[1], out height)))
        height = 45;
}

try
{
    using var engine = new Engine();
    using (var conv = new VideoConverter(engine, path, fps))
    {
        conv.SetResolution(width, height);
        conv.ExtractAudio();
        ZipFile.CreateFromDirectory(conv.FolderPath, $"{Path.GetFileNameWithoutExtension(args[0])}.zip",
            CompressionLevel.Fastest, false);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Done!");
    }
    Directory.Delete("temp", true);
}
catch (Exception e)
{
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine(e);
    Console.Read();
}