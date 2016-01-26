using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AqiTraffic.DataAccess;

namespace AqiTraffic.Utility
{
    public class DBCache
    {
        static WeatherDB dbWea = new WeatherDB();
        static AqiDB dbAqi = new AqiDB();
        public Dictionary<Tuple<string, DateTime>, Weather> weaCache = new Dictionary<Tuple<string, DateTime>, Weather>();
        public Dictionary<Tuple<string, DateTime>, double> aqiCache = new Dictionary<Tuple<string, DateTime>, double>();
        int CompareRecord(Tuple<string, DateTime, Weather> x, Tuple<string, DateTime, Weather> y)
        {
            int tmp = x.Item1.CompareTo(y.Item1);
            if (tmp != 0)
            {
                return tmp;
            }
            return x.Item2.CompareTo(y.Item2);
        }
        int CompareRecord2(Tuple<string, DateTime, double> x, Tuple<string, DateTime, double> y)
        {
            int tmp = x.Item1.CompareTo(y.Item1);
            if (tmp != 0)
            {
                return tmp;
            }
            return x.Item2.CompareTo(y.Item2);
        }
        public DBCache(DateTime from, DateTime to)
        {
            Console.WriteLine("Weather Cache: Querying DataBase...");
            List<Tuple<string, DateTime, Weather>> resw = dbWea.AllRecord(from.AddDays(-5), to);
            dbWea.Close();
            Console.WriteLine("Weather Cache: Gen Mean Timeslot Records...");
            resw.Sort(CompareRecord);
            var data = resw
                .GroupBy(o => o.Item1)
                .Select(o => o.ToList())
                .ToList();
            foreach (var ustation in data)
            {
                Console.WriteLine("Weather Station: "+ustation[0].Item1);
                int pt = 0;
                Weather last_record = null;
                for (DateTime dt = from; dt < to; dt = dt.AddMinutes(15))
                {
                    while (pt != ustation.Count && ustation[pt].Item2 < dt)
                    {
                        last_record = ustation[pt].Item3;
                        pt++;
                    }
                    weaCache.Add(new Tuple<string, DateTime>(ustation[0].Item1, dt), last_record);
                }
            }


            Console.WriteLine("Aqi Cache: Querying DataBase...");
            List<Tuple<string, DateTime, double>> resa = dbAqi.AllRecord(from.AddDays(-5), to);
            dbAqi.Close();
            resa.Sort(CompareRecord2);
            var aqidata = resa.GroupBy(o => o.Item1).Select(o => o.ToList()).ToList();
            foreach (var ustation in aqidata)
            {
                Console.WriteLine("AQI Station: " + ustation[0].Item1);
                int pt = 0;
                double last_record = -1;
                for (DateTime dt = from; dt < to; dt = dt.AddMinutes(15))
                {
                    while (pt != ustation.Count && ustation[pt].Item2 < dt)
                    {
                        last_record = ustation[pt].Item3;
                        pt++;
                    }
                    aqiCache.Add(new Tuple<string, DateTime>(ustation[0].Item1, dt), last_record);
                }
            }

        }
        public Weather GetWeather(string sid, DateTime dt)
        {
            return weaCache[new Tuple<string, DateTime>(sid, dt)];
        }
        public double GetAqi(string sid, DateTime dt)
        {
            return aqiCache[new Tuple<string, DateTime>(sid, dt)];
        }
    }
}
