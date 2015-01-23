using System.Collections.Generic;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace ASP.NET_MVC_Game.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public JsonResult Get()
        {
            System.Web.HttpContext.Current.Application.Lock();
            var status = System.Web.HttpContext.Current.Application.Get("Status");
            System.Web.HttpContext.Current.Application.UnLock();
            if (status != null)
            {
                System.Web.HttpContext.Current.Application.Lock();
                var userName = System.Web.HttpContext.Current.Application.Get("UserName");
                var value = System.Web.HttpContext.Current.Application.Get("Value");
                System.Web.HttpContext.Current.Application.UnLock();
                return Json( new { Status =  status ,  Message = "", Name = userName, Value = value}, JsonRequestBehavior.AllowGet);
            }
            status = Status.NotStarted;
            //HttpContext.Current.Application.Set("Status", Status.NotStarted);
            return Json ( new { Status = Status.NotStarted, Message = "" }, JsonRequestBehavior.AllowGet );
        }

        [HttpPost]
        public ActionResult Set(int value, string userName = "")
        {
            System.Web.HttpContext.Current.Application.Lock();
            var fromApplication = System.Web.HttpContext.Current.Application.Get("Value");
            var status = System.Web.HttpContext.Current.Application.Get("Status");
            var userHistory = (List<string>)System.Web.HttpContext.Current.Application.Get(userName);
            if (userHistory != null)
            {
                userHistory.Add(value.ToString());
            }
            else
            {
                userHistory = new List<string> { value.ToString() };
            }
            System.Web.HttpContext.Current.Application.Set(userName, userHistory);
            System.Web.HttpContext.Current.Application.UnLock();

            if (status == null || status.ToString() == "Finished")
            {
                System.Web.HttpContext.Current.Application.Lock();
                System.Web.HttpContext.Current.Application.Set("Status", Status.Started);
                System.Web.HttpContext.Current.Application.Set("Value", value);
                System.Web.HttpContext.Current.Application.UnLock();
                return Json( new { Status = Status.Starter, Message = "Поздравляем!!! Вы начали новую игру!!! :-)", More = "No", History = userHistory }, JsonRequestBehavior.AllowGet);
            }
            else if (fromApplication.ToString() == value.ToString())
            {
                System.Web.HttpContext.Current.Application.Lock();
                System.Web.HttpContext.Current.Application.Set("Status", Status.Finished);
                System.Web.HttpContext.Current.Application.Set("UserName", userName);
                System.Web.HttpContext.Current.Application.UnLock();                
                return Json(new
                {
                    Status = Status.Winner,
                    Message = "Поздравляем!!! Вы угадали число!!! :-)",
                    More = "No",
                    Value = fromApplication,
                    History = userHistory
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return (int)fromApplication > value ? Json(new{ Status = Status.Started, Message = "", More = "No", History = userHistory }, JsonRequestBehavior.AllowGet) :
                    Json( new { Status = Status.Started, Message = "", More = "Yes", History = userHistory }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}