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

        static void Main(string[] args)
        {
            List<string> wsids = new List<string>();
            List<string> asids = new List<string>();
            DBCache weaData;
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
            weaData = new DBCache(sdt, edt);
            /*
            Console.WriteLine("Processing Air Quality Data...\n");
            using (StreamWriter sw = new StreamWriter("_aqi"))
            {
                sw.WriteLine("id,time,aqi");
                foreach (var aid in asids)
                {
                    Console.WriteLine("Air Quality Station\t" + aid);
                    cnt = 0;
                    for (DateTime dt = sdt; dt < edt; cnt++, dt = dt.AddMinutes(15))
                    {
                        Console.Write((cnt * 100 / tot) + "%\r");
                        sw.WriteLine(aid + ',' + dt + ',' + cache.GetAqi(aid, dt));
                    }
                }
            }*/
            Console.WriteLine("Processing Weather Data...\n");
            using (StreamWriter sw = new StreamWriter("_wea"))
            {
                sw.WriteLine("id,time,rain");
                foreach (var wid in wsids)
                {
                    Console.WriteLine("Weather Station\t" + wid);
                    for (DateTime dt = sdt; dt < edt;  dt = dt.AddMinutes(15))
                    {
                        Weather res = weaData.GetWeather(wid,dt);
                        sw.WriteLine(wid + ',' + dt + ',' + res.rain + ',' + res.temperature + ',' + res.windSpeed + ',' + res.label);
                    }
                }
            }
        }
    }
}
