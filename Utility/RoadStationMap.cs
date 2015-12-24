using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AqiTraffic.Utility
{
    public class RoadStationMap
    {
        Dictionary<int, string> road_weaStation_map;
        Dictionary<int, string> road_aqiStation_map;
        public void Initial(string mapDir)
        {
            Console.WriteLine("Loading Road Station Map...\n");
            road_weaStation_map = new Dictionary<int, string>();
            road_aqiStation_map = new Dictionary<int, string>();
            using (StreamReader sr = new StreamReader(Path.Combine(mapDir, "_map1")))
            {
                string iline = null;
                while ((iline = sr.ReadLine()) != null)
                {
                    string[] strs = iline.Split(',');
                    road_aqiStation_map.Add(int.Parse(strs[0]), strs[1]);
                }
            }
            using (StreamReader sr = new StreamReader(Path.Combine(mapDir, "_map2")))
            {
                string iline = null;
                while ((iline = sr.ReadLine()) != null)
                {
                    string[] strs = iline.Split(',');
                    road_weaStation_map.Add(int.Parse(strs[0]), strs[1]);
                }
            }
        }
        public string GetWeatherStationID(int road_id)
        {
            return road_weaStation_map[road_id];
        }
        public string GetAqiStationID(int road_id)
        {
            return road_aqiStation_map[road_id];
        }
        static RoadStationMap _inst = new RoadStationMap();
        public static RoadStationMap GetInstatnce()
        {
            return _inst;
        }
    }
}
