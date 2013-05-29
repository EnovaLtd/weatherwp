using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnovaWeatherSolution
{
    public static class Utility
    {
        public class Settings
        {
            public string Lat { get; set; }
            public string Long { get; set; }
            public string DisplayName { get; set; }

            public Settings()
            {
                Lat = "";
            }
        }

        public class ForecastMyProperty
        {
            public string timezone { get; set; }
            public string icon { get; set; }
            public int temperature { get; set; }
        }
    }
}
