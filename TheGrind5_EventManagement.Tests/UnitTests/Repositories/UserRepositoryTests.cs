using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Tests.Fixtures;

namespace TheGrind5_EventManagement.Tests.UnitTests.Repositories;

/// <summary>
/// Unit tests cho UserRepository với InMemory database
/// Covers: CRUD operations, Email lookup, Ban management
/// Target: 12 test cases
/// </summary>
public class UserRepositoryTests : IDisposable
{
    private readonly EventDBContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<EventDBContext>()
            .UseInMemoryDatabase(databaseName: $"TestDB_{Guid.NewGuid()}")
            .Options;

        _context = new EventDBContext(options);
        _repository = new UserRepository(_context);
    }

    #region CreateUserAsync Tests

    [Fact]
    public async Task CreateUserAsync_ValidUser_ReturnsCreatedUser()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser();

        // Act
        var result = await _repository.CreateUserAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().BeGreaterThan(0);
        result.Username.Should().Be(user.Username);
        result.Email.Should().Be(user.Email);
        
        var userInDb = await _context.Users.FindAsync(result.UserId);
        userInDb.Should().NotBeNull();
    }

    [Fact(Skip = "InMemory database doesn't enforce unique constraints")]
    public async Task CreateUserAsync_DuplicateEmail_ThrowsException()
    {
        // Note: InMemory database doesn't enforce unique constraints.
        // This test would need a real SQL Server database to validate.
        // Arrange
        var user1 = TestDataBuilder.CreateUser(email: "test@example.com");
        var user2 = TestDataBuilder.CreateUser(email: "test@example.com");
        
        await _repository.CreateUserAsync(user1);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.CreateUserAsync(user2));
    }

    #endregion

    #region GetUserByIdAsync Tests

    [Fact]
    public async Task GetUserByIdAsync_ExistingUser_ReturnsUser()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser();
        await _repository.CreateUserAsync(user);

        // Act
        var result = await _repository.GetUserByIdAsync(user.UserId);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(user.UserId);
        result.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetUserByIdAsync_NonExistentUser_ReturnsNull()
    {
        // Arrange
        var userId = 999;

        // Act
        var result = await _repository.GetUserByIdAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetUserByEmailAsync Tests

    [Fact]
    public async Task GetUserByEmailAsync_ExistingEmail_ReturnsUser()
    {
        // Arrange
        var email = "test@example.com";
        var user = TestDataBuilder.CreateUser(email: email);
        await _repository.CreateUserAsync(user);

        // Act
        var result = await _repository.GetUserByEmailAsync(email);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
    }

    [Fact]
    public async Task GetUserByEmailAsync_NonExistentEmail_ReturnsNull()
    {
        // Arrange
        var email = "nonexistent@example.com";

        // Act
        var result = await _repository.GetUserByEmailAsync(email);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region IsEmailExistsAsync Tests

    [Fact]
    public async Task IsEmailExistsAsync_ExistingEmail_ReturnsTrue()
    {
        // Arrange
        var email = "test@example.com";
        var user = TestDataBuilder.CreateUser(email: email);
        await _repository.CreateUserAsync(user);

        // Act
        var result = await _repository.IsEmailExistsAsync(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsEmailExistsAsync_NonExistentEmail_ReturnsFalse()
    {
        // Arrange
        var email = "nonexistent@example.com";

        // Act
        var result = await _repository.IsEmailExistsAsync(email);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region UpdateUserAsync Tests

    [Fact]
    public async Task UpdateUserAsync_ValidUser_ReturnsTrue()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser();
        await _repository.CreateUserAsync(user);
        
        var newFullName = "Updated Name";
        user.FullName = newFullName;

        // Act
        var result = await _repository.UpdateUserAsync(user);

        // Assert
        result.Should().BeTrue();
        
        var userInDb = await _context.Users.FindAsync(user.UserId);
        userInDb!.FullName.Should().Be(newFullName);
    }

    #endregion

    #region GetAllUsersAsync Tests

    [Fact]
    public async Task GetAllUsersAsync_MultipleUsers_ReturnsAllUsers()
    {
        // Arrange
        var user1 = TestDataBuilder.CreateUser();
        var user2 = TestDataBuilder.CreateUser();
        var user3 = TestDataBuilder.CreateUser();
        
        await _repository.CreateUserAsync(user1);
        await _repository.CreateUserAsync(user2);
        await _repository.CreateUserAsync(user3);

        // Act
        var result = await _repository.GetAllUsersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().BeGreaterThanOrEqualTo(3);
        result.Should().Contain(u => u.UserId == user1.UserId);
        result.Should().Contain(u => u.UserId == user2.UserId);
        result.Should().Contain(u => u.UserId == user3.UserId);
    }

    [Fact]
    public async Task GetAllUsersAsync_NoUsers_ReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetAllUsersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [Fact]
    public async Task GetAllUsersAsync_WithRoleFilter_ReturnsFilteredUsers()
    {
        // Arrange
        var host1 = TestDataBuilder.CreateHostUser();
        var host2 = TestDataBuilder.CreateHostUser();
        var customer = TestDataBuilder.CreateUser(role: "Customer");
        
        await _repository.CreateUserAsync(host1);
        await _repository.CreateUserAsync(host2);
        await _repository.CreateUserAsync(customer);

        // Act
        var result = await _repository.GetAllUsersAsync(role: "Host");

        // Assert
        result.Should().NotBeNull();
        result.All(u => u.Role == "Host").Should().BeTrue();
    }

    #endregion

    #region Ban Management Tests

    [Fact]
    public async Task CreateUserAsync_ThenUpdateBanStatus_UpdatesCorrectly()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser();
        await _repository.CreateUserAsync(user);

        // Act - Update ban status
        user.IsBanned = true;
        user.BannedAt = DateTime.UtcNow;
        user.BanReason = "Test ban";
        var result = await _repository.UpdateUserAsync(user);

        // Assert
        result.Should().BeTrue();
        
        var userInDb = await _context.Users.FindAsync(user.UserId);
        userInDb!.IsBanned.Should().BeTrue();
        userInDb.BannedAt.Should().NotBeNull();
        userInDb.BanReason.Should().Be("Test ban");
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}

