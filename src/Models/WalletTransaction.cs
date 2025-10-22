using System.ComponentModel.DataAnnotations;

namespace TheGrind5_EventManagement.Models
{
    public class WalletTransaction
    {
        public int TransactionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [RegularExpression("^(Deposit|Withdraw|Payment|Refund|Transfer_In|Transfer_Out)$", 
            ErrorMessage = "Transaction type must be Deposit, Withdraw, Payment, Refund, Transfer_In, or Transfer_Out")]
        public string TransactionType { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Pending|Completed|Failed|Cancelled)$", 
            ErrorMessage = "Status must be Pending, Completed, Failed, or Cancelled")]
        public string Status { get; set; } = "Pending";

        public string? Description { get; set; }

        public string? ReferenceId { get; set; } // Reference to OrderId, PaymentId, etc.

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public decimal BalanceBefore { get; set; }

        public decimal BalanceAfter { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
    }
}
