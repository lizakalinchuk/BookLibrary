using BookLibrary.Domain.Abstract;
using BookLibrary.Domain.Entities;
using BookLibrary.Domain.SendEmail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookLibrary.WebUI.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private IBookRepository repository;
        private ISendEmailManager sendMailManager;

        public BookController(IBookRepository repo, ISendEmailManager sendManager)
        {
            repository = repo;
            sendMailManager = sendManager;
        }

        public ViewResult List()
        {
            return View(repository.GetBooks());
        }

        public ViewResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddBook(Book book)
        {
            repository.AddBook(book);
            return Json(Url.Action("List", "Book"));
        }

        public ActionResult ChangeBookQuantity(int id)
        {
            return View(repository.GetBook(id));
        }

        [HttpPost]
        public ActionResult ChangeBookQuantity(Book book)
        {
            repository.ChangeBookQuantity(book);
            return Json(Url.Action("List", "Book"));
        }

        public ActionResult DeleteBook(int id)
        {
            repository.DeleteBook(id);
            return RedirectToAction("List");
        }

        public ActionResult TakeBook(Book book)
        {
            if (repository.CanTakeBook(book.BookId, HttpContext.User.Identity.Name))
            {
                repository.UpdateAvailableBooksCount(book);
                string email = repository.FindEmailByUser(HttpContext.User.Identity.Name);
                sendMailManager.Send(email, book.Title);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ShowHistory(int id)
        {
            return View(repository.ShowHistory(id));
        }
    }
}
