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

namespace demo.staticapp.fn
{
    public static class GetApiFn
    {
        [FunctionName("GetApiFn")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get function started");

            List<appobject> _lst = SeletSql(log);

            log.LogInformation("Get function completed");
            return new OkObjectResult(_lst);
        }
        private static List<appobject> SeletSql(ILogger log)
        {
            SqlConnection con = new SqlConnection("Server=tcp:staticwebserver.database.windows.net,1433;Initial Catalog=staticwebdb;Persist Security Info=False;User ID=staticwebserver;Password=Pass@word1Microsoft;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            SqlCommand cmd;
            try
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("select * from [dbo].[tblwebapp]", con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                con.Close();
                List<appobject> lstapp = new List<appobject>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    lstapp.Add(new appobject { id = (int)dr["id"], name = (string)dr["name"], details = (string)dr["details"] });
                }
                return lstapp;
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
