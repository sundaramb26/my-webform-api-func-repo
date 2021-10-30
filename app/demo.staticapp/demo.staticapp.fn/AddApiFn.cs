using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;

namespace demo.staticapp.fn
{
    public static class AddApiFn
    {
        [FunctionName("AddApiFn")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Insert function started");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            appobject _app = Newtonsoft.Json.JsonConvert.DeserializeObject<appobject>(requestBody);
            
            _app.id = new Random().Next(0, 1000);
            InsertSql(_app, log);
            log.LogInformation("Insert function completed");           
            return new OkObjectResult("saved");           
        }
        private static void InsertSql(appobject _app, ILogger log)
        {
            SqlConnection con = new SqlConnection("Server=tcp:staticwebserver.database.windows.net,1433;Initial Catalog=staticwebdb;Persist Security Info=False;User ID=staticwebserver;Password=Pass@word1Microsoft;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            SqlCommand cmd;
            try
            {
                con.Open();
                string s = "insert into [dbo].[tblwebapp] values(@p1,@p2,@p3)";
                cmd = new SqlCommand(s, con);
                cmd.Parameters.AddWithValue("@p1", _app.id);
                cmd.Parameters.AddWithValue("@p2", _app.name);
                cmd.Parameters.AddWithValue("@p3", _app.details);
                cmd.CommandType = CommandType.Text;
                int i = cmd.ExecuteNonQuery();
                con.Close();

            }
            catch (Exception ex)
            {
                log.LogInformation($"Error {ex.Message}");
                throw ex;
            }
        }
        public class appobject
        {
            public int id { get; set; }
            public string name { get; set; }
            public string details { get; set; }
        }
    }
}
