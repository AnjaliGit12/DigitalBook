using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Data;

namespace BuyBook
{
    public static class BuyBook
    {
        [FunctionName("BuyBook")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,"post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<BuyModel>(requestBody);
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    if (!String.IsNullOrEmpty(input.emailid) && !String.IsNullOrEmpty(input.bookid))
                    {
                        var query = $"INSERT INTO [Purchase](EmailId,BookId,PurchaseDate,PaymentMode,IsRefunded) VALUES('{input.emailid}','{input.bookid}'," +
                            $"'{DateTime.Now.ToString("MM/dd/yyyy")}','Cash',  'Y')";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
                return new BadRequestResult();
            }
            return new OkResult();
        }
        [FunctionName("GetPurchases")]
        public static async Task<IActionResult> GetPurchases(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetPurchases")] HttpRequest req, ILogger log)
        {
            List<BuyModel> taskList = new List<BuyModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    var query = @"Select * from Purchase";
                    SqlCommand command = new SqlCommand(query, connection);
                    var reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        BuyModel task = new BuyModel()
                        {
                            bookid = reader["BookId"].ToString(),
                            emailid = reader["EmailId"].ToString(),
                            purchasedate = (DateTime)reader["PurchaseDate"],
                            IsRefund = reader["IsRefunded"].ToString(),
                            paymentmode = reader["PaymentMode"].ToString()
                        };
                        taskList.Add(task);
                    }
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            if (taskList.Count > 0)
            {
                return new OkObjectResult(taskList);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [FunctionName("GetPurchaseById")]
        public static IActionResult GetPurchaseById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetPurchaseById/{id}")] HttpRequest req, ILogger log, int id)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    var query = @"Select * from Purchase Where Id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", id);
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            if (dt.Rows.Count == 0)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(dt);
        }
    }
}
