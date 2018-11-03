using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class ImageWriter
    {
        private const string downloadDirectory = @"avatars\";
        private static FileStream stream;
        public const int fragmentSize = 32000;
        public static void SetImageLocation(string nickname)
        {
            if (!Directory.Exists(downloadDirectory))
            {
                Directory.CreateDirectory(downloadDirectory);
            }
            stream = File.OpenWrite(downloadDirectory + nickname);
        }

        public static void WriteFragment(byte[] fragment)
        {
            stream.Write(fragment, 0, fragment.Length);
        }

        public static void CloseFile()
        {
            stream.Flush();
            stream.Close();
        }
    }
}
