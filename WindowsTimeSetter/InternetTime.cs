using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsTimeSync
{
    public class InternetTime
    {
        public string client_ip { get; set; }
        public DateTime datetime { get; set; }
    }

    public class InternetTimeZone
    {
        public string status { get; set; }
        public string message { get; set; }
        public string countryCode { get; set; }
        public string countryName { get; set; }
        public string regionName { get; set; }
        public string cityName { get; set; }
        public string zoneName { get; set; }
        public string abbreviation { get; set; }
        public int gmtOffset { get; set; }
        public string dst { get; set; }
        public int zoneStart { get; set; }
        public object zoneEnd { get; set; }
        public object nextAbbreviation { get; set; }
        public int timestamp { get; set; }
        public string formatted { get; set; }
    }

}
