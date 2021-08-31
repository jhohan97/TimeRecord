using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TimeRecord.Common.Models;
using TimeRecord.Functions.Entities;

namespace TimeRecord.Tests.Helpers
{
    public class TesFactory
    {
        public static EmployedEntity GetEmployedEntity()
        {
            return new EmployedEntity
            {
                ETag = "*",
                PartitionKey = "EmployeeRegistry",
                RowKey = Guid.NewGuid().ToString(),
                DateAndTime = DateTime.UtcNow,
                Consolidated = false,
                IdEmployee = 1,
                Type = 0
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid EmployeeId, Employee employeeRequest)
        {
            string request = JsonConvert.SerializeObject(employeeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"{EmployeeId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid EmployeeId)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"{EmployeeId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Employee employeeRequest)
        {
            string request = JsonConvert.SerializeObject(employeeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static Employee GetEmployee()
        {
            return new Employee
            {
                Consolidated = false,
                DateAndTime = DateTime.UtcNow,
                IdEmployee = 2,
                Type = 0
            };
        }

        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null) 
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }
            return logger;
        }
    }
}
