using VG.Common.Helper;
using VG.Data.Repository;
using VG.Model.Model;
using System;
using System.Collections.Generic;
using System.Text;
using VG.Model.ResponseModel;

namespace VG.Service.Service
{
    public interface IAccountService
    {
        public LoginResponseModel Login(string phoneNumber, string password);
    }
    public class AccountService : IAccountService
    {
        private IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public LoginResponseModel Login(string phoneNumber, string password)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            if (account == null || !IdentytiHelper.VerifyHashedPassword(account.PassWord, password) || account.Lock == true)
                return null;
            return new LoginResponseModel
            {
                AccountId = account.Id,
                PhoneNumber = account.PhoneNumber,
                ProviderKey = Guid.NewGuid().ToString(),
                LoginTime = DateTime.UtcNow,
                ExpiresTime = DateTime.UtcNow.AddDays(7),
                RoleId = account.RoleId
            };
        }
    }
}
