using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using NLog;

namespace OhmGraphite
{
    public class CsvWriter : IWriteMetrics
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly CsvConfig _config;

        public CsvWriter(CsvConfig config)
        {
            _config = config;
        }

        public async Task ReportMetrics(DateTime reportTime, IEnumerable<ReportedValue> sensors)
        {
            var csvPath = _config.Path;
            var fileAlreadyExist = File.Exists(csvPath);
            var csvWriterConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Don't write the header again.
                HasHeaderRecord = !fileAlreadyExist,
            };

            // Open the file with FileShare.Read to lock the file to read until we are done writing to the csv. 
            using (var stream = File.Open(csvPath, FileMode.Append, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvHelper.CsvWriter(writer, csvWriterConfig))
            {
                await csv.WriteRecordsAsync(sensors);
                await csv.FlushAsync();
            }
        }

        public void Dispose()
        {
        }
    }
}