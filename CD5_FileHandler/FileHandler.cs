using System;
using System.Collections.Generic;
using System.IO;

namespace CD5_DataHandling
{

    public class FileHandler
    {
        private const string folder = @"../../../logfiles/";
        private const string extension = ".txt";


        public String[] Read(string name)
        {

            String[] lines = File.ReadAllLines(folder + name);
            return lines;
        }

        public void Write(List<string> data)
        {
            File.WriteAllLines(folder + DateTime.Now.ToString("d") + "_" + DateTime.Now.ToFileTimeUtc() + extension, data.ToArray());
        }

        public string[] SearchLogFiles()
        {
            if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
            DirectoryInfo info = new DirectoryInfo(folder);
            var content = info.GetFiles("*" + extension);
            string[] logfiles = new string[content.Length];
            int i = 0;
            foreach (var logfile in content)
            {
                logfiles[i] = logfile.Name;
                i++;
            }
            return logfiles;
        }

        public void Delete(string name)
        {
            if (File.Exists(folder + name))
            {
                File.Delete(folder + name);
            }
        }

        public bool CheckFileExists(string name)
        {
            return File.Exists(folder + name + extension);
        }
    }
}