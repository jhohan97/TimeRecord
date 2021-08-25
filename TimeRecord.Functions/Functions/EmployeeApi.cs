using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using TimeRecord.Common.Models;
using TimeRecord.Common.Responces;
using TimeRecord.Functions.Entities;

namespace TimeRecord.Functions.Functions
{
    public static class EmployeeApi
    {
        [FunctionName(nameof(CreateEntry))]
        public static async Task<IActionResult> CreateEntry(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "employee")] HttpRequest req,
            [Table("Employee", Connection = "AzureWebJobsStorage")] CloudTable EmployeeTable,
            ILogger log)
        {
            log.LogInformation("Recieved a new employee.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Employee employee = JsonConvert.DeserializeObject<Employee>(requestBody);

            if (string.IsNullOrEmpty(employee?.IdEmployee.ToString()))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have a IdEmployee "
                });
            }

            EmployedEntity employedEntity = new EmployedEntity
            {
                IdEmployee = employee.IdEmployee,
                EntryTime = DateTime.UtcNow,
                ExitTime = DateTime.UtcNow,
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = "EmployeeRegistry",
                ETag = "*",
                Consolidated = false,
            };

            TableOperation addOperation = TableOperation.Insert(employedEntity);
            await EmployeeTable.ExecuteAsync(addOperation);

            string message = "New employed Registry stored in BD";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = employedEntity
            });
        }
    }
}
