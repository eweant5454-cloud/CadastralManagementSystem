using CadastralManagementSystem.Models;
using System.Web.Mvc;

namespace CadastralManagementSystem.Controllers
{
    public class ObjectsController : Controller
    {
        private DatabaseHelper db = new DatabaseHelper();

        // GET: Список кадастровых объектов
        public ActionResult Index(string search = "")
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var objects = db.GetAllObjects(search);
            ViewBag.Search = search;
            return View(objects);
        }

        // GET: Детали объекта
        public ActionResult Details(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            // TODO: Добавить метод GetObjectById в DatabaseHelper
            return View();
        }
    }
}