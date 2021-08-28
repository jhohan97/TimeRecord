using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace TimeRecord.Functions.Entities
{
    public class ConsolidatedEntity : TableEntity
    {
        public int IdEmployee { get; set; }
        public int TimeWorked { get; set; }
        public DateTime Date { get; set; }
    }
}
