using System;
using System.Collections.Generic;
using System.Linq;
using Interactive_Internship_Application.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Interactive_Internship_Application.Controllers
{
    public class SSController : Controller
    {
        int id;
        public Models.ApplicationDbContext _dataContext { get; set; }
        public SSController(Models.ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageActiveApps()
        {
            List<string> tableData = new List<string>();
            Dictionary<int, List<string>> wholeTable = new Dictionary<int, List<string>>();

            using (var context = new Models.ApplicationDbContext())
            {

                // Get the professors class title. This will be used to query the database for all students
                // taking this class.

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


                // this dictionary will tell the user if the application has been signed or not
                Dictionary<int, string> signed = new Dictionary<int, string>();

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

                    var getSigned = (from data in context.ApplicationData
                                     where data.DataKeyId == 47 && data.RecordId == id
                                     select data.Value).FirstOrDefault();

                    signed.Add(id, getSigned);
                }

                ViewBag.getSigned = signed;
                ViewBag.Students = getStudents;
                ViewBag.tableCols = tableColumns;
                return View(wholeTable);
            }
        }

        public IActionResult SSViewApplication(int appId)
        {
            id = appId;
            string studName, className;
            List<string> cbaInputs = new List<string>();
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

                var ssValues = (from data in context1.ApplicationTemplate
                                where data.Entity == "CBA Student Services"
                                select data.ProperName).ToList();

                cbaInputs = ssValues;
            }
            ViewBag.studName = studName;
            ViewBag.className = className;
            ViewBag.recordId = appId;
            ViewBag.cbaInputs = cbaInputs;
            return View(appDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submitted(IEnumerable<Interactive_Internship_Application.Models.ApplicationTemplate> ApplicationTemplateModel, string response, int record)
        {

            string result = response; //used to determine if a new Student App Num needs to be created
            var context = new ApplicationDbContext();

            //save submitted information into a dictionary
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            context.SaveChanges();

            //get students record ID 
            int currStudentRecordId = record;

            //get number of fields Student Services enters
            int numSSFieldCount = (from x in context.ApplicationTemplate
                                        where x.Entity == "CBA Student Services"
                                        select x).Count();

            // Save the values to the database
            if (response.Contains("Submit"))
            {
                Save(dict, currStudentRecordId, numSSFieldCount);
            }
            return View("Index");
        }
        //function to save student's information to database
        public IActionResult Save(Dictionary<string, string> dict, int currId, int numFields)
        {
            int count = 0; //used to determine if all input data has been saved


            //saves students input information to database
            foreach (var item in dict)
            {

                if (count < (numFields)) //don't count submitted button response
                                             //in field count
                {
                    if (item.Value.Length > 0) //only saves input information (no empty info)
                    {
                        int intKey = Int32.Parse(item.Key.ToString());
                        var appDataCurrent = new ApplicationData { RecordId = currId, DataKeyId = intKey, Value = item.Value };

                        //determine if recordID and dataKeyID already exist in ApplicationData
                        var prevSaved = (from record in _dataContext.ApplicationData
                                         where record.DataKeyId == intKey &&
                                         record.RecordId == currId
                                         select record).SingleOrDefault();

                        if (prevSaved != null) //record was previously saved
                        {
                            prevSaved.Value = appDataCurrent.Value;
                        }
                        else //record was not previously saved
                        {
                            _dataContext.ApplicationData.Add(appDataCurrent);
                        }

                        _dataContext.SaveChanges();
                    }
                    count++;
                }
            }
            return View("Index");
        }
    }
}