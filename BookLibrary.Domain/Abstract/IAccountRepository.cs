using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Domain.Abstract
{
    public interface IAccountRepository
    {
        void AddUser(string userName, string userEmail);
        bool IsUserExistInDB(string userName, string userEmail);
    }
}
