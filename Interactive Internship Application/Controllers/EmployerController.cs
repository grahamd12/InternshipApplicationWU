using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Interactive_Internship_Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Interactive_Internship_Application.Controllers
{
    [Authorize(Roles = "Admin, Employer")]
    public class EmployerController : Controller
    {
        

        //create this to have a local variable to manipulate the database
        //below takes in the database (the data from the view ) and puts it local for this
        //controller to decide what to do to the data. 
        public Models.ApplicationDbContext applicationDbContext { get; set; }
        public EmployerController(Models.ApplicationDbContext dbContext)
        {
            applicationDbContext = dbContext;
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


       }
    //unsure what the bit below does, Dysean had Matea add this. 
      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult Submitted(IEnumerable<Interactive_Internship_Application.Models.ApplicationTemplate> ApplicationTemplateModel)
      {
           /* string name = User.Identity.Name;
           
            var studentId = from e in applicationDbContext.EmployerLogin
                            where e.Email == name
                            && e.StudentEmail == 
                                
                                
                                select e.Id;
                                */
          int count = 0;
          var context = new Models.ApplicationDbContext();
          int numEmployerFieldCount = (from x in context.ApplicationTemplate
                                       where x.Entity == "Employer"
                                       select x).Count();


          var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
              foreach (var item in dict)
              {

                  if (count < numEmployerFieldCount)
                  {
                      int intKey = Int32.Parse(item.Key.ToString());

                  //changed the recordId to not be a foreign key on StudentInformation just to see if it was working. 
                  //Change AppData DB back the right way later
                  //Had to take out the FK's of the AppData table to make it work too
                  var appDataCurrent = new ApplicationData { RecordId = 1, DataKeyId = intKey, Value = item.Value };
                      applicationDbContext.ApplicationData.Add(appDataCurrent);
                      applicationDbContext.SaveChanges();
                  count++;
                  }
               }



          return View("Index");

      } 
        }

}

