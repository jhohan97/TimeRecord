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

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Employee employee = JsonConvert.DeserializeObject<Employee>(requestBody);

            if (string.IsNullOrEmpty(employee?.IdEmployee.ToString()))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have a IdEmployee, DateAndTime and Type "
                });
            }

            EmployedEntity employedEntity = new EmployedEntity
            {
                IdEmployee = employee.IdEmployee,
                DateAndTime = employee.DateAndTime,
                Type = employee.Type,
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


        [FunctionName(nameof(UpdateEntry))]
        public static async Task<IActionResult> UpdateEntry(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "employee/{id}")] HttpRequest req,
            [Table("Employee", Connection = "AzureWebJobsStorage")] CloudTable EmployeeTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Update Employee: {id} Recieved.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Employee employee = JsonConvert.DeserializeObject<Employee>(requestBody);

            if (employee == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have a Type and  DateAndTime"
                });
            }

            //Validate that the Id Employee exist 

            TableOperation findOperation = TableOperation.Retrieve<EmployedEntity>("EmployeeRegistry", id);
            TableResult findResult = await EmployeeTable.ExecuteAsync(findOperation);
            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Employee not found."
                });

            }


            //Update time of employee
            EmployedEntity employedEntity = (EmployedEntity)findResult.Result;
            if (!string.IsNullOrEmpty(employee.Type.ToString()))
            {
                employedEntity.Type = employee.Type;
                employedEntity.DateAndTime = employee.DateAndTime;   
            }

            TableOperation addOperation = TableOperation.Replace(employedEntity);
            await EmployeeTable.ExecuteAsync(addOperation);

            string message = $"Employee : {id} update in table";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = employedEntity
            });
        }

        [FunctionName(nameof(GetAllEntrys))]
        public static async Task<IActionResult> GetAllEntrys(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "employee")] HttpRequest req,
            [Table("Employee", Connection = "AzureWebJobsStorage")] CloudTable EmployeeTable,
            ILogger log)
        {
            log.LogInformation("Get all Entrys received.");

            TableQuery<EmployedEntity> query = new TableQuery<EmployedEntity>();
            TableQuerySegment<EmployedEntity> entrys = await EmployeeTable.ExecuteQuerySegmentedAsync(query, null);


            string message = "Retrived all entrys";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = entrys
            });
        }
    }
}
