using GSTEducationERPLibrary.Counsellor;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static GSTEducationERPLibrary.Counsellor.Counsellor;
using System.Configuration;
using GSTEducationERPLibrary.Bind;
using GSTEducationERPLibrary.Coordinator;
using ClosedXML.Excel;
using System.Runtime.Remoting;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Management;
using System.Net;
using System.Web.Services.Description;
using System.Web.WebPages;

namespace GSTEducationERP.Controllers
{
    public class CounsellorController : Controller
    {
        // GET: Counsellor
        private readonly BALCounsellor objbal;

        public CounsellorController()
        {
            objbal = new BALCounsellor();  //Achieves Abstraction
        }

        public class BreadcrumbItem
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }

        //-------------------HIMANGI PAWAR(DASHBOAERD)----------------//
        Counsellor objCounsellor = new Counsellor();
        //-----------------------------------------------------------//

        //--------------------USED ONLY MAILSENDING------------------//
        string TentativeDate;
        string MailType;
        //-----------------------------------------------------------//

        //Rutuja Reports Start

        [HttpGet]
        public async Task<ActionResult> BranchListForMonnthlyEnquiryRU(DateTime? startDate, DateTime? endDate, string enquiryfor)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }

                Counsellor objnew = new Counsellor();
                objnew.BranchCode = Session["BranchCode"].ToString();
                
                objnew.StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue;
                objnew.EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue;

                // Execute the SQL query to fetch the list of enquiries
                DataSet ds = await objbal.BranchListForEnquiryStudentsRU(objnew);

                // Create a list to hold enquiry data
                List<Counsellor> lstforBranchenquiry = new List<Counsellor>();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Counsellor obju = new Counsellor();
                    obju.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    obju.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    obju.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                    obju.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
                    obju.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                    obju.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                    obju.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                    obju.MonthYear = ds.Tables[0].Rows[i]["MonthYear"].ToString();
                    obju.Qualification = ds.Tables[0].Rows[i]["Qualification"].ToString();

                    lstforBranchenquiry.Add(obju);
                }

                // Filter the list based on enquiryfor if provided
                List<Counsellor> filteredlist = !string.IsNullOrEmpty(enquiryfor)
                    ? lstforBranchenquiry.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList()
                    : lstforBranchenquiry;

                objnew.lstforBranchenquiry = filteredlist;

                return await Task.Run(() => PartialView("BranchListForMonnthlyEnquiryRU", objnew));
            }
            catch (Exception ex)
            {

                // Set an error message to TempData or ViewBag to display it in the view
                TempData["ErrorMessage"] = "An error occurred while processing your request.";

                // Optionally, redirect to an error page or return an appropriate response
                return RedirectToAction("Error", "Home"); // Example of redirecting to an error page
            }
        }


        /// <summary>
        /// method to get AdmittedStudentsList
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> BranchListForAdmittedStudentsRU(DateTime? startDate, DateTime? endDate, string enquiryfor)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    Counsellor objnew = new Counsellor();
                    objnew.BranchCode = Session["BranchCode"].ToString();

                    
                    objnew.StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue;
                    objnew.EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue;

                    DataSet ds = await objbal.BranchListForAdmittedStudentsRU(objnew);

                    List<Counsellor> lstforBranchadmsn = new List<Counsellor>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Counsellor obju = new Counsellor();
                        obju.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                        obju.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                        obju.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                        obju.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
                        obju.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                        obju.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                        obju.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                        obju.MonthYear = ds.Tables[0].Rows[i]["MonthYear"].ToString();
                        obju.Qualification = ds.Tables[0].Rows[i]["Qualification"].ToString();

                        lstforBranchadmsn.Add(obju);
                    }

                    List<Counsellor> filteredlist = new List<Counsellor>();
                    if (!string.IsNullOrEmpty(enquiryfor))
                    {
                        filteredlist = lstforBranchadmsn.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                        objnew.lstforBranchadmsn = filteredlist;
                    }
                    else
                    {
                        objnew.lstforBranchadmsn = lstforBranchadmsn;
                    }

                    return await Task.Run(() => PartialView("BranchListForAdmittedStudentsRU", objnew));
                }
            }
            catch (Exception ex)
            {

                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// method to get DropoutStudentsList
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<ActionResult> BranchListForMonthlyDropoutStudentsRU(DateTime? startDate, DateTime? endDate, string enquiryfor)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    Counsellor objnew = new Counsellor();
                    objnew.BranchCode = Session["BranchCode"].ToString();
                   
                    objnew.StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue;
                    objnew.EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue;

                    DataSet ds = await objbal.BranchListForDropoutStudentsRU(objnew);

                    List<Counsellor> lstforBranchdropout = new List<Counsellor>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Counsellor obju = new Counsellor();
                        obju.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                        obju.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                        obju.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                        obju.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
                        obju.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                        obju.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                        obju.AdmmissionDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["AdmmissionDate"].ToString());
                        obju.MonthYear = ds.Tables[0].Rows[i]["MonthYear"].ToString();
                        obju.FollowUpNote = ds.Tables[0].Rows[i]["FollowUpNote"].ToString();

                        lstforBranchdropout.Add(obju);
                    }

                    List<Counsellor> filteredlist = new List<Counsellor>();
                    if (!string.IsNullOrEmpty(enquiryfor))
                    {
                        filteredlist = lstforBranchdropout.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                        objnew.lstforBranchdropout = filteredlist;
                    }
                    else
                    {
                        objnew.lstforBranchdropout = lstforBranchdropout;
                    }

                    return await Task.Run(() => PartialView("BranchListForMonthlyDropoutStudentsRU", objnew));
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// method to get PlacedStudentsList
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>

        public async Task<ActionResult> BranchListForPlacedStudentsRU(DateTime? startDate, DateTime? endDate, string enquiryfor)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    Counsellor objnew = new Counsellor();
                    objnew.BranchCode = Session["BranchCode"].ToString();
                    
                    objnew.StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue;
                    objnew.EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue;

                    DataSet ds = await objbal.BranchListForPlacedStudentsRU(objnew);

                    List<Counsellor> lstforBranchplaced = new List<Counsellor>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Counsellor obju = new Counsellor();
                        obju.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                        obju.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                        obju.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                        obju.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
                        obju.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                        obju.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                        obju.AdmmissionDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["AdmmissionDate"].ToString());
                        obju.MonthYear = ds.Tables[0].Rows[i]["MonthYear"].ToString();
                        obju.Qualification = ds.Tables[0].Rows[i]["Qualification"].ToString();

                        lstforBranchplaced.Add(obju);
                    }

                    List<Counsellor> filteredlist = new List<Counsellor>();
                    if (!string.IsNullOrEmpty(enquiryfor))
                    {
                        filteredlist = lstforBranchplaced.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                        objnew.lstforBranchplaced = filteredlist;
                    }
                    else
                    {
                        objnew.lstforBranchplaced = lstforBranchplaced;
                    }

                    return await Task.Run(() => PartialView("BranchListForPlacedStudentsRU", objnew));
                }
            }
            catch (Exception ex)
            {

                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
        }
        /// <summary>
        /// this method is used for getting list of lost candidates
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="enquiryfor"></param>
        /// <returns></returns>
        public async Task<ActionResult> BranchListForlostStudentRU(DateTime? startDate, DateTime? endDate, string enquiryfor)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    Counsellor objnew = new Counsellor();
                    objnew.BranchCode = Session["BranchCode"].ToString();
                  
                    objnew.StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue;
                    objnew.EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue;

                    DataSet ds = await objbal.BranchListForlostStudentsRU(objnew);

                    List<Counsellor> lstforBranchlost = new List<Counsellor>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Counsellor obju = new Counsellor();
                        obju.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                        obju.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                        obju.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                        obju.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
                        obju.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                        obju.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                        obju.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                        obju.MonthYear = ds.Tables[0].Rows[i]["MonthYear"].ToString();
                        obju.FollowUpNote = ds.Tables[0].Rows[i]["FollowUpNote"].ToString();

                        lstforBranchlost.Add(obju);
                    }

                    List<Counsellor> filteredlist = new List<Counsellor>();
                    if (!string.IsNullOrEmpty(enquiryfor))
                    {
                        filteredlist = lstforBranchlost.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                        objnew.lstforBranchlost = filteredlist;
                    }
                    else
                    {
                        objnew.lstforBranchlost = lstforBranchlost;
                    }

                    return await Task.Run(() => PartialView("BranchListForlostStudentRU", objnew));
                }
            }
            catch (Exception ex)
            {

                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// this method is used for getting hold candidates
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="enquiryfor"></param>
        /// <returns></returns>
        public async Task<ActionResult> BranchListForHOLDStudentRU(DateTime? startDate, DateTime? endDate, string enquiryfor)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    Counsellor objnew = new Counsellor();
                    objnew.BranchCode = Session["BranchCode"].ToString();
                  
                    objnew.StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue;
                    objnew.EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue;

                    DataSet ds = await objbal.BranchListForHoldStudentsRU(objnew);

                    List<Counsellor> lstforBranchhold = new List<Counsellor>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Counsellor obju = new Counsellor();
                        obju.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                        obju.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                        obju.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                        obju.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
                        obju.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                        obju.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                        obju.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                        obju.MonthYear = ds.Tables[0].Rows[i]["MonthYear"].ToString();
                        obju.FollowUpNote = ds.Tables[0].Rows[i]["FollowUpNote"].ToString();

                        lstforBranchhold.Add(obju);
                    }

                    List<Counsellor> filteredlist = new List<Counsellor>();
                    if (!string.IsNullOrEmpty(enquiryfor))
                    {
                        filteredlist = lstforBranchhold.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                        objnew.lstforBranchhold = filteredlist;
                    }
                    else
                    {
                        objnew.lstforBranchhold = lstforBranchhold;
                    }

                    return await Task.Run(() => PartialView("BranchListForHOLDStudentRU", objnew));
                }
            }
            catch (Exception ex)
            {

                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// MainViewGraph
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> MainGraphViewAsyncRU()
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    await AllReportingStaffBind();

                    Counsellor obj = new Counsellor();
                    List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                    {
                        new BreadcrumbItem { Name = "Counsellor Dashboard", Url =Url.Action("MainDashboardAsyncHP", "Counsellor") },
                        new BreadcrumbItem { Name = "Reports", Url = Url.Action("MainGraphViewAsyncRU", "Counsellor") }
                    };
                    ViewBag.Breadcrumbs = breadcrumbs;

                    return await Task.Run(() => View("MainGraphViewAsyncRU"));
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        /// <summary>
        /// Method to ListMainView
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> ListMainGraphViewAsyncRU()
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    await AllReportingStaffBind();

                    Counsellor ojj = new Counsellor();
                    List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                    {
                        new BreadcrumbItem { Name = "Counsellor Dashboard", Url = Url.Action("MainDashboardAsyncHP", "Counsellor") },
                        new BreadcrumbItem { Name = "Reports", Url = Url.Action("ListMainGraphViewAsyncRU", "Counsellor") }
                    };

                    ViewBag.Breadcrumbs = breadcrumbs;

                    return await Task.Run(() => View("ListMainGraphViewAsyncRU"));
                }
            }
            catch (Exception ex)    
            {
                throw (ex);
            }
        }

        /// <summary>
        /// method to  reporting StaffBind
        /// </summary>
        /// <returns></returns>
        public async Task AllReportingStaffBind()
        {
            try
            {
                Counsellor objnew = new Counsellor();
                objnew.StaffCode = Session["StaffCode"].ToString();
                DataSet ds1 = new DataSet();
                ds1 = await objbal.AllReporingStaffBind(objnew);
                List<SelectListItem> AllReportingStaff = new List<SelectListItem>();
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds1.Tables[0].Rows)
                    {
                        AllReportingStaff.Add(new SelectListItem
                        {
                            Text = dr["StaffName"].ToString(),
                            Value = dr["StaffCode"].ToString()
                        });
                    }
                }
                ViewBag.AllReportingStaffBind = AllReportingStaff;
            }
            catch (Exception ex)
            {
                // Re-throw the exception with a new stack trace
                throw ex;
            }
        }

        /// <summary>
        /// method graph for EnqVSAdmsn
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GraphForEnqVSAdmsnAsyncRU(string staffcode, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    Counsellor objnew = new Counsellor
                    {
                        StaffCode = Session["StaffCode"].ToString(),
                        StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue,
                        EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MinValue
                    };

                    if (!string.IsNullOrEmpty(staffcode))
                    {
                        objnew.StaffCode = staffcode;
                    }

                    // Execute the SQL query to fetch the counts of enquiries and admissions
                    DataSet ds = await objbal.GraphMonthlyCountOfEnqVSAdmsnAsyncRU(objnew);

                    // Create a list to hold data points
                    List<object> dataPoints = new List<object>();

                    // Loop through the retrieved dataset and construct data points
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int year = Convert.ToInt32(row["year"]);
                        string month = row["month_name"].ToString();
                        int leadCount = Convert.ToInt32(row["lead_count"]);
                        int enquiryCount = Convert.ToInt32(row["enquiry_count"]);
                        int admissionCount = Convert.ToInt32(row["admitted_count"]);

                        // Construct data point object with label, month, enquiry count, and admission count
                        dataPoints.Add(new { label = $"{month}-{year}", lead = leadCount, enquiry = enquiryCount, admission = admissionCount });
                    }

                    // Convert the data points to JSON string and pass it to the ViewBag
                    ViewBag.DataPoints = Newtonsoft.Json.JsonConvert.SerializeObject(dataPoints);
                    return PartialView("GraphForEnqVSAdmsnAsyncRU", objnew);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (using a logging framework like log4net, NLog, Serilog, etc.)
                // LogException(ex); // Example of logging

                // You can set an error message to ViewBag or TempData to display it in the view
                TempData["ErrorMessage"] = "An error occurred while processing your request.";

                // Optionally, redirect to an error page or return an appropriate response
                return RedirectToAction("Error", "Home"); // Example of redirecting to an error page
            }
        }


        /// <summary>
        /// metthis graph for Enquiry
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<ActionResult> GraphForEnquiryCountAsyncRU(string staffcode, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    Counsellor objnew = new Counsellor
                    {
                        StaffCode = Session["StaffCode"].ToString(),
                        StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue,
                        EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MinValue
                    };

                    if (!string.IsNullOrEmpty(staffcode))
                    {
                        objnew.StaffCode = staffcode;
                    }

                    // Execute the SQL query to fetch the counts of enquiry
                    DataSet ds = await objbal.GraphMonthlyEnquiryStudentsCountAsyncRU(objnew);

                    // Create a list to hold data points
                    List<object> DS3 = new List<object>();

                    // Loop through the retrieved dataset and construct data points
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int year = Convert.ToInt32(row["year"]);
                        string month = row["month_name"].ToString();
                        int enquiryCount = Convert.ToInt32(row["enquiry_count"]);

                        // Construct data point object with label, month, and enquiry count
                        DS3.Add(new { label = $"{month}-{year}", enquiry = enquiryCount });
                    }

                    // Convert the data points to JSON string and pass it to the ViewBag
                    ViewBag.DS3 = Newtonsoft.Json.JsonConvert.SerializeObject(DS3);

                    return PartialView("GraphForEnquiryCountAsyncRU", objnew); // Assuming you have a partial view named "GraphForEnquiryCountAsyncRU"
                }
            }
            catch (Exception ex)
            {
                // Log the exception (using a logging framework like log4net, NLog, Serilog, etc.)
                // LogException(ex); // Example of logging

                // You can set an error message to ViewBag or TempData to display it in the view
                TempData["ErrorMessage"] = "An error occurred while processing your request.";

                // Optionally, redirect to an error page or return an appropriate response
                return RedirectToAction("Error", "Home"); // Example of redirecting to an error page
            }
        }


        /// <summary>
        /// metthis graph for DropoutStudentCount
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<ActionResult> GraphForDropoutCountAsyncRU(string staffcode, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    Counsellor objnew = new Counsellor
                    {
                        StaffCode = Session["StaffCode"].ToString(),
                        StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue,
                        EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue
                    };

                    if (!string.IsNullOrEmpty(staffcode))
                    {
                        objnew.StaffCode = staffcode;
                    }

                    // Execute the SQL query to fetch the counts of dropout students
                    DataSet ds = await objbal.GraphMonthlyDropoutStudentsCountAsyncRU(objnew);

                    // Create a list to hold data points
                    List<object> DS1 = new List<object>();

                    // Loop through the retrieved dataset and construct data points
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int year = Convert.ToInt32(row["Year"]);
                        string month = row["month_name"].ToString();
                        int dropoutCount = Convert.ToInt32(row["Dropout_count"]);

                        // Construct data point object with label, month, and dropout count
                        DS1.Add(new { label = $"{month}-{year}", dropout = dropoutCount });
                    }

                    // Convert the data points to JSON string and pass it to the ViewBag
                    ViewBag.DS1 = Newtonsoft.Json.JsonConvert.SerializeObject(DS1);

                    return PartialView("GraphForDropoutCountAsyncRU", objnew); // Assuming you have a partial view named "GraphForDropoutCountAsyncRU"
                }
            }
            catch (Exception ex)
            {
                // Log the exception (using a logging framework like log4net, NLog, Serilog, etc.)
                // LogException(ex); // Example of logging

                // You can set an error message to ViewBag or TempData to display it in the view
                TempData["ErrorMessage"] = "An error occurred while processing your request.";

                // Optionally, redirect to an error page or return an appropriate response
                return RedirectToAction("Error", "Home"); // Example of redirecting to an error page
            }
        }


        /// <summary>
        /// metthis graph for Admitted Student  Count
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="Startdate"></param>
        /// <param name="Enddate"></param>
        /// <returns></returns>
        public async Task<ActionResult> GraphForAdmittedStudentCountAsyncRU(string staffcode, DateTime? Startdate, DateTime? Enddate)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    Counsellor objnew = new Counsellor
                    {
                        StaffCode = Session["StaffCode"].ToString(),
                        StartDate = Startdate.HasValue ? Startdate.Value.Date : DateTime.MinValue,
                        EndDate = Enddate.HasValue ? Enddate.Value.Date : DateTime.MaxValue
                    };

                    if (!string.IsNullOrEmpty(staffcode))
                    {
                        objnew.StaffCode = staffcode;
                    }

                    // Execute the SQL query to fetch the counts of admitted students
                    DataSet ds = await objbal.GraphMonthlyAdmittedStudentsCountAsyncRU(objnew);

                    // Create a list to hold data points
                    List<object> datapoint1 = new List<object>();

                    // Loop through the retrieved dataset and construct data points
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int year = Convert.ToInt32(row["year"]);
                        string month = row["month_name"].ToString();
                        int admittedCount = Convert.ToInt32(row["admitted_count"]);

                        // Construct data point object with label, month, and admitted count
                        datapoint1.Add(new { label = $"{month}-{year}", admitted = admittedCount });
                    }

                    // Convert the data points to JSON string and pass it to the ViewBag
                    ViewBag.datapoint2 = Newtonsoft.Json.JsonConvert.SerializeObject(datapoint1);

                    return PartialView("GraphForAdmittedStudentCountAsyncRU", objnew); // Assuming you have a partial view named "GraphForAdmittedStudentCountAsyncRU"
                }
            }
            catch (Exception ex)
            {
                // Log the exception (using a logging framework like log4net, NLog, Serilog, etc.)
                // LogException(ex); // Example of logging

                // You can set an error message to ViewBag or TempData to display it in the view
                TempData["ErrorMessage"] = "An error occurred while processing your request.";

                // Optionally, redirect to an error page or return an appropriate response
                return RedirectToAction("Error", "Home"); // Example of redirecting to an error page
            }
        }


        /// <summary>
        /// metthis graph for AdmsnVSPlacement
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<ActionResult> GraphForAdmittedVSPlacementAsyncRU(string staffcode, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    Counsellor objnew = new Counsellor
                    {
                        StaffCode = Session["StaffCode"].ToString(),
                        StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue,
                        EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue
                    };

                    if (!string.IsNullOrEmpty(staffcode))
                    {
                        objnew.StaffCode = staffcode;
                    }

                    // Execute the SQL query to fetch the counts of admitted students and placements
                    DataSet ds = await objbal.GraphMonthlyCountOfAdmsnVSPlacementAsyncRU(objnew);

                    // Create a list to hold data points
                    List<object> dataPoints = new List<object>();

                    // Loop through the retrieved dataset and construct data points
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int year = Convert.ToInt32(row["Year"]);
                        int admittedCount = Convert.ToInt32(row["Admitted_count"]);
                        int placementCount = Convert.ToInt32(row["Placement_count"]);

                        // Construct data point object with label, year, admitted count, and placement count
                        dataPoints.Add(new { label = $"{year}", admitted = admittedCount, placement = placementCount });
                    }

                    // Convert the data points to JSON string and pass it to the ViewBag
                    ViewBag.DataPoints6 = Newtonsoft.Json.JsonConvert.SerializeObject(dataPoints);

                    return PartialView("GraphForAdmittedVSPlacementAsyncRU", objnew);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (using a logging framework like log4net, NLog, Serilog, etc.)
                // LogException(ex); // Example of logging

                // Set an error message to TempData or ViewBag to display it in the view
                TempData["ErrorMessage"] = "An error occurred while processing your request.";

                // Optionally, redirect to an error page or return an appropriate response
                return RedirectToAction("Error", "Home"); // Example of redirecting to an error page
            }
        }

        // Branchwise
        /// <summary>
        /// method graph for EnqVSAdmsn
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>

        public async Task<ActionResult> GraphForEnqVSAdmsnBranchAsyncRU(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }

                Counsellor objnew = new Counsellor
                {
                    BranchCode = Session["BranchCode"].ToString(),
                    StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue,
                    EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue
                };

                // Execute the SQL query to fetch the counts of enquiries and admissions
                DataSet ds = await objbal.GraphMonthlyCountOfEnqVSAdmsnBranchAsyncRU(objnew);

                // Create a list to hold data points
                List<object> dataPoints = new List<object>();

                // Loop through the retrieved dataset and construct data points
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int year = Convert.ToInt32(row["year"]);
                    string month = row["month_name"].ToString();
                    int leadCount = Convert.ToInt32(row["lead_count"]);
                    int enquiryCount = Convert.ToInt32(row["enquiry_count"]);
                    int admissionCount = Convert.ToInt32(row["admitted_count"]);

                    // Construct data point object with label, month, enquiry count, and admission count
                    dataPoints.Add(new { label = $"{month}-{year}", lead = leadCount, enquiry = enquiryCount, admission = admissionCount });
                }

                // Convert the data points to JSON string and pass it to the ViewBag
                ViewBag.DataPoints8 = Newtonsoft.Json.JsonConvert.SerializeObject(dataPoints);
                return PartialView("GraphForEnqVSAdmsnBranchAsyncRU", objnew);
            }
            catch (Exception ex)
            {
                // Log the exception (using a logging framework like log4net, NLog, Serilog, etc.)
                // LogException(ex); // Example of logging

                // Set an error message to TempData or ViewBag to display it in the view
                TempData["ErrorMessage"] = "An error occurred while processing your request.";

                // Optionally, redirect to an error page or return an appropriate response
                return RedirectToAction("Error", "Home"); // Example of redirecting to an error page
            }
        }

        /// <summary>
        /// method graph for DropoutStudentCount
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>

        public async Task<ActionResult> GraphForDropoutCountBranchAsyncRU(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }

                Counsellor objnew = new Counsellor
                {
                    BranchCode = Session["BranchCode"].ToString(),
                    StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue,
                    EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue
                };

                // Execute the SQL query to fetch the counts of dropout students
                DataSet ds = await objbal.GraphMonthlyDropoutStudentsCountBranchAsyncRU(objnew);

                // Create a list to hold data points
                List<object> DS1 = new List<object>();

                // Loop through the retrieved dataset and construct data points
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int year = Convert.ToInt32(row["year"]);
                    string month = row["month_name"].ToString();
                    int dropoutCount = Convert.ToInt32(row["Dropout_count"]);

                    // Construct data point object with label, month, and dropout count
                    DS1.Add(new { label = $"{month}-{year}", dropout = dropoutCount });
                }

                // Convert the data points to JSON string and pass it to the ViewBag
                ViewBag.DS9 = Newtonsoft.Json.JsonConvert.SerializeObject(DS1);
                return PartialView("GraphForDropoutCountBranchAsyncRU", objnew); // Assuming you have a partial view named "GraphForDropoutCountBranchAsyncRU"
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework like log4net, NLog, Serilog, etc.)
                // LogException(ex); // Example placeholder for logging

                // Set an error message to TempData or ViewBag to display it in the view
                TempData["ErrorMessage"] = "An error occurred while processing your request.";

                // Optionally, redirect to an error page or return an appropriate response
                return RedirectToAction("Error", "Home"); // Example of redirecting to an error page
            }
        }

        /// <summary>
        /// method graph for Admitted Student Count
        /// </summary>
        /// <param name="Startdate"></param>
        /// <param name="Enddate"></param>
        /// <returns></returns>
        public async Task<ActionResult> GraphForAdmittedStudentCountBranchAsyncRU(DateTime? Startdate, DateTime? Enddate)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }

                Counsellor objnew = new Counsellor
                {
                    BranchCode = Session["BranchCode"].ToString(),
                    StartDate = Startdate.HasValue ? Startdate.Value.Date : DateTime.MinValue,
                    EndDate = Enddate.HasValue ? Enddate.Value.Date : DateTime.MaxValue
                };

                // Execute the SQL query to fetch the counts of admitted students
                DataSet ds = await objbal.GraphMonthlyAdmittedStudentsCountBranchAsyncRU(objnew);

                // Create a list to hold data points
                List<object> datapoint1 = new List<object>();

                // Loop through the retrieved dataset and construct data points
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int year = Convert.ToInt32(row["year"]);
                    string month = row["month_name"].ToString();
                    int admittedCount = Convert.ToInt32(row["admitted_count"]);

                    // Construct data point object with label, month, and admitted count
                    datapoint1.Add(new { label = $"{month}-{year}", admitted = admittedCount });
                }

                // Convert the data points to JSON string and pass it to the ViewBag
                ViewBag.datapoint20 = Newtonsoft.Json.JsonConvert.SerializeObject(datapoint1);

                return PartialView("GraphForAdmittedStudentCountBranchAsyncRU", objnew); // Assuming you have a partial view named "GraphForAdmittedStudentsAsyncRU"
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework like log4net, NLog, Serilog, etc.)
                // LogException(ex); // Placeholder for logging

                // Set an error message to TempData or ViewBag to display it in the view
                TempData["ErrorMessage"] = "An error occurred while processing your request.";

                // Optionally, redirect to an error page or return an appropriate response
                return RedirectToAction("Error", "Home"); // Example of redirecting to an error page
            }
        }

        /// <summary>
        /// method graph for EnquiryCount
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<ActionResult> GraphForEnquiryCountBranchAsyncRU(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }

                Counsellor objnew = new Counsellor
                {
                    BranchCode = Session["BranchCode"].ToString(),
                    StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue,
                    EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue
                };

                // Execute the SQL query to fetch the counts of enquiry
                DataSet ds = await objbal.GraphMonthlyEnquiryStudentsCountBranchAsyncRU(objnew);

                // Create a list to hold data points
                List<object> DS3 = new List<object>();

                // Loop through the retrieved dataset and construct data points
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int year = Convert.ToInt32(row["year"]);
                    string month = row["month_name"].ToString();
                    int enquiryCount = Convert.ToInt32(row["enquiry_count"]);

                    // Construct data point object with label, month, and enquiry count
                    DS3.Add(new { label = $"{month}-{year}", enquiry = enquiryCount });
                }

                // Convert the data points to JSON string and pass it to the ViewBag
                ViewBag.DS11 = Newtonsoft.Json.JsonConvert.SerializeObject(DS3);

                return PartialView("GraphForEnquiryCountBranchAsyncRU", objnew); // Assuming you have a partial view named "GraphForEnquiryCountAsyncRU"
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework like log4net, NLog, Serilog, etc.)
                // LogException(ex); // Placeholder for logging

                // Set an error message to TempData or ViewBag to display it in the view
                TempData["ErrorMessage"] = "An error occurred while processing your request.";

                // Optionally, redirect to an error page or return an appropriate response
                return RedirectToAction("Error", "Home"); // Example of redirecting to an error page
            }
        }

        /// <summary>
        /// method graph for AdmsnVSPlacement
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<ActionResult> GraphForAdmittedVSPlacementBranchAsyncRU(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }

                Counsellor objnew = new Counsellor
                {
                    BranchCode = Session["BranchCode"].ToString(),
                    StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue,
                    EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue
                };

                // Execute the SQL query to fetch the counts of admitted students and placements
                DataSet ds = await objbal.GraphMonthlyCountOfAdmsnVSPlacementBranchAsyncRU(objnew);

                // Create a list to hold data points
                List<object> dataPoints = new List<object>();

                // Loop through the retrieved dataset and construct data points
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int year = Convert.ToInt32(row["year"]);
                    int admittedCount = Convert.ToInt32(row["Admitted_count"]);
                    int placementCount = Convert.ToInt32(row["Placement_count"]);

                    // Construct data point object with label, admitted count, and placement count
                    dataPoints.Add(new { label = $"{year}", admitted = admittedCount, placement = placementCount });
                }

                // Convert the data points to JSON string and pass it to the ViewBag
                ViewBag.DataPoints12 = Newtonsoft.Json.JsonConvert.SerializeObject(dataPoints);

                return PartialView("GraphForAdmittedVSPlacementBranchAsyncRU", objnew);
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework like log4net, NLog, Serilog, etc.)
                // LogException(ex); // Placeholder for logging

                // Set an error message to TempData or ViewBag to display it in the view
                TempData["ErrorMessage"] = "An error occurred while processing your request.";

                // Optionally, redirect to an error page or return an appropriate response
                return RedirectToAction("Error", "Home"); // Example of redirecting to an error page
            }
        }
/// <summary>
/// this method is used to get the data for report of montly sourse lost count
/// </summary>
/// <param name="startDate"></param>
/// <param name="endDate"></param>
/// <returns></returns>
public async Task<ActionResult> PieChartForSourseWiseLostTrainAsyncVP(DateTime? startDate, DateTime? endDate)
{
    try
    {
        if (Session["StaffCode"] == null)
        {
            return RedirectToAction("Login", "Account");

        }
        else
        {
            objCounsellor.BranchCode = Session["BranchCode"].ToString();

            // Check if a date range is provided
            DateTime StartDate, EndDate;
            if (startDate.HasValue && endDate.HasValue)
            {
                StartDate = startDate.Value;
                EndDate = endDate.Value;
            }
            else
            {
                // Use default dates if no date range is provided
                StartDate = DateTime.Now.AddYears(-1);
                EndDate = DateTime.Now;
            }
            objCounsellor.StartDate = StartDate;
            objCounsellor.EndDate = EndDate;



            DataSet dsSourceWiseCount = await objbal.PieChartForSourseWiseLostTrainAsyncVP(objCounsellor);
            List<object> SourceWiseLeadCount = new List<object>();

            if (dsSourceWiseCount != null && dsSourceWiseCount.Tables.Count > 0)
            {
                DataTable dt = dsSourceWiseCount.Tables[0];

                // Ensure the column exists


                if (dt.Columns.Contains("SourceName") &&
                    dt.Columns.Contains("EnquiryCount"))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string SourceName = row["SourceName"] == DBNull.Value ? string.Empty : (row["SourceName"].ToString());
                        int EnquiryCount = row["EnquiryCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["EnquiryCount"]);
                        SourceWiseLeadCount.Add(new { Label = SourceName, enquirycount = EnquiryCount });
                    }
                }
            }

            ViewBag.SourceWiseCount = Newtonsoft.Json.JsonConvert.SerializeObject(SourceWiseLeadCount);
            return PartialView("_PieChartForSourseWiseLostTrainAsyncVP");
        }
    }
    catch (Exception ex)
    {
        // Log the exception (consider using a logging framework like log4net, NLog, Serilog, etc.)
        // LogException(ex); // Placeholder for logging

        // Set an error message to TempData or ViewBag to display it in the view
        TempData["ErrorMessage"] = "An error occurred while processing your request.";

        // Optionally, redirect to an error page or return an appropriate response
        return RedirectToAction("Error", "Home"); // Example of redirecting to an error page
    }
}



        [HttpGet]
        public async Task<ActionResult> ListForMonthlyEnquiryVSMonthlyAdmissionssRU(string staffcode, DateTime ? startDate, DateTime ? endDate)
        {

            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {

                Counsellor objnew = new Counsellor();
                objnew.StaffCode = Session["StaffCode"].ToString();
                objnew.StaffCode = staffcode;
                objnew.StartDate = startDate.Value.Date;
                objnew.EndDate = endDate.Value.Date;
                DataSet ds = await objbal.ListForMonthlyEnquiryVSMonthlyAdmissionssRU(objnew);
                Counsellor objC = new Counsellor();
                List<Counsellor> lstforenqVSAdmsn = new List<Counsellor>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Counsellor obju = new Counsellor();

                    obju.AdmissionCount = Convert.ToInt32(ds.Tables[0].Rows[i]["admitted_count"].ToString());
                    obju.EnquiryCount = Convert.ToInt32(ds.Tables[0].Rows[i]["enquiry_count"]);
                    obju.CandidateCode = ds.Tables[0].Rows[i]["candidatecode"].ToString();
                    obju.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                    obju.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["enquirydate"].ToString());
                    obju.EnquiryAddedStaffCode = ds.Tables[0].Rows[i]["EnquiryAddedStaffCode"].ToString();
                    obju.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    obju.AdmmissionDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["AdmmissionDate"].ToString());                  
                    obju.Year = Convert.ToInt32(ds.Tables[0].Rows[i]["EnquiryYear"].ToString());
                    obju.MonthYear = ds.Tables[0].Rows[i]["enquiry_month"].ToString();
                    obju.Year = Convert.ToInt32(ds.Tables[0].Rows[i]["admission_year"].ToString());
                    obju.MonthYear = ds.Tables[0].Rows[i]["admission_month"].ToString();
                    lstforenqVSAdmsn.Add(obju);
                }
                objC.lstforenqVSAdmsn = lstforenqVSAdmsn;
                return await Task.Run(() => PartialView("ListForMonthlyEnquiryVSMonthlyAdmissionssRU", objC));
            }
        }

        /// <summary>
        /// method to get EnquiryStudentsList
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns> 
        public async Task<ActionResult> ListForMonnthlyEnquiryRU(string staffcode, DateTime? startDate, DateTime? endDate, string enquiryfor)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }

                Counsellor objnew = new Counsellor();
               objnew.StaffCode = Session["StaffCode"].ToString();
                if(staffcode != null)
                {
                    objnew.StaffCode = staffcode;
                }
                objnew.StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue;
                objnew.EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue;
               

                // Execute the SQL query to fetch the list of enquiries
                DataSet ds = await objbal.ListForMonthlyEnquiryRU(objnew);

                // Create a list to hold enquiry data
                List<Counsellor> lstforEnquiry = new List<Counsellor>();

                // Populate the list from the DataSet
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Counsellor obju = new Counsellor
                    {
                        SerialNo = Convert.ToInt32(row["SerialNo"].ToString()),
                        CandidateCode = row["CandidateCode"].ToString(),
                        FullName = row["FullName"].ToString(),
                        EmailId = row["EmailId"].ToString(),
                        ContactNumber = row["ContactNumber"].ToString(),
                        EnquiryFor = row["EnquiryFor"].ToString(),
                        EnquiryDate = Convert.ToDateTime(row["EnquiryDate"].ToString()),
                        MonthYear = row["MonthYear"].ToString(),
                        Qualification = row["Qualification"].ToString()
                    };

                    lstforEnquiry.Add(obju);
                }

                // Filter the list based on enquiryfor if provided
                List<Counsellor> filteredlist = !string.IsNullOrEmpty(enquiryfor)
                    ? lstforEnquiry.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList()
                    : lstforEnquiry;

                objnew.lstforEnquiry = filteredlist;

                return await Task.Run(() => PartialView("ListForMonnthlyEnquiryRU", objnew));
            }
            catch (Exception ex)
            {               

                // Set an error message to TempData or ViewBag to display it in the view
                TempData["ErrorMessage"] = "An error occurred while processing your request.";

                // Optionally, redirect to an error page or return an appropriate response
                return RedirectToAction("Error", "Home"); // Example of redirecting to an error page
            }
        }


        /// <summary>
        /// method to get AdmittedStudentsList
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ListForAdmittedStudentsRU(string staffcode, DateTime? startDate, DateTime? endDate, string enquiryfor)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    Counsellor objnew = new Counsellor();
                    objnew.StaffCode = Session["StaffCode"].ToString();

                    objnew.StaffCode = staffcode;
                    objnew.StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue;
                    objnew.EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue;

                    DataSet ds = await objbal.ListForAdmittedStudentsRU(objnew);

                    List<Counsellor> lstforAdmittedStudents = new List<Counsellor>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Counsellor obju = new Counsellor();
                        obju.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                        obju.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                        obju.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                        obju.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
                        obju.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                        obju.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                        obju.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                        obju.MonthYear = ds.Tables[0].Rows[i]["MonthYear"].ToString();
                        obju.Qualification = ds.Tables[0].Rows[i]["Qualification"].ToString();

                        lstforAdmittedStudents.Add(obju);
                    }

                    List<Counsellor> filteredlist = new List<Counsellor>();
                    if (!string.IsNullOrEmpty(enquiryfor))
                    {
                        filteredlist = lstforAdmittedStudents.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                        objnew.lstforAdmittedStudents = filteredlist;
                    }
                    else
                    {
                        objnew.lstforAdmittedStudents = lstforAdmittedStudents;
                    }

                    return await Task.Run(() => PartialView("ListForAdmittedStudentsRU", objnew));
                }
            }
            catch (Exception ex)
            {
               
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// method to get DropoutStudentsList
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<ActionResult> ListForMonthlyDropoutStudentsRU(string staffcode, DateTime? startDate, DateTime? endDate, string enquiryfor)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    Counsellor objnew = new Counsellor();
                    objnew.StaffCode = Session["StaffCode"].ToString();
                    objnew.StaffCode = staffcode;
                    objnew.StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue;
                    objnew.EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue;

                    DataSet ds = await objbal.ListForMonthlyDropoutStudentsRU(objnew);

                    List<Counsellor> lstforDropoutStudents = new List<Counsellor>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Counsellor obju = new Counsellor();
                        obju.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                        obju.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                        obju.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                        obju.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
                        obju.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                        obju.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                        obju.AdmmissionDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["AdmmissionDate"].ToString());
                        obju.MonthYear = ds.Tables[0].Rows[i]["MonthYear"].ToString();
                        obju.FollowUpNote = ds.Tables[0].Rows[i]["FollowUpNote"].ToString();

                        lstforDropoutStudents.Add(obju);
                    }

                    List<Counsellor> filteredlist = new List<Counsellor>();
                    if (!string.IsNullOrEmpty(enquiryfor))
                    {
                        filteredlist = lstforDropoutStudents.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                        objnew.lstforDropoutStudents = filteredlist;
                    }
                    else
                    {
                        objnew.lstforDropoutStudents = lstforDropoutStudents;
                    }

                    return await Task.Run(() => PartialView("ListForMonthlyDropoutStudentsRU", objnew));
                }
            }
            catch (Exception ex)
            {
                
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// method to get PlacedStudentsList
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>

        public async Task<ActionResult> ListForPlacedStudentsRU(string staffcode, DateTime? startDate, DateTime? endDate, string enquiryfor)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    Counsellor objnew = new Counsellor();
                    objnew.StaffCode = Session["StaffCode"].ToString();
                    objnew.StaffCode = staffcode;
                    objnew.StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue;
                    objnew.EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue;

                    DataSet ds = await objbal.ListForPlacedStudentsRU(objnew);

                    List<Counsellor> lstforPlacedStudents = new List<Counsellor>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Counsellor obju = new Counsellor();
                        obju.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                        obju.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                        obju.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                        obju.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
                        obju.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                        obju.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                        obju.AdmmissionDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["AdmmissionDate"].ToString());
                        obju.MonthYear = ds.Tables[0].Rows[i]["MonthYear"].ToString();
                        obju.Qualification = ds.Tables[0].Rows[i]["Qualification"].ToString();

                        lstforPlacedStudents.Add(obju);
                    }

                    List<Counsellor> filteredlist = new List<Counsellor>();
                    if (!string.IsNullOrEmpty(enquiryfor))
                    {
                        filteredlist = lstforPlacedStudents.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                        objnew.lstforPlacedStudents = filteredlist;
                    }
                    else
                    {
                        objnew.lstforPlacedStudents = lstforPlacedStudents;
                    }

                    return await Task.Run(() => PartialView("ListForPlacedStudentsRU", objnew));
                }
            }
            catch (Exception ex)
            {
               
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
        }
        /// <summary>
        /// this method is used for getting list of lost candidates
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="enquiryfor"></param>
        /// <returns></returns>
        public async Task<ActionResult> ListForlostStudentRU(string staffcode, DateTime? startDate, DateTime? endDate, string enquiryfor)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    Counsellor objnew = new Counsellor();
                    objnew.StaffCode = Session["StaffCode"].ToString();
                    objnew.StaffCode = staffcode;
                    objnew.StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue;
                    objnew.EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue;

                    DataSet ds = await objbal.ListForlostStudentsRU(objnew);

                    List<Counsellor> lstforlostcandidate = new List<Counsellor>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Counsellor obju = new Counsellor();
                        obju.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                        obju.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                        obju.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                        obju.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
                        obju.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                        obju.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                        obju.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                        obju.MonthYear = ds.Tables[0].Rows[i]["MonthYear"].ToString();
                        obju.FollowUpNote = ds.Tables[0].Rows[i]["FollowUpNote"].ToString();

                        lstforlostcandidate.Add(obju);
                    }

                    List<Counsellor> filteredlist = new List<Counsellor>();
                    if (!string.IsNullOrEmpty(enquiryfor))
                    {
                        filteredlist = lstforlostcandidate.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                        objnew.lstforlostcandidate = filteredlist;
                    }
                    else
                    {
                        objnew.lstforlostcandidate = lstforlostcandidate;
                    }

                    return await Task.Run(() => PartialView("ListForlostStudentRU", objnew));
                }
            }
            catch (Exception ex)
            {
               
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// this method is used for getting hold candidates
        /// </summary>
        /// <param name="staffcode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="enquiryfor"></param>
        /// <returns></returns>
        public async Task<ActionResult> ListForHOLDStudentRU(string staffcode, DateTime? startDate, DateTime? endDate, string enquiryfor)
        {
            try
            {
                if (Session["StaffCode"] == null)
                {
                    return await Task.Run(() => RedirectToAction("Login", "Account"));
                }
                else
                {
                    Counsellor objnew = new Counsellor();
                    objnew.StaffCode = Session["StaffCode"].ToString();
                    objnew.StaffCode = staffcode;
                    objnew.StartDate = startDate.HasValue ? startDate.Value.Date : DateTime.MinValue;
                    objnew.EndDate = endDate.HasValue ? endDate.Value.Date : DateTime.MaxValue;

                    DataSet ds = await objbal.ListForHoldStudentsRU(objnew);

                    List<Counsellor> lstforholdcandidate = new List<Counsellor>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Counsellor obju = new Counsellor();
                        obju.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                        obju.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                        obju.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                        obju.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
                        obju.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                        obju.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                        obju.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                        obju.MonthYear = ds.Tables[0].Rows[i]["MonthYear"].ToString();
                        obju.FollowUpNote = ds.Tables[0].Rows[i]["FollowUpNote"].ToString();

                        lstforholdcandidate.Add(obju);
                    }

                    List<Counsellor> filteredlist = new List<Counsellor>();
                    if (!string.IsNullOrEmpty(enquiryfor))
                    {
                        filteredlist = lstforholdcandidate.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                        objnew.lstforholdcandidate = filteredlist;
                    }
                    else
                    {
                        objnew.lstforholdcandidate = lstforholdcandidate;
                    }

                    return await Task.Run(() => PartialView("ListForHOLDStudentRU", objnew));
                }
            }
            catch (Exception ex)
            {
               
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
        }






        //*******************************************Rutuja Reports End*******************************************************************

        //******************************************Himangi Dashboard Start*************************************************************



        //------------------------- Branch Wise Data -------------------------------//

        [HttpGet]
        public async Task<ActionResult> MainDashboardAsyncHP()
        {
            if (Session["StaffCode"] == null)
            {

                return await Task.Run(() => RedirectToAction("Login", "Account"));

            }
            else
            {               
                Counsellor obj = new Counsellor();
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Name = " Counsellor Dashboard", Url =Url.Action(" MainDashboardAsyncHP","Counsellor")},
                    new BreadcrumbItem { Name = " Organization Dashboard", Url =Url.Action(" CounsellorOrganizationDashboarddHP","Counsellor")},
                   
                };

                ViewBag.Breadcrumbs = breadcrumbs;

                return await Task.Run(() => View("MainDashboardAsyncHP"));

            }
        }



            public async Task<ActionResult> CounsellorOrganizationDashboarddHP(DateTime? startDate, DateTime? endDate)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {

                Counsellor objnew = new Counsellor();
                objnew.BranchCode = Session["BranchCode"].ToString();
                objnew.StaffCode = Session["StaffCode"].ToString();
				DateTime currentDate = DateTime.Now;
				 string formattedDate = currentDate.ToString("yyyy-MM");
				 if (startDate.HasValue)
				 {
					 formattedDate = startDate.ToString();
				 }


      			objnew.CheckDate = formattedDate.AsDateTime();

                //// Brach Nmae Show
                 SqlDataReader drGetBranchName;
                drGetBranchName = await objbal.BranchCodeShowAsyncHP(objnew);
                //dr.Read();

                if (drGetBranchName.Read())
                {
                    objCounsellor.BranchCode = drGetBranchName["BranchName"].ToString();
                }
                drGetBranchName.Close();

                // Store the branch name in session
                Session["BranchName"] = objCounsellor.BranchCode;

                SqlDataReader drGetDilyEnquiryCount;
                ///  Daily Enquiry count               
                drGetDilyEnquiryCount = await objbal.DailyEnquiryCountAsyncHP(objnew);
                drGetDilyEnquiryCount.Read();
                objCounsellor.BranchCode = (drGetDilyEnquiryCount["todayCount"].ToString());
                drGetDilyEnquiryCount.Close();
                ViewBag.todayCount = objCounsellor.BranchCode;

                SqlDataReader drGetMonthlyLeadEnquiry;
                ///  monthly lead enquiry
                drGetMonthlyLeadEnquiry = await objbal.MonthlyLeadEnquiryCountAsyncHP(objnew);
                drGetMonthlyLeadEnquiry.Read();
                objCounsellor.BranchCode = (drGetMonthlyLeadEnquiry["monthlylead"].ToString());
                drGetMonthlyLeadEnquiry.Close();
                ViewBag.monthlylead = objCounsellor.BranchCode;
				
				     
				  SqlDataReader drmonthlyleadtoenq;
                ///  monthly lead enquiry
                drmonthlyleadtoenq = await objbal.MonthlyLeadTOEnquiryCountAsyncVP(objnew);
                drmonthlyleadtoenq.Read();
                objCounsellor.BranchCode = (drmonthlyleadtoenq["monthlyleadtoenqCount"].ToString());
                drmonthlyleadtoenq.Close();
                ViewBag.monthlyleadtoenqCount = objCounsellor.BranchCode;

                SqlDataReader drGetMonthlyEnquiry;
                ///  monthly enquiry
                drGetMonthlyEnquiry = await objbal.MonthlyEnquiryCountAsyncHP(objnew);
                drGetMonthlyEnquiry.Read();
                objCounsellor.BranchCode = (drGetMonthlyEnquiry["monthlyCount"].ToString());
                drGetMonthlyEnquiry.Close();
                ViewBag.monthlyCount = objCounsellor.BranchCode;

                SqlDataReader drGetMonthlyAdmission;
                ///  monthly addmission
                drGetMonthlyAdmission = await objbal.MonthlyAddmissionCountAsyncHP(objnew);
                drGetMonthlyAdmission.Read();
                objCounsellor.BranchCode = (drGetMonthlyAdmission["Addmission"].ToString());
                drGetMonthlyAdmission.Close();
                ViewBag.Addmission = objCounsellor.BranchCode;

                SqlDataReader drGetMonthlyLostEnquiry;
                /// Lost Enquiry

                drGetMonthlyLostEnquiry = await objbal.MonthlyLostEnquiryCountAsyncHP(objnew);
                drGetMonthlyLostEnquiry.Read();
                objCounsellor.BranchCode = (drGetMonthlyLostEnquiry["LostEnquiry"].ToString());
                drGetMonthlyLostEnquiry.Close();
                ViewBag.LostEnquiry = objCounsellor.BranchCode;

                SqlDataReader drGetMonthlyPendingEnquiry;
                ///  Pending Enquiry

                drGetMonthlyPendingEnquiry = await objbal.MonthlyPendingEnquiryCountAsyncHP(objnew);
                drGetMonthlyPendingEnquiry.Read();
                objCounsellor.BranchCode = (drGetMonthlyPendingEnquiry["PendingEnquiry"].ToString());
                drGetMonthlyPendingEnquiry.Close();
                ViewBag.PenholdEnquiry = objCounsellor.BranchCode;
                /////////////////////////

                SqlDataReader drGetMonthlyHoldEnquiry;

                drGetMonthlyHoldEnquiry = await objbal.MonthlyHoldEnquiryAsyncHP(objnew);
                drGetMonthlyHoldEnquiry.Read();
                objCounsellor.BranchCode = (drGetMonthlyHoldEnquiry["PendingEnquiry"].ToString());
                ViewBag.PendingEnquiry = objCounsellor.BranchCode;
                objCounsellor.BranchCode = (drGetMonthlyHoldEnquiry["HoldEnquiry"].ToString());
                ViewBag.HoldEnquiry = objCounsellor.BranchCode;
                drGetMonthlyHoldEnquiry.Close();

                SqlDataReader drGetMonthlyDiscountStudentCount;
                /// Disacount count

                drGetMonthlyDiscountStudentCount = await objbal.MonthlyDiscountStudentCountAsyncHP(objnew);
                drGetMonthlyDiscountStudentCount.Read();
                objCounsellor.BranchCode = (drGetMonthlyDiscountStudentCount["student_count"].ToString());
                drGetMonthlyDiscountStudentCount.Close();
                ViewBag.student_count = objCounsellor.BranchCode;
				
				//Billing Amount Card
				  SqlDataReader drGetBillingAmount;
                /// billing amount here

                drGetBillingAmount = await objbal.BillingTargetAsyncVP(objnew);
                drGetBillingAmount.Read();
                objCounsellor.BranchCode = (drGetBillingAmount["TotalBilling"].ToString());
                drGetBillingAmount.Close();
                ViewBag.BillingAmount = objCounsellor.BranchCode;

				SqlDataReader drGetTodaysFollowupCount;
				/// Today Followup count

				drGetTodaysFollowupCount = await objbal.TodaysFollowupCount(objnew);
				drGetTodaysFollowupCount.Read();
				objCounsellor.BranchCode = (drGetTodaysFollowupCount["TodaysFollowUpCount"].ToString());
				ViewBag.followup_count = objCounsellor.BranchCode;
				drGetTodaysFollowupCount.Close();

				SqlDataReader drGetMissedFollowupCount;
				/// Missed Followup count

				drGetTodaysFollowupCount = await objbal.MissedFollowup(objnew);
				drGetTodaysFollowupCount.Read();
				objCounsellor.BranchCode = (drGetTodaysFollowupCount["MissedFollowUpCount"].ToString());
				ViewBag.missedfp_count = objCounsellor.BranchCode;
				drGetTodaysFollowupCount.Close();
				
                ///  Upcoming batches count
                SqlDataReader drGetUpcomingBatchesCount;
                drGetUpcomingBatchesCount = await objbal.UpComingBatchAsyncHP(objnew);
                List<Counsellor> upcomingBatches = new List<Counsellor>();

                while (drGetUpcomingBatchesCount.Read())
                {
                    Counsellor batch = new Counsellor(); // Create an instance of your model class
                    batch.CourseName = drGetUpcomingBatchesCount["CourseName"].ToString();
                    batch.BatchName = drGetUpcomingBatchesCount["BatchName"].ToString();
                    batch.StartDate = Convert.ToDateTime(drGetUpcomingBatchesCount["TentativeBatchDate"].ToString());
                    batch.TypeName = drGetUpcomingBatchesCount["TypeName"].ToString();


                    upcomingBatches.Add(batch); // Add the batch to the list
                }

                ViewBag.UpcomingBatches = upcomingBatches; // Set the list in ViewBag
                drGetUpcomingBatchesCount.Close();

                SqlDataReader drGetIntExtAdmissionCount;
                ///  Course VS Placement Addmistion Count
                drGetIntExtAdmissionCount = await objbal.ShowCountCourseandPlacementAddmittedStudentAsyncHP(objnew);
                drGetIntExtAdmissionCount.Read();
                objCounsellor.BranchCode = (drGetIntExtAdmissionCount["CourseAddmisionCount"].ToString());

                ViewBag.CourseAddmisionCount = objCounsellor.BranchCode;
                objCounsellor.BranchCode = (drGetIntExtAdmissionCount["PlacementAddmisionCount"].ToString());
                drGetIntExtAdmissionCount.Close();
                ViewBag.PlacementAddmisionCount = objCounsellor.BranchCode;


              using (SqlDataReader drGetCourseVsPlacementLeads = await objbal.CoursevsPlacementLeadstAsyncHP(objnew))
			 {
				 if (drGetCourseVsPlacementLeads.Read())
				 {
					 ViewBag.CourseLeadsCount = drGetCourseVsPlacementLeads["CourseLeadsCount"].ToString();
					 ViewBag.PlacementLeadsCount = drGetCourseVsPlacementLeads["PlacementLeadsCount"].ToString();
				 }
				 else
				 {
					 ViewBag.CourseLeadsCount = "0";
					 ViewBag.PlacementLeadsCount = "0";
				 }
			 }
				string currentMonthYear = DateTime.Now.ToString("yyyy-MM");

				string previousMonthYear = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");


				ViewBag.StartMonthYear = currentMonthYear;
				ViewBag.EndMonthYear = previousMonthYear;

                SqlDataReader drGetIntVsExtEnquiry;
                ///   Course vs Placement Enquiry Count 

                drGetIntVsExtEnquiry = await objbal.CoursevsPlacementEnquiryAsyncHP(objnew);
                drGetIntVsExtEnquiry.Read();
                objCounsellor.BranchCode = (drGetIntVsExtEnquiry["CourseEnquiryCount"].ToString());

                ViewBag.CourseEnquiryCount = objCounsellor.BranchCode;
                objCounsellor.BranchCode = (drGetIntVsExtEnquiry["PlacementEnquiryCount"].ToString());
                drGetIntVsExtEnquiry.Close();
                ViewBag.PlacementEnquiryCount = objCounsellor.BranchCode;


                ///Prospective count
                SqlDataReader drGetProspectiveCandidateCount;
                drGetProspectiveCandidateCount = await objbal.ProspectiveCandidatecountAsyncHP(objnew);
                drGetProspectiveCandidateCount.Read();
                objCounsellor.BranchCode = (drGetProspectiveCandidateCount["monthlyProspectiveCount"].ToString());
                drGetProspectiveCandidateCount.Close();
                ViewBag.monthlyProspectiveCount = objCounsellor.BranchCode;

                ///Enquir4y form pending
                SqlDataReader drGetEnquiryFormPendingCount;
                drGetEnquiryFormPendingCount = await objbal.EnquiryFormPendingcountAsyncHP(objnew);
                drGetEnquiryFormPendingCount.Read();
                objCounsellor.BranchCode = (drGetEnquiryFormPendingCount["EnquiryFormPending"].ToString());
                drGetEnquiryFormPendingCount.Close();
                ViewBag.EnquiryFormPending = objCounsellor.BranchCode;

                await GraphEnqvsAdmission(startDate, endDate);
                await GraphCourseVsPlacement(startDate, endDate);
                await GraphCourseWiseAdmissionCount(startDate, endDate);
                await SourceWiseLeadVsEnquiry(startDate, endDate);


                return PartialView("CounsellorOrganizationDashboarddHP", objCounsellor);


            }

        }
        /// <summary>
        /// DailyEnquiryList
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> TodayEnquiryListAsyncHP()
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {
                objCounsellor.BranchCode = Session["BranchCode"].ToString();

                Counsellor objc = new Counsellor();
                DataSet dsTD = await objbal.TodayEnquiryListAsyncHP(objCounsellor);
                List<Counsellor> lstfortodaysenquiry = new List<Counsellor>();

                foreach (DataRow row in dsTD.Tables[0].Rows)
                {
                    Counsellor obj = new Counsellor();
                    obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
                    obj.CandidateCode = row["CandidateCode"].ToString();
                    obj.FullName = row["FullName"].ToString();
                    obj.EmailId = row["EmailID"].ToString();
                    obj.ContactNumber = row["ContactNumber"].ToString();
                    obj.EnquiryFor = row["EnquiryFor"].ToString();
                    obj.SourceName = row["SourceName"].ToString();

                    lstfortodaysenquiry.Add(obj);
                }
                objc.lstfortodaysenquiry = lstfortodaysenquiry;
                return PartialView(objc);
            }
        }
        /// <summary>
        /// Discount CountList
        /// </summary>
        /// <returns></returns>

        public async Task<ActionResult> DiscountCandidateListAsyncHP(DateTime selectedDate)
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            { 
                objCounsellor.BranchCode = Session["BranchCode"].ToString();

                Counsellor objc = new Counsellor();
				objCounsellor.CheckDate = selectedDate;
                DataSet dsTD = await objbal.DiscountCandidateListAsyncHP(objCounsellor);
                List<Counsellor> lstforDiscountCandid = new List<Counsellor>();

            foreach (DataRow row in dsTD.Tables[0].Rows)
            {
                Counsellor obj = new Counsellor();
                obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
                obj.CandidateCode = row["CandidateCode"].ToString();
                obj.FullName = row["FullName"].ToString();
                obj.EmailId = row["EmailID"].ToString();
                obj.ContactNumber = row["ContactNumber"].ToString();
                obj.EnquiryFor = row["EnquiryFor"].ToString();
				obj.Discount = int.Parse(row["Discount"].ToString());

                lstforDiscountCandid.Add(obj);
            }
           
            objc.lstforDiscountCandid = lstforDiscountCandid;
            return PartialView(objc);
            }
        }
        /// <summary>
        /// PendingList
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> PendingCandidateListAsyncHP()
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {
                objCounsellor.BranchCode = Session["BranchCode"].ToString();

                Counsellor objc = new Counsellor();
                DataSet dsTD = await objbal.PendingCandidateListAsyncHP(objCounsellor);
                List<Counsellor> lstforPendingCandid = new List<Counsellor>();

                foreach (DataRow row in dsTD.Tables[0].Rows)
                {
                    Counsellor obj = new Counsellor();
                    obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
                    obj.CandidateCode = row["CandidateCode"].ToString();
                    obj.FullName = row["FullName"].ToString();
                    obj.EmailId = row["EmailID"].ToString();
                    obj.ContactNumber = row["ContactNumber"].ToString();
                    obj.EnquiryFor = row["EnquiryFor"].ToString();
                    obj.Status = row["Status"].ToString();
                   
                    lstforPendingCandid.Add(obj);
                }
                objc.lstfortodaysenquiry = lstforPendingCandid;
                return PartialView(objc);
            }
        }
        /// <summary>
        /// LostList
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> LostCandidateListAsyncHP(DateTime selectedDate)
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {
                objCounsellor.BranchCode = Session["BranchCode"].ToString();

                Counsellor objc = new Counsellor();
				objCounsellor.CheckDate = selectedDate;
                DataSet dsLC = await objbal.LostCandidateListAsyncHP(objCounsellor);
                List<Counsellor> lstforlostsenquiry = new List<Counsellor>();

                foreach (DataRow row in dsLC.Tables[0].Rows)
                {
                    Counsellor obj = new Counsellor();
                    obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
                    obj.CandidateCode = row["CandidateCode"].ToString();
                    obj.FullName = row["FullName"].ToString();
                    obj.EmailId = row["EmailID"].ToString();
                    obj.ContactNumber = row["ContactNumber"].ToString();
                    obj.EnquiryFor = row["EnquiryFor"].ToString();              
                    obj.FollowUpNote = row["FollowUpNote"].ToString();
                    lstforlostsenquiry.Add(obj);
                }
                objc.lstforlostsenquiry = lstforlostsenquiry;
                return PartialView(objc);
            }
        }
        /// <summary>
        /// LradList
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> LeadCandidateListAsyncHP(DateTime selectedDate)
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {
                objCounsellor.BranchCode = Session["BranchCode"].ToString();

                Counsellor objc = new Counsellor();
				objCounsellor.CheckDate = selectedDate;
                DataSet dsTD = await objbal.LeadCandidateListAsyncHP(objCounsellor);
                List<Counsellor> lstForLeads = new List<Counsellor>();

                foreach (DataRow row in dsTD.Tables[0].Rows)
                {
                    Counsellor obj = new Counsellor();
                    obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
                    obj.CandidateCode = row["CandidateCode"].ToString();
                    obj.FullName = row["FullName"].ToString();
                    obj.EmailId = row["EmailId"].ToString();
                    obj.ContactNumber = row["ContactNumber"].ToString();
                    obj.SourceName = row["SourceName"].ToString();
                    obj.EnquiryFor = row["EnquiryFor"].ToString();
                    lstForLeads.Add(obj);
                }
                objc.lstForLeads = lstForLeads;
                return PartialView(objc);
            }
        }
		/// <summary>
		 /// this method is written for the getting the view for the lead to enquiry converted candidate list
		 /// </summary>
		 /// <returns></returns>
		 public async Task<ActionResult> LeadToEnqCandidateListAsyncVP(DateTime selectedDate)
		 {

			 if (Session["StaffCode"] == null)
			 {
				 return RedirectToAction("Login", "Account");

			 }
			 else
			 {
				 objCounsellor.BranchCode = Session["BranchCode"].ToString();

				 Counsellor objc = new Counsellor();
				 objCounsellor.CheckDate = selectedDate;
				 DataSet dsTD = await objbal.LeadTOEnqCandidateListAsyncVP(objCounsellor);
				 List<Counsellor> lstForLeads = new List<Counsellor>();

				 foreach (DataRow row in dsTD.Tables[0].Rows)
				 {
					 Counsellor obj = new Counsellor();
					 obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
					 obj.CandidateCode = row["CandidateCode"].ToString();
					 obj.FullName = row["FullName"].ToString();
					 obj.EmailId = row["EmailId"].ToString();
					 obj.ContactNumber = row["ContactNumber"].ToString();
					 obj.SourceName = row["SourceName"].ToString();
					 obj.EnquiryFor = row["EnquiryFor"].ToString();
					 lstForLeads.Add(obj);
				 }
				 objc.lstForLeads = lstForLeads;
				 return PartialView("LeadCandidateListAsyncHP",objc);
			 }
		 }

        /// <summary>
        /// Monthluenquiry
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> EnquiryCandidatetListAsyncHP(DateTime selectedDate)
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {
                objCounsellor.BranchCode = Session["BranchCode"].ToString();

                Counsellor objc = new Counsellor();
				objCounsellor.CheckDate = selectedDate;
                DataSet dsTD = await objbal.EnquiryCandidatetListAsyncHP(objCounsellor);
                List<Counsellor> lstForEnquiryList = new List<Counsellor>();

                foreach (DataRow row in dsTD.Tables[0].Rows)
                {
                    Counsellor obj = new Counsellor();
                    obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
                    obj.CandidateCode = row["CandidateCode"].ToString();
                    obj.FullName = row["FullName"].ToString();
                    obj.EmailId = row["EmailID"].ToString();
                    obj.ContactNumber = row["ContactNumber"].ToString();
                    obj.EnquiryFor = row["EnquiryFor"].ToString();
                    lstForEnquiryList.Add(obj);
                }
                objc.lstForEnquiryList = lstForEnquiryList;
                return PartialView(objc);
            }
        }
        /// <summary>
        /// Addmission List
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> AddmissionCandidatetListAsyncHP(DateTime selectedDate)
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {
                objCounsellor.BranchCode = Session["BranchCode"].ToString();

                Counsellor objc = new Counsellor();
				objCounsellor.CheckDate = selectedDate;
                DataSet dsTD = await objbal.AddmissionCandidatetListAsyncHP(objCounsellor);
                List<Counsellor> lstForAddmissionList = new List<Counsellor>();

                foreach (DataRow row in dsTD.Tables[0].Rows)
                {
                    Counsellor obj = new Counsellor();
                    obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
                    obj.CandidateCode = row["CandidateCode"].ToString();
                    obj.FullName = row["FullName"].ToString();
                    obj.EmailId = row["EmailID"].ToString();
                    obj.ContactNumber = row["ContactNumber"].ToString();
                    obj.EnquiryFor = row["EnquiryFor"].ToString();
					obj.CourseName = row["CourseName"].ToString();
                    lstForAddmissionList.Add(obj);
                }
                objc.lstForAddmissionList = lstForAddmissionList;
                return PartialView(objc);
            }
        }
		/// <summary>
		/// PendingList
		/// </summary>
		/// <returns></returns>
		public async Task<ActionResult> PendingCandidateListAsyncHP(DateTime selectedDate)
		{

			if (Session["StaffCode"] == null)
			{
				return RedirectToAction("Login", "Account");

			}
			else
			{
				objCounsellor.BranchCode = Session["BranchCode"].ToString();

				Counsellor objc = new Counsellor();
				objCounsellor.CheckDate = selectedDate;
				DataSet dsTD = await objbal.PendingCandidateListAsyncHP(objCounsellor);
				List<Counsellor> lstforPendingCandid = new List<Counsellor>();

				foreach (DataRow row in dsTD.Tables[0].Rows)
				{
					Counsellor obj = new Counsellor();
					obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
					obj.CandidateCode = row["CandidateCode"].ToString();
					obj.FullName = row["FullName"].ToString();
					obj.EmailId = row["EmailID"].ToString();
					obj.ContactNumber = row["ContactNumber"].ToString();
					obj.EnquiryFor = row["EnquiryFor"].ToString();
					obj.Status = row["Status"].ToString();

					lstforPendingCandid.Add(obj);
				}
				objc.lstfortodaysenquiry = lstforPendingCandid;
				return PartialView(objc);
			}
		}
        /// <summary>
        /// Enq Form Pendinglist
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> EnquiryFormPendingListAsyncHP(DateTime selectedDate)
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {
                objCounsellor.BranchCode = Session["BranchCode"].ToString();

                Counsellor objc = new Counsellor();
				objCounsellor.CheckDate = selectedDate;
                DataSet dsTD = await objbal.EnquiryFormPendingListAsyncHP(objCounsellor);
                List<Counsellor> lstForPendingform = new List<Counsellor>();

                foreach (DataRow row in dsTD.Tables[0].Rows)
                {
                    Counsellor obj = new Counsellor();
                    obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
                    obj.CandidateCode = row["CandidateCode"].ToString();
                    obj.FullName = row["FullName"].ToString();
                    obj.EmailId = row["EmailID"].ToString();
                    obj.ContactNumber = row["ContactNumber"].ToString();
                    obj.EnquiryFor = row["EnquiryFor"].ToString();
                    lstForPendingform.Add(obj);
                }
                objc.lstForPendingform = lstForPendingform;
                return PartialView(objc);
            }
        }




        //---------------------------------- Staff Wise Data -------------------------------//      


        /// <summary>
        /// This is Counsellor Self Dashboard method for dashboard counts.
        /// </summary>
        /// <returns>Returns the counts.</returns>

        public async Task<ActionResult> SelfDashboardAsyncHP(DateTime? startDate, DateTime? endDate)
        {
            Counsellor objnew = new Counsellor();
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else

                objnew.StaffCode = Session["StaffCode"].ToString();
			DateTime currentDate = DateTime.Now;
			 string formattedDate = currentDate.ToString("yyyy-MM");
			 if (startDate.HasValue)
			 {
				 formattedDate = startDate.ToString();
			 }

			 objnew.CheckDate = formattedDate.AsDateTime();

            SqlDataReader dr;
            ///   Enquiry Count
            dr = await objbal.PersonalMonthlyEnquiryCountAsyncHP(objnew);
            dr.Read();
            objCounsellor.StaffCode = (dr["monthlyCount"].ToString());
            dr.Close();
            ViewBag.monthlyCount = objCounsellor.StaffCode;

            ///  Addmition Count
            dr = await objbal.PersonalMonthlyAddmittedCountAsyncHP(objnew);
            dr.Read();
            objCounsellor.StaffCode = (dr["Addmission"].ToString());
            dr.Close();
            ViewBag.AddmissionCount = objCounsellor.StaffCode;

            ///  Lost Enquiry Count

            dr = await objbal.MonthlyStaffWiseLostEnquiryCountAsyncHP(objnew);
            dr.Read();
            objCounsellor.StaffCode = (dr["LostEnquiry"].ToString());
            dr.Close();
            ViewBag.LostEnquiry = objCounsellor.StaffCode;
            ///  Pending Enquiry Count

            dr = await objbal.MonthlyStaffWisePendingEnquiryCountAsyncHP(objnew);
            dr.Read();
            objCounsellor.StaffCode = (dr["PendingEnquiry"].ToString());
            dr.Close();
            ViewBag.PendingEnquiry = objCounsellor.StaffCode;

            /// hod pending count show
            dr = await objbal.MonthlyPendHoldStaffEnqCountAsyncHP(objnew);
            dr.Read();
            objCounsellor.StaffCode = (dr["PendingEnquiry"].ToString());
            ViewBag.PendingEnquiryy = objCounsellor.StaffCode;
            objCounsellor.StaffCode = (dr["HoldEnquiry"].ToString());
            ViewBag.HoldEnquiryy = objCounsellor.StaffCode;
            dr.Close();

            ///  Course Vs Placement Addmission  Count 
            dr = await objbal.CourseandPlacementAddmittedCountStaffAsyncHP(objnew);
            dr.Read();
            objCounsellor.StaffCode = (dr["CourseEnquiryCount"].ToString());

            ViewBag.CourseAddmisionCount = objCounsellor.StaffCode;
            objCounsellor.StaffCode = (dr["PlacementEnquiryCount"].ToString());
            dr.Close();
            ViewBag.PlacementAddmisionCount = objCounsellor.StaffCode;

            ///  Course Vs Placement Enquiry  Count 
            dr = await objbal.CoursevsPlacementEnquiryStaffAsyncHP(objnew);
            dr.Read();
            objCounsellor.StaffCode = (dr["CourseEnquiryCount"].ToString());

            ViewBag.CourseEnquiryCount = objCounsellor.StaffCode;
            objCounsellor.StaffCode = (dr["PlacementEnquiryCount"].ToString());
            dr.Close();
            ViewBag.PlacementEnquiryCount = objCounsellor.StaffCode;

            await GraphEnqvsAdmissionself(startDate, endDate);
            await StaffCourseWiseAdmissionCount(startDate, endDate);
            //await 
            return PartialView("SelfDashboardAsyncHP", objCounsellor);

        }
        [HttpGet]
        public async Task<ActionResult> GraphEnqvsAdmissionself(DateTime? startDate, DateTime? endDate)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {

                objCounsellor.StaffCode = Session["StaffCode"].ToString();

                // Check if a date range is provided
                DateTime StartDate, EndDate;
                if (startDate.HasValue && endDate.HasValue)
                {
                    StartDate = startDate.Value;
                    EndDate = endDate.Value;
                }
                else
                {
                    // Use default dates if no date range is provided
                    StartDate = DateTime.Now.AddYears(-1);
                    EndDate = DateTime.Now;
                }

                objCounsellor.StartDate = StartDate;
                objCounsellor.EndDate = EndDate;

                DataSet ds = await objbal.EnquiryvsAddmittedCountGraphStaffWiseAsyncHP(objCounsellor);
                List<object> dataPoint = new List<object>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int year = Convert.ToInt32(row["year"]);
                    string monthss = row["month_name"].ToString();
                    string LeadCount = row["lead_count"].ToString();
                    string EnquiryCount = row["enquiry_count"].ToString();
                    string AdmissionCount = row["admitted_count"].ToString();

                    dataPoint.Add(new { Label = $"{monthss}-{year}", Lead = LeadCount, enquiry = EnquiryCount, Admission = AdmissionCount });
                }

                if (startDate.HasValue && endDate.HasValue)
                {
                    ViewBag.FilteredEnqVsAdmGraphStaff = Newtonsoft.Json.JsonConvert.SerializeObject(dataPoint);
                }
                else
                {
                    ViewBag.DataPoint2 = Newtonsoft.Json.JsonConvert.SerializeObject(dataPoint);
                }

                var DataFetchedEnqvsAdmissionStaff = ViewBag.FilteredEnqVsAdmGraphStaff;

                TempData["StartDate"] = StartDate.ToString("yyyy-MM-dd");
                TempData["EndDate"] = EndDate.ToString("yyyy-MM-dd");
                return Json(new { DataFetchedEnqvsAdmissionStaff }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpGet]
        public async Task<ActionResult> GraphEnqvsAdmission(DateTime? startDate, DateTime? endDate)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {

                objCounsellor.BranchCode = Session["BranchCode"].ToString();

                // Check if a date range is provided
                DateTime StartDate, EndDate;
                if (startDate.HasValue && endDate.HasValue)
                {
                    StartDate = startDate.Value;
                    EndDate = endDate.Value;
                }
                else
                {
                    // Use default dates if no date range is provided
                    StartDate = DateTime.Now.AddYears(-1);
                    EndDate = DateTime.Now;
                }

                objCounsellor.StartDate = StartDate;
                objCounsellor.EndDate = EndDate;

                DataSet ds = await objbal.EnquiryVSAddmittedStudentCountBranchAsyncHP(objCounsellor);
                List<object> dataPoint = new List<object>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int year = Convert.ToInt32(row["year"]);
                    string monthss = row["month_name"].ToString();
                    string LeadCount = row["lead_count"].ToString();
                    string EnquiryCount = row["enquiry_count"].ToString();
                    string AdmissionCount = row["admitted_count"].ToString();                  

                    dataPoint.Add(new { Label = $"{monthss}-{year}", Lead = LeadCount, enquiry = EnquiryCount, Admission = AdmissionCount });
                }

                if (startDate.HasValue && endDate.HasValue)
                {
                    ViewBag.FilteredEnqVsAdmGraph = Newtonsoft.Json.JsonConvert.SerializeObject(dataPoint);
                }
                else
                {
                    ViewBag.DataPoint1 = Newtonsoft.Json.JsonConvert.SerializeObject(dataPoint);
                }

                var DataFetchedEnqvsAdmission = ViewBag.FilteredEnqVsAdmGraph;

                TempData["StartDate"] = StartDate.ToString("yyyy-MM-dd");
                TempData["EndDate"] = EndDate.ToString("yyyy-MM-dd");
                return Json(new { DataFetchedEnqvsAdmission }, JsonRequestBehavior.AllowGet);
            }

        }

        public async Task<ActionResult> GraphCourseVsPlacement(DateTime? startDate, DateTime? endDate)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {
                objCounsellor.BranchCode = Session["BranchCode"].ToString();

								// Check if a date range is provided
				 DateTime StartDate, EndDate;

				 // Check if the startDate and endDate have values
				 if (startDate.HasValue && endDate.HasValue)
				 {
					 // Set StartDate to the 1st of the selected start month
					 StartDate = new DateTime(startDate.Value.Year, startDate.Value.Month, 1);

					 // Set EndDate to the last day of the selected end month
					 EndDate = new DateTime(endDate.Value.Year, endDate.Value.Month, 1).AddMonths(1).AddDays(-1);

					 // Get today's date without time
					 DateTime today = DateTime.Today;

					 // If EndDate is in the future, adjust to today's date if it's the current month
					 if (EndDate > today)
					 {
						 if (EndDate.Month == today.Month && EndDate.Year == today.Year)
						 {
							 EndDate = today; // Use today's date if it's within the current month
						 }
					 }
				 }
				 else
				 {
					 // Use default dates if no date range is provided
					 StartDate = DateTime.Now.AddYears(-1); // Default to 1 year ago
					 EndDate = DateTime.Now; // Default to today
				 }
  				objCounsellor.StartDate = StartDate;
                objCounsellor.EndDate = EndDate;


                DataSet ds1 = await objbal.CourseandPlacementAddmittedStudentGraphBrachWiseAsyncHP(objCounsellor);
                List<object> dataPoint3 = new List<object>();

                if (ds1 != null && ds1.Tables.Count > 0)
                {
                    DataTable dt = ds1.Tables[0];

                    // Ensure the column exists
                    if (dt.Columns.Contains("MonthYear")  &&
                        dt.Columns.Contains("CourseAdmissionCount") && dt.Columns.Contains("PlacementAdmissionCount"))
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            //int Year = Convert.ToInt32(row["Year"]);
                            string MonthYear = row["MonthYear"] == DBNull.Value ? string.Empty : row["MonthYear"].ToString();
                            int CourseAdmissionCount = row["CourseAdmissionCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["CourseAdmissionCount"]);
                            int PlacementAdmissionCount = row["PlacementAdmissionCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["PlacementAdmissionCount"]);

                            dataPoint3.Add(new { Label = $"{MonthYear}", Course = CourseAdmissionCount, Placement = PlacementAdmissionCount });
                        }
                    }
                }

                ViewBag.DataPoint3 = Newtonsoft.Json.JsonConvert.SerializeObject(dataPoint3);
                var FilteredLineChartData = ViewBag.DataPoint3;
                return Json(new { FilteredLineChartData }, JsonRequestBehavior.AllowGet);
            }

        }

        public async Task<ActionResult> GraphCourseWiseAdmissionCount(DateTime? startDate, DateTime? endDate)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {
                objCounsellor.BranchCode = Session["BranchCode"].ToString();

                // Check if a date range is provided
                DateTime StartDate, EndDate;
                if (startDate.HasValue && endDate.HasValue)
                {
                    StartDate = startDate.Value;
                    EndDate = endDate.Value;
                }
                else
                {
                    // Use default dates if no date range is provided
                    StartDate = DateTime.Now.AddYears(-1);
                    EndDate = DateTime.Now;
                }
                objCounsellor.StartDate = StartDate;
                objCounsellor.EndDate = EndDate;



                DataSet dsCourseWiseAdmCount = await objbal.ParticulaCoureWiseAddMisionCountAsyncHP(objCounsellor);
                List<object> coursewiseAdmCount = new List<object>();

                if (dsCourseWiseAdmCount != null && dsCourseWiseAdmCount.Tables.Count > 0)
                {
                    DataTable dt = dsCourseWiseAdmCount.Tables[0];

                    // Ensure the column exists
                    if (dt.Columns.Contains("coursename") &&
                        dt.Columns.Contains("admission_count"))
                    {
                        foreach (DataRow row in dt.Rows)
                        {                            
                            string Course = row["coursename"] == DBNull.Value ? string.Empty : row["coursename"].ToString();
                            int admissionCount = row["admission_count"] == DBNull.Value ? 0 : Convert.ToInt32(row["admission_count"]);
                            coursewiseAdmCount.Add(new { Label = Course, AdmCount = admissionCount });
                        }
                    }
                }

                ViewBag.CourseWiseAdmCount = Newtonsoft.Json.JsonConvert.SerializeObject(coursewiseAdmCount);
                var FilteredLollypopChartData = ViewBag.CourseWiseAdmCount;
                return Json(new { FilteredLollypopChartData }, JsonRequestBehavior.AllowGet);
            }

        }


        public async Task<ActionResult> SourceWiseLeadVsEnquiry(DateTime? startDate, DateTime? endDate)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {
                objCounsellor.BranchCode = Session["BranchCode"].ToString();

                // Check if a date range is provided
                DateTime StartDate, EndDate;
                if (startDate.HasValue && endDate.HasValue)
                {
                    StartDate = startDate.Value;
                    EndDate = endDate.Value;
                }
                else
                {
                    // Use default dates if no date range is provided
                    StartDate = DateTime.Now.AddYears(-1);
                    EndDate = DateTime.Now;
                }
                objCounsellor.StartDate = StartDate;
                objCounsellor.EndDate = EndDate;



                DataSet dsSourceWiseCount = await objbal.SourceWiseEnquiryAndLeadCountAsyncHP(objCounsellor);
                List<object> SourceWiseLeadCount = new List<object>();

                if (dsSourceWiseCount != null && dsSourceWiseCount.Tables.Count > 0)
                {
                    DataTable dt = dsSourceWiseCount.Tables[0];

                    // Ensure the column exists


                    if (dt.Columns.Contains("SourceName") &&
                        dt.Columns.Contains("EnquiryCount"))
                    {
                        foreach (DataRow row in dt.Rows)
                        {                          
                            string SourceName = row["SourceName"] == DBNull.Value ? string.Empty : (row["SourceName"].ToString());
                            int EnquiryCount = row["EnquiryCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["EnquiryCount"]);
                            SourceWiseLeadCount.Add(new { Label = SourceName, enquirycount = EnquiryCount });
                        }
                    }
                }

                ViewBag.SourceWiseCount = Newtonsoft.Json.JsonConvert.SerializeObject(SourceWiseLeadCount);
                var FilteredPieChartData = ViewBag.SourceWiseCount;
                return Json(new { FilteredPieChartData }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Monthly Enquiries
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> MonthlyEnquiryListAsyncHP()
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {
                objCounsellor.StaffCode = Session["StaffCode"].ToString();

                Counsellor objc = new Counsellor();
                DataSet dsTD = await objbal.monthlyEnquiryListAsyncHP(objCounsellor);
                List<Counsellor> lstmonthlystaffEnquiries = new List<Counsellor>();

                foreach (DataRow row in dsTD.Tables[0].Rows)
                {
                    Counsellor obj = new Counsellor();
                    obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
                    obj.CandidateCode = row["CandidateCode"].ToString();
                    obj.FullName = row["FullName"].ToString();
                    obj.EmailId = row["EmailID"].ToString();
                    obj.ContactNumber = row["ContactNumber"].ToString();

                    obj.EnquiryFor = row["EnquiryFor"].ToString();

                    lstmonthlystaffEnquiries.Add(obj);
                }
                objc.lstmonthlystaffEnquiries = lstmonthlystaffEnquiries;
                return PartialView(objc);
            }
        }

        /// <summary>
        /// Monthly Admission Staffwise list
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> MonthlyAddmissionListAsyncHP()
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {
                objCounsellor.StaffCode = Session["StaffCode"].ToString();

                Counsellor objc = new Counsellor();
                DataSet dsTD = await objbal.MonthlyAddmissionListAsyncHP(objCounsellor);
                List<Counsellor> lstmonthlystaffAddmissions = new List<Counsellor>();

                foreach (DataRow row in dsTD.Tables[0].Rows)
                {
                    Counsellor obj = new Counsellor();
                    obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
                    obj.CandidateCode = row["CandidateCode"].ToString();
                    obj.FullName = row["FullName"].ToString();
                    obj.EmailId = row["EmailID"].ToString();
                    obj.ContactNumber = row["ContactNumber"].ToString();
                    obj.EnquiryFor = row["EnquiryFor"].ToString();
                    lstmonthlystaffAddmissions.Add(obj);
                }
                objc.lstmonthlystaffAddmissions = lstmonthlystaffAddmissions;
                return PartialView(objc);
            }
        }


        public async Task<ActionResult> LostCandidListAsyncHP()
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {
                objCounsellor.StaffCode = Session["StaffCode"].ToString();
                Counsellor objc = new Counsellor();
                DataSet dsLCL = await objbal.LostCandidListAsyncHP(objCounsellor);
                List<Counsellor> lstlostenquirystaff = new List<Counsellor>();
                foreach (DataRow row in dsLCL.Tables[0].Rows)
                {
                    Counsellor obj = new Counsellor();
                    obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
                    obj.CandidateCode = row["CandidateCode"].ToString();
                    obj.FullName = row["FullName"].ToString();
                    obj.EmailId = row["EmailID"].ToString();
                    obj.ContactNumber = row["ContactNumber"].ToString();
                    obj.EnquiryFor = row["EnquiryFor"].ToString();                  
                    obj.FollowUpNote = row["FollowUpNote"].ToString();
                    lstlostenquirystaff.Add(obj);
                }
                objc.lstlostenquirystaff = lstlostenquirystaff;
                return PartialView(objc);
            }
        }


        public async Task<ActionResult> PendingCandidListAsyncHP()
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {
                objCounsellor.StaffCode = Session["StaffCode"].ToString();

                Counsellor objc = new Counsellor();
                DataSet dsTD = await objbal.PendingCandidListAsyncHP(objCounsellor);
                List<Counsellor> lstpendingenqStaff = new List<Counsellor>();

                foreach (DataRow row in dsTD.Tables[0].Rows)
                {
                    Counsellor obj = new Counsellor();
                    obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
                    obj.CandidateCode = row["CandidateCode"].ToString();
                    obj.FullName = row["FullName"].ToString();
                    obj.EmailId = row["EmailID"].ToString();
                    obj.ContactNumber = row["ContactNumber"].ToString();
                    obj.EnquiryFor = row["EnquiryFor"].ToString();
                    obj.Status = row["Status"].ToString();

                    lstpendingenqStaff.Add(obj);
                }
                objc.lstpendingenqStaff = lstpendingenqStaff;
                return PartialView(objc);
            }
        }


        public async Task<ActionResult> StaffCourseWiseAdmissionCount(DateTime? startDate, DateTime? endDate)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {
                objCounsellor.StaffCode = Session["StaffCode"].ToString();

                // Check if a date range is provided
                DateTime StartDate, EndDate;
                if (startDate.HasValue && endDate.HasValue)
                {
                    StartDate = startDate.Value;
                    EndDate = endDate.Value;
                }
                else
                {
                    // Use default dates if no date range is provided
                    StartDate = DateTime.Now.AddYears(-1);
                    EndDate = DateTime.Now;
                }
                objCounsellor.StartDate = StartDate;
                objCounsellor.EndDate = EndDate;



                DataSet dsCourseWiseAdmCount = await objbal.StaffCoureWiseAddMisionCountAsyncHP(objCounsellor);
                List<object> coursewiseAdmCounts = new List<object>();

                if (dsCourseWiseAdmCount != null && dsCourseWiseAdmCount.Tables.Count > 0)
                {
                    DataTable dt = dsCourseWiseAdmCount.Tables[0];

                    // Ensure the column exists
                    if (dt.Columns.Contains("coursename") &&
                        dt.Columns.Contains("admission_count"))
                    {
                        foreach (DataRow row in dt.Rows)
                        {                       
                            string Course = row["coursename"] == DBNull.Value ? string.Empty : row["coursename"].ToString();
                            int admissionCount = row["admission_count"] == DBNull.Value ? 0 : Convert.ToInt32(row["admission_count"]);

                            coursewiseAdmCounts.Add(new { Label = Course, AdmCount = admissionCount });
                        }
                    }
                }

                ViewBag.CourseWiseAdmCounts = Newtonsoft.Json.JsonConvert.SerializeObject(coursewiseAdmCounts);
                var FilteredLollyPopChartDatastaff = ViewBag.CourseWiseAdmCounts;
                return Json(new { FilteredLollyPopChartDatastaff }, JsonRequestBehavior.AllowGet);
            }

        }



        public async Task<ActionResult> ReadbatchnameAsyncHP(string batchName)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var message = "";
                Counsellor objC = new Counsellor();
                objC.BranchCode = Session["BranchCode"].ToString();
                objC.BatchName = batchName;
                SqlDataReader dr;
                dr = await objbal.ReadBatchNameAsyncHP(objC);
                objC.BatchName = null;
                while (dr.Read())
                {
                    objC.BatchName = (dr["BatchName"].ToString());

                }
                if (objC.BatchName != null)
                {
                    message = "Batch Name already existed";
                }
                else
                {
                    message = "Batch Name verified";
                }
                return Json(new { success = message }, JsonRequestBehavior.AllowGet);
            }
        }


        //Himangi Dashboard End

        //Shrikant Leads Start

        public ActionResult ImportExcelFile()
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor ojj = new Counsellor();
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                {
                     new BreadcrumbItem { Name = "Counsellor Dashboard", Url =Url.Action("MainDashboardAsyncHP","Counsellor")},
                     new BreadcrumbItem { Name = "Leads", Url =Url.Action("ImportExcelFile","Counsellor") },
                };
                ViewBag.Breadcrumbs = breadcrumbs;
                return View("ImportExcelFile");
            }
        }


        [HttpPost]
            public async Task<ActionResult> ImportExcelFile(HttpPostedFileBase file)
        {
            var errors = new List<object>();

            try
            {
                // Validate the uploaded file
                if (file == null || file.ContentLength == 0)
                {
                    return Json(new { success = false, message = "Please select a file to import." });
                }

                if (!file.FileName.EndsWith(".xlsx"))
                {
                    return Json(new { success = false, message = "Please select a valid .xlsx file." });
                }

                using (var workbook = new XLWorkbook(file.InputStream))
                {
                    var worksheet = workbook.Worksheet(1);

                    // Iterate through rows and validate each record
                    for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
                    {
                        var rowErrors = ValidateRow(worksheet, row);
                        if (rowErrors.Any())
                        {
                            errors.AddRange(rowErrors);
                        }
                    }

                    // If validation errors exist, return them
                    if (errors.Any())
                    {
                        return Json(new { success = false, message = "Validation errors occurred.", errors });
                    }

                    // Save data if no errors
                    for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
                    {
                        var counsellor = new Counsellor
                        {
                            FullName = worksheet.Cell(row, 1).GetValue<string>(),
                            Gender = worksheet.Cell(row, 2).GetValue<string>(),
                            PresentAddress = worksheet.Cell(row, 3).GetValue<string>(),
                            PresentPincode = worksheet.Cell(row, 4).GetValue<string>(),
                            ContactNumber = worksheet.Cell(row, 5).GetValue<string>(),
                            AlternateNumber = worksheet.Cell(row, 6).GetValue<string>(),
                            EmailId = worksheet.Cell(row, 7).GetValue<string>(),
                            EquiryDescription = worksheet.Cell(row, 8).GetValue<string>(),
                            EnquiryDate = DateTime.Parse(worksheet.Cell(row, 9).GetValue<string>()),
                            EnquiryFor = worksheet.Cell(row, 10).GetValue<string>(),
                            StaffCode = Session["StaffCode"]?.ToString(),
                            BranchCode = Session["BranchCode"]?.ToString()
                        };

                        await GetMaxCandidateCount(counsellor);
                        counsellor.CandidateCode = TempData["CandidateCode"] as string;
                        await objbal.EcxelEnquirySave(counsellor);
                    }
                }

                return Json(new { success = true, message = "File imported successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
		
		  private List<object> ValidateRow(IXLWorksheet worksheet, int row)
        {
            var errors = new List<object>();

            string fullName = worksheet.Cell(row, 1).GetValue<string>();
            string gender = worksheet.Cell(row, 2).GetValue<string>();
            string presentAddress = worksheet.Cell(row, 3).GetValue<string>();
            string presentPincode = worksheet.Cell(row, 4).GetValue<string>();
            string contactNumber = worksheet.Cell(row, 5).GetValue<string>();
            string alternateNumber = worksheet.Cell(row, 6).GetValue<string>();
            string emailId = worksheet.Cell(row, 7).GetValue<string>();
            string enquiryDateStr = worksheet.Cell(row, 9).GetValue<string>();
            string enquiryFor = worksheet.Cell(row, 10).GetValue<string>();

            // Validate required fields
            if (string.IsNullOrWhiteSpace(fullName))
                errors.Add(new { rowNumber = row, columnName = "FullName", message = "Full Name is required." });

            if (string.IsNullOrWhiteSpace(gender))
                errors.Add(new { rowNumber = row, columnName = "Gender", message = "Gender is required." });

            if (string.IsNullOrWhiteSpace(presentAddress))
                errors.Add(new { rowNumber = row, columnName = "PresentAddress", message = "Present Address is required." });

            if (string.IsNullOrWhiteSpace(presentPincode) || !presentPincode.All(char.IsDigit))
                errors.Add(new { rowNumber = row, columnName = "PresentPincode", message = "Present Pincode must be numeric." });

            if (string.IsNullOrWhiteSpace(contactNumber) || !contactNumber.All(char.IsDigit))
                errors.Add(new { rowNumber = row, columnName = "ContactNumber", message = "Contact Number must be numeric." });

            if (string.IsNullOrWhiteSpace(enquiryFor))
                errors.Add(new { rowNumber = row, columnName = "EnquiryFor", message = "Enquiry For is required." });

            // Validate optional fields
            if (!string.IsNullOrWhiteSpace(alternateNumber) && !alternateNumber.All(char.IsDigit))
                errors.Add(new { rowNumber = row, columnName = "AlternateNumber", message = "Alternate Number must be numeric." });

            if (!string.IsNullOrWhiteSpace(emailId) && !IsValidEmail(emailId))
                errors.Add(new { rowNumber = row, columnName = "EmailId", message = "Invalid Email ID." });

            // Validate enquiry date
            if (!DateTime.TryParse(enquiryDateStr, out _))
                errors.Add(new { rowNumber = row, columnName = "EnquiryDate", message = "Enquiry Date is invalid." });

            return errors;
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return true; // Valid email
            }
            catch (FormatException)
            {
                return false; // Invalid email format
            }
        }
		
        public async Task<ActionResult> ListStudentAsyncSS(DateTime? Startdate, DateTime? Enddate, string enquiryfor)
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor objcoun = new Counsellor();
                objcoun.StartDate = Startdate.Value.Date;
                objcoun.EndDate = Enddate.Value.Date;
                objcoun.StaffCode = Session["StaffCode"].ToString();
                objcoun.BranchCode = Session["BranchCode"].ToString();
                DataSet ds;
                ds = await objbal.AllReferenceListStudentAsyncSS(objcoun);
                List<Counsellor> studlist = new List<Counsellor>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Counsellor objcoun1 = new Counsellor();
                    objcoun1.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objcoun1.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    objcoun1.RefBy = ds.Tables[0].Rows[i]["RefBy"].ToString();
                    objcoun1.RefName = ds.Tables[0].Rows[i]["RefName"].ToString();
                    objcoun1.EquiryDescription = ds.Tables[0].Rows[i]["EquiryDescription"].ToString();
                    objcoun1.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                    objcoun1.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                    objcoun1.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                    objcoun1.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
					objcoun1.CourseName = ds.Tables[0].Rows[i]["CourseName"].ToString();
                    objcoun1.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                    objcoun1.AlternateNumber = ds.Tables[0].Rows[i]["AlternateNumber"].ToString();                    
                    objcoun1.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    objcoun1.EnqStatusId = int.Parse(ds.Tables[0].Rows[i]["Statusid"].ToString());
                    studlist.Add(objcoun1);
                }

                List<Counsellor> filteredlist = new List<Counsellor>();
                if (!string.IsNullOrEmpty(enquiryfor))
                {
                    filteredlist = studlist.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                    objcoun.lstReferenceAsyncSS = filteredlist;
                }
                else
                {
                    objcoun.lstReferenceAsyncSS = studlist;
                }
                return PartialView("ListStudentAsyncSS", objcoun);
            }

        }
        public async Task<ActionResult> WebListStudentAsyncSS(DateTime? Startdate, DateTime? Enddate, string enquiryfor)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor objcoun = new Counsellor();
                objcoun.StartDate = Startdate.Value.Date;
                objcoun.EndDate = Enddate.Value.Date;
                objcoun.StaffCode = Session["StaffCode"].ToString();
                objcoun.BranchCode = Session["BranchCode"].ToString();
                DataSet ds = new DataSet();
                ds = await objbal.WebReferenceListStudentAsyncSS(objcoun);
                List<Counsellor> studlist = new List<Counsellor>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Counsellor objcoun1 = new Counsellor();
                    objcoun1.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objcoun1.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    objcoun1.EquiryDescription = ds.Tables[0].Rows[i]["EquiryDescription"].ToString();
                    objcoun1.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                    objcoun1.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                    objcoun1.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                    objcoun1.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
					objcoun1.CourseName = ds.Tables[0].Rows[i]["CourseName"].ToString();
                    objcoun1.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                    objcoun1.AlternateNumber = ds.Tables[0].Rows[i]["AlternateNumber"].ToString();
                    objcoun1.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    objcoun1.EnqStatusId = int.Parse(ds.Tables[0].Rows[i]["Statusid"].ToString());
                    studlist.Add(objcoun1);
                }
                List<Counsellor> filteredlist = new List<Counsellor>();
                if (!string.IsNullOrEmpty(enquiryfor))
                {
                    filteredlist = studlist.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                    objcoun.lstReferenceAsyncSS = filteredlist;
                }
                else
                {
                    objcoun.lstReferenceAsyncSS = studlist;
                }
                return PartialView("WebListStudentAsyncSS", objcoun);
            }

        }

        public async Task<ActionResult> TelephonicListStudentAsyncSS(DateTime? Startdate, DateTime? Enddate, string enquiryfor)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor objcoun = new Counsellor();
                objcoun.StartDate = Startdate.Value.Date;
                objcoun.EndDate = Enddate.Value.Date;
                objcoun.StaffCode = Session["StaffCode"].ToString();
                objcoun.BranchCode = Session["BranchCode"].ToString();
                DataSet ds = new DataSet();
                ds = await objbal.TelephonicReferenceListStudentAsyncSS(objcoun);
                List<Counsellor> studlist = new List<Counsellor>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Counsellor objcoun1 = new Counsellor();
                    objcoun1.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objcoun1.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    objcoun1.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                    objcoun1.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                    objcoun1.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                    objcoun1.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
					objcoun1.CourseName = ds.Tables[0].Rows[i]["CourseName"].ToString();
                    objcoun1.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                    objcoun1.AlternateNumber = ds.Tables[0].Rows[i]["AlternateNumber"].ToString();
                    objcoun1.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    objcoun1.EnqStatusId = int.Parse(ds.Tables[0].Rows[i]["Statusid"].ToString());
                    studlist.Add(objcoun1);
                }
                List<Counsellor> filteredlist = new List<Counsellor>();
                if (!string.IsNullOrEmpty(enquiryfor))
                {
                    filteredlist = studlist.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                    objcoun.lstReferenceAsyncSS = filteredlist;
                }
                else
                {
                    objcoun.lstReferenceAsyncSS = studlist;
                }
                return PartialView("TelephonicListStudentAsyncSS", objcoun);
            }

        }

        public async Task<ActionResult> PortalListStudentAsyncSS(DateTime? Startdate, DateTime? Enddate, string enquiryfor)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor objcoun = new Counsellor();
                objcoun.StartDate = Startdate.Value.Date;
                objcoun.EndDate = Enddate.Value.Date;
                objcoun.StaffCode = Session["StaffCode"].ToString();
                objcoun.BranchCode = Session["BranchCode"].ToString();
                DataSet ds = new DataSet();
                ds = await objbal.PortalReferenceListStudentAsyncSS(objcoun);
                List<Counsellor> studlist = new List<Counsellor>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Counsellor objcoun1 = new Counsellor();
                    objcoun1.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objcoun1.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    objcoun1.EquiryDescription = ds.Tables[0].Rows[i]["EquiryDescription"].ToString();
                    objcoun1.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                    objcoun1.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                    objcoun1.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                    objcoun1.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
					objcoun1.CourseName = ds.Tables[0].Rows[i]["CourseName"].ToString();
                    objcoun1.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                    objcoun1.AlternateNumber = ds.Tables[0].Rows[i]["AlternateNumber"].ToString();
                    objcoun1.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    objcoun1.EnqStatusId = int.Parse(ds.Tables[0].Rows[i]["Statusid"].ToString());
                    studlist.Add(objcoun1);
                }
                List<Counsellor> filteredlist = new List<Counsellor>();
                if (!string.IsNullOrEmpty(enquiryfor))
                {
                    filteredlist = studlist.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                    objcoun.lstReferenceAsyncSS = filteredlist;
                }
                else
                {
                    objcoun.lstReferenceAsyncSS = studlist;
                }
                return PartialView("PortalListStudentAsyncSS", objcoun);
            }

        }

        public async Task<ActionResult> AdvertisementListStudentAsyncSS(DateTime? Startdate, DateTime? Enddate, string enquiryfor)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor objcoun = new Counsellor();
                objcoun.StartDate = Startdate.Value.Date;
                objcoun.EndDate = Enddate.Value.Date;
                objcoun.StaffCode = Session["StaffCode"].ToString();
                objcoun.BranchCode = Session["BranchCode"].ToString();
                DataSet ds = new DataSet();
                ds = await objbal.AdvertisementReferenceListStudentAsyncSS(objcoun);
                List<Counsellor> studlist = new List<Counsellor>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Counsellor objcoun1 = new Counsellor();
                    objcoun1.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objcoun1.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    objcoun1.EquiryDescription = ds.Tables[0].Rows[i]["EquiryDescription"].ToString();
                    objcoun1.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                    objcoun1.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                    objcoun1.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                    objcoun1.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
					objcoun1.CourseName = ds.Tables[0].Rows[i]["CourseName"].ToString();
                    objcoun1.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                    objcoun1.AlternateNumber = ds.Tables[0].Rows[i]["AlternateNumber"].ToString();
                    objcoun1.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    objcoun1.EnqStatusId = int.Parse(ds.Tables[0].Rows[i]["Statusid"].ToString());
                    studlist.Add(objcoun1);
                }
                objcoun.lstReferenceAsyncSS = studlist;
                List<Counsellor> filteredlist = new List<Counsellor>();
                if (!string.IsNullOrEmpty(enquiryfor))
                {
                    filteredlist = studlist.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                    objcoun.lstReferenceAsyncSS = filteredlist;
                }
                else
                {
                    objcoun.lstReferenceAsyncSS = studlist;
                }
                return PartialView("AdvertisementListStudentAsyncSS", objcoun);
            }

        }

        public async Task<ActionResult> OtherListStudentAsyncSS(DateTime? Startdate, DateTime? Enddate, string enquiryfor)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor objcoun = new Counsellor();
                objcoun.StartDate = Startdate.Value.Date;
                objcoun.EndDate = Enddate.Value.Date;
                objcoun.StaffCode = Session["StaffCode"].ToString();
                objcoun.BranchCode = Session["BranchCode"].ToString();
                DataSet ds = new DataSet();
                ds = await objbal.OtherReferenceListStudentAsyncSS(objcoun);
                List<Counsellor> studlist = new List<Counsellor>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Counsellor objcoun1 = new Counsellor();
                    objcoun1.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objcoun1.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    objcoun1.EquiryDescription = ds.Tables[0].Rows[i]["EquiryDescription"].ToString();
                    objcoun1.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                    objcoun1.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                    objcoun1.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                    objcoun1.ImportDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["ExcelImportDate"].ToString());
                    objcoun1.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
					objcoun1.CourseName = ds.Tables[0].Rows[i]["CourseName"].ToString();
                    objcoun1.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                    objcoun1.AlternateNumber = ds.Tables[0].Rows[i]["AlternateNumber"].ToString();
                    objcoun1.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    objcoun1.EnqStatusId = int.Parse(ds.Tables[0].Rows[i]["Statusid"].ToString());
                    studlist.Add(objcoun1);
                }
                objcoun.lstReferenceAsyncSS = studlist;
                List<Counsellor> filteredlist = new List<Counsellor>();
                if (!string.IsNullOrEmpty(enquiryfor))
                {
                    filteredlist = studlist.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                    objcoun.lstReferenceAsyncSS = filteredlist;
                }
                else
                {
                    objcoun.lstReferenceAsyncSS = studlist;
                }
                return PartialView("OtherListStudentAsyncSS", objcoun);
            }

        }
		public async Task<ActionResult> ShowAllLeadListStudentMB(DateTime? Startdate, DateTime? Enddate, string enquiryfor)
		{

			if (Session["StaffCode"] == null)
			{
				return RedirectToAction("Login", "Account");
			}
			else
			{
				Counsellor objcoun = new Counsellor();
				objcoun.StartDate = Startdate.Value.Date;
				objcoun.EndDate = Enddate.Value.Date;
				objcoun.StaffCode = Session["StaffCode"].ToString();
				objcoun.BranchCode = Session["BranchCode"].ToString();
				objcoun.EnquirySource = "AllLead";
				DataSet ds = new DataSet();
				ds = await objbal.ShowAllLeadListStudentMB(objcoun);
				List<Counsellor> studlist = new List<Counsellor>();
				for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
				{
					Counsellor objcoun1 = new Counsellor();
					objcoun1.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
					objcoun1.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
					objcoun1.EquiryDescription = ds.Tables[0].Rows[i]["EquiryDescription"].ToString();
					objcoun1.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
					objcoun1.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
					objcoun1.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
					objcoun1.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
					objcoun1.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
					objcoun1.AlternateNumber = ds.Tables[0].Rows[i]["AlternateNumber"].ToString();
					objcoun1.CourseName = ds.Tables[0].Rows[i]["CourseName"].ToString();
					objcoun1.EnquirySource = ds.Tables[0].Rows[i]["SourceName"].ToString();
					objcoun1.Status = ds.Tables[0].Rows[i]["Status"].ToString();
					objcoun1.EnqStatusId = int.Parse(ds.Tables[0].Rows[i]["Statusid"].ToString());
					studlist.Add(objcoun1);
				}
				List<Counsellor> filteredlist = new List<Counsellor>();
				if (!string.IsNullOrEmpty(enquiryfor))
				{
					filteredlist = studlist.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
					objcoun.lstReferenceAsyncSS = filteredlist;
				}
				else
				{
					objcoun.lstReferenceAsyncSS = studlist;
				}
				return PartialView("WebListStudentAsyncSS", objcoun);
			}

		}
        public async Task<ActionResult> WalkinReferenceStudentListAsyncSS(DateTime? Startdate, DateTime? Enddate, string enquiryfor)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor objcoun = new Counsellor();
                objcoun.StartDate = Startdate.Value.Date;
                objcoun.EndDate = Enddate.Value.Date;
                objcoun.StaffCode = Session["StaffCode"].ToString();
                objcoun.BranchCode = Session["BranchCode"].ToString();
                DataSet ds = new DataSet();
                ds = await objbal.WalkinStudentListAsyncSS(objcoun);
                List<Counsellor> studlist = new List<Counsellor>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Counsellor objcoun1 = new Counsellor();
                    objcoun1.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objcoun1.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    objcoun1.EquiryDescription = ds.Tables[0].Rows[i]["EquiryDescription"].ToString();
                    objcoun1.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                    objcoun1.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                    objcoun1.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                    objcoun1.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
					objcoun1.CourseName = ds.Tables[0].Rows[i]["CourseName"].ToString();
                    objcoun1.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                    objcoun1.AlternateNumber = ds.Tables[0].Rows[i]["AlternateNumber"].ToString();
                    objcoun1.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    objcoun1.EnqStatusId = int.Parse(ds.Tables[0].Rows[i]["Statusid"].ToString());
                    studlist.Add(objcoun1);
                }
                objcoun.lstReferenceAsyncSS = studlist;
                List<Counsellor> filteredlist = new List<Counsellor>();
                if (!string.IsNullOrEmpty(enquiryfor))
                {
                    filteredlist = studlist.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
                    objcoun.lstReferenceAsyncSS = filteredlist;
                }
                else
                {
                    objcoun.lstReferenceAsyncSS = studlist;
                }
                return PartialView("WalkinReferenceStudentListAsyncSS", objcoun);
            }

        }

        [HttpGet]
        public async Task<JsonResult> GetBranchNameSS()
        {
            DataSet ds = new DataSet();
            ds = await objbal.GETBranchNameSS();
            List<SelectListItem> StatusList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StatusList.Add(new SelectListItem
                {
                    Text = dr["BranchName"].ToString(),
                    Value = dr["BranchCode"].ToString()
                });
            }
            ViewBag.EnquiryForStatus = StatusList;
            return Json(StatusList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> UpdateStudentReferanceSS(string CandidateCode)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor objco = new Counsellor();
                objco.StaffCode = Session["StaffCode"].ToString();
                objco.BranchCode = Session["BranchCode"].ToString();
                objco.CandidateCode = CandidateCode;
                SqlDataReader dr;
                dr = await objbal.FetchStudentData(objco);
                while (dr.Read())
                {
                    objco.CandidateCode = dr["CandidateCode"].ToString();
                    objco.EquiryDescription = dr["EquiryDescription"].ToString();
                    objco.FullName = dr["FullName"].ToString();
                    objco.Gender = dr["Gender"].ToString();
                    objco.PresentAddress = dr["PresentAddress"].ToString();
                    objco.PresentPincode = dr["PresentPincode"].ToString();
                    objco.EnquiryFor = dr["EnquiryFor"].ToString();
                    objco.EnquiryDate = Convert.ToDateTime(dr["EnquiryDate"].ToString());
                    objco.EmailId = dr["EmailId"].ToString();
                    objco.ContactNumber = dr["ContactNumber"].ToString();
                    objco.AlternateNumber = dr["AlternateNumber"].ToString();
                    objco.SourceName = dr["SourceName"].ToString();
                    objco.Status = dr["Status"].ToString();
                    objco.BranchCode = dr["BranchName"].ToString();
                }
                dr.Close();
                return PartialView(objco);
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdatestudentReferanceSS(Counsellor objco)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                objco.StaffCode = Session["StaffCode"].ToString();
                objco.BranchCode = Session["BranchCode"].ToString();
                await objbal.UpdateReferanceSS(objco);
                return RedirectToAction("ImportExcelFile");
            }
        }

        public async Task<JsonResult> GetEnquiryForStatusAsyncST()
        {
            DataSet ds = new DataSet();
            ds = await objbal.EnquiryForStatusBind();
            List<SelectListItem> StatusList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StatusList.Add(new SelectListItem
                {
                    Text = dr["Status"].ToString(),
                    Value = dr["StatusId"].ToString()
                });
            }
            ViewBag.EnquiryForStatus = StatusList;
            return Json(StatusList, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetStatusAsyncST()
        {
            DataSet ds = new DataSet();
            ds = await objbal.BindEnquiryStatusNameSS();
            List<SelectListItem> StatusList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StatusList.Add(new SelectListItem
                {
                    Text = dr["Status"].ToString(),
                    Value = dr["StatusId"].ToString()
                });
            }
            ViewBag.Status = StatusList;
            return Json(StatusList, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetProfessionalStatusAsyncSS()
        {
            DataSet ds = new DataSet();
            ds = await objbal.BindProfessionalStatusNameSS();
            List<SelectListItem> StatusList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StatusList.Add(new SelectListItem
                {
                    Text = dr["Status"].ToString(),
                    Value = dr["StatusId"].ToString()
                });
            }
            ViewBag.ProfessionalStatus = StatusList;
            return Json(StatusList, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetEnquerySourceAsyncSS()
        {
            DataSet ds = new DataSet();
            ds = await objbal.GetSourceNameSS();
            List<SelectListItem> StatusList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StatusList.Add(new SelectListItem
                {
                    Text = dr["SourceName"].ToString(),
                    Value = dr["SourceId"].ToString()
                });
            }
            ViewBag.EnquerySource = StatusList;
            return Json(StatusList, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SelectEnquiryforName()
        {
            DataSet ds = new DataSet();
            ds = await objbal.EnquiryforName();
            List<SelectListItem> StatusList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StatusList.Add(new SelectListItem
                {
                    Text = dr["EnquiryFor"].ToString(),
                    Value = dr["EnquiryFor"].ToString()
                });
            }
            ViewBag.Enqueryfor = StatusList;
            return Json(StatusList, JsonRequestBehavior.AllowGet);
        }

       [HttpGet]
		 public async Task<ActionResult> RegisterNewLeadsSS()
		 {
			 if (Session["StaffCode"] == null)
			 {
				 return RedirectToAction("Login", "Account");
			 }
			 else
			 {
				 await GetEnquerySourceAsyncSS();
				 await SelectEnquiryforName();
				 await GetInterestedCoursePK();
				 Counsellor ObjCo = new Counsellor();
				 ObjCo.StaffCode = Session["StaffCode"].ToString();
				 ObjCo.BranchCode = Session["BranchCode"].ToString();
				 return PartialView(ObjCo);
			 }
		 }

        [HttpPost]
        public async Task<ActionResult> RegisterNewLeadsSS(Counsellor objcoun)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor obj = new Counsellor();
                obj.EnquiryFor = objcoun.EnquiryFor;
                await GetMaxCandidateCount(obj);
                objcoun.StaffCode = Session["StaffCode"].ToString();
                objcoun.BranchCode = Session["BranchCode"].ToString();
                objcoun.CandidateCode = TempData["CandidateCode"] as string;                
                await Task.Run(() => objbal.AddLeadEnquirySave(objcoun));
                if (objcoun.isCheck == true)
                {
                    await objbal.UpdateEnqStatusOnEnquiryFormLinkSent(objcoun);                    
                    string MailType = "EnquiryForm";
                    await SendMailkk(objcoun.EmailId, objcoun.FullName, MailType, objcoun.CandidateCode, 78, objcoun.EnquiryFor, objcoun.Password);
                }
                return RedirectToAction("ImportExcelFile");
            }

        }

        [HttpGet]
        public async Task<JsonResult> SelectBatchName()
        {
            DataSet ds = new DataSet();
            ds = await objbal.GetTypeBatchName();
            List<SelectListItem> BatchList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                BatchList.Add(new SelectListItem
                {
                    Text = dr["TypeName"].ToString(),
                    Value = dr["TypeId"].ToString()
                });
            }
            ViewBag.TypeBatchName = BatchList;
            return Json(BatchList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> SelectCourseName(Counsellor ObjCo)
        {
            ObjCo.BranchCode = Session["BranchCode"].ToString();
            DataSet ds = new DataSet();
            ds = await objbal.GetCourseNameHP(ObjCo);
            List<SelectListItem> coursenamelist = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                coursenamelist.Add(new SelectListItem
                {
                    Text = dr["CourseName"].ToString(),
                    Value = dr["Coursecode"].ToString()
                });
            }
            ViewBag.CourseName = coursenamelist;
            return Json(coursenamelist, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public async Task<JsonResult> SelectStudentReference(string Reference, Counsellor ObjCo)
        {
            ObjCo.BranchCode = Session["BranchCode"].ToString();
            DataSet ds = new DataSet();
            ds = await objbal.SelectCandidateForReference(ObjCo);
            List<SelectListItem> StatusList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StatusList.Add(new SelectListItem
                {
                    Text = dr["FullName"].ToString(),
                    Value = dr["CandidateCode"].ToString()
                });
            }
            return Json(StatusList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> SelectStaffReference(string Reference, Counsellor ObjCo)
        {
            ObjCo.BranchCode = Session["BranchCode"].ToString();
            DataSet ds = new DataSet();
            ds = await objbal.SelectStaffForReference(ObjCo);
            List<SelectListItem> StatusList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StatusList.Add(new SelectListItem
                {
                    Text = dr["StaffName"].ToString(),
                    Value = dr["StaffCode"].ToString()
                });
            }
            return Json(StatusList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> CounsellorCreateBatch()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor ObjCo = new Counsellor();
                ObjCo.StaffCode = Session["StaffCode"].ToString();
                ObjCo.BranchCode = Session["BranchCode"].ToString();
                await SelectBatchName();
                await SelectCourseName(ObjCo);
                return PartialView(ObjCo);
            }
        }
        [HttpPost]
        public async Task<ActionResult> CounsellorCreateBatch(Counsellor objcoun)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor obj = new Counsellor();              
                objcoun.StaffCode = Session["StaffCode"].ToString();
                objcoun.BranchCode = Session["BranchCode"].ToString();                
                await Task.Run(() => objbal.CreateNewBatchCounsellorHP(objcoun));
                return RedirectToAction("MainDashboardAsyncHP");
            }
        }
        [HttpGet]
        public async Task<ActionResult> ViewStudentDetailsSS(string CandidateCode)
        {
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Counsellor objshow = new Counsellor();
                objshow.StaffCode = Session["StaffCode"].ToString();
                objshow.BranchCode = Session["BranchCode"].ToString();
                objshow.CandidateCode = CandidateCode;
                SqlDataReader dr;
                dr = await objbal.ViewStudentDetailsInfo(objshow);

                while (dr.Read())
                {
                    objshow.FullName = dr["FullName"].ToString();
                    objshow.Gender = dr["Gender"].ToString();
                    objshow.PresentAddress = dr["PresentAddress"].ToString();
                    objshow.PresentPincode = dr["PresentPincode"].ToString();
                    objshow.ContactNumber = dr["ContactNumber"].ToString();
                    objshow.AlternateNumber = dr["AlternateNumber"].ToString();
                    objshow.EmailId = dr["EmailId"].ToString();
                    objshow.EnquirySource = dr["SourceName"].ToString();
                    objshow.EquiryDescription = dr["EquiryDescription"].ToString();
                    objshow.EnquiryDate = Convert.ToDateTime(dr["EnquiryDate"].ToString());
                    objshow.EnquiryFor = dr["EnquiryFor"].ToString();
                    objshow.EnquiryAddedStaff = dr["StaffName"].ToString();
                    objshow.Status = dr["EnquiryStatus"].ToString();
                    objshow.BranchCode = dr["BranchName"].ToString();
                    objshow.RefBy = dr["RefBy"].ToString();
                    objshow.RefName = dr["RefName"].ToString();
                }
                dr.Close();
                return PartialView("ViewStudentDetailsSS", objshow);
            }
        }

        //Sarang Placement Start



        [HttpGet]
        /// <summary>
        /// Placement new Enquiry Form
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> PlacementEnquiryFormSks()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Name = " Counsellor Dashboard", Url =Url.Action("MainDashboardAsyncHP","Counsellor")},
                    new BreadcrumbItem { Name = "Placement Enquiry", Url =Url.Action("EnquiryListSks","Counsellor") },
                    new BreadcrumbItem { Name = " Add Placement Enquiry", Url =Url.Action("EnquiryListSks","Counsellor") }
                };

                ViewBag.Breadcrumbs = breadcrumbs;
                Counsellor ObjCo = new Counsellor();
                ObjCo.StaffCode = Session["StaffCode"].ToString();
                ObjCo.BranchCode = Session["BranchCode"].ToString();

                await GetGraduationSK();
                await GetSourceName();
                await professionalstatus();
                await FetchPermanantCountryName();
                await FetchCountryName();
                await Designation();
                await Department();
                return View();

            }
        }
        [HttpPost]
        public async Task<ActionResult> PlacementEnquiryFormSks(Counsellor obj)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                // try
                //{
                //counsellor objCoun = new Counsellor();
                obj.EnquiryFor = "Placement";
                await GetMaxCandidateCount(obj);
                obj.StaffCode = Session["StaffCode"].ToString();
                obj.BranchCode = Session["BranchCode"].ToString();
                obj.CandidateCode = TempData["CandidateCode"] as string;
                await Task.Run(() => objbal.PlacementNewEnquiry(obj));
                // await objbal.PlacementNewEnquiry(obj);

                SqlDataReader dr;
                dr = await objbal.LastCandidateCode(obj);
                while (dr.Read())
                {
                    obj.CandidateCode = (dr["CandidateCode"].ToString());
                }
                await objbal.Saveinexperience(obj);


                await objbal.Saveinqualification(obj);


                return RedirectToAction("EnquiryListSks");


            }
        }

        /// <summary>
        /// Enquiry main list View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> EnquiryListSks(DateTime? Startdate, DateTime? Enddate, string status)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor ojj = new Counsellor();

                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Name = " Counsellor Dashboard", Url =Url.Action("MainDashboardAsyncHP","Counsellor")},
                    new BreadcrumbItem { Name = "Placement Enquiry", Url =Url.Action("EnquiryListSks","Counsellor") }
                };

                ViewBag.Breadcrumbs = breadcrumbs;

                return PartialView("EnquiryListSks");
            }
        }
        /// <summary>
        /// Enquiry main list View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> PlacementEnquiryMainListAndFiltered(DateTime? StartDate, DateTime? EndDate, string Status)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor objcoord = new Counsellor();
                objcoord.StaffCode = Session["StaffCode"].ToString();
                objcoord.BranchCode = Session["BranchCode"].ToString();

                if (StartDate != null && EndDate != null)
                {
                    objcoord.StartDate = StartDate.Value.Date;
                    objcoord.EndDate = EndDate.Value.Date;
                }
                else
                {
                    objcoord.StartDate = DateTime.Now.AddYears(-1);
                    objcoord.EndDate = DateTime.Now.AddYears(-1);
                }

                DataSet ds = new DataSet();
                ds = await objbal.EnquiryListView(objcoord);
                List<Counsellor> lstshowuser = new List<Counsellor>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Counsellor objcoord1 = new Counsellor();
                    objcoord1.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objcoord1.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    objcoord1.EnquiryId = Convert.ToInt32(ds.Tables[0].Rows[i]["EnquiryId"].ToString());
                    objcoord1.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                    objcoord1.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                    objcoord1.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                    objcoord1.EnqStatusId = Convert.ToInt32(ds.Tables[0].Rows[i]["Statusid"].ToString());
                    objcoord1.Status = ds.Tables[0].Rows[i]["studentstatus"].ToString();
                    lstshowuser.Add(objcoord1);
                }

                List<Counsellor> filteredlist = new List<Counsellor>();
                if (StartDate != null && EndDate != null && Status == "")
                {
                    DateTime startDateValue = StartDate.Value.Date.AddDays(-1);
                    DateTime endDateValue = EndDate.Value.Date.AddDays(+1);
                    filteredlist = lstshowuser.Where(e => e.EnquiryDate != null && e.EnquiryDate > startDateValue && e.EnquiryDate < endDateValue).ToList();
                    objcoord.lstReferenceAsyncSS = filteredlist;
                }
                else if (StartDate != null && EndDate != null)
                {
                    DateTime startDateValue = StartDate.Value.Date.AddDays(-1);
                    DateTime endDateValue = EndDate.Value.Date.AddDays(+1);
                    if (Status == "Enquire" || Status == "Hold" || Status == "Prospective" || Status == "Enquiry" || Status == "Enquiry Form Pending")
                    {
                        filteredlist = lstshowuser.Where(e => e.EnquiryDate != null && e.EnquiryDate > startDateValue && e.EnquiryDate < endDateValue && e.Status == Status).ToList();
                        objcoord.lstReferenceAsyncSS = filteredlist;
                    }
                }
                else
                {
                    objcoord.lstReferenceAsyncSS = lstshowuser;
                }

                return PartialView("PlacementEnquiryMainListAndFiltered", objcoord);
            }
        }


        [HttpPost]
        public async Task<ActionResult> UpdateEnquirySks(Counsellor obj)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {

                obj.BranchCode = Session["BranchCode"].ToString();

                await objbal.UpdateEnquiry(obj);
                await objbal.UpdateExperience(obj);
                await objbal.UpdateQualification(obj);
                
                if (obj.EnqStatusId == 78 || obj.EnqStatusId == 63)
                {
                    return Json(new { success = true, message = "Enquiry Added Successfully " });
                }
                else
                {
                    return Json(new { success = true, message = "Enquiry Updated Successfully" });
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // Return an error view or message
                //return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while updating enquiry.");

                if (obj.EnqStatusId == 78 || obj.EnqStatusId == 63)
                {
                    return Json(new { success = true, message = "Error Adding Details" });
                }
                else
                {
                    return Json(new { success = true, message = "Error Updating Details" });
                }
            }

        }
        [HttpGet]
        /// <summary>
        /// Enquiry Update data
        /// </summary>
        /// <param name="CandidateCode"></param>
        /// <returns></returns>
        public async Task<ActionResult> UpdateEnquirySks(string CandidateCode, int? enqStatusId)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Name = " Counsellor Dashboard", Url =Url.Action("MainDashboardAsyncHP","Counsellor")},
                    new BreadcrumbItem { Name = "Placement Enquiry", Url =Url.Action("EnquiryListSks","Counsellor") },
                    new BreadcrumbItem { Name = "Update Placement Enquiry", Url =Url.Action("EnquiryListSks","Counsellor") }
                };

                ViewBag.Breadcrumbs = breadcrumbs;
                Counsellor ObjCounsellor = new Counsellor();

                try
                {
                    await GetGraduationSK();
                    await GetSourceName();
                    await professionalstatus();
                    await FetchPermanantCountryName();
                    await FetchCountryName();
                    await Designation();
                    await Department();

                    //ObjCounsellor.BranchCode = Session["BranchCode"].ToString();
                    ObjCounsellor.CandidateCode = CandidateCode;

                    if (enqStatusId == 65)
                    {

                        using (SqlDataReader dr = await objbal.FechEnquiryList(ObjCounsellor))
                        {
                            if (dr.HasRows)
                            {
                                while (await dr.ReadAsync())
                                {
                                    ObjCounsellor.CandidateCode = dr["CandidateCode"].ToString();
                                    ObjCounsellor.FullName = dr["FullName"].ToString();
                                    ObjCounsellor.Gender = dr["Gender"].ToString();
                                    ObjCounsellor.PresentAddress = dr["PresentAddress"].ToString();
                                    ObjCounsellor.PresentPincode = dr["PresentPincode"].ToString();
                                    ObjCounsellor.CountryId = Convert.ToInt32(dr["PresentCountryId"]);
                                    ObjCounsellor.StateId = Convert.ToInt32(dr["PresentStateId"]);
                                    ObjCounsellor.PresentCityId = Convert.ToInt32(dr["PresentCityId"]);
                                    ObjCounsellor.PresentCity = dr["PresentCity"].ToString();
                                    ObjCounsellor.ContactNumber = dr["ContactNumber"].ToString();
                                    ObjCounsellor.AlternateNumber = dr["AlternateNumber"].ToString();
                                    ObjCounsellor.PermanentAddress = dr["PermanantAddress"].ToString();
                                    ObjCounsellor.PermanentPincode = dr["PermanantPincode"].ToString();
                                    ObjCounsellor.PermanentCountryId = Convert.ToInt32(dr["PermanantCountryId"]);
                                    ObjCounsellor.PermanentStateId = Convert.ToInt32(dr["PermanantStateId"]);
                                    ObjCounsellor.PermanantCityId = Convert.ToInt32(dr["permanantCityId"]);
                                    ObjCounsellor.PermanantCity = dr["PermanentCity"].ToString();
                                    ObjCounsellor.EmailId = dr["EmailId"].ToString();
                                    ObjCounsellor.ProfessionalStatus = dr["ProfessionalStatus"].ToString();
                                    ObjCounsellor.ProfessionalStatusId = Convert.ToInt32(dr["ProfessionalStatusId"].ToString());
                                    ObjCounsellor.EnquirySource = dr["SourceName"].ToString();
                                    ObjCounsellor.EquiryDescription = dr["EquiryDescription"].ToString();
                                    ObjCounsellor.EnquirySourceId = Convert.ToInt32(dr["EnquirySourceId"].ToString());
                                    ObjCounsellor.EnquiryDate = Convert.ToDateTime(dr["EnquiryDate"]);
                                    ObjCounsellor.EnquiryFor = dr["EnquiryFor"].ToString();
                                    ObjCounsellor.EnquiryAddedStaffCode = dr["EnquiryAddedStaffCode"].ToString();
                                    ObjCounsellor.CompanyName = dr["CompanyName"].ToString();
                                    ObjCounsellor.Department = dr["DepartmentName"].ToString();
                                    ObjCounsellor.DepartmentId = Convert.ToInt32(dr["DepartmentId"].ToString());
                                    ObjCounsellor.Designation = dr["DesignationName"].ToString();
                                    ObjCounsellor.DesignationId = Convert.ToInt32(dr["DesignationId"].ToString());
                                    ObjCounsellor.CTC = dr["CTC"].ToString();
                                    ObjCounsellor.ExpectedSalary = dr["ExpectedSalery"].ToString();
                                    ObjCounsellor.Graduation = dr["GraduationName"].ToString();
                                    ObjCounsellor.GraduationId = Convert.ToInt32(dr["GraduationId"].ToString());
                                    ObjCounsellor.PostGraduation = dr["PostGraduationName"].ToString();
                                    ObjCounsellor.PostgraduationId = Convert.ToInt32(dr["PostGraduationId"].ToString());
                                }
                            }
                        }

                        ViewBag.PreCountryId = ObjCounsellor.CountryId;
                        ViewBag.PreStateId = ObjCounsellor.StateId;
                        ViewBag.PreCityId = ObjCounsellor.PresentCityId;
                        ViewBag.PrmCountryId = ObjCounsellor.PermanentCountryId;
                        ViewBag.PrmStateId = ObjCounsellor.PermanentStateId;
                        ViewBag.PrmCityId = ObjCounsellor.PermanantCityId;
                        ViewBag.proProfessionalStatusId = ObjCounsellor.ProfessionalStatusId;
                        ViewBag.enqEnquirySourceId = ObjCounsellor.EnquirySourceId;
                        ViewBag.graGraduation = ObjCounsellor.GraduationId;
                        ViewBag.poPostGraduationId = ObjCounsellor.PostgraduationId;
                        ViewBag.DepartmentId = ObjCounsellor.DepartmentId;
                        ViewBag.DesignationId = ObjCounsellor.DesignationId;
                        ViewBag.Enquiryfor = ObjCounsellor.EnquiryFor;

                        await GetStateNameSk(ObjCounsellor.CountryId);
                        await GetPermanantStateNameSk(ObjCounsellor.PermanentCountryId);
                        await GetCityNameSk(ObjCounsellor.StateId);
                        await GetPermanantCityNameSk(ObjCounsellor.PermanentStateId);
                        await GetPostGraduationSK(ObjCounsellor.PostgraduationId);
                    }
                    if (enqStatusId == 63 || enqStatusId == 78) 
                    {

                        using (SqlDataReader dr = await objbal.FetchStudentData(ObjCounsellor))
                        {
                            if (dr.HasRows)
                            {
                                while (await dr.ReadAsync())
                                {
                                    ObjCounsellor.CandidateCode = dr["CandidateCode"].ToString();
                                    ObjCounsellor.FullName = dr["FullName"].ToString();
                                    ObjCounsellor.Gender = dr["Gender"].ToString();
                                    ObjCounsellor.PresentAddress = dr["PresentAddress"].ToString();
                                    ObjCounsellor.PresentPincode = dr["PresentPincode"].ToString();
                                    ObjCounsellor.ContactNumber = dr["ContactNumber"].ToString();
                                    ObjCounsellor.AlternateNumber = dr["AlternateNumber"].ToString();
                                    ObjCounsellor.EnqStatusId = int.Parse(dr["Statusid"].ToString());
                                    ObjCounsellor.EmailId = dr["EmailId"].ToString();
                                    ObjCounsellor.EnquirySource = dr["SourceName"].ToString();
                                    ObjCounsellor.EquiryDescription = dr["EquiryDescription"].ToString();
                                    ObjCounsellor.EnquiryDate = Convert.ToDateTime(dr["EnquiryDate"]);
                                    ObjCounsellor.EnqStatusId = Convert.ToInt32(dr["Statusid"]);
                                    ObjCounsellor.EnquiryFor = dr["EnquiryFor"].ToString();
                                    //ObjCounsellor.EnquiryAddedStaffCode = dr["EnquiryAddedStaffCode"].ToString();
                                }
                            }
                        }


                    }
                }
                catch (Exception ex)
                {
                    // Log the exception
                    // Return an error view or message
                    //return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while fetching enquiry details.");
                }

                return View(ObjCounsellor);

            }
        }
        /// <summary>
        /// Placemnet Enquriry Details
        /// </summary>
        /// <param name="CandidateCode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> DetailsPlacemnetEnqurirySks(string CandidateCode)
        {
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Counsellor ObjDtl = new Counsellor();
                ObjDtl.StaffCode = Session["StaffCode"].ToString();
                ObjDtl.BranchCode = Session["BranchCode"].ToString();
                ObjDtl.CandidateCode = CandidateCode;
                SqlDataReader dr;
                dr = await objbal.DetailsPlacemnetEnqurirySks(ObjDtl);
                while (dr.Read())
                {
                    ObjDtl.FullName = dr["FullName"].ToString();
                    ObjDtl.Gender = dr["Gender"].ToString();
                    ObjDtl.ContactNumber = dr["ContactNumber"].ToString();
                    ObjDtl.EnquiryDate = Convert.ToDateTime(dr["EnquiryDate"].ToString());
                    ObjDtl.EmailId = dr["Emailid"].ToString();
                    ObjDtl.PresentCity = dr["PresentCity"].ToString();
                    ObjDtl.PresentAddress = dr["PresentAddress"].ToString();
                    ObjDtl.PresentPincode = dr["PresentPincode"].ToString();
                    ObjDtl.PermanentAddress = dr["PermanantAddress"].ToString();
                    ObjDtl.PermanentPincode = dr["PermanantPincode"].ToString();
                    ObjDtl.PermanantCity = dr["PermanantCity"].ToString();
                    ObjDtl.Experience = dr["Experience"].ToString();
                    ObjDtl.CompanyName = dr["CompanyName"].ToString();
                    ObjDtl.SourceName = dr["SourceName"].ToString();
                    ObjDtl.EnquiryAddedStaff = dr["StaffName"].ToString();
                }
                dr.Close();
                return PartialView("DetailsPlacemnetEnqurirySks", ObjDtl);
            }
        }


        public async Task FetchCountryName()
        {
            DataSet ds = await objbal.FetchCountryName();
            List<SelectListItem> selectLists = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                selectLists.Add(new SelectListItem
                {
                    Text = dr["CountryName"].ToString(),
                    Value = dr["CountryId"].ToString()
                });
            }
            ViewBag.Country = selectLists;

        }
        [HttpPost]
        public async Task<JsonResult> GetStateNameSk(int CountryId)
        {
            Counsellor ObjCo = new Counsellor();
            ObjCo.CountryId = CountryId;
            DataSet ds = new DataSet();
            //ObjCo.CountryId = Session["CountryId"].ToString();
            ds = await objbal.GetStateNameSk(ObjCo);
            List<SelectListItem> StateList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StateList.Add(new SelectListItem
                {
                    Text = dr["stateName"].ToString(),
                    Value = dr["stateId"].ToString()
                });
            }
            ViewBag.state = StateList;
            return Json(StateList, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetCityNameSk(int StateId)
        {
            Counsellor ObjCo = new Counsellor();
            ObjCo.StateId = StateId;
            DataSet ds = new DataSet();
            //ObjCo.CountryId = Session["CountryId"].ToString();
            ds = await objbal.GetCityNameSk(ObjCo);
            List<SelectListItem> CityList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CityList.Add(new SelectListItem
                {
                    Text = dr["CityName"].ToString(),
                    Value = dr["CityId"].ToString()
                });
            }
            ViewBag.city = CityList;
            return Json(CityList, JsonRequestBehavior.AllowGet);
        }

        public async Task FetchPermanantCountryName()
        {
            DataSet ds = await objbal.FetchCountryName();
            List<SelectListItem> selectLists = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                selectLists.Add(new SelectListItem
                {
                    Text = dr["CountryName"].ToString(),
                    Value = dr["CountryId"].ToString()
                });
            }
            ViewBag.PermanatCountry = selectLists;
        }
        [HttpPost]
        public async Task<JsonResult> GetPermanantStateNameSk(int CountryId)
        {
            Counsellor ObjCo = new Counsellor();
            ObjCo.CountryId = CountryId;
            DataSet ds = new DataSet();
            //ObjCo.CountryId = Session["CountryId"].ToString();
            ds = await objbal.GetStateNameSk(ObjCo);
            List<SelectListItem> StateList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StateList.Add(new SelectListItem
                {
                    Text = dr["stateName"].ToString(),
                    Value = dr["StateId"].ToString()
                });
            }
            ViewBag.state = StateList;
            return Json(StateList, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetPermanantCityNameSk(int StateId)
        {
            Counsellor ObjCo = new Counsellor();
            ObjCo.StateId = StateId;
            DataSet ds = new DataSet();
            //ObjCo.CountryId = Session["CountryId"].ToString();
            ds = await objbal.GetCityNameSk(ObjCo);
            List<SelectListItem> CityList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CityList.Add(new SelectListItem
                {
                    Text = dr["CityName"].ToString(),
                    Value = dr["CityId"].ToString()
                });
            }
            ViewBag.city = CityList;
            return Json(CityList, JsonRequestBehavior.AllowGet);
        }

        public async Task GetGraduationSK()
        {
            DataSet ds = await objbal.FetchGraduation();
            List<SelectListItem> selectLists1 = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                selectLists1.Add(new SelectListItem
                {
                    Text = dr["GraduationName"].ToString(),
                    Value = dr["GraduationId"].ToString()
                });
            }
            ViewBag.Graduation = selectLists1;
        }


        public async Task<JsonResult> GetPostGraduationSK(int GraduationId)
        {
            Counsellor ObjCo = new Counsellor();
            ObjCo.GraduationId = GraduationId;
            DataSet ds = new DataSet();
            //ObjCo.CountryId = Session["CountryId"].ToString();
            ds = await objbal.FetchPostGraduation(ObjCo);
            List<SelectListItem> PGraduationList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                PGraduationList.Add(new SelectListItem
                {
                    Text = dr["PostGraduationName"].ToString(),
                    Value = dr["PostGraduationId"].ToString()
                });
            }
            ViewBag.PostGraduation = PGraduationList;
            return Json(PGraduationList, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> professionalstatus()
        {
            DataSet ds = new DataSet();
            ds = await objbal.ProfesionalStatus();
            List<SelectListItem> selectLists1 = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                selectLists1.Add(new SelectListItem
                {
                    Text = dr["Status"].ToString(),
                    Value = dr["StatusId"].ToString()
                });
            }
            ViewBag.Professional = selectLists1;
            return Json(selectLists1, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> Designation()
        {
            DataSet ds = new DataSet();
            ds = await objbal.DesignationStatus();
            List<SelectListItem> DesignationList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DesignationList.Add(new SelectListItem
                {
                    Text = dr["DesignationName"].ToString(),
                    Value = dr["DesignationId"].ToString()
                });
            }
            ViewBag.Designation = DesignationList;
            return Json(DesignationList, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> Department()
        {
            DataSet ds = new DataSet();
            ds = await objbal.DepartmentStatus();
            List<SelectListItem> DepartmentList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DepartmentList.Add(new SelectListItem
                {
                    Text = dr["DepartmentName"].ToString(),
                    Value = dr["DepartmentId"].ToString()
                });
            }
            ViewBag.Department = DepartmentList;
            return Json(DepartmentList, JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> GetSourceName()
        {
            DataSet ds = new DataSet();
            ds = await objbal.SourceName();
            List<SelectListItem> selectLists1 = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                selectLists1.Add(new SelectListItem
                {
                    Text = dr["SourceName"].ToString(),
                    Value = dr["SourceId"].ToString()
                });
            }
            ViewBag.Source = selectLists1;
            return Json(selectLists1, JsonRequestBehavior.AllowGet);
        }


        //-------------------------BALAJI----------------------------//

        [HttpGet]
        public async Task<ActionResult> CourseDetailsmainfrom()
        {
            if (Session["BranchCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Counsellor obj = new Counsellor();
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Name = "Counsellor Dashboard", Url =Url.Action("MainDashboardAsyncHP","Counsellor")},
                    new BreadcrumbItem { Name = "Course Details", Url =Url.Action("CourseDetailsmainfrom","Counsellor") }
                };

                ViewBag.Breadcrumbs = breadcrumbs;
                return View();
            }
        }

        [HttpGet]
        public async Task<ActionResult> CounsellorCourselst()
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {

                Counsellor obj = new Counsellor();
                obj.BranchCode = Session["BranchCode"].ToString();
                DataSet ds1 = await objbal.GetCounsellorCoursePS(obj);
                List<Counsellor> LstSyllabus = new List<Counsellor>();
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    Counsellor objcouns = new Counsellor();
                    objcouns.SerialNo = Convert.ToInt32 (ds1.Tables[0].Rows[i]["SerialNo"].ToString());

                    objcouns.CourseCode = ds1.Tables[0].Rows[i]["CourseCode"].ToString();
                    objcouns.CourseName = ds1.Tables[0].Rows[i]["CourseName"].ToString();
                    objcouns.SyllabusFile = ds1.Tables[0].Rows[i]["SyllabusFile"].ToString();
                    objcouns.Description = ds1.Tables[0].Rows[i]["Description"].ToString();
                    objcouns.CourseFee = Convert.ToInt32(ds1.Tables[0].Rows[i]["CourseFee"].ToString());
                    objcouns.NoofInstallment = Convert.ToInt32(ds1.Tables[0].Rows[i]["NoOfInstallment"].ToString());
                    objcouns.CourseDuration = ds1.Tables[0].Rows[i]["SecInDay"].ToString();

                    LstSyllabus.Add(objcouns);
                }
                obj.LstSyllabus = LstSyllabus;

                SqlDataReader dr3;
                dr3 = await objbal.BranchNameBS(obj);
                dr3.Read();
                obj.BranchName = (dr3["BranchName"].ToString());
                obj.CenterName = (dr3["CenterName"].ToString());
                obj.ClientName = (dr3["ClientName"].ToString());
                dr3.Close();
                //ViewBag.BranchNameSB = obj.BranchName;
                ViewData["BranchNameSB"] = obj.BranchName;
                ViewBag.CenterNameSB = obj.CenterName;

                ViewBag.ClientNameSB = obj.ClientName;


                return PartialView("CounsellorCourselst", obj);
            }
        }

        [HttpGet]
        public async Task<ActionResult> CourseDetailsViewBroachersBS()
        {
            Counsellor objt = new Counsellor();
            DataSet ds;
            ds = await objbal.ReadBroachersBS(objt);
            List<Counsellor> LstSyllabus = new List<Counsellor>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Counsellor obj11 = new Counsellor();

                obj11.BroachersId = ds.Tables[0].Rows[i]["BroachersId"].ToString();
                obj11.BroachersName = ds.Tables[0].Rows[i]["BroachersName"].ToString();
                obj11.NameBroachersFile = ds.Tables[0].Rows[i]["NameBroachersFile"].ToString();
                LstSyllabus.Add(obj11);
            }
            objt.LstSyllabus = LstSyllabus;
            return PartialView("CourseDetailsViewBroachersBS", objt);
        }

        //----------------------------Ketan Kamble------------------------------------//
        [HttpGet]
        public async Task<ActionResult> AdmissionDetails()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor ojj = new Counsellor();
                await GetInterestedCoursePK();
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Name = "Counsellor Dashboard", Url =Url.Action("MainDashboardAsyncHP","Counsellor")},
                    new BreadcrumbItem { Name = "Admission", Url =Url.Action("AdmissionDetails","Counsellor") }
                };

                ViewBag.Breadcrumbs = breadcrumbs;
                return View("AdmissionDetails");
            }

        }

        [HttpGet]
        public async Task<JsonResult> GetBankName(Counsellor ObjCo)
        {
            ObjCo.BranchCode = Session["BranchCode"].ToString();
            DataSet ds = new DataSet();
            ds = await objbal.FetchBankName(ObjCo);
            List<SelectListItem> selectLists = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                selectLists.Add(new SelectListItem
                {
                    Text = dr["BankName"].ToString(),
                    Value = dr["BankId"].ToString()
                });
            }
            ViewBag.BankName = selectLists;
            return Json(selectLists, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
       public async Task<ActionResult> internaladdmissionKK(DateTime? Startdate, DateTime? Enddate, string SelectedCourseFilter)
{
    if (Session["StaffCode"] == null)
    {
        return await Task.Run(() => RedirectToAction("Login", "Account"));
    }
    else
    {
        Counsellor objdetials = new Counsellor();
        objdetials.BranchCode = Session["BranchCode"].ToString();
        objdetials.StartDate = Startdate.Value.Date;
        objdetials.EndDate = Enddate.Value.Date;
        DataSet ds = await objbal.ShowinternaladdmissionKK(objdetials);

        //ds = await objbal.ShowinternaladdmissionKK( obj1);

        List<Counsellor> lstuser = new List<Counsellor>();
        //if (ds != null && ds.Tables[0].Rows.Count > 0)
        //{
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            Counsellor objcoun = new Counsellor();
            objcoun.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNumber"].ToString());

            objcoun.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
            objcoun.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
            objcoun.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
            objcoun.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
            objcoun.CourseName = ds.Tables[0].Rows[i]["CourseName"].ToString();
            objcoun.CourseCode = ds.Tables[0].Rows[i]["CourseCode"].ToString();
            objcoun.AdmmissionDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["AdmmissionDate"].ToString());
            objcoun.CourseFee = Convert.ToInt32(ds.Tables[0].Rows[i]["CourseFee"].ToString());
            objcoun.NoofInstallment = Convert.ToInt32(ds.Tables[0].Rows[i]["NoofInstallment"].ToString());
            objcoun.TransactionCount = ds.Tables[0].Rows[i]["TransactionCount"].ToString();
            objcoun.ReciptCode = ds.Tables[0].Rows[i]["FeesCollectioncode"].ToString();
            objcoun.BatchName = ds.Tables[0].Rows[i]["Batches"].ToString();
            objcoun.TentativeBatchStartDate = ds.Tables[0].Rows[i]["TentativeBatchDates"].ToString();

            lstuser.Add(objcoun);
        }
        // objdetials.lstuser = lstuser;
        //}
        List<Counsellor> lstFilteredCourseAdmission = new List<Counsellor>();
        if (Startdate != null && Enddate != null)
        {
            DateTime modifiedStartDate = Startdate.Value.Date;
            DateTime modifiedEndDate = Enddate.Value.Date;
            if (SelectedCourseFilter != null && SelectedCourseFilter != "")
            {
                lstFilteredCourseAdmission = lstuser.Where(item => item.AdmmissionDate > modifiedStartDate && item.AdmmissionDate <= modifiedEndDate && item.CourseCode == SelectedCourseFilter).ToList();
                objdetials.lstuser = lstFilteredCourseAdmission;
            }
            else
            {
                lstFilteredCourseAdmission = lstuser.Where(item => item.AdmmissionDate > modifiedStartDate && item.AdmmissionDate <= modifiedEndDate).ToList();
                objdetials.lstuser = lstFilteredCourseAdmission;
            }
        }
        else
        {
            objdetials.lstuser = lstuser;
        }

        return PartialView("internaladdmissionKK", objdetials);

        //return Json(objdetials, JsonRequestBehavior.AllowGet);
    }
}
        ////Show external addmission
        [HttpGet]

        public async Task<ActionResult> externaladdmissionKK(DateTime? Startdate, DateTime? Enddate, string SelectedCourseFilter)
        {
            Counsellor objdetials2 = new Counsellor();
            objdetials2.BranchCode = Session["BranchCode"].ToString();
            objdetials2.StartDate = Startdate.Value.Date;
            objdetials2.EndDate = Enddate.Value.Date;
            List<Counsellor> lstuser = new List<Counsellor>();

            DataSet ds = await objbal.ShowexternaladdmissionKK(objdetials2);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Counsellor objcount = new Counsellor();
                    objcount.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNumber"].ToString());

                    objcount.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    objcount.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                    objcount.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                    objcount.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
                    objcount.Experience = ds.Tables[0].Rows[i]["Experience"].ToString();
                    objcount.AdmmissionDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["AdmmissionDate"].ToString());
                    objcount.TotalAmount = Convert.ToInt32(ds.Tables[0].Rows[i]["TransactionAmount"].ToString());
                    objcount.PaymentMode = ds.Tables[0].Rows[i]["PaymentMode"].ToString();
                    objcount.ReciptCode = ds.Tables[0].Rows[i]["FeesCollectioncode"].ToString();
                    objcount.TransactionId = (ds.Tables[0].Rows[i]["TransactionID_checqueNumber"].ToString());

                    lstuser.Add(objcount);
                }
                List<Counsellor> lstFilteredCourseAdmission = new List<Counsellor>();
                if (Startdate != null && Enddate != null)
                {
                    DateTime modifiedStartDate = Startdate.Value.Date.AddDays(-1);
                    DateTime modifiedEndDate = Enddate.Value.Date.AddDays(+1);
                    lstFilteredCourseAdmission = lstuser.Where(item => item.AdmmissionDate > modifiedStartDate && item.AdmmissionDate <= modifiedEndDate).ToList();
                    objdetials2.lstuser = lstFilteredCourseAdmission;
                }
                else
                {
                    objdetials2.lstuser = lstuser;
                }
            }
            return PartialView("externaladdmissionKK", objdetials2);

        }
        /// <summary>
        /// /////internal admission form////////////
        /// </summary>
        /// <returns></returns> 


        public async Task<JsonResult> GetTentativeBatches(string courseCode)
        {
            Counsellor ObjCo = new Counsellor();
            ObjCo.CourseCode = courseCode;
            DataSet ds = new DataSet();
            ObjCo.BranchCode = Session["BranchCode"].ToString();
            ds = await objbal.GetTentativeBatchList(ObjCo);
            List<SelectListItem> BatchList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                BatchList.Add(new SelectListItem
                {
                    Text = dr["BatchName"].ToString(),
                    Value = dr["BatchCode"].ToString()
                });
            }
            ViewBag.BatchList = BatchList;
            Session["BatchList"] = BatchList;
            await GetCourseFee(courseCode);
            var CourseFee = ViewBag.CourseFees;
            var result = new
            {
                BatchList = BatchList,
                CourseFee = CourseFee
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<JsonResult> GetStatekk(int CountryId)
        {

            try
            {
                DataSet ds = await objbal.GetState(CountryId);
                DataTable dt = new DataTable();
                dt = ds.Tables[0];
                var jsondata = JsonConvert.SerializeObject(dt);
                return await Task.Run(() => Json(jsondata));
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetCourseFee(string courseCode)
        {
            if (string.IsNullOrEmpty(courseCode))
            {
                return Json(new { success = false, message = "Course code is required." }, JsonRequestBehavior.AllowGet);
            }

            Counsellor ObjCo = new Counsellor
            {
                CourseCode = courseCode,
                BranchCode = Session["BranchCode"].ToString()
            };

            DataSet ds = await objbal.fetchcoursefees(ObjCo);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var courseFee = ds.Tables[0].Rows[0]["CourseFee"].ToString();
                ViewBag.CourseFees = courseFee;
                return Json(new { success = true, courseFee = courseFee }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No data found." }, JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<ActionResult> RecieptList()
        {
            Counsellor objdetials = new Counsellor();
            DataSet ds = new DataSet();
            ds = await objbal.RecieptListkk();

            List<Counsellor> lstuser = new List<Counsellor>();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Counsellor objcounts = new Counsellor();
                    objcounts.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                    objcounts.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                    objcounts.CourseName = ds.Tables[0].Rows[i]["CourseName"].ToString();
                    objcounts.CourseFee = Convert.ToInt32(ds.Tables[0].Rows[i]["CourseFee"].ToString());
                    objcounts.Discount = Convert.ToInt32(ds.Tables[0].Rows[i]["Discount"].ToString());
                    objcounts.TotalAmount = Convert.ToInt32(ds.Tables[0].Rows[i]["Amount"].ToString());
                    objcounts.RecieptDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["ReceiptDate"].ToString());
                    lstuser.Add(objcounts);
                }
                objdetials.lstuser = lstuser;
            }
            return PartialView("RecieptList", objdetials);           
        }


        [HttpGet]
        public async Task<ActionResult> recieptkk(/*Counsellor obj*/  string CandidateCode, string ReciptCode)
        {

            Counsellor objstud = new Counsellor();

            objstud.ReciptCode = ReciptCode; // Corrected parameter name
            objstud.CandidateCode = CandidateCode;
            SqlDataReader dr;
            dr = await objbal.ShowFeeReciptkk(objstud);
            while (dr.Read())
            {
                objstud.ReciptCode = (dr["FeesCollectioncode"].ToString());

                objstud.CandidateCode = (dr["CandidateCode"].ToString());
                objstud.CourseName = (dr["CourseName"].ToString());
                objstud.BatchName = (dr["BatchName"].ToString());
                objstud.FullName = (dr["FullName"].ToString());
                objstud.AdmissionType = (dr["AdmissionType"].ToString());
                objstud.TotalFees = dr["TotalFees"].ToString();
                objstud.PaidFees = Convert.ToInt32(dr["PaidFees"].ToString());
                objstud.RemainingFees = (dr["RemainingFees"].ToString());
                objstud.ReciptCode = dr["FeesCollectioncode"].ToString();
                objstud.CreditAmount = Convert.ToInt32(dr["TransactionAmount"].ToString());               
                objstud.PaymentMode = dr["PaymentMode"].ToString();
                objstud.TransactionId = dr["TransactionID_checqueNumber"].ToString();
                objstud.ReceiptDate = Convert.ToDateTime(dr["FeeCollecteddate"].ToString());
                objstud.Discount = Convert.ToInt32(dr["Discount"].ToString());
                objstud.StaffName = dr["StaffName"].ToString();
            }
            return View(objstud);           
        }


        //////////external addmission form////////////////////
        public async Task<ActionResult> externaladdmissionFormKK()
        {
            ViewBag.Country = await objbal.GetCountry();
            ViewBag.Industry = await objbal.industrykk();
            ViewBag.Department = await objbal.Departmentkk();
            ViewBag.Designation = await objbal.Designationkk();

            ViewBag.Country = await objbal.GetCountry();

            return View();
        }
		
	/*	[HttpPost]
		public ActionResult InternaladdmissionFormKK1(Counsellor obj)
		{
			 return RedirectToAction("AdmissionDetails", "Counsellor");
		}*/
		
		

		

        /// <summary>
        /// Email Sending Action method
        /// </summary>
        /// <param name="EmailId"></param>
        /// <param name="FullName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SendMailkk(string EmailId, string FullName, string mailtype, string candidateCode, int enqstatusid, string enquiryfor, string password)
        {

            if (mailtype == "CourseAdmission")
            {
                if (TentativeDate != null)
                {
                    string senderEmail = "hreducationerp@gmail.com";
                    string senderPassword = "zapg chik ymwk biza";// pass : hjav gexz qbvh alpw hjav gexz qbvh alpw
                    string Subject = "Admission";
                    string Msg = " Congratulation" + FullName + " Your Admission Has been Approved.Now You are Register as a Student. Your Batch Start Date is " + enquiryfor + ". Your login credentials are " + "Email : " + EmailId + " Password : " + password + "." + "  If you have any questions, feel free to reach out to us at " + "9421776337" + ".Thank you!";

                    string smtpHost = "smtp.gmail.com";
                    int smtpPort = 587; // Create a new email message
                    MailMessage mailMessage = new MailMessage(senderEmail, EmailId, Subject, Msg); // Configure the SMTP client

                    SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);
                    smtpClient.EnableSsl = true; // Send the email
                    smtpClient.Send(mailMessage);
                }
                else
                {
                    string senderEmail = "hreducationerp@gmail.com";
                    string senderPassword = "zapg chik ymwk biza";// pass : hjav gexz qbvh alpw hjav gexz qbvh alpw
                    string Subject = "Admission";
                    string Msg = " Congratulation" + FullName + " Your Admission Has been Approved.Now You are Register as a Student. Your login credentials are " + "Email : " + EmailId + " Password : " + password + "." + "  If you have any questions, feel free to reach out to us at " + "9421776337" + ".Thank you!";

                    string smtpHost = "smtp.gmail.com";
                    int smtpPort = 587; // Create a new email message
                    MailMessage mailMessage = new MailMessage(senderEmail, EmailId, Subject, Msg); // Configure the SMTP client

                    SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);
                    smtpClient.EnableSsl = true; // Send the email
                    smtpClient.Send(mailMessage);
                }

            }
            if (mailtype == "PlacementAdmission")
            {
                string senderEmail = "hreducationerp@gmail.com";
                string senderPassword = "zapg chik ymwk biza";// pass : hjav gexz qbvh alpw hjav gexz qbvh alpw
                string Subject = "Admission";
                string Msg = " Congratulation" + FullName + " Your Admission Has been Approved.Now You are Register as a Student. Your login credentials are " + "Email : " + EmailId + " Password : " + password + "." + " If you have any questions, feel free to reach out to us at " + "9421776337" + ".Thank you!";


                string smtpHost = "smtp.gmail.com";
                int smtpPort = 587; // Create a new email message
                MailMessage mailMessage = new MailMessage(senderEmail, EmailId, Subject, Msg); // Configure the SMTP client

                SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);
                smtpClient.EnableSsl = true; // Send the email
                smtpClient.Send(mailMessage);

            }
            if (mailtype == "EnquiryForm")
            {
                string senderEmail = "hreducationerp@gmail.com";
                string senderPassword = "zapg chik ymwk biza";// pass : hjav gexz qbvh alpw hjav gexz qbvh alpw
                string Subject = "Enquiry Form Link";
                string Msg = "Dear " + FullName + ", We are excited to assist you with your enquiry. ";

                if (enquiryfor == "Placement")
                {
                    string actionLink = Url.Action("UpdateSendEnquiryFormSks", "Counsellor", new { CandidateCode = candidateCode, enqStatusId = enqstatusid }, Request.Url.Scheme);
                    Msg += "Please fill out the enquiry form using the following link: " + actionLink + ".This will help us provide you with the most accurate information. If you have any questions, feel free to reach out to us at " + "9421776337" + ".Thank you!";
                }
                if (enquiryfor == "Course")
                {
                    string actionLink = Url.Action("SendUpdateCourseEnquiryFormPK", "Counsellor", new { CandidateCode = candidateCode, enqStatusId = enqstatusid }, Request.Url.Scheme);
                    Msg += "Please fill out the enquiry form using the following link: " + actionLink + ".This will help us provide you with the most accurate information. If you have any questions, feel free to reach out to us at " + "9421776337" + ".Thank you!";
                }
                string smtpHost = "smtp.gmail.com";
                int smtpPort = 587; // Create a new email message
                MailMessage mailMessage = new MailMessage(senderEmail, EmailId, Subject, Msg); // Configure the SMTP client

                SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);
                smtpClient.EnableSsl = true; // Send the email
                smtpClient.Send(mailMessage);
            }
            return await Task.Run(() => RedirectToAction("InternaladdmissionFormKK1"));
        }




        // Pallavi Course Enquiry Start
        /// <summary>
        /// Shows the list of Enquiry
        /// </summary>
        /// <param name="CandidateCode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ListEnquiryCourse(DateTime? startDate, DateTime? endDate)
        {
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Name = " Counsellor Dashboard", Url =Url.Action("MainDashboardAsyncHP","Counsellor")},
                    new BreadcrumbItem { Name = "Course Enquiry", Url =Url.Action("ListEnquiryCourse","Counsellor") },
                };
                ViewBag.Breadcrumbs = breadcrumbs;
                return View();
            }
        }

        /// <summary>
        /// Shows the list of Enquiry
        /// </summary>
        /// <param name="CandidateCode"></param>
        /// <returns></returns>

        public async Task<ActionResult> EnquiryCourseMainListsAndFilteredPk(DateTime? StartDate, DateTime? EndDate, string Status)
        {
            if (Session["StaffCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Counsellor ObjCo = new Counsellor();
                ObjCo.BranchCode = Session["BranchCode"].ToString();
                ObjCo.StaffCode = Session["StaffCode"].ToString();               
                DataSet ds = new DataSet();
                ds = await objbal.ListCourseEnquiry(ObjCo);
                List<Counsellor> lstCourseEnquiry = new List<Counsellor>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Counsellor counsobj = new Counsellor();
                    counsobj.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    counsobj.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    counsobj.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                    counsobj.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                    counsobj.EnquirySourceId = Convert.ToInt32(ds.Tables[0].Rows[i]["EnquirySourceId"].ToString());
                    counsobj.EnqStatusId = Convert.ToInt32(ds.Tables[0].Rows[i]["Statusid"].ToString());
                    counsobj.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    counsobj.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());

                    lstCourseEnquiry.Add(counsobj);
                }
               
                Session["lstCourseEnquiryPK"] = lstCourseEnquiry;
                var CourseEnquiryList = Session["lstCourseEnquiryPK"] as List<Counsellor>;
                List<Counsellor> lstFilteredCourseEnquiryList = new List<Counsellor>();
                if (StartDate != null && EndDate != null && Status == "")
                {
                    DateTime startDateValue = StartDate.Value.Date.AddDays(-1);
                    DateTime endDateValue = EndDate.Value.Date.AddDays(+1);
                    lstFilteredCourseEnquiryList = CourseEnquiryList.Where(item => item.EnquiryDate > startDateValue && item.EnquiryDate < endDateValue).ToList();
                    
                    ObjCo.lstCourseEnquiry = lstFilteredCourseEnquiryList;
                }
                else if (StartDate != null && EndDate != null)
                {
                    if (Status == "Enquire" || Status == "Hold" || Status == "Prospective" || Status == "Enquiry" || Status == "Enquiry Form Pending")
                    {
                        DateTime startDateValue = StartDate.Value.Date.AddDays(-1);
                        DateTime endDateValue = EndDate.Value.Date.AddDays(+1);
                        lstFilteredCourseEnquiryList = CourseEnquiryList.Where(item => item.EnquiryDate > startDateValue && item.EnquiryDate < endDateValue && item.Status == Status).ToList();
                        
                        ObjCo.lstCourseEnquiry = lstFilteredCourseEnquiryList;
                    }
                }
                else
                {
                    ObjCo.lstCourseEnquiry = lstCourseEnquiry;
                }

                return PartialView(ObjCo);
            }
        }

        [HttpPost]
        public async Task GetCountryNamePK()
        {
            DataSet ds = await objbal.GetCountryyPK();
            List<SelectListItem> selectLists = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                selectLists.Add(new SelectListItem
                {
                    Text = dr["CountryName"].ToString(),
                    Value = dr["CountryId"].ToString()
                });
            }
            ViewBag.Country = selectLists;
        }
        public async Task<JsonResult> GetStateNamePK(int CountryId)
        {
            Counsellor ObjCo = new Counsellor();
            ObjCo.CountryId = CountryId;
            DataSet ds = new DataSet();          
            ds = await objbal.GetStatePK(ObjCo);
            List<SelectListItem> StateList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StateList.Add(new SelectListItem
                {
                    Text = dr["StateName"].ToString(),
                    Value = dr["StateId"].ToString()
                });
            }
            ViewBag.state = StateList;
            return Json(StateList, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetCityNamePk(int StateId)
        {
            Counsellor ObjCo = new Counsellor();
            ObjCo.StateId = StateId;
            DataSet ds = new DataSet();          
            ds = await objbal.GetCityPK(ObjCo);
            List<SelectListItem> CityList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CityList.Add(new SelectListItem
                {
                    Text = dr["cityName"].ToString(),
                    Value = dr["cityId"].ToString()
                });
            }
            ViewBag.city = CityList;
            return Json(CityList, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetInterestedCoursePK()
        {
            Counsellor ObjCo = new Counsellor();           
            DataSet ds = new DataSet();
            ObjCo.BranchCode = Session["BranchCode"].ToString();
            ds = await objbal.GetInterestedCoursePK(ObjCo);
            List<SelectListItem> CourseList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CourseList.Add(new SelectListItem
                {
                    Text = dr["CourseName"].ToString(),
                    Value = dr["Coursecode"].ToString()
                });

            }
            ViewBag.Course = CourseList;
            return Json(CourseList, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetTimePreferredPK()
        {
            Counsellor ObjCo = new Counsellor();
            DataSet ds = new DataSet();           
            ds = await objbal.GetTimePreferredPK(ObjCo);
            List<SelectListItem> TimePrefList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                TimePrefList.Add(new SelectListItem
                {
                    Text = dr["Timepreferred"].ToString(),
                    Value = dr["Timepreferred"].ToString()
                });

            }
            ViewBag.TimePrefferd = TimePrefList;
            return Json(TimePrefList, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Second set of dropdown
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task GetAdditionalCountryNamePK()
        {
            DataSet ds = await objbal.GetCountryyPK();
            List<SelectListItem> AdditionalCountry = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AdditionalCountry.Add(new SelectListItem
                {
                    Text = dr["CountryName"].ToString(),
                    Value = dr["CountryId"].ToString()
                });
            }
            ViewBag.AdditionalCountry = AdditionalCountry;
        }

        public async Task<JsonResult> GetAdditionalStateNamePK(int AdditionalCountryId)
        {
            Counsellor ObjCo = new Counsellor();
            ObjCo.CountryId = AdditionalCountryId;
            DataSet ds = new DataSet();
            ds = await objbal.GetStatePK(ObjCo);
            List<SelectListItem> AdditionalStateList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AdditionalStateList.Add(new SelectListItem
                {
                    Text = dr["StateName"].ToString(),
                    Value = dr["StateId"].ToString()
                });
            }
            ViewBag.AdditionalState = AdditionalStateList;
            return Json(AdditionalStateList, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetAdditionalCityNamePk(int AdditionalStateId)
        {
            Counsellor ObjCo = new Counsellor();
            ObjCo.StateId = AdditionalStateId;
            DataSet ds = new DataSet();
            ds = await objbal.GetCityPK(ObjCo);
            List<SelectListItem> AdditionalCityList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AdditionalCityList.Add(new SelectListItem
                {
                    Text = dr["cityName"].ToString(),
                    Value = dr["cityId"].ToString()
                });
            }
            ViewBag.AdditionalCity = AdditionalCityList;
            return Json(AdditionalCityList, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetGraduationPK()
        {
            Counsellor ObjCo = new Counsellor();
            DataSet ds = new DataSet();
            ds = await objbal.GetGraduationPK(ObjCo);
            List<SelectListItem> GraduationList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                GraduationList.Add(new SelectListItem
                {
                    Text = dr["GraduationName"].ToString(),
                    Value = dr["GraduationId"].ToString()
                });

            }
            ViewBag.GraduationList = GraduationList;
            return Json(GraduationList, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetPostGraduationPK(int GraduationId)
        {
            Counsellor ObjCo = new Counsellor();
            ObjCo.GraduationId = GraduationId;
            DataSet ds = new DataSet();          
            ds = await objbal.GetPostGraduationPK(ObjCo);
            List<SelectListItem> PostGraduationList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                PostGraduationList.Add(new SelectListItem
                {
                    Text = dr["PostGraduationName"].ToString(),
                    Value = dr["PostGraduationId"].ToString()
                });

            }
            ViewBag.Postgraduation = PostGraduationList;
            return Json(PostGraduationList, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetProfessionalStatusPK()
        {
            DataSet ds = new DataSet();
            ds = await objbal.GetProfessionalstatusPK();
            List<SelectListItem> Statuslist = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Statuslist.Add(new SelectListItem
                {
                    Text = dr["Status"].ToString(),
                    Value = dr["StatusId"].ToString()
                });
            }
            ViewBag.ProfessionalStatuslist = Statuslist;
            return Json(Statuslist, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetEnquirySourcePK()
        {
            DataSet ds = new DataSet();
            ds = await objbal.GetEnquirySourcePK();
            List<SelectListItem> SourceList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                SourceList.Add(new SelectListItem
                {
                    Text = dr["SourceName"].ToString(),
                    Value = dr["SourceId"].ToString()
                });
            }
            ViewBag.EnquirySourceList = SourceList;
            return Json(SourceList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> UpdateCourseEnquiryPK(string CandidateCode, int? enqStatusId)
{
    if (Session["StaffCode"] == null)
    {
        return RedirectToAction("Login", "Account");
    }
    else
    {

        List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = " Counsellor Dashboard", Url =Url.Action("MainDashboardAsyncHP","Counsellor")},
            new BreadcrumbItem { Name = "Course Enquiry ", Url =Url.Action("ListEnquiryCourse","Counsellor") },
            new BreadcrumbItem { Name = "Update Course Enquiry", Url =Url.Action("UpdateCourseEnquiryPK","Counsellor") },
        };
        ViewBag.Breadcrumbs = breadcrumbs;

        Counsellor objC = new Counsellor();
        objC.BranchCode = Session["BranchCode"].ToString();
        objC.CandidateCode = CandidateCode;
        SqlDataReader dr;
        int StateId = 0;
        int GraduationId = 0;
        if (enqStatusId == 65 || enqStatusId == 77)
        {
            dr = await objbal.GetCandidateCodePK(objC);
            await GetInterestedCoursePK();
            await GetTimePreferredPK();
            await GetCountryNamePK();
            await GetGraduationPK();
            await GetAdditionalCountryNamePK();
            await GetProfessionalStatusPK();
            await GetEnquirySourcePK();
            await GetEnquiryForPK();
            await GetStatusPK();                  

            while (dr.Read())
            {
                objC.CandidateCode = dr["CandidateCode"].ToString();
                objC.FullName = dr["FullName"].ToString();
                objC.ContactNumber = (dr["ContactNumber"].ToString());
                objC.Gender = (dr["Gender"].ToString());
                objC.PresentAddress = (dr["PresentAddress"].ToString());
                objC.PresentPincode = (dr["PresentPincode"].ToString());
                objC.PresentCountryId = Convert.ToInt32(dr["PresentCountryId"].ToString());
                objC.PermanentCountryId = Convert.ToInt32(dr["PermanentCountryId"].ToString());
                objC.PresentStateId = Convert.ToInt32(dr["PresentStateId"].ToString());
                objC.PermanentStateId = Convert.ToInt32(dr["PermanentStateId"].ToString());
                objC.PresentCityId = Convert.ToInt32(dr["PresentCityid"].ToString());
                objC.PermanentAddress = (dr["PermanantAddress"].ToString());
                objC.PermanantPincode = (dr["PermanantPincode"].ToString());
                objC.PermanantCityId = Convert.ToInt32(dr["PermanantCityid"].ToString());
                objC.EmailId = (dr["EmailId"].ToString());
                objC.ProfessionalStatusId = Convert.ToInt32(dr["ProfessionalStatusId"].ToString());
                objC.EnquirySourceId = Convert.ToInt32(dr["EnquirySourceId"].ToString());
                objC.EnquiryDescription = (dr["EquiryDescription"].ToString());
                objC.EnquiryFor = (dr["EnquiryFor"].ToString());
                objC.StatusId = Convert.ToInt32(dr["Statusid"].ToString());
                if (dr["PresentStateId"].ToString() != "")
                {
                    StateId = int.Parse(dr["PresentStateId"].ToString());
                }
                objC.Graduation = dr["GraduationId"].ToString();
                if (dr["GraduationId"].ToString() != "")
                {
                    GraduationId = int.Parse(dr["GraduationId"].ToString());
                }
                objC.PostGraduation = dr["PostGraduationId"].ToString();
                objC.TimePreferred = (dr["TimePreferred"].ToString());
                objC.WantDemo = (dr["WantDemo"].ToString());                

            }

            ViewBag.CountryId = objC.PresentCountryId;
            ViewBag.stateId = objC.PresentStateId;
            ViewBag.cityId = objC.PresentCityId;
            ViewBag.AdditionalCountryId = objC.PermanentCountryId;
            ViewBag.AdditionalStateId = objC.PermanentStateId;
            ViewBag.AdditionalCityId = objC.PermanantCityId;
            ViewBag.GraduationId = objC.Graduation;
            ViewBag.PostgraduationId = objC.PostGraduation;
            
            await GetCityNamePk(StateId);
            await GetPostGraduationPK(GraduationId);
        }
        if (enqStatusId == 63 || enqStatusId == 78)
        {
            dr = await objbal.FetchStudentData(objC);
            await GetInterestedCoursePK();
            await GetTimePreferredPK();
            await GetCountryNamePK();
            await GetGraduationPK();
            await GetAdditionalCountryNamePK();
            await GetProfessionalStatusPK();
            await GetEnquirySourcePK();
            await GetEnquiryForPK();
            await GetStatusPK();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    objC.CandidateCode = dr["CandidateCode"].ToString();
                    objC.FullName = dr["FullName"].ToString();
                    objC.Gender = dr["Gender"].ToString();
                    objC.PresentAddress = dr["PresentAddress"].ToString();
                    objC.PresentPincode = dr["PresentPincode"].ToString();
                    objC.ContactNumber = dr["ContactNumber"].ToString();
                    objC.AlternateNumber = dr["AlternateNumber"].ToString();
                    objC.EnqStatusId = int.Parse(dr["Statusid"].ToString());
                    objC.EmailId = dr["EmailId"].ToString();
                    objC.EnquirySource = dr["SourceName"].ToString();
                    objC.EquiryDescription = dr["EquiryDescription"].ToString();
                    objC.EnquiryDate = Convert.ToDateTime(dr["EnquiryDate"]);
                    objC.EnquiryFor = dr["EnquiryFor"].ToString();
                }
            }
        }


        return PartialView("UpdateCourseEnquiryPK", objC);

    }
		}

        [HttpPost]
        public async Task<ActionResult> UpdateCourseEnquiryPK(Counsellor objC)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {

                await objbal.UpdateCourseEnquiryPK(objC);
                await objbal.SaveInterestedCoursePK(objC);
                
                if (objC.EnqStatusId == 78 || objC.EnqStatusId == 63)
                {
                    await objbal.SaveQualificationPk(objC);

                }
				else
				{
					await objbal.UpdateQualification(objC);
				}
                TempData["AlertMessage"] = " Updated Successfully";
                //return RedirectToAction("ListEnquiryCourse", objC);
                return Json(new { success = "Updated Successfully...!" }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public async Task<ActionResult> DetailsCourseEnquiry(string CandidateCode)
        {
            Counsellor objshow = new Counsellor();          
            objshow.CandidateCode = CandidateCode;

            SqlDataReader dr = await objbal.DetailsCourseEnquiryPK(objshow); // Assuming "DetailsCourseEnquiryPKAsync" is the asynchronous version of the method

            if (dr.Read())
            {
                objshow.FullName = dr["FullName"].ToString();
                objshow.ContactNumber = dr["ContactNumber"].ToString();
                objshow.PresentAddress = dr["PresentAddress"].ToString();
                objshow.PermanentAddress = dr["PermanantAddress"].ToString(); // Fixed typo in "PermanentAddress"
                objshow.EmailId = dr["EmailId"].ToString();
                objshow.TimePreferred = dr["TimePreferred"].ToString();
                objshow.WantDemo = dr["WantDemo"].ToString();
            }
            return PartialView("DetailsCourseEnquiry", objshow); // Assuming "DetailsCourseEnquiry" is the name of your partial view
        }
        [HttpPost]

        public async Task GetEnquiryForPK()
        {
            Counsellor ObjCo = new Counsellor();
            DataSet ds = new DataSet();
            ds = await objbal.GetEnquiryForPK(ObjCo);
            List<SelectListItem> EnquiryForList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                EnquiryForList.Add(new SelectListItem
                {
                    Text = dr["EnquiryFor"].ToString(),
                    Value = dr["EnquiryFor"].ToString()
                });

            }
            ViewBag.EnquiryFor = EnquiryForList;
        }
        [HttpGet]
        public async Task<ActionResult> AddEnquiryPK()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {

                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem { Name = " Counsellor Dashboard", Url =Url.Action("MainDashboardAsyncHP","Counsellor")},
                    new BreadcrumbItem { Name = "Course Enquiry ", Url =Url.Action("ListEnquiryCourse","Counsellor") },
                    new BreadcrumbItem { Name = "Add Course Enquiry", Url =Url.Action("AddEnquiryPK","Counsellor") },
                };
                ViewBag.Breadcrumbs = breadcrumbs;

                Counsellor objcon = new Counsellor();
                objcon.BranchCode = Session["BranchCode"].ToString();
                objcon.StaffCode = Session["StaffCode"].ToString();
                await GetInterestedCoursePK();
                await GetTimePreferredPK();
                await GetCountryNamePK();
                await GetGraduationPK();
                await GetAdditionalCountryNamePK();
                await GetProfessionalStatusPK();
                await GetEnquirySourcePK();
                await GetEnquiryForPK();
                await GetStatusPK();
                return View();
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddEnquiryPK(Counsellor obj, List<string> selectedCourses)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                obj.SelectedCourses = selectedCourses;             
                obj.EnquiryFor = obj.EnquiryFor;
                await GetMaxCandidateCount(obj);
                obj.StaffCode = Session["StaffCode"].ToString();
                obj.BranchCode = Session["BranchCode"].ToString();
                obj.CandidateCode = TempData["CandidateCode"] as string;
                await Task.Run(() => objbal.SaveCourseEnquiryInfoPK(obj));
                await objbal.SaveInterestedCoursePK(obj);
                return RedirectToAction("AddEnquiryPK");
            }
        }
        public async Task<JsonResult> GetStatusPK()
        {
            Counsellor ObjCo = new Counsellor();
            DataSet ds = new DataSet();
            ds = await objbal.GetStatusPK(ObjCo);
            List<SelectListItem> StatusList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StatusList.Add(new SelectListItem
                {
                    Text = dr["Status"].ToString(),
                    Value = dr["StatusId"].ToString()
                });

            }
            ViewBag.statuslist = StatusList;
            return Json(StatusList, JsonRequestBehavior.AllowGet);
        }

        //Vijay Followup start


        [HttpGet]
        public async Task<ActionResult> AllStudentList()
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor ojj = new Counsellor();
                List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>
                {
                     new BreadcrumbItem { Name = "Counsellor Dashboard", Url =Url.Action("MainDashboardAsyncHP","Counsellor")},
                     new BreadcrumbItem { Name = "Follow-Up", Url =Url.Action("AllStudentList","Counsellor") },
                };
                ViewBag.Breadcrumbs = breadcrumbs;

                return View();
            }
        }

        [HttpGet]
        public async Task<ActionResult> ListEnquiryFollowupAsyncVU(DateTime? startdate, DateTime? enddate)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor objC = new Counsellor();
                DateTime effectiveStartDate = startdate ?? DateTime.Now.AddYears(-2); // Default to one year ago
                DateTime effectiveEndDate = enddate ?? DateTime.Now; // Default to today
                objC.StaffCode = Session["StaffCode"].ToString();
                objC.StartDate = effectiveStartDate;
                objC.EndDate = effectiveEndDate;
                objC.EnquiryFor = "Course";                
                DataSet ds = await objbal.ListEnquiryFollowupAsyncVU(objC);
                Counsellor objcl = new Counsellor();
                List<Counsellor> ListEnquiry = new List<Counsellor>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Counsellor objC1 = new Counsellor();

                    objC1.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();                   
                    objC1.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
                    objC1.FollowUpNote = ds.Tables[0].Rows[i]["FollowUpNote"].ToString();
                    objC1.CandidateCode = (ds.Tables[0].Rows[i]["CandidateCode"].ToString());
                    objC1.FollowUpTakenDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["FollowUpTakenDate"].ToString());
					if (ds.Tables[0].Rows[i]["NextFollowUpDate"] != DBNull.Value &&
					  !string.IsNullOrEmpty(ds.Tables[0].Rows[i]["NextFollowUpDate"].ToString()))
					 {
						 objC1.NextFollowUpDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["NextFollowUpDate"].ToString());
					 }
					 else
					 {
						 objC1.NextFollowUpDate = DateTime.MinValue; 
					 }                    objC1.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    objC1.FollowUpStatus = ds.Tables[0].Rows[i]["FStatus"].ToString();
                    objC1.EnqStatusId = Convert.ToInt32(ds.Tables[0].Rows[i]["Statusid"].ToString());
                    objC1.FollowUpStatusId = Convert.ToInt32(ds.Tables[0].Rows[i]["FStatusId"].ToString());
                    objC1.StaffName = ds.Tables[0].Rows[i]["StaffName"].ToString();
                    objC1.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
                    ListEnquiry.Add(objC1);
                }
                objcl.lstEnquiryfollowup = ListEnquiry;              
                Session["listEnqFollowUp"] = objcl.lstEnquiryfollowup;               
                return PartialView("ListEnquiryFollowupAsyncVU", objcl);
            }
        }
        [HttpGet]
         public async Task<ActionResult> EnquiryFollowUpVU(string CandidateCode)
        {
            await GetFollowUpStatusVu();
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor ObjCo = new Counsellor();
                ObjCo.StaffCode = Session["StaffCode"].ToString();
                ObjCo.CandidateCode = CandidateCode;
                SqlDataReader dr;
                dr = await objbal.GetFlwUpCandidateDetails(ObjCo);                
                while (dr.Read())
                {                   
                    ObjCo.CandidateCode = dr["CandidateCode"].ToString();
                    ObjCo.FullName = dr["FullName"].ToString();
                    ObjCo.ContactNumber = dr["ContactNumber"].ToString();
                    ObjCo.AlternateNumber = dr["AlternateNumber"].ToString();
                    ObjCo.EmailId = dr["EmailId"].ToString();
                    ObjCo.EnquiryFor = dr["EnquiryFor"].ToString();
                    ObjCo.EnqStatusId = int.Parse(dr["Statusid"].ToString());
                    ObjCo.FollowUpStatusId = int.Parse(dr["FollowUpStatus"].ToString());                   
                    ObjCo.CourseName = (dr["InterestedCourses"].ToString());                   
                }
				ObjCo.StatusId = ObjCo.EnqStatusId;
				await GetEnquiryStatusVu(ObjCo.EnqStatusId);
                DataSet ds = new DataSet();
                ds = await objbal.GetCandidateEnqFollowUpHistory(ObjCo);
                List<Counsellor> ListEnquiry = new List<Counsellor>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Counsellor objC1 = new Counsellor();

                    objC1.FollowUpNote = ds.Tables[0].Rows[i]["FollowUpNote"].ToString();
                    objC1.FollowUpTakenDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["FollowUpTakenDate"].ToString());
                    objC1.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    objC1.StaffName = ds.Tables[0].Rows[i]["StaffName"].ToString();
                    ListEnquiry.Add(objC1);
                }
                ObjCo.lstEnquiryfollowup = ListEnquiry;

                return PartialView(ObjCo);
            }
        }

        [HttpGet]
        public async Task<ActionResult> MissedListStudentVU(DateTime? startDate, DateTime? endDate, string filterType, string TabType, int? EnqStatusId, int FollowUpStatusId)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {


                Counsellor obj = new Counsellor();
                List<Counsellor> filteredList = new List<Counsellor>();

                var coursePendingFollowUplst = Session["listEnqFollowUp"] as List<Counsellor>;

                if (filterType == "Course" && TabType == "MissedTab")
                {
                    filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now).ToList();
                }

                if (filterType == "Placement" && TabType == "MissedTab")
                {
                    filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now).ToList();
                }

                if (filterType == "Course" && TabType == "UpcomingFollowUpTab")
                {
                    filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now).ToList();
                }

                if (filterType == "Placement" && TabType == "UpcomingFollowUpTab")
                {
                    filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now).ToList();
                }
                if (filterType == "Course" && TabType == "TodayFollowUpTab")
                {
                    filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate == DateTime.Now).ToList();
                }
                if (filterType == "Placement" && TabType == "TodayFollowUpTab")
                {
                    filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate == DateTime.Now).ToList();
                }
                if (filterType == "Course" && TabType == "MissedTab" && EnqStatusId == 65)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 65 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 65).ToList();
                    }
                }

                if (filterType == "Course" && TabType == "MissedTab" && EnqStatusId == 63)
                {

                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 63 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }

                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 63).ToList();
                    }
                }
                if (filterType == "Course" && TabType == "MissedTab" && EnqStatusId == 77)
                {

                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 77 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }

                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 77).ToList();
                    }
                }
                if (filterType == "Course" && TabType == "MissedTab" && EnqStatusId == 78)
                {

                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 78 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }

                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 78).ToList();
                    }
                }
                if (filterType == "Course" && TabType == "MissedTab" && EnqStatusId == 37)
                {

                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 37 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }

                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 37).ToList();
                    }
                }
                if (filterType == "Course" && TabType == "MissedTab" && EnqStatusId == 22)
                {

                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 22 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }

                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 22).ToList();
                    }
                }

                if (filterType == "Placement" && TabType == "MissedTab" && EnqStatusId == 65)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 65 || item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 65).ToList();
                    }
                }

                if (filterType == "Placement" && TabType == "MissedTab" && EnqStatusId == 63)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 63 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 63).ToList();
                    }
                }
                if (filterType == "Placement" && TabType == "MissedTab" && EnqStatusId == 22)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 22 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 22).ToList();
                    }
                }
                if (filterType == "Placement" && TabType == "MissedTab" && EnqStatusId == 37)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 37 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 37).ToList();
                    }
                }
                if (filterType == "Placement" && TabType == "MissedTab" && EnqStatusId == 77)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 77 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 77).ToList();
                    }
                }
                if (filterType == "Placement" && TabType == "MissedTab" && EnqStatusId == 78)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 78 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate < DateTime.Now && item.EnqStatusId == 78).ToList();
                    }
                }

                if (filterType == "Course" && TabType == "UpcomingFollowUpTab" && EnqStatusId == 63)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 63 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 63).ToList();
                    }
                }

                if (filterType == "Course" && TabType == "UpcomingFollowUpTab" && EnqStatusId == 65)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 65 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 65).ToList();
                    }
                }
                if (filterType == "Course" && TabType == "UpcomingFollowUpTab" && EnqStatusId == 77)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 77 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 77).ToList();
                    }
                }
                if (filterType == "Course" && TabType == "UpcomingFollowUpTab" && EnqStatusId == 78)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 78 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 78).ToList();
                    }
                }
                if (filterType == "Course" && TabType == "UpcomingFollowUpTab" && EnqStatusId == 37)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 37 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 37).ToList();
                    }
                }
                if (filterType == "Course" && TabType == "UpcomingFollowUpTab" && EnqStatusId == 22)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 22 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 22).ToList();
                    }
                }

                if (filterType == "Placement" && TabType == "UpcomingFollowUpTab" && EnqStatusId == 65)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 65 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 65).ToList();
                    }
                }

                if (filterType == "Placement" && TabType == "UpcomingFollowUpTab" && EnqStatusId == 63)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 63 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 63).ToList();
                    }
                }
                if (filterType == "Placement" && TabType == "UpcomingFollowUpTab" && EnqStatusId == 77)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 77 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 77).ToList();
                    }
                }
                if (filterType == "Placement" && TabType == "UpcomingFollowUpTab" && EnqStatusId == 78)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 78 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 78).ToList();
                    }
                }
                if (filterType == "Placement" && TabType == "UpcomingFollowUpTab" && EnqStatusId == 37)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 37 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 37).ToList();
                    }
                }
                if (filterType == "Placement" && TabType == "UpcomingFollowUpTab" && EnqStatusId == 22)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 22 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate > DateTime.Now && item.EnqStatusId == 22).ToList();
                    }
                }

                if (filterType == "Course" && TabType == "TodayFollowUpTab" && EnqStatusId == 65)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 65 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 65).ToList();
                    }
                }
                if (filterType == "Course" && TabType == "TodayFollowUpTab" && EnqStatusId == 63)
                {
                    var date = DateTime.Now;
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 63 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 63).ToList();
                    }
                }
                if (filterType == "Course" && TabType == "TodayFollowUpTab" && EnqStatusId == 77)
                {
                    var date = DateTime.Now;
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 77 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 77).ToList();
                    }
                }
                if (filterType == "Course" && TabType == "TodayFollowUpTab" && EnqStatusId == 78)
                {
                    var date = DateTime.Now;
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 78 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 78).ToList();
                    }
                }
                if (filterType == "Course" && TabType == "TodayFollowUpTab" && EnqStatusId == 37)
                {
                    var date = DateTime.Now;
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 37 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 37).ToList();
                    }
                }
                if (filterType == "Course" && TabType == "TodayFollowUpTab" && EnqStatusId == 22)
                {
                    var date = DateTime.Now;
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 22 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 22).ToList();
                    }
                }
                if (filterType == "Placement" && TabType == "TodayFollowUpTab" && EnqStatusId == 65)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 65 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 65).ToList();
                    }
                }
                if (filterType == "Placement" && TabType == "TodayFollowUpTab" && EnqStatusId == 63)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 63 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 63).ToList();
                    }
                }
                if (filterType == "Placement" && TabType == "TodayFollowUpTab" && EnqStatusId == 77)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 77 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 77).ToList();
                    }
                }
                if (filterType == "Placement" && TabType == "TodayFollowUpTab" && EnqStatusId == 78)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 78 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 78).ToList();
                    }
                }
                if (filterType == "Placement" && TabType == "TodayFollowUpTab" && EnqStatusId == 37)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 37 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 37).ToList();
                    }
                }
                if (filterType == "Placement" && TabType == "TodayFollowUpTab" && EnqStatusId == 22)
                {
                    if (FollowUpStatusId > 0 || FollowUpStatusId == 1 || FollowUpStatusId == 3 || FollowUpStatusId == 2)
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 22 && item.FollowUpStatusId == FollowUpStatusId).ToList();
                    }
                    else
                    {
                        filteredList = coursePendingFollowUplst.Where(item => item.EnquiryFor != null && item.EnquiryFor.StartsWith(filterType) && item.NextFollowUpDate.ToShortDateString() == DateTime.Now.ToShortDateString() && item.EnqStatusId == 22).ToList();
                    }
                }

                obj.lstEnquiryfollowup = filteredList;
                ViewBag.List = filteredList;

                return PartialView("MissedListStudentVU", obj);
            }

        }
        public async Task<ActionResult> InternalAddLastFollowUpVU(string candidatecode, DateTime startdate, DateTime enddate)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                {
                    Counsellor objC = new Counsellor();
                    objC.StaffCode = Session["StaffCode"].ToString();
                    objC.StartDate = startdate;
                    objC.EndDate = enddate;
                    string CandidateCode = candidatecode;
                    DataSet ds = await objbal.ListEnquiryFollowupAsyncVU(objC);
                    List<Counsellor> ListEnquiry = new List<Counsellor>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Counsellor objC1 = new Counsellor();

                        objC1.FollowUpNote = ds.Tables[0].Rows[i]["FollowUpNote"].ToString();
                        objC1.FollowUpTakenDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["FollowUpTakenDate"].ToString());
                        objC1.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                        objC1.StaffName = ds.Tables[0].Rows[i]["StaffName"].ToString();
                        objC1.Status = ds.Tables[0].Rows[i]["Status"].ToString();

                        ListEnquiry.Add(objC1);
                    }
                    objC.lstEnquiryfollowup = ListEnquiry;

                    return View("InternalAddLastFollowUpVU", objC);
                }
            }

        }
        [HttpGet]
        public async Task<JsonResult> GetEnquiryStatusVu(int enqStatusId)
        {
            DataSet ds = new DataSet();
            ds = await objbal.GetEnquiryStatus();
            List<SelectListItem> StatusList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string statusId = dr["StatusId"].ToString();

                if (enqStatusId == 65 && statusId == "78")
                {
                    continue;
                }

                StatusList.Add(new SelectListItem
                {
                    Text = dr["Status"].ToString(),
                    Value = statusId
                });
            }
            ViewBag.EnquiryStatus = StatusList;
            return Json(StatusList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<JsonResult> GetFollowUpStatusVu()
        {
            DataSet ds = new DataSet();
            ds = await objbal.GetFollowUpStatus();
            List<SelectListItem> StatusList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StatusList.Add(new SelectListItem
                {
                    Text = dr["Status"].ToString(),
                    Value = dr["StatusId"].ToString()
                });
            }
            ViewBag.FollowUpStatus = StatusList;
            return Json(StatusList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> AdmissionFetchVU()
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor ObjCo = new Counsellor();
                ObjCo.StaffCode = Session["StaffCode"].ToString();
                SqlDataReader dr;
                dr = await objbal.AdmissionFetchVU(ObjCo);
                while (dr.Read())
                {
                    ObjCo.FullName = dr["FullName"].ToString();
                    ObjCo.ContactNumber = dr["ContactNumber"].ToString();
                    ObjCo.EmailId = dr["EmailId"].ToString();
                    ObjCo.DateOfBirth = Convert.ToDateTime(dr["DateOfBirth"].ToString());
                    ObjCo.Gender = dr["Gender"].ToString();
                    ObjCo.BloodGroup = dr["BloodGroup"].ToString();
                    ObjCo.PermanantAddress = dr["PermanantAddress"].ToString();
                    ObjCo.PermanantCityId = Convert.ToInt32(dr["PresentCityId"].ToString());
                    ObjCo.PresentAddress = dr["PermanantAddress"].ToString();
                    ObjCo.PresentCityId = Convert.ToInt32(dr["PresentCityId"].ToString());

                    ObjCo.Graduation = dr["Graduation"].ToString();
                    ObjCo.IndustryId = Convert.ToInt32(dr["Graduation"].ToString());
                    ObjCo.CompanyName = dr["CompanyName"].ToString();
                    ObjCo.Department = dr["Department"].ToString();

                    ObjCo.CTC = dr["CTC"].ToString();

                }
                return View("");
            }


        }
        [HttpPost]
        public async Task<ActionResult> EnquiryFollowUpVU(Counsellor cobj)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {


                cobj.FollowUpTakenByStaffCode = Session["StaffCode"].ToString();
                await objbal.AddEnqFollowupVU(cobj);
                if (cobj.EnqStatusId != 0 || cobj.EnqStatusId != null)
                {
                    await objbal.UpdateEnquiryStatusVU(cobj);

                }
                if (cobj.isCheck == true)
                {
                    
                    string MailType = "EnquiryForm";
                    await SendMailkk(cobj.EmailId, cobj.FullName, MailType, cobj.CandidateCode, 78, cobj.EnquiryFor, cobj.Password);
                }

                return Json(new { success = true, message = "follow up added" });
            }

        }
        /// <summary>
        /// Getting the max candidate count for setting a new candidate code
        /// </summary>
        /// <param name="cobj"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetMaxCandidateCount(Counsellor  obj)
        {
                Counsellor ObjCo = new Counsellor();
                ObjCo.ClientPrefix = Session["ClientPrefix"].ToString();
            string NewCandidateCode = "";
            if (obj.EnquiryFor == "Placement")
            {
              
                ObjCo.ClientPrefix = "E" + ObjCo.ClientPrefix;
                SqlDataReader dr;
                dr = await objbal.GetMaxCountForCandidate(ObjCo);
                while (dr.Read())
                {
                    ObjCo.CandidateCode = dr["MaxCandidateCode"].ToString();
                }
                if (ObjCo.CandidateCode != null && ObjCo.CandidateCode != "")
                {
                    string characters = Regex.Match(ObjCo.CandidateCode, @"^[A-Za-z]+").Value;
                    int digits = int.Parse(Regex.Match(ObjCo.CandidateCode, @"\d+$").Value);
                    int NewCount = digits + 1;
                    NewCandidateCode = characters + NewCount;
                }
                else 
                {
                    ObjCo.CandidateCode = "0";
                    string characters = Regex.Match(ObjCo.CandidateCode, @"^[A-Za-z]+").Value;
                    int digits = int.Parse(Regex.Match(ObjCo.CandidateCode, @"\d+$").Value);
                    int NewCount = digits + 1;
                    NewCandidateCode = ObjCo.ClientPrefix + NewCount;
                }
                TempData["CandidateCode"] = NewCandidateCode;
            }
            else
            {
                SqlDataReader dr;
                dr = await objbal.GetMaxCountForCandidate(ObjCo);
                while (dr.Read())
                {
                    ObjCo.CandidateCode = dr["MaxCandidateCode"].ToString();
                }
                if (ObjCo.CandidateCode != null && ObjCo.CandidateCode != "")
                {
                    string characters = Regex.Match(ObjCo.CandidateCode, @"^[A-Za-z]+").Value;
                    int digits = int.Parse(Regex.Match(ObjCo.CandidateCode, @"\d+$").Value);
                    int NewCount = digits + 1;
                    NewCandidateCode = characters + NewCount;
                }
                else
                {
                    ObjCo.CandidateCode = "0";
                    string characters = Regex.Match(ObjCo.CandidateCode, @"^[A-Za-z]+").Value;
                    int digits = int.Parse(Regex.Match(ObjCo.CandidateCode, @"\d+$").Value);
                    int NewCount = digits + 1;
                    NewCandidateCode = ObjCo.ClientPrefix + NewCount;
                }
                TempData["CandidateCode"] = NewCandidateCode;
            }
                return View(ObjCo);
        }



        [HttpGet]
        /// <summary>
        /// Enquiry Update data
        /// </summary>
        /// <param name="CandidateCode"></param>
        /// <returns></returns>
        public async Task<ActionResult> UpdateSendEnquiryFormSks(string CandidateCode, int enqStatusId)
        {
            
            Counsellor ObjCounsellor = new Counsellor();

            try
            {
                await GetGraduationSK();
                await GetSourceName();
                await professionalstatus();
                await FetchPermanantCountryName();
                await FetchCountryName();
                await Designation();
                await Department();

                ObjCounsellor.CandidateCode = CandidateCode;

                if (enqStatusId == 65)
                {

                    using (SqlDataReader dr = await objbal.FechEnquiryList(ObjCounsellor))
                    {
                        if (dr.HasRows)
                        {
                            while (await dr.ReadAsync())
                            {
                                ObjCounsellor.CandidateCode = dr["CandidateCode"].ToString();
                                ObjCounsellor.FullName = dr["FullName"].ToString();
                                ObjCounsellor.Gender = dr["Gender"].ToString();
                                ObjCounsellor.PresentAddress = dr["PresentAddress"].ToString();
                                ObjCounsellor.PresentPincode = dr["PresentPincode"].ToString();
                                ObjCounsellor.CountryId = Convert.ToInt32(dr["PresentCountryId"]);
                                ObjCounsellor.StateId = Convert.ToInt32(dr["PresentStateId"]);
                                ObjCounsellor.PresentCityId = Convert.ToInt32(dr["PresentCityId"]);
                                ObjCounsellor.PresentCity = dr["PresentCity"].ToString();
                                ObjCounsellor.ContactNumber = dr["ContactNumber"].ToString();
                                ObjCounsellor.PermanentAddress = dr["PermanantAddress"].ToString();
                                ObjCounsellor.PermanentPincode = dr["PermanantPincode"].ToString();
                                ObjCounsellor.PermanentCountryId = Convert.ToInt32(dr["PermanantCountryId"]);
                                ObjCounsellor.PermanentStateId = Convert.ToInt32(dr["PermanantStateId"]);
                                ObjCounsellor.PermanantCityId = Convert.ToInt32(dr["permanantCityId"]);
                                ObjCounsellor.PermanantCity = dr["PermanentCity"].ToString();
                                ObjCounsellor.EmailId = dr["EmailId"].ToString();
                                ObjCounsellor.ProfessionalStatus = dr["ProfessionalStatus"].ToString();
                                ObjCounsellor.EnquirySource = dr["SourceName"].ToString();
                                ObjCounsellor.EquiryDescription = dr["EquiryDescription"].ToString();
                                ObjCounsellor.EnquiryDate = Convert.ToDateTime(dr["EnquiryDate"]);
                                ObjCounsellor.EnquiryFor = dr["EnquiryFor"].ToString();
                                ObjCounsellor.EnquiryAddedStaffCode = dr["EnquiryAddedStaffCode"].ToString();
                                ObjCounsellor.CompanyName = dr["CompanyName"].ToString();
                                ObjCounsellor.Department = dr["DepartmentName"].ToString();
                                ObjCounsellor.Designation = dr["DesignationName"].ToString();
                                ObjCounsellor.CTC = dr["CTC"].ToString();
                                ObjCounsellor.ExpectedSalary = dr["ExpectedSalery"].ToString();
                                ObjCounsellor.Graduation = dr["GraduationName"].ToString();
                                ObjCounsellor.PostGraduation = dr["PostGraduationName"].ToString();
                            }
                        }
                    }

                    ViewBag.PreCountryId = ObjCounsellor.CountryId;
                    ViewBag.PreStateId = ObjCounsellor.StateId;
                    ViewBag.PreCityId = ObjCounsellor.PresentCityId;
                    ViewBag.PrmCountryId = ObjCounsellor.PermanentCountryId;
                    ViewBag.PrmStateId = ObjCounsellor.PermanentStateId;
                    ViewBag.PrmCityId = ObjCounsellor.PermanantCityId;
                    await GetStateNameSk(ObjCounsellor.CountryId);
                    await GetPermanantStateNameSk(ObjCounsellor.PermanentCountryId);
                    await GetCityNameSk(ObjCounsellor.StateId);
                    await GetPermanantCityNameSk(ObjCounsellor.PermanentStateId);
                }
                if (enqStatusId == 63 || enqStatusId == 78)
                {

                    using (SqlDataReader dr = await objbal.FetchStudentData(ObjCounsellor))
                    {
                        if (dr.HasRows)
                        {
                            while (await dr.ReadAsync())
                            {
                                ObjCounsellor.CandidateCode = dr["CandidateCode"].ToString();
                                ObjCounsellor.FullName = dr["FullName"].ToString();
                                ObjCounsellor.Gender = dr["Gender"].ToString();
                                ObjCounsellor.PresentAddress = dr["PresentAddress"].ToString();
                                ObjCounsellor.PresentPincode = dr["PresentPincode"].ToString();
                                ObjCounsellor.ContactNumber = dr["ContactNumber"].ToString();
                                ObjCounsellor.AlternateNumber = dr["AlternateNumber"].ToString();
                                ObjCounsellor.EnqStatusId = int.Parse(dr["Statusid"].ToString());
                                ObjCounsellor.EmailId = dr["EmailId"].ToString();
                                ObjCounsellor.EnquirySource = dr["SourceName"].ToString();
                                ObjCounsellor.EquiryDescription = dr["EquiryDescription"].ToString();
                                ObjCounsellor.EnquiryDate = Convert.ToDateTime(dr["EnquiryDate"]);
                                ObjCounsellor.EnqStatusId = Convert.ToInt32(dr["Statusid"]);
                                ObjCounsellor.EnquiryFor = dr["EnquiryFor"].ToString();                               
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // Return an error view or message
                //return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while fetching enquiry details.");
            }

            return View(ObjCounsellor);


        }

        [HttpGet]
        public async Task<ActionResult> SendUpdateCourseEnquiryFormPK(string CandidateCode, int enqStatusId)
        {
        
                Counsellor objC = new Counsellor();
                objC.BranchCode = Session["BranchCode"].ToString();
                objC.CandidateCode = CandidateCode;
                SqlDataReader dr;
                int StateId = 0;
                int GraduationId = 0;
                if (enqStatusId == 65)
                {
                    dr = await objbal.GetCandidateCodePK(objC);
                    await GetInterestedCoursePK();
                    await GetTimePreferredPK();
                    await GetCountryNamePK();
                    await GetGraduationPK();
                    await GetAdditionalCountryNamePK();
                    await GetProfessionalStatusPK();
                    await GetEnquirySourcePK();
                    await GetEnquiryForPK();
                    await GetStatusPK();
                    //await GetStatusPYK();

                    while (dr.Read())
                    {
                        objC.CandidateCode = dr["CandidateCode"].ToString();
                        objC.FullName = dr["FullName"].ToString();
                        objC.ContactNumber = (dr["ContactNumber"].ToString());
                        objC.Gender = (dr["Gender"].ToString());
                        objC.PresentAddress = (dr["PresentAddress"].ToString());
                        objC.PresentPincode = (dr["PresentPincode"].ToString());
                        objC.PresentCityId = Convert.ToInt32(dr["PresentCityid"].ToString());
                        objC.PermanentAddress = (dr["PermanantAddress"].ToString());
                        objC.PermanantPincode = (dr["PermanantPincode"].ToString());
                        objC.PermanantCityId = Convert.ToInt32(dr["PermanantCityid"].ToString());
                        objC.EmailId = (dr["EmailId"].ToString());
                        objC.ProfessionalStatusId = Convert.ToInt32(dr["ProfessionalStatusId"].ToString());
                        objC.EnquirySourceId = Convert.ToInt32(dr["EnquirySourceId"].ToString());
                        objC.EnquiryDescription = (dr["EquiryDescription"].ToString());
                        objC.EnquiryFor = (dr["EnquiryFor"].ToString());
                        objC.StatusId = Convert.ToInt32(dr["Statusid"].ToString());
                        objC.GraduationId = Convert.ToInt32(dr["GraduationId"].ToString());
                        objC.PostgraduationId = Convert.ToInt32(dr["PostGraduationId"].ToString());
                        objC.TimePreferred = (dr["TimePreferred"].ToString());
                        objC.WantDemo = (dr["WantDemo"].ToString());
                        StateId = Convert.ToInt32(dr["StateId"].ToString());
                        GraduationId = Convert.ToInt32(dr["GraduationId"].ToString());

                    }
                    await GetCityNamePk(StateId);
                    await GetPostGraduationPK(GraduationId);
                }
                if (enqStatusId == 63 || enqStatusId == 78)
                {
                    dr = await objbal.FetchStudentData(objC);
                    await GetInterestedCoursePK();
                    await GetTimePreferredPK();
                    await GetCountryNamePK();
                    await GetGraduationPK();
                    await GetAdditionalCountryNamePK();
                    await GetProfessionalStatusPK();
                    await GetEnquirySourcePK();
                    await GetEnquiryForPK();
                    await GetStatusPK();

                    if (dr.HasRows)
                    {
                        while (await dr.ReadAsync())
                        {
                            objC.CandidateCode = dr["CandidateCode"].ToString();
                            objC.FullName = dr["FullName"].ToString();
                            objC.Gender = dr["Gender"].ToString();
                            objC.PresentAddress = dr["PresentAddress"].ToString();
                            objC.PresentPincode = dr["PresentPincode"].ToString();
                            objC.ContactNumber = dr["ContactNumber"].ToString();
                            objC.AlternateNumber = dr["AlternateNumber"].ToString();
                            objC.EnqStatusId = int.Parse(dr["Statusid"].ToString());
                            objC.EmailId = dr["EmailId"].ToString();
                            objC.EnquirySource = dr["SourceName"].ToString();
                            objC.EquiryDescription = dr["EquiryDescription"].ToString();
                            objC.EnquiryDate = Convert.ToDateTime(dr["EnquiryDate"]);
                            objC.EnquiryFor = dr["EnquiryFor"].ToString();
                        }
                    }

                }
                return PartialView("UpdateCourseEnquiryPK", objC);          
        }
		      /// <summary>
        /// This method is used for getting the data of a particular enquiry student and bind it in the same view
        /// </summary>
        /// <param name="CandidateCode"> It gets the candidate code from the list and pass it to the BAL class.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> IntExtAdmissionFormAM(string CandidateCode)
        {

            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Counsellor ObjCo = new Counsellor();
                ObjCo.BranchCode = Session["BranchCode"].ToString();
                ObjCo.StaffCode = Session["StaffCode"].ToString();
                ViewBag.Country = await objbal.GetCountry();
                ViewBag.Industry = await objbal.industrykk();
                ViewBag.Department = await objbal.Departmentkk();
                ViewBag.Designation = await objbal.Designationkk();
                ViewBag.Course = await objbal.FetchIntrestedCourse(CandidateCode);
                ViewBag.OtherCourses =  await objbal.GetCourseNameHP(ObjCo);
                await GetCountryNamePK();
                await GetAdditionalCountryNamePK();
                await GetGraduationPK();
                await GetBankName(ObjCo);
              
                ObjCo.CandidateCode = CandidateCode;
                SqlDataReader dr;
                dr = await objbal.AdmissionFetchVU(ObjCo);
                while (dr.Read())
                {
                    ObjCo.EnquiryFor = dr["EnquiryFor"].ToString();
                    ObjCo.CandidateCode = dr["CandidateCode"].ToString();
                    ObjCo.FullName = dr["FullName"].ToString();
                    ObjCo.StudMobileNo = dr["ContactNumber"].ToString();
                    ObjCo.AlternateNumber = dr["AlternateNumber"].ToString();
                    ObjCo.EmailId = dr["EmailId"].ToString();
                    ObjCo.Gender = dr["Gender"].ToString();
                    ObjCo.PermanentAddress = dr["PermanantAddress"].ToString();
                    ObjCo.PresentAddress = dr["PresentAddress"].ToString();
                    ObjCo.PermanantPincode = dr["PermanantPincode"].ToString();
                    ObjCo.PresentPincode = dr["PresentPincode"].ToString();
                    ObjCo.PresentCityId = int.Parse(dr["PresentCityid"].ToString());
                    ObjCo.PermanentCityId = int.Parse(dr["PermanantCityid"].ToString());
                    ObjCo.PresentStateId = int.Parse(dr["PresentStateId"].ToString());
                    ObjCo.PermanentStateId = int.Parse(dr["PermanentStateId"].ToString());
                    ObjCo.PresentCountryId = int.Parse(dr["PresentCountryId"].ToString());
                    ObjCo.PermanentCountryId = int.Parse(dr["PermanentCountryId"].ToString());
                    ObjCo.Graduation = dr["GraduationId"].ToString();

                    ObjCo.PostGraduation = dr["PostGraduationId"].ToString();
					   if (ObjCo.Graduation.ToString() != null)
                    {
                        ObjCo.GraduationId = 0;
                    }
                    else
                    {
                        ObjCo.GraduationId = int.Parse(ObjCo.Graduation);
                    }
                }
                ViewBag.PreCountryId = ObjCo.PresentCountryId;
                ViewBag.PreStateId = ObjCo.PresentStateId;
                ViewBag.PreCityId = ObjCo.PresentCityId;
                ViewBag.PrmCountryId = ObjCo.PermanentCountryId;
                ViewBag.PrmStateId = ObjCo.PermanentStateId;
                ViewBag.PrmCityId = ObjCo.PermanentCityId;
                ViewBag.graGraduation = ObjCo.GraduationId;
                ViewBag.poPostgraduationId = ObjCo.PostGraduation;

                await GetStateNameSk(ObjCo.PresentCountryId);
                await GetPermanantStateNameSk(ObjCo.PermanentCountryId);
                await GetCityNameSk(ObjCo.PresentStateId);
                await GetPermanantCityNameSk(ObjCo.PermanentStateId);
			
                await GetPostGraduationSK(ObjCo.GraduationId);


                return View(ObjCo);
            }

        }
        /// <summary>
        /// This method is used to save the data of student who is taking the admission.
        /// </summary>
        /// <param name="obj">using this object we pass the data to the BAL.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> IntExtAdmissionFormAM(Counsellor obj)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login","Account");            }
            else 
            {
                obj.StaffCode = Session["StaffCode"].ToString();
                obj.BranchCode = Session["BranchCode"].ToString();

                if (obj.UploadSSCYearFile != null)
                {
                    var newFileName = Path.GetFileNameWithoutExtension(obj.UploadSSCYearFile.FileName);
                    var extention = Path.GetExtension(obj.UploadSSCYearFile.FileName);
                    newFileName = Guid.NewGuid().ToString() + extention;
                    var file = "~/Content/Student/Docs/" + newFileName;
                    obj.SSCYearFile = file;
                    newFileName = Path.Combine(Server.MapPath("~/Content/Student/Docs/"), newFileName);
                    obj.UploadSSCYearFile.SaveAs(newFileName);
                }
                if (obj.UPloadHSCYearFile != null)
                {
                    var newFileName = Path.GetFileNameWithoutExtension(obj.UPloadHSCYearFile.FileName);
                    var extention = Path.GetExtension(obj.UPloadHSCYearFile.FileName);
                    newFileName = Guid.NewGuid().ToString() + extention;
                    var file = "~/Content/Student/Docs/" + newFileName;
                    obj.HSCYearFile = file;
                    newFileName = Path.Combine(Server.MapPath("~/Content/Student/Docs/"), newFileName);
                    obj.UPloadHSCYearFile.SaveAs(newFileName);
                }
                if (obj.UploadGraduationFile != null)
                {
                    var newFileName = Path.GetFileNameWithoutExtension(obj.UploadGraduationFile.FileName);
                    var extention = Path.GetExtension(obj.UploadGraduationFile.FileName);
                    newFileName = Guid.NewGuid().ToString() + extention;
                    var file = "~/Content/Student/Docs/" + newFileName;
                    obj.GraduationFile = file;
                    newFileName = Path.Combine(Server.MapPath("~/Content/Student/Docs/"), newFileName);
                    obj.UploadGraduationFile.SaveAs(newFileName);
                }
                if (obj.UPloadPGFile != null)
                {
                    var newFileName = Path.GetFileNameWithoutExtension(obj.UPloadPGFile.FileName);
                    var extention = Path.GetExtension(obj.UPloadPGFile.FileName);
                    newFileName = Guid.NewGuid().ToString() + extention;
                    var file = "~/Content/Student/Docs/" + newFileName;
                    obj.PGFile = file;
                    newFileName = Path.Combine(Server.MapPath("~/Content/Student/Docs/"), newFileName);
                    obj.UPloadPGFile.SaveAs(newFileName);
                }
                if (obj.UploadAadharCard != null)
                {
                    var newFileName = Path.GetFileNameWithoutExtension(obj.UploadAadharCard.FileName);
                    var extention = Path.GetExtension(obj.UploadAadharCard.FileName);
                    newFileName = Guid.NewGuid().ToString() + extention;
                    var file = "~/Content/Student/Docs/" + newFileName;
                    obj.AadharCard = file;
                    newFileName = Path.Combine(Server.MapPath("~/Content/Student/Docs/"), newFileName);
                    obj.UploadAadharCard.SaveAs(newFileName);
                }
                if (obj.UploadPanCard != null)
                {
                    var newFileName = Path.GetFileNameWithoutExtension(obj.UploadPanCard.FileName);
                    var extention = Path.GetExtension(obj.UploadPanCard.FileName);
                    newFileName = Guid.NewGuid().ToString() + extention;
                    var file = "~/Content/Student/Docs/" + newFileName;
                    obj.PanCard = file;
                    newFileName = Path.Combine(Server.MapPath("~/Content/Student/Docs/"), newFileName);
                    obj.UploadPanCard.SaveAs(newFileName);
                }
                if (obj.UploadExperienceLetter != null)
                {
                    var newFileName = Path.GetFileNameWithoutExtension(obj.UploadExperienceLetter.FileName);
                    var extention = Path.GetExtension(obj.UploadExperienceLetter.FileName);
                    newFileName = Guid.NewGuid().ToString() + extention;
                    var file = "~/Content/Student/Docs/" + newFileName;
                    obj.ExperienceLetter = file;
                    newFileName = Path.Combine(Server.MapPath("~/Content/Student/Docs/"), newFileName);
                    obj.UploadExperienceLetter.SaveAs(newFileName);
                }
                if (obj.UploadSalarySlip != null)
                {
                    var newFileName = Path.GetFileNameWithoutExtension(obj.UploadSalarySlip.FileName);
                    var extention = Path.GetExtension(obj.UploadSalarySlip.FileName);
                    newFileName = Guid.NewGuid().ToString() + extention;
                    var file = "~/Content/Student/Docs/" + newFileName;
                    obj.SalarySlip = file;
                    newFileName = Path.Combine(Server.MapPath("~/Content/Student/Docs/"), newFileName);
                    obj.UploadSalarySlip.SaveAs(newFileName);
                }
                if (obj.UploadPhoto != null)
                {
                    var newFileName = Path.GetFileNameWithoutExtension(obj.UploadPhoto.FileName);
                    var extention = Path.GetExtension(obj.UploadPhoto.FileName);
                    newFileName = Guid.NewGuid().ToString() + extention;
                    var file = "~/Content/Student/Docs/" + newFileName;
                    obj.Photo = file;
                    newFileName = Path.Combine(Server.MapPath("~/Content/Student/Docs/"), newFileName);
                    obj.UploadPhoto.SaveAs(newFileName);
                }

                string namePart = new string(obj.FullName.Replace(" ", "").Take(4).ToArray());
                if (namePart.Length < 4)
                {
                    namePart = namePart.PadRight(4, 'X'); // Pad with 'X' if less than 4 characters
                }

                // Extract the birthdate and birthmonth
                string dobPart = obj.DateOfBirth.ToString("ddMM");

                // Generate the password
                obj.Password = namePart + dobPart;



                if (obj.EnquiryFor == "Course")
                {
                    MailType = "CourseAdmission";
                    string input = obj.BatchCode;
                    var batchList = Session["BatchList"] as IEnumerable<SelectListItem>;

                    string selectedText = batchList?.FirstOrDefault(i => i.Value == input)?.Text;
                    if (selectedText != null)
                    {
                        int startIndex = selectedText.IndexOf('(') + 1;

                        // Find the position of the first closing parenthesis after the startIndex
                        int endIndex = selectedText.IndexOf(')', startIndex);
                        TentativeDate = selectedText.Substring(startIndex, endIndex - startIndex).Trim();
                    }
					else
					{
						TentativeDate = "NA";
					}
                }
                else
                {
                    MailType = "PlacementAdmission";
                }

                await objbal.SaveAdmissionkk(obj);
                if (obj.IsExperience == "Yes")
                {
                    var experienceDataJson = Request.Form["experienceData"];
                    List<Counsellor> experiencedata = JsonConvert.DeserializeObject<List<Counsellor>>(experienceDataJson);

                    obj.AddedExperiences = experiencedata;
                    await objbal.SaveExperiencekk(obj);
                }
                await objbal.UpdateQualificationkk(obj);
                await objbal.SaveAdmissionDocuments(obj);

                await objbal.SaveFeeCollectionkk(obj);
                await objbal.SaveTransactionkk(obj);
                //await SendMailkk(obj.EmailId, obj.FullName, MailType, obj.CandidateCode, 39, TentativeDate, obj.Password);

                var response = new
                {
                    success = "Admission done Successfully...!"
                };
                return Json(response);
            }

        }
		[HttpPost]
        public async Task<ActionResult> ValidateEmailId(string email)
        {
            Counsellor objC = new Counsellor();
            objC.EmailId = email;
            var response = "";
            SqlDataReader dr;
            dr = await  objbal.ValidateEmail(objC);
            if (dr.Read())
            {
                objC.EmailId = dr["EmailId"].ToString();
                response = "Email Already registered.";
            }
            else 
            {
                response = "Email Id does not exists";
            }
            return Json(response);
            //return View();
        }

		  [HttpGet]

  public async Task<ActionResult> DropOutStudentsAD(DateTime? Startdate, DateTime? Enddate, string SelectedCourseFilter)
  {
      Counsellor objdetials2 = new Counsellor();
      objdetials2.BranchCode = Session["BranchCode"].ToString();
      objdetials2.StartDate = Startdate.Value.Date;
      objdetials2.EndDate = Enddate.Value.Date;
      List<Counsellor> lstuserAD = new List<Counsellor>();

      DataSet ds = await objbal.DropoutStudentAD(objdetials2);
      if (ds != null && ds.Tables[0].Rows.Count > 0)
      {
          for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
          {
              Counsellor objcount = new Counsellor();
              objcount.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNumber"].ToString());

              objcount.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
              objcount.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
              objcount.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
              objcount.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
              objcount.AdmmissionDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["AdmmissionDate"].ToString());
              objcount.CourseName = ds.Tables[0].Rows[i]["CourseName"].ToString();

              lstuserAD.Add(objcount);
          }
          List<Counsellor> lstFilteredCourseAdmission = new List<Counsellor>();
          if (Startdate != null && Enddate != null)
          {
              DateTime modifiedStartDate = Startdate.Value.Date.AddDays(-1);
              DateTime modifiedEndDate = Enddate.Value.Date.AddDays(+1);
              lstFilteredCourseAdmission = lstuserAD.Where(item => item.AdmmissionDate > modifiedStartDate && item.AdmmissionDate < modifiedEndDate).ToList();
              objdetials2.lstuser = lstFilteredCourseAdmission;
          }
          else
          {
              objdetials2.lstuser = lstuserAD;
          }
          objdetials2.lstuser = lstuserAD;
      }
      return PartialView("DropOutStudentsAD", objdetials2);

  }
		
		/// <summary>
		/// This methoud shows todays followup list
		/// </summary>
		/// <returns></returns>

		public async Task<ActionResult> TodaysFollowupList()
		{

			if (Session["StaffCode"] == null)
			{
				return RedirectToAction("Login", "Account");

			}
			else
			{
				objCounsellor.BranchCode = Session["BranchCode"].ToString();

				Counsellor objc = new Counsellor();
				DataSet dsTD = await objbal.TodaysFollowupList(objCounsellor);
				List<Counsellor> lstfortodayfollowup = new List<Counsellor>();

				foreach (DataRow row in dsTD.Tables[0].Rows)
				{
					Counsellor obj = new Counsellor();
					obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
					obj.CandidateCode = row["CandidateCode"].ToString();
					obj.FullName = row["FullName"].ToString();
					obj.ContactNumber = row["ContactNumber"].ToString();
					obj.EnquiryFor = row["EnquiryFor"].ToString();
					obj.LastFollowup = row["LastFollowUpNote"].ToString();
					if (DateTime.TryParse(row["LastFollowUpDate"].ToString(), out DateTime parsedDate))
					{
						obj.LastFollowupDate = parsedDate;
					}
					else
					{
						// Handle parsing failure, e.g., assign a default value
						obj.LastFollowupDate = DateTime.MinValue; // Or any appropriate default
					}

					lstfortodayfollowup.Add(obj);
				}
				objc.lstfortodaysfollowup = lstfortodayfollowup;
				return PartialView(objc);
			}
		}

		/// <summary>
		/// This method shows missed followup list
		/// </summary>
		/// <returns></returns>
		public async Task<ActionResult> MissedFollowupList()
		{

			if (Session["StaffCode"] == null)
			{
				return RedirectToAction("Login", "Account");

			}
			else
			{
				objCounsellor.BranchCode = Session["BranchCode"].ToString();

				Counsellor objc = new Counsellor();
				DataSet dsTD = await objbal.MissedFollowupList(objCounsellor);
				List<Counsellor> lstforMissedFollowup = new List<Counsellor>();

				foreach (DataRow row in dsTD.Tables[0].Rows)
				{
					Counsellor obj = new Counsellor();
					obj.SerialNo = Convert.ToInt32(row["SerialNo"].ToString());
					obj.CandidateCode = row["CandidateCode"].ToString();
					obj.FullName = row["FullName"].ToString();
					obj.ContactNumber = row["ContactNumber"].ToString();
					obj.LastFollowup = row["LastFollowUpTakenDate"].ToString();
					if (DateTime.TryParse(row["LastFollowUpTakenDate"].ToString(), out DateTime parsedDate))
					{
						obj.LastFollowupDate = parsedDate;
					}
					else
					{
						// Handle parsing failure, e.g., assign a default value
						obj.LastFollowupDate = DateTime.MinValue; // Or any appropriate default
					}
					if (DateTime.TryParse(row["MissedFollowUpDate"].ToString(), out DateTime parseddate))
					{
						obj.MissedFollowupDate = parseddate;
					}
					else
					{
						// Handle parsing failure, e.g., assign a default value
						obj.MissedFollowupDate = DateTime.MinValue; // Or any appropriate default
					}

					lstforMissedFollowup.Add(obj);
				}
				objc.lstforMissFollowup = lstforMissedFollowup;
				return PartialView(objc);
			}
		}
		
		        public async Task<ActionResult> FilterGraphEnqvsAdmission(DateTime? startDate, DateTime? endDate, string courseId)
        {
            if (Session["StaffCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }
            else
            {

                objCounsellor.BranchCode = Session["BranchCode"].ToString();

                // Check if a date range is provided
                DateTime StartDate, EndDate;
                if (startDate.HasValue && endDate.HasValue)
                {
                    StartDate = startDate.Value;
                    EndDate = endDate.Value;
                }
                else
                {
                    // Use default dates if no date range is provided
                    StartDate = DateTime.Now.AddYears(-1);
                    EndDate = DateTime.Now;
                }
                objCounsellor.StartDate = StartDate;
                objCounsellor.EndDate = EndDate;
                objCounsellor.CourseCode = courseId;
                DataSet ds=null;
                if (courseId=="")
                {
                     ds = await objbal.EnquiryVSAddmittedStudentCountBranchAsyncHP(objCounsellor);

                }
                else

                {
                     ds = await objbal.EnquiryVSAddmittedStudentCountBranchAsyncSS(objCounsellor);

                }

                objCounsellor.StartDate = StartDate;
                objCounsellor.EndDate = EndDate;
                objCounsellor.CourseCode = courseId;

                List<object> dataPoint = new List<object>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int year = Convert.ToInt32(row["year"]);
                    string monthss = row["month_name"].ToString();
                    string LeadCount = row["lead_count"].ToString();
                    string EnquiryCount = row["enquiry_count"].ToString();
                    string AdmissionCount = row["admitted_count"].ToString();

                    dataPoint.Add(new { Label = $"{monthss}-{year}", Lead = LeadCount, enquiry = EnquiryCount, Admission = AdmissionCount });
                }

                if (startDate.HasValue && endDate.HasValue)
                {
                    ViewBag.FilteredEnqVsAdmGraph = Newtonsoft.Json.JsonConvert.SerializeObject(dataPoint);
                }
                else
                {
                    ViewBag.DataPoint1 = Newtonsoft.Json.JsonConvert.SerializeObject(dataPoint);
                }

                var DataFetchedEnqvsAdmission = ViewBag.FilteredEnqVsAdmGraph;

                TempData["StartDate"] = StartDate.ToString("yyyy-MM-dd");
                TempData["EndDate"] = EndDate.ToString("yyyy-MM-dd");
                return Json(new { DataFetchedEnqvsAdmission }, JsonRequestBehavior.AllowGet);
            }
        }

		public async Task<ActionResult> ShowAllLostLeadListStudentMB(DateTime? Startdate, DateTime? Enddate, string enquiryfor)
 {

     if (Session["StaffCode"] == null)
     {
         return RedirectToAction("Login", "Account");
     }
     else
     {
         Counsellor objcoun = new Counsellor();
         objcoun.StartDate = Startdate.Value.Date;
         objcoun.EndDate = Enddate.Value.Date;
         objcoun.StaffCode = Session["StaffCode"].ToString();
         objcoun.BranchCode = Session["BranchCode"].ToString();
         objcoun.EnquirySource = "LostLead";
         DataSet ds = new DataSet();
         ds = await objbal.ShowAllLostLeadListStudentMB(objcoun);
         List<Counsellor> studlist = new List<Counsellor>();
         for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
         {
             Counsellor objcoun1 = new Counsellor();
             objcoun1.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
             objcoun1.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
             objcoun1.EquiryDescription = ds.Tables[0].Rows[i]["EquiryDescription"].ToString();
             objcoun1.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
             objcoun1.EnquiryFor = ds.Tables[0].Rows[i]["EnquiryFor"].ToString();
             objcoun1.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
             objcoun1.EmailId = ds.Tables[0].Rows[i]["EmailId"].ToString();
             objcoun1.ContactNumber = ds.Tables[0].Rows[i]["ContactNumber"].ToString();
             objcoun1.AlternateNumber = ds.Tables[0].Rows[i]["AlternateNumber"].ToString();
             //objcoun1.EnquirySource = ds.Tables[0].Rows[i]["SourceName"].ToString();
             objcoun1.Status = ds.Tables[0].Rows[i]["Status"].ToString();
             objcoun1.EnqStatusId = int.Parse(ds.Tables[0].Rows[i]["Statusid"].ToString());
             studlist.Add(objcoun1);
         }
         List<Counsellor> filteredlist = new List<Counsellor>();
         if (!string.IsNullOrEmpty(enquiryfor))
         {
             filteredlist = studlist.Where(e => e.EnquiryFor != null && e.EnquiryFor.StartsWith(enquiryfor)).ToList();
             objcoun.lstReferenceAsyncSS = filteredlist;
         }
         else
         {
             objcoun.lstReferenceAsyncSS = studlist;
         }
         return PartialView("WebListStudentAsyncSS", objcoun);
     }

 }


    }
}

