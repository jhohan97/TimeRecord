using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using TimeRecord.Common.Models;
using TimeRecord.Functions.Functions;
using TimeRecord.Tests.Helpers;
using Xunit;

namespace TimeRecord.Tests.Test
{
    public class EntryApiTest
    {
        private readonly ILogger logger = TesFactory.CreateLogger();

        [Fact]
        public async void CreateEntry_Should_Return_200()
        {
            //Arrange
            MockCloudTableEmployees mockEmployees = new MockCloudTableEmployees(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Employee employeeRequest = TesFactory.GetEmployee();
            DefaultHttpRequest request = TesFactory.CreateHttpRequest(employeeRequest);

            // Act
            IActionResult response = await EmployeeApi.CreateEntry(request, mockEmployees, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

        }

        [Fact]
        public async void UpdateEntry_Should_Return_200()
        {
            //Arrange
            MockCloudTableEmployees mockEmployees = new MockCloudTableEmployees(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Employee employeeRequest = TesFactory.GetEmployee();
            DefaultHttpRequest request = TesFactory.CreateHttpRequest(employeeRequest);

            // Act
            IActionResult response = await EmployeeApi.UpdateEntry(request, mockEmployees,employeeRequest.IdEmployee.ToString(),logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

        }

    }
}
