using System.IO.Compression;

namespace Csharp.Core
{
    public class ObjectStore
    {
        public static void WriteObject(string sha1, byte[] data)
        {
            string dir = Path.Combine(".gitadr", "objects", sha1.Substring(0, 2));
            string file = sha1.Substring(2);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string path = Path.Combine(dir, file);
            using var fs = File.Create(path);
            using var zlib = new ZLibStream(fs, CompressionMode.Compress);
            zlib.Write(data, 0, data.Length);
        }
    }
}
