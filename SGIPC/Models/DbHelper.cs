using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using System.Data.SqlClient;

namespace SGIPC.Models
{
    public class DbHelper
    {
        private static string _conn;

        public static SqlConnection GetConnection()
        {
            if (_conn == null)
            {
                var connString = ConfigurationManager.ConnectionStrings["SGIPCDb"];
                if (connString == null)
                {
                    throw new ConfigurationErrorsException("Connection string 'SGIPCDb' not found in web.config");
                }
                _conn = connString.ConnectionString;
            }
            return new SqlConnection(_conn);
        }
    }
}