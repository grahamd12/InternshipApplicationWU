
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
        int id;
        public Models.ApplicationDbContext _dataContext { get; set; }
        public ProfessorController(Models.ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index()
        {
            string profClass;
            List<string> tableData = new List<string>();
            Dictionary<int, List<string>> wholeTable = new Dictionary<int, List<string>>();

            using (var context = new Models.ApplicationDbContext())
            {
                // First grab the professor's email. This will be used to get their class information.
                var profEmail = User.Identity.Name.ToString();

                // Get the professors class title. This will be used to query the database for all students
                // taking this class.

                profClass = (from faculty in context.FacultyInformation
                         where faculty.ProfEmail == profEmail
                         select faculty.CourseName).ToString();

                var tableRowSize = (from apps in context.StudentAppNum
                                    where apps.Status != "Complete"
                                    select apps.Id).ToList();

                var tableColumns = (from temp in context.ApplicationTemplate
                                    where temp.FieldName == "class_enrolled" || temp.FieldName == "semester" ||
                                           temp.FieldName == "name" || temp.FieldName == "graduation year" ||
                                           temp.FieldName == "major_conc" || temp.FieldName == "org_name"
                                    select temp.ProperName).ToList();

                tableColumns.Add("Status");
                tableColumns.Add("Signed");
                tableColumns.Add("View Application Details");

                var getStudents = (from num in context.StudentAppNum
                                   join data in context.ApplicationData on num.Id equals data.RecordId
                                   join temp in context.ApplicationTemplate on data.DataKeyId equals temp.Id
                                   where temp.FieldName == "class_enrolled" || temp.FieldName == "semester" ||
                                           temp.FieldName == "name" || temp.FieldName == "graduation year" ||
                                           temp.FieldName == "major_conc" ||
                                           temp.FieldName == "org_name" && num.Status != "Complete"
                                   select new { id = num.Id, field = temp.FieldName, value = data.Value }).ToList();

                foreach (var id in tableRowSize)
                {
                    // add each column's data to the list
                    tableData = (from data in getStudents
                                 where data.id == id
                                 select data.value).ToList();

                    // add status
                    tableData.Add((from num in context.StudentAppNum
                                  where num.Id == id && num.Status != "Complete"
                                  select num.Status).First().ToString());
                    tableData.Insert(0, id.ToString());

                    // ass data for each application to dictionary
                    wholeTable.Add(id, tableData);
                }

                ViewBag.Students = getStudents;
                ViewBag.Professor = profClass;
                ViewBag.tableCols = tableColumns;
                return View(wholeTable);
            }
        }
      
        public IActionResult ProfViewApplication(int appId)
        {
            id = appId;
            string studName, className;
           List<string> professorInputs = new List<string>();
            Dictionary<Models.ApplicationTemplate, Models.ApplicationData> appDetails 
                = new Dictionary<Models.ApplicationTemplate, Models.ApplicationData>();
            using (var context1 = new Models.ApplicationDbContext())
            {
                var combined = from data in context1.ApplicationData
                               join fields in context1.ApplicationTemplate on data.DataKeyId equals fields.Id
                               where data.RecordId == appId
                               select new { data, fields };

                studName = (from student in context1.ApplicationData
                            where student.RecordId == appId && student.DataKeyId == 3
                            select student.Value).FirstOrDefault();

                className = (from data in context1.ApplicationData
                             where data.RecordId == appId && data.DataKeyId == 1
                             select data.Value).FirstOrDefault();
                appDetails = combined.ToDictionary(t => t.fields, t => t.data);

                var profValues = (from data in context1.ApplicationTemplate
                                  where data.Entity == "Professor"
                                  select data.ProperName).ToList();

                professorInputs = profValues;
            }

            ViewBag.studName = studName;
            ViewBag.className = className;
            ViewBag.recordId = appId;
            ViewBag.profInputs = professorInputs;

            return View(appDetails);
        }


        [HttpPost]

        [ValidateAntiForgeryToken]

        public ActionResult Submitted(IEnumerable<Interactive_Internship_Application.Models.ApplicationTemplate> ApplicationTemplateModel, string response)

        {



            string result = response;  //used to determine if a new Student App Num needs to be created

            var context = new ApplicationDbContext();


            //save submitted information into a dictionary

            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());


                context.SaveChanges();


          
            if (response.Contains("Submit"))

            {

                foreach (var rec in dict)

                {

                    if (rec.Value.Length <= 0)

                    {

                        return View("Application");

                    }

                }
                _dataContext.SaveChanges();
            }
            return View("Index");
        }
    }
}