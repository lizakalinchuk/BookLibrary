using BookLibrary.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Domain.Abstract
{
    public class BookRepository : IBookRepository
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["LibraryConnectionString"].ConnectionString;

        public List<Book> GetBooks()
        {
            List<Book> listOfBooks = new List<Book>();
            string sqlExpression = "SELECT Books.BookId, Books.BookTitle, Books.AvailableBooks, Books.AllBooks, Authors.AuthorName FROM Books " +
                                    "INNER JOIN Authors_Books ON Books.BookId = Authors_Books.BookId " +
                                    "INNER JOIN Authors ON Authors.AuthorId = Authors_Books.AuthorId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sqlExpression, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int bookId = (int)reader.GetValue(0);
                            string title = (string)reader.GetValue(1);
                            int availableBooks = (int)reader.GetValue(2);
                            int allBooks = (int)reader.GetValue(3);
                            string author = (string)reader.GetValue(4);
                            Book temp = listOfBooks.Find(book => book.Title == title);
                            if (temp != null)
                            {
                                temp.Authors.Add(new Author() { AuthorName = author });
                            }
                            else
                            {
                                listOfBooks.Add(new Book()
                                {
                                    BookId = bookId,
                                    Title = title,
                                    AvailableBooks = availableBooks,
                                    AllBooks = allBooks,
                                    Authors = new List<Author>() { new Author() { AuthorName = author } }
                                });
                            }
                        }
                    }
                }
            }
            return listOfBooks;
        }

        public void AddBook(Book book)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    SqlTransaction transaction = connection.BeginTransaction();
                    command.Transaction = transaction;

                    try
                    {
                        command.CommandText = "INSERT INTO Books VALUES(@Title, @AllBooks, @AvailableBooks)";
                        SqlParameter titleBookParam = new SqlParameter("@Title", book.Title);
                        SqlParameter countBookParam = new SqlParameter("@AllBooks", book.AllBooks);
                        SqlParameter readerBookParam = new SqlParameter("@AvailableBooks", book.AvailableBooks);
                        command.Parameters.Add(titleBookParam);
                        command.Parameters.Add(countBookParam);
                        command.Parameters.Add(readerBookParam);
                        command.ExecuteNonQuery();

                        command.CommandText = "SELECT TOP 1 Books.BookId FROM Books ORDER BY Books.BookId DESC";
                        int bookId = (int)command.ExecuteScalar();

                        for (int i = 0; i < book.Authors.Count; i++)
                        {
                            command.CommandText = string.Format("INSERT INTO  Authors VALUES(@AuthorName{0})", i);
                            SqlParameter authorParam = new SqlParameter(string.Format("@AuthorName{0}", i), book.Authors[i].AuthorName);
                            command.Parameters.Add(authorParam);
                            command.ExecuteNonQuery();

                            command.CommandText = "SELECT TOP 1 Authors.AuthorId FROM Authors ORDER BY Authors.AuthorId DESC";
                            int authorId = (int)command.ExecuteScalar();

                            command.CommandText = string.Format("INSERT INTO Authors_Books VALUES(@BookId{0}, @AuthorId{0})", i);
                            SqlParameter bookIdParam = new SqlParameter(string.Format("@BookId{0}", i), bookId);
                            SqlParameter authorIdParam = new SqlParameter(string.Format("@AuthorId{0}", i), authorId);
                            command.Parameters.Add(bookIdParam);
                            command.Parameters.Add(authorIdParam);
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        public void DeleteBook(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    SqlTransaction transaction = connection.BeginTransaction();
                    command.Transaction = transaction;

                    try
                    {
                        command.CommandText = "DELETE FROM Books WHERE Books.BookId = @BookId1";
                        SqlParameter idBookParam1 = new SqlParameter("@BookId1", id);
                        command.Parameters.Add(idBookParam1);
                        command.ExecuteNonQuery();

                        command.CommandText = " DELETE FROM Authors WHERE AuthorId in (SELECT AuthorId FROM Authors_Books WHERE BookId = @BookId2);";
                        SqlParameter idBookParam2 = new SqlParameter("@BookId2", id);
                        command.Parameters.Add(idBookParam2);
                        command.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }

                }
            }
        }

        public Book GetBook(int id)
        {
            Book book = new Book();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Books WHERE Books.BookId = @BookId";
                    SqlParameter idBookParam = new SqlParameter("@BookId", id);
                    command.Parameters.Add(idBookParam);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            book.BookId = (int)reader.GetValue(0);
                            book.Title = (string)reader.GetValue(1);
                            book.AllBooks = (int)reader.GetValue(2);
                            book.AvailableBooks = (int)reader.GetValue(3);
                        }
                    }
                }
            }
            return book;
        }

        public void ChangeBookQuantity(Book book)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Books SET AllBooks = @AllBooks, AvailableBooks = @AvailableBooks WHERE BookId = @BookId";
                    SqlParameter allBookParam = new SqlParameter("@AllBooks", book.AllBooks);
                    SqlParameter availableBookParam = new SqlParameter("@AvailableBooks", book.AvailableBooks);
                    SqlParameter idBookParam = new SqlParameter("@BookId", book.BookId);
                    command.Parameters.Add(allBookParam);
                    command.Parameters.Add(availableBookParam);
                    command.Parameters.Add(idBookParam);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateAvailableBooksCount(Book book)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Books SET AvailableBooks = @AvailableBooks WHERE BookId = @BookId";
                    SqlParameter countBookParam = new SqlParameter("@AvailableBooks", book.AvailableBooks - 1);
                    SqlParameter idBookParam = new SqlParameter("@BookId", book.BookId);
                    command.Parameters.Add(countBookParam);
                    command.Parameters.Add(idBookParam);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool CanTakeBook(int bookId, string userName)
        {
            int userId = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Id FROM Users WHERE UserName = @UserName";
                    SqlParameter userParam = new SqlParameter("@UserName", userName);
                    command.Parameters.Add(userParam);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                userId = (int)reader.GetValue(0);
                            }
                        }
                    }
                    command.CommandText = "SELECT BookId FROM Book_User WHERE UserId = @UserId";
                    SqlParameter userIdParam = new SqlParameter("@UserId", userId);
                    command.Parameters.Add(userIdParam);
                    var bookIdFromRepository = 0;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                bookIdFromRepository = (int)reader.GetValue(0);
                                if (bookIdFromRepository == bookId) break;
                            }
                        }
                    }
                    if (bookIdFromRepository != bookId)
                    {
                        command.CommandText = "INSERT INTO Book_User VALUES(@BookId, @UserId1, @DateOfEvent)";
                        SqlParameter bookIdParam = new SqlParameter("@BookId", bookId);
                        command.Parameters.Add(bookIdParam);
                        SqlParameter userIdParam1 = new SqlParameter("@UserId1", userId);
                        command.Parameters.Add(userIdParam1);
                        SqlParameter dateParam = new SqlParameter("@DateOfEvent", DateTime.Now);
                        command.Parameters.Add(dateParam);
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            return false;
        }

        public List<History> ShowHistory(int bookId)
        {
            List<History> history = new List<History>();
            string bookTitle = String.Empty;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT TOP 1 BookTitle  FROM Books WHERE BookId = @BookId1";
                    SqlParameter bookIdParam1 = new SqlParameter("@BookId1", bookId);
                    command.Parameters.Add(bookIdParam1);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                bookTitle = (string)reader.GetValue(0);
                            }
                        }
                    }

                    command.CommandText = "SELECT Users.UserName, Book_User.DataOfEvent FROM Users INNER JOIN Book_User " +
                                            "ON Users.Id = Book_User.UserId WHERE Book_User.BookId = @BookId2";
                    SqlParameter bookIdParam2 = new SqlParameter("@BookId2", bookId);
                    command.Parameters.Add(bookIdParam2);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                history.Add(new History()
                                {
                                    UserName = (string)reader.GetValue(0),
                                    Date = (DateTime)reader.GetValue(1),
                                    BookTitle = bookTitle
                                });
                            }
                        }
                    }
                }
            }
            return history;
        }

        public string FindEmailByUser(string userName)
        {
            string userEmail = string.Empty;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT UserEmail FROM Users WHERE UserName = @UserName";
                    SqlParameter userParam = new SqlParameter("@UserName", userName);
                    command.Parameters.Add(userParam);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                userEmail = (string)reader.GetValue(0);
                            }
                        }
                    }
                }
            }
            return userEmail;
        }
    }
}
