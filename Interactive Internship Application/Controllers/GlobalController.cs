/*
 * Controller for Employer's Regenerate Pin and Invalid Login errors
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Interactive_Internship_Application.Controllers
{
    public class GlobalController : Controller
    {
        public IActionResult Index()
        {            
            return View("Login");
        }


        public IActionResult ErrorRegeneratePin()
        {
            return View();
        }

        public IActionResult ErrorInvalidEmailForgotLogin()
        {
            return View();
        }
    }
}