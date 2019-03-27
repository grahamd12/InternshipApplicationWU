using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Interactive_Internship_Application.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Interactive_Internship_Application.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DERController : Controller
    {
        public Models.ApplicationDbContext applicationDbContext { get; set; }
        public DERController(Models.ApplicationDbContext dbContext)
        {
            applicationDbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
      public IActionResult EditAppTemp()
        {
            return View();
        }

        public IActionResult EditAppTempEntity()
        {
            return View();
        }


        public IActionResult EditWebsite()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ManageActiveApps()
        {
            //returns all entries in the StudentInfo table
            using (var context = new Models.ApplicationDbContext())
            { 
                //var getStudents = context.StudentInformation.ToList();
                var getStudentsInfo = context.ApplicationData.ToList();

                    var getActiveStudentsInfoRightOrder =
                     (from e in getStudentsInfo.AsQueryable<ApplicationData>()
                     orderby e.RecordId
                     select new { ID = e.DataKeyId, value = e.Value, student = e.RecordId })
                     .ToList();


                ViewBag.info = getActiveStudentsInfoRightOrder;
          
                return View(context.ApplicationData.ToList());
            }
        }

        [HttpGet]
        public IActionResult ManageActiveAppsBAD()
        {
            //returns all entries in the StudentInfo table
            using (var context = new Models.ApplicationDbContext())
            {
            
                return View(context.ApplicationData.ToList());
            }
        }
        public IActionResult ManagePreviousApps()
        {
            return View();
        }
        public IActionResult ManageUsers()
        {
            return View();
        }
    }
}