using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookLibrary.Domain.Entities
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int AvailableBooks { get; set; }
        public int AllBooks { get; set; }
        public List<Author> Authors { get; set; }
    }
}