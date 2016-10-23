using BookLibrary.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Domain.Abstract
{
    public interface IBookRepository
    {
         List<Book> GetBooks();
         void AddBook(Book book);
         void DeleteBook(int id);
         void ChangeBookQuantity(Book book);
         Book GetBook(int id);
         void UpdateAvailableBooksCount(Book book);
         bool CanTakeBook(int Bookid, string userName);
         List<History> ShowHistory(int bookId);
         string FindEmailByUser(string userName);
    }
}
