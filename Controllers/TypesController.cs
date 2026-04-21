using CadastralManagementSystem.Models;
using System.Web.Mvc;

namespace CadastralManagementSystem.Controllers
{
    public class TypesController : Controller
    {
        private DatabaseHelper db = new DatabaseHelper();

        // GET: Список типов объектов
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var role = Session["UserRole"]?.ToString();
            if (role != "Admin" && role != "Employee")
                return new HttpStatusCodeResult(403, "Доступ запрещен");

            var types = db.GetAllTypes();
            return View(types);
        }

        // GET: Создание типа
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var role = Session["UserRole"]?.ToString();
            if (role != "Admin" && role != "Employee")
                return new HttpStatusCodeResult(403, "Доступ запрещен");

            return View();
        }

        // POST: Создание типа
        [HttpPost]
        public ActionResult Create(CadastralObjectType type)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            // TODO: Добавить метод AddType в DatabaseHelper
            TempData["Success"] = "Тип объекта добавлен";
            return RedirectToAction("Index");
        }
    }
}