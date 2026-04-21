using CadastralManagementSystem.Models;
using System;
using System.Web.Mvc;

namespace CadastralManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private DatabaseHelper db = new DatabaseHelper();

        // GET: Страница входа
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Обработка входа
        [HttpPost]
        public ActionResult Login(string email, string password, string returnUrl)
        {
            var user = db.GetUser(email, password);
            if (user != null)
            {
                // Сохраняем пользователя в сессию
                Session["UserId"] = user.Id;
                Session["UserRole"] = user.Role;
                Session["UserEmail"] = user.Email;
                Session["UserFullName"] = user.FullName;

                // Перенаправление
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Неверный email или пароль";
            return View();
        }

        // GET: Страница регистрации
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Обработка регистрации
        [HttpPost]
        public ActionResult Register(string fullName, string email, string password, string confirmPassword)
        {
            // Валидация
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Все поля обязательны для заполнения";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Пароли не совпадают";
                return View();
            }

            if (db.UserExists(email))
            {
                ViewBag.Error = "Пользователь с таким email уже существует";
                return View();
            }

            // Создаем пользователя (роль Applicant по умолчанию)
            var newUser = new UserTemp
            {
                FullName = fullName,
                Email = email,
                Password = password,
                Role = "Applicant",
                RegisteredAt = DateTime.Now,
                IsActive = true
            };

            db.AddUser(newUser);

            // Автоматический вход после регистрации
            var user = db.GetUser(email, password);
            Session["UserId"] = user.Id;
            Session["UserRole"] = user.Role;
            Session["UserEmail"] = user.Email;
            Session["UserFullName"] = user.FullName;

            return RedirectToAction("Index", "Home");
        }

        // Выход из системы
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}