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
        private static String server = "192.168.1.108"; 
        private static String database = "newerp";
        private static String userId = "postgres";
        private static String password = "shibin";

        NpgsqlConnection conn = new NpgsqlConnection($"Server={server};Port=5432;Database={database};User Id={userId};Password={password};CommandTimeout=1");
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

                  
                if (queryType == "select" || queryType=="insert")
                {

                    var dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    var y = dataTable;
                    string JSONString = string.Empty;
                    JSONString = JsonConvert.SerializeObject(dataTable);
                    JArray jsonData = JArray.Parse(JSONString);
                    return jsonData;


                }
                else
                {
                    JObject obj = new JObject();
                    obj.Add("isSuccess", "true");
                    JArray jsonData = new JArray();
                    jsonData.Add(obj);
                    return jsonData;
                }
            }
            catch
            {
                //JObject obj = new JObject();
                JArray jsonData = new JArray();
                //jsonData.Add(obj);
                return jsonData;
            }
            finally
            {
                conn.Close();
                //Mouse.OverrideCursor = null;
            }

        }

       
    }
}
