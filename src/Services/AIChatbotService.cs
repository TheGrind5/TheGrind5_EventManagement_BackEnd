using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Services;

public class AIChatbotService : IAIChatbotService
{
    private readonly EventDBContext _context;
    private readonly IHuggingFaceService _huggingFaceService;
    private readonly ILogger<AIChatbotService> _logger;

    public AIChatbotService(
        EventDBContext context,
        IHuggingFaceService huggingFaceService,
        ILogger<AIChatbotService> logger)
    {
        _context = context;
        _huggingFaceService = huggingFaceService;
        _logger = logger;
    }

    public async Task<ChatbotResponse> GetChatbotResponseAsync(string question, int? eventId = null, int? userId = null)
    {
        try
        {
            var lowerQuestion = question.ToLower().Trim();
            var relatedLinks = new List<string>();

            _logger.LogInformation($"Processing chatbot question: '{question}' (EventId: {eventId}, UserId: {userId})");

            // Greeting questions - Câu chào hỏi
            if (lowerQuestion.Contains("chào") || lowerQuestion.Contains("hello") || 
                lowerQuestion.Contains("xin chào") || lowerQuestion.Contains("hi") ||
                lowerQuestion.StartsWith("chào") || lowerQuestion.StartsWith("hello"))
            {
                return new ChatbotResponse
                {
                    Answer = "Xin chào! Tôi là AI Assistant của TheGrind5. Tôi có thể giúp bạn với thông tin về sự kiện, vé, thanh toán và ví điện tử. Bạn cần hỗ trợ gì không?",
                    RelatedLinks = new List<string> { "/" },
                    Confidence = "High"
                };
            }

            // Event-specific questions
            if (eventId.HasValue)
            {
                var eventData = await _context.Events
                    .Include(e => e.TicketTypes)
                    .ThenInclude(tt => tt.Tickets)
                    .FirstOrDefaultAsync(e => e.EventId == eventId.Value);

                if (eventData != null)
                {
                    relatedLinks.Add($"/events/{eventId}");
                    return await HandleEventSpecificQuestion(question, eventData);
                }
            }

            // General event information - Mở rộng keywords
            if (lowerQuestion.Contains("event") || lowerQuestion.Contains("sự kiện") ||
                lowerQuestion.Contains("sukien") || lowerQuestion.Contains("hội") ||
                lowerQuestion.Contains("tổ chức") || lowerQuestion.Contains("diễn ra"))
            {
                relatedLinks.Add("/");
                return await HandleEventInformationQuestion(question);
            }

            // Payment and refund questions
            if (lowerQuestion.Contains("refund") || lowerQuestion.Contains("hoàn tiền") || 
                lowerQuestion.Contains("payment") || lowerQuestion.Contains("thanh toán") ||
                lowerQuestion.Contains("trả tiền") || lowerQuestion.Contains("đổi trả"))
            {
                relatedLinks.Add("/wallet");
                return await HandlePaymentRefundQuestion(question, userId);
            }

            // Ticket questions
            if (lowerQuestion.Contains("ticket") || lowerQuestion.Contains("vé") ||
                lowerQuestion.Contains("ve") || lowerQuestion.Contains("mã"))
            {
                relatedLinks.Add("/my-tickets");
                return await HandleTicketQuestion(question, userId);
            }

            // Wallet questions
            if (lowerQuestion.Contains("wallet") || lowerQuestion.Contains("ví") ||
                lowerQuestion.Contains("số dư") || lowerQuestion.Contains("nạp tiền") ||
                lowerQuestion.Contains("rút tiền"))
            {
                relatedLinks.Add("/wallet");
                return await HandleWalletQuestion(question, userId);
            }

            // Help/Support questions
            if (lowerQuestion.Contains("help") || lowerQuestion.Contains("giúp") ||
                lowerQuestion.Contains("hỗ trợ") || lowerQuestion.Contains("làm sao"))
            {
                return new ChatbotResponse
                {
                    Answer = "Tôi có thể giúp bạn với:\n• Tìm kiếm và xem thông tin sự kiện\n• Hướng dẫn mua vé\n• Thông tin về thanh toán và hoàn tiền\n• Quản lý ví điện tử\n• Câu hỏi về đơn hàng\n\nBạn muốn hỏi về điều gì cụ thể?",
                    RelatedLinks = new List<string> { "/" },
                    Confidence = "High"
                };
            }

            // Default AI response - Sử dụng context từ question để generate answer
            return await HandleGeneralQuestion(question);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chatbot question");
            return new ChatbotResponse
            {
                Answer = "Xin lỗi, tôi gặp sự cố khi xử lý câu hỏi của bạn. Vui lòng thử lại sau hoặc liên hệ bộ phận hỗ trợ.",
                RelatedLinks = new List<string>(),
                Confidence = "Low"
            };
        }
    }

    private async Task<ChatbotResponse> HandleEventSpecificQuestion(string question, Models.Event eventData)
    {
        // Lấy thông tin thực về số vé đã bán và còn lại
        var totalTickets = eventData.TicketTypes.Sum(tt => tt.Quantity);
        var soldTickets = eventData.TicketTypes
            .SelectMany(tt => tt.Tickets)
            .Count(t => t.Status == "Assigned" || t.Status == "Used");
        var availableTickets = totalTickets - soldTickets;
        var minPrice = eventData.TicketTypes.Any() ? eventData.TicketTypes.Min(tt => tt.Price) : 0;
        var maxPrice = eventData.TicketTypes.Any() ? eventData.TicketTypes.Max(tt => tt.Price) : 0;
        var priceRange = minPrice == maxPrice ? $"{minPrice:N0} VNĐ" : $"{minPrice:N0} - {maxPrice:N0} VNĐ";

        var context = $"Sự kiện: {eventData.Title}\n" +
                     $"Mô tả: {eventData.Description}\n" +
                     $"Thời gian: {eventData.StartTime:dd/MM/yyyy HH:mm} - {eventData.EndTime:dd/MM/yyyy HH:mm}\n" +
                     $"Địa điểm: {eventData.Location}\n" +
                     $"Danh mục: {eventData.Category}\n" +
                     $"Giá vé: {priceRange}\n" +
                     $"Tổng số vé: {totalTickets}\n" +
                     $"Đã bán: {soldTickets} vé\n" +
                     $"Còn lại: {availableTickets} vé\n" +
                     $"Trạng thái: {eventData.Status}";

        var answer = await _huggingFaceService.GenerateWithContextAsync(question, context, 300);

        return new ChatbotResponse
        {
            Answer = answer,
            RelatedLinks = new List<string> { $"/events/{eventData.EventId}" },
            Confidence = "High"
        };
    }

    private async Task<ChatbotResponse> HandleEventInformationQuestion(string question)
    {
        // Lấy dữ liệu thực từ database
        var totalEvents = await _context.Events.CountAsync(e => e.Status == "Open");
        var upcomingEvents = await _context.Events
            .Where(e => e.Status == "Open" && e.StartTime > DateTime.UtcNow)
            .CountAsync();
        var categories = await _context.Events
            .Where(e => e.Status == "Open")
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();
        
        var categoriesText = categories.Any() 
            ? string.Join(", ", categories.Select(c => $"{c.Category} ({c.Count} sự kiện)"))
            : "Business, Entertainment, Education, Sports, Music";

        var context = "TheGrind5 là hệ thống quản lý và bán vé sự kiện. " +
                     $"Hiện tại có {totalEvents} sự kiện đang mở, trong đó {upcomingEvents} sự kiện sắp diễn ra. " +
                     $"Các danh mục phổ biến: {categoriesText}. " +
                     "Bạn có thể duyệt các sự kiện, mua vé, quản lý ví điện tử và tạo sự kiện.";

        var answer = await _huggingFaceService.GenerateWithContextAsync(question, context, 250);

        return new ChatbotResponse
        {
            Answer = answer,
            RelatedLinks = new List<string> { "/" },
            Confidence = "Medium"
        };
    }

    private async Task<ChatbotResponse> HandlePaymentRefundQuestion(string question, int? userId)
    {
        var context = "Hệ thống hỗ trợ thanh toán qua ví điện tử, thẻ tín dụng hoặc chuyển khoản ngân hàng. " +
                     "Bạn có thể yêu cầu hoàn tiền cho đơn hàng chưa sử dụng trong vòng 7 ngày trước sự kiện. " +
                     "Tiền sẽ được hoàn lại vào ví điện tử trong vòng 3-5 ngày làm việc.";

        // Lấy thông tin thực của user nếu có
        if (userId.HasValue)
        {
            var user = await _context.Users.FindAsync(userId.Value);
            if (user != null)
            {
                var refundableOrders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.TicketType)
                            .ThenInclude(tt => tt.Event)
                    .Where(o => o.CustomerId == userId.Value && 
                               o.Status == "Paid" &&
                               o.OrderItems.Any(oi => oi.TicketType.Event.StartTime > DateTime.UtcNow.AddDays(7)))
                    .CountAsync();

                if (refundableOrders > 0)
                {
                    context += $" Bạn hiện có {refundableOrders} đơn hàng đủ điều kiện hoàn tiền.";
                }
            }
        }

        var answer = await _huggingFaceService.GenerateWithContextAsync(question, context, 250);

        return new ChatbotResponse
        {
            Answer = answer,
            RelatedLinks = new List<string> { "/wallet", "/my-tickets" },
            Confidence = "High"
        };
    }

    private async Task<ChatbotResponse> HandleTicketQuestion(string question, int? userId)
    {
        var context = "Vé điện tử được gửi qua email sau khi thanh toán thành công. " +
                     "Bạn có thể xem và quản lý vé của mình trong phần 'Vé của tôi'. " +
                     "Mỗi vé có mã serial number duy nhất và có thể hiển thị QR code.";

        // Lấy thông tin vé thực của user nếu có
        if (userId.HasValue)
        {
            // Lấy danh sách OrderItemIds của user trước
            var orderItemIds = await _context.Orders
                .Where(o => o.CustomerId == userId.Value && o.Status == "Paid")
                .SelectMany(o => o.OrderItems.Select(oi => oi.OrderItemId))
                .ToListAsync();

            // Sau đó query tickets từ OrderItemIds
            var userTickets = await _context.Tickets
                .Include(t => t.TicketType)
                    .ThenInclude(tt => tt.Event)
                .Where(t => t.OrderItemId.HasValue && orderItemIds.Contains(t.OrderItemId.Value))
                .ToListAsync();

            var totalTickets = userTickets.Count;
            var usedTickets = userTickets.Count(t => t.Status == "Used");
            var availableTickets = userTickets.Count(t => t.Status == "Assigned");
            var upcomingTickets = userTickets.Count(t => t.TicketType.Event.StartTime > DateTime.UtcNow);

            if (totalTickets > 0)
            {
                context += $" Bạn hiện có {totalTickets} vé, trong đó {availableTickets} vé chưa sử dụng, " +
                          $"{usedTickets} vé đã sử dụng, và {upcomingTickets} vé cho sự kiện sắp tới.";
            }
        }

        var answer = await _huggingFaceService.GenerateWithContextAsync(question, context, 250);

        return new ChatbotResponse
        {
            Answer = answer,
            RelatedLinks = new List<string> { "/my-tickets" },
            Confidence = "High"
        };
    }

    private async Task<ChatbotResponse> HandleWalletQuestion(string question, int? userId)
    {
        var context = "Ví điện tử của bạn cho phép nạp tiền, rút tiền và thanh toán mua vé. " +
                     "Bạn có thể xem lịch sử giao dịch chi tiết trong phần 'Ví của tôi'. " +
                     "Nạp tiền và thanh toán được xử lý ngay lập tức.";

        // Lấy thông tin ví thực của user nếu có
        if (userId.HasValue)
        {
            var user = await _context.Users.FindAsync(userId.Value);
            if (user != null)
            {
                var walletBalance = user.WalletBalance;
                var recentTransactions = await _context.WalletTransactions
                    .Where(wt => wt.UserId == userId.Value)
                    .OrderByDescending(wt => wt.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                context += $" Số dư hiện tại của bạn: {walletBalance:N0} VNĐ.";
                
                if (recentTransactions.Any())
                {
                    var lastTransaction = recentTransactions.First();
                    context += $" Giao dịch gần nhất: {lastTransaction.TransactionType} " +
                              $"{(lastTransaction.Amount >= 0 ? "+" : "")}{lastTransaction.Amount:N0} VNĐ " +
                              $"vào {lastTransaction.CreatedAt:dd/MM/yyyy}.";
                }
            }
        }

        var answer = await _huggingFaceService.GenerateWithContextAsync(question, context, 250);

        return new ChatbotResponse
        {
            Answer = answer,
            RelatedLinks = new List<string> { "/wallet" },
            Confidence = "High"
        };
    }

    private async Task<ChatbotResponse> HandleGeneralQuestion(string question)
    {
        // Lấy thống kê thực từ hệ thống
        var totalEvents = await _context.Events.CountAsync(e => e.Status == "Open");
        var totalUsers = await _context.Users.CountAsync();
        var totalOrders = await _context.Orders.CountAsync(o => o.Status == "Paid");
        var popularCategories = await _context.Events
            .Where(e => e.Status == "Open")
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(3)
            .ToListAsync();

        // Tạo context động dựa trên question
        var context = $"Câu hỏi của bạn: {question}\n\n" +
                     $"Tôi là trợ lý AI của TheGrind5 - hệ thống quản lý sự kiện với: " +
                     $"{totalEvents} sự kiện đang mở, {totalUsers} người dùng, và {totalOrders} đơn hàng đã thanh toán. " +
                     $"Các danh mục phổ biến: {(popularCategories.Any() ? string.Join(", ", popularCategories.Select(c => $"{c.Category} ({c.Count})")) : "Business, Entertainment, Education")}. " +
                     "Tôi có thể giúp bạn với: thông tin về sự kiện, hướng dẫn mua vé, thanh toán và hoàn tiền, quản lý ví điện tử. " +
                     "Hãy trả lời câu hỏi của người dùng một cách tự nhiên và hữu ích.";

        var answer = await _huggingFaceService.GenerateWithContextAsync(question, context, 300);

        // Nếu API không hoạt động, trả về response thông minh hơn
        if (string.IsNullOrWhiteSpace(answer) || 
            answer.Contains("Xin lỗi") || 
            answer.Length < 20) // Nếu answer quá ngắn, có thể là fallback
        {
            var lowerQuestion = question.ToLower();
            
            if (lowerQuestion.Contains("tôi hỏi") || lowerQuestion.Contains("được không") ||
                lowerQuestion.Contains("có thể"))
            {
                return new ChatbotResponse
                {
                    Answer = "Tất nhiên rồi! Tôi sẵn sàng trả lời mọi câu hỏi của bạn về TheGrind5. Bạn muốn hỏi về sự kiện, vé, thanh toán hay ví điện tử?",
                    RelatedLinks = new List<string> { "/" },
                    Confidence = "High"
                };
            }
            
            return new ChatbotResponse
            {
                Answer = $"Cảm ơn bạn đã hỏi về '{question}'. TheGrind5 hiện có {totalEvents} sự kiện đang mở và {totalUsers} người dùng. " +
                        $"Bạn có thể khám phá các sự kiện tại trang chủ hoặc hỏi tôi về vé, thanh toán hoặc ví điện tử.",
                RelatedLinks = new List<string> { "/" },
                Confidence = "Medium"
            };
        }

        return new ChatbotResponse
        {
            Answer = answer,
            RelatedLinks = new List<string> { "/" },
            Confidence = "Medium"
        };
    }
}

