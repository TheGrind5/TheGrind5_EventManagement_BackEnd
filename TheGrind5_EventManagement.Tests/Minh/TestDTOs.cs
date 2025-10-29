using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Tests.Minh
{

// Test DTOs for unit testing
public class OrderReadDTO
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public int EventId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class WalletTransactionDTO
{
    public int TransactionId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// PaymentRequest is already defined in the main DTOs, so we don't need to redefine it here
}
