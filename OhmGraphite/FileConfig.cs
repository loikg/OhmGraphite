using System;

namespace OhmGraphite
{
    public class FileConfig
    {
        public FileConfig(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public static FileConfig ParseAppSettings(IAppConfig config)
        {
            string path = config["path"] ?? "metric.txt";

            return new FileConfig(path);
        }

    }
}