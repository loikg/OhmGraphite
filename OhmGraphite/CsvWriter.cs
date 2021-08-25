using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using NLog;

namespace OhmGraphite
{
    public class TimedReportedValue: ReportedValue {
        public static TimedReportedValue FromReportedValue(ReportedValue sensor, DateTime reportedTime) {
            return new TimedReportedValue(sensor.Identifier, sensor.Sensor, sensor.Value, sensor.SensorType, sensor.Hardware, sensor.HardwareType, sensor.HardwareInstance, sensor.SensorIndex, reportedTime);
        }

        private int dateTimeToUnixTimestamp(DateTime time) {
            return (int)time.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public TimedReportedValue(string identifier, 
            string sensor,
            float value,
            SensorType sensorType,
            string hardware,
            HardwareType hardwareType,
            string hwInstance,
            int sensorIndex,
            DateTime reportedTime): base(identifier, sensor, value, sensorType, hardware, hardwareType, hwInstance, sensorIndex) {
               Time = dateTimeToUnixTimestamp(reportedTime);
            }
         public int Time { get; }
    }
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
                // If the file already exist then we assume the CSV header is already written.
                HasHeaderRecord = !fileAlreadyExist,
            };

            
            var dataEntries = sensors.Select(s => TimedReportedValue.FromReportedValue(s, reportTime)).ToList();

            // Open the file with FileShare.Read to lock the file until we are done writing to the csv. 
            using (var stream = File.Open(csvPath, FileMode.Append, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvHelper.CsvWriter(writer, csvWriterConfig))
            {
                await csv.WriteRecordsAsync(dataEntries);
                await csv.FlushAsync();
            }
        }

        public void Dispose()
        {
        }
    }
}