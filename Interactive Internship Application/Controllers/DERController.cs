using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Interactive_Internship_Application.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Interactive_Internship_Application.Controllers
{
    [Authorize(Roles = "Admin")]
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

        public IActionResult EditAppTempEntity()
        {
            return View();
        }


        public IActionResult EditWebsite()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ManageActiveApps()
        {                                                                                                       
            //returns all entries in the StudentInfo table
            using (var context = new Models.ApplicationDbContext())
            {
                //var getStudents = context.StudentInformation.ToList();
                int records = (from actives in context.ApplicationData
                               select actives.RecordId).Distinct().Count();

                string[,] tableArray = new string[records+1,8];
                tableArray[0, 0] = "1";
                tableArray[0, 1] = "2";
                tableArray[0, 2] = "3";
                tableArray[0, 3] = "7";
                tableArray[0, 4] = "8";
                tableArray[0, 5] = "11";
                tableArray[0, 6] = "14";
                tableArray[0, 7] = "20";

                var activeAppsQuery = (from actives in context.ApplicationData
                                      orderby actives.RecordId
                                      select actives)
                                      .ToList();

                int currentDataID = 0;
                foreach(var item in activeAppsQuery)
                {
                    if((currentDataID <8 ) && (item.DataKeyId).ToString() == tableArray[0, currentDataID])
                    {
                        tableArray[item.RecordId, currentDataID] = item.Value;
                        currentDataID++;

                    }
                }
                ViewBag.tableArray = tableArray;

                return View(tableArray);
                //List<actives> activeList = activeAppsQuery.ToList<actives>();
                //    var getActiveStudentsInfoRightOrder =
                //     (from e in getStudentsInfo.AsQueryable<ApplicationData>()
                //     orderby e.RecordId
                //     select new { ID = e.DataKeyId, value = e.Value, student = e.RecordId })
                //     .ToList();

                //ViewBag.info = getActiveStudentsInfoRightOrder;

                //return View(getActiveStudentsInfoRightOrder);
            }
        }

        [HttpGet]
        public IActionResult ManageActiveAppsBAD()
        {
            //returns all entries in the StudentInfo table
            using (var context = new Models.ApplicationDbContext())
            {
            
                return View(context.ApplicationData.ToList());
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