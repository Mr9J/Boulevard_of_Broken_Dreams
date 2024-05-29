using BoulevardOfBrokenDreams.Interface;
using System.Net;
using System.Net.Mail;

namespace BoulevardOfBrokenDreams.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "siechengye@gmail.com";
            var password = "okqyobakvubpxewc";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(mail, password),
                EnableSsl = true,
            };

            return client.SendMailAsync(
                new MailMessage(from: "no_reply@mumumsit158.com", to: email, subject, message));
        }
    }
}
