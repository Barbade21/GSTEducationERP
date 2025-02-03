using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GSTEducationERPLibrary.Student;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.Remoting;
using GSTEducationERPLibrary.Placement;
using GSTEducationERPLibrary.Coordinator;
using System.IO;
using GSTEducationERPLibrary.Trainer;
using System.Net;

namespace GSTEducationERP.Controllers
{
    public class StudentController : Controller
    {
        private readonly BALStudent objbal;
        // GET: Student

        public StudentController()
        {
            objbal = new BALStudent();
        }

        //******************************************Ankita*******************************************************************//
        public async Task<ActionResult> ConductedPendingTest()
        {
            if (Session["CandidateCode"] == null)
            {

                return await Task.Run(() => RedirectToAction("Login", "Account"));

            }
            else
            {
                Student objtr2 = new Student();
                objtr2.CandidateCode = Session["CandidateCode"].ToString();


                DataSet ds = new DataSet();
                ds = await objbal.ConductedPendingTest(objtr2);
                Student objdetail = new Student();
                List<Student> AllTestList1 = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student objtm = new Student();
                    objtm.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objtm.TestName = ds.Tables[0].Rows[i]["TestName"].ToString();
                    objtm.SectionName = ds.Tables[0].Rows[i]["SectionName"].ToString();
                    objtm.TopicName = ds.Tables[0].Rows[i]["TopicName"].ToString();
                    objtm.LabName = ds.Tables[0].Rows[i]["LabName"].ToString();

                    try
                    {
                        objtm.Date = Convert.ToDateTime(ds.Tables[0].Rows[i]["TestDate"].ToString());
                        objtm.Time = Convert.ToDateTime(ds.Tables[0].Rows[i]["TestTime"].ToString());
                    }
                    catch (FormatException ex)
                    {
                        // Handle the case where date or time conversion fails
                    }

                    objtm.Duration = ds.Tables[0].Rows[i]["Duration"].ToString();


                    float totalMarks;
                    if (float.TryParse(ds.Tables[0].Rows[i]["TotalMarks"].ToString(), out totalMarks))
                    {
                        objtm.TotalMarks = totalMarks;
                    }
                    else
                    {

                    }

                    objtm.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    AllTestList1.Add(objtm);
                }
                objdetail.AllTestList = AllTestList1;
                return PartialView("ConductedPendingTest", objdetail);
            }
        }
        public async Task<ActionResult> Fees()
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                try
                {
                    Student objF = new Student();
                    objF.CandidateCode = Session["CandidateCode"].ToString();
                    //objF.StudentStatus = Session["StatusId"].ToString();


                    DataSet ds = await objbal.ShowFees(objF);

                    if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    {
                        // Handle case where no data is returned
                        return new HttpStatusCodeResult(HttpStatusCode.NoContent, "No fees data found.");
                    }

                    List<Student> lstFeesDetails = new List<Student>();

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Student obj = new Student();
                        obj.Totalfee = Convert.ToDouble(row["TotalFees"].ToString());
                        obj.TotalPaidFees = row["paidFees"].ToString();
                        obj.TotalRemainingFees = Convert.ToDouble(row["RemainingFees"].ToString());

                        lstFeesDetails.Add(obj);
                    }

                    objF.lstFeesDetails = lstFeesDetails;
                    return PartialView("Fees", objF);
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine(ex.Message);
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Error fetching fees data.");
                }
            }
        }



        public async Task<ActionResult> ProjectViewAttendanceAP()
        {
            //int AssignProjectId = 3;
            if (Session["CandidateCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Student obja = new Student();
            obja.CandidateCode = Session["CandidateCode"].ToString();
            //obja.AssignProjectId = AssignProjectId;
            DataSet ds = await objbal.StudentViewProjectAttendance(obja);

            List<Student> lstviewprojectattendance = new List<Student>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Student objurser = new Student();
                objurser.AssignProjectId = Convert.ToInt32(ds.Tables[0].Rows[i]["AssignProjectId"]);
                objurser.Date = Convert.ToDateTime(ds.Tables[0].Rows[i]["Date"]);
                objurser.InTime = ds.Tables[0].Rows[i]["InTime"].ToString();
                objurser.ProjectName = ds.Tables[0].Rows[i]["ProjectName"].ToString();
                objurser.OutTime = ds.Tables[0].Rows[i]["OutTime"].ToString();
                objurser.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                lstviewprojectattendance.Add(objurser);
            }

            Student objassing = new Student
            {
                ProjectName = ds.Tables[0].Rows[0]["ProjectName"].ToString(),
                lstviewprojectattendance = lstviewprojectattendance
            };

            return PartialView("ProjectViewAttendanceAP", objassing); // Ensure this name matches your partial view
        }



        public async Task<ActionResult> EventDetails()
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student objStudent = new Student();
                objStudent.CandidateCode = Session["CandidateCode"].ToString();
                objStudent.StudentStatus = Session["StatusId"].ToString();

                DataSet DS = await objbal.ShowEventDetails(objStudent);
                List<Student> lstEventDetails = new List<Student>();

                foreach (DataRow row in DS.Tables[0].Rows)
                {

                    Student obj = new Student();

                    obj.EventName = row["eventsNames"].ToString();
                    obj.EventDate = row["StartDate"].ToString();


                    lstEventDetails.Add(obj);
                }
                objStudent.lstEventDetails = lstEventDetails;
                return PartialView(objStudent);
            }

        }

        public async Task<ActionResult> Topic()
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student objStudent = new Student();
                objStudent.CandidateCode = Session["CandidateCode"].ToString();
                objStudent.StudentStatus = Session["StatusId"].ToString();

                DataSet DS = await objbal.ShowTopicDetails(objStudent);
                List<Student> lstTopicAp = new List<Student>();

                foreach (DataRow row in DS.Tables[0].Rows)
                {

                    Student obj = new Student();

                    obj.TopicName = row["TopicName"].ToString();
                    obj.TopicDate = row["StartDate"].ToString();


                    lstTopicAp.Add(obj);
                }
                objStudent.lstTopicAp = lstTopicAp;
                return PartialView(objStudent);
            }
        }

        public async Task<ActionResult> TotalTest()
        {
            if (Session["CandidateCode"] == null)
            {

                return await Task.Run(() => RedirectToAction("Login", "Account"));

            }
            else
            {
                Student objtr2 = new Student();
                objtr2.CandidateCode = Session["CandidateCode"].ToString();


                DataSet ds = new DataSet();
                ds = await objbal.ShowtotalTest(objtr2);
                Student objdetail = new Student();
                List<Student> AllTestList1 = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student objtm = new Student();
                    objtm.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objtm.TestName = ds.Tables[0].Rows[i]["TestName"].ToString();
                    objtm.SectionName = ds.Tables[0].Rows[i]["SectionName"].ToString();
                    objtm.TopicName = ds.Tables[0].Rows[i]["TopicName"].ToString();
                    objtm.LabName = ds.Tables[0].Rows[i]["LabName"].ToString();

                    try
                    {
                        objtm.Date = Convert.ToDateTime(ds.Tables[0].Rows[i]["TestDate"].ToString());
                        objtm.Time = Convert.ToDateTime(ds.Tables[0].Rows[i]["TestTime"].ToString());
                    }
                    catch (FormatException ex)
                    {
                        // Handle the case where date or time conversion fails
                    }

                    objtm.Duration = ds.Tables[0].Rows[i]["Duration"].ToString();


                    float totalMarks;
                    if (float.TryParse(ds.Tables[0].Rows[i]["TotalMarks"].ToString(), out totalMarks))
                    {
                        objtm.TotalMarks = totalMarks;
                    }
                    else
                    {

                    }

                    objtm.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    AllTestList1.Add(objtm);
                }
                objdetail.AllTestList = AllTestList1;
                return PartialView("TotalTest", objdetail);
            }
        }


        public async Task<ActionResult> DashboardAP()
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student objStudent = new Student();
                objStudent.CandidateCode = Session["CandidateCode"].ToString();
                objStudent.StudentStatus = Session["StatusId"].ToString();

                /// <summary>
                /// This function for Student batch name
                /// </summary>
                /// <param name="objB"></param>
                /// <returns></returns>
                /// ////batch name
                SqlDataReader drbatchname;
                drbatchname = await objbal.StudentBatchNameShowGJ(objStudent);
                drbatchname.Read();
                objStudent.BatchName = (drbatchname["BatchName"].ToString());
                objStudent.BatchCode = (drbatchname["BatchCode"].ToString());
                drbatchname.Close();
                ViewBag.BatchName = objStudent.BatchName;


                //Statusmsg
                SqlDataReader StatusMsg;
                StatusMsg = await objbal.StudentStatus(objStudent);
                StatusMsg.Read();
                objStudent.StudentStatus = (StatusMsg["Status"].ToString());
                objStudent.Welcomeback = (StatusMsg["WelcomeMessage"].ToString()); ;
                StatusMsg.Close();
                ViewBag.MSG = objStudent.Welcomeback;
                ViewBag.Status = objStudent.StudentStatus;
                /// <summary>
                /// This Function will show the Assign test count 
                /// </summary>
                /// <param name="objP"></param>
                /// <returns></returns> 
                //Topic
                SqlDataReader drTopicName;
                drTopicName = await objbal.ShowtotalTopic(objStudent);
                drTopicName.Read();
                objStudent.Topic = (drTopicName["TopicName"].ToString());
                objStudent.TopicDate = (drTopicName["StartDate"].ToString());
                drTopicName.Close();
                ViewBag.Date = objStudent.TopicDate.ToString(); ;
                ViewBag.TopicName = objStudent.Topic;

                /// <summary>
                /// This Function Will show the Assign Test count
                /// </summary>
                /// <param name="objP"></param>
                /// <returns></returns>
                //ALLTest
                SqlDataReader drALLTest;
                drALLTest = await objbal.ShowTotalAssignTest(objStudent);
                drALLTest.Read();
                objStudent.TotalAssignmentTestCount = Convert.ToInt32(drALLTest["AllTest"].ToString());
                drALLTest.Close();
                ViewBag.AllTest = objStudent.TotalAssignmentTestCount;

                /// <summary>
                /// This Function Will show the Arrange Test count
                /// </summary>
                /// <param name="objP"></param>
                /// <returns></returns>
                //Arrange Test
                SqlDataReader drArrangeTest;
                drArrangeTest = await objbal.ShowTotalArrangeTest(objStudent);
                drArrangeTest.Read();
                objStudent.TotalArrangeTest = Convert.ToInt32(drArrangeTest["ArrangedTest"].ToString());
                drArrangeTest.Close();
                ViewBag.ArrangeTest = objStudent.TotalArrangeTest;

                /// <summary>
                /// This Function Will show the Conducted Test count
                /// </summary>
                /// <param name="objP"></param>
                /// <returns></returns>
                //ConductedTest
                SqlDataReader drConductedTest;
                drConductedTest = await objbal.ShowConductedTestCount(objStudent);
                drConductedTest.Read();
                objStudent.TotalConductedTestCount = Convert.ToInt32(drConductedTest["ConductedTest"].ToString());
                drConductedTest.Close();
                ViewBag.ConductedTest = objStudent.TotalConductedTestCount;

                /// <summary>
                /// This Function Will show the Pending Test count
                /// </summary>
                /// <param name="objP"></param>
                /// <returns></returns>
                //PendingTest
                SqlDataReader drPendingTest;
                drPendingTest = await objbal.ShowPendingTest(objStudent);
                drPendingTest.Read();
                objStudent.TotalPendingTest = Convert.ToInt32(drPendingTest["PendingTest"].ToString());
                drPendingTest.Close();
                ViewBag.PendingTest = objStudent.TotalPendingTest;

                /// <summary>
                /// This Function Will show the project name which is assign
                /// </summary>
                /// <param name="objP"></param>
                /// <returns></returns>
                //AssignProject
                SqlDataReader drProject;
                drProject = await objbal.ShowAssignProject(objStudent);
                drProject.Read();
                objStudent.ProjectPresentCount = Convert.ToInt32(drProject["PresentCountP"].ToString());
                objStudent.ProjectAbsentCount = Convert.ToInt32(drProject["AbsentCountP"].ToString());
                drProject.Close();
                ViewBag.ProjectPresentCount = objStudent.ProjectPresentCount;
                ViewBag.ProjectAbsentCount = objStudent.ProjectAbsentCount;



                /// <summary>
                /// This Function Will show the Upcoming Event Name and Date
                /// </summary>
                /// <param name="objP"></param>
                /// <returns></returns>
                //EventCount
                SqlDataReader drTotalEvent;
                drTotalEvent = await objbal.ShowEvent(objStudent);
                drTotalEvent.Read();
                objStudent.EventCount = (drTotalEvent["TotalEvent"].ToString());
                drTotalEvent.Close();
                ViewBag.TotalEvent = objStudent.EventCount;

                //show the Event Name And Date
                //EventName
                SqlDataReader drEventName;
                drEventName = await objbal.ShowEventName(objStudent);
                drEventName.Read();
                objStudent.EventName = (drEventName["eventsNames"].ToString());
                objStudent.EventDate = (drEventName["StartDate"].ToString());
                drEventName.Close();
                ViewBag.EventName = objStudent.EventName;
                ViewBag.EventDate = objStudent.EventDate;

                /// <summary>
                /// This function for Total course fees count 
                /// </summary>
                /// <param name="objB"></param>
                /// <returns></returns>
                //total fee
                SqlDataReader drTotalFee;
                drTotalFee = await objbal.ReadTotalFeesCountGJ(objStudent);
                drTotalFee.Read();
                objStudent.Totalfee = Convert.ToDouble(drTotalFee["DiscountTotalFees"].ToString());
                drTotalFee.Close();
                ViewBag.TotalFees = objStudent.Totalfee;

                /// <summary>
                /// This function for Paid fees count 
                /// </summary>
                /// <param name="objG"></param>
                /// <returns></returns>
                SqlDataReader drPaidFee;
                drPaidFee = await objbal.ReadPaidFeesCountGJ(objStudent);
                drPaidFee.Read();
                objStudent.TotalPaidFees = (drPaidFee["paidFees"].ToString());
                drPaidFee.Close();
                ViewBag.paidFees = objStudent.TotalPaidFees;


                /// <summary>
                /// This function for remaining fees count 
                /// </summary>
                /// <param name="objJ"></param>
                /// <returns></returns>
                // remaining fee
                SqlDataReader drRemainingFee;
                drRemainingFee = await objbal.ReadRemainingFeesCountGJ(objStudent);
                drRemainingFee.Read();
                objStudent.TotalRemainingFees = Convert.ToDouble(drRemainingFee["RemainingFees"].ToString());
                drRemainingFee.Close();
                ViewBag.RemainingFees = objStudent.TotalRemainingFees;


                await ShowListAP();

                return View();
            }
        }

        /// <summary>
        /// This function will show the Perticular student  Event list 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<ActionResult> EventList()
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student objStudent = new Student();
                objStudent.CandidateCode = Session["CandidateCode"].ToString();
                DataSet ds = new DataSet();
                ds = await objbal.EventListViewAP(objStudent);
                Student objD = new Student();
                List<Student> lstEventAP = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student objst = new Student();
                    objst.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objst.EventName = ds.Tables[0].Rows[i]["EventName"].ToString();
                    objst.NoOfParticipants = Convert.ToInt32(ds.Tables[0].Rows[i]["NoOfParticipants"].ToString());
                    objst.StartDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["StartDate"].ToString());
                    objst.EndDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EndDate"].ToString());
                    objst.Location = ds.Tables[0].Rows[i]["Location"].ToString();
                    objst.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                    objst.OrganizedBy = ds.Tables[0].Rows[i]["OrganizedBy"].ToString();
                    lstEventAP.Add(objst);

                }

                objD.lstEventAP = lstEventAP;
                return View(objD);
            }
        }

        /// <summary>
        /// This Function Will show the List which that student Schedule
        /// </summary>
        /// <param name="objP"></param>
        /// <returns></returns>
        public async Task<ActionResult> ShowListAP(/*String CandidateCode*/)
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student objStudent = new Student();
                objStudent.CandidateCode = Session["CandidateCode"].ToString();

                DataSet ds = new DataSet();
                ds = await objbal.ShowListViewAP(objStudent);
                Student objD = new Student();
                List<Student> lstTimetableAp = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student objst = new Student();
                    objst.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objst.SectionName = ds.Tables[0].Rows[i]["SectionName"].ToString();
                    objst.TopicName = ds.Tables[0].Rows[i]["TopicName"].ToString();
                    objst.StaffName = ds.Tables[0].Rows[i]["StaffName"].ToString();
                    objst.LabName = ds.Tables[0].Rows[i]["LabName"].ToString();
                    objst.Duration = ds.Tables[0].Rows[i]["Duration"].ToString();
                    objst.StartDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["StartDate"].ToString());
                    objst.EndDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EndDate"].ToString());
                    lstTimetableAp.Add(objst);

                }

                objD.lstTimetableAp = lstTimetableAp;
                return View(objD);
            }

        }


        ///////////////////////////////////////Akshata///////////////////////////////////

        /// <summary>
        /// THIS IS THE MAIN VIEW ON THAT THEIR ARE TWO TABS TOPIC AND PROJECT
        /// </summary>
        /// <param name=" "></param>
        /// <returns>IT RETURNS TABS FOR PROJECT AND TOPIC ON LOAD EVENT TOPIC SCHEDULE </returns>

        public ActionResult AllScheduleAsyncAPP()
        {

            return View();
        }
        /// <summary>
        /// THIS METHOD IS USED FOR FETCH STUDENT ASSIGNED SCHEDULE 
        /// </summary>
        /// <param name="CandidateCode"></param>
        /// <returns>IT RETURNS TOPIC WISE  STUDENT SCHEDULE </returns>
        [HttpGet]
        public async Task<ActionResult> StudentAssignScheduleAsyncAP(string Status)
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student obj = new Student();
                obj.CandidateCode = Session["CandidateCode"].ToString();
                DataSet ds = await objbal.StudentReadAssingScheduleAsync(obj);
                Student objdetail = new Student();
                List<Student> lstStudentAssignSchedule = new List<Student>();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)

                {
                    Student objedit = new Student();
                    objedit.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objedit.TopicId = Convert.ToInt32(ds.Tables[0].Rows[i]["TopicId"].ToString());
                    objedit.AssignScheduleId = Convert.ToInt32(ds.Tables[0].Rows[i]["AssignScheduleId"].ToString());
                    objedit.SectionName = ds.Tables[0].Rows[i]["SectionName"].ToString();
                    objedit.TopicName = ds.Tables[0].Rows[i]["TopicName"].ToString();
                    objedit.LabName = ds.Tables[0].Rows[i]["LabName"].ToString();
                    objedit.NoOfSessions = Convert.ToInt32(ds.Tables[0].Rows[i]["NoOfSessions"].ToString());
                    objedit.Duration = ds.Tables[0].Rows[i]["Duration"].ToString();
                    objedit.StartDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["StartDate"].ToString());
                    objedit.EndDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EndDate"].ToString());
                    objedit.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    lstStudentAssignSchedule.Add(objedit);
                }


                //   Session["lstStudentSchedule"] = lstStudentAssignSchedule;
                List<Student> objFilteredStudentList = new List<Student>();
                Session["lstStudentSchedule"] = lstStudentAssignSchedule;
                if (Status != null && Status != "")
                {
                    objFilteredStudentList = lstStudentAssignSchedule.Where(item => item.Status == Status).ToList();
                    objdetail.lstStudentAssignSchedule = objFilteredStudentList;
                }
                else
                {
                    objdetail.lstStudentAssignSchedule = lstStudentAssignSchedule;
                }

                SqlDataReader dr3;
                dr3 = await objbal.StudentBatchNameBS(obj);
                dr3.Read();
                obj.BatchName = (dr3["BatchName"].ToString());
                dr3.Close();
                ViewBag.BatchNameBS = obj.BatchName;

                return await Task.Run(() => PartialView(objdetail));
            }

        }
        /// <summary>
        /// THIS METHOD IS USED FOR SHOWING TOPICWISE ATTENDANCE OF CANDIDATE 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IT RETURNS TOPIC WISE  ATTENDANCE OF CANDIDATE  </returns>
        [HttpGet]
        public async Task<ActionResult> StudentAttendanceAsyncAP(int TopicId)
        {

            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student obja = new Student();
                obja.CandidateCode = Session["CandidateCode"].ToString();
                obja.TopicId = TopicId;
                DataSet ds = await objbal.StudentAttendanceAsync(obja);
                Student objassing = new Student();
                List<Student> lstAttendanceTopicSS = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student objurser = new Student();
                    objassing.TopicId = Convert.ToInt32(ds.Tables[0].Rows[i]["TopicId"].ToString());
                    objassing.TopicName = ds.Tables[0].Rows[i]["TopicName"].ToString();
                    objassing.StaffName = ds.Tables[0].Rows[i]["StaffName"].ToString();

                    lstAttendanceTopicSS.Add(objurser);
                    objurser.Date = Convert.ToDateTime(ds.Tables[0].Rows[i]["Date"].ToString());
                    objurser.Status = ds.Tables[0].Rows[i]["Status"].ToString();

                }
                objassing.lstAttendanceTopicSS = lstAttendanceTopicSS;
                return await Task.Run(() => PartialView(objassing));
            }
        }
        /// <summary>
        /// THIS METHOD IS USED TO SHOW  TOPIC WISE ASSIGNMENT TO CANDIDATE
        /// </summary>
        /// <param name="AssignScheduleId"></param>
        /// <returns>IT RETURNS ASSIGNMENT </returns>

        [HttpGet]
        public async Task<ActionResult> DetailsStudentViewAssignmentsAsync(int AssignScheduleId)
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student obja = new Student();
                obja.CandidateCode = Session["CandidateCode"].ToString();
                obja.AssignScheduleId = AssignScheduleId;
                DataSet ds = await objbal.StudentViewAssingmentAsync(obja);
                Student objassing = new Student();
                List<Student> lstviewassignment = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student objurser = new Student();
                    objurser.AssignmentId = Convert.ToInt32(ds.Tables[0].Rows[i]["AssignmentId"].ToString());
                    objurser.TopicName = ds.Tables[0].Rows[i]["TopicName"].ToString();
                    objurser.AssignmentFile = ds.Tables[0].Rows[i]["AssignmentFile"].ToString();
                    lstviewassignment.Add(objurser);
                }
                objassing.lstviewassignment = lstviewassignment;
                return await Task.Run(() => PartialView(objassing));
            }
        }
        /// <summary>
        /// THIS METHOD IS USED TO SHOW PROJECT SCHEDULE TO CANDIDATE
        /// </summary>
        /// <param name="CandidateCode"></param>
        /// <returns>IT RETUNS SCHEDULE OF PROJECT ON ASSIGNPROJECTID </returns>
        [HttpGet]
        public async Task<ActionResult> StudentAssignProjectScheduleAsyncAP(string CandidateCode)
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {

                Student obj = new Student();
                obj.CandidateCode = Session["CandidateCode"].ToString();
                DataSet ds = await objbal.StudentReadProjectScheduleAsync(obj);
                Student projectdetails = new Student();
                List<Student> lstAssignProject = new List<Student>();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)

                {
                    Student objedit = new Student();
                    objedit.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objedit.AssignProjectId = Convert.ToInt32(ds.Tables[0].Rows[i]["AssignProjectId"].ToString());
                    objedit.ProjectName = ds.Tables[0].Rows[i]["ProjectName"].ToString();
                    objedit.TechnologyName = ds.Tables[0].Rows[i]["TechnologyName"].ToString();
                    objedit.ClientName = ds.Tables[0].Rows[i]["ClientName"].ToString();
                    objedit.ProjectManager = ds.Tables[0].Rows[i]["ProjectManager"].ToString();
                    objedit.TeamLeaderFullName = ds.Tables[0].Rows[i]["TeamLeaderFullName"].ToString();
                    objedit.Duration = ds.Tables[0].Rows[i]["Duration"].ToString();
                    objedit.StartDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["StartDate"].ToString());
                    objedit.EndDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EndDate"].ToString());
                    objedit.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    lstAssignProject.Add(objedit);
                }
                projectdetails.lstAssignProject = lstAssignProject;

                SqlDataReader dr3;
                dr3 = await objbal.StudentBatchNameBS(obj);
                dr3.Read();
                obj.BatchName = (dr3["BatchName"].ToString());
                dr3.Close();
                ViewBag.BatchNameBS = obj.BatchName;

                return await Task.Run(() => PartialView(projectdetails));
            }

        }
        /// <summary>
        /// THIS METHOD IS USED TO PROJECT ATTENDANCE
        /// </summary>
        /// <param name="AssignProjectId"></param>
        /// <returns>IT RETURNS PROJECT DATE AND STATUS </returns>
        public async Task<ActionResult> StudentProjectViewAttendanceAsync(int AssignProjectId)
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student obja = new Student();
                obja.CandidateCode = Session["CandidateCode"].ToString();
                obja.AssignProjectId = AssignProjectId;
                DataSet ds = await objbal.StudentViewProjectAttendanceAsync(obja);
                Student objassing = new Student();
                List<Student> lstviewprojectattendance = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student objurser = new Student();
                    objurser.AssignProjectId = Convert.ToInt32(ds.Tables[0].Rows[i]["AssignProjectId"].ToString());
                    objurser.Date = Convert.ToDateTime(ds.Tables[0].Rows[i]["Date"].ToString());
                    objurser.InTime = ds.Tables[0].Rows[i]["InTime"].ToString();
                    objurser.OutTime = ds.Tables[0].Rows[i]["OutTime"].ToString();
                    objurser.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    lstviewprojectattendance.Add(objurser);
                    objassing.ProjectName = ds.Tables[0].Rows[i]["ProjectName"].ToString();
                }
                objassing.lstviewprojectattendance = lstviewprojectattendance;
                return await Task.Run(() => PartialView(objassing));
            }
        }



        //*************************************Nilofar**************************************************//
        /// <summary>
        /// THIS IS THE MAIN VIEW FOR LEAVE ON THIS THEIR ARE FOUR TABS.
        /// </summary>
        ///  /// <param name="candidateCode"></param>
        /// <returns>IT RETURNS TABS AND CARD NAME </returns>
        /// 

        [HttpGet]
        public async Task<ActionResult> LeaveMainViewAsyncNS()
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student objnew = new Student();
                objnew.CandidateCode = Session["CandidateCode"].ToString();
                SqlDataReader dr;
                dr = await objbal.GetLeavesCount(objnew);
                dr.Read();
                objnew.YearMonth = (dr["YearMonth"].ToString());
                objnew.TotalPresentCount = (dr["PresentCount"].ToString());
                objnew.TotalAbsentCount = (dr["AbsentCount"].ToString());
                dr.Close();
                ViewBag.YearMonth = objnew.YearMonth;
                ViewBag.PresentCount = objnew.TotalPresentCount;
                ViewBag.AbsentCount = objnew.TotalAbsentCount;

                dr = await objbal.StudentBatchNameBS(objnew);
                dr.Read();
                objnew.BatchName = (dr["BatchName"].ToString());
                dr.Close();
                ViewBag.BatchNameBS = objnew.BatchName;
                return View();
            }
        }
        /// <summary>
        /// GET METHOD FOR APPLY LEAVE
        /// </summary>
        ///  /// <param name="ApplyLeaveId"></param>
        /// <returns> IT RETURNS GET METHOD FOR APPLY LEAVE</returns>


        [HttpGet]
        public async Task<ActionResult> ApplyLeaveAsyncNSSSS()
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student ObjS = new Student();
                ObjS.CandidateCode = Session["CandidateCode"].ToString();
                return PartialView(ObjS);
            }

        }

        /// <summary>
        /// POST METHOD FOR APPLIED LEAVE
        /// </summary>
        ///  /// <param name="ApplyLeaveId"></param>
        /// <returns>THIS METHOD SAVES APPLIED LEAVE</returns>
        [HttpPost]
        public async Task<ActionResult> ApplyLeaveAsyncNSSSS(Student ObjS)
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                ObjS.CandidateCode = Session["CandidateCode"].ToString();
                if (ObjS.LeaveDocument != null && ObjS.LeaveDocument.ContentLength > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(ObjS.LeaveDocument.FileName);
                    var extension = Path.GetExtension(ObjS.LeaveDocument.FileName);
                    fileName = fileName + extension;
                    var filePath = "~/Content/Placement/docs/TestFiles/" + fileName; // Relative path
                    ObjS.UploadDocument = filePath;
                    fileName = Path.Combine(Server.MapPath("~/Content/Placement/docs/TestFiles/"), fileName);
                    ObjS.LeaveDocument.SaveAs(fileName);
                }
                else
                {
                    ObjS.UploadDocument = null;
                }
                await objbal.SaveApplyLeaveAsyncNS(ObjS);
                return RedirectToAction("LeaveMainViewAsyncNS");
            }

        }

        /// <summary>
        ///THIS METHOD FOR SHOWING CANDIDATES ALL LEAVES.
        /// </summary>
        ///  /// <param name="CandidateCode"></param>
        /// <returns> IT RETURNS CANDIDATES ALL LEAVES FROM STATUS APPROVED,PENDING,REJECTED</returns>
        [HttpGet]

        public async Task<ActionResult> ShowStudentAllLeaveAsyncNS()
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {

                Student objStudent = new Student();
                objStudent.CandidateCode = Session["CandidateCode"].ToString();
                DataSet ds = await objbal.ShowStudentAllLeaveAsyncNS(objStudent);
                Student objLeave = new Student();
                List<Student> lstStudentLeave = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student obju = new Student();

                    obju.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    obju.ApplyLeaveId = Convert.ToInt32(ds.Tables[0].Rows[i]["ApplyLeaveId"].ToString());
                    obju.AppliedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["AppliedDate"].ToString());
                    obju.Reason = ds.Tables[0].Rows[i]["Reason"].ToString();
                    obju.StartDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["StartDate"].ToString());
                    obju.EndDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EndDate"].ToString());
                    obju.NoOfDays = float.Parse(ds.Tables[0].Rows[i]["NoOfDays"].ToString());
                    obju.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    lstStudentLeave.Add(obju);
                }
                objLeave.lstStudentLeave = lstStudentLeave;

                return PartialView("ShowStudentAllLeaveAsyncNS", objLeave);
            }
        }

        /// <summary>
        /// THIS METHOD SHOWS APPROVED LEAVES OF THAT CANDIDATE
        /// </summary>
        ///  /// <param name="CandidateCode"></param>
        /// <returns>IT RETURNS APPROVED LEAVES FROM STATUS APPROVED</returns>
        public async Task<ActionResult> ApprovedLeavesAsyncNS()
        {

            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student objStudent = new Student();
                objStudent.CandidateCode = Session["CandidateCode"].ToString();
                DataSet ds = await objbal.GetApprovedLeavesAsyncNS(objStudent);
                Student objLeave = new Student();
                List<Student> lstApprovedLeave = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student obju = new Student();

                    obju.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    obju.ApplyLeaveId = Convert.ToInt32(ds.Tables[0].Rows[i]["ApplyLeaveId"].ToString());
                    obju.AppliedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["AppliedDate"].ToString());
                    obju.Reason = ds.Tables[0].Rows[i]["Reason"].ToString();
                    obju.StartDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["StartDate"].ToString());
                    obju.EndDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EndDate"].ToString());
                    obju.NoOfDays = float.Parse(ds.Tables[0].Rows[i]["NoOfDays"].ToString());
                    obju.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    obju.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    lstApprovedLeave.Add(obju);
                }
                objLeave.lstApprovedLeave = lstApprovedLeave;

                return PartialView("ApprovedLeavesAsyncNS", objLeave);
            }
        }
        /// <summary>
        /// THIS METHOD SHOWS REJECTED LEAVES OF THAT CANDIDATE
        /// </summary>
        ///  /// <param name="CandidateCode"></param>
        /// <returns IT RETURNS REJECTED LEAVES FROM STATUS REJECTED></returns>

        [HttpGet]
        public async Task<ActionResult> RejectedLeavesAsyncNS()
        {

            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student objStudent = new Student();
                objStudent.CandidateCode = Session["CandidateCode"].ToString();
                DataSet ds = await objbal.GetRejectedLeavesAsyncNS(objStudent);
                Student objLeave = new Student();
                List<Student> lstRejectedLeave = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student obju = new Student();

                    obju.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    obju.ApplyLeaveId = Convert.ToInt32(ds.Tables[0].Rows[i]["ApplyLeaveId"].ToString());
                    obju.AppliedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["AppliedDate"].ToString());
                    obju.Reason = ds.Tables[0].Rows[i]["Reason"].ToString();
                    obju.StartDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["StartDate"].ToString());
                    obju.EndDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EndDate"].ToString());
                    obju.NoOfDays = float.Parse(ds.Tables[0].Rows[i]["NoOfDays"].ToString());
                    obju.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    obju.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    lstRejectedLeave.Add(obju);
                }
                objLeave.lstRejectedLeave = lstRejectedLeave;
                return PartialView("RejectedLeavesAsyncNS", objLeave);
            }
        }
        /// <summary>
        /// THIS METHOD SHOWS PENDING LEAVES OF THAT CANDIDATE
        /// </summary>
        ///  /// <param name="CandidateCode"></param>
        /// <returns>IT RETURNS PENDING LEAVES FROM STATUS PENDING</returns>
        public async Task<ActionResult> PendingLeavesAsyncNS()
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student objStudent = new Student();
                objStudent.CandidateCode = Session["CandidateCode"].ToString();

                DataSet ds = await objbal.GetPendingLeavesAsyncNS(objStudent);
                Student objLeave = new Student();
                List<Student> lstPendingLeave = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student obju = new Student();

                    obju.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    obju.ApplyLeaveId = Convert.ToInt32(ds.Tables[0].Rows[i]["ApplyLeaveId"].ToString());
                    obju.AppliedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["AppliedDate"].ToString());
                    obju.Reason = ds.Tables[0].Rows[i]["Reason"].ToString();
                    obju.StartDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["StartDate"].ToString());
                    obju.EndDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EndDate"].ToString());
                    obju.NoOfDays = float.Parse(ds.Tables[0].Rows[i]["NoOfDays"].ToString());
                    obju.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    obju.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    lstPendingLeave.Add(obju);
                }
                objLeave.lstPendingLeave = lstPendingLeave;

                return PartialView("PendingLeavesAsyncNS", objLeave);
            }
        }

        /// <summary>
        /// GET METHOD FOR UPDATE PENDING LEAVES
        /// </summary>
        ///  /// <param name="ApplyLeaveId"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<ActionResult> UpdatePendingLeavesAsyncNS(int ApplyLeaveId)
        {
            if (Session["CandidateCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Student objS = new Student();
                objS.CandidateCode = Session["CandidateCode"].ToString();
                SqlDataReader dr;
                dr = await objbal.UpdateLeave(ApplyLeaveId);
                while (dr.Read())
                {

                    objS.ApplyLeaveId = Convert.ToInt32(dr["ApplyLeaveId"].ToString());
                    objS.ApplyDate = Convert.ToDateTime(dr["ApplyDate"].ToString());
                    objS.StartDate = Convert.ToDateTime(dr["StartDate"].ToString());
                    objS.EndDate = Convert.ToDateTime(dr["EndDate"].ToString());
                    objS.NoOfDays = float.Parse(dr["NoOfDays"].ToString());
                    objS.Reason = dr["Reason"].ToString();
                    objS.CandidateCode = dr["CandidateCode"].ToString();
                }
                return PartialView("UpdatePendingLeavesAsyncNS", objS);
            }
        }
        /// <summary>
        /// POST METHOD FOR UPDATE PENDING LEAVES.
        /// </summary>
        ///  /// <param name="ApplyLeaveId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UpdatePendingLeavesAsyncNS(Student objS)
        {
            if (Session["CandidateCode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Student objStudent = new Student();
                objStudent.CandidateCode = Session["CandidateCode"].ToString();
                await objbal.UpdatePendingLeaveAsyncNS(objS);
                TempData["AlertMessage"] = "Updated Successfully.........!";
                return RedirectToAction("LeaveMainViewAsyncNS");
            }

        }


        //*******************************Fayaj*************************************//

        /// <summary>
        /// This Method is show the Student Deatils.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> DetailsStudentfProfileFSAsync()
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student Objst = new Student();
                Objst.CandidateCode = Session["CandidateCode"].ToString();
                SqlDataReader dr;
                dr = await objbal.GetStudentDeatilsFS(Objst);
                while (dr.Read())
                {
                    Objst.CandidateCode = dr["CandidateCode"].ToString();
                    Objst.FullName = dr["FullName"].ToString();
                    Objst.AdmmissionDate = Convert.ToDateTime(dr["AdmmissionDate"].ToString());
                    Objst.DOB = Convert.ToDateTime(dr["DOB"].ToString());
                    Objst.ContactNumber = dr["ContactNumber"].ToString();
                    Objst.AlternateContact = dr["AlternateContact"].ToString();
                    Objst.PresentAddress = dr["PresentAddress"].ToString();
                    Objst.PermanantAddress = dr["PermanantAddress"].ToString();
                    Objst.EmailId = dr["EmailId"].ToString();
                    Objst.Gender = dr["Gender"].ToString();
                    Objst.CandidateCode = dr["CandidateCode"].ToString();
                    Objst.SSCYear = dr["SSCYear"].ToString();
                    Objst.HSCYear = dr["HSCYear"].ToString();
                    Objst.GraduationName = dr["GraduationName"].ToString();
                    Objst.GraduationYear = dr["GraduationYear"].ToString();
                    Objst.PostGraduationName = dr["PostGraduationName"].ToString();
                    Objst.PostGraduationYear = dr["PostGraduationYear"].ToString();
                }
                dr.Close();
                return PartialView("DetailsStudentfProfileFSAsync", Objst);
            }
        }

        /// <summary>
        /// This Function is Show the Student Document Show
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> DetailsStudentDocumetAsyncFS()
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));

            }
            else
            {
                Student Objstu = new Student();
                Objstu.CandidateCode = Session["CandidateCode"].ToString();
                SqlDataReader dr;
                dr = await objbal.GetStudentDacumentFS(Objstu);
                if (dr.Read())
                {
                    Objstu.PanCard = dr["PanCard"].ToString();
                    Objstu.AadharCard = dr["AadharCard"].ToString();
                    Objstu.SSC = dr["SSC"].ToString();
                    Objstu.HSC = dr["HSC"].ToString();
                    Objstu.Graduation = dr["Graduation"].ToString();
                    Objstu.PostGraduation = dr["PostGraduation"].ToString();
                    Objstu.Photograph = dr["Photograph"].ToString();

                }
                dr.Close();
                return await Task.Run(() => PartialView(Objstu));
            }

        }


        /// <summary>
        /// This Method use to Student Profile.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> StudentProfileFSAsync()
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student Objs = new Student();
                Objs.CandidateCode = Session["CandidateCode"].ToString();
                SqlDataReader dr;
                dr = await objbal.GetStudentProfileAsync(Objs);
                while (dr.Read())
                {
                    //Objs.CandidateCode = dr["CandidateCode"].ToString();
                    Objs.FullName = dr["FullName"].ToString();
                    Objs.Photograph = dr["Photograph"].ToString();
                    Objs.GraduationName = dr["GraduationName"].ToString();
                    Objs.PostGraduationName = dr["PostGraduationName"].ToString();
                    Objs.PresentAddress = dr["PresentAddress"].ToString();
                    //ObjCo.SkillNames = dr["SkillNames"].ToString();
                }
                dr.Close();
                return View("StudentProfileFSAsync", Objs);

            }

        }

        //***********************************Balaji*********************************//
        /// <summary>
        /// USED TO  SHOW COURSE DETAILS
        /// </summary>
        /// <param name="Objst"></param>
        /// <returns>list shows course details to candidates</returns>
        [HttpGet]

        public async Task<ActionResult> ListCandidateCourse(Student objst)
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student obj = new Student();
                obj.BranchCode = Session["BranchCode"].ToString();
                obj.CandidateCode = Session["CandidateCode"].ToString();
                DataSet ds = new DataSet();
                ds = await objbal.CandiCourseDetailsAsyncBS(obj);
                List<Student> ListCourseDetailsBS = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student objstud = new Student();
                    objstud.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objstud.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    objstud.CourseName = ds.Tables[0].Rows[i]["CourseName"].ToString();
                    objstud.SyllabusFile = ds.Tables[0].Rows[i]["SyllabusFile"].ToString();
                    objstud.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                    ListCourseDetailsBS.Add(objstud);
                }
                obj.ListCourseDetailsBS = ListCourseDetailsBS;


                SqlDataReader dr3;
                dr3 = await objbal.StudentBatchNameBS(obj);
                dr3.Read();
                obj.BatchName = (dr3["BatchName"].ToString());
                dr3.Close();
                ViewBag.BatchNameBS = obj.BatchName;
                // }
                return View(obj);
            }
        }


        //**************************************Prachi**********************************//
        public async Task<ActionResult> MainTestViewAsyncPA()
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                return View("MainTestViewAsyncPA");
            }

        }

        [HttpGet]
        public async Task<ActionResult> AllTestListAsyncPA()
        {
            if (Session["CandidateCode"] == null)
            {

                return await Task.Run(() => RedirectToAction("Login", "Account"));

            }
            else
            {
                Student objtr2 = new Student();
                objtr2.CandidateCode = Session["CandidateCode"].ToString();


                DataSet ds = new DataSet();
                ds = await objbal.AllTestList(objtr2);
                Student objdetail = new Student();
                List<Student> AllTestList1 = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student objtm = new Student();
                    objtm.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objtm.TestName = ds.Tables[0].Rows[i]["TestName"].ToString();
                    objtm.SectionName = ds.Tables[0].Rows[i]["SectionName"].ToString();
                    objtm.TopicName = ds.Tables[0].Rows[i]["TopicName"].ToString();
                    objtm.LabName = ds.Tables[0].Rows[i]["LabName"].ToString();

                    try
                    {
                        objtm.Date = Convert.ToDateTime(ds.Tables[0].Rows[i]["TestDate"].ToString());
                        objtm.Time = Convert.ToDateTime(ds.Tables[0].Rows[i]["TestTime"].ToString());
                    }
                    catch (FormatException ex)
                    {
                        // Handle the case where date or time conversion fails
                    }

                    objtm.Duration = ds.Tables[0].Rows[i]["Duration"].ToString();

                    // Parsing TotalMarks safely using float.TryParse
                    float totalMarks;
                    if (float.TryParse(ds.Tables[0].Rows[i]["TotalMarks"].ToString(), out totalMarks))
                    {
                        objtm.TotalMarks = totalMarks;
                    }
                    else
                    {
                        // Handle the case where TotalMarks conversion fails
                        // For example, assign a default value or log the error
                    }

                    objtm.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    AllTestList1.Add(objtm);
                }
                objdetail.AllTestList = AllTestList1;
                return PartialView("AllTestListAsyncPA", objdetail);
            }
        }




        public async Task<ActionResult> PendingTestListAsyncPA()
        {

            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                BALStudent objSt1 = new BALStudent();
                objSt1.CandidateCode = Session["CandidateCode"].ToString();

                DataSet ds = new DataSet();

                ds = await objSt1.PendingTestList();
                Student objtr2 = new Student();
                List<Student> AllTestList2 = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student objtm1 = new Student();
                    objtm1.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objtm1.CandidateCode = ds.Tables[0].Rows[i]["CandidateCode"].ToString();
                    objtm1.TestName = ds.Tables[0].Rows[i]["TestName"].ToString();
                    objtm1.SectionName = ds.Tables[0].Rows[i]["SectionName"].ToString();
                    objtm1.TopicName = ds.Tables[0].Rows[i]["TopicName"].ToString();
                    objtm1.LabName = ds.Tables[0].Rows[i]["LabName"].ToString();

                    try
                    {
                        objtm1.Date = Convert.ToDateTime(ds.Tables[0].Rows[i]["TestDate"].ToString());
                        objtm1.Time = Convert.ToDateTime(ds.Tables[0].Rows[i]["TestTime"].ToString());
                    }
                    catch (FormatException ex)
                    {
                        // Handle the case where date or time conversion fails
                    }

                    objtm1.Duration = ds.Tables[0].Rows[i]["Duration"].ToString();

                    // Parsing TotalMarks safely using float.TryParse
                    float totalMarks;
                    if (float.TryParse(ds.Tables[0].Rows[i]["TotalMarks"].ToString(), out totalMarks))
                    {
                        objtm1.TotalMarks = totalMarks;
                    }
                    else
                    {
                        // Handle the case where TotalMarks conversion fails
                        // For example, assign a default value or log the error
                    }

                    objtm1.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    AllTestList2.Add(objtm1);
                }
                objtr2.AllTestList = AllTestList2;
                return PartialView("PendingTestListAsyncPA", objtr2);
            }
        }




        public async Task<ActionResult> ConductedTestListAsyncPA()
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                BALStudent objSt5 = new BALStudent();
                objSt5.CandidateCode = Session["CandidateCode"].ToString();
                DataSet ds = new DataSet();

                ds = await objSt5.ConductedTestList();
                Student objtr5 = new Student();
                List<Student> ConductedTestList1 = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student objtm5 = new Student();
                    objtm5.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objtm5.TestName = ds.Tables[0].Rows[i]["TestName"].ToString();
                    objtm5.SectionName = ds.Tables[0].Rows[i]["SectionName"].ToString();
                    objtm5.TopicName = ds.Tables[0].Rows[i]["TopicName"].ToString();
                    objtm5.LabName = ds.Tables[0].Rows[i]["LabName"].ToString();

                    try
                    {
                        objtm5.Date = Convert.ToDateTime(ds.Tables[0].Rows[i]["TestDate"].ToString());
                        objtm5.Time = Convert.ToDateTime(ds.Tables[0].Rows[i]["TestTime"].ToString());
                    }
                    catch (FormatException ex)
                    {
                        // Handle the case where date or time conversion fails
                    }

                    objtm5.Duration = ds.Tables[0].Rows[i]["Duration"].ToString();

                    // Parsing TotalMarks safely using float.TryParse
                    float totalMarks;
                    if (float.TryParse(ds.Tables[0].Rows[i]["TotalMarks"].ToString(), out totalMarks))
                    {
                        objtm5.TotalMarks = totalMarks;
                    }
                    else
                    {
                        // Handle the case where TotalMarks conversion fails
                        // For example, assign a default value or log the error
                    }
                    objtm5.ObtainedMarks = ds.Tables[0].Rows[i]["ObtainedMarks"].ToString();


                    objtm5.Status = ds.Tables[0].Rows[i]["Status"].ToString();

                    objtm5.TestPaperFile = ds.Tables[0].Rows[i]["TestPaperFile"].ToString();

                    ConductedTestList1.Add(objtm5);
                }
                objtr5.ConductedTestList = ConductedTestList1;
                return PartialView("ConductedTestListAsyncPA", objtr5);
            }

        }



        public async Task<ActionResult> UpcomingTestListAsyncPA1()
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {

                BALStudent objSt = new BALStudent();
                objSt.CandidateCode = Session["CandidateCode"].ToString();
                DataSet ds = new DataSet();

                ds = await objSt.UpcomingTestList();
                Student objtr2 = new Student();
                List<Student> UpcomingTestList = new List<Student>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student objtm = new Student();
                    objtm.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objtm.CandidateCode = Session["CandidateCode"].ToString();
                    objtm.TestName = ds.Tables[0].Rows[i]["TestName"].ToString();
                    objtm.SectionName = ds.Tables[0].Rows[i]["SectionName"].ToString();
                    objtm.TopicName = ds.Tables[0].Rows[i]["TopicName"].ToString();
                    objtm.LabName = ds.Tables[0].Rows[i]["LabName"].ToString();

                    try
                    {
                        objtm.Date = Convert.ToDateTime(ds.Tables[0].Rows[i]["TestDate"].ToString());
                        objtm.Time = Convert.ToDateTime(ds.Tables[0].Rows[i]["TestTime"].ToString());
                    }
                    catch (FormatException ex)
                    {
                        // Handle the case where date or time conversion fails
                    }

                    objtm.Duration = ds.Tables[0].Rows[i]["Duration"].ToString();

                    // Parsing TotalMarks safely using float.TryParse
                    float totalMarks;
                    if (float.TryParse(ds.Tables[0].Rows[i]["TotalMarks"].ToString(), out totalMarks))
                    {
                        objtm.TotalMarks = totalMarks;
                    }
                    else
                    {
                        // Handle the case where TotalMarks conversion fails
                        // For example, assign a default value or log the error
                    }

                    objtm.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                    UpcomingTestList.Add(objtm);
                }
                objtr2.UpcomingTestList = UpcomingTestList;
                return PartialView("UpcomingTestListAsyncPA1", objtr2);
            }
        }


        //__________________________________Girija ________________________________

        /// <summary>
        /// This is amain view of student fees details
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> StudentFeeDetailsViewGJ()
        {

            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {          


                await ShowListFeesGJ();
                Student objstudent = new Student();
                objstudent.CandidateCode = Session["CandidateCode"].ToString();



                //course fees
                SqlDataReader drCourseFee;
                drCourseFee = await objbal.ReadCourseFeesGJ(objstudent);
                drCourseFee.Read();
                objstudent.CourseFee = (drCourseFee["CourseFee"].ToString());
                drCourseFee.Close();
                ViewBag.CourseFee1 = objstudent.CourseFee;

                //fees discount
                SqlDataReader drDiscount;
                drDiscount = await objbal.ReadFeesDiscountCountGJ(objstudent);
                drDiscount.Read();
                objstudent.Discount = (drDiscount["Discount"].ToString());

                objstudent.DiscountMessage = (drDiscount["DiscountMessage"].ToString());
                drDiscount.Close();
                ViewBag.Discount1 = objstudent.Discount;
                ViewBag.DiscountMessage = objstudent.DiscountMessage;



                //total fees
                SqlDataReader dr;
                dr = await objbal.ReadTotalFeesCountGG(objstudent);
                dr.Read();
                objstudent.TotalFeesCount = Convert.ToInt32(dr["TotalFees"].ToString());
                dr.Close();


                ViewBag.TotalFeesCount1G = objstudent.TotalFeesCount;


                //PAid fees

                SqlDataReader drPaidFees;
                drPaidFees = await objbal.ReadPaidFeesCountGG(objstudent);
                drPaidFees.Read();
                objstudent.PaidFeesCount = (drPaidFees["paidFees"].ToString());
                drPaidFees.Close();
                ViewBag.PaidFeesCount1G = objstudent.PaidFeesCount;

                //Remaining fees
                SqlDataReader drRemainingFees;
                drRemainingFees = await objbal.ReadRemainingFeesCountGG(objstudent);
                drRemainingFees.Read();
                objstudent.RemainingFeesCount = (drRemainingFees["RemainingFees"].ToString());
                drRemainingFees.Close();
                ViewBag.RemainingFeesCount1G = objstudent.RemainingFeesCount;

                //Batch name
                SqlDataReader drBatchName;
                drBatchName = await objbal.StudentBatchNameShowGJ(objstudent);
                drBatchName.Read();
                objstudent.BatchName = (drBatchName["BatchName"].ToString());
                drBatchName.Close();
                ViewBag.BatchName1G = objstudent.BatchName;

                // Installment dates & amounts
                //SqlDataReader dr4;
                //dr4 = await objbal.InstallmentAmountDates(objstudent);

                //List<DateTime> installmentDates = new List<DateTime>();
                //List<string> installmentAmounts = new List<string>();

                //if (dr4.HasRows)
                //{
                //    while (dr4.Read())
                //    {
                //        // Check if the message "Batch is not allocated" is returned
                //        if (dr4["InstallmentDate"].ToString() == "Batch is not allocated")
                //        {
                //            ViewBag.Message = "Batch is not allocated";
                //            break;
                //        }

                //        // If the InstallmentDate is not null, process the data
                //        if (dr4["InstallmentDate"] != DBNull.Value)
                //        {
                //            DateTime installmentDate = Convert.ToDateTime(dr4["InstallmentDate"].ToString());
                //            string installmentAmount = dr4["InstallmentAmount"].ToString();

                //            installmentDates.Add(installmentDate);
                //            installmentAmounts.Add(installmentAmount);
                //        }
                //    }
                //}
                //else
                //{
                //    // No data found, set a default message
                //    ViewBag.Message = "No installment dates found.";
                //}

                //dr4.Close();

                //// Pass the lists to the ViewBag if dates were found
                //if (installmentDates.Count > 0)
                //{
                //    ViewBag.InstallmentDates = installmentDates;
                //    ViewBag.InstallmentAmounts = installmentAmounts;
                //}

                //return View();


                // Installment dates & amounts
                SqlDataReader dr4;
                dr4 = await objbal.InstallmentAmountDates(objstudent);

                List<string> formattedInstallmentDates = new List<string>();
                List<string> installmentAmounts = new List<string>();

                if (dr4.HasRows)
                {
                    while (dr4.Read())
                    {
                        // Check if the message "Batch is not allocated" is returned
                        if (dr4["InstallmentDate"].ToString() == "Batch is not allocated")
                        {
                            ViewBag.Message = "Batch is not allocated";
                            break;
                        }

                        // If the InstallmentDate is not null, process the data
                        if (dr4["InstallmentDate"] != DBNull.Value)
                        {
                            DateTime installmentDate = Convert.ToDateTime(dr4["InstallmentDate"].ToString());
                            string formattedDate = installmentDate.ToString("dd-MMMM-yyyy"); // Format the date
                            string installmentAmount = dr4["InstallmentAmount"].ToString();

                            formattedInstallmentDates.Add(formattedDate);
                            installmentAmounts.Add(installmentAmount);
                        }
                    }
                }
                else
                {
                    // No data found, set a default message
                    ViewBag.Message = "No installment dates found.";
                }

                dr4.Close();

                // Pass the lists to the ViewBag if dates were found
                if (formattedInstallmentDates.Count > 0)
                {
                    ViewBag.InstallmentDates = formattedInstallmentDates;
                    ViewBag.InstallmentAmounts = installmentAmounts;
                }

                return View();
            }
        }

        /// <summary>
        /// This function for show the list of coourse fees
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ShowListFeesGJ()
        {
            Student objstud = new Student();

            try
            {
                objstud.CandidateCode = Session["CandidateCode"].ToString();
                DataSet ds = new DataSet();
                ds = await objbal.ShowListFeesDetailsGJ(objstud);
                List<Student> lstfeesDetailsGJ = new List<Student>();

                DateTime currentDate = DateTime.Now; // Get current date

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Student objst = new Student();

                    objst.CandidateCode = (ds.Tables[0].Rows[i]["CandidateCode"].ToString());
                    objst.SerialNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SerialNo"].ToString());
                    objst.ReciptCode = ds.Tables[0].Rows[i]["FeesCollectioncode"].ToString();
                    objst.TransactionNOCount = ds.Tables[0].Rows[i]["TransactionCount"].ToString();
                    objst.FeesType = ds.Tables[0].Rows[i]["FeesType"].ToString();
                    objst.InstallmentDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["FeeCollecteddate"].ToString());
                    objst.CreditAmount = Convert.ToInt32(ds.Tables[0].Rows[i]["TransactionAmount"].ToString());
                    objst.PaymentMode = ds.Tables[0].Rows[i]["PaymentMode"].ToString();

                    objst.TransactionId = ds.Tables[0].Rows[i]["TransactionID_checqueNumber"].ToString();
                    lstfeesDetailsGJ.Add(objst);

                    // Check if the installment date is after the current date and if it's the next installment
                    if (objst.InstallmentDate >= currentDate)
                    {
                        objstud.NextInstallmentDate = objst.InstallmentDate;
                    }
                }

                objstud.lstfeesDetailsGJ = lstfeesDetailsGJ;
                return View(objstud);
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, return an error view, or set an error message on objstud)
                // For example, you could set an error message on objstud:
                objstud.ErrorMessage = "An error occurred while processing your request. Please try again later.";
                // Optionally, log the exception (not shown here for brevity)

                // Return the view with the error message set on objstud
                return View(objstud);
            }
        }
        /// <summary>
        /// This function for view or show fees recipt
        /// </summary>
        /// <param name="ReciptCode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ShowFeeReciptGJ(string ReciptCode)
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student objstud = new Student();
                objstud.CandidateCode = Session["CandidateCode"].ToString();
                objstud.ReciptCode = ReciptCode; // Corrected parameter name

                try
                {
                    SqlDataReader dr = await objbal.ShowFeeReciptGJ(objstud);
                    if (dr != null && dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            objstud.ReciptCode = (dr["FeesCollectioncode"].ToString());
                            objstud.CandidateCode = (dr["CandidateCode"].ToString());
                            objstud.CourseName = (dr["CourseName"].ToString());
                            objstud.BatchName = (dr["BatchName"].ToString());
                            objstud.StudentName = (dr["FullName"].ToString());
                            objstud.AdmissionType = (dr["AdmissionType"].ToString());
                            objstud.TotalFees = Convert.ToInt32(dr["TotalFees"].ToString());
                            objstud.PaidFees = (dr["PaidFees"].ToString());
                            objstud.RemainingFees = (dr["RemainingFees"].ToString());
                            objstud.ReciptCode = dr["FeesCollectioncode"].ToString();
                            objstud.CreditAmount = Convert.ToInt32(dr["TransactionAmount"].ToString());
                            // objstud.DrawnOn = dr["DrawnOn"].ToString();
                            objstud.PaymentMode = dr["PaymentMode"].ToString();
                            objstud.TransactionId = dr["TransactionID_checqueNumber"].ToString();
                            objstud.ReceiptDate = Convert.ToDateTime(dr["FeeCollecteddate"].ToString());
                            objstud.Discount = (dr["Discount"].ToString());
                            objstud.StaffName = dr["StaffName"].ToString();
                        }
                    }
                    else
                    {
                        // Set an error message if no data is found
                        objstud.ErrorMessage = "No data found for the given receipt code.";
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions by setting an error message
                    objstud.ErrorMessage = "An error occurred while fetching the fee receipt: " + ex.Message;
                }

                return View(objstud);
            }
        }      

        /// <summary>
        /// THIS METHOD FOR SHOW OTHER FEES DETAILS POPUP
        /// </summary>
        /// <param name="Candidatecode"></param>
        /// <returns></returns>
        public async Task<ActionResult> popup(string Candidatecode)
        {
            if (Session["CandidateCode"] == null)
            {
                return await Task.Run(() => RedirectToAction("Login", "Account"));
            }
            else
            {
                Student objstudent = new Student();

                objstudent.CandidateCode = Session["CandidateCode"].ToString();
                //stationary fee
                SqlDataReader drStationaryFee;
                drStationaryFee = await objbal.stationaryfeecount(objstudent);
                drStationaryFee.Read();
                objstudent.StationaryFeecount = Convert.ToInt32(drStationaryFee["TransactionAmount"].ToString());
                drStationaryFee.Close();
                ViewBag.StationaryFeecount1G = objstudent.StationaryFeecount;


                // Event fee
                SqlDataReader drEventFees;
                drEventFees = await objbal.Eventsfeecount(objstudent);
                drEventFees.Read();
                objstudent.EventFeesCount = Convert.ToInt32(drEventFees["TransactionAmount"].ToString());
                drEventFees.Close();
                ViewBag.EventFeesCount1G = objstudent.EventFeesCount;

                // Library fee
                SqlDataReader drLibraryFee;
                drLibraryFee = await objbal.Libraryfeecount(objstudent);
                drLibraryFee.Read();
                objstudent.LibraryFeecount = Convert.ToInt32(drLibraryFee["TransactionAmount"].ToString());
                drLibraryFee.Close();
                ViewBag.LibraryFeecount1G = objstudent.LibraryFeecount;


                // Exam fee
                SqlDataReader drExamFee;
                drExamFee = await objbal.Examfeecount(objstudent);
                drExamFee.Read();
                objstudent.ExamFeecount = Convert.ToInt32(drExamFee["TransactionAmount"].ToString());
                drExamFee.Close();
                ViewBag.ExamFeecount1G = objstudent.ExamFeecount;


                // Activity fee
                SqlDataReader drActivityFees;
                drActivityFees = await objbal.Extra_Currriculum_ActivityFeeCount(objstudent);
                drActivityFees.Read();
                objstudent.ActivityFeesCount = Convert.ToInt32(drActivityFees["TransactionAmount"].ToString());
                drActivityFees.Close();
                ViewBag.ActivityFeesCount1G = objstudent.ActivityFeesCount;


                // Late fee
                SqlDataReader drLateFee;
                drLateFee = await objbal.Latefeecount(objstudent);
                drLateFee.Read();
                objstudent.LateFeecount = Convert.ToInt32(drLateFee["TransactionAmount"].ToString());
                drLateFee.Close();
                ViewBag.LateFeecount1G = objstudent.LateFeecount;

                return PartialView("popup");
            }
        }

    }

}







