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
                string sql = "SELECT weather_station_id,longitude,latitude FROM ruiyuan_test_2015_12_19_before.dbo.WeatherStation where weather_station_id like '001%'";
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
        public Tuple<double,double,double,int> QueryWeather(string stationID, DateTime startTime)
        {
            string sql = null;
            if (startTime < new DateTime(2015, 12, 20))
            {
                sql = string.Format("SELECT TOP 1 RainFall,Temperature,WindSpeed,Weather FROM ruiyuan_test_2015_12_19_before.dbo.Meteorology where id = {0} " +
                "and DATEDIFF(MINUTE,'{1}', update_time) > 0 and DATEDIFF(MINUTE,'{2}', update_time) <= 0 ORDER BY update_time DESC",
                stationID.ToString(), startTime.AddDays(-1).ToString(), startTime.ToString());
            }
            else
            {
                sql = string.Format("SELECT TOP 1 RainFall,Temperature,WindSpeed,Weather FROM ruiyuan_test.dbo.Meteorology where id = {0} " +
                "and DATEDIFF(MINUTE,'{1}', update_time) > 0 and DATEDIFF(MINUTE,'{2}', update_time) <= 0 ORDER BY update_time DESC",
                stationID.ToString(), startTime.AddDays(-1).ToString(), startTime.ToString());
            }

            SqlCommand cmd = new SqlCommand(sql, _conn);
            SqlDataReader sqlReader = cmd.ExecuteReader();
            double rain = -9999, temperature = -9999, wind = -9999;
            int weather =-9999;
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
            return new Tuple<double, double, double, int>(rain, temperature, wind, weather);
        }
    }
}
