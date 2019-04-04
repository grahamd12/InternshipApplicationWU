
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interactive_Internship_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNet.Identity;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Interactive_Internship_Application.Controllers
{
    public class ProfessorController : Controller
    {
        
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
      
        public IActionResult ProfViewApplication(int y)
        {
            var profClass = new List<string>();
            var profEmail = User.Identity.Name.ToString();
            var studTemp = new List<string>();
            using (var context1 = new Models.ApplicationDbContext())
            {

                profClass = (from faculty in context1.FacultyInformation
                                 where faculty.ProfEmail == profEmail
                                 select faculty.CourseName).ToList();
                    
                var studentData = (from student in context1.ApplicationData
                               where student.RecordId == y
                               select student).ToList();
                var properList = (from template in context1.ApplicationTemplate
                                  select template).ToList(); 
                var profFields = (from prof in context1.ApplicationTemplate
                                  where prof.Entity == "Professor"
                                  select prof).ToList();
                var dict = new List<string>();
                for(int i = 0; i < properList.Count(); i++)
                {
                    for(int j = 0; j<studentData.Count();j++)
                    {
                        if (properList[i].Id == studentData[j].DataKeyId)
                        {
                            dict.Add(properList[i].ProperName);
                            dict.Add(studentData[j].Value);
                        }
                    }
                }

                ViewBag.profData = profFields;

                ViewBag.bigDict = dict;
                return View();
            }
        }
    }
}