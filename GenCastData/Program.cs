using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AqiTraffic.Utility;
using System.IO;

namespace GenCastData
{
    class Program
    {

        static void Main(string[] args)
        {
            List<string> wsids = new List<string>();
            List<string> asids = new List<string>();
            MemCache cache = new MemCache();
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
            Console.WriteLine("Processing Air Quality Data...\n");
            using (StreamWriter sw = new StreamWriter("_aqi"))
            {
                sw.WriteLine("id,time,aqi");
                foreach (var aid in asids)
                {
                    Console.WriteLine("Air Quality Station\t" + aid);
                    DateTime sdt = new DateTime(2015, 7, 1);
                    DateTime edt = new DateTime(2015, 12, 21);
                    for (DateTime dt = sdt; dt < edt; sw.WriteLine(aid + ',' + dt + ',' + cache.GetAqi(aid, dt)), dt = dt.AddMinutes(15)) ;
                }
            }
            Console.WriteLine("Processing Weather Data...\n");
            using (StreamWriter sw = new StreamWriter("_wea"))
            {
                sw.WriteLine("id,time,rain");
                foreach (var wid in wsids)
                {
                    Console.WriteLine("Weather Station\t" + wid);
                    DateTime sdt = new DateTime(2015, 7, 1);
                    DateTime edt = new DateTime(2015, 12, 21);
                    for (DateTime dt = sdt; dt < edt; sw.WriteLine(wid + ',' + dt + ',' + cache.GetWeather(wid, dt)), dt = dt.AddMinutes(15)) ;
                }
            }
        }
    }
}
