using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;

namespace Unison.Cloud.Core.Interfaces.Services
{
    public interface IAccountService
    {
        IEnumerable<Account> GetAll();
        Account Find(int id);
        Account Authenticate(string username, string password);
        Account Create(Account account, string password);
        void Update(Account account, string password = null);
        void Remove(int id);
    }
}
