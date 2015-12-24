using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AqiTraffic.DataAccess;
using AqiTraffic.Utility;

namespace GenerateCasesCW
{
    class Program
    {
        static RoadStationMap rsmap;
        static MemCache cache;

        static void Main(string[] args)
        {
            cache = MemCache.GetInstance();
            rsmap = RoadStationMap.GetInstatnce();
        }
    }
}
