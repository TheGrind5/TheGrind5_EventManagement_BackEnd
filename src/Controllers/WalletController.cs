using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetWalletBalance()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var balance = await _walletService.GetWalletBalanceAsync(userId.Value);
                
                var response = new WalletBalanceResponse
                {
                    Balance = balance,
                    Currency = "VND",
                    LastUpdated = DateTime.UtcNow
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy số dư ví", error = ex.Message });
            }
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });

                var transaction = await _walletService.DepositAsync(
                    userId.Value, 
                    request.Amount, 
                    request.Description);

                var response = new WalletOperationResponse
                {
                    Message = "Nạp tiền vào ví thành công",
                    TransactionId = transaction.TransactionId,
                    Amount = transaction.Amount,
                    NewBalance = transaction.BalanceAfter,
                    Status = transaction.Status
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi nạp tiền", error = ex.Message });
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });

                var transaction = await _walletService.WithdrawAsync(
                    userId.Value, 
                    request.Amount, 
                    request.Description);

                var response = new WalletOperationResponse
                {
                    Message = "Rút tiền từ ví thành công",
                    TransactionId = transaction.TransactionId,
                    Amount = transaction.Amount,
                    NewBalance = transaction.BalanceAfter,
                    Status = transaction.Status
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi rút tiền", error = ex.Message });
            }
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var transactions = await _walletService.GetUserTransactionsAsync(userId.Value, page, pageSize);
                
                var transactionResponses = transactions.Select(t => new WalletTransactionResponse
                {
                    TransactionId = t.TransactionId,
                    Amount = t.Amount,
                    TransactionType = t.TransactionType,
                    Status = t.Status,
                    Description = t.Description,
                    ReferenceId = t.ReferenceId,
                    CreatedAt = t.CreatedAt,
                    CompletedAt = t.CompletedAt,
                    BalanceBefore = t.BalanceBefore,
                    BalanceAfter = t.BalanceAfter
                }).ToList();

                // Note: In a real implementation, you'd need to get total count from service
                var response = new WalletTransactionListResponse
                {
                    Transactions = transactionResponses,
                    TotalCount = transactionResponses.Count, // This should be actual total count
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)transactionResponses.Count / pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy lịch sử giao dịch", error = ex.Message });
            }
        }

        [HttpGet("transactions/{transactionId}")]
        public async Task<IActionResult> GetTransaction(int transactionId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var transaction = await _walletService.GetTransactionByIdAsync(transactionId);
                if (transaction == null)
                    return NotFound(new { message = "Không tìm thấy giao dịch" });

                // Check if transaction belongs to current user
                if (transaction.UserId != userId.Value)
                    return Forbid("Bạn chỉ có thể xem giao dịch của mình");

                var response = new WalletTransactionResponse
                {
                    TransactionId = transaction.TransactionId,
                    Amount = transaction.Amount,
                    TransactionType = transaction.TransactionType,
                    Status = transaction.Status,
                    Description = transaction.Description,
                    ReferenceId = transaction.ReferenceId,
                    CreatedAt = transaction.CreatedAt,
                    CompletedAt = transaction.CompletedAt,
                    BalanceBefore = transaction.BalanceBefore,
                    BalanceAfter = transaction.BalanceAfter
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy thông tin giao dịch", error = ex.Message });
            }
        }

        [HttpGet("check-balance")]
        public async Task<IActionResult> CheckSufficientBalance([FromQuery] decimal amount)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                if (amount <= 0)
                    return BadRequest(new { message = "Số tiền phải lớn hơn 0" });

                var hasSufficientBalance = await _walletService.HasSufficientBalanceAsync(userId.Value, amount);
                var currentBalance = await _walletService.GetWalletBalanceAsync(userId.Value);

                return Ok(new
                {
                    hasSufficientBalance,
                    currentBalance,
                    requiredAmount = amount,
                    shortfall = hasSufficientBalance ? 0 : amount - currentBalance
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi kiểm tra số dư", error = ex.Message });
            }
        }

        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}
