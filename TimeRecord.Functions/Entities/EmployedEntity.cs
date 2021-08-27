using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace TimeRecord.Functions.Entities
{
    public class EmployedEntity : TableEntity
    {
        public DateTime DateAndTime { get; set; }
        public int IdEmployee { get; set; }
        public int Type { get; set; }
        public bool Consolidated { get; set; }
    }
}
