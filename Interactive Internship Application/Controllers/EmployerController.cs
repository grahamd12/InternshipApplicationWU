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
    [Authorize(Roles = "Admin, Employer")]
    public class EmployerController : Controller
    {
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
            return View();


        }
        //Below takes the employer's submitted data and puts it into the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submitted(IEnumerable<Interactive_Internship_Application.Models.ApplicationTemplate> ApplicationTemplateModel)
        {

            int count = 0;
            ApplicationDbContext context = new Models.ApplicationDbContext();
            int numEmployerFieldCount = (from x in context.ApplicationTemplate
                                         where x.Entity == "Employer"
                                         select x).Count();

            //below gets the student's ID that the employer is tied to for input in to application 


            var employerCorrelationToStudentEmail = (from employer in context.EmployerLogin
                                         where employer.Email == User.Identity.Name.ToString()
                                         select employer.StudentEmail).FirstOrDefault();

            var employersStudentEmailToStudentInformation = (from student in context.StudentInformation
                                       where student.Email == employerCorrelationToStudentEmail.ToString()
                                       select student.Email).FirstOrDefault();

            var currentEmployerId = (from employer in context.EmployerLogin
                                    where employer.Email == User.Identity.Name.ToString()
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

            return View();

        }

    }

}
