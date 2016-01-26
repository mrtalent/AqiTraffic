using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AqiTraffic.Utility;
using AqiTraffic.DataAccess;
using System.IO;

namespace GenCastData
{
    class Program
    {
        delegate double del(double x1, double y1, double x2, double y2);
        static Dictionary<Tuple<string, DateTime>, double> FillMissingAQI(DBCache castData, DateTime from, DateTime to)
        {
            Dictionary<Tuple<string, DateTime>, double> ret = new Dictionary<Tuple<string, DateTime>, double>();
            AqiDB dbAqi = new AqiDB();
            Dictionary<string, Tuple<double, double>> loc = dbAqi.GetAqiLocation();
            List<string> aslist = loc.Keys.ToList();
            del Dis = (x1, y1, x2, y2) => (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);

            // Sort Nearest Stations For Each Station
            Dictionary<string, List<string>> nn = aslist.Select(
                    centr => aslist.OrderBy(
                    near => Dis(loc[near].Item1, loc[near].Item2, loc[centr].Item1, loc[centr].Item2))
                    .ToList()).ToDictionary(o => o[0]);

            for (DateTime dt = from; dt < to; dt.AddMinutes(15))
            {
                foreach (string sid in aslist)
                {
                    // Find nearest, at most, 3 stations and alternate with average of them if value missed
                    if (castData.GetAqi(sid, dt) == -1)
                    {
                        // The number of neighbors having valid values
                        int has_val_cnt = 0;
                        // Sum of Aqi of neighbors
                        double nn_aqi_sum = 0;
                        int pt = 1;
                        while (has_val_cnt < 3)
                        {
                            double tmp = castData.GetAqi(nn[sid][pt], dt);
                            if (tmp != -1)
                            {
                                nn_aqi_sum += tmp;
                                has_val_cnt += 1;
                            }
                            pt += 1;
                        }
                        if (has_val_cnt != 0)
                        {
                            ret[new Tuple<string, DateTime>(sid, dt)] = nn_aqi_sum / has_val_cnt;
                        }
                    }
                    else
                    {
                        ret[new Tuple<string, DateTime>(sid, dt)] = castData.GetAqi(sid, dt);
                    }
                }
            }
            return ret;
        }
        static void Main(string[] args)
        {
            List<string> wsids = new List<string>();
            List<string> asids = new List<string>();
            DBCache castData;
            using (StreamReader sr = new StreamReader(
                @"D:\Users\v-tianhe\aqiTraffic\Code\AqiTraffic\GenRoadStationMapCW\bin\Debug\_loc1"))
            {
                string iline = null;
                while ((iline = sr.ReadLine()) != null)
                {
                    asids.Add(iline.Split(',')[0]);
                }
            }
            using (StreamReader sr = new StreamReader(
                @"D:\Users\v-tianhe\aqiTraffic\Code\AqiTraffic\GenRoadStationMapCW\bin\Debug\_loc2"))
            {
                string iline = null;
                while ((iline = sr.ReadLine()) != null)
                {
                    wsids.Add(iline.Split(',')[0]);
                }
            }
            DateTime sdt = new DateTime(2015, 7, 1);
            DateTime edt = new DateTime(2015, 12, 31);
            castData = new DBCache(sdt, edt);


            // AQI Data Processing
            Console.WriteLine("Processing Air Quality Data...\n");
            using (StreamWriter sw = new StreamWriter("_aqi"))
            {
                sw.WriteLine("id,time,aqi");
                foreach (var aid in asids)
                {
                    int cnt = 0;
                    for (DateTime dt = sdt; dt < edt; cnt++, dt = dt.AddMinutes(15))
                    {
                        sw.WriteLine(aid + ',' + dt + ',' + castData.GetAqi(aid, dt));
                    }
                }
            }


            Console.WriteLine("Processing Weather Data...\n");
            using (StreamWriter sw = new StreamWriter("_wea"))
            {
                sw.WriteLine("id,time,rain");
                foreach (var wid in wsids)
                {
                    Console.WriteLine("Weather Station\t" + wid);
                    for (DateTime dt = sdt; dt < edt; dt = dt.AddMinutes(15))
                    {
                        Weather res = castData.GetWeather(int.Parse(wid).ToString(), dt);
                        sw.WriteLine(wid + ',' + dt + ',' + res.rain + ',' + res.temperature + ',' + res.windSpeed + ',' + res.label);
                    }
                }
            }
        }
    }
}
