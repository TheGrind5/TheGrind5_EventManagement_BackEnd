using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly EventDBContext _context;

    public PaymentRepository(EventDBContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
    {
        return await _context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
    }

    public async Task<Payment?> GetPaymentByOrderIdAsync(int orderId)
    {
        return await _context.Payments
            .Include(p => p.Order)
            .Where(p => p.OrderId == orderId)
            .OrderByDescending(p => p.PaymentDate)
            .FirstOrDefaultAsync();
    }

    public async Task<Payment?> GetPaymentByTransactionIdAsync(string transactionId)
    {
        return await _context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
    }

    public async Task<Payment> CreatePaymentAsync(Payment payment)
    {
        payment.CreatedAt = DateTime.UtcNow;
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<bool> UpdatePaymentStatusAsync(int paymentId, string status, string? transactionId = null)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment == null) return false;

        payment.Status = status;
        payment.UpdatedAt = DateTime.UtcNow;
        if (transactionId != null)
        {
            payment.TransactionId = transactionId;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Payment>> GetPaymentsByOrderIdAsync(int orderId)
    {
        return await _context.Payments
            .Where(p => p.OrderId == orderId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }
}

