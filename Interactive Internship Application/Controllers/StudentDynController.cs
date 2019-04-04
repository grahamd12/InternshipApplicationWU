﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Interactive_Internship_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Interactive_Internship_Application.Controllers
{
    [Authorize(Roles = "Admin,Student")]

    public class StudentDynController : Controller
    {
        public Models.ApplicationDbContext _dataContext { get; set; }
        public StudentDynController(Models.ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
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
           
            if (currStudentRecordId.Count > 0)
            {
                List<string> classNames = new List<string>();

                foreach (int id in currStudentRecordId)
                {
                
                    
                    var classNameCurr = (from appData in context.ApplicationData
                                         where appData.RecordId == id 
                                         where appData.DataKeyId == 1
                                         select appData.Value).FirstOrDefault().ToString();
                    classNames.Add(classNameCurr); 
                 

                }
                
            }
            //no else needed

            //show create new application


            return View();
        }

        /*Displays current application template fields onto web page  */
        [HttpGet]
        public IActionResult Application()
        {
            //returns all entries in the application template table
            using (var context = new Interactive_Internship_Application.Models.ApplicationDbContext())
            {
                  var getSingleFieldName = context.ApplicationTemplate.ToList();
                // ViewBag.allFieldNames = getSingleFieldName;
                // return View(context.ApplicationTemplate.ToList());
                 return View(getSingleFieldName);

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
            NewEmail("milojkovicm2@mailbox.winthrop.edu", "password", "mateamilojkovic@yahoo.com", "Winthrop University Internship Application", "Body Text");


            int count = 0;
            var context = new Models.ApplicationDbContext();
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

            
            return View("Index");

        }

        //email function
        public void NewEmail(string fromEmail, string password, string toAddress, string subject, string body)
        {
            using (System.Net.Mail.MailMessage myMail = new System.Net.Mail.MailMessage())
            {
                //create a new blank email
                myMail.From = new MailAddress(fromEmail);

                //fill email with correct receiver address, subject, and body message
                myMail.To.Add(toAddress);
                myMail.Subject = subject;
                myMail.IsBodyHtml = true;
                myMail.Body = body;

                //send email
                using (System.Net.Mail.SmtpClient s = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587))
                {
                    s.DeliveryMethod = SmtpDeliveryMethod.Network;
                    s.UseDefaultCredentials = false;
                    s.Credentials = new System.Net.NetworkCredential(myMail.From.ToString(), password);
                    s.EnableSsl = true;
                    s.Send(myMail);
                }
            }
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
