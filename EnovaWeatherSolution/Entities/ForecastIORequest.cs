﻿using ForecastIO.Extensions;
using System;
using System.Globalization;
using System.Web.Script.Serialization;

namespace ForecastIO
{
    public class ForecastIORequest
    {
        private string apiKey;
        private string latitude;
        private string longitude;
        private string unit;
        private string exclude;
        private string time;
        private string displayName;

        private static string currentForecastURL = "https://api.forecast.io/forecast/{0}/{1},{2}?units={3}&exclude={4}";
        private static string periodForecastURL = "https://api.forecast.io/forecast/{0}/{1},{2},{3}?units={4}&exclude={5}";

        public ForecastIOResponse Get()
        {
            var url = (time == null) ? String.Format(currentForecastURL, apiKey, latitude, longitude, unit, exclude) :
                String.Format(periodForecastURL, apiKey, latitude, longitude, time, unit, exclude);

            string result;
            using (CompressionEnabledWebClient client = new CompressionEnabledWebClient())
            {
                result = RequestHelpers.FormatResponse(client.DownloadString(url));
            }

            var serializer = new JavaScriptSerializer();
            var dataObject = serializer.Deserialize<ForecastIOResponse>(result);

            return dataObject;

        }

        public ForecastIORequest(string _apiKey, string _lat, string _long, string _displayname, Unit _unit, params Exclude[] _exclude)
        {
            apiKey = _apiKey;
            latitude = _lat.ToString(CultureInfo.InvariantCulture);
            longitude = _long.ToString(CultureInfo.InvariantCulture);
            displayName = _displayname;
            unit = Enum.GetName(typeof(Unit), _unit);
            exclude = RequestHelpers.FormatExcludeString(_exclude);
        }

        
        //public ForecastIORequest(string _apiKey, float _lat, float _long, DateTime _time, Unit _unit, params Exclude[] _exclude)
        //{
        //    apiKey = _apiKey;
        //    latitude = _lat.ToString(CultureInfo.InvariantCulture);
        //    longitude = _long.ToString(CultureInfo.InvariantCulture);
        //    time = _time.ToUTCString();
        //    unit = Enum.GetName(typeof(Unit), _unit);
        //    exclude = RequestHelpers.FormatExcludeString(_exclude);
        //}
    }
}
