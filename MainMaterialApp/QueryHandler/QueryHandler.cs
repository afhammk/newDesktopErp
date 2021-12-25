using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;


namespace MainMaterialApp.QueryHandler
{
  

    class QueryHandler
    {
        private static String server = "localhost"; 
        private static String database = "DesktopDatabase";
        private static String userId = "postgres";
        private static String password = "admin";

        NpgsqlConnection conn = new NpgsqlConnection($"Server={server};Port=5432;Database={database};User Id={userId};Password={password};Timeout=3");
        NpgsqlCommand comm = new NpgsqlCommand();

        public JArray HandleQuery(string query , string queryType )
        {

            try
            {
                //Thread.Sleep(1500);
                conn.Open();
                comm.Connection = conn;
                comm.CommandType = CommandType.Text;
                comm.CommandText = query;
                NpgsqlDataReader dataReader = comm.ExecuteReader();

                var dataTable = new DataTable();
                dataTable.Load(dataReader);
                var y = dataTable;
                string JSONString = string.Empty;
          
                JSONString = JsonConvert.SerializeObject(dataTable);
                JArray jsonData = JArray.Parse(JSONString);
                return jsonData;

            }
            catch
            {           
                return null;
            }
            finally
            {
                conn.Close();
                //Mouse.OverrideCursor = null;
            }

        }

       
    }
}
