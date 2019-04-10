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
    public void StudentToEmployerEmail(string host, string port, string username, string password, string studentName, string empEmail, string empName, string empTitle, string companyName,short pin, string course)
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
            using (var message = new MailMessage(username, empEmail)
            {

                Subject = "Winthrop University College of Business Administration: Internship Application",
                Body = "Hello " + empTitle + " " + empName + "\n \n" +
                "" +
                "Your intern " + studentName + " is applying to take " + course +
                ", which would allow " + studentName + " to get class credit for the internship " +
                studentName + " will be " +
                "completing for you at your company, " + companyName +
                " and needs your approval for this internship to be taken for class credit." +
                "Please complete the form at this link: ." + "\n \n"+
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
    }

}

