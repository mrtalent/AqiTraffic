using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AqiTraffic.DataAccess;
using System.IO;

namespace GeneratRoadStationMapCW
{
    class Program
    {

        static WeatherDB dbWea = new WeatherDB();
        static AqiDB dbAqi = new AqiDB();
        static Dictionary<string, Tuple<double, double>> aqiLocation;
        static Dictionary<string, Tuple<double, double>> weaLocation;
        static Dictionary<string, int> rlvmap;

        static double distance(double lng1, double lat1, double lng2, double lat2)
        {
            double coef = Math.Cos((lat1 + lat2) / 2 / 180 * Math.PI);
            double dlng = lng2 - lng1;
            double dlat = lat2 - lat1;
            return Math.Sqrt((dlng * dlng * coef * coef) + (dlat * dlat));
        }
        static string NeareastNeighbor(double lng, double lat, Dictionary<string, Tuple<double, double>> loc)
        {
            double mindis = 999999999;
            string minid = null;
            foreach (var item in loc)
            {
                double tlng = item.Value.Item1;
                double tlat = item.Value.Item2;
                string weaid = item.Key;
                double tdis = distance(tlng, tlat, lng, lat);
                if (tdis < mindis)
                {
                    minid = weaid;
                    mindis = tdis;
                }
            }
            return minid;
        }
        static string GetNeareastWS(double lng, double lat)
        {
            return NeareastNeighbor(lng, lat, weaLocation);
        }
        static string GetNeareastAS(double lng, double lat)
        {
            return NeareastNeighbor(lng, lat, aqiLocation);
        }

        static void GenRSMaps(string roadCoordsFile, string storePath)
        {
            using (StreamReader sr = new StreamReader(roadCoordsFile))
            {
                StreamWriter swAqi = new StreamWriter(Path.Combine(storePath, "_map1"));
                StreamWriter swWea = new StreamWriter(Path.Combine(storePath, "_map2"));
                string iline = null;
                while ((iline = sr.ReadLine()) != null)
                {
                    string[] strs = iline.Split(',');
                    List<string> towAqi =new List<string>(new string[] { strs[0] });
                    List<string> towWea = new List<string>(new string[] { strs[0] });
                    for (int i = 1; i < strs.Length; i++)
                    {
                        string segstr = strs[i];
                        IEnumerable<double> vals = segstr.Split(' ').Select(o => double.Parse(o));
                        List<double> lngs = vals.Where((o, ind) => ind % 2 == 0).ToList();
                        List<double> lats = vals.Where((o, ind) => ind % 2 == 1).ToList();
                        for (int j = 0; j < lngs.Count; j++)
                        {
                            string nna = GetNeareastAS(lngs[j], lats[j]);
                            if(!towAqi.Contains(nna))
                            {
                                towAqi.Add(nna);
                            }
                            string nnw = GetNeareastWS(lngs[j], lats[j]);
                            if(!towWea.Contains(nnw))
                            {
                                towWea.Add(nnw);
                            }
                        }
                    }
                    swAqi.WriteLine(string.Join(",", towAqi));
                    swWea.WriteLine(string.Join(",", towWea));
                }
                swAqi.Flush();
                swWea.Flush();
                swAqi.Dispose();
                swWea.Dispose();
            }
        }
        static void Main(string[] args)
        {
            aqiLocation = dbAqi.GetAqiLocation();
            weaLocation = dbWea.GetWeatherLocation();
            if (args.Length == 0)
            {
                if (!Directory.Exists(@"D:\Users\v-tianhe\aqiTraffic\Data\RoadStationMapCW"))
                {
                    Directory.CreateDirectory(@"D:\Users\v-tianhe\aqiTraffic\Data\RoadStationMapCW");
                }
                GenRSMaps(@"D:\Users\v-tianhe\aqiTraffic\Data\Beijing\01road\road_coords", @"D:\Users\v-tianhe\aqiTraffic\Data\RoadStationMapCW");
            }
            else
            {
                if (!Directory.Exists(args[1]))
                {
                    Directory.CreateDirectory(args[1]);
                }
                GenRSMaps(args[0], args[1]);
            }
        }
    }
}
