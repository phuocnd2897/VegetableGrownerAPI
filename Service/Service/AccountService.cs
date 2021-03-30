using VG.Common.Helper;
using VG.Data.Repository;
using VG.Model.Model;
using System;
using System.Collections.Generic;
using System.Text;
using VG.Model.ResponseModel;
using System.Linq;

namespace VG.Service.Service
{
    public interface IAccountService
    {
        LoginResponseModel Login(string phoneNumber, string password);
        void LockAccount(string Id);
    }
    public class AccountService : IAccountService
    {
        private IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public void LockAccount(string Id)
        {
            var account = this._accountRepository.GetSingle(s => s.Id == Id);
            account.Status = false;
            this._accountRepository.Update(account);
            this._accountRepository.Commit();
        }

        public LoginResponseModel Login(string phoneNumber, string password)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber, new string[] { "Members"});
            if (account == null || !IdentytiHelper.VerifyHashedPassword(account.PassWord, password) || account.Status == false)
                return null;
            return new LoginResponseModel
            {
                AccountId = account.Id,
                PhoneNumber = account.PhoneNumber,
                FullName = account.Members.FirstOrDefault().FullName,
                ProviderKey = Guid.NewGuid().ToString(),
                LoginTime = DateTime.UtcNow,
                ExpiresTime = DateTime.UtcNow.AddDays(7),
                RoleId = account.RoleId
            };
        }
    }
}
