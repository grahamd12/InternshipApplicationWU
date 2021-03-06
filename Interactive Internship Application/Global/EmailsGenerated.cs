using System;
using System.Net.Mail;

namespace Interactive_Internship_Application.Global
{
    public class EmailsGenerated
    {
        public EmailsGenerated()
        {
        }
        //function to send employer email after student has completed their part
        public void StudentToEmployerEmail(string host, string port, string username, string password, string studentName, string empEmail, string companyName, short pin, string course)
        {
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
                using (var message = new MailMessage(username, empEmail)
                {

                    Subject = "Winthrop University College of Business Administration: Internship Application",
                    Body = "To whom it may concern, \n \n" +
                    "" +
                    "Your intern " + studentName + " is applying to take " + course +
                    ", which would allow " + studentName + " to get class credit for the internship " +
                    studentName + " will be " +
                    "completing for you at your company, " + companyName +
                    " and needs your approval for this internship to be taken for class credit." +
                    "Please complete the form at this link: ." + "\n \n" +
                    "To login, use your email address and this four digit pin: " + pin + ".\n \n" + "If you have any questions, please don't hesitate to reach out to Mrs. Celeste Tiller " +
                    "at tillerc@winthrop.edu \n\n"
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

        //email function that takes place after the employer completes their part of the application
        public void EmployerToProfessorEmail(string host, string port, string username, string password, string studentName, string profEmail, string companyName, string course)
        {
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
                    Body = "Hello Professor, " + "\n \n" +
                    "" +
                    "Your student " + studentName + " is applying to take " + course + ". "
                    + studentName + " will be interning at " + companyName + " and needs your approval for this job to take place. Please review the information " +
                    "that has been listed and approve/deny the request at here."
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

        //this email is sent when the employer's pin has expired and they need to be sent an email with the new pin.
        public void EmployerRegeneratePinEmail(string host, string port, string username, string password, string empEmail, short pin)
        {
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
                using (var message = new MailMessage(username, empEmail)
                {

                    Subject = "Winthrop University College of Business Administration: Internship Application",
                    Body = "To whom it may concern, \n \n" +
                    "" +
                    "Your pin for the internship application has expired. Your new pin is " +pin+ " . Please use this pin and your email to log back into the system. This pin"
                    + " expires in 48 hours. Thank you."
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

        //this email is sent when the employer forgets their pin upon clicking at the login page
        public void EmployerForgotPin(string host, string port, string username, string password, string empEmail, short pin)
        {
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
                using (var message = new MailMessage(username, empEmail)
                {

                    Subject = "Winthrop University College of Business Administration: Internship Application",
                    Body = "To whom it may concern, \n \n" +
                    "" +
                    "You have requested a new pin. Your new pin is " + pin + " . Please use this pin and your email to log back into the system. This pin"
                    + " expires in 48 hours. Thank you."
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

