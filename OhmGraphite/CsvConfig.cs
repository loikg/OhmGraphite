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
            string path = config["path"] ?? "metric.csv";
            return new CsvConfig(path);
        }

    }
}