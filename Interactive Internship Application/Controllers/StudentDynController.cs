using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Interactive_Internship_Application.Global;
using Interactive_Internship_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

            //if it exists, grab class descriptor(s)
            List<string> classNames = new List<string>();
            Dictionary<string, string> classStatus = new Dictionary<string, string>();

            if (currStudentRecordId.Count > 0)
            {

                foreach (int id in currStudentRecordId)
                {


                    var classNameCurr = (from appData in context.ApplicationData
                                         where appData.RecordId == id
                                         where appData.DataKeyId == 1
                                         select appData.Value).First().ToString();
                    classNames.Add(classNameCurr);


                    var classStatusCurr = (from appData in context.StudentAppNum
                                           where appData.Id == id
                                           select appData.Status).First().ToString();

                    classStatus.Add(classNameCurr, classStatusCurr);

                }

            }
            //no else needed

            ViewBag.classNames = classNames;
            ViewBag.classStatus = classStatus;

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
                var getSingleFieldName = from app in _dataContext.ApplicationTemplate
                                         where app.Deleted == false
                                         select app;
                var studentValues = from data in _dataContext.ApplicationTemplate
                                    where data.Entity == "Student"
                                    where data.FieldName.Contains("sig")
                                    || data.FieldName.Contains("date")
                                    where data.FieldDescription.Contains("guidelines")
                                    || data.FieldDescription.Contains("section")
                                    select data;

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


                    filledOutGood = filledOut.ToDictionary(what => what.DataKeyId, who => who.Value);
                    ViewBag.appID = correctID;
                }

                ViewBag.className = eName;
                ViewBag.fieldNames = getSingleFieldName;
                ViewBag.studentValues = studentValues;

                return View(filledOutGood);

            }
        }


        //called when student presses "Save" or "Submit"
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

            //class enrolled and employer email must be entered
            //show error message if not entered
            if (dict["1"].Length <= 0 || dict["17"].Length <= 0)
            {
                ViewBag.error = "Class enrolled and employer email must be entered in order to save or submit";
                return RedirectToAction("Application");
            }

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
                newEmployerLogin.LastLogin = DateTime.Now;
                context.EmployerLogin.Add(newEmployerLogin);
                context.SaveChanges();

                //save student email, employer ID, and application status to Student App Num Table
                StudentAppNum newApp = new StudentAppNum();
                newApp.StudentEmail = studentsEmail;
                newApp.EmployerId = (from application in context.EmployerLogin
                                     where application.StudentEmail == studentsEmail &&
                                     application.Email == employerEmail
                                     select application.Id).FirstOrDefault();

                newApp.Status = "Incomplete";

                context.StudentAppNum.Add(newApp);
                context.SaveChanges();

            }

            //used to determine which Student App Num ID to use
            int empId = (from application in context.EmployerLogin
                         where application.StudentEmail == studentsEmail &&
                         application.Email == employerEmail
                         select application.Id).FirstOrDefault();

            //get students record ID 
            var currStudentRecordId = (from stuAppNum in context.StudentAppNum
                                       where stuAppNum.StudentEmail == studentsEmail &&
                                       stuAppNum.EmployerId == empId
                                       select stuAppNum.Id).FirstOrDefault();

            //get number of fields student enters
            int numStudentFieldCount = (from x in context.ApplicationTemplate
                                        where x.Entity == "Student"
                                        select x).Count();



            //determine if student wants to submit or save application

            //if saving application, doesn't matter if everything was input; just make sure 
            //class enrolled and employer email is input
            if (response.Contains("Save"))
            {

                Save(dict, currStudentRecordId, numStudentFieldCount);

            }

            //if submitting application, ensure everything is entered by student
            else if (response.Contains("Submit"))
            {
                foreach (var rec in dict)
                {
                    if (rec.Value.Length <= 0)
                    {
                        return View("Application");
                    }
                }

                //if program reaches here, all data has been entered and student wants to submit application
                Save(dict, currStudentRecordId, numStudentFieldCount);

                //send email to employer

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
                emailsGenerated.StudentToEmployerEmail(emailHost, emailPort, emailUsername, emailPassword, studentName, employerEmail, employerCompanyName, employerPin, classEnrolled);

                //change student's application status to "Pending Employer Approval"       

                var changeStatusApp = new StudentAppNum { Id = currStudentRecordId,StudentEmail = studentsEmail,EmployerId =empId };
                changeStatusApp.Status = "Pending Employer Approval";
            
                context.SaveChanges();

                return View("Index");

            }
            return View("Index");
        }

        //function to save student's information to database
        public IActionResult Save(Dictionary<string, string> dict, int currId, int numFields)
        {
            int count = 0; //used to determine if all input data has been saved


            //saves students input information to database
            foreach (var item in dict)
            {

                if (count < (numFields - 3)) //don't count submitted button response
                                             //in field count
                {
                    if (item.Value.Length > 0) //only saves input information (no empty info)
                    {
                        int intKey = Int32.Parse(item.Key.ToString());
                        var appDataCurrent = new ApplicationData { RecordId = currId, DataKeyId = intKey, Value = item.Value };

                        //determine if recordID and dataKeyID already exist in ApplicationData
                        var prevSaved = (from record in _dataContext.ApplicationData
                                         where record.DataKeyId == intKey &&
                                         record.RecordId == currId
                                         select record).SingleOrDefault();

                        if (prevSaved != null) //record was previously saved
                        {
                            prevSaved.Value = appDataCurrent.Value;
                        }
                        else //record was not previously saved
                        {
                            _dataContext.ApplicationData.Add(appDataCurrent);
                        }

                        _dataContext.SaveChanges();
                    }
                    count++;
                }
            }
            return View("Index");
        }

        public IActionResult CheckStatus()
        {

            return View();
        }
    
        //allows a student to view an employer's job description and sign off on it
         public IActionResult ViewJobDescriptions()
        {

            //get current user's email and ID
            var studentsEmail = (from student in _dataContext.StudentInformation
                                 where student.Email == User.Identity.Name.ToString()
                                 select student.Email).FirstOrDefault();

            //check inside Student App Num if that ID exists
            var currStudentRecordId = (from stuAppNum in _dataContext.StudentAppNum
                                       where stuAppNum.StudentEmail == studentsEmail
                                       select stuAppNum.Id).ToList();

            //if it exists, grab class descriptor(s) and studentAppNum row
            List<string> classNames = new List<string>();
            Dictionary<string, int> classInfo = new Dictionary<string, int>();

            if (currStudentRecordId.Count > 0)
            {

                foreach (int id in currStudentRecordId)
                {

                    //class names of classes student has applied for
                    var classNameCurr = (from appData in _dataContext.ApplicationData
                                         where appData.RecordId == id
                                         where appData.DataKeyId == 1
                                         select appData.Value).First().ToString();

                    //status of application of particular class student has applied for
                    var studAppID = (from appData in _dataContext.StudentAppNum
                                     where appData.Id == id &&
                                     appData.Status != "Incomplete" &&
                                     appData.Status != "Pending Employer Approval"
                                     select appData.Id).FirstOrDefault();

                    if (studAppID != 0)
                    {
                        classNames.Add(classNameCurr); //only add class name if pending professor approval

                        int studAppIDint = Convert.ToInt16(studAppID);
                        classInfo.Add(classNameCurr, studAppIDint);
                    }

                }

            }
            //no else needed

            ViewBag.classNames = classNames;
            ViewBag.classInfo = classInfo;

            return View();
        }


        /*Displays current application template fields onto web page  */
        public IActionResult SignJobDescription(int appId)
        {
            int id = appId;
            string className;
            // List<string> studentInputs = new List<string>();
            Dictionary<Models.ApplicationTemplate, Models.ApplicationData> appDetails
                = new Dictionary<Models.ApplicationTemplate, Models.ApplicationData>();

            var combined = from data in _dataContext.ApplicationData
                           join fields in _dataContext.ApplicationTemplate on data.DataKeyId equals fields.Id
                           where data.RecordId == appId
                           select new { data, fields };
            var studentValues = from data in _dataContext.ApplicationTemplate
                                where data.Entity == "Student"
                                where data.FieldName.Contains("sig")
                                || data.FieldName.Contains("date")
                                where data.FieldDescription.Contains("employer")
                                || data.FieldDescription.Contains("learning")
                                select data;

                className = (from data in _dataContext.ApplicationData
                             where data.RecordId == appId && data.DataKeyId == 1
                             select data.Value).FirstOrDefault();
                appDetails = combined.ToDictionary(t => t.fields, t => t.data);

            var prevIds = from data in _dataContext.ApplicationTemplate
                          where data.Entity == "Student"
                          where data.FieldName.Contains("sig")
                          || data.FieldName.Contains("date")
                          where data.FieldDescription.Contains("employer")
                          || data.FieldDescription.Contains("learning")
                          select data.ApplicationData;
          /*  var prevSigned = (from data in _dataContext.ApplicationData
                              where data.DataKeyId == Convert.ToInt32(studentValues.Id)
                                select data.ApplicationData.Value);

                var studentValues = (from data in _dataContext.ApplicationTemplate
                                  where data.Entity == "Student" 
                                  where data.FieldName.Contains("sig")
                                  where data.FieldDescription.Contains("employer")
                                  select data.ProperName).ToList();
               var studentDate = (from data in _dataContext.ApplicationTemplate
                                  where data.Entity == "Student"
                                  where data.FieldName.Contains("date")
                                  where data.FieldDescription.Contains("learning")
                                  select data.ProperName).ToString();
            studentValues.Add(studentDate);

    */
           // studentValues.Add("Today's Date");
          //  studentInputs = studentValues;

            ViewBag.className = className;
            ViewBag.recordId = appId;
            ViewBag.studentInputs = studentValues;
            ViewBag.prevId = prevIds;

            return View(appDetails);
          }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignedJobDescription(IEnumerable<Interactive_Internship_Application.Models.ApplicationTemplate> ApplicationTemplateModel)
        {
            //save submitted information into a dictionary
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            int recId = Convert.ToInt32(dict["recordID"]);

            int count = 0;//used to skip saving last 2 items in dict

            foreach (var item in dict)
            {
                if (count < (dict.Count - 2))
                {
                    var appData = new ApplicationData { RecordId = recId, DataKeyId = Convert.ToInt32(item.Key), Value = item.Value };
                    _dataContext.ApplicationData.Add(appData);
                    _dataContext.SaveChanges();
                }
                count++; 

            }




            return View("Index");
        }

    }
    
}

/*InvalidOperationException: The view 'Index' was not found.The following locations were searched:
/Views/Student/Index.cshtml
/Views/Shared/Index.cshtml
/Pages/Shared/Index.cshtml */
