using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

namespace WindowsTimeSync
{
    class Program
    {
        const string defaultZone = "Asia/Tehran";
        static System.Timers.Timer timer = new System.Timers.Timer(90000);

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            string localZone = defaultZone;
            if (args.Any())
                localZone = args[0].Trim();

            timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => SetTime(localZone);
            timer.Start();

            Console.ReadKey();
        }

        private static void SetTime(string localZone)
        {
            try
            {
                var localDate = DateTime.Now;
                var localDateUtc = DateTime.Now;

                var timeDate = new HttpClient().GetStringAsync("http://worldtimeapi.org/api/timezone/" + localZone).Result;
                var nowDate = JsonSerializer.Deserialize<InternetTime>(timeDate);
                if ((string.IsNullOrEmpty(localZone) && Math.Abs(nowDate.datetime.Subtract(localDateUtc).TotalSeconds) > 3) ||
                    (!string.IsNullOrEmpty(localZone) && Math.Abs(nowDate.datetime.Subtract(localDate).TotalSeconds) > 3))
                {
                    WinDateTime.SetDateTime(nowDate.datetime, localZone);
                    Console.WriteLine(DateTime.Now.ToString("t") + ": Set correct time: " + nowDate.datetime.ToString("t"));
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
    }
}
