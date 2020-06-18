using MVC1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC1.Controllers
{
    public class HomeController : Controller
    {
        private Dictionary<string,string> _users;
        public HomeController()
        {
            System.Web.HttpContext.Current.Application.Lock();
            _users =(Dictionary<string, string>)System.Web.HttpContext.Current.Application.Get("Users");
            System.Web.HttpContext.Current.Application.UnLock();
            
            
        }
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserModel user)
        {
            if (ModelState.IsValid)
            {
                bool IsValidUser = _users.Any(x => x.Key == user.UserName && x.Value == user.Password);

                if (IsValidUser)
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    if (!string.IsNullOrEmpty(Request.QueryString["fromsite"]))
                    {
                        Response.Redirect($"{Request.QueryString["fromsite"]}{Request.QueryString["ReturnUrl"]}" );
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }                    
                }
            }
            ModelState.AddModelError("", "invalid Username or Password");
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(UserModel registerUser)
        {
            if (ModelState.IsValid)
            {
                _users.Add(registerUser.UserName,registerUser.Password);
                System.Web.HttpContext.Current.Application.Lock();
                System.Web.HttpContext.Current.Application.Set("Users",_users);
                System.Web.HttpContext.Current.Application.UnLock();
                return RedirectToAction("Login");

            }
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}