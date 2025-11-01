using Xunit;
using FluentAssertions;
using Moq;
using TheGrind5_EventManagement.Services;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.Mappers;
using TheGrind5_EventManagement.Tests.Fixtures;
using TheGrind5_EventManagement.Tests.Helpers;

namespace TheGrind5_EventManagement.Tests.UnitTests.Services;

/// <summary>
/// Unit tests cho AuthService
/// Covers: Login, Register, Banned user handling
/// Target: 15-18 test cases
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<IPasswordService> _mockPasswordService;
    private readonly Mock<IUserMapper> _mockUserMapper;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockJwtService = new Mock<IJwtService>();
        _mockPasswordService = new Mock<IPasswordService>();
        _mockUserMapper = new Mock<IUserMapper>();
        _authService = new AuthService(
            _mockUserRepository.Object,
            _mockJwtService.Object,
            _mockPasswordService.Object,
            _mockUserMapper.Object
        );
    }

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsLoginResponse()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var user = TestDataBuilder.CreateUser(email: email);
        var userDto = new AuthDTOs.UserReadDto(
            user.UserId, user.FullName, user.Email, user.Phone!, user.Role, user.WalletBalance, user.Avatar);
        var expectedToken = "generated_token_123";

        _mockUserRepository.Setup(x => x.GetUserByEmailAsync(email))
            .ReturnsAsync(user);
        _mockPasswordService.Setup(x => x.VerifyPassword(password, user.PasswordHash))
            .Returns(true);
        _mockJwtService.Setup(x => x.GenerateToken(user))
            .Returns(expectedToken);
        _mockUserMapper.Setup(x => x.MapToUserReadDto(user))
            .Returns(userDto);

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        result.Should().NotBeNull();
        result!.AccessToken.Should().Be(expectedToken);
        result.User.Should().BeEquivalentTo(userDto);
        _mockUserRepository.Verify(x => x.GetUserByEmailAsync(email), Times.Once);
        _mockPasswordService.Verify(x => x.VerifyPassword(password, user.PasswordHash), Times.Once);
        _mockJwtService.Verify(x => x.GenerateToken(user), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_InvalidEmail_ReturnsNull()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "password123";

        _mockUserRepository.Setup(x => x.GetUserByEmailAsync(email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        result.Should().BeNull();
        _mockPasswordService.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockJwtService.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsNull()
    {
        // Arrange
        var email = "test@example.com";
        var password = "wrongpassword";
        var user = TestDataBuilder.CreateUser(email: email);

        _mockUserRepository.Setup(x => x.GetUserByEmailAsync(email))
            .ReturnsAsync(user);
        _mockPasswordService.Setup(x => x.VerifyPassword(password, user.PasswordHash))
            .Returns(false);

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        result.Should().BeNull();
        _mockPasswordService.Verify(x => x.VerifyPassword(password, user.PasswordHash), Times.Once);
        _mockJwtService.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_BannedUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var email = "banned@example.com";
        var password = "password123";
        var bannedUser = TestDataBuilder.CreateUser(email: email, isBanned: true);

        _mockUserRepository.Setup(x => x.GetUserByEmailAsync(email))
            .ReturnsAsync(bannedUser);
        _mockPasswordService.Setup(x => x.VerifyPassword(password, bannedUser.PasswordHash))
            .Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(email, password));
        _mockJwtService.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    #endregion

    #region RegisterAsync Tests

    [Fact]
    public async Task RegisterAsync_ValidRequest_ReturnsUserReadDto()
    {
        // Arrange
        var request = new RegisterRequest(
            "testuser",
            "test@example.com",
            "password123",
            "Test User",
            "0123456789"
        );
        var passwordHash = "hashed_password";
        var user = TestDataBuilder.CreateUser(
            email: request.Email,
            username: request.Username,
            fullName: request.FullName);
        var userDto = new AuthDTOs.UserReadDto(
            user.UserId, user.FullName, user.Email, user.Phone!, user.Role, user.WalletBalance);

        _mockPasswordService.Setup(x => x.HashPassword(request.Password))
            .Returns(passwordHash);
        _mockUserMapper.Setup(x => x.MapFromRegisterRequest(request, passwordHash))
            .Returns(user);
        _mockUserRepository.Setup(x => x.CreateUserAsync(user))
            .ReturnsAsync(user);
        _mockUserMapper.Setup(x => x.MapToUserReadDto(user))
            .Returns(userDto);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(userDto);
        _mockPasswordService.Verify(x => x.HashPassword(request.Password), Times.Once);
        _mockUserRepository.Verify(x => x.CreateUserAsync(It.IsAny<User>()), Times.Once);
        _mockUserMapper.Verify(x => x.MapToUserReadDto(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var request = new RegisterRequest(
            "testuser",
            "test@example.com",
            "password123",
            "Test User"
        );
        var passwordHash = "hashed_password";
        var user = TestDataBuilder.CreateUser();

        _mockPasswordService.Setup(x => x.HashPassword(request.Password))
            .Returns(passwordHash);
        _mockUserMapper.Setup(x => x.MapFromRegisterRequest(request, passwordHash))
            .Returns(user);
        _mockUserRepository.Setup(x => x.CreateUserAsync(user))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(request));
    }

    [Fact]
    public async Task RegisterAsync_MapperThrowsException_ThrowsException()
    {
        // Arrange
        var request = new RegisterRequest(
            "testuser",
            "test@example.com",
            "password123",
            "Test User"
        );
        var passwordHash = "hashed_password";

        _mockPasswordService.Setup(x => x.HashPassword(request.Password))
            .Returns(passwordHash);
        _mockUserMapper.Setup(x => x.MapFromRegisterRequest(request, passwordHash))
            .Throws(new Exception("Mapper error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(request));
    }

    #endregion
}

