using BoulevardOfBrokenDreams.Interface;
using System.Net;
using System.Net.Mail;

namespace BoulevardOfBrokenDreams.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "mumufundraising@gmail.com";
            var password = "qnikxflaizlgdowb";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(mail, password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage(from: "noreply@mumumsit158.com", to: email, subject, message);
            mailMessage.IsBodyHtml = true;

            return client.SendMailAsync(mailMessage);
        }
    }
}
