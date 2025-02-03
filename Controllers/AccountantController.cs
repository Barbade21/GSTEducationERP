

using GSTEducationERPLibrary.Account;
using GSTEducationERPLibrary.Accountant;
using GSTEducationERPLibrary.Bind;
using GSTEducationERPLibrary.Placement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace GSTEducationERP.Controllers
{
    public class AccountantController : Controller
    {
        BALAccountant objbal = new BALAccountant();
        Accountant objac = new Accountant();
        BALBind objBALbind = new BALBind();

        Accountant objprop = new Accountant();
        public class BreadcrumbItem
        {
            public string Name { get; set; }
            public string Url { get; set; }

        }
        private const string StaffListSessionKey = "FullStaffList";
        private const string StaffAttendanceListSessionKey = "FullStaffAttendanceList";
        private const string AdvancePayStaffListSessionKey = "AdvancePayStaffList";
        private const string AllStaffAttendanceListSessionKey = "AllFullStaffAttendanceList";
        //------------------------------------------------Somnath Hambire------------------------------------------------------------
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// This method is used for main view where all the partial views are called
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> studentFeeMainpageasyncSH()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                 {
                   //new BreadcrumbItem { Name = "Home", Url = "/" },
                   new BreadcrumbItem { Name = "Accountant Dashboard", Url = "AccountantDashboardAsyncSGS/Accountant" },
                   new BreadcrumbItem { Name = "Fee Collection", Url = "studentFeeMainpageasyncSH/Accountant" } // Adjust URL as needed
                 };

                ViewBag.Breadcrumbs = breadcrumbs;
                await ListAllCourseAsyncSH();
                return View("studentFeeMainpageasyncSH");
            }
        }

        /// <summary>
        /// This method gets list of internal students for fee collection
        /// </summary>
        /// <returns></returns>
        /// 


        [HttpGet]
        public async Task<ActionResult> ListInternalStudentasyncSH()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                List<Accountant> Mo = await InternalStudentFeeDataAsyncSH();
                Accountant obj = new Accountant();
                obj.lstStudentFees = Mo;
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                 {
                   //new BreadcrumbItem { Name = "Home", Url = "/" },
                   new BreadcrumbItem { Name = "Accountant Dashboard", Url = "AccountantDashboardAsyncSGS/Accountant" },
                   new BreadcrumbItem { Name = "Fee Collection", Url = "studentFeeMainpageasyncSH/Accountant" },
                   new BreadcrumbItem { Name = "Internal Student", Url = "ListInternalStudentasyncSH/Accountant" }
                 };

                ViewBag.Breadcrumbs = breadcrumbs;
                await ListAllCourseAsyncSH();
                return PartialView("_ListInternalStudentasyncSH", obj);
            }

        }
        /// <summary>
        /// This method gets list of All courses
        /// </summary>
        /// <returns></returns>
        public async Task ListAllCourseAsyncSH()
        {

            DataSet ds = await objbal.ListAllCourseSH();
            List<SelectListItem> CourseList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CourseList.Add(new SelectListItem { Text = dr["CourseName"].ToString(), Value = dr["Coursecode"].ToString() });
            }
            ViewBag.Course = CourseList;
        }
        /// <summary>
        /// This action is used to get list of released batch.
        /// </summary>
        /// <param name="CourseCode">Coursecode is passed to get coursewise batches.</param>
        /// <returns>It returns the coursewise batches.</returns>
        [HttpGet]
        public async Task<JsonResult> ListALLBatchSNAsync(string CourseCode)
        {

            DataSet ds = await objbal.ListAllBatchSH(CourseCode);
            List<SelectListItem> BatchList = new List<SelectListItem>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                BatchList.Add(new SelectListItem { Text = dr["BatchName"].ToString(), Value = dr["BatchCode"].ToString() });
            }

            return Json(BatchList, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method gets list of internal students for fee collection
        /// </summary>
        /// <returns></returns>
        public async Task<List<Accountant>> InternalStudentFeeDataAsyncSH()
        {

            string BranchCode = Session["BranchCode"].ToString();
            await ListAllCourseAsyncSH();
            DataSet ds = await objbal.ListofInternalStudentFeesAsyncSH(BranchCode);
            List<Accountant> lstData1 = new List<Accountant>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Accountant obju = new Accountant
                {
                    StudetCode = row["CandidateCode"]?.ToString() ?? string.Empty,
                    StudentType = row["CandidateType"]?.ToString() ?? string.Empty,
                    StudentName = row["FullName"]?.ToString() ?? string.Empty,
                    Email = row["EmailId"]?.ToString() ?? string.Empty,
                    Contact = row["ContactNumber"] != DBNull.Value ? (row["ContactNumber"]).ToString() : "0",
                    Course = row["CourseName"]?.ToString() ?? string.Empty,
                    Batch = row["BatchName"]?.ToString() ?? string.Empty,
                    CourseFee = row["CourseFee"] != DBNull.Value ? Convert.ToDouble(row["CourseFee"]) : 0.0,
                    RegistrationFee = row["RegistrationFees"] != DBNull.Value ? Convert.ToDouble(row["RegistrationFees"]) : 0.0,
                    Discount = row["Discount"] != DBNull.Value ? Convert.ToInt32(row["Discount"]) : 0,
                    TotalFeesSH = row["TotalFees"] != DBNull.Value ? Convert.ToDouble(row["TotalFees"]) : 0,
                    PaidFees = row["TotalPaid"] != DBNull.Value ? Convert.ToDouble(row["TotalPaid"]) : 0.0,
                    RemainingFees = row["RemainingFees"] != DBNull.Value ? Convert.ToDouble(row["RemainingFees"]) : 0.0,
                    InstallmentAmount = row["NextInstallment"] != DBNull.Value ? Convert.ToDouble(row["NextInstallment"]) : 0.0,
                    LastInstallmentDate = row["LastInstallmentDate"] != DBNull.Value ? Convert.ToDateTime(row["LastInstallmentDate"]) : DateTime.MinValue,
                    NextInstallmentDate = row["NextInstallmentDate"] != DBNull.Value ? Convert.ToDateTime(row["NextInstallmentDate"]) : DateTime.MinValue,
                    Status = row["FeeStatus"] == DBNull.Value || string.IsNullOrEmpty(row["FeeStatus"]?.ToString()) ? "Not Paid" : row["FeeStatus"].ToString(),
                    NumberOfPaidInstallments = row["PaidInstallments"] != DBNull.Value ? Convert.ToInt32(row["PaidInstallments"]) : 0,
                    NoOfInstallments = row["NoofInstallment"] != DBNull.Value ? Convert.ToInt32(row["NoofInstallment"]) : 0,
                    AdmissionDate = row["AdmmissionDate"] != DBNull.Value ? Convert.ToDateTime(row["AdmmissionDate"]) : DateTime.MinValue,
                    LastInstallmentDates = row["LastInstallmentDate"] != DBNull.Value ? Convert.ToDateTime(row["LastInstallmentDate"]).ToString("dd/MM/yyyy") : "Not Available",
                    NextInstallmentDates = row["NextInstallmentDate"] != DBNull.Value ? Convert.ToDateTime(row["NextInstallmentDate"]).ToString("dd/MM/yyyy") : string.Empty,
                    BranchAddress = row["BranchAddress"]?.ToString() ?? string.Empty,

                };

                if (obju.Status == "Not Paid")
                {
                    obju.Status = "Pending";
                }


                DataSet dsStudFee = await objbal.GetFeesDataForSingleStudent(BranchCode, obju.StudetCode);

                foreach (DataRow row1 in dsStudFee.Tables[0].Rows)
                {
                    try
                    {
                        objac.BatchStartDate = DateTime.Parse(row1["StartDate"].ToString());
                        int givenNoOfInstallment = Convert.ToInt32(row1["NoofInstallment"]);
                        double referamount = Convert.ToDouble(row1["ReferenceAmount"]);
                        double totalFees = Convert.ToDouble(row1["TotalFee"]) - referamount;
                        double totalPaid = Convert.ToDouble(row1["TotalTransactionAmount"]);
                        objac.Duration = Convert.ToInt32(row1["InstallmentDuration"]);
                        //   obju.RemainingFees = Convert.ToDouble(row1["TotalTransactionAmount"]);
                        obju.TotalFeesSH = Convert.ToDouble(row1["TotalFee"]) - referamount;
                        obju.PaidFees = Convert.ToDouble(row1["TotalTransactionAmount"]);
                        obju.RegistrationFee = referamount;
                        obju.RemainingFees = obju.RemainingFees - referamount;
                        obju.FixedInstallmentAmount = Math.Round(obju.TotalFeesSH / givenNoOfInstallment, 2);
                        // Calculate unpaid installments
                        List<Accountant> upcomingInstallments = GetUnpaidInstallments(totalFees, totalPaid, givenNoOfInstallment);

                        // Initialize flags and variables for the next installment
                        bool foundNextInstallment = false;
                        double remainingTotalPaid = totalPaid;
                        double TotalCompleted = 0;


                        // Loop through each installment to find the next due one
                        foreach (var installment in upcomingInstallments)
                        {
                            if (remainingTotalPaid <= installment.TotalCompletedAmount)
                            {
                                // The first unpaid installment where remainingTotalPaid is less than TotalCompletedAmount
                                double nextInstallmentAmount = installment.TotalCompletedAmount - remainingTotalPaid;

                                // Set the next installment date and amount
                                obju.NextInstallmentDates = installment.InstallmentDate.ToString("dd-MM-yyyy");
                                obju.InstallmentAmount = Math.Round(nextInstallmentAmount, 2);
                                //  obju.InstallmentNo = installment.InstallmentNo;

                                TotalCompleted = installment.TotalCompletedAmount;
                                // Mark that we've found the next installment
                                foundNextInstallment = true;
                                if (installment.InstallmentDate < DateTime.Now)
                                {
                                    continue;
                                }
                                else
                                {
                                    break;
                                }

                            }
                        }

                        List<Accountant> InstallmentNo = CalculateInstallments(totalFees, givenNoOfInstallment);

                        foreach (var installment in InstallmentNo)
                        {
                            if (remainingTotalPaid >= installment.TotalCompletedAmount)
                            {
                                obju.InstallmentNo = installment.InstallmentNo;
                            }

                            if (remainingTotalPaid < installment.TotalCompletedAmount)
                            {
                                obju.InstallmentNo = installment.InstallmentNo;
                                break;
                            }

                        }

                        // If totalPaid >= totalFees, set the fee as completed
                        if (totalPaid >= totalFees || !foundNextInstallment)
                        {
                            obju.NextInstallmentDates = "Fees Completed";
                            obju.LastInstallmentDates = "Fees Completed";
                            obju.InstallmentAmount = 0;
                            obju.InstallmentNo = obju.InstallmentNo + 1;
                        }

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error processing row: " + ex.Message);
                        continue;
                    }
                }






                lstData1.Add(obju);
            }

            Session["ListforIntFilter"] = lstData1;
            return lstData1;
        }


        /// <summary>
        /// This method gets list of external students for fee collection
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ListOfExternalStudentasyncSH()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                List<Accountant> Mo = await ExternalStudentFeesDetails();
                Accountant obj = new Accountant();
                obj.lstStudentFees = Mo;
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
             {
                   //new BreadcrumbItem { Name = "Home", Url = "/" },
                   new BreadcrumbItem { Name = "Accountant Dashboard", Url = "AccountantDashboardAsyncSGS/Accountant" },
                   new BreadcrumbItem { Name = "Fee Collection", Url = "studentFeeMainpageasyncSH/Accountant" },
                   new BreadcrumbItem { Name = "External Student", Url = "ListOfExternalStudentasyncSH/Accountant" }
             };

                ViewBag.Breadcrumbs = breadcrumbs;

                return PartialView("_ListExternalStudentasyncSH", obj);
            }

        }
        /// <summary>
        /// This method gets list of external students for fee collection
        /// </summary>
        /// <returns></returns>
        public async Task<List<Accountant>> ExternalStudentFeesDetails()
        {
            DataSet ds = await objbal.ListofExternalStudentFeesAsyncSH();
            List<Accountant> lstData1 = new List<Accountant>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Accountant obju = new Accountant
                {

                    StudetCode = row["CandidateCode"]?.ToString() ?? string.Empty,
                    StudentName = row["FullName"]?.ToString() ?? string.Empty,
                    Email = row["EmailId"]?.ToString() ?? string.Empty,
                    Contact = row["ContactNumber"] != DBNull.Value ? (row["ContactNumber"]).ToString() : "0",
                    OneMonthCTC = row["OneMonthCTC"] != DBNull.Value ? Convert.ToDouble(row["OneMonthCTC"]) : 0.0,
                    PayableAmmount = row["PayableAmount"] != DBNull.Value ? Convert.ToDouble(row["PayableAmount"]) : 0.0,
                    Status = row["Fee Status"] == DBNull.Value || string.IsNullOrEmpty(row["Fee Status"]?.ToString()) ? "Not Paid" : row["Fee Status"].ToString(),
                    AdmissionDate = (DateTime)row["AdmmissionDate"]
                };
                lstData1.Add(obju);
            }
            Session["ListforExtFilter"] = lstData1;

            return lstData1;
        }
        /// <summary>
        /// This Method shows each student's Fee Details
        /// </summary>
        /// <param name="StudentCode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> StudentFeeDetailsAsyncSH(string StudentCode)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {

                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
          {
            //new BreadcrumbItem { Name = "Home", Url = "/" },
            new BreadcrumbItem { Name = "Accountant Dashboard", Url = "AccountantDashboardAsyncSGS/Accountant" },
            new BreadcrumbItem { Name = "Fee Collection", Url = "studentFeeMainpageasyncSH/Accountant" },
            new BreadcrumbItem { Name = "Student Details", Url = "StudentFeeDetailsAsyncSH/Accountant" }
          };
                string BranchCode = Session["BranchCode"].ToString();
                ViewBag.Breadcrumbs = breadcrumbs;
                DataSet ds = await objbal.ListofIntStudentFeesDetailsAsyncSH(StudentCode, BranchCode);
                List<Accountant> lstData1 = new List<Accountant>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Accountant obju = new Accountant
                    {
                        StudetCode = row["CandidateCode"].ToString(),
                        StudentType = row["CandidateType"]?.ToString() ?? string.Empty,

                        StudentName = row["FullName"].ToString(),
                        Contact = row["ContactNumber"] != DBNull.Value ? (row["ContactNumber"]).ToString() : "0",
                        Course = row["CourseName"].ToString(),
                        Batch = row["BatchName"].ToString(),
                        FeesType = row["FeesType"].ToString(),
                        RegistrationFee = row["RegistrationFees"] != DBNull.Value ? Convert.ToDouble(row["RegistrationFees"]) : 0.0,
                        CourseFee = row["CourseFee"] != DBNull.Value ? Convert.ToDouble(row["CourseFee"]) : 0.0,
                        TotalFees = row["TotalFee"] != DBNull.Value ? Convert.ToDecimal(row["TotalFee"]) : 0,
                        PaidFees = row["CumulativePaidFees"] != DBNull.Value ? Convert.ToDouble(row["CumulativePaidFees"]) : 0.0,
                        RemainingFees = row["RemainingFees"] != DBNull.Value ? Convert.ToDouble(row["RemainingFees"]) : 0.0,
                        ReceiptNo = row["FeesCollectioncode"].ToString(),
                        TransactionDate = row["TransactionDate"] != DBNull.Value ? Convert.ToDateTime(row["TransactionDate"]) : DateTime.MinValue,

                        TransactionAmount = row["TransactionAmount"] != DBNull.Value ? Convert.ToDecimal(row["TransactionAmount"]) : 0,
                        PaymentMode = row["PaymentMode"].ToString(),
                        TransactionId = row["TransactionID_checqueNumber"] != DBNull.Value ? (row["TransactionID_checqueNumber"]).ToString() : "-",
                        BankName = row["BankName"] != DBNull.Value ? (row["BankName"]).ToString() : "-",
                        CheaqueDate = row["ChequeDate"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["ChequeDate"].ToString()) ? Convert.ToDateTime(row["ChequeDate"]).ToString("dd/MM/yyyy") : "-",
                        Discount = row["Discount"] != DBNull.Value ? Convert.ToInt32(row["Discount"]) : 0,
                        NumberOfPaidInstallments = row["PaidInstallments"] != DBNull.Value ? Convert.ToInt32(row["PaidInstallments"]) : 0,
                        NoOfInstallments = row["NoofInstallment"] != DBNull.Value ? Convert.ToInt32(row["NoofInstallment"]) : 0,


                    };

                    lstData1.Add(obju);
                }

                Accountant obj = new Accountant();
                obj.lstStudentFeesDetails = lstData1;


                return PartialView("StudentFeeDetailsAsyncSH", obj);
            }

        }
        /// <summary>
        /// This method gets filtered list of students for fee collection
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> FilterFeeCollection(string TabID, string Course, string Batch, DateTime? startDate, DateTime? endDate, string dateFilterType)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                try
                {
                    if (TabID == "internal")
                    {
                        List<Accountant> StudentIntFees = Session["ListforIntFilter"] as List<Accountant>;

                        // Filter by Course
                        if (!string.IsNullOrEmpty(Course) && Course != "---Select Course---")
                        {
                            StudentIntFees = StudentIntFees.Where(p => p.Course == Course).ToList();
                        }

                        // Filter by Batch
                        if (!string.IsNullOrEmpty(Batch) && Batch != "---Select Batch---")
                        {
                            StudentIntFees = StudentIntFees.Where(p => p.Batch == Batch).ToList();
                        }

                        // Apply date range filtering based on the selected date type
                        if (!string.IsNullOrEmpty(dateFilterType))
                        {
                            if (dateFilterType == "NextInstallmentDate")
                            {
                                // Filter by NextInstallmentDate within the date range
                                StudentIntFees = StudentIntFees.Where(p =>
                                {
                                    DateTime nextInstallmentDate;
                                    if (DateTime.TryParse(p.NextInstallmentDates, out nextInstallmentDate))
                                    {
                                        return (!startDate.HasValue || nextInstallmentDate >= startDate.Value) &&
                                               (!endDate.HasValue || nextInstallmentDate <= endDate.Value);
                                    }
                                    return false;
                                }).ToList();
                            } else if (dateFilterType == "LastInstallmentDate")
                            {
                                // Filter by LastInstallmentDate within the date range
                                StudentIntFees = StudentIntFees.Where(p =>
                                {
                                    DateTime lastInstallmentDate;
                                    if (DateTime.TryParse(p.LastInstallmentDates, out lastInstallmentDate))
                                    {
                                        return (!startDate.HasValue || lastInstallmentDate >= startDate.Value) &&
                                               (!endDate.HasValue || lastInstallmentDate <= endDate.Value);
                                    }
                                    return false;
                                }).ToList();
                            }
                        }

                        // Prepare the model for view
                        Accountant obj = new Accountant { lstStudentFees = StudentIntFees };
                        return await Task.Run(() => PartialView("_ListInternalStudentasyncSH", obj));
                    } else if (TabID == "external")
                    {
                        List<Accountant> StudentExtFees = Session["ListforExtFilter"] as List<Accountant>;

                        // Filter by AdmissionDate within the date range
                        StudentExtFees = StudentExtFees.Where(p =>
                        {
                            DateTime admissionDate = p.AdmissionDate; // Assuming DateTime
                            return (!startDate.HasValue || admissionDate >= startDate.Value) &&
                                   (!endDate.HasValue || admissionDate <= endDate.Value);
                        }).ToList();

                        Accountant obj = new Accountant { lstStudentFees = StudentExtFees };
                        return await Task.Run(() => PartialView("_ListExternalStudentasyncSH", obj));
                    } else if (TabID == "Pending")
                    {
                        List<Accountant> StudentPendingFees = Session["PendingFilterList"] as List<Accountant>;

                        // Filter by Course
                        if (!string.IsNullOrEmpty(Course) && Course != "---Select Course---")
                        {
                            StudentPendingFees = StudentPendingFees.Where(p => p.CourseName == Course).ToList();
                        }

                        // Filter by Batch
                        if (!string.IsNullOrEmpty(Batch) && Batch != "---Select Batch---")
                        {
                            StudentPendingFees = StudentPendingFees.Where(p => p.Batch == Batch).ToList();
                        }

                        // Apply date range filtering based on the selected date type
                        if (!string.IsNullOrEmpty(dateFilterType))
                        {

                            // Filter by NextInstallmentDate within the date range
                            StudentPendingFees = StudentPendingFees.Where(p =>
                            {
                                DateTime admissionDate = p.InstallmentDate; // Assuming DateTime
                                return (!startDate.HasValue || admissionDate >= startDate.Value) &&
                                       (!endDate.HasValue || admissionDate <= endDate.Value);
                            }).ToList();
                        }

                        // Prepare the model for view
                        Accountant obj = new Accountant { LstPendindFeeStud = StudentPendingFees };
                        return await Task.Run(() => PartialView("PendingInstallmentsListAsyncAD", obj));
                    } else
                    {
                        return await Task.Run(() => View("error"));
                    }
                } catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return await Task.Run(() => View("error"));
                }
            }
        }



        //[HttpGet]
        //public async Task<ActionResult> InternalStudentFeesDetailsAD()
        //{
        //    return await Task.Run(() => View());

        //}



        [HttpGet]
        public async Task<ActionResult> InternalStudentFeesDetailsAD(string StudentCode, string StudentName, string StudentType, string CourseFee, string Contact,
   string Discount, string Course, string TotalFees, string Batch, string PaidFees, string NoOfInstallments, string RemainingFees, string Receiptcode,
     DateTime TransactionDate,
     string FeesType,
     double TransactionAmount,
     string paymentMode,
     string TransactionId,
     string BankName,
     DateTime Chequedate,
     string ChequeClearanceDateAD,
     string BankIdAD)
        {

            List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
             {
                   new BreadcrumbItem { Name = "Home", Url = "studentFeeMainpageasyncSH/Accountant" }
             };

            ViewBag.Breadcrumbs = breadcrumbs;

            objac.BankIdAD = BankIdAD;

            objac.BranchCode = Session["BranchCode"].ToString();
            string logo = "";
            string Address = "";
            SqlDataReader drClient;


            drClient = await objbal.ListClientDetailsAsyncVP(objac);
            if (drClient.Read())
            {
                logo = drClient["Logo"].ToString();
                Address = drClient["BranchAddress"].ToString();
            }

            DataSet drb;


            if (objac.BankIdAD != null && objac.BankIdAD != "")
            {
                drb = await objbal.GetBank(objac);
            } else
            {
                drb = null;
            }

            Accountant obju = new Accountant();


            if (drb != null && drb.Tables.Count > 0 && drb.Tables[0].Rows.Count > 0)
            {

                obju.AccountHolderName = drb.Tables[0].Rows[0]["AccountHolderName"].ToString();

            } else
            {
                obju.AccountHolderName = string.Empty; // Set a default value to avoid null reference
            }
            // Create a view model or object to store the received data
            var studentFeeDetail = new Accountant
            {
                StudetCode = StudentCode,
                StudentType = StudentType,
                StudentName = StudentName,
                CourseFee = Convert.ToDouble(CourseFee),
                Contact = Contact,
                Discount = Convert.ToDecimal(Discount),
                Course = Course,
                TotalFees = Convert.ToDecimal(TotalFees),
                Batch = Batch,
                PaidFees = Convert.ToDouble(PaidFees),
                NoOfInstallments = Convert.ToInt32(NoOfInstallments),
                RemainingFees = Convert.ToDouble(RemainingFees),
                ReciptCode = Receiptcode,
                ReceiptDateAD = TransactionDate.ToString("dd-MM-yyyy"),
                FeeType = FeesType,
                TransactionAmount = (decimal?)TransactionAmount,
                PaymentMode = paymentMode,
                TransactionId = TransactionId,
                BankName = BankName,
                ChequeDate = Chequedate,
                ChequeClearanceDateAD = ChequeClearanceDateAD,
                Logo = logo,
                BranchAddress = Address,
                DrawnOn = obju.AccountHolderName,
            };
            //    studentFeeDetail.DrawnOn = obju.AccountHolderName;
            studentFeeDetail.Logo = logo;
            studentFeeDetail.BranchAddress = Address;
            return await Task.Run(() => View(studentFeeDetail));
        }



        //----------------------------------------------------------------------------------------------------  Atharva Deshmukhe    --------------------------------------------------------------------------------------------------------





        public async Task<ActionResult> PendingInstallmentsAsyncAD()
        {

            List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                        {
                          //new BreadcrumbItem { Name = "Home", Url = "/" },
                          new BreadcrumbItem { Name = "Accountant Dashboard", Url = "AccountantDashboardAsyncSGS/Accountant" },
                          new BreadcrumbItem { Name = "Fee Collection", Url = "studentFeeMainpageasyncSH/Accountant" },
                        };
            ViewBag.Breadcrumbs = breadcrumbs;
            return await Task.Run(() => View());
        }

        /// <summary>
        /// Used To Bind Bankname to Dropdown 
        /// </summary>
        /// <returns></returns>
        public async Task BindBankDropdown()
        {
            BALAccountant objbal1 = new BALAccountant();
            DataSet Bank = await objbal1.GetAllBank();
            List<SelectListItem> BankList = new List<SelectListItem>();

            foreach (DataRow row in Bank.Tables[0].Rows)
            {
                BankList.Add(new SelectListItem
                {
                    Text = row["BankName"].ToString(),
                    Value = row["BankId"].ToString(),
                });
            }
            ViewBag.Bank = new SelectList(BankList, "Value", "Text");
        }


        /// <summary>
        /// binding Account Holder name to dropdown of selected BankName
        /// </summary>
        /// <param name="BankId"></param>
        /// <returns></returns>
        public async Task<JsonResult> AccountHolder(int BankId)
        {
            BALAccountant objbal1 = new BALAccountant();
            Accountant accountant = new Accountant();
            accountant.BankId = BankId;
            DataSet AccountHolder = await objbal1.GetAllAccountHolder(accountant);

            List<SelectListItem> AccountHolderList = new List<SelectListItem>();
            foreach (DataRow row in AccountHolder.Tables[0].Rows)
            {
                AccountHolderList.Add(new SelectListItem
                {
                    Text = row["AccountHolderName"].ToString() + " - " + row["AccountNumber"].ToString(),
                    Value = row["BankId"].ToString(),

                });
            }
            ViewBag.AccountHolderList = AccountHolderList;
            return Json(AccountHolderList, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Binding the FeeType to DropDowns
        /// </summary>
        /// <returns></returns>
        public async Task BindFeeTypeDropdown()
        {
            BALAccountant objbal1 = new BALAccountant();
            DataSet FeeType = await objbal1.GetFeeTypeBank();
            List<SelectListItem> FeeTypeList = new List<SelectListItem>();

            foreach (DataRow row in FeeType.Tables[0].Rows)
            {
                FeeTypeList.Add(new SelectListItem
                {
                    Text = row["FeesType"].ToString(),
                    Value = row["FeesTypeId"].ToString(),

                });
            }
            ViewBag.FeesType = new SelectList(FeeTypeList, "Value", "Text");
        }


        [HttpGet]
        public async Task<ActionResult> PendingInstallmentCollectingFeeAsyncAD(string candidateCode, string name, string batch, string courseName, double courseFee, decimal totalFees, double totalPaid, double remainingFees, double installmentAmount, string installmentDate, string DiscountPercentage, string StudentType, string BranchAddress, double MCAmount)
        {
            DateTime parsedInstallmentDate;
            // Attempt to parse the string to a DateTime
            parsedInstallmentDate = DateTime.Parse(installmentDate);
            // Assign parsed DateTime to InstallmentDate property
            var viewModel = new Accountant
            {
                CandidateCode = candidateCode,
                Name = name,
                Batch = batch,
                CourseName = courseName,
                CourseFee = courseFee,
                TotalFees = totalFees,
                TotalPaid = totalPaid,
                InstallmentAmount = installmentAmount,
                InstallmentDate = parsedInstallmentDate,
                DiscountPercentage = DiscountPercentage,
                RemainingFees = remainingFees,
                StudentType = StudentType,
                BranchAddress = BranchAddress,
                MustCompleteAmount = MCAmount,

            };
            await BindFeeTypeDropdown();
            await BindBankDropdown();

            viewModel.StaffCode = Session["StaffCode"].ToString();
            DataSet ds2 = await objbal.BankAccountforVoucherAsyncSGS(viewModel);
            ViewBag.BankAccountList = ds2.Tables[0].AsEnumerable().Select(dr => new SelectListItem
            {
                Text = $"{dr["BankName"]} - {dr["AccountHolderName"]} - {dr["AccountNumber"]}",
                Value = dr["BankId"].ToString()
            }).ToList();


            List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                 {
                   //new BreadcrumbItem { Name = "Home", Url = "/" },
                   new BreadcrumbItem { Name = "Accountant Dashboard", Url = "AccountantDashboardAsyncSGS/Accountant" },
                   new BreadcrumbItem { Name = "Fee Collection", Url = "studentFeeMainpageasyncSH/Accountant" },
                   new BreadcrumbItem { Name = "Collect Installments" }, // Adjust URL as needed
                 };
            ViewBag.Breadcrumbs = breadcrumbs;

            return PartialView("PendingInstallmentCollectingFeeAsyncAD", viewModel);
        }




        [HttpGet]
        public async Task<ActionResult> RecietOfCollectedFeesAsyncAD()
        {
            return await Task.Run(() => View());

        }


        [HttpGet]
        public async Task<ActionResult> ProvisionalReceiptAsyncAD()
        {
            return await Task.Run(() => View());
        }



        /// <summary>
        /// Giving Receipt of Collected fee
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //[HttpPost]


        [HttpPost]
        public async Task<ActionResult> RecietOfCollectedFeesAsyncAD(Accountant model)
        {
            objac.BranchCode = Session["BranchCode"].ToString();
            string logo = "";
            string Address = "";
            List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
          {
                new BreadcrumbItem { Name = "Home", Url = "studentFeeMainpageasyncSH/Accountant" }
          };

            ViewBag.Breadcrumbs = breadcrumbs;


            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                SqlDataReader drClient;
                drClient = await objbal.ListClientDetailsAsyncVP(objac);
                if (drClient.Read())
                {
                    logo = drClient["Logo"].ToString();
                    Address = drClient["BranchAddress"].ToString();
                }

                string staffCode = Session["StaffCode"].ToString();
                model.BranchCode = Session["BranchCode"].ToString();
                BALAccountant bALAccount = new BALAccountant();
                DataSet ds = await bALAccount.ReciptCodeAD(model);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    model.ReciptCode = ds.Tables[0].Rows[0]["NewFeesCollectioncode"].ToString();
                }

                DateTime today = DateTime.Today;
                model.ReceiptDateAD = today.ToString("dd-MM-yyyy");
                model.RemainingFees = (double)model.TotalFees - model.TotalPaid - model.InstallmentAmount;

                DataSet ds1 = await bALAccount.BankHolderNameAD(model);

                if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    model.DrawnOn = ds1.Tables[0].Rows[0]["AccountHolderName"].ToString();
                }

                Accountant feesCollection = new Accountant
                {
                    CandidateCode = model.CandidateCode,
                    Name = model.Name,
                    Description = model.Description,
                    CourseName = model.CourseName,
                    CourseFee = model.CourseFee,
                    BankId = model.BankId,
                    FeeTypeId = model.FeeTypeId,
                    TransactionID_checqueNumber = model.TransactionID_checqueNumber,
                    InstallmentDate = model.InstallmentDate,
                    AccountHolderName = model.AccountHolderName,
                    PaymentModeId = model.PaymentModeId,
                    PaymentType = model.PaymentType,
                    AccountHolderId = model.AccountHolderId,
                    ChequeDate = model.ChequeDate,
                    Amount = (decimal)model.InstallmentAmount,
                    StaffCode = staffCode,
                    BranchCode = model.BranchCode,
                    ReciptCode = model.ReciptCode,
                    ChequeBankName = model.ChequeBankName,
                    DrawnOn = model.DrawnOn,
                    RemainingFees = model.RemainingFees,
                    ReceiptDateAD = model.ReceiptDateAD,
                    CheckBankName = model.CheckBankName,
                    CheckAccountNumber = model.CheckAccountNumber,
                    NextInstallmentAmount = model.NextInstallmentAmount,
                    NextInstallmentDates = model.NextInstallmentDates,
                    TotalFees = model.TotalFees,
                    Logo = logo,
                    BranchAddress = Address,
                    TransactionDate = model.TransactionDate
                };
                await bALAccount.FeeCollectionAsync(feesCollection);

                DataSet dsStudFee = await bALAccount.GetFeesDataForSingleStudent(model.BranchCode, model.CandidateCode);

                foreach (DataRow row in dsStudFee.Tables[0].Rows)
                {
                    try
                    {
                        objac.BatchStartDate = DateTime.Parse(row["StartDate"].ToString());
                        int givenNoOfInstallment = Convert.ToInt32(row["NoofInstallment"]);


                        objac.Duration = Convert.ToInt32(row["InstallmentDuration"]);
                        objac.BatchStartDate = DateTime.Parse(row["StartDate"].ToString());

                        double referamount = Convert.ToDouble(row["ReferenceAmount"]);
                        double totalPaid = Convert.ToDouble(row["TotalTransactionAmount"]);
                        double totalFees = Convert.ToDouble(row["TotalFee"]) - referamount;
                        //   obju.RemainingFees = Convert.ToDouble(row1["TotalTransactionAmount"]);
                        objac.PaidFees = Convert.ToDouble(row["TotalTransactionAmount"]);
                        objac.RegistrationFee = referamount;
                        objac.RemainingFees = objac.RemainingFees - referamount;

                        // Calculate unpaid installments
                        List<Accountant> upcomingInstallments = GetUnpaidInstallments(totalFees, totalPaid, givenNoOfInstallment);

                        // Initialize flags and variables for the next installment
                        bool foundNextInstallment = false;
                        double remainingTotalPaid = totalPaid;
                        double TotalCompleted = 0;


                        // Loop through each installment to find the next due one
                        foreach (var installment in upcomingInstallments)
                        {
                            if (remainingTotalPaid <= installment.TotalCompletedAmount)
                            {
                                // The first unpaid installment where remainingTotalPaid is less than TotalCompletedAmount
                                double nextInstallmentAmount = installment.TotalCompletedAmount - remainingTotalPaid;

                                // Set the next installment date and amount
                                model.NextInstallmentDates = installment.InstallmentDate.ToString("dd-MM-yyyy");
                                model.NextInstallmentAmount = Math.Round(nextInstallmentAmount, 2);
                                model.InstallmentNo = installment.InstallmentNo;

                                TotalCompleted = installment.TotalCompletedAmount;
                                // Mark that we've found the next installment
                                foundNextInstallment = true;


                                //if (installment.InstallmentDate < DateTime.Now)
                                //{
                                //    continue;
                                //}

                                if (installment.InstallmentDate > DateTime.Now && installment.TotalCompletedAmount <= remainingTotalPaid)
                                {
                                    continue;
                                } else if (installment.InstallmentDate < DateTime.Now)
                                {
                                    continue;
                                } else
                                {
                                    break;
                                }

                            }
                        }

                        List<Accountant> InstallmentNo = CalculateInstallments(totalFees, givenNoOfInstallment);

                        foreach (var installment in InstallmentNo)
                        {
                            if (remainingTotalPaid >= installment.TotalCompletedAmount)
                            {
                                model.InstallmentNo = installment.InstallmentNo;
                            }

                            if (remainingTotalPaid < installment.TotalCompletedAmount)
                            {
                                model.InstallmentNo = installment.InstallmentNo;
                                break;
                            }

                        }

                        // If totalPaid >= totalFees, set the fee as completed
                        if (totalPaid >= totalFees || !foundNextInstallment)
                        {
                            model.NextInstallmentDates = "Fee Completed";
                            model.NextInstallmentAmount = 0;
                            model.InstallmentNo = model.InstallmentNo + 1;
                        }

                    } catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error processing row: " + ex.Message);
                        continue;
                    }
                }
                model.Logo = logo;
                model.BranchAddress = Address;
                return await Task.Run(() => View(model));
            }
        }

        /// <summary>
        /// CalculateInstallments and  GetUnpaidInstallments are coded to to get installmentamount and installmentDate
        /// </summary>
        /// <param name="TotalFees"></param>
        /// <param name="GivenNoOfInstallment"></param>
        /// <returns></returns>
        public List<Accountant> CalculateInstallments(double TotalFees, double GivenNoOfInstallment)
        {
            List<Accountant> installments = new List<Accountant>();

            // Calculate the installment amount
            double installmentAmount = TotalFees / GivenNoOfInstallment;
            //decimal installmentNo = 0;
            // Initialize the first installment date
            DateTime currentInstallmentDate = objac.BatchStartDate.AddDays(objac.Duration);
            double totalCompletedAmount = 0;

            // Loop through the number of installments
            for (int i = 0; i < GivenNoOfInstallment; i++)
            {
                totalCompletedAmount += installmentAmount;

                // Add the current installment to the list
                installments.Add(new Accountant
                {
                    InstallmentDate = currentInstallmentDate,
                    InstallmentAmount = installmentAmount,
                    TotalCompletedAmount = totalCompletedAmount,
                    InstallmentNo = i
                });

                // Calculate the next installment date
                currentInstallmentDate = currentInstallmentDate.AddDays(objac.Duration);
            }
            return installments;
        }


        public List<Accountant> GetUnpaidInstallments(double totalFees, double totalPaid, double givenNoOfInstallment)
        {
            List<Accountant> allInstallments = CalculateInstallments(totalFees, givenNoOfInstallment);
            List<Accountant> unpaidInstallments = new List<Accountant>();
            objac.TotalPaid = totalPaid;
            double remainingTotalPaid = objac.TotalPaid; // Keep track of remaining paid amount

            foreach (var installment in allInstallments)
            {
                if (remainingTotalPaid >= installment.TotalCompletedAmount)
                {
                    // If remainingTotalPaid is greater than or equal to TotalCompletedAmount, consider it paid
                    remainingTotalPaid -= installment.TotalCompletedAmount;

                    if (remainingTotalPaid > 0)
                    {
                        unpaidInstallments.Add(new Accountant
                        {
                            InstallmentDate = installment.InstallmentDate,
                            InstallmentAmount = remainingTotalPaid,
                            TotalCompletedAmount = installment.TotalCompletedAmount,
                            InstallmentNo = installment.InstallmentNo
                        }); ;
                    }
                    remainingTotalPaid = 0; // Reset remainingTotalPaid after processing
                } else
                {
                    // Calculate the remaining amount to be paid
                    double remainingAmount = installment.TotalCompletedAmount - remainingTotalPaid;

                    // If the remaining amount is greater than zero, it's due
                    if (remainingAmount > 0)
                    {
                        unpaidInstallments.Add(new Accountant
                        {
                            InstallmentDate = installment.InstallmentDate,
                            InstallmentAmount = remainingAmount,
                            TotalCompletedAmount = installment.TotalCompletedAmount,
                            InstallmentNo = installment.InstallmentNo
                        });
                    }
                    remainingTotalPaid = 0; // Reset remainingTotalPaid after processing
                }
            }
            return unpaidInstallments;
        }


        /// <summary>
        /// Passing data to PendingInstallments tab
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> PendingInstallmentsListAsyncAD()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login page if staff code is not found in session
            } else
            {
                try
                {
                    string staffCode = Session["StaffCode"].ToString();
                    string BranchCode = Session["BranchCode"].ToString();
                    DataSet ds = await objbal.GetDataForNewQuery(BranchCode);
                    Accountant objDetails = new Accountant();
                    List<Accountant> lstData1 = new List<Accountant>();
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        try
                        {
                            objac.BatchStartDate = DateTime.Parse(row["StartDate"].ToString());
                            int givenNoOfInstallment = Convert.ToInt32(row["NoofInstallment"]);
                            double totalFees = Convert.ToDouble(row["FinalFee"]);
                            double totalPaid = Convert.ToDouble(row["TotalTransactionAmount"]);
                            objac.Duration = Convert.ToInt32(row["InstallmentDuration"]);

                            // Skip entry if TotalPaid is greater than or equal to TotalFees
                            if (totalPaid >= totalFees)
                            {
                                continue;
                            }

                            double remainingFees = totalFees - totalPaid;
                            List<Accountant> upcomingInstallments = GetUnpaidInstallments(totalFees, totalPaid, givenNoOfInstallment);

                            Accountant obju = new Accountant();
                            double remainingTotalPaid = totalPaid;
                            double accumulatedUnpaidAmount = 0;
                            double MustCompleteAmount = 0;
                            DateTime? previousInstallmentDate = null; // To store the previous installment date
                            DateTime? currentInstallmentDate = null; // To store the current installment date

                            foreach (Accountant installment in upcomingInstallments)
                            {
                                currentInstallmentDate = installment.InstallmentDate;

                                if (installment.TotalCompletedAmount > remainingTotalPaid)
                                {
                                    double unpaidAmount = installment.TotalCompletedAmount - remainingTotalPaid;
                                    //remainingTotalPaid = 0; // Reset remaining total paid as we've accounted for it
                                    accumulatedUnpaidAmount = unpaidAmount;

                                    MustCompleteAmount = installment.TotalCompletedAmount;

                                    // Check if the installment date has passed
                                    if (installment.InstallmentDate <= DateTime.Now)
                                    {
                                        // Include the installment if the date has passed
                                        obju.InstallmentDate = installment.InstallmentDate;
                                        obju.InstallmentAmount = Math.Round(accumulatedUnpaidAmount);

                                        // Assigning other properties to obju
                                        obju.CandidateCode = row["CandidateCode"].ToString();
                                        obju.Name = row["FullName"].ToString();
                                        obju.ContactNumber = row["ContactNumber"].ToString();
                                        obju.CourseName = row["CourseName"].ToString();
                                        obju.Batch = row["BatchName"].ToString();
                                        obju.CourseFee = Convert.ToDouble(row["CourseFee"]);
                                        obju.DiscountPercentage = row["DiscountPercentage"].ToString();
                                        obju.TotalFees = (decimal)totalFees;
                                        obju.TotalPaid = totalPaid;
                                        obju.RemainingFees = remainingFees;
                                        obju.StudentType = row["studentType"].ToString();
                                        obju.BranchAddress = row["BranchAddress"].ToString();
                                        obju.MustCompleteAmount = MustCompleteAmount;
                                        Session["Logo"] = row["Logo"].ToString();
                                        // Add to lstData1 only if not already added (to avoid duplicate entries)

                                        obju.NoOfInstallments = Convert.ToInt32(row["NoofInstallment"]);
                                        obju.InstallmentNo = installment.InstallmentNo;
                                        obju.NextInstallmentDate = installment.InstallmentDate;



                                        if (!lstData1.Any(x => x.CandidateCode == obju.CandidateCode && x.InstallmentDate == obju.InstallmentDate))
                                        {
                                            lstData1.Add(obju);
                                        }
                                    }
                                    // Track the previous installment date
                                    if (currentInstallmentDate <= DateTime.Now && previousInstallmentDate.HasValue)
                                    {
                                        obju.NextInstallmentDate = previousInstallmentDate.Value;
                                    }
                                }

                                // Update the previous installment date to the current one for the next iteration
                                previousInstallmentDate = currentInstallmentDate;
                            }
                        } catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Error processing row: " + ex.Message);
                            continue;
                        }
                    }

                    objDetails.LstPendindFeeStud = lstData1;
                    Session["PendingFilterList"] = lstData1;


                    List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                        {
                          //new BreadcrumbItem { Name = "Home", Url = "/" },
                          new BreadcrumbItem { Name = "Accountant Dashboard", Url = "AccountantDashboardAsyncSGS/Accountant" },
                          new BreadcrumbItem { Name = "Fee Collection", Url = "studentFeeMainpageasyncSH/Accountant" },
                        };

                    ViewBag.Breadcrumbs = breadcrumbs;

                    //   return PartialView("PendingInstallmentsListAsyncAD", objDetails);
                    return await Task.Run(() => PartialView("PendingInstallmentsListAsyncAD", objDetails));

                } catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error in PendingInstallmentslist: " + ex.Message);
                    return new HttpStatusCodeResult(500, "Internal Server Error");
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> StudentCollectFeeAsyncAD(string candidateCode, string name, string batch, string courseName, double courseFee, double totalFees, double totalPaid, double remainingFees, double installmentAmount, string installmentDate, string DiscountPercentage, string studentType, string BranchAddress)
        {
            //DateTime parsedInstallmentDate;
            // Attempt to parse the string to a DateTime
            //  parsedInstallmentDate = DateTime.Parse(installmentDate);
            // Assign parsed DateTime to InstallmentDate property

            var viewModel = new Accountant
            {
                CandidateCode = candidateCode,
                Name = name,
                Batch = batch,
                CourseName = courseName,
                CourseFee = courseFee,
                TotalFees = (decimal)totalFees,
                TotalPaid = totalPaid,
                InstallmentAmount = installmentAmount,
                InstallmentDates = installmentDate,
                DiscountPercentage = DiscountPercentage,
                RemainingFees = remainingFees,
                StudentType = studentType,
                BranchAddress = BranchAddress

            };
            await BindFeeTypeDropdown();
            await BindBankDropdown();
            viewModel.StaffCode = Session["StaffCode"].ToString();
            DataSet ds2 = await objbal.BankAccountforVoucherAsyncSGS(viewModel);
            ViewBag.BankAccountList = ds2.Tables[0].AsEnumerable().Select(dr => new SelectListItem
            {
                Text = $"{dr["BankName"]} - {dr["AccountHolderName"]} - {dr["AccountNumber"]}",
                Value = dr["BankId"].ToString()
            }).ToList();

            List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                  {
                  //new BreadcrumbItem { Name = "Home", Url = "/" },
                    new BreadcrumbItem { Name = "Accountant Dashboard", Url = "AccountantDashboardAsyncSGS/Accountant" },
                    new BreadcrumbItem { Name = "Fee Collection", Url = "studentFeeMainpageasyncSH/Accountant" },
                    new BreadcrumbItem { Name = "Collect Installments"}, // Adjust URL as needed
                  };
            ViewBag.Breadcrumbs = breadcrumbs;
            return PartialView("StudentCollectFeeAsyncAD", viewModel);


        }


        public async Task<ActionResult> ExternalStudentCollectFeeAsyncAD(string candidateCode, string name, double installmentAmount)
        {
            var viewModel = new Accountant
            {
                CandidateCode = candidateCode,
                Name = name,
                InstallmentAmount = installmentAmount
            };

            await BindFeeTypeDropdown();
            await BindBankDropdown();

            List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                    {
                     new BreadcrumbItem { Name = "Accountant Dashboard", Url = "AccountantDashboardAsyncSGS/Accountant" },
                     new BreadcrumbItem { Name = "Fee Collection", Url = "studentFeeMainpageasyncSH/Accountant" },
                     new BreadcrumbItem { Name = "Collect Installments"}
                    };
            ViewBag.Breadcrumbs = breadcrumbs;

            return PartialView("ExternalStudentCollectFeeAsyncAD", viewModel);
        }



        [HttpPost]
        public async Task<ActionResult> ExternalfeeCollectedAsyncAD(Accountant model)
        {
            string staffCode = Session["StaffCode"].ToString();
            model.BranchCode = Session["BranchCode"].ToString();
            BALAccountant bALAccount = new BALAccountant();
            DataSet ds = await bALAccount.ReciptCodeAD(model);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.ReciptCode = ds.Tables[0].Rows[0]["NewFeesCollectioncode"].ToString();
            }

            model.FeeTypeId = 11;

            DateTime today = DateTime.Today;
            model.ReceiptDateAD = today.ToString("yyyy-MM-dd");

            // Convert TotalFees to double
            double totalFeesInDouble = Convert.ToDouble(model.TotalFees);

            // Perform the calculation using double
            model.RemainingFees = totalFeesInDouble - model.TotalPaid - Convert.ToDouble(model.InstallmentAmount);


            DataSet ds1 = await bALAccount.BankHolderNameAD(model);

            if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                model.DrawnOn = ds1.Tables[0].Rows[0]["AccountHolderName"].ToString();
            }

            Accountant feesCollection = new Accountant
            {
                CandidateCode = model.CandidateCode,
                Name = model.Name,
                Description = model.Description,
                CourseName = model.CourseName,
                BankId = model.BankId,
                FeeTypeId = model.FeeTypeId,
                TransactionID_checqueNumber = model.TransactionID_checqueNumber,
                InstallmentDate = model.InstallmentDate,
                AccountHolderName = model.AccountHolderName,
                PaymentModeId = model.PaymentModeId,
                PaymentType = model.PaymentType,
                AccountHolderId = model.AccountHolderId,
                ChequeDate = model.ChequeDate,
                Amount = (decimal)model.InstallmentAmount,
                StaffCode = staffCode,
                BranchCode = model.BranchCode,
                ReciptCode = model.ReciptCode,
                ChequeBankName = model.ChequeBankName,
                DrawnOn = model.DrawnOn,
                RemainingFees = model.RemainingFees,
                ReceiptDateAD = model.ReceiptDateAD,
                CheckBankName = model.CheckBankName,
                CheckAccountNumber = model.CheckAccountNumber,
            };
            await bALAccount.FeeCollectionAsync(feesCollection);
            // return View(model);

            return RedirectToAction("studentFeeMainpageasyncSH", "Accountant");

        }
        /// <summary>
        /// method to Get the list of SalarySlip based on Accountant 
        /// </summary>
        //<param name = "staffCode" > staffCode for Accountant.</param>
        /// <returns>List of SelfStaffSalary.</returns>

        //-----------------------------------------------------------------  Atharv End ------------------------------------------------------------


        /// <summary>
        /// method to Get the list of SalarySlip based on Accountant 
        /// </summary>
        //<param name = "staffCode" > staffCode for Accountant.</param>
        /// <returns>List of SelfStaffSalary.</returns>


        [HttpGet]
        public async Task<ActionResult> GetSelfSalarySlipsAsyncAG(string year, string month)
        {
            string staffCode = Session["StaffCode"] as string;
            if (string.IsNullOrEmpty(staffCode))
            {
                // Handle case where staff code is not found in session
                return RedirectToAction("Login", "Account");
            }

            DateTime currentDate = DateTime.Now;
            year = string.IsNullOrEmpty(year) ? currentDate.Year.ToString() : year;
            month = string.IsNullOrEmpty(month) ? currentDate.Month.ToString() : month;

            DataSet ds = await objbal.GetSelfSalarySlipsAsyncAG(staffCode, year, month);
            List<Accountant> salarySlips = new List<Accountant>();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Accountant obja = new Accountant
                    {
                        MonthA = dr["MonthA"] != DBNull.Value ? dr["MonthA"].ToString() : string.Empty,
                        Month = dr["Month"] != DBNull.Value ? Convert.ToInt32(dr["Month"]) : 0,
                        Year = dr["Year"] != DBNull.Value ? Convert.ToInt32(dr["Year"]) : 0,
                        StaffCode = dr["StaffCode"] != DBNull.Value ? dr["StaffCode"].ToString() : string.Empty,
                        StaffName = dr["StaffName"] != DBNull.Value ? dr["StaffName"].ToString() : string.Empty,
                        StaffPosition = dr["Designation"] != DBNull.Value ? dr["Designation"].ToString() : string.Empty,
                        StaffCTC = dr["CurrentCTC"] != DBNull.Value ? Convert.ToDecimal(dr["CurrentCTC"]) : 0m,
                        GrossSalary = dr["MonthlyGrossSalary"] != DBNull.Value ? Convert.ToDecimal(dr["MonthlyGrossSalary"]) : 0m,
                        TotalAllowances = dr["MonthlyTotalAllowance"] != DBNull.Value ? Convert.ToDecimal(dr["MonthlyTotalAllowance"]) : 0m,
                        TotalDeductions = dr["MonthlyTotalDeduction"] != DBNull.Value ? Convert.ToDecimal(dr["MonthlyTotalDeduction"]) : 0m,
                        NetSalary = dr["TotalSalary"] != DBNull.Value ? Convert.ToDecimal(dr["TotalSalary"]) : 0m,
                        SalaryCreditedDate = dr["CreditedDate"] != DBNull.Value &&
                     dr["CreditedDate"].ToString() != "NA" &&
                     DateTime.TryParse(dr["CreditedDate"].ToString(), out DateTime creditedDate)
                     ? creditedDate.Date
                     : DateTime.MinValue.Date,

                        // Additional fields you might want to add
                        PFNumber = dr["PFNumber"] != DBNull.Value ? dr["PFNumber"].ToString() : string.Empty,
                        ESINumber = dr["ESINumber"] != DBNull.Value ? dr["ESINumber"].ToString() : string.Empty,
                        BankName = dr["BankName"] != DBNull.Value ? dr["BankName"].ToString() : string.Empty,
                        AccountNumber = dr["AccountNumber"] != DBNull.Value ? dr["AccountNumber"].ToString() : string.Empty,
                        DepartmentName = dr["DepartmentName"] != DBNull.Value ? dr["DepartmentName"].ToString() : string.Empty,
                        BranchName = dr["BranchName"] != DBNull.Value ? dr["BranchName"].ToString() : string.Empty,
                        CenterName = dr["CenterName"] != DBNull.Value ? dr["CenterName"].ToString() : string.Empty,
                        ClientName = dr["ClientName"] != DBNull.Value ? dr["ClientName"].ToString() : string.Empty,
                        //ClientAddress = dr["ClientAddress"] != DBNull.Value ? dr["ClientAddress"].ToString() : string.Empty,
                    };
                    salarySlips.Add(obja);
                }
            } else
            {
                ViewBag.Message = "No salary slips found for the given criteria.";
            }

            ViewBag.SalarySlips = salarySlips;
            List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
{
    new BreadcrumbItem { Name = "AccountantDashboard", Url = Url.Action("AccountantDashboardAsyncSGS", "Accountant") },
    new BreadcrumbItem { Name = "Salary Slip", Url = Url.Action("GetSelfSalarySlipsAsyncAG", "Accountant") }
};
            ViewBag.Breadcrumbs = breadcrumbs;
            return View();
        }
        /// <summary>
        /// this method is using generated salary slip is view
        /// </summary>
        /// <returns></returns>


        [HttpGet]
        public async Task<ActionResult> SalarySlipViewAsyncAG(string staffcode, string year, string month)
        {
            // Retrieve the staff code from the session
            string staffCode = Session["StaffCode"] as string;

            if (string.IsNullOrEmpty(staffCode))
            {
                // Redirect to login if staff code is not found in the session
                return RedirectToAction("Login", "Accountant");
            }

            // Fetch data from the database
            DataSet ds = await objbal.ViewGenerateSelfSalarySlipsAsyncAG(staffCode, month, year);

            // Check if data is retrieved successfully
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var staffData = ds.Tables[0].Rows[0];

                // Helper method to safely get string value
                string SafeGetString(string columnName)
                {
                    return staffData[columnName] != DBNull.Value ? staffData[columnName].ToString().Trim() : string.Empty;
                }

                // Helper method to safely get decimal value
                decimal SafeGetDecimal(string columnName)
                {
                    return staffData[columnName] != DBNull.Value ?
                           Convert.ToDecimal(staffData[columnName]) :
                           0m;
                }

                // Helper method to safely get int value
                int SafeGetInt(string columnName)
                {
                    return staffData[columnName] != DBNull.Value ?
                           Convert.ToInt32(staffData[columnName]) :
                           0;
                }

                // Helper method to safely get datetime value
                DateTime SafeGetDateTime(string columnName)
                {
                    return staffData[columnName] != DBNull.Value ?
                           Convert.ToDateTime(staffData[columnName]) :
                           DateTime.MinValue;
                }

                // Split and parse allowance and deduction components
                var allowances = SafeGetString("AllowanceComponents")
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var allowanceAmounts = SafeGetString("AllowanceAmounts")
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(amount => decimal.TryParse(amount.Trim(), out var result) ? result : 0)
                    .ToArray();

                var deductions = SafeGetString("DeductionComponents")
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var deductionAmounts = SafeGetString("DeductionAmounts")
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(amount => decimal.TryParse(amount.Trim(), out var result) ? result : 0)
                    .ToArray();

                // Populate the Accountant model with null-safe methods
                Accountant objac = new Accountant
                {
                    StaffCode = SafeGetString("StaffCode"),
                    StaffName = SafeGetString("StaffName"),
                    StaffPosition = SafeGetString("Designation"),
                    BranchName = SafeGetString("BranchName"),
                    BankName = SafeGetString("BankName"),
                    AccountNumber = SafeGetString("AccountNumber"),
                    Department = SafeGetString("DepartmentName"),
                    JoiningDate = SafeGetDateTime("JoiningDate"),
                    PANNumber = SafeGetString("PanCardNo"),
                    ESINumber = SafeGetString("ESINumber"),
                    PFNumber = SafeGetString("PFNumber"),
                    WorkedDays = SafeGetInt("PayableDays"),
                    PresentDays = SafeGetInt("PresentDays"),
                    TotalDays = SafeGetInt("TotalDays"),
                    CenterName = SafeGetString("CenterName"),
                    ClientName = SafeGetString("ClientName"),
                    Logo = SafeGetString("Logo"),
                    TotalAllowances = SafeGetDecimal("MonthlyTotalAllowance"),
                    TotalDeductions = SafeGetDecimal("MonthlyTotalDeduction"),
                    NetSalary = SafeGetDecimal("NetSalary"),
                    BasicSalary = SafeGetDecimal("MonthlyBasicSalary"),
                    Address = SafeGetString("ClientAddress"),
                    AllowanceComponents = string.Join(",", allowances),
                    AllowanceAmounts = string.Join(",", allowanceAmounts),
                    DeductionComponents = string.Join(",", deductions),
                    DeductionAmounts = string.Join(",", deductionAmounts)
                };

                return View("SalarySlipViewAsyncAG", objac);
            } else
            {
                // Handle case where no data is found
                return RedirectToAction("Index", "Home");
            }
        }
        //----------------Amruta- End   ----------------------------------- //
        //----------------Mukesh- Start   ----------------------------------- //



        #region//Mukesh Expense Modal Start Here

        /// <summary>
        /// Asynchronously loads the Expense Dashboard view for the logged-in user.
        /// Redirects to the Login page if the session is not active.
        /// </summary>
        /// <returns>Returns the Expense Dashboard view or redirects to the Login page.</returns>
        [HttpGet]

        public async Task<ActionResult> ExpenseDashboardAsyncMB()
        {
            try
            {
                // Check if the user session is active
                if (Session["StaffCode"] == null)
                {
                    // Redirect to the Login page if the session is inactive
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                } else
                {
                    // Create a list of breadcrumb items to display in the view
                    List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                      {
                          new BreadcrumbItem { Name = "Dashboard", Url = "AccountantDashboardAsyncSGS" },
                          new BreadcrumbItem { Name = "Expense List" },
                      };
                    await BillToPayCountForExpenseDashboardMB();
                    // Pass the breadcrumb items to the view using ViewBag
                    ViewBag.Breadcrumbs = breadcrumbs;


                    // Return the Expense Dashboard view
                    return await Task.Run(() => View("ExpenseDashboardAsyncMB"));
                }
            } catch (Exception ex)
            {


                // Handle the error by redirecting to an error page or returning a view with error information
                return await Task.Run(() => View("Error", new HandleErrorInfo(ex, "Expense", "ExpenseDashboardAsyncMB")));
            }
        }


        /// <summary>
        /// Asynchronously retrieves and displays a list of expenses (Regular, Refund, Reference, and Other)
        /// for the given expense ID. If the session is not active, redirects to the Login page.
        /// </summary>
        /// <param name="Id">The ID representing the specific expense record to display.</param>
        /// <returns>Returns a partial view "ListOfExpensesAsyncMB" with the expense details.</returns>
        [HttpGet]
        public async Task<ActionResult> ListOfExpensesAsyncMB(int Id)
        {
            try
            {
                // Check if the user session is active
                if (Session["StaffCode"] == null)
                {
                    // Redirect to the Login page if the session is inactive
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                } else
                {
                    // Create a list of breadcrumb items to display in the view
                    List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
              {
                  new BreadcrumbItem { Name = "Dashboard", Url = "AccountantDashboardAsyncSGS" },
                  new BreadcrumbItem { Name = "Expense List" },
              };

                    // Pass the breadcrumb items and currency symbol to the view
                    ViewBag.Breadcrumbs = breadcrumbs;
                    ViewBag.Currency = "&#x20b9;";

                    // Set the expense ID and store it in the session
                    objac.ExpID = Convert.ToInt32(Id).ToString();
                    Session["ID"] = Convert.ToInt32(Id).ToString();
                    objac.BranchCode = Session["BranchCode"].ToString();
                    // Retrieve the list of expenses asynchronously
                    DataSet ds = await objbal.ListOfExpensesAsyncMB(objac);

                    List<Accountant> lstRegularExpense = new List<Accountant>();
                    decimal totalAmountSum = 0; // Variable to accumulate the total amount

                    // Loop through the dataset and populate the list of expenses
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Accountant objP = new Accountant
                        {
                            TransactionCode = ds.Tables[0].Rows[i]["TransactionCode"].ToString(),
                            ExpenseType = ds.Tables[0].Rows[i]["ExpenseType"].ToString(),
                            ReferenceByName = ds.Tables[0].Rows[i]["Name"].ToString(),
                            ReferenceToName = ds.Tables[0].Rows[i]["ReferenceToCandidate"].ToString(),
                            VendorName = ds.Tables[0].Rows[i]["Name"].ToString(),
                            ExpID = Convert.ToInt32(Id).ToString(),
                            StaffCode_CandidateCode = ds.Tables[0].Rows[i]["StaffName"].ToString(),
                            StaffCode = ds.Tables[0].Rows[i]["StaffCandidateCode"].ToString(),
                            VendorCode = ds.Tables[0].Rows[i]["VendorCode"].ToString(),
                            Date = Convert.ToDateTime(ds.Tables[0].Rows[i]["TransactionDate"].ToString()),
                            ExpenseName = ds.Tables[0].Rows[i]["ExpenseName"].ToString(),
                            Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["TransactionAmount"].ToString()),
                            Balance = Convert.ToDecimal(ds.Tables[0].Rows[i]["BalanceAmount"].ToString()),
                            Status = ds.Tables[0].Rows[i]["Status"].ToString(),
                            Description = ds.Tables[0].Rows[i]["Description"].ToString()
                        };

                        // Calculate the total amount for each expense
                        objP.TotalAmount = (decimal)(objP.Amount + objP.Balance);
                        totalAmountSum += objP.TotalAmount; // Add to the total sum

                        lstRegularExpense.Add(objP);
                    }

                    // Pass the total amount sum to the view
                    ViewBag.TotalAmount = totalAmountSum;
                    ViewBag.ExpenseType = "Direct Expense";
                    // Save the list of expenses and currency in the session
                    objac.lstRegularExpense = lstRegularExpense;
                    Session["ListforFilter"] = lstRegularExpense;
                    Session["Currency"] = "&#x20b9;";
                    await GetExpenceTypeAsynMB(Id);
                    // Return the partial view with the expense details
                    return PartialView("ListOfExpensesAsyncMB", objac);
                }
            } catch (Exception ex)
            {


                // Handle the error by returning an error view with the exception details
                return await Task.Run(() => View("Error", new HandleErrorInfo(ex, "Expense", "ListOfExpensesAsyncMB")));
            }
        }





        /// <summary>
        /// Filters the list of expenses based on the provided status, start date, and end date.
        /// Retrieves the filtered list from the session and displays it in the view.
        /// </summary>
        /// <param name="status">The status of the expense to filter by.</param>
        /// <param name="startDate">The start date from which to filter the expenses.</param>
        /// <param name="endDate">The end date up to which to filter the expenses.</param>
        /// <returns>Returns a partial view "ListOfExpensesAsyncMB" with the filtered list of expenses.</returns>
        [HttpGet]
        public async Task<ActionResult> FilterforlistMB(string status, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // Retrieve the list of expenses from the session
                List<Accountant> purchases = Session["ListforFilter"] as List<Accountant>;

                // Check if a status filter is applied and it is not "SelectAll"
                if (!string.IsNullOrEmpty(status) && status != "SelectAll")
                {
                    // Filter expenses by the selected status
                    purchases = purchases.Where(p => p.Status == status).ToList();
                }

                // Apply the start date filter if provided
                if (startDate.HasValue)
                {
                    // Filter expenses that have a date greater than or equal to the start date
                    purchases = purchases.Where(p => p.Date >= startDate.Value).ToList();
                }

                // Apply the end date filter if provided
                if (endDate.HasValue)
                {
                    // Filter expenses that have a date less than or equal to the end date
                    purchases = purchases.Where(p => p.Date <= endDate.Value).ToList();
                }

                // Retrieve the currency symbol from the session
                ViewBag.Currency = Session["Currency"].ToString();

                // Create an Accountant object with the filtered list
                Accountant obj = new Accountant { lstRegularExpense = purchases };
                obj.ExpID = Session["ID"].ToString();

                // Return the partial view with the filtered list of expenses
                return await Task.Run(() => PartialView("ListOfExpensesAsyncMB", obj));
            } catch (Exception ex)
            {
                // Handle the error by displaying an error message in the view
                ViewBag.ErrorMessage = ex.Message;

                // Return the error view in case of an exception
                return await Task.Run(() => View("error"));
            }
        }











        /// <summary>
        /// here we can pass the data to Add Expense modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> AddExpensesAsyncMB()
        {
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            } else
            {

                Accountant obj = new Accountant();

                obj.Date = DateTime.Now;
                await GetExpenceCategoryMB();
                await GetRefundCandidate();
                await GetReferenceByStudentsAsyncMB();
                await ListVendorAsyncVP();//getting the vendor list for add Expense
                await GetStaffNameAsyncMB();
                await GetSalaryAdvanceStaffNameAsyncMB();
                await GetMonthlyCreditSalaryAsyncMB();
                await GetRegularExpenseTypeAsyncMB();
                return PartialView("AddExpensesAsyncMB", obj);
            }

        }

        /// <summary>
        /// get the refund candidate 
        /// </summary>
        /// <returns>AddExpenseView</returns>
        [HttpGet]

        public async Task GetRefundCandidate()
        {
            DataSet ds = await objbal.GetRefundCandidateMB();
            List<SelectListItem> lstRefundCandidate = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                lstRefundCandidate.Add(new SelectListItem { Text = dr["FullName"].ToString(), Value = dr["CandidateCode"].ToString() });
            }

            await Task.Run(() => ViewBag.RefundCandidatelst = lstRefundCandidate);

        }

        /// <summary>
        /// get the Regular Expense Type eg. like rent,lightbill etc
        /// </summary>
        /// <returns></returns>
        [HttpGet]

        public async Task GetRegularExpenseTypeAsyncMB()
        {
            DataSet ds = await objbal.GetRegularExpenseTypeAsyncMB();
            List<SelectListItem> lstRegularExpType = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                lstRegularExpType.Add(new SelectListItem { Text = dr["ExpenseName"].ToString(), Value = dr["ExpenseId"].ToString() });
            }

            await Task.Run(() => ViewBag.RegularExpTypelst = lstRegularExpType);

        }

        /// <summary>
        /// get the refund candiadate paid fee 
        /// </summary>
        /// <param name="CandidateCode"></param>
        /// <returns> addexpenseview</returns>
        [HttpGet]
        public async Task<JsonResult> GetTotalPaidFeeOfCandidateAsyncMB(string CandidateCode)
        {
            Accountant obj = new Accountant();
            obj.CandidateCode = CandidateCode;
            SqlDataReader dr = await objbal.GetTotalPaidFeeOfCandidateAsyncMB(obj);
            string CandidatePaidFee = string.Empty;
            string CourseFee = string.Empty;

            while (dr.Read())
            {
                CandidatePaidFee = dr["TransactionAmount"].ToString();
                CourseFee = dr["CourseFee"].ToString();
            }

            return Json(new { success = true, CandidatePaidFee = CandidatePaidFee, CourseFee = CourseFee }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public async Task<JsonResult> GetLastExpenseEntryAsyncMB(int selectedType)
        {
            Accountant obj = new Accountant();
            obj.ExpenseId = selectedType;
            SqlDataReader dr = await objbal.GetLastExpenseEntryAsyncMB(obj);
            string VendorCode = string.Empty;
            string Amount = string.Empty;

            while (dr.Read())
            {
                Amount = dr["TransactionAmount"].ToString();
                VendorCode = dr["VendorCode"].ToString();
            }

            return Json(new { success = true, VendorCode = VendorCode, Amount = Amount }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Get the Reference By Candiadate 
        /// </summary>
        /// <returns>AddExpenseView</returns>
        [HttpGet]
        public async Task GetReferenceByStudentsAsyncMB()
        {
            Accountant obj = new Accountant();
            obj.BranchCode = Session["BranchCode"].ToString();
            DataSet ds = await objbal.GetReferenceByCandidatesAsyncMB(obj);
            List<SelectListItem> lstReferenceByStudent = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                lstReferenceByStudent.Add(new SelectListItem { Text = dr["FullName"].ToString(), Value = dr["RefBy"].ToString() });
            }
            await Task.Run(() => ViewBag.ReferenceByStudentlst = lstReferenceByStudent);
        }

        /// <summary>
        /// Get the staffname for giving them Advance
        /// </summary>
        /// <returns>AddExpenseView </returns>
        [HttpGet]
        public async Task GetStaffNameAsyncMB()
        {
            Accountant obj = new Accountant();
            obj.BranchCode = Session["BranchCode"].ToString();
            DataSet ds = await objbal.GetStaffNameAsyncMB(obj);
            List<SelectListItem> lstStaff = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                lstStaff.Add(new SelectListItem { Text = dr["StaffName"].ToString(), Value = dr["StaffCode"].ToString() });
            }
            await Task.Run(() => ViewBag.lstStaff = lstStaff);
        }

        /// <summary>
        /// Get the staffname for giving them Advance
        /// </summary>
        /// <returns>AddExpenseView </returns>
        [HttpGet]
        public async Task GetSalaryAdvanceStaffNameAsyncMB()
        {
            Accountant obj = new Accountant();
            obj.BranchCode = Session["BranchCode"].ToString();
            DataSet ds = await objbal.GetSalaryAdvanceStaffNameAsyncMB(obj);
            List<SelectListItem> lstStaff = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                lstStaff.Add(new SelectListItem { Text = $"{dr["StaffName"].ToString()} -  {dr["Amount"]}", Value = dr["AmountPaidTo"].ToString() });
            }
            await Task.Run(() => ViewBag.AdvSalaryStafflst = lstStaff);
        }

        /// <summary>
        /// Get the monthly salary credit details 
        /// </summary>
        /// <returns>AddExpenseView </returns>
        [HttpGet]
        public async Task GetMonthlyCreditSalaryAsyncMB()
        {
            DataSet ds = await objbal.GetMonthlyCreditSalaryAsyncMB();
            List<SelectListItem> MnthCredSalary = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                MnthCredSalary.Add(new SelectListItem { Text = $"{dr["MonthYear"].ToString()} -  {dr["Amount"]}", Value = dr["SalaryMonthId"].ToString() });
            }
            await Task.Run(() => ViewBag.MnthCredSalary = MnthCredSalary);
        }

        /// <summary>
        /// Get Reference To Candidate 
        /// </summary>
        /// <param name="CandidateCode"></param>
        /// <returns>AddExpenseView</returns>
        [HttpGet]

        public async Task<JsonResult> GettheReferenceToCandidateAsyncMB(string CandidateCode)
        {
            Accountant obj = new Accountant();
            obj.CandidateCode = CandidateCode;
            DataSet ds = await objbal.GetReferenceToCandidatesAsyncMB(obj);
            List<SelectListItem> ReferenceToCandidate = new List<SelectListItem>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ReferenceToCandidate.Add(new SelectListItem { Text = dr["FullName"].ToString(), Value = dr["RefTo"].ToString() });
            }

            /// fetch the course fee of reference by students
            SqlDataReader drF = await objbal.GetReferenceAmountAsyncMB(obj);
            string ReferenceAmount = "";
            if (drF.Read())
            {
                // Retrieve the ReferenceAmount as a string
                ReferenceAmount = drF["ReferenceAmount"].ToString();


            }
            return Json(new { success = true, ReferenceAmount = ReferenceAmount, candidates = ReferenceToCandidate }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Expense Category
        /// </summary>
        /// <returns>AddExpenseView</returns>
        [HttpGet]
        public async Task GetExpenceCategoryMB()
        {
            DataSet ds = await objbal.GetExpenceCategoryMB();
            List<SelectListItem> Courselist = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Courselist.Add(new SelectListItem { Text = dr["ExpenseCategory"].ToString(), Value = dr["ExpenseCategoryId"].ToString() });
            }
            await Task.Run(() => ViewBag.CourseList = Courselist);

        }

        /// <summary>
        /// Get Expense Type when we select expense Category
        /// </summary>
        /// <param name="ExpCategoryId"></param>
        /// <returns>AddExpenseView</returns>
        [HttpGet]
        public async Task<JsonResult> GetExpenceTypeAsynMB(int ExpCategoryId)
        {
            Accountant obj = new Accountant();
            obj.ExpID = Convert.ToInt32(ExpCategoryId).ToString();
            SqlDataReader dr = await objbal.GetTheExpenceTypeAsyncMB(obj);
            string expenseType = string.Empty;

            while (dr.Read())
            {
                expenseType = dr["ExpenseType"].ToString();
            }
            ViewBag.ExpenseType = expenseType;
            return Json(new { success = true, expenseType = expenseType }, JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        /// Asynchronously saves the details of an expense. Generates a unique expense code, sets the necessary properties
        /// based on the expense type, and saves the expense data to the database.
        /// </summary>
        /// <param name="objA">The Accountant object containing the expense details.</param>
        /// <param name="ReferenceToCandidateCode">Code of the candidate referenced in the expense, if applicable.</param>
        /// <param name="EmployeeCode">Code of the employee associated with the expense, if applicable.</param>
        /// <returns>Returns a JSON result indicating success or failure along with the new expense code if successful.</returns>
        [HttpPost]
        public async Task<JsonResult> AddExpensesAsyncMBPost(Accountant objA, string EmployeeCode)
        {
            // Check if the user session is active
            if (Session["StaffCode"] == null)
            {
                // Return JSON indicating failure and redirect to the Login page if the session is inactive
                return Json(new { success = false, redirect = Url.Action("Login", "Account") });
            } else
            {
                try
                {
                    string maxCode = null;  // Variable to store the maximum expense code from the database
                    string newCode;         // Variable to store the new expense code
                    string BranchPerfix = Session["Prefix"].ToString();   // Branch prefix from session
                    objA.BranchCode = Session["BranchCode"].ToString();
                    // Retrieve the maximum expense code asynchronously for auto-increment
                    SqlDataReader dr = await objbal.GetMaxExpenseCodeForAutoIncrement(objA);
                    if (dr.Read())
                    {
                        maxCode = dr["MaxCode"].ToString();

                    }

                    // Check if the BranchPerfix matches the prefix of the maxCode or if no maxCode exists
                    if (string.IsNullOrEmpty(maxCode) || !maxCode.StartsWith(BranchPerfix + "EXP"))
                    {
                        // If no code exists or BranchPerfix does not match, start with new code
                        newCode = BranchPerfix + "EXP001";  // Initial code for the new branch
                    } else
                    {
                        // If the prefix matches, increment the numeric part of the existing code
                        int numericPart = int.Parse(maxCode.Substring(BranchPerfix.Length + 3)) + 1; // Extract and increment the numeric part
                        newCode = BranchPerfix + "EXP" + numericPart.ToString("D3");  // Format the new expense code
                    }

                    // Assign the new expense code to the Accountant object
                    objA.TransactionCode = newCode;

                    // Set properties based on the type of expense
                    if (objA.ExpID == "3")
                    {
                        //objA.ReferenceToName = ReferenceToCandidateCode;
                        objA.RegularExpenseType = "5";
                    }

                    if (objA.ExpID == "5")
                    {
                        objA.RegularExpenseType = "7";
                        objA.Comment = EmployeeCode;
                    }

                    if (objA.ExpID == "4")
                    {
                        objA.RegularExpenseType = "6";
                        objA.StaffCode_CandidateCode = EmployeeCode;
                    }

                    if (objA.ExpID == "2")
                    {
                        objA.RegularExpenseType = "4";
                    }

                    // Assign the logged-in staff code from the session
                    objA.LoginStaffCode = Session["StaffCode"].ToString();

                    // Save the expense details asynchronously
                    await objbal.SavetheExpenceMB(objA);

                    // Return JSON indicating success and the new expense code
                    return Json(new { success = true, newCode = newCode });
                } catch (Exception ex)
                {
                    // Log the error message (adjust based on your logging framework)
                    // Example: _logger.LogError(ex, "An error occurred while saving the expense.");

                    // Handle the error by returning a JSON result with the error message
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }


        /// <summary>
        /// Asynchronously displays a pop-up for matching a voucher to an expense. Initializes the Accountant object with
        /// transaction details and invokes the method to fetch vouchers based on the vendor, staff code, and expense type.
        /// </summary>
        /// <param name="TCode">Transaction code of the expense.</param>
        /// <param name="Amount">Amount of the expense to be matched with a voucher.</param>
        /// <param name="VendorName">Name of the vendor associated with the expense.</param>
        /// <param name="staffcode">Code of the staff responsible for the transaction.</param>
        /// <param name="ExpTypeID">Expense type ID for categorizing the transaction.</param>
        /// <returns>Returns a partial view to display the match voucher pop-up with initialized transaction data.</returns>
        [HttpGet]
        public async Task<ActionResult> ExpenseMatchVoucherAsyncVM(string TCode, float Amount, string VendorCode, string VendorName, string staffcode, string ExpTypeID, string Expenses, string Description)
        {
            try
            {
                Accountant obj = new Accountant();
                String AmountCreditTo = null;
                if (ExpTypeID == 5.ToString())
                {
                    AmountCreditTo = Description;

                    obj.CreditTo = Description;
                } else if (Expenses == "Salary")
                {
                    obj.CreditTo = Expenses;
                } else
                {
                    AmountCreditTo = VendorName;
                    obj.CreditTo = VendorName;
                }
                // Create a new Accountant object and initialize it with the transaction details

                obj.TransactionCode = TCode;
                obj.Amount = Convert.ToDecimal(Amount); // Convert the amount to decimal

                //VendorName = AmountCreditTo,
                obj.ExpenseName = Expenses;
                // obj.Description = Description;



                // Call the method to list vouchers asynchronously, passing vendor, staff code, and expense type ID
                await ListVoucherAsyncMB(VendorCode, staffcode, ExpTypeID);

                // Return the partial view with the initialized Accountant object for matching vouchers
                return PartialView("_MatchVoucherAsyncVMB", obj);
            } catch (Exception ex)
            {
                // Log the error (adjust based on your logging framework)
                // Example: _logger.LogError(ex, "An error occurred while preparing the match voucher pop-up.");

                // Handle the error by returning an error view with the exception details
                ViewBag.ErrorMessage = ex.Message;
                return View("Error"); // Adjust as necessary if you have a specific error view
            }
        }

        /// <summary>
        /// Asynchronously lists the vouchers to match with a transaction based on the provided vendor name, staff code, and expense type ID.
        /// </summary>
        /// <param name="vendorName">The name of the vendor associated with the voucher.</param>
        /// <param name="staffcode">The code of the staff responsible for the transaction.</param>
        /// <param name="ExpTypeID">The expense type ID to filter the vouchers.</param>
        /// <returns>Returns a JSON result containing the list of vouchers if successful, or an error message if the session is invalid.</returns>
        [HttpGet]
        public async Task<JsonResult> ListVoucherAsyncMB(string VendorCode, string staffcode, string ExpTypeID)
        {
            if (Session["StaffCode"] == null)
            {
                // Return an error if the user session is not found
                return Json(new { success = false, message = "Cannot find the user." }, JsonRequestBehavior.AllowGet);
            } else
            {
                try
                {
                    // Create a new Accountant object and initialize it with session and parameter values
                    Accountant objac = new Accountant
                    {
                        BranchCode = Session["BranchCode"].ToString(),
                        VendorName = VendorCode,
                        StaffCode = staffcode
                    };

                    // Initialize the list to store voucher details
                    List<SelectListItem> VoucherList = new List<SelectListItem>();

                    // Check the expense type and fetch the appropriate list of vouchers
                    if (ExpTypeID == "1" || ExpTypeID == "2" || ExpTypeID == "3" || ExpTypeID == "5")
                    {
                        // Fetch vouchers from the first DataSet table
                        DataSet ds = await objbal.ListVouchersAsyncMB(objac);
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            VoucherList.Add(new SelectListItem { Text = $"{dr["PaymentMode"].ToString() + ":" + dr["VoucherCode"].ToString() + "-" + dr["PaidTo"].ToString() + "-" + dr["Balance"].ToString()}", Value = dr["VoucherCode"].ToString() });
                        }
                    } else
                    {
                        // Fetch vouchers from the second DataSet table
                        DataSet ds = await objbal.ListVouchersAsyncMB(objac);
                        foreach (DataRow dr in ds.Tables[1].Rows)
                        {
                            VoucherList.Add(new SelectListItem
                            {
                                Text = $"{dr["VoucherCode"]} - {dr["PaidTo"]} - {dr["Balance"]} -  {dr["VoucherType"]}",
                                Value = dr["VoucherCode"].ToString()
                            });
                        }
                    }

                    // Store the voucher list in ViewBag
                    ViewBag.VoucherCode = VoucherList;

                    // Return the list of vouchers as a JSON result
                    return Json(new { success = true, data = VoucherList }, JsonRequestBehavior.AllowGet);
                } catch (Exception ex)
                {
                    // Log the error (adjust based on your logging framework)
                    // Example: _logger.LogError(ex, "An error occurred while fetching vouchers.");

                    // Handle the error by returning a JSON result with the exception message
                    return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
        }






        /// <summary>
        /// Updates the voucher amounts based on the selected vouchers and the paid amount.
        /// </summary>
        /// <param name="paidAmount">The total amount paid to be distributed among the selected vouchers.</param>
        /// <param name="voucherCodes">A list of voucher codes to update.</param>
        /// <param name="TranscationCode">The transaction code associated with the vouchers.</param>
        /// <param name="Description">A description of the transaction.</param>
        /// <returns>Returns a JSON result indicating success and a redirect URL.</returns>
        [HttpPost]
        public async Task<ActionResult> UpdateTheVoucherAmountAsyncMB(decimal? paidAmount, List<string> voucherCodes, string TranscationCode, string Description)
        {
            try
            {
                // Initialize an Accountant object and a list to store voucher balances
                Accountant obj = new Accountant();
                List<(string VoucherCode, decimal Balance, string PaymentMode)> voucherBalances = new List<(string, decimal, string)>();

                // Fetch balances for the selected vouchers
                foreach (var voucherCode in voucherCodes)
                {
                    obj.VoucherCode = voucherCode;

                    // Retrieve the balance for each voucher
                    SqlDataReader dr = await objbal.GetVoucherAmountAsyncMB(obj);
                    while (dr.Read())
                    {
                        string paymentMode = dr["PaymentMode"].ToString();
                        voucherBalances.Add((voucherCode, decimal.Parse(dr["Balance"].ToString()), paymentMode));
                    }
                }

                // *Prioritize vouchers with PaymentMode == "Cheque"*
                voucherBalances = voucherBalances
                    .OrderByDescending(v => v.PaymentMode.Equals("Cheque", StringComparison.OrdinalIgnoreCase)) // Cheque first
                    .ThenBy(v => v.Balance) // Optionally sort by balance for other vouchers
                    .ToList();

                // Initialize the remaining paid amount
                decimal remainingPaidAmount = paidAmount.Value;

                // Update the vouchers with the paid amount
                foreach (var (VoucherCode, Balance, PaymentMode) in voucherBalances)
                {
                    if (remainingPaidAmount <= 0)
                    {
                        break; // Stop processing if no amount is left to distribute
                    }

                    // Calculate the amount to use for each voucher
                    decimal amountToUse = Math.Min(remainingPaidAmount, Balance);
                    remainingPaidAmount -= amountToUse;
                    decimal newBalance = Balance - amountToUse;

                    // Set properties for the Accountant object to update the voucher
                    obj.VoucherCode = VoucherCode;
                    obj.Amount = Convert.ToDecimal(amountToUse.ToString());
                    obj.TransactionCode = TranscationCode;
                    obj.Description = Description;

                    // Update the voucher with the transaction
                    await objbal.VoucherLinkWithTransaction(obj);
                }


                // Return success JSON response with redirect URL
                return Json(new { success = true, redirectUrl = Url.Action("ExpenseDashboardAsyncMB", "Accountant") }, JsonRequestBehavior.AllowGet);
            } catch (Exception ex)
            {


                // Handle the error by returning a JSON result with the exception message
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// here we can show the detail expense 
        /// </summary>
        /// <param name="TCode"></param>
        /// <param name="ExpId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ViewTransactionAsyncMB(string TCode, string ExpId)
        {
            Accountant objac = new Accountant();
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                try
                {
                    //bread crumbs here
                    List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
   {
       new BreadcrumbItem { Name = "Dashboard", Url = "AccountantDashboardAsyncSGS" },
       new BreadcrumbItem { Name = "Expense", Url = "ExpenseDashboardAsyncMB" },
       new BreadcrumbItem { Name = "View Detail Transactions" },
   };

                    ViewBag.Breadcrumbs = breadcrumbs;
                    objac.TransactionCode = TCode;
                    Session["TCode"] = TCode;
                    objac.BranchCode = Session["BranchCode"].ToString();
                    SqlDataReader dr;
                    SqlDataReader drClient;
                    dr = await objbal.ListExpenseDetailsAsyncMB(objac);
                    drClient = await objbal.ListClientDetailsAsyncVP(objac);
                    if (dr.Read() && drClient.Read())
                    {
                        //client details
                        ViewBag.ClientName = drClient["ClientName"].ToString();
                        ViewBag.Logo = drClient["Logo"].ToString();
                        ViewBag.BranchName = drClient["BranchName"].ToString();
                        // ViewBag.BranchCity = drClient["BranchCity"].ToString();
                        ViewBag.BranchAddress = drClient["BranchAddress"].ToString();
                        ViewBag.BranchContact = drClient["ContactNo"].ToString();
                        ViewBag.BranchEmailId = drClient["BranchEmailId"].ToString();
                        //vendor details
                        ViewBag.VendorName = dr["VendorName"].ToString();//VendorCityId,v.VendorContact,v.VendorEmail
                        ViewBag.VendorCity = dr["CityName"].ToString();
                        ViewBag.VendorAddress = dr["VendorAddress"].ToString();
                        ViewBag.VendorNo = dr["VendorContact"].ToString();
                        ViewBag.VendorMail = dr["VendorEmail"].ToString();
                        //transaction details
                        DateTime transactionDate = Convert.ToDateTime(dr["TransactionDate"]);
                        ViewBag.TransactionDate = transactionDate.ToString("dd-MM-yyyy");
                        ViewBag.TransactionCode = TCode;
                        ViewBag.BillNumber = dr["BillNumber"].ToString();
                        ViewBag.TransactionAmount = dr["TransactionAmount"].ToString() == "0" ? 0 : decimal.Parse(dr["TransactionAmount"].ToString());
                        ViewBag.BalanceAmount = dr["BalanceAmount"].ToString() == "0" ? 0 : decimal.Parse(dr["BalanceAmount"].ToString());
                        // Reference candidate details
                        ViewBag.ReferenceToCandidate = dr["RefToCandidate"].ToString();
                        ViewBag.RefundCandidate = dr["RefundCandidate"].ToString();
                        ViewBag.Description = dr["Description"].ToString();
                        ViewBag.StaffName = dr["StaffName"].ToString();
                    }
                    //fetching the purchased item list here
                    var (listitem, listtransaction) = await ListExpenseDetailsAsyncMB();
                    objac.ListGiveExpenseMB = listitem;
                    objac.ListMatchVoucherToExpense = listtransaction;
                    ViewBag.transactionList = listtransaction;
                    objac.ExpID = ExpId;
                    //giving the currency hard coded
                    ViewBag.Currency = "&#x20b9;";
                    return await Task.Run(() => View("ViewTransactionAsyncMB", objac));
                } catch
                {
                    return await Task.Run(() => View("Error"));
                }



            }
        }

        /// <summary>
        /// Retrieves details of transactions along with the matched vouchers.
        /// </summary>
        /// <returns>A tuple containing two lists: one for expense details and one for transaction details.</returns>
        private async Task<(List<Accountant>, List<Accountant>)> ListExpenseDetailsAsyncMB()
        {
            try
            {


                // Initialize lists to hold expense and transaction details
                List<Accountant> lstitems = new List<Accountant>();
                List<Accountant> lsttransaction = new List<Accountant>();

                // Create Accountant object and set transaction code
                Accountant objA = new Accountant
                {
                    TransactionCode = Session["TCode"].ToString()
                };

                // Fetch expense and voucher data
                DataSet ds = await objbal.ListExpenseVoucherAsyncMB(objA);

                // Process expense details
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Accountant objP = new Accountant
                        {
                            VendorName = row["VendorName"].ToString(),
                            ReferenceToName = row["RefToCandidate"].ToString(),
                            StudentName = row["RefundCandidate"].ToString(),
                            StaffName = row["StaffName"].ToString(),
                            TransactionCode = Session["TCode"].ToString(),
                            Amount = Convert.ToDecimal(row["TransactionAmount"].ToString()),
                            Balance = Convert.ToDecimal(row["BalanceAmount"].ToString()),
                            Description = row["Description"].ToString()
                        };

                        // Calculate transaction amount
                        objP.TransactionAmount = objP.Amount + objP.Balance;

                        // Add to the list of expense items
                        lstitems.Add(objP);
                    }
                }

                // Process transaction details
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        Accountant objT = new Accountant
                        {
                            Date = Convert.ToDateTime(row["Date"]),
                            VoucherCode = row["VoucherCode"].ToString(),
                            PaymentMode = row["PaymentMode"].ToString(),
                            TransactionAmount = Convert.ToDecimal(row["Amount"].ToString()),
                            Balance = Convert.ToDecimal(row["BalanceAmount"].ToString())
                        };

                        // Handle payment mode specific details
                        if (objT.PaymentMode == "BANK" || objT.PaymentMode == "CHEQUE")
                        {
                            objT.TransactionId = row["TransactionId_ChequeNo"].ToString();

                            if (objT.PaymentMode == "CHEQUE")
                            {
                                objT.ChequeDate = string.IsNullOrEmpty(row["ChequeDate"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(row["ChequeDate"]);
                                objT.ChequeClearenceDate = string.IsNullOrEmpty(row["ChequeClearenceDate"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(row["ChequeClearenceDate"]);
                            }
                        }

                        // Add to the list of transaction details
                        lsttransaction.Add(objT);
                    }
                }

                // Return the lists of expense and transaction details
                return (lstitems, lsttransaction);
            } catch (Exception ex)
            {

                throw new Exception("An error occurred while retrieving expense details.", ex);
            }
        }

        /// <summary>
        /// Retrieves and processes the count and balance amounts of bills to pay for various expense categories 
        /// for the expense dashboard. 
        /// This method reads data from the database using a SqlDataReader and handles potential DBNull values 
        /// to ensure safe data processing. The results are assigned to ViewBag properties and returned as a JSON response.
        /// </summary>
        /// <returns>
        /// A JSON object containing the total count and balance amounts of bills to pay, categorized by expense types
        /// such as Rent, Light, Water Supplier, Refund, Reference, and Others.
        /// </returns>
        [HttpGet]
        public async Task<JsonResult> BillToPayCountForExpenseDashboardMB()
        {
            Accountant obj = new Accountant();
            obj.BranchCode = Session["BranchCode"].ToString();
            SqlDataReader dr = await objbal.BillToPayCountForExpenseDashboardMB(obj);

            int totalBillsToPay = 0;
            decimal totalBalanceAmount = 0;

            int rentBillsToPay = 0;
            decimal rentBalanceAmount = 0;

            int lightBillsToPay = 0;
            decimal lightBalanceAmount = 0;

            int WaterBillsToPay = 0;
            decimal WaterSupplierBalanceAmount = 0;

            int refundBillsToPay = 0;
            decimal refundBalanceAmount = 0;

            int referenceBillsToPay = 0;
            decimal referenceBalanceAmount = 0;

            int otherBillsToPay = 0;
            decimal otherBalanceAmount = 0;

            while (dr.Read())
            {
                totalBillsToPay = dr["TotalBillsToPay"] != DBNull.Value ? Convert.ToInt32(dr["TotalBillsToPay"]) : 0;
                totalBalanceAmount = dr["TotalBalanceAmount"] != DBNull.Value ? Convert.ToDecimal(dr["TotalBalanceAmount"]) : 0;

                rentBillsToPay = dr["RentBillsToPay"] != DBNull.Value ? Convert.ToInt32(dr["RentBillsToPay"]) : 0;
                rentBalanceAmount = dr["RentBalanceAmount"] != DBNull.Value ? Convert.ToDecimal(dr["RentBalanceAmount"]) : 0;

                lightBillsToPay = dr["LightBillsToPay"] != DBNull.Value ? Convert.ToInt32(dr["LightBillsToPay"]) : 0;
                lightBalanceAmount = dr["LightBalanceAmount"] != DBNull.Value ? Convert.ToDecimal(dr["LightBalanceAmount"]) : 0;

                WaterBillsToPay = dr["WaterSupplierBillsToPay"] != DBNull.Value ? Convert.ToInt32(dr["WaterSupplierBillsToPay"]) : 0;
                WaterSupplierBalanceAmount = dr["WaterSupplierBalanceAmount"] != DBNull.Value ? Convert.ToDecimal(dr["WaterSupplierBalanceAmount"]) : 0;

                refundBillsToPay = dr["RefundBillsToPay"] != DBNull.Value ? Convert.ToInt32(dr["RefundBillsToPay"]) : 0;
                refundBalanceAmount = dr["RefundBalanceAmount"] != DBNull.Value ? Convert.ToDecimal(dr["RefundBalanceAmount"]) : 0;

                referenceBillsToPay = dr["ReferenceBillsToPay"] != DBNull.Value ? Convert.ToInt32(dr["ReferenceBillsToPay"]) : 0;
                referenceBalanceAmount = dr["ReferenceBalanceAmount"] != DBNull.Value ? Convert.ToDecimal(dr["ReferenceBalanceAmount"]) : 0;

                otherBillsToPay = dr["OtherBillsToPay"] != DBNull.Value ? Convert.ToInt32(dr["OtherBillsToPay"]) : 0;
                otherBalanceAmount = dr["OtherBalanceAmount"] != DBNull.Value ? Convert.ToDecimal(dr["OtherBalanceAmount"]) : 0;
            }

            ViewBag.TotalBillsToPay = totalBillsToPay;
            ViewBag.TotalBalanceAmount = totalBalanceAmount;

            ViewBag.RentBillsToPay = rentBillsToPay;
            ViewBag.RentBalanceAmount = rentBalanceAmount;

            ViewBag.LightBillsToPay = lightBillsToPay;
            ViewBag.LightBalanceAmount = lightBalanceAmount;

            ViewBag.WaterBillsToPay = WaterBillsToPay;
            ViewBag.WaterSupplierBalanceAmount = WaterSupplierBalanceAmount;

            ViewBag.RefundBillsToPay = refundBillsToPay;
            ViewBag.RefundBalanceAmount = refundBalanceAmount;

            ViewBag.ReferenceBillsToPay = referenceBillsToPay;
            ViewBag.ReferenceBalanceAmount = referenceBalanceAmount;

            ViewBag.OtherBillsToPay = otherBillsToPay;
            ViewBag.OtherBalanceAmount = otherBalanceAmount;

            return Json(new
            {
                success = true,
                TotalBillsToPay = totalBillsToPay,
                TotalBalanceAmount = totalBalanceAmount,
                RentBillsToPay = rentBillsToPay,
                RentBalanceAmount = rentBalanceAmount,
                LightBillsToPay = lightBillsToPay,
                LightBalanceAmount = lightBalanceAmount,
                WaterBillsToPay = WaterBillsToPay,
                WaterSupplierBalanceAmount = WaterSupplierBalanceAmount,
                RefundBillsToPay = refundBillsToPay,
                RefundBalanceAmount = refundBalanceAmount,
                ReferenceBillsToPay = referenceBillsToPay,
                ReferenceBalanceAmount = referenceBalanceAmount,
                OtherBillsToPay = otherBillsToPay,
                OtherBalanceAmount = otherBalanceAmount
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        //----------------Mukesh- End   ----------------------------------- //



        #region //Vishals purchase modules starts here
        //======================================================Vishal's Purchase Modules starts here===================================================================================
        #region//main view code for purchase by vishal pardeshi
        /// <summary>
        /// this action result methode for the purchase dashboard ...getting the all the purchases 
        /// </summary>
        /// <returns> this list of purchase</returns>
        [HttpGet]
        public async Task<ActionResult> DetailsPurchaseAsyncVP()
        {
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                //try
                //{
                objac.BranchCode = Session["BranchCode"].ToString();
                objac.StaffCode = Session["StaffCode"].ToString();
                List<Accountant> model = await ListPurchasesAsyncVP();
                objac.lstPurchaseVP = model;
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
          {
              new BreadcrumbItem { Name = "Dashboard", Url = "AccountantDashboardAsyncSGS" },
              new BreadcrumbItem { Name = "Purchase" },
          };
                ViewBag.Breadcrumbs = breadcrumbs;
                ViewBag.Currency = "&#x20b9;";
                //}
                //catch (Exception ex)
                //{
                //    throw (ex);
                //}
                Session["ListforFilter"] = model;
                Session["Currency"] = "&#x20b9;";
                return await Task.Run(() => View("DetailsPurchaseAsyncVP", objac));

            }
        }
        /// <summary>
        /// this action result methode is written for thr purchase purpose
        /// </summary>
        /// <param name="status"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>filter llst</returns>
        [HttpGet]
        public async Task<ActionResult> FilterPurchases(string status, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                List<Accountant> purchases = Session["ListforFilter"] as List<Accountant>;
                if (!string.IsNullOrEmpty(status) && status != "selectall")
                {
                    purchases = purchases.Where(p => p.Status == status).ToList();
                }

                if (startDate.HasValue)
                {
                    purchases = purchases.Where(p => p.TransactionDate >= startDate.Value).ToList();
                }

                if (endDate.HasValue)
                {
                    purchases = purchases.Where(p => p.TransactionDate <= endDate.Value).ToList();
                }
                ViewBag.Currency = Session["Currency"].ToString();
                Accountant obj = new Accountant { lstPurchaseVP = purchases };
                return await Task.Run(() => PartialView("_PurchaseListAsyncVP", obj));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return await Task.Run(() => View("error"));
            }
        }

        /// <summary>
        /// this action methode is wrriten for the getting the view for the add purchase 
        /// </summary>
        /// <returns>the viewbags for the dropdowns in the add purchase</returns>
        [HttpGet]
        public async Task<ActionResult> AddPurchaseAsyncVP()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                //try
                //{
                objac.BranchCode = Session["BranchCode"].ToString();
                objac.StaffCode = Session["StaffCode"].ToString();
                //breadcrumbs here
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
              {

                      new BreadcrumbItem { Name = "Dashboard", Url =Url.Action("AccountantDashboardAsyncSGS", "Accountant")  },
                      new BreadcrumbItem { Name = "Purchase", Url = Url.Action("DetailsPurchaseAsyncVP","Accountant") },
                      new BreadcrumbItem { Name = "Add Purchase" },
              };

                ViewBag.Breadcrumbs = breadcrumbs;
                //getting the last purchase code and making increment to it and inserting it to database
                objac.TransactionCode = await GetPurchaseCoedAsyncVP();
                objac.TransactionDate = DateTime.Now;
                await ListHsnCodeAsyncVP();//getting thehsncode link for dropdown
                                           // await ListTaxAsyncVP();//getting the applied tax viewbag from methode
                await PaymentmodesAsyncVP();//getting the payment modes to dropdown
                await ListVendorAsyncVP();//getting the vendor list for add purchase
                await objbal.ListClientDetailsAsyncVP(objac);
                ViewBag.Currency = "&#x20b9;";
                ViewBag.IsitEdit = false;//sending the viewbag for checking the view is edit or not
                                         //return await Task.Run(() => PartialView("AddPurchaseAsyncVP", objac));
                return await Task.Run(() => PartialView("AddPurchaseAsyncVP", objac));
                //catch (Exception ex)
                //{
                //    ViewBag.ErrorMessage = "An error occurred while processing the request." + ex;
                //    return View("Error");
                //}
            }
        }
        /// <summary>
        /// post methode for saving the purchase details to database
        /// </summary>
        /// <param name="objac"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddPurchaseAsyncVP(Accountant objac)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                //saving the details to database about the purchase
                try
                {
                    objac.TransactionCode = await GetPurchaseCoedAsyncVP();
                    objac.StaffCode = Session["StaffCode"].ToString();
                    objac.BranchCode = Session["BranchCode"].ToString();
                    await objbal.SavePurchaseAsyncVP(objac);
                    return Json(new { success = true, PurchaseCode = objac.TransactionCode }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Invalid purchase items data." + ex }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        /// <summary>
        /// this is the action result methode for the match voucher partial view
        /// </summary>
        /// <param name="TCode"></param>
        /// <param name="Amount"></param>
        /// <returns>partial view of match voucher</returns>
        [HttpGet]
        public async Task<ActionResult> MatchVoucherAsyncVM(string TCode, decimal? Amount)
        {
            Accountant obj = new Accountant();
            obj.BranchCode = Session["BranchCode"].ToString();
            obj.TransactionAmount = Amount;
            var parts = TCode.Split('-');
            obj.TransactionCode = parts[0];
            obj.VendorName = parts[1];
            await ListVoucherAsyncVP(parts[1]);//getting the voucher list for select2 dropdown
            return PartialView("_MatchVoucherAsyncVM", obj);
        }
        [HttpPost]
        public async Task<ActionResult> MatchVoucherAsyncVM(string TranscationCode, decimal TransactionAmount, List<Accountant> vouchers, string Description)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (vouchers == null || !vouchers.Any())
            {
                return Json(new { success = false, message = "No vouchers provided." });
            }



            // TransactionAmount.Value is used because we already checked for null
            decimal remainingPaidAmount = TransactionAmount;

            foreach (var voucher in vouchers)
            {
                if (remainingPaidAmount <= 0)
                {
                    break;
                }

                // Use Math.Min to determine the amount to use
                decimal test = voucher.Amount;
                decimal amountToUse = Math.Min(remainingPaidAmount, test);
                remainingPaidAmount -= amountToUse;
                decimal? newBalance = voucher.Amount - amountToUse;

                // Cast or convert to ensure type compatibility
                voucher.TransactionAmount = (decimal)amountToUse;
                voucher.TransactionCode = TranscationCode;
                voucher.Description = Description;

                try
                {
                    await objbal.SaveVoucherPurchaseAsyncVP(voucher);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error updating voucher {voucher.VoucherCode}: {ex.Message}" });
                }
            }

            // Return success message and redirect URL
            return Json(new { success = true, redirectUrl = Url.Action("DetailsPurchaseAsyncVP", "Accountant") }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// this action result methode is written for pop for add vendor 
        /// </summary>
        /// <returns>add vendor pop up</returns>
        [HttpGet]
        public async Task<ActionResult> AddVendorAsyncVP()
        {
            objac.BranchCode = Session["BranchCode"].ToString();
            SqlDataReader dr = await objbal.GetMaxVendorAsyncVP(objac);
            if (dr.Read())
            {
                string vendorcode = dr["VendorCode"].ToString();
                if (vendorcode == "")
                {
                    objac.VendorCode = "VND001";
                }
                else
                {
                    objac.VendorCode = objbal.IncrementPurchaseCode(vendorcode);
                }
            }
            ViewBag.Country = await objBALbind.GetCountry();
            return await Task.Run(() => PartialView("_AddVendorAsyncVP", objac));
        }
        public async Task<JsonResult> GetAppliedtaxAsyncVP(string hsncode, string VendorCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(hsncode))
                {
                    objac.HSNCode = hsncode;
                    DataSet ds = await objbal.ListtTaxAsyncVP(objac);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var taxOptions = new List<dynamic>();
                        if (VendorCode != null)
                        {
                            objac.VendorCode = VendorCode;
                            objac.BranchCode = Session["BranchCode"].ToString();
                            bool isintra = await objbal.VendorIsIntra(objac);
                            //intra state
                            if (isintra)
                            {
                                foreach (DataRow row in ds.Tables[0].Rows)
                                {
                                    var sgstPercentage = int.Parse(row["SGST"].ToString());
                                    var cgstPercentage = int.Parse(row["CGST"].ToString());

                                    if (sgstPercentage != 0 && cgstPercentage != 0)
                                    {
                                        taxOptions.Add(new { text = $"SGST-{sgstPercentage}% + CGST-{cgstPercentage}%", value = $"{sgstPercentage + cgstPercentage}" });
                                    }
                                }
                            }
                            //inter state
                            else
                            {
                                foreach (DataRow row in ds.Tables[0].Rows)
                                {
                                    var igstPercentage = row["IGST"].ToString();

                                    if (!string.IsNullOrEmpty(igstPercentage))
                                    {
                                        taxOptions.Add(new { text = $"IGST-{igstPercentage}%", value = igstPercentage });
                                    }
                                }
                            }
                        }
                        return Json(new { success = true, taxOptions = taxOptions }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }




        /// <summary>
        /// this action methode is written for the saving the vendor details to database
        /// </summary>
        /// <param name="Objac"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AddVendorAsyncVP(Accountant Objac)
        {

            //try
            //{
            Objac.StaffCode = Session["StaffCode"].ToString();
            await objbal.SaveVendorAsyncVP(Objac);
            return Json(new { success = true, redirectUrl = Url.Action("ListAllVouchersAsyncSGS", "Accountant") }, JsonRequestBehavior.AllowGet);
            //}
            //catch
            //{
            //}



        }
        #endregion


        /// <summary>
        /// this list methode is written for the fetching the Purchased item list 
        /// </summary>
        /// <param name="PurchaseCode"></param>
        /// <returns>this methode returns the list of purchased itms for update </returns>
        private async Task<(List<Accountant>, List<Accountant>)> ListPurchasItemsAsyncVP(string PurchaseCode)
        {
            //fetching the list of purchased itmes here
            List<Accountant> lstitems = new List<Accountant>();
            List<Accountant> lsttransaction = new List<Accountant>();
            objac.BranchCode = Session["BranchCode"].ToString();
            DataSet ds = await objbal.ListPurchasedItemsAsyncVP(objac);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Accountant objP = new Accountant();

                    objP.TransactionCode = PurchaseCode;
                    objP.ItemName = ds.Tables[0].Rows[i]["ItemName"].ToString();
                    objP.HSNCode = ds.Tables[0].Rows[i]["HSNCode"].ToString();
                    objP.Quantity = ds.Tables[0].Rows[i]["Quantity"].ToString() + "(" + ds.Tables[0].Rows[i]["Unit"].ToString() + ")";
                    objP.UnitPrice = decimal.Parse(ds.Tables[0].Rows[i]["UnitPrice"].ToString());
                    objP.Discount = ds.Tables[0].Rows[i]["Discount"] != DBNull.Value && !string.IsNullOrEmpty(ds.Tables[0].Rows[i]["Discount"].ToString())
                ? Convert.ToDecimal(ds.Tables[0].Rows[i]["Discount"])
                : 0;

                    objP.Tax = ds.Tables[0].Rows[i]["TaxRate"] != DBNull.Value
                               ? ds.Tables[0].Rows[i]["TaxRate"].ToString()
                               : "No tax";

                    objP.Balance = ds.Tables[0].Rows[i]["DiscountAmount"] != DBNull.Value && !string.IsNullOrEmpty(ds.Tables[0].Rows[i]["DiscountAmount"].ToString())
                                    ? Convert.ToDecimal(ds.Tables[0].Rows[i]["DiscountAmount"])
                                    : 0;

                    objP.AppliedTax = ds.Tables[0].Rows[i]["TaxAmount"] != DBNull.Value && !string.IsNullOrEmpty(ds.Tables[0].Rows[i]["TaxAmount"].ToString())
                                      ? decimal.Parse(ds.Tables[0].Rows[i]["TaxAmount"].ToString())
                                      : 0;

                    objP.TransactionAmount = ds.Tables[0].Rows[i]["TotalPrice"] != DBNull.Value && !string.IsNullOrEmpty(ds.Tables[0].Rows[i]["TotalPrice"].ToString())
                                             ? Convert.ToDecimal(ds.Tables[0].Rows[i]["TotalPrice"])
                                             : 0;


                    lstitems.Add(objP);
                }
            }
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    Accountant objT = new Accountant();

                    string mode = ds.Tables[1].Rows[i]["PaymentMode"].ToString();
                    objT.TransactionDate = Convert.ToDateTime(ds.Tables[1].Rows[i]["Date"]);
                    objT.VoucherCode = ds.Tables[1].Rows[i]["VoucherCode"].ToString();
                    objT.PaymentMode = mode;
                    if (mode == "BANK")
                    {
                        objT.TranId_CheqNo = ds.Tables[1].Rows[i]["TransactionId_ChequeNo"].ToString();
                    }
                    else if (mode == "CHEQUE")
                    {
                        objT.TranId_CheqNo = ds.Tables[1].Rows[i]["TransactionId_ChequeNo"].ToString();
                        objT.ChequeDate = string.IsNullOrEmpty(ds.Tables[1].Rows[i]["ChequeDate"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(ds.Tables[1].Rows[i]["ChequeDate"]);
                        objT.ChequeClearenceDate = string.IsNullOrEmpty(ds.Tables[1].Rows[i]["ChequeClearenceDate"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(ds.Tables[1].Rows[i]["ChequeClearenceDate"]);
                    }
                    objT.TransactionAmount = Convert.ToDecimal(ds.Tables[1].Rows[i]["Amount"]);
                    objT.BalanceAmount = Convert.ToDecimal(ds.Tables[1].Rows[i]["BalanceAmount"]);


                    lsttransaction.Add(objT);
                }
            }
            return (lstitems, lsttransaction);
        }
        /// <summary>
        /// the action is to return the list for the pending purchase list
        /// </summary>
        /// <returns>Viewbag for Purchase and list for the all purchase</returns>
        private async Task<List<Accountant>> ListPurchasesAsyncVP()
        {
            objac.BranchCode = Session["BranchCode"].ToString();
            objac.StaffCode = Session["StaffCode"].ToString();
            DataSet ds = await objbal.ListPurchasesAsyncVP(objac);
            List<Accountant> lstpurchase = new List<Accountant>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Accountant objP = new Accountant
                    {
                        TransactionCode = row.IsNull("TransactionCode") ? string.Empty : row["TransactionCode"].ToString(),
                        VendorName = row.IsNull("VendorName") ? string.Empty : row["VendorName"].ToString(),
                        ItemName = row.IsNull("Items") ? string.Empty : row["Items"].ToString(),
                        TransactionAmount = row.IsNull("TransactionAmount") ? 0.0m : Convert.ToDecimal(row["TransactionAmount"]),
                        BalanceAmount = row.IsNull("BalanceAmount") ? 0.0m : Convert.ToDecimal(row["BalanceAmount"]),
                        TransactionDate = row.IsNull("TransactionDate") ? DateTime.MinValue : Convert.ToDateTime(row["TransactionDate"]),
                        Description = row.IsNull("Description") ? string.Empty : row["Description"].ToString(),
                        PaymentMode = row.IsNull("PaymentMode") ? string.Empty : row["PaymentMode"].ToString(),
                        TranId_CheqNo = row.IsNull("TransactionID") ? string.Empty : row["TransactionID"].ToString(),
                        Status = row.IsNull("Status") ? string.Empty : row["Status"].ToString()
                    };

                    lstpurchase.Add(objP);
                }
            }
            return lstpurchase;
        }
        /// <summary>
        /// this mthode is written for the validating the purchase here 
        /// </summary>
        /// <param name="PurchaseCode"></param>
        /// <returns>true/false</returns>
        [HttpPost]
        public async Task<ActionResult> ValidatePurchaseAsyncVP(string PurchaseCode)
        {
            objac.PurchaseCode = PurchaseCode;
            SqlDataReader dr;
            objac.BranchCode = Session["BranchCode"].ToString();
            dr = await objbal.ValidatePurchaseAsyncVP(objac);
            if (dr.Read())
            {
                objac.VendorName = dr["VendorName"].ToString();
                //purchase code exists in the database
                return await Task.Run(() => Json(new { success = false }, JsonRequestBehavior.AllowGet));
            }
            else
            {
                //purchase code doesn't exists
                return await Task.Run(() => Json(new { success = true }, JsonRequestBehavior.AllowGet));
            }
        }
        /// <summary>
        /// validating the vendor name here for add vendor form
        /// </summary>
        /// <param name="VendorName1"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> ValidateVendorAsync(string VendorName1)
        {

            if (!string.IsNullOrEmpty(VendorName1))
            {
                objac.VendorName = VendorName1;
                SqlDataReader dr = await objbal.ValidateVendorAsyncVP(objac);

                if (dr.Read())
                {
                    return Json(new { success = false, message = "Vendor name already exists." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false, message = "Invalid vendor name or vendor list." }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// this action result methode is for the saving the purchased items details
        /// </summary>
        /// <param name="PurchaseItemsAsyncVP"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddPurchaseItemAsyncVP(List<Accountant> PurchaseItemsAsyncVP)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                //try
                //{

                //saving the details to database about the purchase
                Accountant objpi = new Accountant();
                foreach (var item in PurchaseItemsAsyncVP)
                {
                    objpi.TransactionCode = item.TransactionCode;
                    objpi.ItemName = item.ItemName;
                    objpi.Unit = item.Unit;
                    objpi.Quantity = item.Quantity;
                    objpi.HSNCode = item.HSNCode;
                    objpi.UnitPrice = item.UnitPrice;
                    objpi.Discount = item.Discount;
                    //objpi.AppliedTax = item.AppliedTax;
                    await objbal.SavePurchasedItemsAsyncVP(objpi);
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);

                //}
                //catch (Exception ex)
                //{
                //    return Json(new { success = false, message = "Invalid purchase items data." + ex });

                //}
            }
        }

        /// <summary>
        /// this action methode written for the viewing the purchase bill for the purchase dashboard
        /// </summary>
        /// <returns>the purchase bill for selected purchase</returns>
        [HttpGet]
        public async Task<ActionResult> ViewPurchaseAsyncVP(string PurchaseCode)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                try
                {
                    //bread crumbs here
                    List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
  {

      new BreadcrumbItem { Name = "Dashboard", Url =Url.Action("AccountantDashboardAsyncSGS", "Accountant")  },
      new BreadcrumbItem { Name = "Purchase", Url = Url.Action("DetailsPurchaseAsyncVP","Accountant") },
      new BreadcrumbItem { Name = "View Purchase" },

  };

                    ViewBag.Breadcrumbs = breadcrumbs;
                    objac.PurchaseCode = PurchaseCode;
                    objac.BranchCode = Session["BranchCode"].ToString();
                    SqlDataReader dr;
                    SqlDataReader drClient;
                    dr = await objbal.ListPurchasesDetailsAsyncVP(objac);
                    drClient = await objbal.ListClientDetailsAsyncVP(objac);
                    if (dr.Read() && drClient.Read())
                    {
                        //client details
                        ViewBag.ClientName = drClient["ClientName"].ToString();
                        ViewBag.Logo = drClient["Logo"].ToString();
                        ViewBag.BranchName = drClient["BranchName"].ToString();
                        // ViewBag.BranchCity = drClient["BranchCity"].ToString();
                        ViewBag.BranchAddress = drClient["BranchAddress"].ToString();
                        ViewBag.BranchContact = drClient["ContactNo"].ToString();
                        ViewBag.BranchEmailId = drClient["BranchEmailId"].ToString();
                        //vendor details
                        ViewBag.VendorName = dr["VendorName"].ToString();//VendorCityId,v.VendorContact,v.VendorEmail
                        ViewBag.VendorCity = dr["CityName"].ToString();
                        ViewBag.VendorAddress = dr["VendorAddress"].ToString();
                        ViewBag.VendorNo = dr["VendorContact"].ToString();
                        ViewBag.VendorMail = dr["VendorEmail"].ToString();
                        //transaction details
                        DateTime transactionDate = Convert.ToDateTime(dr["TransactionDate"]);
                        ViewBag.TransactionDate = transactionDate.ToString("dd-MM-yyyy");
                        ViewBag.TransactionCode = PurchaseCode;
                        ViewBag.BillNumber = dr["BillNumber"].ToString();
                        ViewBag.TransactionAmount = dr["TransactionAmount"].ToString() == "0" ? 0 : decimal.Parse(dr["TransactionAmount"].ToString());
                        ViewBag.BalanceAmount = dr["BalanceAmount"].ToString() == "0" ? 0 : decimal.Parse(dr["BalanceAmount"].ToString());
                        //ViewBag.PaymentMode = dr["PaymentMode"].ToString();
                        //ViewBag.TranId_CheqNo = dr["TransactionID_checqueNumber"].ToString();
                        ViewBag.Description = dr["Description"].ToString();
                    }
                    //fetching the purchased item list here
                    var (listitem, listtransaction) = await ListPurchasItemsAsyncVP(PurchaseCode);
                    objac.lstPurchaseItemVP = listitem;
                    objac.lstTransactionVP = listtransaction;
                    ViewBag.transactionList = listtransaction;
                    //giving the currency hard coded
                    ViewBag.Currency = "&#x20b9;";
                    return await Task.Run(() => View("ViewPurchaseAsyncVP", objac));
                }
                catch
                {
                    return await Task.Run(() => View("Error"));
                }

            }
        }
        /// <summary>
        /// this action result methode for getting the details for updating the purchase in purchase dashboard
        /// </summary>
        /// <param name="TransactionCode"></param>
        /// <returns>this returns the saved details for selected purchase</returns>
        [HttpGet]
        public async Task<ActionResult> UpdatePurchaseAsyncVP(string PurchaseCode)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Accountant accountant = new Accountant();
                //bread crumbs here
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
          {

                  new BreadcrumbItem { Name = "Dashboard", Url =Url.Action("AccountantDashboardAsyncSGS", "Accountant")  },
                  new BreadcrumbItem { Name = "Purchase", Url = Url.Action("DetailsPurchaseAsyncVP","Accountant") },
                  new BreadcrumbItem { Name = "Update Purchase" },
          };

                ViewBag.Breadcrumbs = breadcrumbs;
                //getting the details from the database for this purchase code
                objac.TransactionCode = PurchaseCode;
                objac.PurchaseCode = PurchaseCode;
                objac.BranchCode = Session["BranchCode"].ToString();
                ViewBag.Currency = "&#x20b9;";
                ViewBag.IsitEdit = true;
                SqlDataReader dr = await objbal.ListPurchasesDetailsAsyncVP(objac);
                if (dr.Read())
                {
                    accountant.BillNumber = dr["BillNumber"].ToString();
                    accountant.TransactionCode = PurchaseCode;
                    accountant.VendorName = dr["VendorName"].ToString();
                    accountant.TransactionDate = Convert.ToDateTime(dr["TransactionDate"].ToString());
                    accountant.BalanceAmount = decimal.Parse(dr["BalanceAmount"].ToString());
                    accountant.Description = dr["Description"].ToString();
                    ViewBag.VendorCode = dr["VendorCode"].ToString();
                }
                await ListHsnCodeAsyncVP();//getting thehsncode link for dropdown
                await ListVendorAsyncVP();//getting the vendor list
                List<Accountant> lst1 = new List<Accountant>();
                List<Accountant> lst2 = new List<Accountant>();

                ViewBag.IsGSTBill = await ListPurchasedItemsAsyncVP(objac);//getting the list of purchased items
                await PaymentmodesAsyncVP();//getting the payment modes to dropdown
                return await Task.Run(() => View("AddPurchaseAsyncVP", accountant));
            }
        }
        /// <summary>
        /// this methode is wrriten to get the purchased items list for the purchase doen or pending
        /// </summary>
        /// <param name="objA"></param>
        /// <returns>ViewBag.ListofItems</returns>
        [HttpGet]
        private async Task<bool> ListPurchasedItemsAsyncVP(Accountant objA)
        {
            bool IsGSTbill = false;
            //fetching the list of purchased itmes here
            List<Accountant> lstitems = new List<Accountant>();
            DataSet ds = await objbal.ListPurchasedItemsAsyncVP(objA);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Accountant objP = new Accountant();
                    objP.ItemId = int.Parse(ds.Tables[0].Rows[i]["PurchaseItemId"].ToString());
                    objP.TransactionCode = objA.PurchaseCode;
                    objP.ItemName = ds.Tables[0].Rows[i]["ItemName"].ToString();
                    objP.HSNCode = ds.Tables[0].Rows[i]["HSNCode"].ToString();
                    if (objP.HSNCode.Length > 2)
                    {
                        IsGSTbill = true;
                    }
                    else
                    {
                        IsGSTbill = false;
                    }
                    objP.Unit = ds.Tables[0].Rows[i]["Unit"].ToString().Trim();
                    objP.Quantity = ds.Tables[0].Rows[i]["Quantity"].ToString();
                    objP.UnitPrice = decimal.Parse(ds.Tables[0].Rows[i]["UnitPrice"].ToString());
                    objP.Discount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Discount"].ToString());
                    //objP.AppliedTax = decimal.Parse(ds.Tables[0].Rows[i]["foruddl"].ToString());
                    //objP.Tax = ds.Tables[0].Rows[i]["foruddl"].ToString();
                    lstitems.Add(objP);
                }
            }
            ViewBag.ListofItems = lstitems;
            return IsGSTbill;
        }
        /// <summary>
        /// updating the purchase details in transactions for purchase
        /// </summary>
        /// <param name="objA"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UpdatePurchaseAsyncVP(Accountant objA)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                //try
                //{

                objA.StaffCode = Session["StaffCode"].ToString();
                await objbal.UpdatePurchaseAsyncVP(objA);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                //}
                //catch
                //{
                //    return await Task.Run(() => View("Error"));
                //}
            }
        }
        [HttpPost]
        public async Task<ActionResult> UpdatePurchaseItemAsyncVP(List<Accountant> PurchaseItemsAsyncVP)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                try
                {

                    Accountant objpi = new Accountant();
                    foreach (var item in PurchaseItemsAsyncVP)
                    {
                        objpi.ItemId = item.ItemId;
                        objpi.TransactionCode = item.TransactionCode;
                        objpi.TransactionCode = item.TransactionCode;
                        objpi.ItemName = item.ItemName;
                        objpi.Quantity = item.Quantity;
                        objpi.HSNCode = item.HSNCode;
                        objpi.UnitPrice = item.UnitPrice;
                        objpi.Discount = item.Discount;
                        //objpi.AppliedTax = item.AppliedTax;

                        if (objpi.ItemId == 0 || objpi.ItemId == null)
                        {

                            await objbal.SavePurchasedItemsAsyncVP(objpi);
                        }
                        else
                        {
                            await objbal.UpdatePurchasedItemsAsyncVP(objpi);
                            //update items in the purchased item table
                        }

                    }
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        /// <summary>
        /// fetching the last purchase code and making the increment by 1 and sending it to add purchase form
        /// </summary>
        /// <param name="PurchaseCode"></param>
        /// <returns></returns>
        [HttpGet]
        private async Task<string> GetPurchaseCoedAsyncVP()
        {
            objac.BranchCode = Session["BranchCode"].ToString();
            objac.StaffCode = Session["StaffCode"].ToString();
            string newPurchaseCode = await objbal.GetTaskPurchaseCode(objac);
            return newPurchaseCode;
        }
        /// <summary>
        /// fetching the status here any bropdown in purchase i need
        /// </summary>
        /// <param name="Bank"></param>
        /// <returns></returns>
        [HttpGet]
        private async Task ListStatusAsyncVP()
        {
            //fetching the banks here for the add purchase 
            DataSet ds = await objbal.ListStatusAsyncVP();
            List<SelectListItem> StatusList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StatusList.Add(new SelectListItem { Text = dr["Status"].ToString(), Value = dr["StatusId"].ToString() });
            }
            ViewBag.StatusId = StatusList;
        }
        /// <summary>
        /// this methode is written for getting the vendor details from database
        /// </summary>
        /// <param name="ObjAc"></param>
        /// <returns>ViewBag.VendorName</returns>
        public async Task<JsonResult> ListVendorAsyncVP()
        {
            //fetching the banks here for the add purchase 
            objac.BranchCode = Session["BranchCode"].ToString();
            DataSet ds = await objbal.ListVendorAsyncVP(objac);
            List<SelectListItem> Vendoerlist = new List<SelectListItem>();
            foreach (DataRow ds1 in ds.Tables[0].Rows)
            {
                Vendoerlist.Add(new SelectListItem { Text = ds1["VendorName"].ToString(), Value = ds1["VendorCode"].ToString() });
            }
            ViewBag.Vendoerlist = Vendoerlist;
            return Json(new { success = true,data= Vendoerlist },JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// this methode is wrriten for the all the payments modes for the purchase module ie. cahs and bank
        /// </summary>
        /// <returns>viewbag for the payment mode for add purchase</returns>
        private async Task PaymentmodesAsyncVP()
        {

            List<SelectListItem> lstp = new List<SelectListItem>
      {
          new SelectListItem { Value = "CASH", Text = "CASH" },
          new SelectListItem { Value = "BANK", Text = "BANK" },
          new SelectListItem { Value = "CHEQUE", Text = "CHEQUE" }
      };
            await Task.Run(() => ViewBag.PaymentModes = lstp);
            //return await Task.Run(() => lstp);
        }

        ///<summary>
        ///This action methode for the getting the list of the vouchers from the database
        /// </summary>
        /// <param name=""></param>
        /// <return>ListVoucherList</return>
        [HttpGet]
        public async Task<JsonResult> ListVoucherAsyncVP(string VendorName)
        {
            if (Session["StaffCode"] == null)
            {
                //return RedirectToAction("Login", "Account");
                return Json(new { success = false, message = "cannot find the user " }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    if (VendorName != null)
                    {
                        objac.BranchCode = Session["BranchCode"].ToString();
                        //fetching the banks here for the add purchase 
                        objac.VendorName = VendorName;
                        DataSet ds = await objbal.ListVouchersAsyncVP(objac);
                        List<SelectListItem> VoucherList = new List<SelectListItem>();
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            VoucherList.Add(new SelectListItem { Text = $"{dr["PaymentMode"].ToString() + ":" + dr["VoucherCode"].ToString() + ":" + dr["PaidTo"].ToString() + ":" + dr["Balance"].ToString()}", Value = dr["VoucherCode"].ToString() });
                        }

                        ViewBag.VoucherCode = VoucherList;
                        return await Task.Run(() => Json(new { success = true, data = VoucherList }, JsonRequestBehavior.AllowGet));
                    }
                    else
                    {
                        return await Task.Run(() => Json(new { success = true, message = "vendor code is null" }, JsonRequestBehavior.AllowGet));
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "There is this problem:" + ex }, JsonRequestBehavior.AllowGet);
                }

            }
        }
        /// <summary>
        /// this action result methode is wrriten for the Listing the HSN code and Category. for the dropdown for the add purchase
        /// </summary>
        /// <returns>the list of the HSNcode to view</returns>
        [HttpGet]
        private async Task ListHsnCodeAsyncVP()
        {
            DataSet ds = await objbal.ListHSNCategoryAsyncVP();
            List<SelectListItem> HsnList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                HsnList.Add(new SelectListItem { Text = dr["text"].ToString(), Value = dr["value"].ToString() });
            }
            ViewBag.HSNCode = HsnList;
        }
        /// <summary>
        /// this json result methode is wrritten for deleting purchased items from table so updated entires can be entred into table
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns>Json</returns>
        [HttpPost]
        public async Task<ActionResult> DeletePurchaseItemAsyncVP(int itemId)
        {
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                try
                {
                    if (itemId != 0)
                    {
                        objac.ItemId = itemId;
                        await objbal.DeletePurchasedItemAsyncVP(objac);
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "itemid is null" }, JsonRequestBehavior.AllowGet);

                    }

                }
                catch
                {
                    return Json(new { success = false });
                }
            }
        }
        //------------------------------------Vishal's Purchase Modules ends here------------------------------------------------------------

        #endregion



        //------------------Ajay Start ----------------------------------------------------------//

        /// <summary>
        /// Show Cash Transactions Base View
        /// </summary>
        /// <returns>List of Cash Transactions</returns>
        public async Task<ActionResult> CashTransactionsAsyncAN()
        {
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                objac.BranchCode = Session["BranchCode"].ToString();
                SqlDataReader dr = await objbal.CashInHandAN(objac);

                if (dr.Read())
                {
                    //Session["TotalExpense"] = dr["TotalExpense"];
                    string Expense = string.Format(new System.Globalization.CultureInfo("en-IN"), "{0:N2} ₹", dr["TotalExpense"]);
                    string Collection = string.Format(new System.Globalization.CultureInfo("en-IN"), "{0:N2} ₹", dr["TotalCollection"]);
                    string CurrentBalance = string.Format(new System.Globalization.CultureInfo("en-IN"), "{0:N2} ₹", dr["CashInHand"]);
                    ViewBag.TotalExpense = Expense;
                    ViewBag.TotalCollection = Collection;
                    ViewBag.CurrentBalance = CurrentBalance;
                }

                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                {
                new BreadcrumbItem { Name = "Dashboard", Url = "AccountantDashboardAsyncSGS" },
                new BreadcrumbItem { Name = "Cash Transactions" },
                };
                ViewBag.Breadcrumbs = breadcrumbs;
                return await Task.Run(() => View());
            }
        }

        /// <summary>
        /// Show the list of Cash Recieved Form Students or Any Other ways
        /// </summary>
        /// <returns>Cash Reciept List</returns>
        [HttpGet]
        public async Task<ActionResult> CashReceiptListAsyncAN(string id)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                objac.BranchCode = Session["BranchCode"].ToString();
                DataSet ds = await objbal.CashReceiptListAsyncAN(objac);
                List<Accountant> lstCashrcvd = new List<Accountant>();
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Accountant objCash = new Accountant();
                        objCash.FeeCollectionCode = row["FeesCollectioncode"].ToString();
                        objCash.Date = Convert.ToDateTime(row["TransactionDate"].ToString());
                        objCash.StudentName = row["FullName"].ToString();
                        objCash.Amount = Convert.ToDecimal(row["TransactionAmount"].ToString());
                        lstCashrcvd.Add(objCash);
                    }
                }
                objac.lstCashList = lstCashrcvd;
                ViewBag.RegularExpense = ds;
                Session["ListForFilter"] = lstCashrcvd;
                Session["ID"] = id;

                return PartialView(objac);
            }
        }

        /// <summary>
        /// List of Expense through Cash
        /// </summary>
        /// <returns>Expense Cash List</returns>
        public async Task<ActionResult> CashExpenceListAsyncAN(string id)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                objac.BranchCode = Session["BranchCode"].ToString();
                DataSet ds = await objbal.CashExpenceListAsyncAN(objac);
                List<Accountant> lstCashExp = new List<Accountant>();
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Accountant objCash = new Accountant();
                        objCash.TransactionCode = row["TransactionCode"].ToString();
                        objCash.Date = Convert.ToDateTime(row["TransactionDate"].ToString());
                        objCash.VendorName = row["Name"].ToString();
                        objCash.Amount = Convert.ToDecimal(row["TransactionAmount"].ToString());
                        lstCashExp.Add(objCash);
                    }
                }
                objac.lstCashList = lstCashExp;
                ViewBag.RegularExpense = ds;
                Session["ListForFilter"] = lstCashExp;
                Session["ID"] = id;

                return PartialView(objac);
            }
        }

        public async Task<ActionResult> CashDepositTransactions(string id)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                objac.BranchCode = Session["BranchCode"].ToString();
                DataSet ds = await objbal.CashDepositTransactions();
                List<Accountant> lstCashExp = new List<Accountant>();
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Accountant objCash = new Accountant();
                        objCash.TransactionCode = row["TransactionCode"].ToString();
                        objCash.Date = Convert.ToDateTime(row["TransactionDate"].ToString());
                        objCash.VendorName = row["Name"].ToString();
                        objCash.Amount = Convert.ToDecimal(row["TransactionAmount"].ToString());
                        lstCashExp.Add(objCash);
                    }
                }
                objac.lstCashList = lstCashExp;
                ViewBag.RegularExpense = ds;
                Session["ListForFilter"] = lstCashExp;
                Session["ID"] = id;

                return PartialView(objac);
            }
        }

        [HttpGet]
        public async Task<ActionResult> CashToBankAN(string currentBalance)
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                // Bank Account List
                List<SelectListItem> BankAccountList = new List<SelectListItem>();
                objac.StaffCode = Session["StaffCode"].ToString();
                objac.BranchCode = Session["BranchCode"].ToString();
                DataSet ds2 = await objbal.BankAccountforVoucherAsyncSGS(objac);
                List<SelectListItem> BankList = new List<SelectListItem>();
                foreach (DataRow dr in ds2.Tables[0].Rows)
                {
                    string bankName = dr["BankName"].ToString();
                    string accountHolderName = dr["AccountHolderName"].ToString();
                    string accountNumber = dr["AccountNumber"].ToString();

                    // Masking the account number
                    string maskedAccountNumber = "****" + accountNumber.Substring(accountNumber.Length - 4);

                    BankList.Add(new SelectListItem
                    {
                        Text = $"{bankName} - {accountHolderName} - {maskedAccountNumber}",
                        Value = dr["BankId"].ToString()
                    });
                }
                BankAccountList.AddRange(BankList);
                ViewBag.BankAccountList = BankAccountList;

                ViewBag.CurrentBalance = currentBalance;
                // Load any necessary data for the partial view
                return await Task.Run(() => PartialView("CashToBankAN")); // Replace with your partial view name
            }
        }

        [HttpPost]
        public async Task<ActionResult> CashToBankAN(Accountant model)
        {


            decimal currentBalance = model.Balance;
            decimal enteredAmount = model.Amount;
            string paymentType = model.PaymentType;
            int bankId = model.BankId;
            DateTime DepositDate = model.BankAccountOpeningDate;
            DateTime currentTime = DateTime.Now;

            // Combine DepositDate (date part) with current time
            DateTime combinedDateTime = new DateTime(
                DepositDate.Year,
                DepositDate.Month,
                DepositDate.Day,
                currentTime.Hour,
                currentTime.Minute,
                currentTime.Second
            );

            // Save the combined date and time into a new variable
            DateTime savedDepositDateTime = combinedDateTime;

            model.StaffCode = Session["StaffCode"].ToString();
            objac.BranchCode = Session["BranchCode"].ToString();
            model.StaffCode = Session["StaffCode"].ToString();
            model.BankId = bankId;
            ////model.VoucherCode = await objbal.GetMaxVoucherCodeAsyncSGS(objac);
            model.BankAccountOpeningDate = savedDepositDateTime;
            await objbal.CashToBankAN(model);
            TempData["SuccessMessage"] = "Transaction saved successfully!";
            return RedirectToAction("CashTransactionsAsyncAN");
        }

        [HttpGet]
        public async Task<ActionResult> Filterforlist(string Status, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                List<Accountant> purchases = Session["ListforFilter"] as List<Accountant>;

                if (startDate.HasValue)
                {
                    purchases = purchases.Where(p => p.Date >= startDate.Value).ToList();
                }

                if (endDate.HasValue)
                {
                    purchases = purchases.Where(p => p.Date <= endDate.Value).ToList();
                }
                Accountant obj = new Accountant { lstCashList = purchases };
                String BankName = Session["ID"].ToString();

                if (BankName == "1")
                {
                    return await Task.Run(() => PartialView("CashReceiptListAsyncAN", obj));
                }

                else if (BankName == "2")
                {
                    return await Task.Run(() => PartialView("CashExpenceListAsyncAN", obj));
                }
                else
                {
                    return await Task.Run(() => PartialView("CashDepositTransactions", obj));
                }

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return await Task.Run(() => View("error"));
            }
        }

        /// <summary>
        /// Cash Expense Transaction View
        /// </summary>
        /// <param name="TransactionCode"></param>
        /// <returns></returns>
        public async Task<ActionResult> CashExpenseViewAN(string TransactionCode)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Accountant objView = new Accountant();
                objView.TransactionCode = TransactionCode;
                objView.BranchCode = Session["BranchCode"].ToString();
                SqlDataReader dr = await objbal.CashDeductionTransferViewAN(objView);

                if (dr.Read())
                {
                    objView.VendorName = dr["Name"].ToString();
                    objView.TransactionCode = dr["TransactionCode"].ToString();
                    objView.Date = Convert.ToDateTime(dr["TransactionDate"].ToString());
                    objView.Amount = Convert.ToDecimal(dr["TransactionAmount"].ToString());
                    objView.Description = dr["Description"].ToString();
                    objView.Status = dr["Status"].ToString();
                    objView.ExpenseCategory = dr["ExpenseCategory"].ToString();
                }

                dr.Close();

                return PartialView("CashExpenseViewAN", objView);
            }
        }

        /// <summary>
        /// Cash Reciept Transaction View Details
        /// </summary>
        /// <param name="TransactionId"></param>
        /// <returns>View</returns>
        public async Task<ActionResult> CashRecievedViewAN(string TransactionId)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Accountant objView = new Accountant();
                objView.TransactionCode = TransactionId;
                SqlDataReader dr = await objbal.CashRecievedTransferViewAN(objView);

                if (dr.Read())
                {
                    objView.Date = Convert.ToDateTime(dr["TransactionDate"].ToString());
                    objView.Amount = Convert.ToDecimal(dr["TransactionAmount"].ToString());
                    objView.Description = dr["Description"].ToString();
                    objView.StudentName = dr["FullName"].ToString();
                    objView.EmailId = dr["EmailId"].ToString();
                    objView.ContactNumber = dr["ContactNumber"].ToString();
                    objView.PaymentType = dr["FeesType"].ToString();
                }

                dr.Close();

                return PartialView("CashRecievedViewAN", objView);
            }
        }

        /// <summary>
        /// Return List of Bank Account List
        /// </summary>
        /// <param name="obj1"></param>
        /// <returns>Account List</returns>
        public async Task<ActionResult> BankAccountListAsyncAN(Accountant obj1)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                obj1.BranchCode = Session["BranchCode"].ToString();
                DataSet ds = await objbal.BankAccountsListAN(obj1);
                List<Accountant> lstBanklist = new List<Accountant>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Accountant obj = new Accountant();
                    obj.BankId = Convert.ToInt32(ds.Tables[0].Rows[i]["BankId"].ToString());
                    obj.BankName = ds.Tables[0].Rows[i]["BankName"].ToString();
                    obj.BankAccountNumber = Convert.ToInt64(ds.Tables[0].Rows[i]["AccountNumber"].ToString());
                    obj.AccountHolderName = ds.Tables[0].Rows[i]["AccountHolderName"].ToString();
                    obj.AccountType = ds.Tables[0].Rows[i]["AccountType"].ToString();
                    obj.BankBrach = ds.Tables[0].Rows[i]["Branch"].ToString();
                    obj.BankAmount = float.Parse(ds.Tables[0].Rows[i]["FinalBalance"].ToString());
                    obj.TransactionCount = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalTransactions"].ToString());
                    lstBanklist.Add(obj);
                }
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
           {
               new BreadcrumbItem { Name = "Dashboard", Url = "AccountantDashboardAsyncSGS" },
               new BreadcrumbItem { Name = "Bank Account List", Url = "" },
           };
                ViewBag.Breadcrumbs = breadcrumbs;
                objac.lstBankAccounts = lstBanklist;
                return await Task.Run(() => View(objac));
            }
        }
        public async Task<ActionResult> BankAccountAsyncAN()
        {
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
 {
     new BreadcrumbItem { Name = "Dashboard", Url = "AccountantDashboardAsyncSGS" },
     new BreadcrumbItem { Name = "Bank Account" },
 };
                ViewBag.Breadcrumbs = breadcrumbs;
                return await Task.Run(() => View());
            }
        }

        [Route("Bank Account Statement")]
        public async Task<ActionResult> AccountStatementAN(int BankId)
        {
            ViewBag.BankId = BankId;
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Accountant objDetails = new Accountant();
                objDetails.BankId = BankId;
                objDetails.BranchCode = Session["BranchCode"].ToString();
                SqlDataReader dr = await objbal.BankAccountTransactionDetailsAsync(objDetails);

                if (dr.Read())
                {
                    Session["BankName"] = dr["BankName"].ToString();
                    Session["IFSCCode"] = dr["IFSCCode"].ToString();
                    Session["OpeningBalance"] = dr["FinancialYearOpeningBalance"].ToString();
                    Session["BankAccountNumber"] = Convert.ToInt64(dr["AccountNumber"].ToString());
                    Session["AccountHolderName"] = dr["AccountHolderName"].ToString();
                    Session["BankAmount"] = float.Parse(dr["Balance"].ToString());
                }
                dr.Close();


                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
{
    new BreadcrumbItem { Name = "Dashboard", Url = "AccountantDashboardAsyncSGS" },
              new BreadcrumbItem { Name = "Bank Account List", Url = "BankAccountListAsyncAN" },
              new BreadcrumbItem { Name = "Bank Account Statement" },
};
                ViewBag.Breadcrumbs = breadcrumbs;
                return await Task.Run(() => View());
            }
        }

        /// <summary>
        /// Bank Account Statement Partial View to Show all Transactions
        /// </summary>
        /// <param name="BankId"></param>
        /// <returns>Bank Statement</returns>
        [HttpGet]
        public async Task<ActionResult> _ListAccountStatementAN(int BankId)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Accountant objDetails = new Accountant();
                objDetails.BankId = BankId;
                objDetails.BranchCode = Session["BranchCode"].ToString();
                SqlDataReader dr = await objbal.BankAccountTransactionDetailsAsync(objDetails);

                if (dr.Read())
                {
                    Session["BankName"] = dr["BankName"].ToString();
                    Session["BankAccountNumber"] = Convert.ToInt64(dr["AccountNumber"].ToString());
                    Session["AccountHolderName"] = dr["AccountHolderName"].ToString();
                    Session["BankAmount"] = float.Parse(dr["Balance"].ToString());
                }
                dr.Close();


                DataSet ds = await objbal.BankTransactionHistory(objDetails);
                List<Accountant> lstBankTrans = new List<Accountant>();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Accountant obj = new Accountant();
                    obj.Date = Convert.ToDateTime(ds.Tables[0].Rows[i]["TransactionDate"].ToString());
                    obj.TransactionCode = ds.Tables[0].Rows[i]["TransactionCode"].ToString();
                    obj.Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                    obj.ReceiverBankAccountHolderName = ds.Tables[0].Rows[i]["BankAccountHolderName"].ToString();
                    obj.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    obj.TransactionId = ds.Tables[0].Rows[i]["TransactionId"].ToString();
                    obj.TransactionType = ds.Tables[0].Rows[i]["TransactionType"].ToString();
                    obj.PaymentType = ds.Tables[0].Rows[i]["PaymentType"].ToString();
                    obj.Balance = decimal.Parse(ds.Tables[0].Rows[i]["RunningBalance"].ToString());

                    lstBankTrans.Add(obj);
                }


                objac.lstCashList = lstBankTrans;
                ViewBag.RegularExpense = ds;
                Session["AccountStatement"] = lstBankTrans;

                return PartialView(objac);
            }
        }

        /// <summary>
        /// Used to Filter Bank Account Statement With Transaction Date
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>Filtered Transactions</returns>

        [HttpGet]
        public async Task<ActionResult> FilterforStatementlist(string Status, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                List<Accountant> purchases = Session["AccountStatement"] as List<Accountant>;

                if (startDate.HasValue)
                {
                    purchases = purchases.Where(p => p.Date >= startDate.Value).ToList();
                }

                if (endDate.HasValue)
                {
                    purchases = purchases.Where(p => p.Date <= endDate.Value).ToList();
                }
                Accountant obj = new Accountant { lstCashList = purchases };
                // String BankName = Session["ID"].ToString();

                return await Task.Run(() => PartialView("_ListAccountStatementAN", obj));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return await Task.Run(() => View("error"));
            }
        }

        public async Task<ActionResult> _AccountListAsyncAN()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                objac.BranchCode = Session["BranchCode"].ToString();
                DataSet ds = await objbal.BankAccountsListAN(objac);
                List<Accountant> lstBanklist = new List<Accountant>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Accountant obj = new Accountant();
                    obj.BankId = Convert.ToInt32(ds.Tables[0].Rows[i]["BankId"].ToString());
                    obj.BankName = ds.Tables[0].Rows[i]["BankName"].ToString();
                    obj.BankAccountNumber = Convert.ToInt64(ds.Tables[0].Rows[i]["AccountNumber"].ToString());
                    obj.AccountHolderName = ds.Tables[0].Rows[i]["AccountHolderName"].ToString();
                    obj.AccountType = ds.Tables[0].Rows[i]["AccountType"].ToString();
                    obj.BankBrach = ds.Tables[0].Rows[i]["Branch"].ToString();
                    obj.BankAmount = float.Parse(ds.Tables[0].Rows[i]["FinalBalance"].ToString());
                    obj.TransactionCount = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalTransactions"].ToString());
                    lstBanklist.Add(obj);
                }
                objac.lstBankAccounts = lstBanklist;
                return PartialView("_AccountListAsyncAN", objac);
            }
        }

        [HttpGet]
        public async Task<JsonResult> ValidateAccountNumberAsync(string AccountNumber)
        {

            if (!string.IsNullOrEmpty(AccountNumber))
            {
                objac.AccountNumber = AccountNumber;
                SqlDataReader dr = await objbal.ValidateBanAccountAsyncVP(objac);

                if (dr.Read())
                {
                    return Json(new { success = false, message = "Bank Account already exists." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false, message = "Invalid Account" }, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Add New Bank Account GET Method
        /// </summary>
        /// <returns>Add bank</returns>
        [HttpGet]
        public async Task<ActionResult> AddNewBankAccountAsyncAN()
        {


            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                try
                {
                    objac.BranchCode = Session["BranchCode"].ToString();
                    objac.StaffCode = Session["StaffCode"].ToString();
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                return PartialView(objac);
            }
        }




        /// <summary>
        /// Save Bank Account Details
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Save the New Bank Account Details to DB</returns>
        [HttpPost]
        public async Task<ActionResult> AddNewBankAccountAsyncAN(Accountant obj)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                obj.StaffCode = Session["StaffCode"].ToString();
                await objbal.AddNewBankAccountAsyncAN(obj);
                return RedirectToAction("BankAccountListAsyncAN");
            }
        }

        /// <summary>
        /// Show  the Bank Details of bank Account 
        /// </summary>
        /// <param name="obj1"></param>
        /// <returns>Account Details</returns>
        [HttpGet]
        public async Task<ActionResult> BankAccountTransactionDetailsAsyncAN(int BankId)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Accountant objDetails = new Accountant();
                objDetails.BankId = BankId;
                objDetails.BranchCode = Session["BranchCode"].ToString();
                SqlDataReader dr = await objbal.BankAccountTransactionDetailsAsync(objDetails);

                if (dr.Read())
                {
                    objDetails.BankId = Convert.ToInt32(dr["BankId"].ToString());
                    objDetails.BankName = dr["BankName"].ToString();
                    objDetails.BankAccountNumber = Convert.ToInt64(dr["AccountNumber"].ToString());
                    objDetails.AccountHolderName = dr["AccountHolderName"].ToString();
                    objDetails.AccountType = dr["AccountType"].ToString();
                    objDetails.BankBrach = dr["Branch"].ToString();
                    objDetails.BankAccountOpeningDate = Convert.ToDateTime(dr["AccountOpeningDate"].ToString());
                    objDetails.Date = Convert.ToDateTime(dr["Date"].ToString());
                    objDetails.IFSCCode = dr["IFSCCode"].ToString();
                    objDetails.MICRCode = dr["MICRCode"].ToString();
                    objDetails.BankAmount = float.Parse(dr["Balance"].ToString());
                    objDetails.BankAccountOpeningBalance = float.Parse(dr["FinancialYearOpeningBalance"].ToString());
                    objDetails.BankAccountMinimumBalance = float.Parse(dr["MinimumBalance"].ToString());
                }
                dr.Close();
                return PartialView("BankAccountTransactionDetailsAsyncAN", objDetails);
            }
        }



        /// <summary>
        /// Delete Bank Account if if they don't have any Transactions Yet
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Delete Bank Account</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteBankAccountAN(int id)
        {
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                try
                {

                    objac.BankId = id;
                    await objbal.DeleteBankAccountAN(objac);
                    return await Task.Run(() => Json(new { success = true, message = "Bank account deleted successfully" }, JsonRequestBehavior.AllowGet));
                }
                catch (Exception ex)
                {
                    throw (ex);
                }

            }
        }



        //------------------Ajay End ----------------------------------------------------------//
        //-----------------Shrikant StaffPayRoll Start -----------------------------------------------------------------------//


        /// <summary>
        /// This method is Used for just checking user is loged or not 
        /// if loged than show main view of staff payRoll
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetStaffForPaySSAsync()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                await DeparmentBindSSAsync();
                await PositionBindSSAsync();
                await LoadAndStoreFullStaffList();
                StorePayingStatus();

                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
 {
     new BreadcrumbItem { Name = "Accountant Dashboard", Url = Url.Action("AccountantDashboardAsyncSGS", "Accountant") },
     new BreadcrumbItem { Name = "Staff Pay Roll", Url = Url.Action("GetStaffForPaySSAsync", "Accountant") }
 };

                ViewBag.Breadcrumbs = breadcrumbs;

                return View();
            }
        }

        /// <summary>
        /// this method create to Load and Store Full Staff List in Session
        /// </summary>
        /// <returns>it not returns any thing</returns>
        [HttpGet]
        private async Task LoadAndStoreFullStaffList()
        {

            if (Session[StaffListSessionKey] == null)
            {
                string BranchCode = Session["BranchCode"].ToString();
                DataSet ds = await objbal.StaffListSSAsync(BranchCode);
                List<Accountant> fullStaffList = ConvertDataSetToAccountantList(ds);
                Session[StaffListSessionKey] = fullStaffList;
            }
        }

        /// <summary>
        /// this Method is used Store data into model property 
        /// </summary>
        /// <param name="ds"></param>
        /// <returns>List of Accountant</returns>
        private List<Accountant> ConvertDataSetToAccountantList(DataSet ds)
        {
            List<Accountant> staffList = new List<Accountant>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Accountant objP = new Accountant
                    {
                        StaffCode = Convert.ToString(dr["StaffCode"]),
                        DepartmentID = dr["DepartmentId"] != DBNull.Value
                            ? Convert.ToInt32(dr["DepartmentId"])
                            : 0,
                        StaffName = Convert.ToString(dr["Staff Name"]),
                        DepartmentName = Convert.ToString(dr["Department"]),
                        Designation = Convert.ToString(dr["Designation"]),
                        BankName = Convert.ToString(dr["Bank"]),
                        BranchName = Convert.ToString(dr["Branch Name"]),
                        AccountNo = dr["Account Number"] != DBNull.Value
                            ? Convert.ToInt64(dr["Account Number"].ToString())
                            : 0,
                        IFSCCode = Convert.ToString(dr["IFSCCode"]),
                        GrossSalary = dr["Monthly Gross Salary"] != DBNull.Value
                            ? Convert.ToDecimal(dr["Monthly Gross Salary"])
                            : 0
                    };
                    staffList.Add(objP);
                }
            }
            return staffList;
        }
        /// <summary>
        /// This method is Partial View of StaffDetailes Page
        /// we call this method when page is load
        /// </summary>
        /// <returns> partial View</returns>
        public async Task<ActionResult> ListOfStaffSSAsync()
        {
            List<Accountant> fullStaffList = Session[StaffListSessionKey] as List<Accountant>;
            Accountant obj = new Accountant { lstEmp = fullStaffList };
            List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
 {
     new BreadcrumbItem { Name = "Accountant Dashboard", Url = Url.Action("AccountantDashboardAsyncSGS", "Accountant") },
     new BreadcrumbItem { Name = "Staff PayRoll", Url = Url.Action("GetStaffForPaySSAsync", "Accountant") },
    // new BreadcrumbItem { Name = "List OF Staff", Url = Url.Action("_ListOfStaffSSAsync", "Accountant") }
 };

            ViewBag.Breadcrumbs = breadcrumbs;
            return await Task.Run(()=> PartialView("_ListOfStaffSSAsync", obj));

        }

        /// <summary>
        /// This is nonAction method used for just get List of Departments
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        private async Task DeparmentBindSSAsync()
        {
            DataSet ds = await objbal.DeparmentBindSSAsync();
            List<SelectListItem> DepList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DepList.Add(new SelectListItem
                {
                    Text = dr["DepartmentName"].ToString(),
                    Value = dr["DepartmentId"].ToString()
                });
            }
            ViewBag.Department = new SelectList(DepList, "Value", "Text");
        }
        public void StorePayingStatus()
        {
            var statusList = new List<SelectListItem>
    {
        new SelectListItem { Text = "Not Processed", Value = "Not Processed" },
        new SelectListItem { Text = "Approved", Value = "Approved" },
        new SelectListItem { Text = "Created", Value = "Created" },
        new SelectListItem { Text = "Credit", Value = "Credit" }
    };

            ViewBag.ListSalaryStatus = new SelectList(statusList, "Value", "Text");
        }


        /// <summary>
        /// This non Action Method used to Fech List of Positions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task PositionBindSSAsync()
        {
            DataSet ds = await objbal.PositionBindSSAsync();
            List<SelectListItem> DepList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DepList.Add(new SelectListItem
                {
                    Text = dr["DesignationName"].ToString(),
                    Value = dr["DesignationId"].ToString()
                });
            }
            ViewBag.Positon = new SelectList(DepList, "Value", "Text");


        }


        /// <summary>
        /// Display employees filtered by DepartmentID
        /// </summary>
        /// <param name="DepartmentID"></param>
        /// <returns>PartialView</returns>
        public ActionResult WithIdListOfStaffSSAsync(string DepartmentText, string PositionName)
        {

            DepartmentText = DepartmentText == "Select Department" ? null : DepartmentText;
            PositionName = PositionName == "Select Position" ? null : PositionName;


            List<Accountant> fullStaffList = Session[StaffListSessionKey] as List<Accountant>;
            if (fullStaffList == null)
            {

                return PartialView("_ListOfStaffSSAsync", new Accountant { lstEmp = new List<Accountant>() });
            }


            List<Accountant> filteredStaff = fullStaffList;

            if (!string.IsNullOrEmpty(DepartmentText))
            {
                filteredStaff = filteredStaff.Where(s => s.DepartmentName == DepartmentText).ToList();
            }
            if (!string.IsNullOrEmpty(PositionName))
            {
                filteredStaff = filteredStaff.Where(s => s.Designation == PositionName).ToList();
            }


            Accountant obj = new Accountant { lstEmp = filteredStaff };
            return PartialView("_ListOfStaffSSAsync", obj);
        }

        /// <summary>
        /// this method is used to save staff code in GSTtblSalaryCredit which Salary Credit
        /// </summary>
        /// <param name="selectedStaffCodes"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ConfirmationStaffPayasyncSS(List<string> selectedStaffCodes, string selectedStaffWithAdvance, int ExpenseId, decimal FinalAmount)
        {
            try
            {
                if ((selectedStaffCodes == null || !selectedStaffCodes.Any()) && string.IsNullOrEmpty(selectedStaffWithAdvance))
                {
                    return Json(new { success = false, error = "No staff selected for payment." });
                }

                int Month = Convert.ToInt32(Session["month"]);
                int Year = Convert.ToInt32(Session["year"].ToString());
                string loggedStaffCode = Session["StaffCode"]?.ToString();

                if (string.IsNullOrEmpty(loggedStaffCode))
                {
                    return Json(new { success = false, error = "Staff code not found in session." });
                }

                string monthName = new DateTime(Year, Month, 1).ToString("MMMM").ToUpper();
                string combinedMonthYear = $"{monthName}{Year}";

                // Process regular salary payments
                if (selectedStaffCodes != null && selectedStaffCodes.Any())
                {
                    objac.ListofStafftoPay = string.Join(",", selectedStaffCodes);
                    objac.combinedMonthYear = combinedMonthYear;
                    objac.ExpenseId = ExpenseId;
                    objac.FinalAmount = Math.Round(FinalAmount, 2);
                    objac.StaffCode = loggedStaffCode;

                    await objbal.CreditSalarySaveAsyncSS(objac);
                }

                if (!string.IsNullOrEmpty(selectedStaffWithAdvance))
                {
                    var staffWithAdvance = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(selectedStaffWithAdvance);
                    foreach (var kvp in staffWithAdvance)
                    {
                        string staffCode = kvp.Key;
                        decimal advanceAmount = kvp.Value.advanceAmount;

                        objac.ListofStafftoPay = staffCode;
                        objac.combinedMonthYear = combinedMonthYear;
                        objac.ExpenseId = 3;
                        objac.FinalAmount = advanceAmount;
                        objac.StaffCode = loggedStaffCode;

                        await objbal.CreditSalarySaveAsyncSS(objac);
                    }
                }

                await AttendenceOfStaffssAsync(Session["month"].ToString(), Session["year"].ToString());

                return Json(new { success = true });
            } catch (Exception ex)
            {
                // Log the exception
                Console.Error.WriteLine($"Error in ConfirmationStaffPayasyncSS: {ex}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// This Post Method is used to show view for CreditAdvance
        /// </summary>
        /// <param name="StaffName"></param>
        /// <param name="staffCode"></param>
        /// <param name="advanceAmount"></param>
        /// <param name="finalAmount"></param>
        /// <param name="Netsalary"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreditAdvanceSalaryAsyncSS(string StaffName, string staffCode, decimal advanceAmount, decimal finalAmount, decimal Netsalary, string month, string year)
        {
            objac.StaffName = StaffName;
            objac.StaffCode = staffCode;
            objac.AdvanceAmount = advanceAmount;
            objac.FinalAmount = finalAmount;
            objac.NetSalary = Netsalary;
            string monthName = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1).ToString("MMMM").ToUpper();
            objac.combinedMonthYear = $"{monthName}{year}";
            return PartialView("_CreditAdvanceSalaryAsyncSS", objac);
        }

        /// <summary>
        /// this is Non Action Method Used to Load And Store Attendance of Staff
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private async Task LoadAndStoreFullAttendanceSSasync(string month, string year)
        {
            string branchCode = Session["BranchCode"].ToString();
            DataSet ds = await objbal.LoadAndStoreFullAttendanceSSasync(branchCode, month, year);
            List<Accountant> fullStaffAttendanceList = ConvertDataSetToAccountantListForAttendance(ds);

            decimal totalAdjustedNetSalary = fullStaffAttendanceList.Sum(staff => staff.AdjustedNetSalary);

            Session[StaffAttendanceListSessionKey] = fullStaffAttendanceList;

            ViewBag.TotalAdjustedNetSalary = totalAdjustedNetSalary;
        }

        /// <summary>
        /// Non-Action Method to convert DataSet to Accountant List for Attendance
        /// </summary>
        private List<Accountant> ConvertDataSetToAccountantListForAttendance(DataSet ds)
        {
            List<Accountant> staffList = new List<Accountant>();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    decimal payableDays = Math.Round(Convert.ToDecimal(dr["PayableDays"]), 1);
                    string formattedPayableDays = payableDays.ToString("0.0");

                    Accountant objP = new Accountant
                    {
                        StaffCode = dr["StaffCode"].ToString(),
                        StaffName = dr["StaffName"].ToString(),
                        DepartmentName = dr["DepartmentName"].ToString(),
                        Designation = dr["Designation"].ToString(),
                        PaidDays = Convert.ToInt32(dr["TotalMonthDays"]),
                        PaidLeaveCount = Convert.ToInt32(dr["PaidLeaveCount"]),
                        PresentDays = Convert.ToInt32(dr["WorkedDays"]),
                        AbsentDays = Convert.ToInt32(dr["AbsentDays"]),
                        PayableDaysS = formattedPayableDays,
                        MonthlyBasicSalary = Convert.ToDecimal(dr["MonthlyNetSalary"]),
                        AdjustedNetSalary = Convert.ToDecimal(dr["PayableSalary"]),
                        PayingStatus = dr["PayingStatus"].ToString(),
                        FinalAmount = Convert.ToDecimal(dr["FinalAmount"]),
                        AdvanceAmountCut = Convert.ToDecimal(dr["AdvanceAmount"])
                    };
                    staffList.Add(objP);
                }
            }
            return staffList;
        }
    

        /// <summary>
        /// This method is called to show the attendance of staff for a specific month and year
        /// </summary>
        public async Task<ActionResult> AttendenceOfStaffssAsync(string month, string year)
        {
            await LoadAndStoreFullAttendanceSSasync(month, year);

            List<Accountant> fullStaffAttendanceList = Session[StaffAttendanceListSessionKey] as List<Accountant>;
            Accountant obj = new Accountant { lstEmpAttendance = fullStaffAttendanceList,PayingStatus= "Not Processed" };
            var statusList = new List<SelectListItem>
    {
        new SelectListItem { Text = "Not Processed", Value = "Not Processed" },
        new SelectListItem { Text = "Approved", Value = "Approved" },
        new SelectListItem { Text = "Created", Value = "Created" },
        new SelectListItem { Text = "Credit", Value = "Credit" }
    };

            ViewBag.ListSalaryStatus = statusList;
            Session["month"] = month;
            Session["year"] = year;
            ViewBag.TotalAdjustedNetSalary = ViewBag.TotalAdjustedNetSalary;

            return PartialView("_AttendenceOfStaffssAsync", obj);
        }


        /// <summary>
        /// This method is used to Filter data From Session StaffAttendanceListSessionKey
        /// </summary>
        /// <param name="departmentText"></param>
        /// <param name="positionName"></param>
        /// <returns></returns>
        public ActionResult WithIdListOfStaffAttendanceSSAsync(string departmentText, string positionName)
        {


            departmentText = departmentText == "Select Department" ? null : departmentText;
            positionName = positionName == "Select Position" ? null : positionName;

            List<Accountant> fullStaffList = Session[StaffAttendanceListSessionKey] as List<Accountant>;
            if (fullStaffList == null)
            {
                ViewBag.TotalAdjustedNetSalary = 0;
                return PartialView("_AttendenceOfStaffssAsync", new Accountant { lstEmpAttendance = new List<Accountant>() });
            }

            List<Accountant> filteredStaff = fullStaffList;

            if (!string.IsNullOrEmpty(departmentText))
            {
                filteredStaff = filteredStaff.Where(s => s.DepartmentName == departmentText).ToList();
            }
            if (!string.IsNullOrEmpty(positionName))
            {
                filteredStaff = filteredStaff.Where(s => s.Designation == positionName).ToList();
            }

            // Recalculate the total AdjustedNetSalary for the filtered list
            decimal totalAdjustedNetSalary = filteredStaff.Sum(staff => staff.AdjustedNetSalary);
            ViewBag.TotalAdjustedNetSalary = totalAdjustedNetSalary;

            Accountant obj = new Accountant { lstEmpAttendance = filteredStaff };
            return PartialView("_AttendenceOfStaffssAsync", obj);
        }


        /// <summary>
        /// This Method used to Show Staff Details Of Staff
        /// </summary>
        /// <param name="StaffCode"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DetailsOfStaffAsyncSS(string StaffCode)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
 {
     new BreadcrumbItem { Name = "Accountant Dashboard", Url = Url.Action("AccountantDashboardAsyncSGS", "Accountant") },
     new BreadcrumbItem { Name = "Staff List", Url = Url.Action("GetStaffForPaySSAsync", "Accountant") },
   //  new BreadcrumbItem { Name = "List OF Staff", Url = Url.Action("_ListOfStaffSSAsync", "Accountant") },
     new BreadcrumbItem { Name = "My Salary", Url = Url.Action("DetailsOfStaffAsyncSS", "Accountant") }
 };
                ViewBag.Breadcrumbs = breadcrumbs;
                return View("mm");
            }

        }

        /// <summary>
        /// This Method used to Show Salary History of specific Staff
        /// </summary>
        /// <param name="StaffCode"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<ActionResult> ShowAllSalaryAsyncSS(string StaffCode = null)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                string branchCode = Session["BranchCode"].ToString();

                DataSet ds = await objbal.ShowAllSalaryAsyncSS(StaffCode, branchCode);
                List<Accountant> salarySlips = new List<Accountant>();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Accountant obja = new Accountant
                        {
                            StaffCode = dr["StaffCode"].ToString(),
                            StaffName = dr["StaffName"].ToString(),
                            StaffPosition = dr["Designation"].ToString(),
                            GrossSalary = Convert.ToDecimal(dr["MonthlyGrossSalary"]),
                            AdvanceAmount = Convert.ToDecimal(dr["Amount"]),
                            NetSalary = Convert.ToDecimal(dr["PayableSalary"]),
                            ClientName = (dr["SalaryCreditDate"]).ToString(),
                            combinedMonthYear = (dr["MonthYear"]).ToString(),
                            Status = (dr["Status"]).ToString()


                        };
                        salarySlips.Add(obja);
                    }
                } else
                {
                    ViewBag.Message = "No salary slips found for the given criteria.";
                }

                ViewBag.SalarySlips = salarySlips;
            }

            List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
 {
     new BreadcrumbItem { Name = "Accountant Dashboard", Url = Url.Action("AccountantDashboardAsyncSGS", "Accountant") },
     new BreadcrumbItem { Name = "Staff Pay Roll", Url = Url.Action("GetStaffForPaySSAsync", "Accountant") },
   //  new BreadcrumbItem { Name = "List OF Staff", Url = Url.Action("GetStaffForPaySSAsync", "Accountant") },
     new BreadcrumbItem { Name = "Salary Details", Url = Url.Action("ShowAllSalaryAsyncSS", "Accountant", new { StaffCode = StaffCode }) }
 };
            ViewBag.Breadcrumbs = breadcrumbs;



            return View();


        }


        /// <summary>
        /// Load And Store All Staff Attendance
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet]

        public async Task LoadAndStoreAllFullAttendanceSSasync(string month, string year)
        {
            string branchCode = Session["BranchCode"].ToString();
            Session["QueryMonth"] = month;
            Session["QueryYear"] = year;
            DataSet ds = await objbal.ShowAttendanceOfAllStaffAsyncSS(branchCode, month, year);
            List<Accountant> AllFullStaffAttendanceList = ConvertDataSetToAccountantAttendance(ds);
            Session[AllStaffAttendanceListSessionKey] = AllFullStaffAttendanceList;
        }

        /// <summary>
        /// This Method Used to Ds data into List
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public List<Accountant> ConvertDataSetToAccountantAttendance(DataSet ds)
        {
            List<Accountant> staffList = new List<Accountant>();

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DateTime date;
                    bool isValidDate = DateTime.TryParse(dr["Date"].ToString(), out date);
                    Accountant objP = new Accountant
                    {
                        StaffCode = dr["StaffCode"].ToString(),
                        StaffName = dr["StaffName"].ToString(),
                        Status = dr["Status"].ToString(),
                        DateS = isValidDate ? date.ToString("dd/MM/yyyy") : string.Empty,
                    };
                    staffList.Add(objP);
                }

                // Store the query month and year in session variables
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Session["QueryMonth"] = ds.Tables[0].Rows[0]["QueryMonth"].ToString();
                    Session["QueryYear"] = ds.Tables[0].Rows[0]["QueryYear"].ToString();
                }
            }
            return staffList;
        }

        /// <summary>
        /// This Method used to call _AttendenceOfAllStaffssAsync this view
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<ActionResult> AttendenceOfAllStaffssAsync(string month, string year)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                await LoadAndStoreAllFullAttendanceSSasync(month, year);

                List<Accountant> AllFullStaffAttendanceList = Session[AllStaffAttendanceListSessionKey] as List<Accountant>;
                Accountant obj = new Accountant { lstAllEmpAttendance = AllFullStaffAttendanceList };
                return PartialView("_AttendenceOfAllStaffssAsync", obj);
            }
        }

        /// <summary>
        /// This Method used to get specific staff attendance
        /// </summary>
        /// <param name="staffCode"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetSpecificStaffAttendanceAsyncSS(string staffCode)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                List<Accountant> AllFullStaffAttendanceList = Session[AllStaffAttendanceListSessionKey] as List<Accountant>;


                // Filter the list for the specific staff code
                List<Accountant> SpecificStaffAttendanceList = AllFullStaffAttendanceList
                    .Where(a => a.StaffCode == staffCode)
                    .ToList();

                Accountant obj = new Accountant { lstAllEmpAttendance = SpecificStaffAttendanceList };
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
 {
     new BreadcrumbItem { Name = "Accountant Dashboard", Url = Url.Action("AccountantDashboardAsyncSGS", "Accountant") },
     new BreadcrumbItem { Name = "Staff Pay Roll", Url = Url.Action("GetStaffForPaySSAsync", "Accountant") },
     new BreadcrumbItem { Name = "Attendence", Url = Url.Action("AttendenceOfAllStaffssAsync", "Accountant") }
 };

                ViewBag.Breadcrumbs = breadcrumbs;
                return await Task.Run(() => View(obj));


            }
        }




        /// <summary>
        /// Advance Payment View 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> AdvanceStaffPaySSAsync()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                await LoadAndStoreAdvanceSSasync();

                List<Accountant> AdvancePayStaffList = Session[AdvancePayStaffListSessionKey] as List<Accountant>;
                Accountant obj = new Accountant { lstAllEmpAdvancePay = AdvancePayStaffList };
                return PartialView("_AdvanceStaffPaySSAsync", obj);
            }
        }

        /// <summary>
        /// This Method Used to call method From bal class
        /// </summary>
        /// <returns></returns>
        public async Task LoadAndStoreAdvanceSSasync()
        {
            string branchCode = Session["BranchCode"].ToString();
            DataSet ds = await objbal.LoadAndStoreAdvanceSSasync(branchCode);
            List<Accountant> AdvancePayStaffList = ConvertDataSetToAccountantAdvance(ds);
            Session[AdvancePayStaffListSessionKey] = AdvancePayStaffList;
        }

        /// <summary>
        /// Bind data with Model
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public List<Accountant> ConvertDataSetToAccountantAdvance(DataSet ds)
        {
            List<Accountant> staffList = new List<Accountant>();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {


                    Accountant objP = new Accountant
                    {
                        StaffCode = dr["StaffCode"].ToString(),
                        StaffName = dr["StaffName"].ToString(),
                        //Status = dr["Status"].ToString(),
                        DepartmentName = dr["DepartmentName"].ToString(),
                        StaffPosition = dr["StaffPosition"].ToString(),
                        AdvanceAmount = Convert.ToDecimal(dr["AdvanceAmount"]),
                        MonthlyBasicSalary = Convert.ToDecimal(dr["MonthlyBasicSalary"]),
                        // Date = isValidDate ? date.ToString("dd/MM/yyyy") : string.Empty,
                    };
                    staffList.Add(objP);
                }
            }
            return staffList;
        }
        /// <summary>
        /// This Method will show all Salary Voucher
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> SalaryVoucherTabSSAsync()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                await SalaryVoucherSSasync();

                List<Accountant> AdvancePayStaffList = Session[AdvancePayStaffListSessionKey] as List<Accountant>;
                Accountant obj = new Accountant { lstAllEmpAdvancePay = AdvancePayStaffList };
                return PartialView("_SalaryVoucherTabSSAsync", obj);
            }
        }

        /// <summary>
        /// This Method Used to call method From bal class
        /// </summary>
        /// <returns></returns>
        public async Task SalaryVoucherSSasync()
        {
            string branchCode = Session["BranchCode"].ToString();
            DataSet ds = await objbal.SalaryVoucherSSasync(branchCode);
            List<Accountant> AdvancePayStaffList = ConvertDataSetToSalaryVoucher(ds);
            Session[AdvancePayStaffListSessionKey] = AdvancePayStaffList;
        }

        /// <summary>
        /// This method is used to get into model
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public List<Accountant> ConvertDataSetToSalaryVoucher(DataSet ds)
        {
            List<Accountant> staffList = new List<Accountant>();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {


                    Accountant objP = new Accountant
                    {
                        SalaryMonthId = Convert.ToInt32(dr["SalaryMonthId"]),
                        combinedMonthYear = dr["MonthYear"].ToString(),
                        Status = dr["Status"].ToString(),
                        AdvanceAmount = Convert.ToDecimal(dr["Amount"]),

                    };
                    staffList.Add(objP);
                }
            }
            return staffList;
        }
        //-----------------Shrikant StaffPayRoll END -----------------------------------------------------------------------//



        //[HttpGet]
        //public async Task<ActionResult> AddVoucherAsyncSGS(string salaryMonthIds, int voucherType = 0, decimal amount = 0)
        //{
        //    if (Session["StaffCode"] == null)
        //    {
        //        return await Task.Run(() => RedirectToAction("Login", "Account"));
        //    }
        //    else
        //    {
        //        Session["salaryMonthIds"] = salaryMonthIds;
        //        objac.salaryMonthIds = salaryMonthIds;
        //        objac.StaffCode = Session["StaffCode"].ToString(); // Retrieve staff code from session
        //        objac.BranchCode = Session["BranchCode"].ToString(); // Retrieve branch code from session

        //        // Process the Voucher Type and amount here if needed
        //        // You can store these in the objac object or use them directly

        //        // Voucher Type List
        //        List<SelectListItem> VoucherTypeList = new List<SelectListItem>();
        //        DataSet ds = await objbal.VoucherTypeAsyncSGS(objac);
        //        List<SelectListItem> TypeList = new List<SelectListItem>();
        //        foreach (DataRow dr in ds.Tables[0].Rows)
        //        {
        //            TypeList.Add(new SelectListItem
        //            {
        //                Text = dr["VoucherType"].ToString(),
        //                Value = dr["VoucherTypeId"].ToString()
        //            });
        //        }
        //        VoucherTypeList.AddRange(TypeList);
        //        ViewBag.VoucherTypeList = VoucherTypeList;


        //        // Voucher Code
        //        objac.VoucherCode = await objbal.GetMaxVoucherCodeAsyncSGS(objac);
        //        objac.VoucherDate = DateTime.Now;
        //        ViewBag.VoucherNumber = objac.VoucherCode; // This line might be redundant now

        //        // Staff List
        //        List<SelectListItem> combinedReportingList = new List<SelectListItem>();
        //        DataSet ds1 = await objbal.StaffNameforVoucherAsyncSGS(objac);
        //        List<SelectListItem> StaffList = new List<SelectListItem>();
        //        foreach (DataRow dr in ds1.Tables[0].Rows)
        //        {
        //            StaffList.Add(new SelectListItem
        //            {
        //                Text = dr["StaffName"].ToString(),
        //                Value = dr["StaffCode"].ToString()
        //            });
        //        }
        //        combinedReportingList.AddRange(StaffList);
        //        ViewBag.combinedReportingList = combinedReportingList;
        //        // Staff List
        //        List<SelectListItem> VendorList = new List<SelectListItem>();
        //        DataSet vds = await objbal.VendorNameforVoucherAsyncSGS(objac);
        //        List<SelectListItem> VendorNameList = new List<SelectListItem>();
        //        foreach (DataRow dr in vds.Tables[0].Rows)
        //        {
        //            VendorNameList.Add(new SelectListItem
        //            {
        //                Text = dr["VendorName"].ToString(),
        //                Value = dr["VendorCode"].ToString()
        //            });
        //        }
        //        VendorList.AddRange(VendorNameList);
        //        ViewBag.VendorList = VendorList;

        //        // Bank Account List
        //        List<SelectListItem> BankAccountList = new List<SelectListItem>();
        //        DataSet ds2 = await objbal.BankAccountforVoucherAsyncSGS(objac);
        //        List<SelectListItem> BankList = new List<SelectListItem>();
        //        foreach (DataRow dr in ds2.Tables[0].Rows)
        //        {
        //            string bankName = dr["BankName"].ToString();
        //            string accountHolderName = dr["AccountHolderName"].ToString();
        //            string accountNumber = dr["AccountNumber"].ToString();

        //            // Masking the account number
        //            string maskedAccountNumber = "****" + accountNumber.Substring(accountNumber.Length - 4);

        //            BankList.Add(new SelectListItem
        //            {
        //                Text = $"{bankName} - {accountHolderName} - {maskedAccountNumber}",
        //                Value = dr["BankId"].ToString()
        //            });
        //        }
        //        BankAccountList.AddRange(BankList);
        //        ViewBag.BankAccountList = BankAccountList;


        //        // Breadcrumbs
        //        List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
        //        {
        //            new BreadcrumbItem { Name = "AccountantDashboard", Url = "AccountantDashboardAsyncSGS" },
        //            new BreadcrumbItem { Name = "Voucher Managment", Url = "ListAllVouchersAsyncSGS" },
        //            new BreadcrumbItem { Name = "Add Voucher", Url = "AddCashVoucherAsyncSGS" },
        //        };

        //        ViewBag.Breadcrumbs = breadcrumbs;

        //        // You can also pass the voucherType and amount to the view if needed
        //        ViewBag.SelectedVoucherType = voucherType;
        //        ViewBag.Amount = amount;

        //        return PartialView(objac);
        //    }
        //}
        //[HttpPost]
        //public async Task<ActionResult> AddVoucherAsyncSGS(Accountant objA)
        //{
        //    if (Session["StaffCode"] == null)
        //    {
        //        return await Task.Run(() => RedirectToAction("Login", "Account"));
        //    }

        //    try
        //    {
        //        // Set basic information
        //        objA.StaffCode = Session["StaffCode"].ToString();
        //        objA.BranchCode = Session["BranchCode"].ToString();

        //        // First add the voucher
        //        await objbal.AddVoucherAsyncSGS(objA);

        //        // Get and process salary month IDs
        //        string salaryMonthIds = Session["salaryMonthIds"].ToString();
        //        var monthIdArray = salaryMonthIds.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

        //        // Process each salary month ID
        //        foreach (var monthId in monthIdArray)
        //        {
        //            try
        //            {
        //                // Create a new Accountant object for each iteration to ensure clean data
        //                var monthAccountant = new Accountant
        //                {
        //                    StaffCode = objA.StaffCode,
        //                    BranchCode = objA.BranchCode,
        //                    // Assuming you have a property for SalaryMonthId in your Accountant class
        //                    combinedMonthYear = (monthId.Trim())
        //                    // Copy other necessary properties from objA if needed
        //                };

        //                await objbal.ChangeStatusOfsalaryMonthIdsAsyncSS(monthAccountant);
        //            }
        //            catch (Exception monthEx)
        //            {
        //                // Log the error for this specific month ID but continue processing others
        //                // You might want to add proper logging here
        //                System.Diagnostics.Debug.WriteLine($"Error processing month ID {monthId}: {monthEx.Message}");
        //            }
        //        }

        //        return RedirectToAction("ListAllVouchersAsyncSGS");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "An error occurred while saving data: " + ex.Message });
        //    }
        //}






        #region 
        //Shreyas's Region  About Dashboard and Voucher Submodules Starts Here
        /// <summary>
        /// Accountant Dashboard View
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<ActionResult> AccountantDashboardAsyncSGS(int month = 0)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string staffCode = Session["StaffCode"].ToString();
            objac.StaffCode = staffCode;
            Accountant dashboardData = new Accountant();

            // Determine the date range dynamically
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            // Fetch income and expense data
            DataSet TIncExp = await objbal.GetTIncExpDashData(startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            if (TIncExp.Tables.Count > 0 && TIncExp.Tables[0].Rows.Count > 0)
            {
                DataRow dr = TIncExp.Tables[0].Rows[0];
                dashboardData.TotalIncome = dr["AverageMonthlyCredit"] != DBNull.Value ? Convert.ToDecimal(dr["AverageMonthlyCredit"]) : 0.0m;
                dashboardData.TotalExpenses = dr["AverageMonthlyDebit"] != DBNull.Value ? Convert.ToDecimal(dr["AverageMonthlyDebit"]) : 0.0m;
            }

            DataSet DSCashInHand = await objbal.GetCashInHandDashData(DateTime.Now.ToString("yyyy-MM-dd"));

            if (DSCashInHand.Tables.Count > 0 && DSCashInHand.Tables[0].Rows.Count > 0)
            {
                DataRow dr = DSCashInHand.Tables[0].Rows[0];
                dashboardData.CashInHand = dr["CashInHand"] != DBNull.Value ? Convert.ToDecimal(dr["CashInHand"]) : 0.0m;
            }

            // Fetch bank account list
            DataSet ds2 = await objbal.BankAccountforVoucherAsyncSGS(objac);
            ViewBag.BankAccountList = ds2.Tables[0].AsEnumerable().Select(dr => new SelectListItem
            {
                Text = $"{dr["BankName"]} - {dr["AccountHolderName"]} - {dr["AccountNumber"]}",
                Value = dr["BankId"].ToString()
            }).ToList();

            List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
            {
                //new BreadcrumbItem { Name = "Home", Url = "/" },
                new BreadcrumbItem { Name = "Accountant Dashboard" },
            };

            ViewBag.Breadcrumbs = breadcrumbs;

            return View(dashboardData);
        }
        public async Task<ActionResult> AccountantDashboardAsyncSGS1(int month = 0)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string staffCode = Session["StaffCode"].ToString();
            objac.StaffCode = staffCode;
            Accountant dashboardData = new Accountant();

            // Determine the date range dynamically
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            // Fetch income and expense data
            DataSet TIncExp = await objbal.GetTIncExpDashData(startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            if (TIncExp.Tables.Count > 0 && TIncExp.Tables[0].Rows.Count > 0)
            {
                DataRow dr = TIncExp.Tables[0].Rows[0];
                dashboardData.TotalIncome = dr["AverageMonthlyCredit"] != DBNull.Value ? Convert.ToDecimal(dr["AverageMonthlyCredit"]) : 0.0m;
                dashboardData.TotalExpenses = dr["AverageMonthlyDebit"] != DBNull.Value ? Convert.ToDecimal(dr["AverageMonthlyDebit"]) : 0.0m;
            }

            DataSet DSCashInHand = await objbal.GetCashInHandDashData(DateTime.Now.ToString("yyyy-MM-dd"));

            if (DSCashInHand.Tables.Count > 0 && DSCashInHand.Tables[0].Rows.Count > 0)
            {
                DataRow dr = DSCashInHand.Tables[0].Rows[0];
                dashboardData.CashInHand = dr["CashInHand"] != DBNull.Value ? Convert.ToDecimal(dr["CashInHand"]) : 0.0m;
            }

            // Fetch bank account list
            DataSet ds2 = await objbal.BankAccountforVoucherAsyncSGS(objac);
            ViewBag.BankAccountList = ds2.Tables[0].AsEnumerable().Select(dr => new SelectListItem
            {
                Text = $"{dr["BankName"]} - {dr["AccountHolderName"]} - {dr["AccountNumber"]}",
                Value = dr["BankId"].ToString()
            }).ToList();

            List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
            {
                //new BreadcrumbItem { Name = "Home", Url = "/" },
                new BreadcrumbItem { Name = "Accountant Dashboard" },
            };

            ViewBag.Breadcrumbs = breadcrumbs;

            return View(dashboardData);
        }

        /// <summary>
        /// Json Method to fetch Income Expense data from database
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> TotalIncExpAsyncSGS(DateTime startDate, DateTime endDate)
        {
            DataSet TIncExp = await objbal.GetTIncExpDashData(startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            if (TIncExp.Tables.Count > 0 && TIncExp.Tables[0].Rows.Count > 0)
            {
                DataRow dr = TIncExp.Tables[0].Rows[0];
                objac.TotalIncome = dr["AverageMonthlyCredit"] != DBNull.Value ? Convert.ToDecimal(dr["AverageMonthlyCredit"]) : 0.0m;
                objac.TotalExpenses = dr["AverageMonthlyDebit"] != DBNull.Value ? Convert.ToDecimal(dr["AverageMonthlyDebit"]) : 0.0m;
            }

            return Json(new { success = true, objac }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Json Method to fetch Cash in Hand data from database
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetCashInHandAsyncSGS(DateTime date)
        {
            DataSet TCashInHand = await objbal.GetCashInHandDashData(date.ToString("yyyy-MM-dd"));

            if (TCashInHand.Tables.Count > 0 && TCashInHand.Tables[0].Rows.Count > 0)
            {
                DataRow dr = TCashInHand.Tables[0].Rows[0];
                objac.CashInHand = dr["CashInHand"] != DBNull.Value ? Convert.ToDecimal(dr["CashInHand"]) : 0.0m;
            }

            return Json(new { success = true, objac }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Json Method to fetch Income Expense Graph data from database
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> IncvsExpGraphAsyncSGS(DateTime startDate, DateTime endDate)
        {
            // Call the business logic layer method to get the data
            DataSet TIncvsExpGraph = await objbal.GetIncvsExpGraphDashData(startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            // Initialize a list to store month-wise data
            var resultData = new List<dynamic>();

            // Check if the dataset has data
            if (TIncvsExpGraph.Tables.Count > 0 && TIncvsExpGraph.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in TIncvsExpGraph.Tables[0].Rows)
                {
                    // Extract values from the DataRow
                    string month = dr["Month"].ToString();  // Assumes the column name in the dataset is "Month"
                    decimal income = dr["Income"] != DBNull.Value ? Convert.ToDecimal(dr["Income"]) : 0.0m;
                    decimal expense = dr["Expense"] != DBNull.Value ? Convert.ToDecimal(dr["Expense"]) : 0.0m;

                    // Add data to the result list
                    resultData.Add(new
                    {
                        Month = month,
                        Income = income,
                        Expense = expense
                    });
                }
            }

            return Json(resultData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Json Method to fetch Expense chart data from database
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> ExpensesChartAsyncSGS(DateTime startDate, DateTime endDate)
        {
            // Fetch the dataset for expenses within the date range
            DataSet expensesDataSet = await objbal.GetExpensesDashData(startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            // Create a list to store expense data for the donut graph
            List<object> expenseChartData = new List<object>();

            // Check if data is returned
            if (expensesDataSet.Tables.Count > 0 && expensesDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in expensesDataSet.Tables[0].Rows)
                {
                    // Create an anonymous object for each expense row
                    var expenseData = new
                    {
                        ExpenseName = row["ExpenseName"].ToString(),
                        TotalAmount = row["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["TotalAmount"]) : 0.0m
                    };

                    // Add the expense data to the list
                    expenseChartData.Add(expenseData);
                }
            }

            // Return the data as JSON for the frontend to use in the donut graph
            return Json(new { success = true, data = expenseChartData }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Json Method to fetch Cash Flow chart data from database
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> CashFlowChartAsyncSGS(DateTime startDate, DateTime endDate)
        {
            // Fetch the dataset for cash flow within the specified date range
            DataSet cashFlowDataSet = await objbal.GetCashFlowDashData(startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            // Create a list to store cash flow data for the line graph
            List<object> cashFlowChartData = new List<object>();

            // Check if data is returned
            if (cashFlowDataSet.Tables.Count > 0 && cashFlowDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in cashFlowDataSet.Tables[0].Rows)
                {
                    // Create an anonymous object for each cash flow row
                    var cashFlowData = new
                    {
                        Month = row["Month"].ToString(),
                        TotalIncome = row["TotalIncome"] != DBNull.Value ? Convert.ToDecimal(row["TotalIncome"]) : 0.0m,
                        TotalExpense = row["TotalExpense"] != DBNull.Value ? Convert.ToDecimal(row["TotalExpense"]) : 0.0m,
                        GrandTotal = row["GrandTotal"] != DBNull.Value ? Convert.ToDecimal(row["GrandTotal"]) : 0.0m,
                        Cash = row["TotalCash"] != DBNull.Value ? Convert.ToDecimal(row["TotalCash"]) : 0.0m,
                        Bank = row["TotalBank"] != DBNull.Value ? Convert.ToDecimal(row["TotalBank"]) : 0.0m,
                        Cheque = row["TotalCheque"] != DBNull.Value ? Convert.ToDecimal(row["TotalCheque"]) : 0.0m,
                        Other = row["OtherExpense"] != DBNull.Value ? Convert.ToDecimal(row["OtherExpense"]) : 0.0m
                    };

                    // Add the cash flow data to the list
                    cashFlowChartData.Add(cashFlowData);
                }
            }

            // Return the data as JSON for the frontend to use in the line graph
            return Json(new { success = true, data = cashFlowChartData }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Json Method to feth Current bank balance data from database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetCurrentBankBalancesAsyncSGS()
        {
            try
            {
                // Fetch the dataset for current bank balances
                DataSet bankBalanceDataSet = await objbal.GetCurrentBankBalancesDashData();

                // Create a list to store bank data
                List<object> bankBalances = new List<object>();

                // Check if data is returned
                if (bankBalanceDataSet.Tables.Count > 0 && bankBalanceDataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in bankBalanceDataSet.Tables[0].Rows)
                    {
                        // Mask the account number except for the last 4 digits
                        string accountNumber = row["AccountNumber"].ToString();
                        string maskedAccountNumber = accountNumber.Length > 4
                            ? new string('*', accountNumber.Length - 4) + accountNumber.Substring(accountNumber.Length - 4)
                            : accountNumber;

                        // Create an anonymous object for each bank balance row
                        var bankData = new
                        {
                            BankName = row["BankName"].ToString(),
                            AccountType = row["AccountType"].ToString(),
                            AccountNumber = maskedAccountNumber,
                            CurrentBalance = row["CurrentBalance"] != DBNull.Value ? Convert.ToDecimal(row["CurrentBalance"]) : 0.0m
                        };

                        // Add the bank data to the list
                        bankBalances.Add(bankData);
                    }
                }

                // Return the data as JSON for the frontend
                return Json(new { success = true, data = bankBalances }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = "Error fetching bank balances." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Json Method to fetch Daily Cash Flow data from database
        /// </summary>
        /// <param name="selectedDate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetDailyCashFlowDataAsync(string selectedDate)
        {
            try
            {
                // Use the provided date or fallback to today's date
                var date = string.IsNullOrWhiteSpace(selectedDate)
                           ? DateTime.Now.ToString("yyyy-MM-dd")
                           : selectedDate;

                // Call BAL method to fetch the data
                DataSet cashFlowDataSet = await objbal.GetDailyCashFlowDataAsync(date);

                if (cashFlowDataSet.Tables.Count > 0 && cashFlowDataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow row = cashFlowDataSet.Tables[0].Rows[0];

                    var cashFlowData = new
                    {
                        StartingBalance = row["StartingBalance"] != DBNull.Value ? Convert.ToDecimal(row["StartingBalance"]) : 0.0m,
                        Incoming = row["Incoming"] != DBNull.Value ? Convert.ToDecimal(row["Incoming"]) : 0.0m,
                        Banktransfer = row["BankTransfers"] != DBNull.Value ? Convert.ToDecimal(row["BankTransfers"]) : 0.0m,
                        Outgoing = row["Outgoing"] != DBNull.Value ? Convert.ToDecimal(row["Outgoing"]) : 0.0m,
                        EndingBalance = row["EndingBalance"] != DBNull.Value ? Convert.ToDecimal(row["EndingBalance"]) : 0.0m
                    };
                    ViewBag.CashInHand = Convert.ToDecimal(cashFlowData.EndingBalance);
                    return Json(new { success = true, data = cashFlowData }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, message = "No data available for the selected date." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while fetching cash flow data." }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Json Method to fetch Total Receivables and Total Payables data from database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetReceivablesPayablesDataAsyncSGS()
        {
            try
            {
                DataSet dataSet = await objbal.GetReceivablesPayablesDashData();

                decimal totalReceivables = 0;
                decimal totalReceived = 0;
                decimal overdueReceivables = 0;
                decimal totalPayables = 0;
                decimal totalPaid = 0;
                decimal overduePayables = 0;

                // Voucher-specific variables
                int totalVouchers = 0;
                decimal totalVoucherAmount = 0;
                int usedVoucherCount = 0;
                decimal usedVoucherAmount = 0;
                int unusedVoucherCount = 0;
                decimal unusedVoucherAmount = 0;

                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow row = dataSet.Tables[0].Rows[0];
                    totalReceivables = Convert.ToDecimal(row["TotalReceivables"]);
                    totalReceived = Convert.ToDecimal(row["TotalReceived"]);
                    overdueReceivables = Convert.ToDecimal(row["TotalOverdueReceivables"]);
                    totalPayables = Convert.ToDecimal(row["TotalPayables"]);
                    totalPaid = Convert.ToDecimal(row["TotalPaid"]);
                    overduePayables = Convert.ToDecimal(row["TotalOverduePayables"]);

                    // Extract voucher data
                    totalVouchers = Convert.ToInt32(row["TotalVouchers"]);
                    totalVoucherAmount = Convert.ToDecimal(row["TotalVoucherAmount"]);
                    usedVoucherCount = Convert.ToInt32(row["UsedVoucherCount"]);
                    usedVoucherAmount = Convert.ToDecimal(row["UsedVoucherAmount"]);
                    unusedVoucherCount = Convert.ToInt32(row["UnusedVoucherCount"]);
                    unusedVoucherAmount = Convert.ToDecimal(row["UnusedVoucherAmount"]);
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        TotalReceivables = totalReceivables,
                        TotalReceived = totalReceived,
                        OverdueReceivables = overdueReceivables,
                        TotalPayables = totalPayables,
                        TotalPaid = totalPaid,
                        OverduePayables = overduePayables,
                        TotalVouchers = totalVouchers,
                        TotalVoucherAmount = totalVoucherAmount,
                        UsedVoucherCount = usedVoucherCount,
                        UsedVoucherAmount = usedVoucherAmount,
                        UnusedVoucherCount = unusedVoucherCount,
                        UnusedVoucherAmount = unusedVoucherAmount
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Json Method to fetch Bank Balance data from database
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetBankBalanceSGS(int accountId)
        {
            try
            {
                decimal bankBalance = await GetBankBalanceAsyncSGS(accountId);
                return Json(new { success = true, balance = bankBalance }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an appropriate response
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Fetches Bank BAlance based on account id and returns bank balance
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>

        private async Task<decimal> GetBankBalanceAsyncSGS(int accountId)
        {
            // Call BAL method to get the bank balance data
            var result = await objbal.GetBankBalanceDashData(accountId.ToString());

            // Check if the result has any tables and rows
            if (result?.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
            {
                var bankBalanceValue = result.Tables[0].Rows[0][1];

                if (bankBalanceValue != DBNull.Value && decimal.TryParse(bankBalanceValue.ToString(), out decimal bankBalance))
                {
                    return bankBalance;
                }
            }

            return 0;  // Default to 0 if no data is found or conversion fails
        }

        /// <summary>
        /// Json Method to fetch Bank Balance of Voucher data from database
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetBankBalanceVoucherSGS(int accountId)
        {
            try
            {
                // Call the method to fetch bank balance and minimum balance
                var result = await GetBankBalanceVoucherAsyncSGS(accountId);

                if (result.success)
                {
                    return Json(new
                    {
                        success = true,
                        balance = result.balance,
                        minimumBalance = result.minimumBalance
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, error = "Unable to fetch balance details." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// A method that returns current balance and minimum balance to its method reference
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        private async Task<(bool success, decimal balance, decimal minimumBalance)> GetBankBalanceVoucherAsyncSGS(int accountId)
        {
            try
            {
                var result = await objbal.GetBankBalanceVoucher(accountId.ToString());

                if (result?.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    var bankBalanceValue = result.Tables[0].Rows[0]["CurrentBalance"];
                    var minimumBalanceValue = result.Tables[0].Rows[0]["MinimumBalance"];

                    decimal bankBalance = bankBalanceValue != DBNull.Value && decimal.TryParse(bankBalanceValue.ToString(), out var bb) ? bb : 0;
                    decimal minimumBalance = minimumBalanceValue != DBNull.Value && decimal.TryParse(minimumBalanceValue.ToString(), out var mb) ? mb : 0;

                    return (true, bankBalance, minimumBalance);
                }
            }
            catch
            {
                // Log exception
            }

            return (false, 0, 0);
        }

        /// <summary>
        /// View to show List of all Vouchers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ListAllVouchersAsyncSGS()
        {
            //List<Accountant> vouchers = await GetVouchersList();
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                // Voucher Type List
                List<SelectListItem> VoucherTypeList = new List<SelectListItem>();
                DataSet ds = await objbal.VoucherTypeAsyncSGS(objac);
                List<SelectListItem> TypeList = new List<SelectListItem>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    TypeList.Add(new SelectListItem
                    {
                        Text = dr["VoucherType"].ToString(),
                        Value = dr["VoucherTypeId"].ToString()
                    });
                }
                VoucherTypeList.AddRange(TypeList);
                ViewBag.VoucherTypeList = VoucherTypeList;
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
            {
                //new BreadcrumbItem { Name = "Home", Url = "/" },
                new BreadcrumbItem { Name = "AccountantDashboard", Url = "AccountantDashboardAsyncSGS" },
               new BreadcrumbItem { Name = "Voucher Managment"}, // Adjust URL as needed
            };

                ViewBag.Breadcrumbs = breadcrumbs;
                return View();
            }
        }
        [HttpGet]
        public async Task<ActionResult> ListAllVouchersAsyncSGS1()
        {
            //List<Accountant> vouchers = await GetVouchersList();
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                // Voucher Type List
                List<SelectListItem> VoucherTypeList = new List<SelectListItem>();
                DataSet ds = await objbal.VoucherTypeAsyncSGS(objac);
                List<SelectListItem> TypeList = new List<SelectListItem>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    TypeList.Add(new SelectListItem
                    {
                        Text = dr["VoucherType"].ToString(),
                        Value = dr["VoucherTypeId"].ToString()
                    });
                }
                VoucherTypeList.AddRange(TypeList);
                ViewBag.VoucherTypeList = VoucherTypeList;
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
            {
                //new BreadcrumbItem { Name = "Home", Url = "/" },
                new BreadcrumbItem { Name = "AccountantDashboard", Url = "AccountantDashboardAsyncSGS" },
               new BreadcrumbItem { Name = "Voucher Managment"}, // Adjust URL as needed
            };

                ViewBag.Breadcrumbs = breadcrumbs;
                return View();
            }
        }

        /// <summary>
        /// View to show List of recently created Vouchers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ListVoucherAsyncSGS()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login page if staff code is not found in session
            }
            else
            {
                string staffCode = Session["StaffCode"].ToString(); // Retrieve staff code from session
                                                                    //string branchCode = Session["BranchCode"].ToString(); // Retrieve branch code from session

                DataSet ds = new DataSet();

                // Pass staff code and branch code to the method for fetching tests
                ds = await objbal.ListVoucherAsyncSGS(staffCode);

                Accountant objtr1 = new Accountant();
                List<Accountant> AddExpense1 = new List<Accountant>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Accountant objtn = new Accountant
                    {
                        VoucherId = row.IsNull("VoucherId") ? 0 : Convert.ToInt32(row["VoucherId"]),
                        VoucherCode = row.IsNull("VoucherCode") ? string.Empty : row["VoucherCode"].ToString(),

                        // Determine AmountReceiver based on VendorName and StaffName
                        AmountReceiver = !row.IsNull("VendorName") && !string.IsNullOrEmpty(row["VendorName"].ToString()) ? row["VendorName"].ToString() :
                     !row.IsNull("StaffName") && !string.IsNullOrEmpty(row["StaffName"].ToString()) ? row["StaffName"].ToString() : string.Empty,

                        // Determine AmountReceiverType based on VendorName and StaffName
                        AmountReceiverType = !row.IsNull("VendorName") && !string.IsNullOrEmpty(row["VendorName"].ToString()) ? "Vendor" :
                         !row.IsNull("StaffName") && !string.IsNullOrEmpty(row["StaffName"].ToString()) ? "Staff" : string.Empty,

                        VendorName = row.IsNull("VendorName") ? string.Empty : row["VendorName"].ToString(),
                        Amount = row.IsNull("Amount") ? 0.0m : decimal.Parse(row["Amount"].ToString()),
                        AmountPaidTo = row.IsNull("StaffName") ? string.Empty : row["StaffName"].ToString(),
                        Description = row.IsNull("Description") ? string.Empty : row["Description"].ToString(),
                        PaymentMode = row.IsNull("PaymentMode") ? string.Empty : row["PaymentMode"].ToString(),
                        BankId = row.IsNull("BankId") ? 0 : Convert.ToInt32(row["BankId"]),
                        ChequeDate = row.IsNull("ChequeDate") ? DateTime.MinValue : DateTime.Parse(row["ChequeDate"].ToString()),
                        Balance = row.IsNull("Balance") ? 0.0m : decimal.Parse(row["Balance"].ToString()),
                        Currency = row.IsNull("Currency") ? string.Empty : row["Currency"].ToString(),
                        TransactionId = row.IsNull("TransactionId_ChequeNo") ? string.Empty : row["TransactionId_ChequeNo"].ToString(),
                        VoucherType = row.IsNull("VoucherType") ? string.Empty : row["VoucherType"].ToString(),
                        VoucherDate = row.IsNull("VoucherDate") ? DateTime.MinValue : DateTime.Parse(row["VoucherDate"].ToString()),
                        StaffCode = row.IsNull("StaffCode") ? string.Empty : row["StaffCode"].ToString(),
                        StatusId = row.IsNull("StatusId") ? 0 : Convert.ToInt32(row["StatusId"])
                    };


                    AddExpense1.Add(objtn);
                }

                objtr1.lstVoucher = AddExpense1;

                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
            {
                //new BreadcrumbItem { Name = "Home", Url = "/" },
                new BreadcrumbItem { Name = "TrainerDashboard", Url = "TrainerDashboardAsyncTS/Trainer" },
               new BreadcrumbItem { Name = "Test Managment", Url = "ListTestManagementAsynchTS/Trainer" }, // Adjust URL as needed
            };

                ViewBag.Breadcrumbs = breadcrumbs;
                return PartialView("ListVoucherAsyncSGS", objtr1);
            }
        }

        /// <summary>
        /// View to show List of Pending Vouchers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ListPendingVoucherAsyncSGS()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login page if staff code is not found in session
            }
            else
            {
                string staffCode = Session["StaffCode"].ToString(); // Retrieve staff code from session
                                                                    //string branchCode = Session["BranchCode"].ToString(); // Retrieve branch code from session

                DataSet ds = new DataSet();

                // Pass staff code and branch code to the method for fetching tests
                ds = await objbal.ListPendingVoucherAsyncSGS(staffCode);

                Accountant objtr1 = new Accountant();
                List<Accountant> AddExpense1 = new List<Accountant>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Accountant objtn = new Accountant
                    {
                        VoucherId = row.IsNull("VoucherId") ? 0 : Convert.ToInt32(row["VoucherId"]),
                        VoucherCode = row.IsNull("VoucherCode") ? string.Empty : row["VoucherCode"].ToString(),
                        // Determine AmountReceiver based on VendorName and StaffName
                        AmountReceiver = !row.IsNull("VendorName") && !string.IsNullOrEmpty(row["VendorName"].ToString()) ? row["VendorName"].ToString() :
                     !row.IsNull("StaffName") && !string.IsNullOrEmpty(row["StaffName"].ToString()) ? row["StaffName"].ToString() : string.Empty,

                        // Determine AmountReceiverType based on VendorName and StaffName
                        AmountReceiverType = !row.IsNull("VendorName") && !string.IsNullOrEmpty(row["VendorName"].ToString()) ? "Vendor" :
                         !row.IsNull("StaffName") && !string.IsNullOrEmpty(row["StaffName"].ToString()) ? "Staff" : string.Empty,
                        VendorName = row.IsNull("VendorName") ? string.Empty : row["VendorName"].ToString(),
                        Amount = row.IsNull("Amount") ? 0.0m : decimal.Parse(row["Amount"].ToString()),
                        AmountPaidTo = row.IsNull("StaffName") ? string.Empty : row["StaffName"].ToString(),
                        Description = row.IsNull("Description") ? string.Empty : row["Description"].ToString(),
                        PaymentMode = row.IsNull("PaymentMode") ? string.Empty : row["PaymentMode"].ToString(),
                        BankId = row.IsNull("BankId") ? 0 : Convert.ToInt32(row["BankId"]),
                        ChequeDate = row.IsNull("ChequeDate") ? DateTime.MinValue : DateTime.Parse(row["ChequeDate"].ToString()),
                        Balance = row.IsNull("Balance") ? 0.0m : decimal.Parse(row["Balance"].ToString()),
                        Currency = row.IsNull("Currency") ? string.Empty : row["Currency"].ToString(),
                        TransactionId = row.IsNull("TransactionId_ChequeNo") ? string.Empty : row["TransactionId_ChequeNo"].ToString(),
                        VoucherType = row.IsNull("VoucherType") ? string.Empty : row["VoucherType"].ToString(),
                        VoucherDate = row.IsNull("VoucherDate") ? DateTime.MinValue : DateTime.Parse(row["VoucherDate"].ToString()),
                        StaffCode = row.IsNull("StaffCode") ? string.Empty : row["StaffCode"].ToString(),
                        StatusId = row.IsNull("StatusId") ? 0 : Convert.ToInt32(row["StatusId"])
                    };

                    AddExpense1.Add(objtn);
                }

                objtr1.lstVoucher = AddExpense1;

                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
            {
                //new BreadcrumbItem { Name = "Home", Url = "/" },
                new BreadcrumbItem { Name = "TrainerDashboard", Url = "TrainerDashboardAsyncTS/Trainer" },
               new BreadcrumbItem { Name = "Test Managment", Url = "ListTestManagementAsynchTS/Trainer" }, // Adjust URL as needed
            };

                ViewBag.Breadcrumbs = breadcrumbs;
                return PartialView("ListPendingVoucherAsyncSGS", objtr1);
            }
        }

        /// <summary>
        /// json datat that contains List of recently created Vouchers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> ListVoucherAsyncSGS1()
        {
            if (Session["StaffCode"] == null)
            {
                return Json(new { success = false, message = "User not logged in." }, JsonRequestBehavior.AllowGet);
            }

            string staffCode = Session["StaffCode"].ToString();
            DataSet ds = await objbal.ListVoucherAsyncSGS(staffCode);

            List<Accountant> AddExpense1 = new List<Accountant>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                AddExpense1.Add(new Accountant
                {
                    VoucherId = row.IsNull("VoucherId") ? 0 : Convert.ToInt32(row["VoucherId"]),
                    VoucherCode = row.IsNull("VoucherCode") ? string.Empty : row["VoucherCode"].ToString(),
                    AmountReceiver = !row.IsNull("VendorName") && !string.IsNullOrEmpty(row["VendorName"].ToString()) ? row["VendorName"].ToString() :
                                     !row.IsNull("StaffName") && !string.IsNullOrEmpty(row["StaffName"].ToString()) ? row["StaffName"].ToString() : string.Empty,
                    AmountReceiverType = !row.IsNull("VendorName") && !string.IsNullOrEmpty(row["VendorName"].ToString()) ? "Vendor" :
                                         !row.IsNull("StaffName") && !string.IsNullOrEmpty(row["StaffName"].ToString()) ? "Staff" : string.Empty,
                    VendorName = row.IsNull("VendorName") ? string.Empty : row["VendorName"].ToString(),
                    Amount = row.IsNull("Amount") ? 0.0m : decimal.Parse(row["Amount"].ToString()),
                    AmountPaidTo = row.IsNull("StaffName") ? string.Empty : row["StaffName"].ToString(),
                    Description = row.IsNull("Description") ? string.Empty : row["Description"].ToString(),
                    PaymentMode = row.IsNull("PaymentMode") ? string.Empty : row["PaymentMode"].ToString(),
                    BankId = row.IsNull("BankId") ? 0 : Convert.ToInt32(row["BankId"]),
                    ChequeDate = row.IsNull("ChequeDate") ? DateTime.MinValue : DateTime.Parse(row["ChequeDate"].ToString()),
                    Balance = row.IsNull("Balance") ? 0.0m : decimal.Parse(row["Balance"].ToString()),
                    Currency = row.IsNull("Currency") ? string.Empty : row["Currency"].ToString(),
                    TransactionId = row.IsNull("TransactionId_ChequeNo") ? string.Empty : row["TransactionId_ChequeNo"].ToString(),
                    VoucherType = row.IsNull("VoucherType") ? string.Empty : row["VoucherType"].ToString(),
                    VoucherDate = row.IsNull("VoucherDate") ? DateTime.MinValue : DateTime.Parse(row["VoucherDate"].ToString()),
                    StaffCode = row.IsNull("StaffCode") ? string.Empty : row["StaffCode"].ToString(),
                    StatusId = row.IsNull("StatusId") ? 0 : Convert.ToInt32(row["StatusId"])
                });
            }

            return Json(new { success = true, data = AddExpense1 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// json datat that contains List of Pending Vouchers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> ListPendingVoucherAsyncSGS1()
        {
            if (Session["StaffCode"] == null)
            {
                return Json(new { success = false, message = "User not logged in." }, JsonRequestBehavior.AllowGet);
            }

            string staffCode = Session["StaffCode"].ToString();
            DataSet ds = await objbal.ListPendingVoucherAsyncSGS(staffCode);

            List<Accountant> AddExpense1 = new List<Accountant>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                AddExpense1.Add(new Accountant
                {
                    VoucherId = row.IsNull("VoucherId") ? 0 : Convert.ToInt32(row["VoucherId"]),
                    VoucherCode = row.IsNull("VoucherCode") ? string.Empty : row["VoucherCode"].ToString(),
                    AmountReceiver = !row.IsNull("VendorName") && !string.IsNullOrEmpty(row["VendorName"].ToString()) ? row["VendorName"].ToString() :
                                     !row.IsNull("StaffName") && !string.IsNullOrEmpty(row["StaffName"].ToString()) ? row["StaffName"].ToString() : string.Empty,
                    AmountReceiverType = !row.IsNull("VendorName") && !string.IsNullOrEmpty(row["VendorName"].ToString()) ? "Vendor" :
                                         !row.IsNull("StaffName") && !string.IsNullOrEmpty(row["StaffName"].ToString()) ? "Staff" : string.Empty,
                    VendorName = row.IsNull("VendorName") ? string.Empty : row["VendorName"].ToString(),
                    Amount = row.IsNull("Amount") ? 0.0m : decimal.Parse(row["Amount"].ToString()),
                    AmountPaidTo = row.IsNull("StaffName") ? string.Empty : row["StaffName"].ToString(),
                    Description = row.IsNull("Description") ? string.Empty : row["Description"].ToString(),
                    PaymentMode = row.IsNull("PaymentMode") ? string.Empty : row["PaymentMode"].ToString(),
                    VoucherType = row.IsNull("VoucherType") ? string.Empty : row["VoucherType"].ToString(),
                    Balance = row.IsNull("Balance") ? 0.0m : decimal.Parse(row["Balance"].ToString()),
                    VoucherDate = row.IsNull("VoucherDate") ? DateTime.MinValue : DateTime.Parse(row["VoucherDate"].ToString())
                });
            }

            return Json(new { success = true, data = AddExpense1 }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// json datat that contains List of Completed Vouchers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> ListCompletedVoucherAsyncSGS1()
        {
            if (Session["StaffCode"] == null)
            {
                return Json(new { success = false, message = "User not logged in." }, JsonRequestBehavior.AllowGet);
            }

            string staffCode = Session["StaffCode"].ToString();
            DataSet ds = await objbal.ListCompletedVoucherAsyncSGS(staffCode);

            List<Accountant> AddExpense1 = new List<Accountant>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                AddExpense1.Add(new Accountant
                {
                    VoucherId = row.IsNull("VoucherId") ? 0 : Convert.ToInt32(row["VoucherId"]),
                    VoucherCode = row.IsNull("VoucherCode") ? string.Empty : row["VoucherCode"].ToString(),
                    AmountReceiver = !row.IsNull("VendorName") && !string.IsNullOrEmpty(row["VendorName"].ToString()) ? row["VendorName"].ToString() :
                                     !row.IsNull("StaffName") && !string.IsNullOrEmpty(row["StaffName"].ToString()) ? row["StaffName"].ToString() : string.Empty,
                    AmountReceiverType = !row.IsNull("VendorName") && !string.IsNullOrEmpty(row["VendorName"].ToString()) ? "Vendor" :
                                         !row.IsNull("StaffName") && !string.IsNullOrEmpty(row["StaffName"].ToString()) ? "Staff" : string.Empty,
                    VendorName = row.IsNull("VendorName") ? string.Empty : row["VendorName"].ToString(),
                    Amount = row.IsNull("Amount") ? 0.0m : decimal.Parse(row["Amount"].ToString()),
                    AmountPaidTo = row.IsNull("StaffName") ? string.Empty : row["StaffName"].ToString(),
                    Description = row.IsNull("Description") ? string.Empty : row["Description"].ToString(),
                    VoucherType = row.IsNull("VoucherType") ? string.Empty : row["VoucherType"].ToString(),
                    PaymentMode = row.IsNull("PaymentMode") ? string.Empty : row["PaymentMode"].ToString(),
                    Balance = row.IsNull("Balance") ? 0.0m : decimal.Parse(row["Balance"].ToString()),
                    VoucherDate = row.IsNull("VoucherDate") ? DateTime.MinValue : DateTime.Parse(row["VoucherDate"].ToString())
                });
            }

            return Json(new { success = true, data = AddExpense1 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// View to show List of completed Vouchers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ListCompletedVoucherAsyncSGS()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login page if staff code is not found in session
            }
            else
            {
                string staffCode = Session["StaffCode"].ToString(); // Retrieve staff code from session
                string branchCode = Session["BranchCode"].ToString(); // Retrieve branch code from session

                DataSet ds = new DataSet();

                // Pass staff code and branch code to the method for fetching tests
                ds = await objbal.ListCompletedVoucherAsyncSGS(staffCode);

                Accountant objtr1 = new Accountant();
                List<Accountant> AddExpense1 = new List<Accountant>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Accountant objtn = new Accountant
                    {
                        VoucherId = row.IsNull("VoucherId") ? 0 : Convert.ToInt32(row["VoucherId"]),
                        VoucherCode = row.IsNull("VoucherCode") ? string.Empty : row["VoucherCode"].ToString(),
                        // Determine AmountReceiver based on VendorName and StaffName
                        AmountReceiver = !row.IsNull("VendorName") && !string.IsNullOrEmpty(row["VendorName"].ToString()) ? row["VendorName"].ToString() :
                        !row.IsNull("StaffName") && !string.IsNullOrEmpty(row["StaffName"].ToString()) ? row["StaffName"].ToString() : string.Empty,

                        // Determine AmountReceiverType based on VendorName and StaffName
                        AmountReceiverType = !row.IsNull("VendorName") && !string.IsNullOrEmpty(row["VendorName"].ToString()) ? "Vendor" :
                         !row.IsNull("StaffName") && !string.IsNullOrEmpty(row["StaffName"].ToString()) ? "Staff" : string.Empty,
                        VendorName = row.IsNull("VendorName") ? string.Empty : row["VendorName"].ToString(),
                        Amount = row.IsNull("Amount") ? 0.0m : decimal.Parse(row["Amount"].ToString()),
                        AmountPaidTo = row.IsNull("StaffName") ? string.Empty : row["StaffName"].ToString(),
                        Description = row.IsNull("Description") ? string.Empty : row["Description"].ToString(),
                        PaymentMode = row.IsNull("PaymentMode") ? string.Empty : row["PaymentMode"].ToString(),
                        BankId = row.IsNull("BankId") ? 0 : Convert.ToInt32(row["BankId"]),
                        ChequeDate = row.IsNull("ChequeDate") ? DateTime.MinValue : DateTime.Parse(row["ChequeDate"].ToString()),
                        Balance = row.IsNull("Balance") ? 0.0m : decimal.Parse(row["Balance"].ToString()),
                        Currency = row.IsNull("Currency") ? string.Empty : row["Currency"].ToString(),
                        TransactionId = row.IsNull("TransactionId_ChequeNo") ? string.Empty : row["TransactionId_ChequeNo"].ToString(),
                        VoucherType = row.IsNull("VoucherType") ? string.Empty : row["VoucherType"].ToString(),
                        VoucherDate = row.IsNull("VoucherDate") ? DateTime.MinValue : DateTime.Parse(row["VoucherDate"].ToString()),
                        StaffCode = row.IsNull("StaffCode") ? string.Empty : row["StaffCode"].ToString(),
                        StatusId = row.IsNull("StatusId") ? 0 : Convert.ToInt32(row["StatusId"])
                    };

                    AddExpense1.Add(objtn);
                }

                objtr1.lstVoucher = AddExpense1;

                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
            {
                //new BreadcrumbItem { Name = "Home", Url = "/" },
                new BreadcrumbItem { Name = "AccountantDashboard", Url = "AccountantDashboardAsyncSGS/Accountant" },
               new BreadcrumbItem { Name = "Voucher Managment", Url = "ListAllVouchersAsyncSGS/Accountant" }, // Adjust URL as needed
            };

                ViewBag.Breadcrumbs = breadcrumbs;

                return PartialView("ListCompletedVoucherAsyncSGS", objtr1);
            }
        }

        /// <summary>
        /// View to add voucher
        /// </summary>
        /// <param name="salaryMonthIds"></param>
        /// <param name="voucherType"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> AddVoucherAsyncSGS(string salaryMonthIds, int voucherType = 0, decimal amount = 0, string context = "voucherModule")
        {
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }

            // Populate session variables
            Session["salaryMonthIds"] = salaryMonthIds;
            objac.salaryMonthIds = salaryMonthIds;
            objac.StaffCode = Session["StaffCode"].ToString();
            objac.BranchCode = Session["BranchCode"].ToString();

            // Fetch Voucher Types
            ViewBag.VoucherTypeList = (await objbal.VoucherTypeAsyncSGS(objac))
                .Tables[0]
                .AsEnumerable()
                .Select(dr => new SelectListItem
                {
                    Text = dr["VoucherType"].ToString(),
                    Value = dr["VoucherTypeId"].ToString()
                }).ToList();

            // Set Default Voucher Date and Voucher Number
            objac.VoucherDate = DateTime.Now;
            ViewBag.VoucherNumber = objac.VoucherCode;

            // Fetch Staff List
            ViewBag.combinedReportingList = (await objbal.StaffNameforVoucherAsyncSGS(objac))
                .Tables[0]
                .AsEnumerable()
                .Select(dr => new SelectListItem
                {
                    Text = dr["StaffName"].ToString(),
                    Value = dr["StaffCode"].ToString()
                }).ToList();

            // Fetch Vendor List
            ViewBag.VendorList = (await objbal.VendorNameforVoucherAsyncSGS(objac))
                .Tables[0]
                .AsEnumerable()
                .Select(dr => new SelectListItem
                {
                    Text = dr["VendorName"].ToString(),
                    Value = dr["VendorCode"].ToString()
                }).ToList();

            // Fetch Bank Accounts with Masked Account Numbers
            ViewBag.BankAccountList = (await objbal.BankAccountforVoucherAsyncSGS(objac))
                .Tables[0]
                .AsEnumerable()
                .Select(dr =>
                {
                    string accountNumber = dr["AccountNumber"].ToString();
                    string maskedAccountNumber = "" + accountNumber.Substring(accountNumber.Length - 4);
                    return new SelectListItem
                    {
                        Text = $"{dr["BankName"]} - {dr["AccountHolderName"]} - {maskedAccountNumber}",
                        Value = dr["BankId"].ToString()
                    };
                }).ToList();

            // Fetch Cash In Hand
            DataSet cashFlowDataSet = await objbal.GetDailyCashFlowDataAsync(DateTime.Now.ToString("yyyy-MM-dd"));
            ViewBag.CashInHand = cashFlowDataSet.Tables.Count > 0 && cashFlowDataSet.Tables[0].Rows.Count > 0
                ? Convert.ToDecimal(cashFlowDataSet.Tables[0].Rows[0]["EndingBalance"] ?? 0.0m)
                : 0.0m;

            // Breadcrumbs
            ViewBag.Breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Name = "AccountantDashboard", Url = "AccountantDashboardAsyncSGS" },
                new BreadcrumbItem { Name = "Voucher Management", Url = "ListAllVouchersAsyncSGS" },
                new BreadcrumbItem { Name = "Add Voucher", Url = "AddCashVoucherAsyncSGS" }
            };

            // Additional View Data
            ViewBag.SelectedVoucherType = voucherType;
            ViewBag.Amount = amount;
            ViewBag.Context = context; // Pass context to the view
            objac.IsitEdit = true; // or false, based on the condition


            return PartialView(objac);
        }




        /// <summary>
        /// Post method for adding voucher
        /// </summary>
        /// <param name="objA"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AddVoucherAsyncSGS(Accountant objA, string context = "voucherModule")
        {
            try
            {
                // Validate session variables
                if (Session["StaffCode"] == null || Session["BranchCode"] == null)
                {
                    throw new Exception("StaffCode or BranchCode is missing in the session.");
                }

                objA.StaffCode = Session["StaffCode"].ToString();
                objA.BranchCode = Session["BranchCode"].ToString();
                objA.VoucherCode = await objbal.GetMaxVoucherCodeAsyncSGS(objA);

                // Add voucher
                await objbal.AddVoucherAsyncSGS(objA);

                // Process salary month IDs if available
                string salaryMonthIds = Session["salaryMonthIds"] as string;
                if (!string.IsNullOrEmpty(salaryMonthIds))
                {
                    var monthIdArray = salaryMonthIds.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    foreach (var monthId in monthIdArray)
                    {
                        try
                        {
                            var monthAccountant = new Accountant
                            {
                                StaffCode = objA.StaffCode,
                                BranchCode = objA.BranchCode,
                                combinedMonthYear = monthId.Trim()
                            };

                            DataSet ds1 = await objbal.addexpcodeinsalaray();
                            if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                            {
                                objA.BatchCode = ds1.Tables[0].Rows[0]["TransactionCode"].ToString();
                            }

                            await objbal.ChangeStatusOfsalaryMonthIdsAsyncSS(monthAccountant, objA);
                        }
                        catch (Exception monthEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error processing month ID {monthId}: {monthEx.Message}");
                        }
                    }
                }

                // Build JSON response
                var response = new
                {
                    success = true,
                    redirectUrl = (context == "voucherModule") ? Url.Action("ListAllVouchersAsyncSGS", "Accountant") : null,
                    voucherCode = (context == "matchVoucher") ? objA.VoucherCode : null,
                    voucherText = (context == "matchVoucher") ? $"{objA.VoucherCode}: {objA.Description}" : null,
                    message = "Voucher added successfully."
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred while saving data: {ex.Message}" });
            }
        }



        /// <summary>
        /// Json Method to fetch Staff Bank Details data from database
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetStaffBankDetailsAsync(string staffId)
        {
            // Call the business logic layer method to get the data
            DataSet staffDetails = await objbal.StaffBankDataforVoucherAsyncSGS(staffId);

            // Initialize a list to store month-wise data

            // Check if the dataset has data
            if (staffDetails.Tables.Count > 0 && staffDetails.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in staffDetails.Tables[0].Rows)
                {
                    // Extract values from the DataRow
                    string BankName = dr["BankName"].ToString();
                    long BankAccountNumber = dr["AccountNumber"] != DBNull.Value ? long.Parse(dr["AccountNumber"].ToString()) : 0L;
                    string AccountHolderName = dr["AccountHolderName"].ToString();
                    string IFSCCode = dr["IFSCCode"].ToString();

                    // Add data to the result list
                    var resultData = new
                    {
                        bankName = BankName,
                        accountNumber = BankAccountNumber,
                        accountHolderName = AccountHolderName,
                        ifscCode = IFSCCode
                    };
                    return Json(resultData, JsonRequestBehavior.AllowGet);

                }

            }
            return Json(new { }, JsonRequestBehavior.AllowGet); // Return an empty object if no data


        }
        /// <summary>
        /// Json Method to fetch Vendor Bank Details data from database
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetVendorBankDetailsAsync(string vendorId)
        {
            // Call the business logic layer method to get the data
            DataSet vendorDetails = await objbal.VendorBankDataforVoucherAsyncSGS(vendorId);

            if (vendorDetails.Tables.Count > 0 && vendorDetails.Tables[0].Rows.Count > 0)
            {
                DataRow dr = vendorDetails.Tables[0].Rows[0];

                long BankAccountNumber = dr["AccountNumber"] != DBNull.Value ? long.Parse(dr["AccountNumber"].ToString()) : 0L;
                string AccountHolderName = dr["AccountHolderName"].ToString();
                string IFSCCode = dr["IFSCCode"].ToString();

                var resultData = new
                {
                    accountNumber = BankAccountNumber,
                    accountHolderName = AccountHolderName,
                    ifscCode = IFSCCode
                };

                return Json(resultData, JsonRequestBehavior.AllowGet);
            }

            return Json(new { }, JsonRequestBehavior.AllowGet); // Return an empty object if no data
        }

        public BALAccountant GetObjbal()
        {
            return objbal;
        }

        /// <summary>
        /// Json Method to fetch Staff of Admin Details data from database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetAdminStaff()
        {
            try
            {
                // Initialize Accountant object with session data
                Accountant objac = new Accountant
                {
                    StaffCode = Session["StaffCode"]?.ToString(),
                    BranchCode = Session["BranchCode"]?.ToString()
                };

                // Validate session variables
                if (string.IsNullOrEmpty(objac.StaffCode) || string.IsNullOrEmpty(objac.BranchCode))
                {
                    return Json(new { success = false, message = "Session expired. Please log in again." }, JsonRequestBehavior.AllowGet);
                }

                // Fetch admin staff list
                DataSet staffDataSet = await objbal.GetAdminStaff(objac);
                if (staffDataSet == null || staffDataSet.Tables.Count == 0 || staffDataSet.Tables[0].Rows.Count == 0)
                {
                    return Json(new { success = false, message = "No staff data found." }, JsonRequestBehavior.AllowGet);
                }

                // Map the data to JSON response
                var staffList = staffDataSet.Tables[0].AsEnumerable().Select(row => new
                {
                    StaffId = row["StaffId"],
                    StaffCode = row["StaffCode"],
                    StaffName = row["StaffName"],
                    StaffPositionId = row["StaffPositionId"],
                    Email = row["Email"],
                    PhoneNo = row["PhoneNo"]
                }).ToList();

                return Json(new { success = true, data = staffList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// View to show Details of Vouchers
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> DetailsVoucherSGS(string id)
        {
            // Check if session exists, if not redirect to Login page
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                // Retrieve staff and branch codes from the session
                objac.StaffCode = Session["StaffCode"].ToString();
                objac.BranchCode = Session["BranchCode"].ToString();

                SqlDataReader drClient;
                drClient = await objbal.ListClientDetailsAsyncVP(objac);
                if (drClient.Read())
                {
                    ViewBag.logo = drClient["Logo"].ToString();
                    ViewBag.address = drClient["BranchAddress"].ToString();
                    ViewBag.ClientName = drClient["ClientName"].ToString();
                }

                DataSet ds = new DataSet();
                DataSet dsVoucherStatement = new DataSet(); // To hold voucher statement data
                objac.VoucherCode = id;

                // Fetch voucher data
                ds = await objbal.GetVoucherDataSGS(objac);

                Accountant objtr1 = new Accountant();
                List<Accountant> AddExpense1 = new List<Accountant>();

                // Extract and map the data to Accountant object
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string receiverBankIFSCCode = !row.IsNull("VendorIFSCCode") ? row["VendorIFSCCode"].ToString() :
                                                   (row.IsNull("StaffIFSCCode") ? string.Empty : row["StaffIFSCCode"].ToString());

                    string bankName = string.Empty;
                    if (!string.IsNullOrEmpty(receiverBankIFSCCode))
                    {
                        if (!row.IsNull("VendorIFSCCode"))
                        {
                            // Fetch bank name via Razorpay IFSC API if Vendor's data exists
                            bankName = await FetchBankNameFromIFSCAsync(receiverBankIFSCCode);
                        }
                        else
                        {
                            // Use staff bank name if vendor data is not available
                            bankName = row.IsNull("StaffBankName") ? string.Empty : row["StaffBankName"].ToString();
                        }
                    }

                    Accountant objtn = new Accountant
                    {
                        VoucherId = row.IsNull("VoucherId") ? 0 : Convert.ToInt32(row["VoucherId"]),
                        VoucherCode = row.IsNull("VoucherCode") ? string.Empty : row["VoucherCode"].ToString(),
                        VendorName = row.IsNull("VendorName") ? string.Empty : row["VendorName"].ToString(),
                        Amount = row.IsNull("Amount") ? 0.0m : decimal.Parse(row["Amount"].ToString()),
                        AmountPaidTo = row.IsNull("StaffName") ? string.Empty : row["StaffName"].ToString(),
                        Description = row.IsNull("Description") ? string.Empty : row["Description"].ToString(),
                        PaymentMode = row.IsNull("PaymentMode") ? string.Empty : row["PaymentMode"].ToString(),
                        BankId = row.IsNull("BankId") ? 0 : Convert.ToInt32(row["BankId"]),
                        BankName = row.IsNull("SelfBank") ? string.Empty : row["SelfBank"].ToString(),
                        ReceiverBankAccountNumber = !row.IsNull("VendorAccountNumber") ? Convert.ToInt64(row["VendorAccountNumber"].ToString()) :
                                                     (row.IsNull("StaffAccountNumber") ? 0 : Convert.ToInt64(row["StaffAccountNumber"].ToString())),
                        ReceiverBankAccountHolderName = !row.IsNull("VendorAccountHolderName") ? row["VendorAccountHolderName"].ToString() :
                                                         (row.IsNull("StaffAccountHolderName") ? string.Empty : row["StaffAccountHolderName"].ToString()),
                        ReceiverBankIFSCCode = receiverBankIFSCCode,
                        ReceiverBankName = bankName, // Set dynamically from API or staff data
                        ChequeDate = row.IsNull("ChequeDate") ? DateTime.MinValue : DateTime.Parse(row["ChequeDate"].ToString()),
                        Balance = row.IsNull("Balance") ? 0.0m : decimal.Parse(row["Balance"].ToString()),
                        Currency = row.IsNull("Currency") ? string.Empty : row["Currency"].ToString(),
                        TransactionId = row.IsNull("TransactionId_ChequeNo") ? string.Empty : row["TransactionId_ChequeNo"].ToString(),
                        VoucherType = row.IsNull("VoucherType") ? string.Empty : row["VoucherType"].ToString(),
                        VoucherDate = row.IsNull("VoucherDate") ? DateTime.MinValue : DateTime.Parse(row["VoucherDate"].ToString()),
                        StaffCode = row.IsNull("StaffCode") ? string.Empty : row["StaffCode"].ToString(),
                        StatusId = row.IsNull("StatusId") ? 0 : Convert.ToInt32(row["StatusId"])
                    };

                    // Add entry to the list
                    objtr1 = objtn;
                }

                // Call BAL class to get voucher statement data
                dsVoucherStatement = await objbal.GetVoucherStatementDataSGS(objac);

                // Create a new list for voucher statements and populate dynamically
                List<Accountant> lstVoucherStatement = new List<Accountant>();

                foreach (DataRow row in dsVoucherStatement.Tables[0].Rows)
                {
                    Accountant objStatement = new Accountant
                    {
                        SrNumber = row.IsNull("SrNumber") ? 0 : Convert.ToInt32(row["SrNumber"]),
                        VoucherDate = row.IsNull("VoucherDate") ? DateTime.MinValue : DateTime.Parse(row["VoucherDate"].ToString()),
                        Description = row.IsNull("Description") ? string.Empty : row["Description"].ToString(),
                        Amount = row.IsNull("Amount") ? 0.0m : decimal.Parse(row["Amount"].ToString()),
                        Balance = row.IsNull("Balance") ? 0.0m : decimal.Parse(row["Balance"].ToString())
                    };

                    lstVoucherStatement.Add(objStatement);
                }

                // Assign data to Accountant object
                objtr1.lstVoucherStatement = lstVoucherStatement;

                // Create breadcrumb items
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = "AccountantDashboard", Url = Url.Action("AccountantDashboardAsyncSGS", "Accountant") },
            new BreadcrumbItem { Name = "Voucher Management", Url = Url.Action("ListAllVouchersAsyncSGS", "Accountant") }
        };

                ViewBag.Breadcrumbs = breadcrumbs;

                // Pass the Accountant object to the partial view
                return PartialView("DetailsVoucherSGS", objtr1);
            }
        }

        // Helper function to fetch bank name based on IFSC code using Razorpay API
        private async Task<string> FetchBankNameFromIFSCAsync(string ifscCode)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"https://ifsc.razorpay.com/{ifscCode}";  // Razorpay IFSC API
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var bankDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
                    if (bankDetails != null && bankDetails.ContainsKey("BANK"))
                    {
                        return bankDetails["BANK"];
                    }
                }
            }
            return string.Empty;  // Fallback if no bank is found
        }



        /// <summary>
        /// Method to delete recently created voucher
        /// </summary>
        /// <param name="voucherId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> DeleteVoucher(string voucherId)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    Accountant objT = new Accountant();
                    objT.VoucherCode = voucherId;
                    objT.BranchCode = Session["BranchCode"].ToString();
                    objT.StaffCode = Session["StaffCode"].ToString();
                    await objbal.RemoveDataVoucher(objT);
                    return Json(new { success = true, message = "Voucher deleted successfully" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// method to collect voucher if balance is remaining
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> CollectVoucherAsyncSGS(string id)
        {
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                // Retrieve staff code and branch code from session
                objac.StaffCode = Session["StaffCode"].ToString();
                objac.BranchCode = Session["BranchCode"].ToString();

                DataSet ds = new DataSet();
                objac.VoucherCode = id;
                // Pass staff code and branch code to the method for fetching tests
                ds = await objbal.GetVoucherDataSGS(objac);

                Accountant objtr1 = new Accountant();
                List<Accountant> AddExpense1 = new List<Accountant>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Accountant objtn = new Accountant
                    {
                        VoucherId = row.IsNull("VoucherId") ? 0 : Convert.ToInt32(row["VoucherId"]),
                        VoucherCode = row.IsNull("VoucherCode") ? string.Empty : row["VoucherCode"].ToString(),
                        VendorName = row.IsNull("VendorName") ? string.Empty : row["VendorName"].ToString(),
                        Amount = row.IsNull("Amount") ? 0.0m : decimal.Parse(row["Amount"].ToString()),
                        AmountPaidTo = row.IsNull("StaffName") ? string.Empty : row["StaffName"].ToString(),
                        Description = row.IsNull("Description") ? string.Empty : row["Description"].ToString(),
                        PaymentMode = row.IsNull("PaymentMode") ? string.Empty : row["PaymentMode"].ToString(),
                        BankId = row.IsNull("BankId") ? 0 : Convert.ToInt32(row["BankId"]),
                        BankName = row.IsNull("SelfBank") ? string.Empty : row["SelfBank"].ToString(),
                        ChequeDate = row.IsNull("ChequeDate") ? DateTime.MinValue : DateTime.Parse(row["ChequeDate"].ToString()),
                        Balance = row.IsNull("Balance") ? 0.0m : decimal.Parse(row["Balance"].ToString()),
                        Currency = row.IsNull("Currency") ? string.Empty : row["Currency"].ToString(),
                        TransactionId = row.IsNull("TransactionId_ChequeNo") ? string.Empty : row["TransactionId_ChequeNo"].ToString(),
                        VoucherType = row.IsNull("VoucherType") ? string.Empty : row["VoucherType"].ToString(),
                        VoucherDate = row.IsNull("VoucherDate") ? DateTime.MinValue : DateTime.Parse(row["VoucherDate"].ToString()),
                        StaffCode = row.IsNull("StaffCode") ? string.Empty : row["StaffCode"].ToString(),
                        StatusId = row.IsNull("StatusId") ? 0 : Convert.ToInt32(row["StatusId"])
                    };
                    objtr1 = objtn;
                    AddExpense1.Add(objtn);
                }

                // Set the list of vouchers in the Accountant object
                objtr1.lstVoucher = AddExpense1;

                // Create breadcrumb items
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                {
                    // new BreadcrumbItem { Name = "Home", Url = "/" },
                    new BreadcrumbItem { Name = "AccountantDashboard", Url = "AccountantDashboardAsyncSGS/Accountant" },
                    new BreadcrumbItem { Name = "Voucher Management", Url = "ListAllVouchersAsyncSGS/Accountant" }, // Adjust URL as needed
                };

                ViewBag.Breadcrumbs = breadcrumbs;

                // Pass the Accountant object to the partial view
                return PartialView("CollectVoucherAsyncSGS", objtr1);
            }
        }
        #endregion //Shreyas's Region About Dashboard and Voucher Submodules End here 


        //----------------Jayash-  Accountant -----------------------------------start //
        /// <summary>
        ///This is for fetching Personal Attendance 
        /// </summary>
        /// 
        [HttpGet]





        public async Task<ActionResult> GetPersonalAttendanceAsyncJY(string StaffCode, string year, string month, string Breadcrumbs)
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                string staffCode;
                if (StaffCode != null)
                {
                    staffCode = StaffCode;
                    Session["Status"] = true;


                } else
                {
                    Session["Status"] = false;
                    DateTime currentDate = DateTime.Now;
                    year = string.IsNullOrEmpty(year) ? currentDate.Year.ToString() : year;
                    month = string.IsNullOrEmpty(month) ? currentDate.Month.ToString() : month;

                    staffCode = Session["StaffCode"].ToString();

                }


                // Fetch summary attendance
                DataSet dsSummary = await objbal.FetchPersonalAttendanceAsyncCountJY(staffCode, year, month);
                if (dsSummary.Tables.Count > 0 && dsSummary.Tables[0].Rows.Count > 0)
                {
                    var row = dsSummary.Tables[0].Rows[0];
                    objac.StaffName = (row["StaffName"].ToString());
                    objac.HalfDays = Convert.ToInt32(row["HalfDays"].ToString());
                    objac.WorkedDays = Convert.ToInt32(row["WorkedDays"].ToString());
                    objac.TotalMonthDays = Convert.ToInt32(row["TotalMonthDays"].ToString());
                    objac.LateMark = Convert.ToInt32(row["LateMarks"].ToString());
                    objac.MissPunching = Convert.ToInt32(row["MissPunching"].ToString());
                    objac.PayableDays = Convert.ToDecimal(row["PayableDays"].ToString());
                    objac.AbsentDays = Convert.ToInt32(row["AbsentDays"].ToString());

                }

                // Fetch detailed attendance
                List<Accountant> lstAttendance = new List<Accountant>();
                DataSet dsDetail = await objbal.FetchPersonalAttendanceAsyncJY(staffCode, year, month);
                if (dsDetail.Tables.Count > 0 && dsDetail.Tables[0].Rows.Count > 0)
                {
                    objac.LstAttendence = new List<Accountant>();
                    foreach (DataRow row in dsDetail.Tables[0].Rows)
                    {
                        Accountant ac = new Accountant();

                        if (row["Date"] != DBNull.Value)
                        {
                            if (DateTime.TryParse(row["Date"].ToString(), out DateTime date))
                            {
                                ac.Date = date;
                            } else
                            {
                                ac.Date = DateTime.MinValue;
                            }
                        }




                        if (row.IsNull("InTime"))
                        {
                            ac.InTime = null;
                        } else
                        {
                            ac.InTime = (TimeSpan)row["InTime"];
                        }


                        if (row.IsNull("OutTime"))
                        {
                            ac.OutTime = null;
                        } else
                        {
                            ac.OutTime = (TimeSpan)row["OutTime"];
                        }


                        // Handle OutTime field, allow null values

                        // Handle HoursWorked field, allow null values
                        ac.Hrs = row["HoursWorked"] != DBNull.Value ? row["HoursWorked"].ToString() : null;

                        // Handle Status field, allow null values
                        ac.Status = row["Status"] != DBNull.Value ? row["Status"].ToString() : null;

                        // Handle Remarks field, allow null values
                        ac.Remark = row["Remarks"] != DBNull.Value ? row["Remarks"].ToString() : null;

                        lstAttendance.Add(ac);
                    }



                }
                objac.LstAttendence = lstAttendance;
                if (Request.IsAjaxRequest())
                {
                    return PartialView("PersonalAttendancePartialJY", objac);
                }
                if (Breadcrumbs == "FromSalaryPayRoll")
                {

                    List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
  {
           new BreadcrumbItem { Name = "Dashboard", Url = "AccountantDashboardAsyncSGS/Accountant" },
      new BreadcrumbItem { Name = "Staff Pay Roll", Url = Url.Action("GetStaffForPaySSAsync", "Accountant") },
      new BreadcrumbItem { Name = "Attendence", Url = Url.Action("AttendenceOfAllStaffssAsync", "Accountant") }
  };

                    ViewBag.Breadcrumbs = breadcrumbs;
                } else
                {
                    List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
  {
           new BreadcrumbItem { Name = "Dashboard", Url = "AccountantDashboardAsyncSGS/Accountant" },
          new BreadcrumbItem { Name = "Attendance ", Url ="GetPersonalAttendanceAsyncJY" },
  };

                    ViewBag.Breadcrumbs = breadcrumbs;

                }

                return View(objac);
            }
        }




        //----------------Jayash-  Accountant -----------------------------------ENd //

        //        // GET: Accountant


        //        [HttpGet]
        //        public async Task StaffBindSSAsync()
        //        {
        //            string BranchCode = Session["BranchCode"].ToString();
        //            DataSet ds = await objbal.StaffListSSAsync(BranchCode);
        //            List<Accountant> AllStaff = new List<Accountant>();
        //            foreach (DataRow dr in ds.Tables[0].Rows)
        //            {
        //                Accountant objP = new Accountant
        //                {
        //                    StaffName = dr["Staff Name"].ToString(),
        //                    Department = dr["Department"].ToString(),
        //                    Designation = dr["Designation"].ToString(),
        //                    BankName = dr["Bank"].ToString(),
        //                    AccountNo =Convert.ToInt64( dr["Account Number"].ToString()),
        //                    IFSCCode = dr["IFSCCode"].ToString(),
        //                    GrossSalary = Convert.ToInt64(dr["GrossSalary"])

        //                };
        //                AllStaff.Add(objP);
        //                // AllStaff.Add(new SelectListItem { Text = dr["StaffName"].ToString(), Value = dr["StaffCode"].ToString() });
        //            }
        //            ViewBag.StaffList = AllStaff;
        //            objac = new Accountant { lstEmp = AllStaff };

        //        }




        #region //Siddhi's Cheque Receipt Details
        /// <summary>
        /// This method shows list of students who has given cheque
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ListAllChequeReceiptAsyncSM(string filter = "All", string status = "All")
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                objprop.Prefix = Session["Prefix"].ToString();
                objprop.BranchCode = Session["BranchCode"].ToString();
                DataSet ds = await objbal.ListAllChequeReceiptAsyncSM(objprop);
                List<Accountant> lstData1 = new List<Accountant>();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string admissionType = ds.Tables[0].Rows[i]["ADMISSION TYPE"].ToString();
                    string chequeStatus = ds.Tables[0].Rows[i]["STATUS"].ToString();

                    if ((filter == "All" || admissionType.Equals(filter, StringComparison.OrdinalIgnoreCase)) &&
                        (status == "All" || chequeStatus.Equals(status, StringComparison.OrdinalIgnoreCase)))
                    {
                        Accountant objAcc = new Accountant
                        {
                            TransactionCodeSM = ds.Tables[0].Rows[i]["RECEIPT NO."].ToString(),
                            TransactionDateSM = Convert.ToDateTime(ds.Tables[0].Rows[i]["RECEIVED DATE"].ToString()),
                            BankNameSM = ds.Tables[0].Rows[i]["BANK NAME"].ToString(),
                            ChequeNumberSM = ds.Tables[0].Rows[i]["CHEQUE NO."].ToString(),
                            //ChequeDateSM = Convert.ToDateTime(ds.Tables[0].Rows[i]["CHEQUE DATE"].ToString()),
                            StudentNameSM = ds.Tables[0].Rows[i]["STUDENT NAME"].ToString(),
                            AddmissionTypeSM = admissionType,
                            AmountSM = Convert.ToInt64(ds.Tables[0].Rows[i]["Amount"].ToString()),
                            CandidateCode = ds.Tables[0].Rows[i]["CANDIDATE CODE"].ToString(),
                        };

                        if (ds.Tables[0].Rows[i]["CHEQUE DATE"] != DBNull.Value &&
                           !string.IsNullOrEmpty(ds.Tables[0].Rows[i]["CHEQUE DATE"].ToString()))
                        {
                            objAcc.ChequeDateSM = Convert.ToDateTime(ds.Tables[0].Rows[i]["CHEQUE DATE"].ToString());
                        }

                        if (ds.Tables[0].Rows[i]["CHEQUE CLEARANCE DATE"] != DBNull.Value &&
                            !string.IsNullOrEmpty(ds.Tables[0].Rows[i]["CHEQUE CLEARANCE DATE"].ToString()))
                        {
                            objAcc.ChequeClearanceDateSM = Convert.ToDateTime(ds.Tables[0].Rows[i]["CHEQUE CLEARANCE DATE"].ToString());
                        }

                        objAcc.Status = chequeStatus;

                        // Ensure unique TransactionCodesm
                        if (!lstData1.Any(x => x.TransactionCodeSM == objAcc.TransactionCodeSM))
                        {
                            lstData1.Add(objAcc);
                        }
                    }
                }
                Accountant obj = new Accountant { lstChequeReceipt = lstData1 };

                return PartialView("ListAllChequeReceiptAsyncSM", obj);

            }
        }
        /// <summary>
        /// This is main view where the list binds
        /// </summary>
        /// <returns></returns>

        public async Task<ActionResult> ListAllChequesAsyncSM()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
         {

                 new BreadcrumbItem { Name = "Dashboard", Url =Url.Action("AccountantDashboardAsyncSGS", "Accountant")  },
                 new BreadcrumbItem { Name = "Cheque Details",  }


         };

                ViewBag.Breadcrumbs = breadcrumbs;
                return await Task.Run(() => View());
            }
        }
        /// <summary>
        /// This method is used to show the student fee details and clear cheque date
        /// </summary>
        /// <param name="id"></param>
        /// <param name="chequeDate"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<ActionResult> ShowStudentFeeChequeAsyncSM(string transactioncode, DateTime? chequeDate, DateTime? receiveddate)
        {
            Accountant accountant = new Accountant();
            accountant.TransactionCodeSM = transactioncode;
            accountant.ChequeDateSM = chequeDate ?? DateTime.Now;
            accountant.NewClearanceDateSM = chequeDate ?? DateTime.Now;
            accountant.TransactionDateSM = receiveddate ?? DateTime.Now;
            //DateTime cheqdate = accountant.ChequeDateReceipt;
            //string DateOnCheque = cheqdate.ToString("yyyy-MM-dd");
            SqlDataReader dr;
            dr = await objbal.ViewStudentFeeChequeAsyncSM(accountant);
            while (dr.Read())
            {
                accountant.TransactionCodeSM = dr["RECEIPT NO."].ToString();
                accountant.StudentNameSM = dr["STUDENT NAME"].ToString();
                accountant.TransactionDateSM = Convert.ToDateTime(dr["RECEIVED DATE"]);
                accountant.ChequeNumberSM = dr["CHEQUE NO."].ToString();
                accountant.AmountSM = Convert.ToInt64(dr["AMOUNT"]);
                accountant.DrawnOnSM = dr["DRAWN ON"].ToString();
                accountant.BatchSM = dr["BATCH NAME"].ToString();
                accountant.CourseSM = dr["COURSE CODE"].ToString();
                accountant.PaymentModeSM = "Cheque";
                accountant.AddmissionTypeSM = dr["ADMISSION TYPE"].ToString();
            }
            dr.Close();

            return PartialView(accountant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ShowStudentFeeChequeAsyncSM(Accountant objAcc)
        {
            await objbal.UpdateFeeTransactionChequeAsyncSM(objAcc);
            return RedirectToAction("ListAllChequesAsyncSM");
        }

        /// <summary>
        /// It displays list of cheque given by client to vendor through expense or purchase 
        /// </summary>
        /// <returns></returns>


        [HttpGet]
        public async Task<ActionResult> _ListAllChequeExpenseAsyncSM(string status = "All")
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            } else
            {
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
         {
                 new BreadcrumbItem { Name = "Dashboard", Url =Url.Action("AccountantDashboardAsyncSGS", "Accountant")  },
                 new BreadcrumbItem { Name = "Cheque Details",  }

         };

                ViewBag.Breadcrumbs = breadcrumbs;
                objprop.Prefix = Session["Prefix"].ToString();
                objprop.BranchCode = Session["BranchCode"].ToString();
                DataSet ds = await objbal.ListAllChequeExpenseAsyncSM(objprop);
                List<Accountant> lstData1 = new List<Accountant>();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string chequeStatus = ds.Tables[0].Rows[i]["STATUS"].ToString();

                    if (status == "All" || chequeStatus.Equals(status, StringComparison.OrdinalIgnoreCase))
                    {
                        Accountant objAcc = new Accountant
                        {
                            TransactionCodeSM = ds.Tables[0].Rows[i]["RECEIPT NO."].ToString(),
                            VoucherCodeSM = ds.Tables[0].Rows[i]["Voucher Code"].ToString(),
                            ChequeNumberSM = ds.Tables[0].Rows[i]["CHEQUE NO."].ToString(),
                            VendorNameSM = ds.Tables[0].Rows[i]["VENDOR NAME"].ToString(),
                            BankNameSM = ds.Tables[0].Rows[i]["BANK NAME"].ToString(),
                            AccountNumberSM = Convert.ToInt64(ds.Tables[0].Rows[i]["ACCOUNT NUMBER"].ToString()),
                            AmountSM = Convert.ToInt64(ds.Tables[0].Rows[i]["Amount"].ToString())
                        };

                        // Check and parse the "HANDOVERED DATE"
                        DateTime transactionDate;
                        if (DateTime.TryParse(ds.Tables[0].Rows[i]["HANDOVERED DATE"].ToString(), out transactionDate))
                        {
                            objAcc.TransactionDateSM = transactionDate;
                        } else
                        {
                            // Handle the case where the date is invalid or not provided
                            objAcc.TransactionDateSM = DateTime.MinValue; // or some default value
                        }

                        // Check and parse the "CHEQUE DATE"
                        DateTime chequeDate;
                        if (DateTime.TryParse(ds.Tables[0].Rows[i]["CHEQUE DATE"].ToString(), out chequeDate))
                        {
                            objAcc.ChequeDateSM = chequeDate;
                        } else
                        {
                            // Handle the case where the date is invalid or not provided
                            objAcc.ChequeDateSM = DateTime.MinValue; // or some default value
                        }

                        // Check and parse the "CHEQUE CLEARANCE DATE" if it's not DBNull or empty
                        if (ds.Tables[0].Rows[i]["CHEQUE CLEARANCE DATE"] != DBNull.Value &&
                            !string.IsNullOrEmpty(ds.Tables[0].Rows[i]["CHEQUE CLEARANCE DATE"].ToString()))
                        {
                            DateTime chequeClearanceDate;
                            if (DateTime.TryParse(ds.Tables[0].Rows[i]["CHEQUE CLEARANCE DATE"].ToString(), out chequeClearanceDate))
                            {
                                objAcc.ChequeClearanceDateSM = chequeClearanceDate;
                            }
                        }

                        objAcc.Status = chequeStatus;
                        lstData1.Add(objAcc);
                    }
                }

                Accountant obj = new Accountant { lstChequeExpense = lstData1 };

                return PartialView("_ListAllChequeExpenseAsyncSM", obj);
            }
        }

        /// <summary>
        /// This is used to clear cheque date given to vendors
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="amount"></param>
        /// <param name="chequeno"></param>
        /// <param name="chequedate"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<ActionResult> UpdateExpenseChequeDateAsyncSM(string id, string name, long amount, string chequeno, DateTime chequedate, string bankName, long accNumber, DateTime givendate, string vouchercode)
        {
            Accountant model = new Accountant
            {
                TransactionCodeSM = id,
                VendorNameSM = name,
                AmountSM = amount,
                ChequeNumberSM = chequeno,
                ChequeDateSM = chequedate,
                NewClearanceDateSM = chequedate,
                BankNameSM = bankName,
                AccountNumberSM = accNumber,
                TransactionDateSM = givendate,
                VoucherCodeSM = vouchercode
            };
            return await Task.Run(() => PartialView("UpdateExpenseChequeDateAsyncSM", model));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateExpenseChequeDateAsyncSM(Accountant objAcc)
        {
            await objbal.UpdateFeeTransactionChequeAsyncSM(objAcc);
            return RedirectToAction("ListAllChequesAsyncSM");
        }


        public async Task<ActionResult> StudentReceiptAsyncSM(string transactionCode, float amount, string candidatecode)
        {
            string branchCode = Session["BranchCode"].ToString();
            Accountant accountant = new Accountant
            {
                TransactionCodeSM = transactionCode

            };


            List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
         {

                 new BreadcrumbItem { Name = "Home", Url =Url.Action("ListAllChequesAsyncSM", "Accountant")  },

         };

            ViewBag.Breadcrumbs = breadcrumbs;


            SqlDataReader dr = await objbal.ViewStudentFeeChequeAsyncSM(accountant);

            if (dr.Read())
            {
                // Declare a different name for the parsed amount to avoid the conflict
                Accountant model = new Accountant
                {
                    ReciptCode = dr["RECEIPT NO."]?.ToString(),
                    Name = dr["STUDENT NAME"]?.ToString(),
                    DrawnOn = dr["DRAWN ON"]?.ToString(),
                    ReceiptDateAD = DateTime.Today.ToString("dd-MM-yyyy"),
                    TransactionID_checqueNumber = dr["CHEQUE NO."]?.ToString(),
                    ChequeDate = DateTime.TryParse(dr["CHEQUE DATE"]?.ToString(), out DateTime chequeDate) ? chequeDate : DateTime.MinValue,
                    ChequeClearanceDate = dr["CHEQUE CLEARANCE DATE"] != DBNull.Value && DateTime.TryParse(dr["CHEQUE CLEARANCE DATE"]?.ToString(), out DateTime chequeClearanceDate) ? chequeClearanceDate : DateTime.MinValue,
                    StaffName = dr["STAFF NAME"]?.ToString(),
                    CourseName = dr["COURSE CODE"]?.ToString(),
                    Batch = dr["BATCH NAME"]?.ToString(),
                    ChequeBankName = dr["BANK NAME"]?.ToString(),
                    AdmissionType = dr["ADMISSION TYPE"]?.ToString(),
                    BranchAddress = dr["ADDRESS"]?.ToString(),
                    PaymentModeId = "Cheque",
                    TotalFees = decimal.TryParse(dr["TOTAL FEE"]?.ToString(), out decimal totalFee) ? totalFee : 0,
                    CandidateCode = candidatecode,
                    BranchCode = branchCode,
                    Logo = dr["LOGO"]?.ToString(),
                };

                // Parse amount and calculate outstanding amount using a different variable name
                if (decimal.TryParse(dr["AMOUNT"]?.ToString(), out decimal parsedAmount))
                {
                    model.Amount = (long)parsedAmount;
                    model.OutstandingAmount = model.TotalFees - parsedAmount;
                } else
                {
                    model.Amount = 0;
                    model.OutstandingAmount = model.TotalFees;
                }


                DataSet dsStudFee = await objbal.GetFeesDataForSingleStudent(model.BranchCode, model.CandidateCode);

                foreach (DataRow row in dsStudFee.Tables[0].Rows)
                {
                    try
                    {
                        objac.BatchStartDate = DateTime.Parse(row["StartDate"].ToString());
                        int givenNoOfInstallment = Convert.ToInt32(row["NoofInstallment"]);
                        double totalFees = Convert.ToDouble(row["TotalFee"]);
                        double totalPaid = Convert.ToDouble(row["TotalTransactionAmount"]);
                        objac.Duration = Convert.ToInt32(row["InstallmentDuration"]);

                        model.OutstandingAmount = (decimal)totalFees - (decimal)totalPaid;

                        // Calculate unpaid installments
                        List<Accountant> upcomingInstallments = GetUnpaidInstallments(totalFees, totalPaid, givenNoOfInstallment);

                        // Initialize flags and variables for the next installment
                        bool foundNextInstallment = false;
                        double remainingTotalPaid = totalPaid;
                        double TotalCompleted = 0;


                        // Loop through each installment to find the next due one
                        foreach (var installment in upcomingInstallments)
                        {
                            if (remainingTotalPaid <= installment.TotalCompletedAmount)
                            {
                                // The first unpaid installment where remainingTotalPaid is less than TotalCompletedAmount
                                double nextInstallmentAmount = installment.TotalCompletedAmount - remainingTotalPaid;

                                // Set the next installment date and amount
                                model.NextInstallmentDates = installment.InstallmentDate.ToString("dd-MM-yyyy");
                                model.NextInstallmentAmount = Math.Round(nextInstallmentAmount, 2);
                                model.InstallmentNo = installment.InstallmentNo;

                                TotalCompleted = installment.TotalCompletedAmount;
                                // Mark that we've found the next installment
                                foundNextInstallment = true;


                                //if (installment.InstallmentDate < DateTime.Now)
                                //{
                                //    continue;
                                //}

                                if (installment.InstallmentDate > DateTime.Now && installment.TotalCompletedAmount <= remainingTotalPaid)
                                {
                                    continue;
                                } else if (installment.InstallmentDate < DateTime.Now)
                                {
                                    continue;
                                } else
                                {
                                    break;
                                }

                            }
                        }

                        List<Accountant> InstallmentNo = CalculateInstallments(totalFees, givenNoOfInstallment);

                        foreach (var installment in InstallmentNo)
                        {
                            if (remainingTotalPaid >= installment.TotalCompletedAmount)
                            {
                                model.InstallmentNo = installment.InstallmentNo;
                            }

                            if (remainingTotalPaid < installment.TotalCompletedAmount)
                            {
                                model.InstallmentNo = installment.InstallmentNo;
                                break;
                            }

                        }

                        // If totalPaid >= totalFees, set the fee as completed
                        if (totalPaid >= totalFees || !foundNextInstallment)
                        {
                            model.NextInstallmentDates = "Fee Completed";
                            model.NextInstallmentAmount = 0;
                            model.InstallmentNo = model.InstallmentNo + 1;
                        }



                    } catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error processing row: " + ex.Message);
                        continue;
                    }
                }



                return await Task.Run(() => View(model));
            }
            return View();
        }

        #endregion

    }
}