using System;
using System.Collections.Generic;
using System.IO;

namespace Client
{
    public class ImageReader
    {
        public const int fragmentSize = 32000;
        private static FileStream stream;

        public static bool SetPath(string path)
        {
            try
            {
                stream = File.OpenRead(path);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        public static IEnumerable<byte[]> ImageFragments()
        {
            var fragment = new byte[fragmentSize];
            while (true)
            {
                int fragmentLength = ReadFragment(fragment);

                var imageFragment = new byte[fragmentLength];
                Array.Copy(fragment, imageFragment, fragmentLength);

                if (fragmentLength != 0)
                {
                    yield return imageFragment;
                }
                if (fragmentLength != fragment.Length)
                {
                    yield break;
                }
            }
        }

        private static int ReadFragment(byte[] fragment)
        {
            var pos = 0;
            while (pos < fragment.Length)
            {
                int read = stream.Read(fragment, pos, fragment.Length - pos);
                if (read == 0)
                {
                    break;
                }
                pos += read;
            }
            return pos;
        }

        public static void CloseFile()
        {
            stream.Close();
        }
    }
}
