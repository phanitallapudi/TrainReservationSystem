using MailKit.Net.Smtp;
using MimeKit;

namespace TrainReservationSystem.Services
{
    public class EmailService
    {
        public void SendEmail(string body, string emailAddress)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("werner9@ethereal.email"));
            email.To.Add(MailboxAddress.Parse(emailAddress));

            email.Subject = "Train ticket confirmation";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate("werner9@ethereal.email", "pmrU3VwJS5BVZ5bhTd");
            smtp.Send(email);
            smtp.Disconnect(true);

        }
    }
}
