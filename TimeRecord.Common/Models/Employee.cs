using System;

namespace TimeRecord.Common.Models
{
    public class Employee
    {
        public DateTime DateAndTime { get; set; }
        public int IdEmployee { get; set; }
        public int Type { get; set; }
        public bool Consolidated { get; set; }
    }
}
