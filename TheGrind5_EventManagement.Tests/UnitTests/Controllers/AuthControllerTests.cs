using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using TheGrind5_EventManagement.Controllers;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.Tests.Fixtures;
using TheGrind5_EventManagement.Tests.Helpers;
using System.Security.Claims;

namespace TheGrind5_EventManagement.Tests.UnitTests.Controllers;

/// <summary>
/// Unit tests cho AuthController
/// Covers: Login, Register, Profile management, Wallet operations
/// Target: 20 test cases
/// </summary>
public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_mockAuthService.Object, _mockUserRepository.Object, _mockLogger.Object);
    }

    #region Login Tests

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var request = new AuthDTOs.LoginRequest("test@example.com", "password123");
        var user = TestDataBuilder.CreateUser(email: "test@example.com");
        var loginResult = new AuthDTOs.LoginResponse("token123", DateTime.UtcNow.AddHours(1), 
            new AuthDTOs.UserReadDto(user.UserId, user.FullName, user.Email, user.Phone!, user.Role, user.WalletBalance, user.Avatar));

        _mockAuthService.Setup(x => x.LoginAsync("test@example.com", "password123"))
            .ReturnsAsync(loginResult);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockAuthService.Verify(x => x.LoginAsync("test@example.com", "password123"), Times.Once);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = new AuthDTOs.LoginRequest("test@example.com", "wrongpassword");
        
        _mockAuthService.Setup(x => x.LoginAsync("test@example.com", "wrongpassword"))
            .ReturnsAsync((AuthDTOs.LoginResponse?)null);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_EmptyEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new AuthDTOs.LoginRequest("", "password123");

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_BannedUser_ReturnsForbidden()
    {
        // Arrange
        var request = new AuthDTOs.LoginRequest("banned@example.com", "password123");
        
        _mockAuthService.Setup(x => x.LoginAsync("banned@example.com", "password123"))
            .ThrowsAsync(new UnauthorizedAccessException("User is banned"));

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult?.StatusCode.Should().Be(403);
    }

    #endregion

    #region Register Tests

    [Fact]
    public async Task Register_ValidData_ReturnsOk()
    {
        // Arrange
        var request = new RegisterRequest(
            "testuser",
            "test@example.com",
            "password123",
            "Test User"
        );

        var user = TestDataBuilder.CreateUser(email: "test@example.com", username: "testuser");
        var registerResult = new AuthDTOs.UserReadDto(user.UserId, user.FullName, user.Email, user.Phone!, user.Role, user.WalletBalance, user.Avatar);

        _mockUserRepository.Setup(x => x.IsEmailExistsAsync("test@example.com"))
            .ReturnsAsync(false);
        _mockAuthService.Setup(x => x.RegisterAsync(request))
            .ReturnsAsync(registerResult);

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockAuthService.Verify(x => x.RegisterAsync(request), Times.Once);
    }

    [Fact]
    public async Task Register_ExistingEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest(
            "testuser",
            "existing@example.com",
            "password123",
            "Test User"
        );

        _mockUserRepository.Setup(x => x.IsEmailExistsAsync("existing@example.com"))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_WeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest(
            "testuser",
            "test@example.com",
            "123", // Too short
            "Test User"
        );

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_InvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest(
            "testuser",
            "invalidemail", // Missing @
            "password123",
            "Test User"
        );

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region GetCurrentUser Tests

    [Fact]
    public async Task GetCurrentUser_ValidToken_ReturnsOk()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser();
        var claimsPrincipal = TestHelper.CreateClaimsPrincipalFromUser(user);
        _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
        {
            User = claimsPrincipal
        };

        _mockUserRepository.Setup(x => x.GetUserByIdAsync(user.UserId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetCurrentUser();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetCurrentUser_NoToken_ReturnsUnauthorized()
    {
        // Arrange
        _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
        {
            User = new ClaimsPrincipal()
        };

        // Act
        var result = await _controller.GetCurrentUser();

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetCurrentUser_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = 999;
        var claimsPrincipal = TestHelper.CreateClaimsPrincipal(userId);
        _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
        {
            User = claimsPrincipal
        };

        _mockUserRepository.Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _controller.GetCurrentUser();

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetProfile Tests

    [Fact]
    public async Task GetCurrentUserProfile_ValidToken_ReturnsOk()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser();
        var claimsPrincipal = TestHelper.CreateClaimsPrincipalFromUser(user);
        _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
        {
            User = claimsPrincipal
        };

        _mockUserRepository.Setup(x => x.GetUserByIdAsync(user.UserId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetCurrentUserProfile();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region UpdateProfile Tests

    [Fact]
    public async Task UpdateProfile_ValidData_ReturnsOk()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser();
        var claimsPrincipal = TestHelper.CreateClaimsPrincipalFromUser(user);
        _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
        {
            User = claimsPrincipal
        };

        var request = new ProfileDTOs.UpdateProfileRequest
        {
            fullName = "Updated Name",
            phone = "0123456789",
            gender = "Male"
        };

        _mockUserRepository.Setup(x => x.GetUserByIdAsync(user.UserId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.UpdateProfile(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockUserRepository.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Once);
    }

    #endregion

    #region GetWalletBalance Tests

    [Fact]
    public async Task GetWalletBalance_ValidUser_ReturnsBalance()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser(walletBalance: 1000m);
        var claimsPrincipal = TestHelper.CreateClaimsPrincipalFromUser(user);
        _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
        {
            User = claimsPrincipal
        };

        _mockUserRepository.Setup(x => x.GetUserByIdAsync(user.UserId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetWalletBalance();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    #endregion
}

