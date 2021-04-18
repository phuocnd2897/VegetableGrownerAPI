using VG.Common.Helper;
using VG.Data.Repository;
using VG.Model.Model;
using System;
using System.Collections.Generic;
using System.Text;
using VG.Model.ResponseModel;
using System.Linq;
using VG.Model.RequestModel;

namespace VG.Service.Service
{
    public interface IAccountService
    {
        LoginResponseModel Login(string phoneNumber, string password, string deviceToken);
        void LockAccount(string Id);
        string GetAccountPassword(string phoneNumber);
        void ChangePassword(ChangePasswordRequestModel newItem);
    }
    public class AccountService : IAccountService
    {
        private IAccountRepository _accountRepository;
        private IAppAccountLoginRepository _appAccountLoginRepository;
        public AccountService(IAccountRepository accountRepository, IAppAccountLoginRepository appAccountLoginRepository)
        {
            _accountRepository = accountRepository;
            _appAccountLoginRepository = appAccountLoginRepository;
        }

        public void ChangePassword(ChangePasswordRequestModel newItem)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == newItem.PhoneNumber);
            if (account == null)
            {
                throw new Exception("Có lỗi xảy ra. Vui lòng thử lại");
            }
            account.PassWord = IdentityHelper.HashPassword(newItem.NewPassword);
            this._accountRepository.Update(account);
        }

        public string GetAccountPassword(string phoneNumber)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            if (account == null)
            {
                throw new Exception("Số điện thoại không tồn tại");
            }
            return account.PassWord;
        }

        public void LockAccount(string Id)
        {
            var account = this._accountRepository.GetSingle(s => s.Id == Id);
            account.Status = false;
            this._accountRepository.Update(account);
            this._accountRepository.Commit();
        }

        public LoginResponseModel Login(string phoneNumber, string password, string deviceToken)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber, new string[] { "Members" });
            if (account == null || !IdentityHelper.VerifyHashedPassword(account.PassWord, password) || account.Status == false)
                return null;
            var result = new LoginResponseModel()
            {
                AccountId = account.Id,
                PhoneNumber = account.PhoneNumber,
                FullName = account.Members.FirstOrDefault().FullName,
                ProviderKey = Guid.NewGuid().ToString(),
                LoginTime = DateTime.UtcNow,
                ExpiresTime = DateTime.UtcNow.AddDays(7),
                DeviceToken = deviceToken,
                RoleId = account.RoleId
            };
            var userlogin = this._appAccountLoginRepository.GetSingle(s => s.AppAccountId == account.Id && s.DeviceToken == deviceToken);
            if (userlogin == null)
            {
                userlogin = new AppAccountLogin();
                userlogin.AppAccountId = result.AccountId;
                userlogin.ProviderKey = result.ProviderKey;
                userlogin.LoginTime = result.LoginTime;
                userlogin.ExpiresTime = result.ExpiresTime;
                userlogin.DeviceToken = result.DeviceToken;
                this._appAccountLoginRepository.Add(userlogin);
            }
            else
            {
                userlogin.ProviderKey = result.ProviderKey;
                userlogin.LoginTime = result.LoginTime;
                userlogin.ExpiresTime = result.ExpiresTime;
                this._appAccountLoginRepository.Update(userlogin);
            }
            this._appAccountLoginRepository.Commit();
            return result;
        }
    }
}
