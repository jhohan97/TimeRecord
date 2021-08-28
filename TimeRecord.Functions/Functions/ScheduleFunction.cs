using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;
using TimeRecord.Functions.Entities;

namespace TimeRecord.Functions.Functions
{
    public static class ScheduleFunction
    {
        
        [FunctionName("ScheduleFunction")]

        public static async Task Run(
            [TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,
            [Table("Employee", Connection = "AzureWebJobsStorage")] CloudTable EmployeeTable,
            ILogger log)
        {
            log.LogInformation($"Deleting completed function executed at: {DateTime.Now}");
            string filter1 = TableQuery.GenerateFilterConditionForBool("Consolidated", QueryComparisons.Equal, false);
            string filter2 = TableQuery.GenerateFilterConditionForInt("type", QueryComparisons.Equal, 0);
            string filter3 = TableQuery.GenerateFilterConditionForInt("type", QueryComparisons.Equal, 1);

            TableQuery<EmployedEntity> query1 = new TableQuery<EmployedEntity>().Where(filter2 + filter1);
            TableQuerySegment<EmployedEntity> unConsolidateEntrysDateEntry = await EmployeeTable.ExecuteQuerySegmentedAsync(query1, null);

            TableQuery<EmployedEntity> query2 = new TableQuery<EmployedEntity>().Where(filter3 + filter1);
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
                            TimeWorked = Convert.ToInt32((item.DateAndTime - item2.DateAndTime).TotalHours),
                            Date = item2.DateAndTime,
                            RowKey = Guid.NewGuid().ToString(),
                            PartitionKey = "Consolidate",
                            ETag = "*"
                        };

                        TableOperation addOperation = TableOperation.Insert(consolidateEntity);
                        await EmployeeTable.ExecuteAsync(addOperation);
                    }
                }
                
            }
        }
    }
}
