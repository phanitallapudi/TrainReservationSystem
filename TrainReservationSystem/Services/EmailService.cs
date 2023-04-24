using System.Net.Mail;

namespace TrainReservationSystem.Services
{
    public class EmailService
    {
        public void SendEmail(string body, string emailAddress)
        {
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "testerrone5@gmail.com";
            string smtpPassword = "zaqrrpqsyvczpgpy";

            MailMessage message = new MailMessage();
            message.From = new MailAddress("testerrone5@gmail.com");
            message.To.Add(emailAddress);
            message.Subject = "Train Booking Notification";
            message.Body = body;
			;

			SmtpClient client = new SmtpClient(smtpServer, smtpPort);
            client.Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
            client.EnableSsl = true;

            client.Send(message);
        }
    }
}
