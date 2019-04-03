using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.AspNetCore.Rewrite.Internal.UrlActions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;

namespace Interactive_Internship_Application.Global
{
    public class EmailsGenerated
    {


        //email function
        public void EmployerToProfessorEmail(string host, string port, string username, string password)
        {
            //SmtpClient smtpClient = ConfigureEmailServices();

            try
            {
                SmtpClient smtpClient = new SmtpClient
                {
                    Host = host,
                    Port = Convert.ToInt32(port),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(username, password)
                };
                using (var message = new MailMessage(username, "brandonadill1@gmail.com")
                {
                    Subject = "Fill out dat info boi",
                    Body = "fill it out"
                })
                {
                    smtpClient.Send(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

}

