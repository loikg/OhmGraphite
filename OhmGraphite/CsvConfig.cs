using System;

namespace OhmGraphite
{
    public class CsvConfig
    {
        public CsvConfig(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public static CsvConfig ParseAppSettings(IAppConfig config)
        {
            string path = config["path"] ?? "metric.txt";

            return new CsvConfig(path);
        }

    }
}