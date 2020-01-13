using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DeadlyOnline.Logic
{
    public static class SaveHelper
    {
        public static void WriteToSystemFile(string name, object option, string systemFilePath)
        {
            bool isNotfound = true;
            string formattedText = $"{name}={option}";
            string tmpFilePath = Path.GetTempFileName();

            {
                using StreamReader reader = new StreamReader(systemFilePath);
                using StreamWriter writer = new StreamWriter(tmpFilePath, false);

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.StartsWith(name + '='))
                    {
                        if (isNotfound)
                        {
                            writer.WriteLine(formattedText);
                            isNotfound = false;
                        }
                    }
                    else
                    {
                        writer.WriteLine(line);
                    }
                }

                if (isNotfound)
                {
                    writer.WriteLine(formattedText);
                }
            }

            File.Copy(tmpFilePath, systemFilePath, /*overwrite:*/ true);

            File.Delete(tmpFilePath);
        }

    }
}
