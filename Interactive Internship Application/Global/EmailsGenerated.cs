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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Diagnostics;
using Interactive_Internship_Application.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Interactive_Internship_Application.Global
{
    public class EmailsGenerated
    {
        public EmailsGenerated()
        {
        }

        //email function
        public void EmployerToProfessorEmail(string host, string port, string username, string password, string studentName, string profEmail, string companyName, string course)
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
                using (var message = new MailMessage(username, profEmail)
                {
                   
                Subject = "Winthrop University College of Business Administration: Internship Application",
                    Body = "Hello Professor, " +
                    "" +
                    "Your student " + studentName + " is applying to take " + course + ". " 
                    + studentName + " will be interning at " + companyName + " and needs your approval for this job to take place."
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

