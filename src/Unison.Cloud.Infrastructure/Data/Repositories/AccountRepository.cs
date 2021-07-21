using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Core.Interfaces.Data;

namespace Unison.Cloud.Infrastructure.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(AppDbContext context, ILogger<AccountRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(Account account)
        {
            _context.Accounts.Add(account);
        }

        public IEnumerable<Account> GetAll()
        {
            return _context.Accounts.ToList();
        }

        public Account Find(int id)
        {
            return _context.Accounts.Find(id);
        }

        public Account FindByUsername(string username)
        {
            return _context.Accounts.SingleOrDefault(a => a.Username == username);
        }

        public void Remove(Account account)
        {
            _context.Accounts.Remove(account);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
