using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Services;
using TheGrind5_EventManagement.Helpers;

namespace TheGrind5_EventManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VoucherController : ControllerBase
{
    private readonly IVoucherService _voucherService;

    public VoucherController(IVoucherService voucherService)
    {
        _voucherService = voucherService;
    }

    /// <summary>
    /// Validate voucher code and calculate discount
    /// </summary>
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateVoucher([FromBody] VoucherValidationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.VoucherCode))
            {
                return BadRequest(ApiResponseHelper.BadRequest("Mã voucher không được để trống"));
            }

            if (request.OriginalAmount <= 0)
            {
                return BadRequest(ApiResponseHelper.BadRequest("Số tiền phải lớn hơn 0"));
            }

            var result = await _voucherService.ValidateVoucherAsync(request);
            
            if (result.IsValid)
            {
                return Ok(ApiResponseHelper.Success(result, "Voucher hợp lệ"));
            }
            else
            {
                return BadRequest(ApiResponseHelper.BadRequest(result.Message));
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponseHelper.InternalServerError($"Lỗi khi validate voucher: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get voucher by code
    /// </summary>
    [HttpGet("code/{voucherCode}")]
    public async Task<IActionResult> GetVoucherByCode(string voucherCode)
    {
        try
        {
            var voucher = await _voucherService.GetVoucherByCodeAsync(voucherCode);
            
            if (voucher == null)
            {
                return NotFound(ApiResponseHelper.NotFound("Voucher không tồn tại"));
            }

            return Ok(ApiResponseHelper.Success(voucher));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponseHelper.InternalServerError($"Lỗi khi lấy thông tin voucher: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get all vouchers (Admin only)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllVouchers()
    {
        try
        {
            var vouchers = await _voucherService.GetAllVouchersAsync();
            return Ok(ApiResponseHelper.Success(vouchers));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponseHelper.InternalServerError($"Lỗi khi lấy danh sách voucher: {ex.Message}"));
        }
    }

    /// <summary>
    /// Create new voucher (Admin only)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateVoucher([FromBody] VoucherCreateDTO createDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(createDto.VoucherCode))
            {
                return BadRequest(ApiResponseHelper.BadRequest("Mã voucher không được để trống"));
            }

            if (createDto.DiscountPercentage <= 0 || createDto.DiscountPercentage > 100)
            {
                return BadRequest(ApiResponseHelper.BadRequest("Phần trăm giảm giá phải từ 1 đến 100"));
            }

            if (createDto.ValidFrom >= createDto.ValidTo)
            {
                return BadRequest(ApiResponseHelper.BadRequest("Ngày bắt đầu phải nhỏ hơn ngày kết thúc"));
            }

            var voucher = await _voucherService.CreateVoucherAsync(createDto);
            return Ok(ApiResponseHelper.Success(voucher, "Tạo voucher thành công"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponseHelper.InternalServerError($"Lỗi khi tạo voucher: {ex.Message}"));
        }
    }
}
