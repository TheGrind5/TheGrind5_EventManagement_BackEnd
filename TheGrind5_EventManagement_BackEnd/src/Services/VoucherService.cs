using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Services;

public interface IVoucherService
{
    Task<VoucherValidationResponse> ValidateVoucherAsync(VoucherValidationRequest request);
    Task<VoucherDTO?> GetVoucherByCodeAsync(string voucherCode);
    Task<IEnumerable<VoucherDTO>> GetAllVouchersAsync();
    Task<VoucherDTO> CreateVoucherAsync(VoucherCreateDTO createDto);
}

public class VoucherService : IVoucherService
{
    private readonly EventDBContext _context;

    public VoucherService(EventDBContext context)
    {
        _context = context;
    }

    public async Task<VoucherValidationResponse> ValidateVoucherAsync(VoucherValidationRequest request)
    {
        var response = new VoucherValidationResponse
        {
            VoucherCode = request.VoucherCode
        };

        // Tìm voucher theo code
        var voucher = await _context.Vouchers
            .FirstOrDefaultAsync(v => v.VoucherCode == request.VoucherCode && v.IsActive);

        if (voucher == null)
        {
            response.IsValid = false;
            response.Message = "Voucher không tồn tại";
            return response;
        }

        // Kiểm tra thời hạn
        var now = DateTime.UtcNow;
        if (now < voucher.ValidFrom)
        {
            response.IsValid = false;
            response.Message = "Voucher chưa có hiệu lực";
            return response;
        }

        if (now > voucher.ValidTo)
        {
            response.IsValid = false;
            response.Message = "Voucher này đã hết hạn";
            return response;
        }

        // Voucher hợp lệ
        response.IsValid = true;
        response.Message = "Voucher hợp lệ";
        response.DiscountPercentage = voucher.DiscountPercentage;
        response.DiscountAmount = voucher.CalculateDiscountAmount(request.OriginalAmount);
        response.FinalAmount = voucher.CalculateFinalAmount(request.OriginalAmount);

        return response;
    }

    public async Task<VoucherDTO?> GetVoucherByCodeAsync(string voucherCode)
    {
        var voucher = await _context.Vouchers
            .FirstOrDefaultAsync(v => v.VoucherCode == voucherCode);

        if (voucher == null)
            return null;

        return new VoucherDTO
        {
            VoucherId = voucher.VoucherId,
            VoucherCode = voucher.VoucherCode,
            DiscountPercentage = voucher.DiscountPercentage,
            ValidFrom = voucher.ValidFrom,
            ValidTo = voucher.ValidTo,
            IsActive = voucher.IsActive,
            CreatedAt = voucher.CreatedAt
        };
    }

    public async Task<IEnumerable<VoucherDTO>> GetAllVouchersAsync()
    {
        var vouchers = await _context.Vouchers
            .OrderBy(v => v.VoucherCode)
            .ToListAsync();

        return vouchers.Select(v => new VoucherDTO
        {
            VoucherId = v.VoucherId,
            VoucherCode = v.VoucherCode,
            DiscountPercentage = v.DiscountPercentage,
            ValidFrom = v.ValidFrom,
            ValidTo = v.ValidTo,
            IsActive = v.IsActive,
            CreatedAt = v.CreatedAt
        });
    }

    public async Task<VoucherDTO> CreateVoucherAsync(VoucherCreateDTO createDto)
    {
        var voucher = new Voucher
        {
            VoucherCode = createDto.VoucherCode,
            DiscountPercentage = createDto.DiscountPercentage,
            ValidFrom = createDto.ValidFrom,
            ValidTo = createDto.ValidTo,
            IsActive = createDto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.Vouchers.Add(voucher);
        await _context.SaveChangesAsync();

        return new VoucherDTO
        {
            VoucherId = voucher.VoucherId,
            VoucherCode = voucher.VoucherCode,
            DiscountPercentage = voucher.DiscountPercentage,
            ValidFrom = voucher.ValidFrom,
            ValidTo = voucher.ValidTo,
            IsActive = voucher.IsActive,
            CreatedAt = voucher.CreatedAt
        };
    }
}
