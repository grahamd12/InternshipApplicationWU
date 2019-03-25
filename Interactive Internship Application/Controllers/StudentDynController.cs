using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Interactive_Internship_Application.Controllers
{
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

                //   var getSingleFieldName = context.ApplicationTemplate.ToList();
                // ViewBag.allFieldNames = getSingleFieldName;
                return View(context.ApplicationTemplate.ToList());
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
            foreach (var key in HttpContext.Request.Form.Keys)
            {
                var test = HttpContext.Request.Form[key]; //to get all names from form
                Console.WriteLine("Test ", test);




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
