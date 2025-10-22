using System.ComponentModel.DataAnnotations;

namespace TheGrind5_EventManagement.DTOs
{
    // Request DTOs
    public record DepositRequest
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; init; }

        public string? Description { get; init; }
    }

    public record WithdrawRequest
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; init; }

        public string? Description { get; init; }
    }

    // Response DTOs
    public record WalletBalanceResponse
    {
        public decimal Balance { get; init; }
        public string Currency { get; init; } = "VND";
        public DateTime LastUpdated { get; init; }
    }

    public record WalletTransactionResponse
    {
        public int TransactionId { get; init; }
        public decimal Amount { get; init; }
        public string TransactionType { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? ReferenceId { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? CompletedAt { get; init; }
        public decimal BalanceBefore { get; init; }
        public decimal BalanceAfter { get; init; }
    }

    public record WalletTransactionListResponse
    {
        public List<WalletTransactionResponse> Transactions { get; init; } = new();
        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalPages { get; init; }
    }

    public record WalletOperationResponse
    {
        public string Message { get; init; } = string.Empty;
        public int TransactionId { get; init; }
        public decimal Amount { get; init; }
        public decimal NewBalance { get; init; }
        public string Status { get; init; } = string.Empty;
    }
}
