using System;
using TimeRecord.Functions.Functions;
using TimeRecord.Tests.Helpers;
using Xunit;

namespace TimeRecord.Tests.Test
{
    public class ScheduleFunctionTest
    {

        [Fact]
        public void ScheuleFunction_Should_Log_Message()
        {
            //Arrange
            MockCloudTableEmployees mockEmployees = new MockCloudTableEmployees(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            MockCloudTableConsolidated mockConsolidated = new MockCloudTableConsolidated(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TesFactory.CreateLogger(LoggerTypes.List);

            // Act
            ScheduleFunction.Run(null, mockEmployees, mockConsolidated, logger);
            string message = logger.logs[0];
            //Assert
            Assert.Contains("Consolidated completed", message);

        }
    }
}
