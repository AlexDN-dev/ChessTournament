using BLL.Interfaces;
using BLL.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BLL.Service;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }
    
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var mail = new MimeMessage();
        
        mail.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        mail.To.Add(MailboxAddress.Parse(toEmail));
        mail.Subject = subject;
        mail.Body = new TextPart("html") { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
        await smtp.SendAsync(mail);
        await smtp.DisconnectAsync(true);
    }
}