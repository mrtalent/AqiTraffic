using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AqiTraffic.DataAccess;
using System.IO;

namespace GeneratRoadStationMap
{
    class Program
    {

        static WeatherDB dbWea = new WeatherDB();
        static AqiDB dbAqi = new AqiDB();
        static Dictionary<string, Tuple<double, double>> aqiLocation;
        static Dictionary<string, Tuple<double, double>> weaLocation;
        static Dictionary<int, Tuple<int, double, double>> remap;
        static void GetRoadCoords(string remapPath)
        {
            remap = new Dictionary<int, Tuple<int, double, double>>();
            using (StreamReader sr = new StreamReader(remapPath))
            {
                string iline = null;
                while ((iline = sr.ReadLine()) != null)
                {
                    string[] strs = iline.Split(',');
                    remap.Add(int.Parse(strs[0]),
                        new Tuple<int, double, double>(int.Parse(strs[1]),
                            double.Parse(strs[2]),
                            double.Parse(strs[3])));
                }
            }
        }
        /// <summary>
        /// generate mapping between roads and stations,
        /// according to the geo-info, using NN
        /// </summary>
        static void road_weather_mapping(string storePath)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(storePath, "_map2")))
            {
                foreach (var item in remap)
                {

                    int rid = item.Key;

                    double lng = item.Value.Item2;
                    double lat = item.Value.Item3;
                    double min_dis = 10000;
                    string min_sid = "";
                    foreach (var item2 in weaLocation)
                    {
                        string sid = item2.Key;
                        double slng = item2.Value.Item1;
                        double slat = item2.Value.Item2;
                        double dis = Math.Sqrt((slat - lat) * (slat - lat) + (slng - lng) * (slng - lng));
                        if (dis < min_dis)
                        {
                            min_dis = dis;
                            min_sid = sid;
                        }
                    }
                    sw.WriteLine(string.Format("{0},{1}", rid, min_sid));
                }
            }
        }
        static void road_aqi_mapping(string storePath)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(storePath, "_map1")))
            {
                foreach (var item in remap)
                {
                    int rid = item.Key;
                    double lng = item.Value.Item2;
                    double lat = item.Value.Item3;
                    double min_dis = 10000;
                    string min_sid = "";
                    foreach (var item2 in aqiLocation)
                    {
                        string sid = item2.Key;
                        double slng = item2.Value.Item1;
                        double slat = item2.Value.Item2;
                        double dis = Math.Sqrt((slat - lat) * (slat - lat) + (slng - lng) * (slng - lng));
                        if (dis < min_dis)
                        {
                            min_dis = dis;
                            min_sid = sid;
                        }
                    }
                    sw.WriteLine(string.Format("{0},{1}", rid, min_sid));
                }
            }
        }
        static void Main(string[] args)
        {
            aqiLocation = dbAqi.GetAqiLocation();
            weaLocation = dbWea.GetWeatherLocation();
            if (args.Length == 0)
            {
                GetRoadCoords(@"D:\Users\v-tianhe\aqiTraffic\Data\TrafficEsti\RoadData\road_center_01.txt");
                if(!Directory.Exists(@"D:\Users\v-tianhe\aqiTraffic\Data\RoadStationMap"))
                {
                    Directory.CreateDirectory(@"D:\Users\v-tianhe\aqiTraffic\Data\RoadStationMap");
                }
                road_weather_mapping(@"D:\Users\v-tianhe\aqiTraffic\Data\RoadStationMap");
                road_aqi_mapping(@"D:\Users\v-tianhe\aqiTraffic\Data\RoadStationMap");
            }
            else
            {
                GetRoadCoords(args[0]);
                if (!Directory.Exists(args[1]))
                {
                    Directory.CreateDirectory(args[1]);
                }
                road_weather_mapping(args[1]);
                road_aqi_mapping(args[1]);
            }
        }
    }
}
