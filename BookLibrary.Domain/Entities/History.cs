using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Domain.Entities
{
    public class History
    {
        public string BookTitle { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
    }
}
