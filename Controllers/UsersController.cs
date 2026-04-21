using CadastralManagementSystem.Models;
using System;
using System.Web.Mvc;

namespace CadastralManagementSystem.Controllers
{
    public class UsersController : Controller
    {
        private DatabaseHelper db = new DatabaseHelper();

        // GET: Список всех пользователей
        public ActionResult Index()
        {
            // Проверка авторизации
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            // Проверка роли (только Admin)
            if (Session["UserRole"]?.ToString() != "Admin")
                return new HttpStatusCodeResult(403, "Доступ запрещен");

            var users = db.GetAllUsers();
            return View(users);
        }

        // GET: Создание пользователя
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (Session["UserRole"]?.ToString() != "Admin")
                return new HttpStatusCodeResult(403, "Доступ запрещен");

            return View();
        }

        // POST: Создание пользователя
        [HttpPost]
        public ActionResult Create(string fullName, string email, string password, string role)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (Session["UserRole"]?.ToString() != "Admin")
                return new HttpStatusCodeResult(403, "Доступ запрещен");

            if (db.UserExists(email))
            {
                ViewBag.Error = "Пользователь с таким email уже существует";
                return View();
            }

            var user = new UserTemp
            {
                FullName = fullName,
                Email = email,
                Password = password,
                Role = role,
                RegisteredAt = DateTime.Now,
                IsActive = true
            };

            db.AddUser(user);
            TempData["Success"] = "Пользователь успешно создан";
            return RedirectToAction("Index");
        }

        // GET: Редактирование пользователя
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (Session["UserRole"]?.ToString() != "Admin")
                return new HttpStatusCodeResult(403, "Доступ запрещен");

            // TODO: Добавить метод GetUserById в DatabaseHelper
            return View();
        }

        // GET: Удаление пользователя
        public ActionResult Delete(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (Session["UserRole"]?.ToString() != "Admin")
                return new HttpStatusCodeResult(403, "Доступ запрещен");

            // TODO: Добавить метод DeleteUser в DatabaseHelper
            TempData["Success"] = "Пользователь удален";
            return RedirectToAction("Index");
        }
    }
}