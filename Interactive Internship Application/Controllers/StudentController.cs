using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Interactive_Internship_Application.Controllers
{
    public class StudentController : Controller
    {
        
        [Authorize(Roles = "Admin,Student")]
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult ViewApplications()
        {
            return View();
        }

         public IActionResult Application()
        {
            return View();
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