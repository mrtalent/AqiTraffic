using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace AqiTraffic.DataAccess
{
    public class WeatherDB : DBAccess
    {
        double[] monthMeanTempe = new double[] { -3.5, -0.5, 6, 14, 19, 24.5, 26.5, 25.5, 20.5, 11.5, 13.5, 5, -1.5 };
        
        private int NULLINT = -9999;
        public Dictionary<string, Tuple<double, double>> GetWeatherLocation()
        {
            string fileName = "_loc2";
            Dictionary<string, Tuple<double, double>> ret = new Dictionary<string, Tuple<double, double>>();
            if (File.Exists(fileName))
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string iline = null;
                    while ((iline = sr.ReadLine()) != null)
                    {
                        string[] strs = iline.Split(',');
                        ret.Add(strs[0], new Tuple<double, double>(double.Parse(strs[1]), double.Parse(strs[2])));
                    }
                }
            }
            else
            {
                string sql = "SELECT weather_station_id,longitude,latitude FROM ruiyuan_test.dbo.WeatherStation where weather_station_id like '001%'";
                SqlCommand cmd = new SqlCommand(sql, _conn);
                SqlDataReader sqlReader = cmd.ExecuteReader();
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    while (sqlReader.Read())
                    {
                        string sid = sqlReader.GetString(0);
                        double lng = sqlReader.GetFloat(1);
                        double lat = sqlReader.GetFloat(2);
                        sw.WriteLine(string.Format("{0},{1},{2}", sid, lng, lat));
                        ret.Add(sid, new Tuple<double, double>(lng, lat));
                    }
                }
                sqlReader.Close();
            }
            return ret;
        }
        /// <summary>
        /// temporarily return RainFall
        /// </summary>
        /// <param name="stationID"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public Weather QueryWeather(string stationID, DateTime startTime)
        {
            string sql = null;
            sql = string.Format("SELECT TOP 1 RainFall,Temperature,WindSpeed,Weather FROM AqiTraffic.dbo.Meteorology where id = {0} " +
            "and DATEDIFF(MINUTE,'{1}', update_time) > 0 and DATEDIFF(MINUTE,'{2}', update_time) <= 0 ORDER BY update_time DESC",
            stationID.ToString(), startTime.AddDays(-1).ToString(), startTime.ToString());
            SqlCommand cmd = new SqlCommand(sql, _conn);
            SqlDataReader sqlReader = cmd.ExecuteReader();
            double rain = NULLINT, temperature = NULLINT, wind = NULLINT;
            int weather = NULLINT;
            sqlReader.Read();
            if (sqlReader.HasRows)
            {
                if (!sqlReader.IsDBNull(0))
                {
                    rain = sqlReader.GetFloat(0);
                }
                if (!sqlReader.IsDBNull(1))
                {
                    temperature = sqlReader.GetFloat(1);
                }
                if (!sqlReader.IsDBNull(2))
                {
                    wind = sqlReader.GetFloat(2);
                }
                if (!sqlReader.IsDBNull(3))
                {
                    weather = sqlReader.GetInt16(3);
                }
            }
            sqlReader.Close();
            return new Weather(rain, temperature, wind, weather);
        }

        public List<Tuple<string, DateTime, Weather>> AllRecord(DateTime from, DateTime to)
        {
            List<Tuple<string, DateTime, Weather>> ret = new List<Tuple<string, DateTime, Weather>>();
            string sql = string.Format("SELECT id,update_time,RainFall,Temperature,WindSpeed,Weather FROM AqiTraffic.dbo.Meteorology where "
                + "DATEDIFF(MINUTE,'{0}', update_time) >= 0 and DATEDIFF(MINUTE,'{1}', update_time) < 0 ORDER BY update_time",
                from.ToString(), to.ToString());
            SqlCommand cmd = new SqlCommand(sql, _conn);
            SqlDataReader sqlReader = cmd.ExecuteReader();
            while (sqlReader.Read())
            {
                string id = sqlReader.GetString(0);
                DateTime dt = sqlReader.GetDateTime(1);
                // fill missing value
                double rain = sqlReader.IsDBNull(2) ? 0 : sqlReader.GetFloat(2);
                double temperature = sqlReader.IsDBNull(3) ? monthMeanTempe[dt.Month-1] : sqlReader.GetFloat(3);
                double wind = sqlReader.IsDBNull(4) ? 0 : sqlReader.GetFloat(4);
                int label = sqlReader.IsDBNull(5) ? 0 : sqlReader.GetInt16(5);
                ret.Add(new Tuple<string, DateTime, Weather>(id, dt, new Weather(rain, temperature, wind, label)));
            }
            sqlReader.Close();
            return ret;
        }
    }
}
