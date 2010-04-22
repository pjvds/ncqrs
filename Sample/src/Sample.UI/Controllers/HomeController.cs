using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sample.UI.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Welcome()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Error()
        {
            var error = (Exception)Session["Error"];
            return View(error);
        }
    }
}