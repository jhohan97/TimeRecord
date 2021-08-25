using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeRecord.Functions.Entities
{
    public class EmployedEntity : TableEntity
    {
        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }
        public int IdEmployee { get; set; }
        public int Type { get; set; }
        public int Consolidated { get; set; }
    }
}
