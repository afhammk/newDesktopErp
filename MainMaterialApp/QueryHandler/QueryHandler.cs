using System;
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
        NpgsqlConnection conn = new NpgsqlConnection("Server=192.168.1.108;Port=5432;Database=newerp;User Id=postgres;Password=shibin;");
        NpgsqlCommand comm = new NpgsqlCommand();

        public void HandleQuery(string query , string queryType , Action<JArray> method)
        {
             
            List<dynamic> parameters = new List<dynamic>();
            parameters.Add(query);
            parameters.Add(queryType);
            parameters.Add(method);
           

            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync(parameters);

            
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var par = e.Result as List<dynamic>;
            var res = par[0] as JArray;
            var action = par[1] as Action<JArray>;
            action(res);
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<dynamic> parameters = new List<dynamic>();
            var par = e.Argument as List<dynamic>;
            var query = par[0] as string;
            var queryType = par[1] as string;
            var del = par[2] as Action<JArray>;
          
            try
            {
                 Thread.Sleep(1500);
                conn.Open();
                comm.Connection = conn;
                comm.CommandType = CommandType.Text;
                comm.CommandText = query;
                NpgsqlDataReader dataReader = comm.ExecuteReader();
                if (queryType == "select")
                {
                    var dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    string JSONString = string.Empty;
                    JSONString = JsonConvert.SerializeObject(dataTable);
                    JArray jsonData = JArray.Parse(JSONString);
                    parameters.Add(jsonData);
                    parameters.Add(del);
                    e.Result = parameters;
                    
                }
                else
                {
                    JObject obj = new JObject();
                    obj.Add("isSuccess", "true");
                    JArray jsonData = new JArray();
                    jsonData.Add(obj);
                    parameters.Add(jsonData);
                    parameters.Add(del);
                    e.Result = parameters;
                }
            }
            catch
            {
                JObject obj = new JObject();
                JArray jsonData = new JArray();
                jsonData.Add(obj);
                parameters.Add(jsonData);
                parameters.Add(del);
                e.Result = parameters;
            }
            finally
            {
                conn.Close();
                //Mouse.OverrideCursor = null;
            }
        }
    }
}
