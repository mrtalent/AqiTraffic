using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AqiTraffic.Utility;

namespace GenerateCases
{
    /// <summary>
    /// save Key-Value Pairs (road_id, date_time) -> weather/aqi in memory,
    /// ensure least query to Data-Base
    /// </summary>
    
    class Program
    {

        /// <summary>
        /// key: road id
        /// value: tuple(road level, lat value, lng value)
        /// </summary>
        static Dictionary<int, Tuple<int, double, double>> roadinfo;
        static RoadStationMap rsmap;
        static MemCache cache;
        static void SpeedLineParser(
            string str,
            out int rid,
            out int hhours,
            out double speed,
            out int supp,
            out double vari)
        {
            string[] tmparr = str.Split(' ');
            rid = int.Parse(tmparr[0]);
            hhours = int.Parse(tmparr[1]);
            speed = double.Parse(tmparr[2]);
            supp = int.Parse(tmparr[3]);
            vari = double.Parse(tmparr[4]);
        }

        static void GetRoadData(string remapPath)
        {
            Console.WriteLine("Loading Road Geography Info...\n");
            roadinfo = new Dictionary<int, Tuple<int, double, double>>();
            using (StreamReader sr = new StreamReader(remapPath))
            {
                string iline = null;
                while ((iline = sr.ReadLine()) != null)
                {
                    string[] strs = iline.Split(',');
                    roadinfo.Add(int.Parse(strs[0]),
                        new Tuple<int, double, double>(int.Parse(strs[1]),
                            double.Parse(strs[2]),
                            double.Parse(strs[3])));
                }
            }
        }


        /// <summary>
        /// 1. read road and speed records
        /// 2. 
        /// </summary>
        /// <param name="speedDir"></param>
        /// <param name="volumeDir"></param>
        static void GenerateCases(string speedDir, string volumeDir)
        {
            Console.WriteLine("Generate Cases...");
            List<string> speedFiles = new List<string>(Directory.EnumerateFiles(speedDir));
            string genDir = Path.Combine(Directory.GetParent(speedDir).FullName, "Cases");
            if(!Directory.Exists(genDir))
            {
                Directory.CreateDirectory(genDir);
            }
            for (int i = 0; i < speedFiles.Count; i++)
            {
                using (StreamReader srSpeed = new StreamReader(speedFiles[i]))
                {
                    Console.WriteLine("Processing file:\t" + speedFiles[i]);
                    string strDate = Path.GetFileNameWithoutExtension(speedFiles[i]);
                    using (StreamWriter sw = new StreamWriter(Path.Combine(genDir, strDate+".record")))
                    {
                        int process_cnt = 0;
                        int tot = 234048;
                        int pre = -1;
                        string iline = null;
                        while ((iline = srSpeed.ReadLine()) != null)
                        {
                            if(100 * process_cnt / tot != pre)
                            {
                                pre = 100 * process_cnt / tot;
                                Console.Write(pre + "%\r");
                            }
                            int rid = 0, supp = 0, hhours = 0, rlv = 0;
                            double speed = 0, vari = 0, lng = 0, lat = 0;
                            SpeedLineParser(iline, out rid, out hhours, out speed, out supp, out vari);
                            rlv = roadinfo[rid].Item1;
                            lng = roadinfo[rid].Item2;
                            lat = roadinfo[rid].Item3;
                            DateTime dt = new DateTime(
                                int.Parse(strDate.Substring(0, 4)),
                                int.Parse(strDate.Substring(4, 2)),
                                int.Parse(strDate.Substring(6)), hhours / 2, hhours % 2, 0);

                            //get weather and aqi data corresponding to coordinates
                            Tuple<double, double, double, int> weather = cache.GetWeather(rsmap.GetWeatherStationID(rid), dt);
                            double aqi = cache.GetAqi(rsmap.GetAqiStationID(rid), dt);
                            sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", speed, aqi,
                                rlv, weather.Item1, weather.Item2, weather.Item3, weather.Item4));
                            process_cnt++;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// args[]
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            cache = MemCache.GetInstance();
            rsmap = RoadStationMap.GetInstatnce();
            if (args.Length == 0)
            {
                rsmap.Initial(@"D:\Users\v-tianhe\aqiTraffic\Data\RoadStationMap");
                GetRoadData(@"D:\Users\v-tianhe\aqiTraffic\Data\TrafficEsti\RoadData\road_center_01.txt");
                GenerateCases(@"D:\Users\v-tianhe\aqiTraffic\Data\TrafficEsti\01SpeedPredicted",
                    @"D:\Users\v-tianhe\aqiTraffic\Data\TrafficEsti\Volume");
            }
            else
            {
                rsmap.Initial(args[3]);
                GetRoadData(args[2]);
                GenerateCases(args[0], args[1]);
            }
            cache.Close();
            return;
        }
    }
}
