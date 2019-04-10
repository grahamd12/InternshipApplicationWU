using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Interactive_Internship_Application.Global;
using Interactive_Internship_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Interactive_Internship_Application.Controllers
{

    //struct used for saving a new employer login row to Employer_Login table
  /*  public struct empLogin
    {
        public int empPin;
        public string empEmail, studEmail;

    }

        */

    [Authorize(Roles = "Admin,Student")]

    public class StudentDynController : Controller
    {
        public Models.ApplicationDbContext _dataContext { get; set; }
        IConfiguration configuration;
        public StudentDynController(Models.ApplicationDbContext dataContext, IConfiguration iconfiguration)
        {
            _dataContext = dataContext;
            configuration = iconfiguration;

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewApplications()
        {
            var context = new Models.ApplicationDbContext();

            //get current user's email and ID
            var studentsEmail = (from student in context.StudentInformation
                                 where student.Email == User.Identity.Name.ToString()
                                 select student.Email).FirstOrDefault();

            //check inside Student App Num if that ID exists
            var currStudentRecordId = (from stuAppNum in context.StudentAppNum
                                       where stuAppNum.StudentEmail == studentsEmail
                                       select stuAppNum.Id).ToList();

            //if yes, grab class descriptor(s)
            List<string> classNames = new List<string>();

            if (currStudentRecordId.Count > 0)
            {

                foreach (int id in currStudentRecordId)
                {


                    var classNameCurr = (from appData in context.ApplicationData
                                         where appData.RecordId == id
                                         where appData.DataKeyId == 1
                                         select appData.Value).First().ToString();
                    classNames.Add(classNameCurr);


                }

            }
            //no else needed

            //show create new application

            ViewBag.classNames = classNames;

            return View();
        }

        /*Displays current application template fields onto web page  */
        public IActionResult Application(string eName)
        {
            var entity = new entityInfo();
            entity.entityName = eName;

            //returns all entries in the application template table
            //and data filled out by student to date
            using (var context = new Interactive_Internship_Application.Models.ApplicationDbContext())
            {
                var getSingleFieldName = context.ApplicationTemplate.ToList();
                var filledOutGood = new Dictionary<int, string>();
                //if previously saved application, grab data that was saved
                if (eName != "createNew")
                {
                    //get current user's email and ID for class
                    var studentEmail = (from student in context.StudentInformation
                                        where student.Email == User.Identity.Name.ToString()
                                        select student.Email).FirstOrDefault();

                    //get all application IDs for student 
                    var currStudentRecordId = (from stuAppNum in context.StudentAppNum
                                               where stuAppNum.StudentEmail == studentEmail
                                               select stuAppNum.Id).ToList();

                    //get applicationID for particular class
                    int correctID = 0;
                    foreach (var currID in currStudentRecordId)
                    {
                        var classNameCurr = (from appData in context.ApplicationData
                                             where appData.RecordId == currID
                                             where appData.DataKeyId == 1
                                             select appData.Value).First().ToString();

                        if (classNameCurr == eName)
                            correctID = currID;
                    }

                    //get applicationData for particular class's application
                    var filledOut = from appData in context.ApplicationData
                                     where appData.RecordId == correctID
                                     select appData;


                     filledOutGood = filledOut.ToDictionary(what => what.DataKeyId, who=>who.Value);
                    ViewBag.appID = correctID;
                }
                
                ViewBag.className = eName;
                ViewBag.fieldNames = getSingleFieldName;

                return View(filledOutGood);

            }
        }
    

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submitted(IEnumerable<Interactive_Internship_Application.Models.ApplicationTemplate> ApplicationTemplateModel, string response)
        {

            string result = response;  //used to determine if a new Student App Num needs to be created
            var context = new ApplicationDbContext();
    
            //below gets the student's email using queries
            var studentsEmail = (from student in context.StudentInformation
                                 where student.Email == User.Identity.Name.ToString()
                                 select student.Email).FirstOrDefault();

     
            //save submitted information into a dictionary
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());

            //get employer email from submitted information (will need to be used whether or 
            //not a new application is created
            var employerEmail = dict["17"];

            //determine if new StudentAppNum needs to be created
            if (response == "Submit New Application" || response == "Save New Application")
            {

                //generate random number pin (4 digits) for employer
                Random rnd = new Random();
                int pin = rnd.Next(0000, 9999);

                //save employer email, pin, and student email to Employer Login Table
                EmployerLogin newEmployerLogin = new EmployerLogin();
                newEmployerLogin.StudentEmail = studentsEmail;
                newEmployerLogin.Email = employerEmail;
                newEmployerLogin.Pin = Convert.ToInt16(pin);

                context.EmployerLogin.Add(newEmployerLogin);
                context.SaveChanges();

                //save student email, employer ID, and application status to Student App Num Table
                StudentAppNum newApp = new StudentAppNum();
                newApp.StudentEmail = studentsEmail;
                newApp.EmployerId = (from application in context.EmployerLogin
                                     where application.StudentEmail == studentsEmail &&
                                     application.Email == employerEmail
                                     select application.Id).FirstOrDefault(); 

                /* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                 * NEED TO CHANGE THIS IN THE CASE THEY SUBMIT IT ALL IN ONE GO
                   !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!   
                 * */
                newApp.Status = "Incomplete";

                context.StudentAppNum.Add(newApp);
                context.SaveChanges();
                
            }

            //get students record ID 
            var currStudentRecordId = (from stuAppNum in context.StudentAppNum
                                       where stuAppNum.StudentEmail == studentsEmail
                                       select stuAppNum.Id).FirstOrDefault();

            //get number of fields student enters
            int numStudentFieldCount = (from x in context.ApplicationTemplate
                                        where x.Entity == "Student"
                                        select x).Count();

            int count = 0; //used to determine if all input data has been saved

            //determine if student wants to submit or save application

            //if saving application, doesn't matter if everything was input; just make sure 
              //class enrolled and employer email is input
            if(response.Contains("Save"))
            {

                //saves students input information to database
                foreach (var item in dict)
                {

                    if (count < (numStudentFieldCount - 3)) //don't count submitted button response
                                                            //in field count
                    {
                        int intKey = Int32.Parse(item.Key.ToString());
                        var appDataCurrent = new ApplicationData { RecordId = currStudentRecordId, DataKeyId = intKey, Value = item.Value };

                        //determine if recordID and dataKeyID already exist in ApplicationData
                        var prevSaved = (from record in _dataContext.ApplicationData
                                      where record.DataKeyId == intKey &&
                                      record.RecordId == currStudentRecordId
                                      select record).SingleOrDefault();

                        if (prevSaved != null) //record was previously saved
                        {
                            prevSaved.Value = appDataCurrent.Value;
                        }
                        else //record was not previously saved
                        {
                            _dataContext.ApplicationData.Add(appDataCurrent);
                        }

                        /*
                         CHECK IF THAT RECORDID AND DATAKEYID PAIR HAVE ALREADY BEEN ENTERED
                         IF YES, DELETE THAT ROW

                          NO ELSE


                          INSERT/REINSERT ROW

                        ALTER TABLE IN SQL
                        MODIFY IN LINQ?


                         */


                        _dataContext.SaveChanges();
                        count++;
                    }
                }

            }


            //if submitting application, ensure everything is saved in database


            //get employer's name
            var employerName = (from appData in context.ApplicationData
                                where appData.RecordId == currStudentRecordId
                                && appData.DataKeyId == 14
                                select appData.Value).FirstOrDefault();

            //get employer's title
            var employerTitle = (from appData in context.ApplicationData
                                 where appData.RecordId == currStudentRecordId
                                 && appData.DataKeyId == 15
                                 select appData.Value).FirstOrDefault();

            //get employer's pin
            var empPin = (from empData in context.EmployerLogin
                          where empData.Email == employerEmail
                          select empData.Pin).FirstOrDefault();

            short employerPin = Convert.ToInt16(empPin);

            //get employer's Company name
            var employerCompanyName = (from appData in context.ApplicationData
                                       where appData.RecordId == currStudentRecordId
                                       && appData.DataKeyId == 11
                                       select appData.Value).FirstOrDefault();

            //get student's name
            var studentName = (from appData in context.ApplicationData
                               where appData.RecordId == currStudentRecordId
                               && appData.DataKeyId == 3
                               select appData.Value).FirstOrDefault();

            //get class student is trying to enroll in
            var classEnrolled = (from appData in context.ApplicationData
                                 where appData.RecordId == currStudentRecordId &&
                                 appData.DataKeyId == 1
                                 select appData.Value).FirstOrDefault().ToString();


           

            string emailHost = configuration["Email:Smtp:Host"];
            string emailPort = configuration["Email:Smtp:Port"];
            string emailUsername = configuration["Email:Smtp:Username"];
            string emailPassword = configuration["Email:Smtp:Password"];


            Global.EmailsGenerated emailsGenerated = new EmailsGenerated();
            emailsGenerated.StudentToEmployerEmail(emailHost, emailPort, emailUsername, emailPassword, studentName, employerEmail, employerName, employerTitle, employerCompanyName,employerPin,classEnrolled);
            return View("Index");

        }



        public IActionResult CheckStatus()

        {
            return View();
        }

        public IActionResult SignJobDescription()
        {
            return View();
        }
    }
}

/*InvalidOperationException: The view 'Index' was not found.The following locations were searched:
/Views/Student/Index.cshtml
/Views/Shared/Index.cshtml
/Pages/Shared/Index.cshtml */
