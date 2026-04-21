using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CadastralManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.UserRole = Session["UserRole"]?.ToString();
            ViewBag.UserName = Session["UserFullName"]?.ToString();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Система кадастрового управления.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Контакты кадастровой палаты.";
            return View();
        }
    }
}