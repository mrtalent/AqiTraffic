using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AqiTraffic.DataAccess;

namespace AqiTraffic.Utility
{
    public class MemCache
    {
        static WeatherDB dbWea = new WeatherDB();
        static AqiDB dbAqi = new AqiDB();
        Dictionary<Tuple<string, DateTime>, Weather> weaCache = new Dictionary<Tuple<string, DateTime>, Weather>();
        Dictionary<Tuple<string, DateTime>, double> aqiCache = new Dictionary<Tuple<string, DateTime>, double>();
        public Dictionary<Tuple<string, DateTime>, double> AqiCache
        {
            get
            {
                return aqiCache;
            }
        }
        public Dictionary<Tuple<string, DateTime>, Weather> WeatherCache
        {
            get
            {
                return weaCache;
            }
        }
        public Weather GetWeather(string sid, DateTime dt)
        {
            Tuple<string, DateTime> qr = new Tuple<string, DateTime>(sid, dt);
            if (weaCache.ContainsKey(qr))
            {
                return weaCache[qr];
            }
            Weather ret = dbWea.QueryWeather(sid, dt);
            weaCache.Add(qr, ret);
            return ret;
        }

        public double GetAqi(string sid, DateTime dt)
        {
            Tuple<string, DateTime> qr = new Tuple<string, DateTime>(sid, dt);
            if (weaCache.ContainsKey(qr))
            {
                return aqiCache[qr];
            }
            double ret = dbAqi.QueryAqi(sid, dt);
            aqiCache.Add(qr, ret);
            return ret;
        }

        public void Close()
        {
            dbAqi.Close();
            dbWea.Close();
        }
        static MemCache _inst = new MemCache();
        public static MemCache GetInstance()
        {
            return _inst;
        }
    }
}
