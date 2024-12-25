using EComercial.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using EComercial.Core;
using Microsoft.EntityFrameworkCore;
using EComercial.DbDataContext;
using System.Net;

namespace EComercial.Services
{
    internal class EmailService
    {
        private readonly DataContext _context = new DataContext();


        // send email for new registered user 
        public static void SendEmail(string recipientEmail, string subject, string body)
        {
            string senderEmail = "davitimnadze98@gmail.com";
            string appPassword =                                                                                                         "worb shar amhq kjup"; 

            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true 
                };

                mail.To.Add(recipientEmail);
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(senderEmail, appPassword),
                    EnableSsl = true
                };
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        // send email with activation code for user which trying to login and still not actived
        public void SendActivationCode(Customer user , string activationCode)
        {
            user.ActivationCode = activationCode;
            user.CodeExpirationTime = DateTime.UtcNow.AddMinutes(5);
            user.IsActive = false;

            _context.customers.Update(user);
            _context.SaveChanges();

            SendEmail(
                user.Email,
                "Activation Code",
                $"Hello {user.Name},<br><br>Your activation code is <strong>{activationCode}</strong>. It is valid for 5 minutes.<br><br>Thank you!"
            );
        }

    }
}
