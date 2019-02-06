using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Interactive_Internship_Application.Models;

namespace Interactive_Internship_Application.Controllers
{
    public class DERController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
      public IActionResult EditAppTemp()
        {
            return View();
        }

        public IActionResult EditWebsite()
        {
            return View();
        }
        public IActionResult ManageActiveApps()
        {
            return View();
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