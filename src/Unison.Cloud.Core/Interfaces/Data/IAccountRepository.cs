using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;

namespace Unison.Cloud.Core.Interfaces.Data
{
    public interface IAccountRepository : IBaseRepository
    {
        void Add(Account account);
        Account Find(int id);
        Account FindByUsername(string username);
        IEnumerable<Account> GetAll();
        void Remove(Account account);
    }
}
