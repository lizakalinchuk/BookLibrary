using BookLibrary.Domain.Abstract;
using BookLibrary.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BookLibrary.WebUI.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private IAccountRepository accountManager;

        public AccountController(IAccountRepository manager)
        {
            accountManager = manager;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User model)
        {
            if (ModelState.IsValid)
            {
                if (ValidateUser(model.UserName, model.UserEmail))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, true);
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User model)
        {
            if (!ValidateUser(model.UserName, model.UserEmail))
            {
                accountManager.AddUser(model.UserName, model.UserEmail);
                FormsAuthentication.SetAuthCookie(model.UserName, true);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }

        private bool ValidateUser(string login, string email)
        {
            return accountManager.IsUserExistInDB(login, email);
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}
