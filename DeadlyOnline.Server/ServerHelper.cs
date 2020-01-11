using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace DeadlyOnline.Server
{
    static class ServerHelper
    {
        public const string MainMapFilePath = "MainMapFilePath";
        public const string EncountRate = "EncountRate";
        private const string TmpFilePath = @"tmp\tmp.deo";
        private const string TmpFileDirectory = @"tmp";

        public static void WriteToSystemFile(string name, object option)
        {
            string formattedText = $"{name}={option}";
            string path = Server.SystemFilePath;

            bool isfound = false;

            if (!File.Exists(TmpFilePath))
            {
                if (!Directory.Exists(TmpFileDirectory))
                {
                    Directory.CreateDirectory(TmpFileDirectory);
                }
                File.Create(TmpFilePath).Close();
            }

            {
                using StreamReader reader = new StreamReader(path);
                using StreamWriter writer = new StreamWriter(TmpFilePath, false);

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.StartsWith(name + '='))
                    {
                        if (isfound) { continue; }
                        writer.WriteLine(formattedText);
                        isfound = true;
                    }
                    else
                    {
                        writer.WriteLine(line);
                    }
                }

                if (!isfound)
                {
                    writer.WriteLine(formattedText);
                }
            }

            {
                using StreamReader reader = new StreamReader(TmpFilePath);
                using StreamWriter writer = new StreamWriter(path, false);

                while (!reader.EndOfStream)
                {
                    writer.WriteLine(reader.ReadLine());
                }
            }

            File.Delete(TmpFilePath);
        }
    }
}
