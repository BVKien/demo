using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using OJTEDU.Api;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.DTOs;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OJTEDU.Tests.IntegrationTests
{
    public class StudentIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public StudentIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetStudentDetail_ValidUserId_ReturnsStudentDetails()
        {
            // Arrange
            int validUserId = 1; // Assuming we have a test user with ID 1

            // Act
            var response = await _client.GetAsync($"/api/student/student-detail/{validUserId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<StudentDTO.StudentDetailForStudentDTO>>(responseData);

            Xunit.Assert.NotNull(apiResponse);
            Xunit.Assert.Equal("Student information retrieved successfully!", apiResponse.Message);
            Xunit.Assert.NotNull(apiResponse.Data);
            Xunit.Assert.Equal(validUserId, apiResponse.Data.StudentId);
        }

        [Fact]
        public async Task GetStudentDetail_InvalidUserId_ReturnsNotFound()
        {
            // Arrange
            int invalidUserId = -1;

            // Act
            var response = await _client.GetAsync($"/api/student/student-detail/{invalidUserId}");

            // Assert
            Xunit.Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            var responseData = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(responseData);

            Xunit.Assert.NotNull(apiResponse);
            Xunit.Assert.Contains("Not found user with id", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateStudent_ValidUserId_ReturnsUpdatedStudentDetails()
        {
            // Arrange
            int validUserId = 1; // Replace with a valid test user ID
            var updateInput = new
            {
                Image = "updatedImage.jpg",
                AlternativeEmail = "newemail@example.com",
                Phone = "1234567890",
                Dob = "2000-01-01",
                Gender = true,
                Detail = "Updated Address Detail",
                WardId = 1,
                DistrictId = 1,
                ProvinceId = 1
            };
            var content = new StringContent(JsonConvert.SerializeObject(updateInput), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/api/student/update/{validUserId}", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<StudentDTO.StudentDetailForStudentDTO>>(responseData);

            Xunit.Assert.NotNull(apiResponse);
            Xunit.Assert.Equal("Student information updated and retrieved successfully!", apiResponse.Message);
            Xunit.Assert.NotNull(apiResponse.Data);
            Xunit.Assert.Equal(updateInput.AlternativeEmail, apiResponse.Data.AlternativeEmail);
            Xunit.Assert.Equal(updateInput.Phone, apiResponse.Data.Phone);
        }

        [Fact]
        public async Task UpdateStudent_InvalidUserId_ReturnsNotFound()
        {
            // Arrange
            int invalidUserId = -1;
            var updateInput = new
            {
                Image = "updatedImage.jpg",
                AlternativeEmail = "newemail@example.com",
                Phone = "1234567890",
                Dob = "2000-01-01",
                Gender = true,
                Detail = "Updated Address Detail",
                WardId = 1,
                DistrictId = 1,
                ProvinceId = 1
            };
            var content = new StringContent(JsonConvert.SerializeObject(updateInput), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/api/student/update/{invalidUserId}", content);

            // Assert
            Xunit.Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            var responseData = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(responseData);

            Xunit.Assert.NotNull(apiResponse);
            Xunit.Assert.Contains("Not found user with id", apiResponse.Message);
        }
    }
}
