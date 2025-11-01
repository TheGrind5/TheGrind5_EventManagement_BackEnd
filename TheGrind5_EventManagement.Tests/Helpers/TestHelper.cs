using System.Security.Claims;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Tests.Helpers;

/// <summary>
/// Helper methods cho tests
/// </summary>
public static class TestHelper
{
    /// <summary>
    /// Tạo ClaimsPrincipal từ user ID và role
    /// </summary>
    public static ClaimsPrincipal CreateClaimsPrincipal(int userId, string role = "Customer", string email = "test@example.com")
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Email, email)
        };

        var identity = new ClaimsIdentity(claims, "Test");
        return new ClaimsPrincipal(identity);
    }

    /// <summary>
    /// Tạo ClaimsPrincipal từ User object
    /// </summary>
    public static ClaimsPrincipal CreateClaimsPrincipalFromUser(User user)
    {
        return CreateClaimsPrincipal(user.UserId, user.Role, user.Email);
    }

    /// <summary>
    /// Verify rằng một object có property với value cụ thể
    /// </summary>
    public static void VerifyProperty<T>(T obj, string propertyName, object expectedValue)
    {
        var property = typeof(T).GetProperty(propertyName);
        if (property == null)
        {
            throw new ArgumentException($"Property {propertyName} not found on type {typeof(T).Name}");
        }

        var actualValue = property.GetValue(obj);
        if (actualValue == null && expectedValue == null)
        {
            return;
        }

        if (!object.Equals(actualValue, expectedValue))
        {
            throw new Exception($"Property {propertyName} expected {expectedValue} but was {actualValue}");
        }
    }

    /// <summary>
    /// Tạo random string cho tests
    /// </summary>
    public static string RandomString(int length = 10)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Tạo random email
    /// </summary>
    public static string RandomEmail()
    {
        return $"test_{RandomString(8)}@example.com";
    }

    /// <summary>
    /// Tạo random phone number
    /// </summary>
    public static string RandomPhone()
    {
        var random = new Random();
        return $"0{random.Next(100000000, 999999999)}";
    }

    /// <summary>
    /// Verify rằng một collection chứa item thỏa điều kiện
    /// </summary>
    public static void VerifyContains<T>(IEnumerable<T> collection, Func<T, bool> predicate, string errorMessage = "Collection does not contain expected item")
    {
        if (!collection.Any(predicate))
        {
            throw new Exception(errorMessage);
        }
    }

    /// <summary>
    /// Verify rằng một collection không chứa item thỏa điều kiện
    /// </summary>
    public static void VerifyNotContains<T>(IEnumerable<T> collection, Func<T, bool> predicate, string errorMessage = "Collection contains unexpected item")
    {
        if (collection.Any(predicate))
        {
            throw new Exception(errorMessage);
        }
    }

    /// <summary>
    /// Tạo DateTime trong tương lai
    /// </summary>
    public static DateTime FutureDate(int daysFromNow = 7)
    {
        return DateTime.UtcNow.AddDays(daysFromNow);
    }

    /// <summary>
    /// Tạo DateTime trong quá khứ
    /// </summary>
    public static DateTime PastDate(int daysAgo = 7)
    {
        return DateTime.UtcNow.AddDays(-daysAgo);
    }

    /// <summary>
    /// Check rằng một task throws exception cụ thể
    /// </summary>
    public static async Task AssertThrowsAsync<TException>(Func<Task> task) where TException : Exception
    {
        try
        {
            await task();
            throw new Exception($"Expected exception of type {typeof(TException).Name} but no exception was thrown");
        }
        catch (TException)
        {
            // Expected exception
        }
        catch (Exception ex)
        {
            throw new Exception($"Expected exception of type {typeof(TException).Name} but got {ex.GetType().Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Verify rằng status is valid
    /// </summary>
    public static void VerifyStatus(string status, params string[] validStatuses)
    {
        if (!validStatuses.Contains(status))
        {
            throw new ArgumentException($"Invalid status: {status}. Valid statuses are: {string.Join(", ", validStatuses)}");
        }
    }

    /// <summary>
    /// Wait một thời gian ngắn cho async operations
    /// </summary>
    public static async Task DelayAsync(int milliseconds = 100)
    {
        await Task.Delay(milliseconds);
    }
}

