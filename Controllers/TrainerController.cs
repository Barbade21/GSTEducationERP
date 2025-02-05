using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using GSTEducationERPLibrary.Bind;
using GSTEducationERPLibrary.Trainer;


namespace GSTEducationERP.Controllers
{
    public class TrainerController : Controller
    {
        BALTrainer objTrainer = new BALTrainer();
        BALBind objBALbind = new BALBind();

        public TrainerController()
        {
            objTrainer = new BALTrainer();
        }
    
        public ActionResult Index()
        {
            return View();
        }
        public class BreadcrumbItem
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }
        private const string StaffListSessionKey = "FullStaffList";
        private const string StaffAttendanceListSessionKey = "FullStaffAttendanceList";
        private const string AdvancePayStaffListSessionKey = "AdvancePayStaffList";
        private const string AllStaffAttendanceListSessionKey = "AllFullStaffAttendanceList";

        public async Task<ActionResult> studentFeeMainpageasyncSH()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                 {
                   //new BreadcrumbItem { Name = "Home", Url = "/" },
                   new BreadcrumbItem { Name = "Batch Attendance", Url = "BatchAttendace/Trainer" },
                   //new BreadcrumbItem { Name = "Fee Collection", Url = "studentFeeMainpageasyncSH/Accountant" } // Adjust URL as needed
                 };

                ViewBag.Breadcrumbs = breadcrumbs;
                await AllCourseBind();
                return View("BatchAttendance");
            }
        }

        public async Task AllCourseBind()
        {
            Trainer obj = new Trainer();
        //    obj.BranchCode = Session["BranchCode"].ToString();
            DataSet ds1 = new DataSet();
            ds1 = await objTrainer.AllCourse();
            List<SelectListItem> AllCourseBind = new List<SelectListItem>();
            if (ds1.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds1.Tables[0].Rows)
                {
                    AllCourseBind.Add(new SelectListItem
                    {
                        Text = dr["CourseName"].ToString(),
                        Value = dr["Coursecode"].ToString()
                    });
                }
            }
            ViewBag.AllCourseBind = AllCourseBind;

        }

        public async Task<ActionResult> BatchAttendance()
        {
            DataSet ds = await objTrainer.GetBatchAttendance();
            if (ds != null && ds.Tables.Count > 0)
            {
                List<Trainer> batchAtt = ds.Tables[0].AsEnumerable().Select(row => new Trainer
                {
                    BatchCode = row.Field<string>("BatchCode"),
                    BatchName = row.Field<string>("BatchName"),
                    BatchTime = row.Field<string>("BatchTime"),
                    TotalCandidate = row.Field<int?>("Total Candidates").ToString() ?? "",
                    
                }).ToList();

                DataSet ds1 = new DataSet();
                ds1 = await objTrainer.AllCourse();
                List<SelectListItem> AllCourseBind = new List<SelectListItem>();
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds1.Tables[0].Rows)
                    {
                        AllCourseBind.Add(new SelectListItem
                        {
                            Text = dr["CourseName"].ToString(),
                            Value = dr["Coursecode"].ToString()
                        });
                    }
                }

                DataSet ds2 = new DataSet();
                ds2 = await objTrainer.StatusListAB();
                List<SelectListItem> AllStatusBind = new List<SelectListItem>();
                if (ds2.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds2.Tables[0].Rows)
                    {
                        AllStatusBind.Add(new SelectListItem
                        {
                            Text = dr["Status"].ToString(),
                            Value = dr["StatusId"].ToString()
                        });
                    }
                }

                ViewBag.AllCourse = AllCourseBind;
                ViewBag.AllStatus=AllStatusBind;
                return View(batchAtt);
            }
           
            return View(new List<Trainer>());
        }

/// <summary>
/// Fetch All Batches from Database related to selected Course
/// </summary>
/// <param name="CourseCode"></param>
/// <returns></returns>
        public async Task<JsonResult> AllBatches(string CourseCode)
        {
            Trainer objT = new Trainer();
            objT.CourseCode = CourseCode;
           
            BALTrainer objC = new BALTrainer();
            DataSet ds = await objC.CourseWiseBatches(objT.CourseCode);

            List<SelectListItem> BatchesList = new List<SelectListItem>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                BatchesList.Add(new SelectListItem
                {
                    Text = row["BatchName"].ToString(),
                    Value = row["BatchCode"].ToString(),
                });
                ViewBag.AllBatches = BatchesList;
            }
            return Json(BatchesList, JsonRequestBehavior.AllowGet);

        }


        public async Task<ActionResult> ProjectAttendance()
        {
            DataSet ds = await objTrainer.GetBatchAttendance();
            if (ds != null && ds.Tables.Count > 0)
            {
                List<Trainer> batchAtt = ds.Tables[0].AsEnumerable().Select(row => new Trainer
                {
                    BatchCode = row.Field<string>("BatchCode"),
                    BatchName = row.Field<string>("BatchName"),
                    BatchTime = row.Field<string>("BatchTime"),
                    TotalCandidate = row.Field<int?>("Total Candidates").ToString() ?? "",

                }).ToList();

                DataSet ds1 = new DataSet();
                ds1 = await objTrainer.AllCourse();
                List<SelectListItem> AllCourseBind = new List<SelectListItem>();
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds1.Tables[0].Rows)
                    {
                        AllCourseBind.Add(new SelectListItem
                        {
                            Text = dr["CourseName"].ToString(),
                            Value = dr["Coursecode"].ToString()
                        });
                    }
                }

                DataSet ds2 = new DataSet();
                ds2 = await objTrainer.StatusListAB();
                List<SelectListItem> AllStatusBind = new List<SelectListItem>();
                if (ds2.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds2.Tables[0].Rows)
                    {
                        AllStatusBind.Add(new SelectListItem
                        {
                            Text = dr["Status"].ToString(),
                            Value = dr["StatusId"].ToString()
                        });
                    }
                }

                ViewBag.AllCourse = AllCourseBind;
                ViewBag.AllStatus = AllStatusBind;
                return View(batchAtt);
            }

            return View(new List<Trainer>());
        }



        /// <summary>
        /// pk example///
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> ProjectAttendancepk()
        {
            DataSet ds = await objTrainer.GetBatchAttendance();
            if (ds != null && ds.Tables.Count > 0)
            {
                List<Trainer> batchAtt = ds.Tables[0].AsEnumerable().Select(row => new Trainer
                {
                    BatchCode = row.Field<string>("BatchCode"),
                    BatchName = row.Field<string>("BatchName"),
                    BatchTime = row.Field<string>("BatchTime"),
                    TotalCandidate = row.Field<int?>("Total Candidates").ToString() ?? "",

                }).ToList();

                DataSet ds1 = new DataSet();
                ds1 = await objTrainer.AllCourse();
                List<SelectListItem> AllCourseBind = new List<SelectListItem>();
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds1.Tables[0].Rows)
                    {
                        AllCourseBind.Add(new SelectListItem
                        {
                            Text = dr["CourseName"].ToString(),
                            Value = dr["Coursecode"].ToString()
                        });
                    }
                }

                DataSet ds2 = new DataSet();
                ds2 = await objTrainer.StatusListAB();
                List<SelectListItem> AllStatusBind = new List<SelectListItem>();
                if (ds2.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds2.Tables[0].Rows)
                    {
                        AllStatusBind.Add(new SelectListItem
                        {
                            Text = dr["Status"].ToString(),
                            Value = dr["StatusId"].ToString()
                        });
                    }
                }

                ViewBag.AllCourse = AllCourseBind;
                ViewBag.AllStatus = AllStatusBind;
                return View(batchAtt);
            }

            return View(new List<Trainer>());
        }




    }

}