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

            
          

            //determine if new StudentAppNum needs to be created
            if (eName == "createNew")
            {
             /*   using (var context = new Models.ApplicationDbContext())
                {

                    var studentsEmail = (from student in context.StudentInformation
                                         where student.Email == User.Identity.Name.ToString()
                                         select student.Email).FirstOrDefault();

                    string employerID = "3";
                    var status = "Incomplete";

                    List<string> newAppNum = new List<string>();
                    newAppNum.Add(studentsEmail);
                    newAppNum.Add(employerID);
                    newAppNum.Add(status);


                    // Add the new object to the Orders collection.
                    context.SaveChanges(newAppNum);                  

                }

            */
            }

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
    

        //using a separate controller method with the same name as the default controller 
        //method but contains parameters
        /*After user submits form, this method stores form data into database*/
        /*[HttpPost]
        public IActionResult ApplicationDynamic(string name)
          {
            Console.WriteLine("Submitted ", name);
            return View("Index");
        }
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submitted(IEnumerable<Interactive_Internship_Application.Models.ApplicationTemplate> ApplicationTemplateModel)
        {

            //PUT CODE TO SEND EMAIL AFTER STUDENT'S INFO IS SAVED TO DB; HERE FOR NOW FOR TESTING
            //send email to employer

            int count = 0;
            var context = new ApplicationDbContext();
            int numStudentFieldCount = (from x in context.ApplicationTemplate
                                         where x.Entity == "Student"
                                         select x).Count();
            

            //below gets the student's record id using queries

            var studentsEmail = (from student in context.StudentInformation
                                 where student.Email == User.Identity.Name.ToString()
                                 select student.Email).FirstOrDefault();

            var currStudentRecordId = (from stuAppNum in context.StudentAppNum
                                       where stuAppNum.StudentEmail == studentsEmail
                                       select stuAppNum.Id).FirstOrDefault();

            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            foreach (var item in dict)
            {

                if (count < (numStudentFieldCount-3)) //don't count submitted button response
                                                      //in field count
                {
                    int intKey = Int32.Parse(item.Key.ToString());

                    //changed the recordId to not be a foreign key on StudentInformation just to see if it was working. 
                    //Change AppData DB back the right way later
                    //Had to take out the FK's of the AppData table to make it work too


                    var appDataCurrent = new ApplicationData { RecordId = currStudentRecordId, DataKeyId = intKey, Value = item.Value };
                    _dataContext.ApplicationData.Add(appDataCurrent);
                    _dataContext.SaveChanges();
                    count++;
                }
            }

            //connect student's email with employer ID; inside employer Login, grab employer's email
            int employerID = (from application in context.StudentAppNum
                              where application.StudentEmail == studentsEmail
                              select application.EmployerId).FirstOrDefault();

            var employerEmail = (from appData in context.ApplicationData
                                 where appData.RecordId == currStudentRecordId &&
                                 appData.DataKeyId == 17
                                 select appData.Value).FirstOrDefault();


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


            //generate random number pin (4 digits) for employer
            Random rnd = new Random();
            int pin = rnd.Next(0000, 9999);

            string emailHost = configuration["Email:Smtp:Host"];
            string emailPort = configuration["Email:Smtp:Port"];
            string emailUsername = configuration["Email:Smtp:Username"];
            string emailPassword = configuration["Email:Smtp:Password"];


            Global.EmailsGenerated emailsGenerated = new EmailsGenerated();
            emailsGenerated.StudentToEmployerEmail(emailHost, emailPort, emailUsername, emailPassword, studentName, employerEmail, employerName, employerTitle, employerCompanyName, classEnrolled, pin);
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
