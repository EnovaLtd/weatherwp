using ForecastIO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Web;
using EnovaWeatherSolution;
using Microsoft.SharePoint.WebControls;


namespace EnovaWeatherSolution.Weather_WP
{
    [ToolboxItemAttribute(false)]
    public partial class Weather_WP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]

        [WebBrowsable(true),
       WebDisplayName("API Key"),
       Personalizable(PersonalizationScope.Shared),
       Category("Enova Custom Properties")]
        public string APIKey { get; set; }

        [WebBrowsable(true),
        WebDisplayName("Locations"),
        Personalizable(PersonalizationScope.Shared),
        Category("Enova Custom Properties")]
        public string Locations { get; set; }

        [WebBrowsable(true),
        WebDisplayName("Display Type"),
        Personalizable(PersonalizationScope.Shared),
        Category("Enova Custom Properties")]
        public DisplayType DisplayTypeName { get; set; }

        [WebBrowsable(true),
        WebDisplayName("Display Duration"),
        Personalizable(PersonalizationScope.Shared),
        Category("Enova Custom Properties")]
        public string DisplayDuration { get; set; }

        [WebBrowsable(true),
        WebDisplayName("Cache Lifetime"),
        Personalizable(PersonalizationScope.Shared),
        Category("Enova Custom Properties")]
        public string CacheLifetime { get; set; }

        public enum DisplayType
        {
            slideleft,
            allvertical,
            allhorizantal,
            slideright,
            slideup,
            slidedown,
            fade
        }

        public Weather_WP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.Locations))
            {
            

                if (SPContext.Current.FormContext.FormMode == SPControlMode.Edit)
                {
                    HttpContext.Current.Cache.Remove("ForecastCache");
                }
                else
                {
                    var responses = new List<Utility.ForecastMyProperty>();

                    if (HttpContext.Current.Cache["ForecastCache"] != null)
                    {
                        responses = (List<Utility.ForecastMyProperty>)HttpContext.Current.Cache["ForecastCache"];
                    }
                    else
                    {
                        var settings = this.GetLocationsLatitude(this.Locations);
                       
                        foreach (var setting in settings.Keys)
                        {
                            var value = settings[setting];
                            var request = new ForecastIORequest(APIKey, value.Lat, value.Long, value.DisplayName, Unit.si);
                            var response = request.Get();
                            responses.Add(new Utility.ForecastMyProperty()
                            {
                                timezone = value.DisplayName,
                                temperature = Convert.ToInt32(response.currently.temperature),
                                icon = response.currently.icon
                            });
                        }

                       HttpContext.Current.Cache.Add("ForecastCache", responses, null, DateTime.Now.AddSeconds(Convert.ToDouble(this.CacheLifetime)), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, null);
                    }

                    rptForecast.DataSource = responses;
                    rptForecast.DataBind();
                }         
            }
        }

        private IDictionary<int, Utility.Settings> GetLocationsLatitude(string location)
        {
            IDictionary<int, Utility.Settings> result = new Dictionary<int, Utility.Settings>();

            var settings = location.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            if (settings.Length > 0)
            {
                int index = 1;

                foreach (string setting in settings)
                {
                    var v = setting.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (v.Length == 3)
                    {
                        var _lat = v[0];
                        var _long = v[1];
                        var _display = v[2];
                        result.Add(index++, new Utility.Settings()
                        {
                            DisplayName = _display,
                            Lat = _lat,
                            Long = _long
                        });
                    }
                }
            }

            return result;
        }

        //public static IList<string> GetLocationsLongtitude(string location)
        //{
        //    IList<string> locLongtitudeList;

        //    string[] locsArray = location.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

        //    IList<string> list = new List<string>();

        //    foreach (string l in locsArray)
        //    {
        //        string[] latValue = l.Split(new char[] { ',' });
        //        list.Add(latValue[1]);
        //    }

        //    locLongtitudeList = list;

        //    return locLongtitudeList;
        //}

        //public static IList<string> GetLocationsDisplayName(string location)
        //{
        //    IList<string> locDisplayNameList;

        //    string[] locsArray = location.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

        //    IList<string> list = new List<string>();

        //    foreach (string l in locsArray)
        //    {
        //        string[] latValue = l.Split(new char[] { ',' });
        //        list.Add(latValue[2]);
        //    }

        //    locDisplayNameList = list;

        //    return locDisplayNameList;
        //}

    }
}
