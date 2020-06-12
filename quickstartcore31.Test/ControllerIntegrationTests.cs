using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace quickstartcore31.Test
{
    public class ControllerIntegrationTests
    {

        private readonly WebApplicationFactory<quickstartcore31.Startup> _factory;

        public ControllerIntegrationTests()
        {
            _factory = new WebApplicationFactory<quickstartcore31.Startup>();
        }

        [Fact]
        public async void TestComsodb()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var responseString = await response.Content.ReadAsStringAsync();            
            // Assert
            Assert.Equal(2426, responseString.Length);
            // Assert.AreEqual(0, context.ChangeTracker.Entries().Count());
        }
    }
}
