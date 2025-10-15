using System.Net;
using System.Net.Mail;
using TheGrind5_EventManagement.Business;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TheGrind5_EventManagement.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendOtpEmailAsync(string toEmail, string otpCode)
    {
        try
        {
            var smtpHost = "smtp.gmail.com";
            var smtpPort = 587;
            var senderEmail = "minhkp2pro@gmail.com";
            var senderPassword = "pzyf gjdd mlca rmar"; // App Password

            using var client = new SmtpClient(smtpHost, smtpPort);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(senderEmail, senderPassword);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, "TheGrind5 Event Management"),
                Subject = "Mã OTP để đặt lại mật khẩu",
                Body = CreateOtpEmailBody(otpCode),
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
            
            _logger.LogInformation($"OTP email sent successfully to {toEmail}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send OTP email to {toEmail}");
            return false;
        }
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
}
