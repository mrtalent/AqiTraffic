using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace AqiTraffic.DataAccess
{
    public class AqiDB : DBAccess
    {
        public Dictionary<string, Tuple<double, double>> GetAqiLocation()
        {
            string fileName = "_loc1";
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
                string sql = "SELECT station_id,longitude,latitude FROM ruiyuan_test.dbo.Station where station_id like '001%'";
                SqlCommand cmd = new SqlCommand(sql, _conn);
                SqlDataReader sqlReader = cmd.ExecuteReader();
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    while (sqlReader.Read())
                    {
                        string tmp_sid = sqlReader.GetString(0);
                        double tmp_lng = Decimal.ToDouble(sqlReader.GetDecimal(1));
                        double tmp_lat = Decimal.ToDouble(sqlReader.GetDecimal(2));
                        sw.WriteLine(string.Format("{0},{1},{2}", tmp_sid, tmp_lng, tmp_lat));
                        ret.Add(tmp_sid, new Tuple<double, double>(tmp_lng, tmp_lat));
                    }
                }
                sqlReader.Close();
            }
            return ret;
        }
        public double QueryAqi(string stationID, DateTime startTime)
        {
            string sql = null;

            sql = string.Format("SELECT TOP 1 PM25_Concentration FROM AqiTraffic.dbo.AirQuality where station_id={0} "
            + "and DATEDIFF(MINUTE,'{1}', time) > 0 and DATEDIFF(MINUTE,'{2}', time) <= 0 ORDER BY time DESC",
            stationID.ToString(), startTime.AddDays(-4).ToString(), startTime.ToString());

            SqlCommand cmd = new SqlCommand(sql, _conn);
            SqlDataReader sqlReader = cmd.ExecuteReader();
            double ret = -1;
            sqlReader.Read();
            if (sqlReader.HasRows && !sqlReader.IsDBNull(0))
            {
                ret = sqlReader.GetFloat(0);
            }
            sqlReader.Close();
            return ret;
        }

        public List<Tuple<string, DateTime, double>> AllRecord(DateTime from, DateTime to)
        {
            List<Tuple<string, DateTime, double>> ret = new List<Tuple<string, DateTime, double>>();
            string sql = string.Format("SELECT station_id,update_time,PM25_Concentration FROM AqiTraffic.dbo.AirQuality where "
                + "DATEDIFF(MINUTE,'{0}', update_time) >= 0 and DATEDIFF(MINUTE,'{1}', update_time) < 0 ORDER BY update_time",
                from.ToString(), to.ToString());
            SqlCommand cmd = new SqlCommand(sql, _conn);
            SqlDataReader sqlReader = cmd.ExecuteReader();
            while (sqlReader.Read())
            {
                string id = sqlReader.GetString(0);
                DateTime dt = sqlReader.GetDateTime(1);
                double aqi = sqlReader.IsDBNull(2) ? -1 : sqlReader.GetFloat(2);
                ret.Add(new Tuple<string, DateTime, double>(id, dt, aqi));
            }
            sqlReader.Close();
            return ret;
        }
    }
}
