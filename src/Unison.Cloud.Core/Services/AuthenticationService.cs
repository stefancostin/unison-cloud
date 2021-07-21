using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Core.Exceptions;
using Unison.Cloud.Core.Interfaces.Data;
using Unison.Cloud.Core.Interfaces.Services;

namespace Unison.Cloud.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(IAccountRepository accountRepository, ILogger<AuthenticationService> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public IEnumerable<Account> GetAll()
        {
            return _accountRepository.GetAll();
        }

        public Account FindById(int id)
        {
            return _accountRepository.Find(id);
        }

        public Account Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _accountRepository.FindByUsername(username);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public Account Create(Account account, string password)
        {
            ValidateUsername(account.Username);

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            account.PasswordHash = passwordHash;
            account.PasswordSalt = passwordSalt;

            _accountRepository.Add(account);
            _accountRepository.Save();

            return account;
        }

        public void Update(Account accountUpdate, string password = null)
        {
            Account account = _accountRepository.Find(accountUpdate.Id);

            if (account == null)
                throw new InvalidRequestException("Account does not exist.");

            if (!string.IsNullOrWhiteSpace(accountUpdate.Username) && accountUpdate.Username != account.Username)
            {
                ValidateUsername(accountUpdate.Username);
                account.Username = accountUpdate.Username;
            }

            if (!string.IsNullOrWhiteSpace(accountUpdate.FirstName) && accountUpdate.FirstName != account.FirstName)
                account.FirstName = accountUpdate.FirstName;

            if (!string.IsNullOrWhiteSpace(accountUpdate.LastName) && accountUpdate.LastName != account.LastName)
                account.LastName = accountUpdate.LastName;

            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                account.PasswordHash = passwordHash;
                account.PasswordSalt = passwordSalt;
            }

            _accountRepository.Save();
        }


        public void Remove(int id)
        {
            var user = _accountRepository.Find(id);

            if (user == null)
                return;

            _accountRepository.Remove(user);
            _accountRepository.Save();
        }

        private void ValidateUsername(string username)
        {
            Account accountWithSameUsername = _accountRepository.FindByUsername(username);

            if (accountWithSameUsername != null)
                throw new InvalidRequestException("An account with the same username already exists.");
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("A password must be provided.");

            using var hmac = new System.Security.Cryptography.HMACSHA512();

            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
                throw new ArgumentException("A password must be provided.");
            if (storedHash.Length != 64)
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).");
            if (storedSalt.Length != 128)
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).");

            using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i])
                    return false;
            }

            return true;
        }
    }
}
