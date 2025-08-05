using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

namespace WindowsTimeSync
{
    class Program
    {
        const string defaultZone = "Asia/Tehran";
        const string lat = "35.6944";
        const string lng = "51.4215";
        const string apiKey = "V2ZJELAQIGBW";
        static System.Timers.Timer timer = new System.Timers.Timer(90000);

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            string localZone = defaultZone;
            if (args.Any())
                localZone = args[0].Trim();

            if (string.IsNullOrEmpty(lat))
                timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => SetTimeByWorldTimeApi(localZone);
            else
                timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => SetTimeByTimeZonedb(localZone, lat, lng);
            timer.Start();

            Console.ReadKey();
        }

        private static void SetTimeByWorldTimeApi(string localZone)
        {
            try
            {
                var localDate = DateTime.Now;
                var localDateUtc = DateTime.Now;

                var timeDate = new HttpClient().GetStringAsync("http://worldtimeapi.org/api/timezone/" + localZone).Result;
                var serverDate = JsonSerializer.Deserialize<InternetTime>(timeDate);
                if ((string.IsNullOrEmpty(localZone) && Math.Abs(serverDate.datetime.Subtract(localDateUtc).TotalSeconds) > 3) ||
                    (!string.IsNullOrEmpty(localZone) && Math.Abs(serverDate.datetime.Subtract(localDate).TotalSeconds) > 3))
                {
                    WinDateTime.SetDateTime(serverDate.datetime, localZone);
                    Console.WriteLine(DateTime.Now.ToString("t") + ": Set correct time: " + serverDate.datetime.ToString("t"));
                }
                else
                {
                    Console.WriteLine(DateTime.Now.ToString("t") + " No changes detected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (ex.Message.Contains("A required privilege is not held by the client"))
                {
                    Console.WriteLine("\n---------\nPlease run program as administrator\n");
                    timer.Stop();
                }
            }
        }

        private static void SetTimeByTimeZonedb(string localZone, string lat, string lng)
        {
            try
            {
                var localDate = DateTime.Now;
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var timeDate = new HttpClient().GetStringAsync($"https://api.timezonedb.com/v2.1/get-time-zone?key={apiKey}&format=json&by=position&lat={lat}&lng={lng}").Result;
                stopWatch.Stop();
                if (stopWatch.Elapsed.TotalSeconds > 2)
                {
                    Console.WriteLine("so slowly! " + stopWatch.Elapsed.TotalSeconds);
                    return;
                }

                localDate = localDate.AddMilliseconds(-500);
                var result = JsonSerializer.Deserialize<InternetTimeZone>(timeDate);
                var serverDate = DateTime.Parse(result.formatted);
                if (Math.Abs(serverDate.Subtract(localDate).TotalSeconds) > 3)
                {
                    WinDateTime.SetDateTime(serverDate, localZone);
                    Console.WriteLine(DateTime.Now.ToString("t") + ": Set correct time: " + serverDate.ToString("t"));
                }
                else
                {
                    Console.WriteLine(DateTime.Now.ToString("t") + " No changes detected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (ex.Message.Contains("A required privilege is not held by the client"))
                {
                    Console.WriteLine("\n---------\nPlease run program as administrator\n");
                    timer.Stop();
                }
            }
        }

        public static DateTime GetDate(int datetime)
        {
            return new DateTime(1970, 1, 1).AddSeconds(datetime).Add(DateTime.Now.Subtract(DateTime.UtcNow));
        }
    }
}
