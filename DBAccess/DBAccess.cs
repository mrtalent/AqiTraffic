using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace AqiTraffic.DataAccess
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
}
