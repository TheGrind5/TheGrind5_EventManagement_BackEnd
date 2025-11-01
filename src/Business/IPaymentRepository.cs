using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Business;

public interface IPaymentRepository
{
    Task<Payment?> GetPaymentByIdAsync(int paymentId);
    Task<Payment?> GetPaymentByOrderIdAsync(int orderId);
    Task<Payment?> GetPaymentByTransactionIdAsync(string transactionId);
    Task<Payment> CreatePaymentAsync(Payment payment);
    Task<bool> UpdatePaymentStatusAsync(int paymentId, string status, string? transactionId = null);
    Task<List<Payment>> GetPaymentsByOrderIdAsync(int orderId);
}

