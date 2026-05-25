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
        private static string _conn =
            ConfigurationManager.ConnectionStrings["SGIPCDb"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_conn);
        }
    }
}