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
            string fieldName = propName.ToLower();
            fieldName = fieldName.Replace(" ", "_");

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
                    Deleted = del
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
            using (var context = new Models.ApplicationDbContext())
            {
                // get current number of records in database
                int records = (from actives in context.ApplicationData
                               select actives.RecordId).Distinct().Count();

                // make 2d array with amount of records and columns
                string[,] tableArray = new string[records+1,8];

                // the string numbers are the values we want from 
                // our database for our table 
                tableArray[0, 0] = "1";
                tableArray[0, 1] = "2";
                tableArray[0, 2] = "3";
                tableArray[0, 3] = "7";
                tableArray[0, 4] = "8";
                tableArray[0, 5] = "11";
                tableArray[0, 6] = "14";
                tableArray[0, 7] = "20";

                // get everything from ApplicationData and sort by record_id
                var activeAppsQuery = (from actives in context.ApplicationData
                                      orderby actives.RecordId
                                      select actives)
                                      .ToList();

                // go through all records, pull out the data_key_ids we want and
                // insert them into the 2d array, row by row
                int currentDataID = 0;
                foreach(var item in activeAppsQuery)
                {
                    if((currentDataID <8 ) && (item.DataKeyId).ToString() == tableArray[0, currentDataID])
                    {
                        tableArray[item.RecordId, currentDataID] = item.Value;
                        currentDataID++;

                    }
                }
                return View(tableArray);

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
            return View();
        }
        public IActionResult ManageUsers()
        {
            return View();
        }
    }
}