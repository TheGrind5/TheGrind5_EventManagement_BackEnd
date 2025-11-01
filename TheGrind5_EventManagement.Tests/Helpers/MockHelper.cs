using Moq;
using Microsoft.Extensions.Logging;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.Services;

namespace TheGrind5_EventManagement.Tests.Helpers;

/// <summary>
/// Helper methods để tạo mock objects
/// </summary>
public static class MockHelper
{
    /// <summary>
    /// Tạo mock ILogger
    /// </summary>
    public static Mock<ILogger<T>> CreateMockLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }

    /// <summary>
    /// Tạo mock IUserRepository
    /// </summary>
    public static Mock<IUserRepository> CreateMockUserRepository()
    {
        return new Mock<IUserRepository>();
    }

    /// <summary>
    /// Tạo mock IEventRepository
    /// </summary>
    public static Mock<IEventRepository> CreateMockEventRepository()
    {
        return new Mock<IEventRepository>();
    }

    /// <summary>
    /// Tạo mock IOrderRepository
    /// </summary>
    public static Mock<IOrderRepository> CreateMockOrderRepository()
    {
        return new Mock<IOrderRepository>();
    }

    /// <summary>
    /// Tạo mock IAuthService
    /// </summary>
    public static Mock<IAuthService> CreateMockAuthService()
    {
        return new Mock<IAuthService>();
    }

    /// <summary>
    /// Tạo mock IEventService
    /// </summary>
    public static Mock<IEventService> CreateMockEventService()
    {
        return new Mock<IEventService>();
    }

    /// <summary>
    /// Tạo mock IOrderService
    /// </summary>
    public static Mock<IOrderService> CreateMockOrderService()
    {
        return new Mock<IOrderService>();
    }

    /// <summary>
    /// Tạo mock ITicketService
    /// </summary>
    public static Mock<ITicketService> CreateMockTicketService()
    {
        return new Mock<ITicketService>();
    }

    /// <summary>
    /// Tạo mock IWalletService
    /// </summary>
    public static Mock<IWalletService> CreateMockWalletService()
    {
        return new Mock<IWalletService>();
    }

    /// <summary>
    /// Tạo mock IJwtService
    /// </summary>
    public static Mock<IJwtService> CreateMockJwtService()
    {
        return new Mock<IJwtService>();
    }

    /// <summary>
    /// Tạo mock IPasswordService
    /// </summary>
    public static Mock<IPasswordService> CreateMockPasswordService()
    {
        return new Mock<IPasswordService>();
    }

    /// <summary>
    /// Tạo mock IEmailService
    /// </summary>
    public static Mock<IEmailService> CreateMockEmailService()
    {
        return new Mock<IEmailService>();
    }

    /// <summary>
    /// Tạo mock IFileManagementService
    /// </summary>
    public static Mock<IFileManagementService> CreateMockFileManagementService()
    {
        return new Mock<IFileManagementService>();
    }

    /// <summary>
    /// Tạo mock IVoucherService
    /// </summary>
    public static Mock<IVoucherService> CreateMockVoucherService()
    {
        return new Mock<IVoucherService>();
    }

    /// <summary>
    /// Tạo mock INotificationService
    /// </summary>
    public static Mock<INotificationService> CreateMockNotificationService()
    {
        return new Mock<INotificationService>();
    }

    /// <summary>
    /// Setup mock repository với user data
    /// </summary>
    public static void SetupMockUserRepository(Mock<IUserRepository> mock, User? user = null)
    {
        if (user == null)
        {
            user = Fixtures.TestDataBuilder.CreateUser();
        }

        mock.Setup(x => x.GetUserByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => id == user.UserId ? user : null);

        mock.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) => email == user.Email ? user : null);
    }

    /// <summary>
    /// Setup mock service với success response
    /// </summary>
    public static void SetupSuccessResponse<T>(Mock<T> mock, string methodName, object returnValue) where T : class
    {
        var method = typeof(T).GetMethod(methodName);
        if (method != null)
        {
            // This is a simplified setup - in practice, you'd need more specific setup based on method signature
        }
    }

    /// <summary>
    /// Verify mock được gọi với parameters cụ thể
    /// </summary>
    public static void VerifyCalled<T>(Mock<T> mock, System.Linq.Expressions.Expression<System.Action<T>> expression, Times times) where T : class
    {
        mock.Verify(expression, times);
    }

    /// <summary>
    /// Verify mock được gọi với async method
    /// </summary>
    public static void VerifyCalledAsync<T>(Mock<T> mock, System.Linq.Expressions.Expression<System.Func<T, Task>> expression, Times times) where T : class
    {
        mock.Verify(expression, times);
    }

    /// <summary>
    /// Verify mock KHÔNG được gọi
    /// </summary>
    public static void VerifyNotCalled<T>(Mock<T> mock, System.Linq.Expressions.Expression<System.Action<T>> expression) where T : class
    {
        mock.Verify(expression, Times.Never);
    }
}

