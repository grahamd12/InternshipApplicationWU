
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interactive_Internship_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Interactive_Internship_Application.Controllers
{
    public class ProfessorController : Controller
    {
        [Authorize(Roles = "Admin,Professor")]

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index()
        {
            using (var context = new Models.ApplicationDbContext())
            {

                var getStudents = context.ApplicationData.ToList();

                var getActiveStudentsInfoRightOrder =
                    from e in getStudents.AsQueryable<ApplicationData>()
                    orderby e.RecordId
                    select e;

                return View(getStudents);
            }
        }
        public IActionResult ProfAppView1()
        {

            using (var context1 = new Models.ApplicationDbContext())
            {
                var getStudents = context1.ApplicationData.ToList();

                var getActiveStudentsInfoRightOrder =
                    from e in getStudents.AsQueryable<ApplicationData>()
                    orderby e.RecordId
                    select e;

                return View(getStudents);
            }
        }
        public IActionResult ProfAppView2()
        {
            return View();
        }
        public IActionResult ProfAppView3()
        {
            return View();
        }
    }
}