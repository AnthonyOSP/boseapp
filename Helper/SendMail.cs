using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Net.Mail;
using DotNetEnv;

namespace boseapp.Helper
{
    public class SendMail
    {
        private readonly string smtpServer = "smtp-relay.brevo.com";
        private readonly int smtpPort = 587;
        private readonly string smtpUser = "7d3afd002@smtp-brevo.com";
        private readonly string? smtpPass;
        private readonly string senderEmail = "anthonysaa93@gmail.com"; // Reemplaza con un remitente validado
        private readonly string senderName = "Anthony Saa";

        public SendMail()
        {
            Env.Load();
            smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS");
            Console.WriteLine($"SMTP_PASS loaded: {smtpPass}");
        }

        public async Task EnviarCorreoAsync(string emailTo, string subject, string body)
        {
            var smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(emailTo);
            mailMessage.Headers.Add("Reply-To", senderEmail);
            mailMessage.Headers.Add("Return-Path", senderEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine("Correo enviado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando correo: {ex.Message}");
            }
        }
    }
}