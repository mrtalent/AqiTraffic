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
        Dictionary<Tuple<string, DateTime>, double> weaCache = new Dictionary<Tuple<string, DateTime>, double>();
        Dictionary<Tuple<string, DateTime>, double> aqiCache = new Dictionary<Tuple<string, DateTime>, double>();

        public double GetWeather(string sid, DateTime dt)
        {
            Tuple<string, DateTime> qr = new Tuple<string, DateTime>(sid, dt);
            if (weaCache.ContainsKey(qr))
            {
                return weaCache[qr];
            }
            double ret = dbWea.QueryWeather(sid, dt);
            weaCache.Add(qr, ret);
            return ret;
        }

        public double GetAqi(string sid, DateTime dt)
        {
            Tuple<string, DateTime> qr = new Tuple<string, DateTime>(sid, dt);
            if (weaCache.ContainsKey(qr))
            {
                return weaCache[qr];
            }
            double ret = dbAqi.QueryAqi(sid, dt);
            weaCache.Add(qr, ret);
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
