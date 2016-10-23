using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Domain.SendEmail
{
    public interface ISendEmailManager
    {
        void Send(string EmailTo, string title);
    }
}
