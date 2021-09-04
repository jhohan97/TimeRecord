using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.IO;
using System.Threading.Tasks;
using TimeRecord.Common.Responces;
using TimeRecord.Functions.Entities;

namespace TimeRecord.Functions.Functions
{
    public static class ScheduleFunction
    {

        [FunctionName("ScheduleFunction")]

        public static async Task Run(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            [Table("Employee", Connection = "AzureWebJobsStorage")] CloudTable EmployeeTable,
            [Table("Consolidate", Connection = "AzureWebJobsStorage")] CloudTable ConsolidateTable,
            ILogger log)
        {
            log.LogInformation($"Consolidated completed function executed at: {DateTime.Now}");
            string filter1 = TableQuery.GenerateFilterConditionForBool("Consolidated", QueryComparisons.Equal, false);
            string filter2 = TableQuery.GenerateFilterConditionForInt("Type", QueryComparisons.Equal, 0);
            string filter3 = TableQuery.GenerateFilterConditionForInt("Type", QueryComparisons.Equal, 1);

            TableQuery<EmployedEntity> query1 = new TableQuery<EmployedEntity>().Where(TableQuery.CombineFilters(filter2, TableOperators.And, filter1));
            TableQuerySegment<EmployedEntity> unConsolidateEntrysDateEntry = await EmployeeTable.ExecuteQuerySegmentedAsync(query1, null);

            TableQuery<EmployedEntity> query2 = new TableQuery<EmployedEntity>().Where(TableQuery.CombineFilters(filter3, TableOperators.And, filter1));
            TableQuerySegment<EmployedEntity> unConsolidateDateExit = await EmployeeTable.ExecuteQuerySegmentedAsync(query2, null);

            foreach (EmployedEntity item in unConsolidateEntrysDateEntry)
            {
                foreach (EmployedEntity item2 in unConsolidateDateExit)
                {
                    if (item.IdEmployee.Equals(item2.IdEmployee))
                    {
                        ConsolidatedEntity consolidateEntity = new ConsolidatedEntity
                        {
                            IdEmployee = item.IdEmployee,
                            TimeWorked = Convert.ToInt32((item2.DateAndTime - item.DateAndTime).TotalMinutes),
                            Date = item2.DateAndTime,
                            RowKey = Guid.NewGuid().ToString(),
                            PartitionKey = "Consolidate",
                            ETag = "*"
                        };
                        await Insert(consolidateEntity, ConsolidateTable);
                        await UpdateEntry(EmployeeTable, item.RowKey);
                        await UpdateEntry(EmployeeTable, item2.RowKey);
                    }
                }

            }
        }

        public static async Task Insert(
            ConsolidatedEntity consolidatedEntity,
            [Table("Consolidate", Connection = "AzureWebJobsStorage")] CloudTable EmployeeTable)
        {

            TableOperation addOperation = TableOperation.Insert(consolidatedEntity);
            await EmployeeTable.ExecuteAsync(addOperation);
        }

        public static async Task UpdateEntry(
            [Table("Employee", Connection = "AzureWebJobsStorage")] CloudTable EmployeeTable,
            string id)
        {
            TableOperation findOperation = TableOperation.Retrieve<EmployedEntity>("EmployeeRegistry", id);
            TableResult findResult = await EmployeeTable.ExecuteAsync(findOperation);

            //Update time of employee
            EmployedEntity employedEntity = (EmployedEntity)findResult.Result;
            employedEntity.Consolidated = true;

            TableOperation addOperation = TableOperation.Replace(employedEntity);
            await EmployeeTable.ExecuteAsync(addOperation);
        }
    }
}
