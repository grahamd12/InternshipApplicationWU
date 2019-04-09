using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using excel = Microsoft.Office.Interop.Excel;
using System;
using System.IO;

namespace Interactive_Internship_Application.Controllers
{
    [Authorize(Roles = "Admin")]
    public class entityInfo
    {
        public string entityName;
    }

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

        // the name of the entity's template being edited is passed in so that the code knows which
        // data to pull from the database
        public IActionResult EditAppTempEntity(string eName)
        {
            var entity = new entityInfo();
            entity.entityName = eName;
            var disabledFields = new List<string>();
            var enabledFields = new List<string>();
            using (var context = new Models.ApplicationDbContext())
            {
                
                disabledFields = (from temp in context.ApplicationTemplate
                                  where temp.Entity == entity.entityName &&
                                  temp.Deleted == true
                                  select temp.ProperName).ToList();

                enabledFields = (from temp in context.ApplicationTemplate
                                where temp.Entity == entity.entityName
                                select temp.ProperName).ToList();
            }

            ViewBag.entity = entity.entityName;
            ViewBag.enabledFields = enabledFields;
            ViewBag.disabledFields = disabledFields;
            return View();
        }

        public IActionResult EnableFieldInDB(string entity)
        {
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());

            if (dict["disabledFieldType"] == "select")
            {
                //need some exception handling/warning to user

            }

            else
            {
                using (var context = new Models.ApplicationDbContext())
                {
                    var fieldToEnable = (from temp in context.ApplicationTemplate
                                         where temp.ProperName == dict["disabledFieldType"] &&
                                               temp.Entity == entity
                                         select temp).FirstOrDefault();

                    fieldToEnable.Deleted = false;
                    fieldToEnable.RequiredField = true;

                    context.SaveChanges();
                }
            }
            return View("EditAppTemp");
        }

        public IActionResult SaveFieldToDB(string entity)
        {
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            string fieldType = dict["fieldType"];
            string fieldDesc = dict["fieldDesc"];
            string propName = dict["fieldName"];
            bool del = false;
            bool req = false;
            string fieldName = propName.ToLower();
            fieldName = fieldName.Replace(" ", "_");

            if((dict.ContainsKey("required")) && dict["required"] == "1")
            {
                req = true;
            }


            if (fieldType == "select" || fieldDesc == "" || propName == "")
            {

            }

            else
            {
                var newTemplateField = new Models.ApplicationTemplate
                {
                    FieldName = fieldName,
                    FieldDescription = fieldDesc,
                    Entity = entity,
                    ControlType = fieldType,
                    ProperName = propName,
                    Deleted = del,
                    RequiredField = req,
                };

                applicationDbContext.ApplicationTemplate.Add(newTemplateField);
                applicationDbContext.SaveChanges();
            }
            return View("EditAppTemp");
        }

        public IActionResult DisableFieldInDB(string entity)
        {
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());

            if (dict["deleteFieldType"] == "select")
            {
                //need some exception handling/warning to user

            }

            else
            {
                using (var context = new Models.ApplicationDbContext())
                {
                    var fieldToEnable = (from temp in context.ApplicationTemplate
                                         where temp.ProperName == dict["deleteFieldType"] &&
                                               temp.Entity == entity
                                         select temp).FirstOrDefault();

                    fieldToEnable.Deleted = true;
                    fieldToEnable.RequiredField = false;

                    context.SaveChanges();
                }
            }
            return View("EditAppTemp");
        }

        public IActionResult EditWebsite()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ManageActiveApps()
        {
            List<string> tableData = new List<string>();
            Dictionary<int, List<string>> wholeTable = new Dictionary<int, List<string>>();

            using (var context = new Models.ApplicationDbContext())
            {

                // get amount of active applications
                var tableRowSize = (from apps in context.StudentAppNum
                                    where apps.Status != "Complete"
                                    select apps.Id).ToList();

                // get proper column names that we want to display in table
                var tableColumns = (from temp in context.ApplicationTemplate
                                    where temp.FieldName == "class_enrolled" || temp.FieldName == "semester" ||
                                           temp.FieldName == "name" || temp.FieldName == "graduation year" || temp.FieldName == "major_conc" ||
                                           temp.FieldName == "org_name"
                                    select temp.ProperName).ToList();

                tableColumns.Add("Status");
                tableColumns.Add("Signed");
                tableColumns.Add("View Application Details");

                // get data on each active application
                var columnData = (from num in context.StudentAppNum
                                   join data in context.ApplicationData on num.Id equals data.RecordId
                                   join temp in context.ApplicationTemplate on data.DataKeyId equals temp.Id
                                   where temp.FieldName == "class_enrolled" || temp.FieldName == "semester" ||
                                      temp.FieldName == "name" || temp.FieldName == "graduation year" || temp.FieldName == "major_conc" ||
                                      temp.FieldName == "org_name" && num.Status != "Complete"
                                   select new { id = num.Id, field = temp.FieldName, value = data.Value })
                                   .ToList();

                foreach (var id in tableRowSize)
                {
                    // add each columns data to list
                    tableData = (from data in columnData
                                 where data.id == id
                                 select data.value).ToList();

                    // add status of each app to list
                    tableData.Add((from num in context.StudentAppNum
                                  where num.Id == id && num.Status != "Complete"
                                  select num.Status).First().ToString());

                    tableData.Insert(0, id.ToString());

                    //add data for each application to dictionary
                    wholeTable.Add(id, tableData);
                }

                ViewBag.tableCols = tableColumns;
                return View(wholeTable);
            }
        }

        [HttpPost]
        public ActionResult Report()
        {
            using (var context = new Models.ApplicationDbContext())
            {
                // get column names for report
                var reportDataColumns = (from temp in context.ApplicationTemplate
                                        orderby temp.Id
                                        select temp)
                                        .ToList();

                // get actual data to be fitted under each column
                var reportData = (from data in context.ApplicationData
                                  orderby data.RecordId
                                  select data)
                                  .ToList();

                // make excel object
                excel.Application excelApp = new excel.Application();
                excel.Workbook wrkBook = excelApp.Workbooks.Add(System.Reflection.Missing.Value);
                excel.Worksheet workSheet = (excel.Worksheet)wrkBook.ActiveSheet;

                // attempt to write columns to excel file...
                int colNum = 0;
                foreach(var columns in reportDataColumns)
                {
                    var dummy = columns.ProperName;
                    workSheet.Cells[1, colNum] = columns.ProperName;
                    colNum++;
                }

                // attempt to write data to excel file...
                int currID = 2;
                int dataID = 1;
                foreach(var item in reportData)
                {
                    if(currID-1 != item.RecordId)
                    {
                        currID++;
                    }
                    var otherDummy = item.Value;
                    workSheet.Cells[currID, dataID] = item.Value;
                    dataID++;
                }

                // get current time and save workbook as following
                DateTime currTime = DateTime.Today;
                wrkBook.SaveAs("Active_Applications_Generated_Report_" + currTime.ToString("MM-dd-yyy") + ".xlsx");
                wrkBook.Close();
                excelApp.Quit();
                
                return View("Index");
            }
        }

        public IActionResult ManagePreviousApps()
        {
            List<string> tableData = new List<string>();
            Dictionary<int, List<string>> wholeTable = new Dictionary<int, List<string>>();

            using (var context = new Models.ApplicationDbContext())
            {

                // get amount of active applications
                var tableRowSize = (from apps in context.StudentAppNum
                                    where apps.Status == "Complete"
                                    select apps.Id).ToList();

                // get proper column names that we want to display in table
                var tableColumns = (from temp in context.ApplicationTemplate
                                    where temp.FieldName == "class_enrolled" || temp.FieldName == "semester" ||
                                           temp.FieldName == "name" || temp.FieldName == "graduation year" || temp.FieldName == "major_conc" ||
                                           temp.FieldName == "org_name"
                                    select temp.ProperName).ToList();

                tableColumns.Add("View Application Details");


                // get data on each active application
                var columnData = (from num in context.StudentAppNum
                                  join data in context.ApplicationData on num.Id equals data.RecordId
                                  join temp in context.ApplicationTemplate on data.DataKeyId equals temp.Id
                                  where temp.FieldName == "class_enrolled" || temp.FieldName == "semester" ||
                                     temp.FieldName == "name" || temp.FieldName == "graduation year" || temp.FieldName == "major_conc" ||
                                     temp.FieldName == "org_name" && num.Status != "Complete"
                                  select new { id = num.Id, field = temp.FieldName, value = data.Value })
                                  .ToList();

                foreach (var id in tableRowSize)
                {
                    // add data from each application to list
                    tableData = (from data in columnData
                                 where data.id == id
                                 select data.value).ToList();

                    tableData.Insert(0, id.ToString());
                    // add "complete" status to each of the applications
                    tableData.Add("Complete");

                    //add data for each application to dictionary
                    wholeTable.Add(id, tableData);
                }

                ViewBag.tableCols = tableColumns;
                return View(wholeTable);
            }
        }

        public IActionResult ApplicationDetails(int appID)
        {
            Dictionary<Models.ApplicationTemplate, Models.ApplicationData> appDetails = new Dictionary<Models.ApplicationTemplate, Models.ApplicationData>();
            string studName, className;
            using (var context = new Models.ApplicationDbContext())
            {
                var combined = from data in context.ApplicationData
                               join fields in context.ApplicationTemplate on data.DataKeyId equals fields.Id
                               where data.RecordId == appID
                               select new { data, fields };

                studName = (from data in context.ApplicationData
                            where data.RecordId == appID && data.DataKeyId == 3
                            select data.Value).FirstOrDefault();

                className = (from data in context.ApplicationData
                             where data.RecordId == appID && data.DataKeyId == 1
                             select data.Value).FirstOrDefault();

                appDetails = combined.ToDictionary(t => t.fields, t => t.data); 
            }

            ViewBag.studName = studName;
            ViewBag.className = className;
            return View(appDetails);
        }

        public IActionResult ManageUsers()
        {
            return View();
        }
    }
}