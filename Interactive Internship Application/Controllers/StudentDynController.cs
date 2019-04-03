using System;
using System.Collections.Generic;
using System.Linq;
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
            



            return View("Index");

        }


    

    /*

      //attempt at using the same controller method to process the data
      [HttpGet]
    public IActionResult ApplicationDynamic()
    {
        if (Request.HttpContext == POST) //doesn't compile
         {
              return View("Index");
          }
      else
      { 
            //returns all entries in the application template table
            using (var context = new Interactive_Internship_Application.Models.IIPContext())
            {

                var getSingleFieldName = context.ApplicationTemplate.ToList();
                ViewBag.allFieldNames = getSingleFieldName;
                return View();
            }
        }
     }

       //using a separate controller method named the formaction value from the form

      */

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
