using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VG.Common.Helper;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.RequestModel;
using VG.Model.ResponseModel;

namespace VG.Service.Service
{
    public interface IAccountDetailService
    {
        AccountRequestModel RegistrationAccount(AccountRequestModel newItem);
        AccountRequestModel UpdateAccountDetail(AccountRequestModel newItem);
        AccountRequestModel GetAccountDetailById(string Id);
        IEnumerable<AccountRequestModel> GetAllAccount();
        IEnumerable<SelectedResponseModel> SearchAccount(string searchValue);
    }
    public class AccountDetailService : IAccountDetailService
    {
        private IAccountRepository _accountRepository;
        private IAccountDetailRepository _accountDetailRepository;
        public AccountDetailService(IAccountRepository accountRepository, IAccountDetailRepository accountDetailRepository)
        {
            _accountRepository = accountRepository;
            _accountDetailRepository = accountDetailRepository;

        }

        public AccountRequestModel GetAccountDetailById(string Id)
        {
            var result = this._accountRepository.GetSingle(s => s.Id == Id, new string[] { "Members" });
            return new AccountRequestModel
            {
                Id = result.Id,
                PhoneNumber = result.PhoneNumber,
                FullName = result.Members.FirstOrDefault().FullName,
                BirthDate = result.Members.FirstOrDefault().BirthDate,
                Email = result.Members.FirstOrDefault().Email,
                Sex = result.Members.FirstOrDefault().Sex,
            };
        }

        public IEnumerable<AccountRequestModel> GetAllAccount()
        {
            return this._accountRepository.GetAll().Select(s => new AccountRequestModel
            {
                Id = s.Id,
                PhoneNumber = s.PhoneNumber,
                Password = s.PassWord,
                FullName = s.Members.FirstOrDefault().FullName,
                BirthDate = s.Members.FirstOrDefault().BirthDate,
                Email = s.Members.FirstOrDefault().Email,
                Sex = s.Members.FirstOrDefault().Sex,
            });
        }

        public AccountRequestModel RegistrationAccount(AccountRequestModel newItem)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == newItem.PhoneNumber);
            if (account != null)
                throw new Exception("Số điện thoại này đã được đăng kí. Vui lòng thử lại số khác");
            account = this._accountRepository.Add(new AppAccount
            {
                PhoneNumber = newItem.PhoneNumber,
                PassWord = IdentityHelper.HashPassword(newItem.Password),
                Status = true,
                RoleId = 2
            });
            var accountDetail = this._accountDetailRepository.Add(new Member
            {
                AccountId = account.Id,
                FullName = newItem.FullName,
                BirthDate = newItem.BirthDate,
                Sex = newItem.Sex,
                Email = newItem.Email,
                CreatedDate = DateTime.Now
            });
            this._accountDetailRepository.Commit();
            newItem.Id = account.Id;
            return newItem;
        }

        public IEnumerable<SelectedResponseModel> SearchAccount(string searchValue)
        {
            var result = this._accountDetailRepository.GetMulti(s => s.FullName.Contains(searchValue) && s.FullName.EndsWith(searchValue), new string[] { "AppAccount" })
                .Select(s => new SelectedResponseModel
                {
                    Id = s.AppAccount.Id,
                    Text = s.FullName
                });
            return result;
        }

        public AccountRequestModel UpdateAccountDetail(AccountRequestModel newItem)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == newItem.PhoneNumber);
            if (account != null)
            {
                var accountDetail = this._accountDetailRepository.GetSingle(s => s.AccountId == account.Id);

                accountDetail.FullName = newItem.FullName;
                accountDetail.BirthDate = newItem.BirthDate;
                accountDetail.Sex = newItem.Sex;
                this._accountDetailRepository.Update(accountDetail);
            }
            else
                throw new Exception("Có lỗi xảy ra.  Vui lòng thử lại");
            this._accountDetailRepository.Commit();
            return newItem;
        }
    }
}
