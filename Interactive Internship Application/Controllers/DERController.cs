using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using excel = Microsoft.Office.Interop.Excel;
using System;
using System.IO;
using System.Text;

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
                // get any disabled fields
                disabledFields = (from temp in context.ApplicationTemplate
                                  where temp.Entity == entity.entityName &&
                                  temp.Deleted == true
                                  select temp.ProperName).ToList();

                // get all enabled fields
                enabledFields = (from temp in context.ApplicationTemplate
                                where temp.Entity == entity.entityName
                                select temp.ProperName).ToList();
            }

            // entity being changed as well as a list of enabled and disabled fields
            // are returned to the view
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

                    // change appropriate fields
                    fieldToEnable.Deleted = false;
                    fieldToEnable.RequiredField = true;

                    //save to DB
                    context.SaveChanges();
                }
            }
            return View("EditAppTemp");
        }

        public IActionResult SaveFieldToDB(string entity)
        {
            //get everything from the form into variables
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
                // if the field is required, set variable to true for later use in model
                req = true;
            }


            if (fieldType == "select" || fieldDesc == "" || propName == "")
            {
                //if any of the fields empty, don't do anything
            }

            else
            {
                // create new model for inputted data
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

                // save to DB
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

                    // change appropriate fields and save to DB
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
                // get fields and their data
                var combined = from data in context.ApplicationData
                               join fields in context.ApplicationTemplate on data.DataKeyId equals fields.Id
                               where data.RecordId == appID
                               select new { data, fields };

                // get the student's name
                studName = (from data in context.ApplicationData
                            where data.RecordId == appID && data.DataKeyId == 3
                            select data.Value).FirstOrDefault();

                // get the class the student is applying for
                className = (from data in context.ApplicationData
                             where data.RecordId == appID && data.DataKeyId == 1
                             select data.Value).FirstOrDefault();

                // fancy lambda functions for combining the two lists into a dictionary
                appDetails = combined.ToDictionary(t => t.fields, t => t.data); 
            }

            // student and class name returned in viewbag, dictionary returned as model for frontend
            ViewBag.studName = studName;
            ViewBag.className = className;
            return View(appDetails);
        }


        [HttpPost]
        public ActionResult Report(string actInact)
        { 

            //var columnData = new List();
            using (var context = new Models.ApplicationDbContext())
            {
                dynamic columnData = "filler...";

                //if active applications excel report button was pressed
                if(actInact == "Active")
                {
                    columnData = (from num in context.StudentAppNum
                                      join data in context.ApplicationData on num.Id equals data.RecordId
                                      join temp in context.ApplicationTemplate on data.DataKeyId equals temp.Id
                                      where num.Status != "Complete"
                                      select new { id = num.Id, prop = temp.ProperName, field = temp.FieldName, ent = temp.Entity, value = data.Value })
                                      .ToList();
                }

                //if inactive applications excel report button was pressed
                else if (actInact == "Inactive")
                {
                    columnData = (from num in context.StudentAppNum
                                      join data in context.ApplicationData on num.Id equals data.RecordId
                                      join temp in context.ApplicationTemplate on data.DataKeyId equals temp.Id
                                      where num.Status == "Complete"
                                      select new { id = num.Id, prop = temp.ProperName, field = temp.FieldName, ent = temp.Entity, value = data.Value })
                                      .ToList();
                }

                // get current time and decide name for generated csv filed
                DateTime currTime = DateTime.Today;
                string filePath = @"C:\Users\Daniel Branham\Desktop\" + actInact + "_Applications_Generated_Report_" + currTime.ToString("MM - dd - yyy") + ".csv";
                string delimiter = ",";
                string columnString = "";
                string dataString = "";
                var csv = new StringBuilder();
            
                // write columns to csv object
                int first = columnData[0].id;
                int column = 0;
                while(column < columnData.Count && first == columnData[column].id)
                {
                    // accounts for commas in any of them field names
                    columnString += "\"" + columnData[column].prop + "\"" + " (" + columnData[column].ent + ")" + delimiter;
                    column++;
                }
                csv.AppendLine(columnString);

                // write data to csv object
                int recID = columnData[0].id;
                foreach (var item in columnData)
                {
                    // if the current record id is different from the one we've been using
                    // write to the object, clear the string, and start a new one for the next line
                    if (recID != item.id)
                    {
                        csv.AppendLine(dataString);
                        dataString = "";
                        recID = item.id;
                    }
                    dataString += "\"" + item.value + "\"" + delimiter;
                }
                csv.AppendLine(dataString);

                // write all data in csv object to csv file
                try { System.IO.File.WriteAllText(filePath, csv.ToString()); }
                catch (System.IO.IOException)
                { // don't do anything if the file is currently open for some reason
                }
                    
            }
            return View("Index");
        }

        public IActionResult ManageUsers()
        {
            return View();
        }
    }
}