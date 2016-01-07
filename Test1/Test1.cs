using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Test1
{
    public class DBAccess
    {
        protected string _connectionString = "data source=URBCOMP01;initial catalog=ruiyuan_test_2015_12_19_before;user id=sa;password=abcd1234!;Timeout=30000;";
        protected SqlConnection _conn;
        public DBAccess()
        {
            _conn = new SqlConnection(_connectionString);
            _conn.Open();
        }
        public void Close()
        {
            _conn.Close();
        }
    }

    class Test1
    {
        static void TestDB()
        {
            string _connectionString = "data source=URBCOMP01;initial catalog=ruiyuan_test_2015_12_19_before;user id=sa;password=abcd1234!;Timeout=30000;";
            SqlConnection _conn;
            _conn = new SqlConnection(_connectionString);
            _conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT TOP 100 PM25_Concentration,time FROM ruiyuan_test.dbo.AirQuality where station_id='001025' and DATEDIFF(MINUTE, '2014-02-11 16:00:00', time) > 0"
                , _conn);
            SqlDataReader sqlReader = cmd.ExecuteReader();
            if (!sqlReader.HasRows)
            {
                Console.WriteLine("fff");
            }
            while (sqlReader.Read())
            {
                Console.WriteLine(sqlReader[0]);
            }
            _conn.Close();
        }
        static void Main(string[] args)
        {

            TestDB();
        }
    }
}
