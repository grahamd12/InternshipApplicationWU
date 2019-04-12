using System;
using System.Collections.Generic;
using System.Linq;
using Interactive_Internship_Application.Global;
using Microsoft.AspNetCore.Mvc;
using Interactive_Internship_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace Interactive_Internship_Application.Controllers
{
    // [Authorize(Roles = "Admin, Employer")]
    static class EmployerEmail
    {
        public static string employerEmail;
    }
    public class EmployerController : Controller
    {
        ApplicationDbContext context = new Models.ApplicationDbContext();
        IConfiguration configuration;
       

        //create this to have a local variable to manipulate the database
        //below takes in the database (the data from the view ) and puts it local for this
        //controller to decide what to do to the data. 
        public Models.ApplicationDbContext applicationDbContext { get; set; }
        public EmployerController(Models.ApplicationDbContext dbContext, IConfiguration iconfiguration)
        {
            applicationDbContext = dbContext;
            configuration = iconfiguration;

        }

        //look in database to see count of entries for Employer entity 

        public IActionResult Index()
        {
            return View();
        }

        //login credentials to look at the Employer_Login table. This is out of scope, so identity cannot be used

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login()
        {
            var dictionary = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            var pin = dictionary["pass"];
            var username = dictionary["email"];
            EmployerEmail.employerEmail = username.ToString();
            var intPin = Convert.ToInt16(pin);
            var currentEmployerPin = (from employer in context.EmployerLogin
                                      where employer.Email == username
                                      select employer.Pin).FirstOrDefault();
            if (currentEmployerPin == intPin)
            {
                var dateTime = DateTime.Now;
                var lastTimeLoggedIn = (from employer in context.EmployerLogin
                                        where employer.Email == username
                                        select employer.LastLogin).First();
                TimeSpan differenceOfTime = dateTime - lastTimeLoggedIn;

                double secondsSinceLastLoggedIn = differenceOfTime.TotalSeconds;

                if (secondsSinceLastLoggedIn > 10000)
                {
                    //put error to user here to tell them to log in with the newly generated pin. 

                    //generate the new pin for the user
                    //generate random number pin (4 digits) for employer to add back to database. 
                    Random rnd = new Random();
                    short newPin = Convert.ToInt16(rnd.Next(0000, 9999));

                    //grab the employers current row and save the newly generated pin here. 
                    var employerLoginRow = (from employer in context.EmployerLogin
                                            where employer.Email == username
                                            select employer).First();
                    employerLoginRow.Pin = newPin;
                    employerLoginRow.LastLogin = DateTime.Now;
              //      context.EmployerLogin.d(employerLoginRow);
                    context.SaveChanges();

                    //call the method that sends the email to the employer with the new pin information 

                    //grab email credentials to pass into the email method
                    string emailHost = configuration["Email:Smtp:Host"];
                    string emailPort = configuration["Email:Smtp:Port"];
                    string emailUsername = configuration["Email:Smtp:Username"];
                    string emailPassword = configuration["Email:Smtp:Password"];

                    Global.EmailsGenerated emailsGenerated = new EmailsGenerated();
                    emailsGenerated.EmployerRegeneratePinEmail(emailHost, emailPort, emailUsername, emailPassword, username, newPin);

                    return LocalRedirect("/Global/ErrorRegeneratePin");
                }
                else
                {
                    return LocalRedirect("~/Employer/CompanyInformation");

                }

            }
            else
            {
                //for now redirect to the regenerate pin page just to show we are doing something. Need to figure out error handling
                return LocalRedirect("/Global/ErrorRegeneratePin");
            }
        }

        [HttpGet]
        public IActionResult CompanyInformation()
        {
            //returns all entries in the application template table
            using (var context = new Models.ApplicationDbContext())
            {
                var getCompanyInfo = context.ApplicationTemplate.ToList();

                var companyInfoRightOrder =
                    from e in getCompanyInfo.AsQueryable<ApplicationTemplate>()
                    orderby e.Id
                    select e;
                ViewBag.companyInfo = companyInfoRightOrder;

                return View(context.ApplicationTemplate.ToList());
            }

        }
        //Below takes the employer's submitted data and puts it into the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submitted(IEnumerable<Interactive_Internship_Application.Models.ApplicationTemplate> ApplicationTemplateModel)
        {

            int count = 0;
            int numEmployerFieldCount = (from x in context.ApplicationTemplate
                                         where x.Entity == "Employer"
                                         select x).Count();

            //below gets the student's ID that the employer is tied to for input in to application 


            var employerCorrelationToStudentEmail = (from employer in context.EmployerLogin
                                         where employer.Email == EmployerEmail.employerEmail
                                         select employer.StudentEmail).FirstOrDefault();

            var employersStudentEmailToStudentInformation = (from student in context.StudentInformation
                                       where student.Email == employerCorrelationToStudentEmail.ToString()
                                       select student.Email).FirstOrDefault();

            var currentEmployerId = (from employer in context.EmployerLogin
                                    where employer.Email == EmployerEmail.employerEmail
                                    && employer.StudentEmail == employersStudentEmailToStudentInformation
                                    select employer.Id).FirstOrDefault();

            var studentUniqueRecordNum = (from studentUniqueNum in context.StudentAppNum
                                          where studentUniqueNum.StudentEmail == employersStudentEmailToStudentInformation
                                          && studentUniqueNum.EmployerId == currentEmployerId
                                          select studentUniqueNum.Id).FirstOrDefault();
        


            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            foreach (var item in dict)
            {

                if (count < numEmployerFieldCount)
                {
                    int intKey = Int32.Parse(item.Key.ToString());

                    //changed the recordId to not be a foreign key on StudentInformation just to see if it was working. 
                    //Change AppData DB back the right way later
                    //Had to take out the FK's of the AppData table to make it work too
                    var appDataCurrent = new ApplicationData { RecordId = studentUniqueRecordNum, DataKeyId = intKey, Value = item.Value };
                    applicationDbContext.ApplicationData.Add(appDataCurrent);
                    applicationDbContext.SaveChanges();
                    count++;
                }
            }


            //below puts the appsettings into local variables to be passed into the EmailsGenerated.cs file for email deployment
            try
            {
                //below gets the current class the student is taking so an email can be sent to that respective professor 
                var classEnrolled = (from appData in context.ApplicationData
                                              where appData.RecordId  == studentUniqueRecordNum &&
                                              appData.DataKeyId == 1
                                              select appData.Value).FirstOrDefault().ToString();

                        var studentName = (from appData in context.ApplicationData
                                               where appData.RecordId == studentUniqueRecordNum 
                                               && appData.DataKeyId == 3
                                               select appData.Value).FirstOrDefault();

                var employerCompanyName = (from appData in context.ApplicationData
                                   where appData.RecordId == studentUniqueRecordNum
                                   && appData.DataKeyId == 11
                                   select appData.Value).FirstOrDefault();

                //below gets the professor's email for the student 

                var professorEmail = (from facultyData in context.FacultyInformation
                                     where facultyData.CourseName == classEnrolled
                                     select facultyData.ProfEmail).FirstOrDefault();

                string emailHost = configuration["Email:Smtp:Host"];
                string emailPort = configuration["Email:Smtp:Port"];
                string emailUsername = configuration["Email:Smtp:Username"];
                string emailPassword = configuration["Email:Smtp:Password"];

                Global.EmailsGenerated emailsGenerated = new EmailsGenerated();
                emailsGenerated.EmployerToProfessorEmail(emailHost, emailPort, emailUsername, emailPassword, studentName, professorEmail, employerCompanyName, classEnrolled);


            }
            catch
            {
                
                return View();
            }
            return View("~/Views/Employer/ThankYouLogout.cshtml");
        }

        

        public IActionResult ThankYouLogout()
        {
            return View();
        }

        public IActionResult ClickToEmployerForgotLogin()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotLogin(string empEmail)
        {
            try

            {
                //checks to see on the forgot login page if the employer has entered a valid email in the database
                var checkEmailExists = context.EmployerLogin.Where(u => u.Email == empEmail).Select(x => x.Email).FirstOrDefault();

                //if the email doesn't exist, throw an error
                if (checkEmailExists == null)
                {
                    //need to error check here just returning to the login page
                    return LocalRedirect("~/Employer");
                }

                //employer's email did exist
                else
                {
                    Random rnd = new Random();
                    short newPin = Convert.ToInt16(rnd.Next(0000, 9999));
                    var employerEmail = EmployerEmail.employerEmail;
                    //grab the employers current row and save the newly generated pin here. 
                    var employerLoginRow = (from employer in context.EmployerLogin
                                            where employer.Email == EmployerEmail.employerEmail
                                            select employer).First();
                    employerLoginRow.Pin = newPin;
                    employerLoginRow.LastLogin = DateTime.Now;
                    //      context.EmployerLogin.d(employerLoginRow);
                    context.SaveChanges();
                    string emailHost = configuration["Email:Smtp:Host"];
                    string emailPort = configuration["Email:Smtp:Port"];
                    string emailUsername = configuration["Email:Smtp:Username"];
                    string emailPassword = configuration["Email:Smtp:Password"];

                    Global.EmailsGenerated emailsGenerated = new EmailsGenerated();
                    emailsGenerated.EmployerForgotPin(emailHost, emailPort, emailUsername, emailPassword, employerEmail, newPin);

                    return LocalRedirect("~/Employer");
                }
               
            }
            catch
            {
                return View();
            }


        }

    }

}
