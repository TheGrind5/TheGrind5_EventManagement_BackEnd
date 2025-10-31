using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using MimeKit;
using TheGrind5_EventManagement.Business;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;

namespace TheGrind5_EventManagement.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private GmailService? _gmailService;

    // Email configuration - centralized
    private string SenderEmail => _configuration["Gmail:SenderEmail"] ?? "nguyenluonghoangthien5605@gmail.com";

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private async Task<GmailService> GetGmailServiceAsync()
    {
        if (_gmailService != null)
            return _gmailService;

        try
        {
            var clientId = _configuration["Gmail:ClientId"] ?? "YOUR_CLIENT_ID";
            var clientSecret = _configuration["Gmail:ClientSecret"] ?? "YOUR_CLIENT_SECRET";
            var refreshToken = _configuration["Gmail:RefreshToken"] ?? "";

            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogError("Gmail RefreshToken is not configured");
                throw new InvalidOperationException("Gmail RefreshToken is not configured");
            }

            var applicationName = _configuration["Gmail:ApplicationName"] ?? "TheGrind5 Event Management";

            var tokenResponse = new TokenResponse
            {
                RefreshToken = refreshToken
            };

            var secrets = new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets,
                Scopes = new[] { GmailService.Scope.GmailSend }
            });

            var credential = new UserCredential(flow, "user", tokenResponse);

            _gmailService = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });

            _logger.LogInformation("Gmail API service initialized successfully");
            return _gmailService;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Gmail API service");
            throw;
        }
    }

    private async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            // Create MIME message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("TheGrind5 Event Management", SenderEmail));
            message.To.Add(new MailboxAddress("", toEmail));

            message.Subject = subject;
            
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            // Convert to Gmail API format
            using (var stream = new MemoryStream())
            {
                message.WriteTo(stream);
                var rawMessage = Convert.ToBase64String(stream.ToArray())
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Replace("=", "");

                var service = await GetGmailServiceAsync();
                var gmailMessage = new Message { Raw = rawMessage };

                await service.Users.Messages.Send(gmailMessage, "me").ExecuteAsync();
            }

            _logger.LogInformation($"Email sent successfully to {toEmail}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to {toEmail}");
            return false;
        }
    }

    public async Task<bool> SendOtpEmailAsync(string toEmail, string otpCode)
    {
        return await SendEmailAsync(toEmail, "Mã OTP để đặt lại mật khẩu", CreateOtpEmailBody(otpCode));
    }

    private string CreateOtpEmailBody(string otpCode)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                .content {{ background-color: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }}
                .otp-code {{ 
                    font-size: 32px; 
                    font-weight: bold; 
                    color: #007bff; 
                    text-align: center; 
                    background-color: #e9ecef; 
                    padding: 20px; 
                    border-radius: 8px; 
                    margin: 20px 0; 
                    letter-spacing: 8px;
                }}
                .warning {{ 
                    background-color: #fff3cd; 
                    border: 1px solid #ffeaa7; 
                    color: #856404; 
                    padding: 15px; 
                    border-radius: 5px; 
                    margin: 20px 0; 
                }}
                .footer {{ text-align: center; margin-top: 30px; color: #6c757d; font-size: 14px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>TheGrind5 Event Management</h1>
                </div>
                <div class='content'>
                    <h2>Mã OTP để đặt lại mật khẩu</h2>
                    <p>Xin chào,</p>
                    <p>Bạn đã yêu cầu đặt lại mật khẩu cho tài khoản của mình. Vui lòng sử dụng mã OTP sau đây:</p>
                    
                    <div class='otp-code'>{otpCode}</div>
                    
                    <div class='warning'>
                        <strong>⚠️ Lưu ý quan trọng:</strong><br>
                        • Mã OTP này chỉ có hiệu lực trong 60 giây<br>
                        • Không chia sẻ mã này với bất kỳ ai<br>
                        • Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này
                    </div>
                    
                    <p>Nếu bạn gặp vấn đề, vui lòng liên hệ với chúng tôi.</p>
                </div>
                <div class='footer'>
                    <p>© 2024 TheGrind5 Event Management. Tất cả quyền được bảo lưu.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    public async Task<bool> SendEventReminderEmailAsync(string toEmail, string eventName, DateTime eventStartTime)
    {
        return await SendEmailAsync(toEmail, $"Nhắc nhở sự kiện: {eventName}", CreateEventReminderEmailBody(eventName, eventStartTime));
    }

    public async Task<bool> SendEventUpdateEmailAsync(string toEmail, string eventName, string updateMessage)
    {
        return await SendEmailAsync(toEmail, $"Cập nhật sự kiện: {eventName}", CreateEventUpdateEmailBody(eventName, updateMessage));
    }

    public async Task<bool> SendEventCancelledEmailAsync(string toEmail, string eventName)
    {
        return await SendEmailAsync(toEmail, $"Sự kiện đã bị hủy: {eventName}", CreateEventCancelledEmailBody(eventName));
    }

    public async Task<bool> SendOrderConfirmationEmailAsync(string toEmail, int orderId, decimal amount)
    {
        return await SendEmailAsync(toEmail, "Xác nhận đơn hàng", CreateOrderConfirmationEmailBody(orderId, amount));
    }

    public async Task<bool> SendPaymentSuccessEmailAsync(string toEmail, int orderId, decimal amount)
    {
        return await SendEmailAsync(toEmail, "Thanh toán thành công", CreatePaymentSuccessEmailBody(orderId, amount));
    }

    public async Task<bool> SendRefundEmailAsync(string toEmail, int orderId, decimal refundAmount)
    {
        return await SendEmailAsync(toEmail, "Hoàn tiền đã được xử lý", CreateRefundEmailBody(orderId, refundAmount));
    }

    private string CreateEventReminderEmailBody(string eventName, DateTime eventStartTime)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                .header {{ background-color: #28a745; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                .content {{ background-color: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }}
                .event-info {{ background-color: white; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #28a745; }}
                .footer {{ text-align: center; margin-top: 30px; color: #6c757d; font-size: 14px; }}
                .button {{ background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block; margin-top: 10px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>📅 Nhắc nhở sự kiện</h1>
                </div>
                <div class='content'>
                    <p>Xin chào,</p>
                    <p>Chúng tôi muốn nhắc nhở bạn về sự kiện sắp tới:</p>
                    
                    <div class='event-info'>
                        <h3>{eventName}</h3>
                        <p><strong>Thời gian:</strong> {eventStartTime:dd/MM/yyyy HH:mm}</p>
                    </div>
                    
                    <p>Hãy sắp xếp thời gian để tham gia sự kiện. Chúng tôi hy vọng sẽ được gặp bạn!</p>
                </div>
                <div class='footer'>
                    <p>© 2024 TheGrind5 Event Management. Tất cả quyền được bảo lưu.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    private string CreateEventUpdateEmailBody(string eventName, string updateMessage)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                .header {{ background-color: #ffc107; color: #333; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                .content {{ background-color: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }}
                .update-box {{ background-color: #fff3cd; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #ffc107; }}
                .footer {{ text-align: center; margin-top: 30px; color: #6c757d; font-size: 14px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>📢 Cập nhật sự kiện</h1>
                </div>
                <div class='content'>
                    <p>Xin chào,</p>
                    <p>Chúng tôi có thông tin cập nhật về sự kiện:</p>
                    
                    <div class='update-box'>
                        <h3>{eventName}</h3>
                        <p>{updateMessage}</p>
                    </div>
                    
                    <p>Vui lòng kiểm tra thông tin và cập nhật lịch trình nếu cần thiết.</p>
                </div>
                <div class='footer'>
                    <p>© 2024 TheGrind5 Event Management. Tất cả quyền được bảo lưu.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    private string CreateEventCancelledEmailBody(string eventName)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                .header {{ background-color: #dc3545; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                .content {{ background-color: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }}
                .warning-box {{ background-color: #f8d7da; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #dc3545; }}
                .footer {{ text-align: center; margin-top: 30px; color: #6c757d; font-size: 14px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>❌ Sự kiện đã bị hủy</h1>
                </div>
                <div class='content'>
                    <p>Xin chào,</p>
                    <p>Chúng tôi rất tiếc phải thông báo rằng sự kiện sau đã bị hủy:</p>
                    
                    <div class='warning-box'>
                        <h3>{eventName}</h3>
                    </div>
                    
                    <p><strong>Tiền sẽ được hoàn lại:</strong><br>
                    Tiền đã thanh toán cho sự kiện này sẽ được hoàn lại vào ví của bạn trong vòng 3-5 ngày làm việc.</p>
                    
                    <p>Chúng tôi xin lỗi vì sự bất tiện này. Cảm ơn bạn đã thông cảm!</p>
                </div>
                <div class='footer'>
                    <p>© 2024 TheGrind5 Event Management. Tất cả quyền được bảo lưu.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    private string CreateOrderConfirmationEmailBody(int orderId, decimal amount)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                .header {{ background-color: #17a2b8; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                .content {{ background-color: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }}
                .info-box {{ background-color: white; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #17a2b8; }}
                .footer {{ text-align: center; margin-top: 30px; color: #6c757d; font-size: 14px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>✓ Xác nhận đơn hàng</h1>
                </div>
                <div class='content'>
                    <p>Xin chào,</p>
                    <p>Cảm ơn bạn đã đặt hàng! Đơn hàng của bạn đã được xác nhận:</p>
                    
                    <div class='info-box'>
                        <h3>Mã đơn hàng: #{orderId}</h3>
                        <p><strong>Tổng giá trị:</strong> {amount:N0} VND</p>
                    </div>
                    
                    <p>Đơn hàng của bạn sẽ được xử lý trong thời gian sớm nhất. Bạn sẽ nhận được email khi đơn hàng được thanh toán thành công.</p>
                </div>
                <div class='footer'>
                    <p>© 2024 TheGrind5 Event Management. Tất cả quyền được bảo lưu.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    private string CreatePaymentSuccessEmailBody(int orderId, decimal amount)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                .header {{ background-color: #28a745; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                .content {{ background-color: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }}
                .success-box {{ background-color: #d4edda; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #28a745; }}
                .footer {{ text-align: center; margin-top: 30px; color: #6c757d; font-size: 14px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>✓ Thanh toán thành công</h1>
                </div>
                <div class='content'>
                    <p>Xin chào,</p>
                    <p>Chúc mừng! Giao dịch thanh toán của bạn đã được thực hiện thành công:</p>
                    
                    <div class='success-box'>
                        <h3>Mã đơn hàng: #{orderId}</h3>
                        <p><strong>Số tiền:</strong> {amount:N0} VND</p>
                    </div>
                    
                    <p>Vé của bạn đã được kích hoạt. Hãy kiểm tra email để xem chi tiết vé hoặc đăng nhập vào tài khoản để xem vé của bạn.</p>
                </div>
                <div class='footer'>
                    <p>© 2024 TheGrind5 Event Management. Tất cả quyền được bảo lưu.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    private string CreateRefundEmailBody(int orderId, decimal refundAmount)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                .header {{ background-color: #6f42c1; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                .content {{ background-color: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }}
                .refund-box {{ background-color: #e7d9ff; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #6f42c1; }}
                .footer {{ text-align: center; margin-top: 30px; color: #6c757d; font-size: 14px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>💰 Hoàn tiền đã được xử lý</h1>
                </div>
                <div class='content'>
                    <p>Xin chào,</p>
                    <p>Tiền hoàn trả của bạn đã được xử lý:</p>
                    
                    <div class='refund-box'>
                        <h3>Mã đơn hàng: #{orderId}</h3>
                        <p><strong>Số tiền hoàn trả:</strong> {refundAmount:N0} VND</p>
                    </div>
                    
                    <p>Tiền đã được chuyển vào ví của bạn. Bạn có thể kiểm tra số dư ví hoặc rút tiền ra bất cứ lúc nào.</p>
                </div>
                <div class='footer'>
                    <p>© 2024 TheGrind5 Event Management. Tất cả quyền được bảo lưu.</p>
                </div>
            </div>
        </body>
        </html>";
    }
}
