using Microsoft.AspNetCore.Mvc;
using Moq;
using OJTEDU.Api.Configuration;
using OJTEDU.Api.Controllers.ComonControllers;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Api.Input.CommonControllers.AuthenticationController;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.UnitTests.Controllers.CommonControllers
{
    [TestFixture]
    public class AuthenticationControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private AuthenticationController _controller;

        [SetUp]
        public void SetUp()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new AuthenticationController(_userServiceMock.Object);
        }

        [Test]
        public async Task LoginWithGoogle_ShouldReturnBadRequest_WhenAuthorizeCodeIsNullOrEmpty()
        {
            // Arrange
            var request = new LoginRequest { AuthorizeCode = null };

            // Act
            var result = await _controller.LoginWithGoogle(request);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            var apiResponse = (ApiResponse<object>)badRequestResult.Value;
            Assert.AreEqual("AuthorizeCode is required.", apiResponse.Message);
        }

        [Test]
        public async Task LoginWithGoogle_ShouldReturnOk_WhenServiceReturnsValidData()
        {
            // Arrange
            var request = new LoginRequest { AuthorizeCode = "valid-token" };
            var serviceResponse = new DataResponse<UserReadForAuthDTO>
            {
                Data = new UserReadForAuthDTO { UserId = 1, Name = "Test User", Email = "test@example.com" },
                Message = "Login successful!",
                StatusCode = 200
            };

            _userServiceMock.Setup(s => s.LoginWithGoogleAsync(It.IsAny<string>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.LoginWithGoogle(request);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            var apiResponse = (ApiResponse<UserReadForAuthDTO>)okResult.Value;
            Assert.AreEqual("Login successful!", apiResponse.Message);
            Assert.IsNotNull(apiResponse.Data);
            Assert.AreEqual(1, apiResponse.Data.UserId);
        }

        [Test]
        public async Task LoginWithGoogle_ShouldReturnUnauthorized_WhenGoogleTokenIsInvalid()
        {
            // Arrange
            var request = new LoginRequest { AuthorizeCode = "invalid-token" };
            var serviceResponse = new DataResponse<UserReadForAuthDTO>
            {
                Data = null,
                Message = "Invalid Google Token.",
                StatusCode = 401
            };

            _userServiceMock.Setup(s => s.LoginWithGoogleAsync(It.IsAny<string>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.LoginWithGoogle(request);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.AreEqual(401, objectResult.StatusCode);
            var apiResponse = (ApiResponse<object>)objectResult.Value;
            Assert.AreEqual("Invalid Google Token.", apiResponse.Message);
        }

        [Test]
        public async Task LoginWithGoogle_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new LoginRequest { AuthorizeCode = "valid-token" };
            var serviceResponse = new DataResponse<UserReadForAuthDTO>
            {
                Data = null,
                Message = "User not found.",
                StatusCode = 404
            };

            _userServiceMock.Setup(s => s.LoginWithGoogleAsync(It.IsAny<string>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.LoginWithGoogle(request);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.AreEqual(404, objectResult.StatusCode);
            var apiResponse = (ApiResponse<object>)objectResult.Value;
            Assert.AreEqual("User not found.", apiResponse.Message);
        }

        [Test]
        public async Task LoginWithGoogle_ShouldReturnConflict_WhenUserAccountIsNotActivated()
        {
            // Arrange
            var request = new LoginRequest { AuthorizeCode = "valid-token" };
            var serviceResponse = new DataResponse<UserReadForAuthDTO>
            {
                Data = null,
                Message = "User account is not activated.",
                StatusCode = 409
            };

            _userServiceMock.Setup(s => s.LoginWithGoogleAsync(It.IsAny<string>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.LoginWithGoogle(request);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.AreEqual(409, objectResult.StatusCode);
            var apiResponse = (ApiResponse<object>)objectResult.Value;
            Assert.AreEqual("User account is not activated.", apiResponse.Message);
        }

        [Test]
        public async Task LoginWithGoogle_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var request = new LoginRequest { AuthorizeCode = "valid-token" };

            _userServiceMock.Setup(s => s.LoginWithGoogleAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _controller.LoginWithGoogle(request);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.AreEqual(500, objectResult.StatusCode);
            var apiResponse = (ApiResponse<object>)objectResult.Value;
            Assert.AreEqual("Internal Server Error: Some error", apiResponse.Message);
        }
    }
}
