using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Interactive_Internship_Application.Controllers
{
    public class ProfessorController : Controller
    {
        [Authorize(Roles = "Admin,Professor")]

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ProfAppView1()
        {
            return View();
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
