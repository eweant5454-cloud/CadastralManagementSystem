using CadastralManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CadastralManagementSystem.Controllers
{
    public class ApplicationsController : Controller
    {
        private DatabaseHelper db = new DatabaseHelper();

        // GET: Все заявки (для сотрудников)
        public ActionResult Index(string status = "")
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var role = Session["UserRole"]?.ToString();
            if (role != "Admin" && role != "Employee")
                return new HttpStatusCodeResult(403, "Доступ запрещен");

            var applications = db.GetAllApplications(status);
            ViewBag.StatusFilter = status;
            return View(applications);
        }

        // GET: Мои заявки (для заявителя)
        // GET: Мои заявки (для заявителя)
        public ActionResult MyApplications()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];

            // Получаем заявки текущего пользователя
            var applications = new List<Application>();

            using (var conn = new System.Data.SqlClient.SqlConnection(
                System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                string sql = @"SELECT a.Id, a.ApplicantId, u.FullName, a.CadastralObjectTypeId, t.Name, 
                              a.Address, a.Area, a.ApplicantComment, a.Status, a.SubmissionDate, 
                              a.ProcessedDate, a.EmployeeComment, a.ResultingCadastralNumber
                       FROM Applications a
                       JOIN Users u ON a.ApplicantId = u.Id
                       JOIN CadastralObjectTypes t ON a.CadastralObjectTypeId = t.Id
                       WHERE a.ApplicantId = @UserId
                       ORDER BY a.SubmissionDate DESC";

                using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            applications.Add(new Application
                            {
                                Id = reader.GetInt32(0),
                                ApplicantId = reader.GetInt32(1),
                                ApplicantName = reader.GetString(2),
                                CadastralObjectTypeId = reader.GetInt32(3),
                                CadastralObjectTypeName = reader.GetString(4),
                                Address = reader.GetString(5),
                                Area = reader.IsDBNull(6) ? (double?)null : Convert.ToDouble(reader.GetValue(6)),
                                ApplicantComment = reader.IsDBNull(7) ? null : reader.GetString(7),
                                Status = reader.GetString(8),
                                SubmissionDate = reader.GetDateTime(9),
                                ProcessedDate = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10),
                                EmployeeComment = reader.IsDBNull(11) ? null : reader.GetString(11),
                                ResultingCadastralNumber = reader.IsDBNull(12) ? null : reader.GetString(12)
                            });
                        }
                    }
                }
            }

            return View(applications);
        }

        // GET: Подать заявку
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Types = db.GetAllTypes();
            return View();
        }

        // POST: Подать заявку
        [HttpPost]
        public ActionResult Create(Application application)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            application.ApplicantId = (int)Session["UserId"];
            application.SubmissionDate = DateTime.Now;
            application.Status = "Pending";

            db.AddApplication(application);
            TempData["Success"] = "Заявка успешно подана";
            return RedirectToAction("MyApplications");
        }

        // GET: Обработка заявки (одобрить/отклонить)
        [HttpGet]

        public ActionResult Process(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var role = Session["UserRole"]?.ToString();
            if (role != "Admin" && role != "Employee")
                return new HttpStatusCodeResult(403, "Доступ запрещен");

            // Получаем заявку по ID
            Application application = null;

            using (var conn = new System.Data.SqlClient.SqlConnection(
                System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                string sql = @"SELECT a.Id, a.ApplicantId, u.FullName, a.CadastralObjectTypeId, t.Name, 
                              a.Address, a.Area, a.ApplicantComment, a.Status, a.SubmissionDate, 
                              a.ProcessedDate, a.EmployeeComment, a.ResultingCadastralNumber
                       FROM Applications a
                       JOIN Users u ON a.ApplicantId = u.Id
                       JOIN CadastralObjectTypes t ON a.CadastralObjectTypeId = t.Id
                       WHERE a.Id = @Id";

                using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            application = new Application
                            {
                                Id = reader.GetInt32(0),
                                ApplicantId = reader.GetInt32(1),
                                ApplicantName = reader.GetString(2),
                                CadastralObjectTypeId = reader.GetInt32(3),
                                CadastralObjectTypeName = reader.GetString(4),
                                Address = reader.GetString(5),
                                Area = reader.IsDBNull(6) ? (double?)null : Convert.ToDouble(reader.GetValue(6)),
                                ApplicantComment = reader.IsDBNull(7) ? null : reader.GetString(7),
                                Status = reader.GetString(8),
                                SubmissionDate = reader.GetDateTime(9),
                                ProcessedDate = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10),
                                EmployeeComment = reader.IsDBNull(11) ? null : reader.GetString(11),
                                ResultingCadastralNumber = reader.IsDBNull(12) ? null : reader.GetString(12)
                            };
                        }
                    }
                }
            }

            if (application == null)
                return HttpNotFound();

            return View(application);
        }

        // POST: Одобрить заявку
        [HttpPost]
        public ActionResult Approve(int id, string cadastralNumber, string employeeComment)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            // Получаем заявку (упрощенно, в реальном проекте нужен метод GetApplicationById)
            Application app = null;
            using (var conn = new System.Data.SqlClient.SqlConnection(
                System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                string sql = "SELECT ApplicantId, CadastralObjectTypeId, Address, Area FROM Applications WHERE Id = @Id";
                using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            app = new Application
                            {
                                ApplicantId = reader.GetInt32(0),
                                CadastralObjectTypeId = reader.GetInt32(1),
                                Address = reader.GetString(2),
                                Area = reader.IsDBNull(3) ? (double?)null : Convert.ToDouble(reader.GetValue(3))
                            };
                        }
                    }
                }
            }

            if (app != null)
            {
                // Получаем имя заявителя
                string ownerName = "";
                using (var conn = new System.Data.SqlClient.SqlConnection(
                    System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    conn.Open();
                    string sql = "SELECT FullName FROM Users WHERE Id = @Id";
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", app.ApplicantId);
                        ownerName = cmd.ExecuteScalar()?.ToString() ?? "Не указан";
                    }
                }

                // Создаем кадастровый объект
                db.CreateObjectFromApplication(cadastralNumber, app.CadastralObjectTypeId, app.Address, app.Area, ownerName);
            }

            db.UpdateApplicationStatus(id, "Approved", employeeComment, cadastralNumber);
            TempData["Success"] = "Заявка одобрена, объект зарегистрирован";
            return RedirectToAction("Index");
        }

        // POST: Отклонить заявку
        [HttpPost]
        public ActionResult Reject(int id, string employeeComment)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            db.UpdateApplicationStatus(id, "Rejected", employeeComment);
            TempData["Success"] = "Заявка отклонена";
            return RedirectToAction("Index");
        }
    }
}