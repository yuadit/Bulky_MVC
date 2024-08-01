using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendWithBrevo;

namespace Bulky.Utility;

public class EmailSender : IEmailSender
{
    public string BravoSecret { get; set; }

    public EmailSender(IConfiguration _config)
    {
        BravoSecret = _config.GetValue<string>("Bravo:SecretKey");
    }
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new BrevoClient(BravoSecret);
        var newmail = client.SendAsync(
            new Sender("Bulk Book", "hello@dotnetmastery.com"),
            new List<Recipient> { new("Dear Customer", email) },
            subject,
            htmlMessage // true if body is HTML
        );

        return newmail;
    }
}