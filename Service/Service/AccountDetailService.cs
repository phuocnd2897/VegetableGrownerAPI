using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VG.Common.Constant;
using VG.Common.Helper;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.RequestModel;
using VG.Model.ResponseModel;

namespace VG.Service.Service
{
    public interface IAccountDetailService
    {
        AccountRequestModel RegistrationAccount(AccountRequestModel newItem, string savePath, string url);
        AccountRequestModel UpdateAccountDetail(AccountRequestModel newItem);
        AccountRequestModel GetAccountDetailById(string Id, string phoneNumber);
        IEnumerable<AccountRequestModel> GetAllAccount();
        IEnumerable<SelectedResponseModel> SearchAccount(string searchValue);
        void ChangePassword(string newPass, string phoneNumber);
        void UploadAvatar(IFormFile newItem, string savePath, string url, string phoneNumber);
    }
    public class AccountDetailService : IAccountDetailService
    {
        private IAccountRepository _accountRepository;
        private IAccountDetailRepository _accountDetailRepository;
        private IAccountRequestRepository _accountRequestRepository;
        private IAccountFriendRepository _accountFriendRepository;
        public AccountDetailService(IAccountRepository accountRepository, IAccountDetailRepository accountDetailRepository, IAccountRequestRepository accountRequestRepository, IAccountFriendRepository accountFriendRepository)
        {
            _accountRepository = accountRepository;
            _accountDetailRepository = accountDetailRepository;
            _accountRequestRepository = accountRequestRepository;
            _accountFriendRepository = accountFriendRepository;
        }

        public void ChangePassword(string newPass, string phoneNumber)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            account.PassWord = IdentityHelper.HashPassword(newPass);
            this._accountRepository.Update(account);
            this._accountRepository.Commit();
        }

        public AccountRequestModel GetAccountDetailById(string Id, string phoneNumber)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber, new string[] { "Members" });
            if (Id == account.Id)
            {
                return new AccountRequestModel
                {
                    Id = account.Id,
                    PhoneNumber = account.PhoneNumber,
                    Password = account.PassWord,
                    FullName = account.Members.FirstOrDefault().FullName,
                    BirthDate = account.Members.FirstOrDefault().BirthDate,
                    Email = account.Members.FirstOrDefault().Email,
                    Sex = account.Members.FirstOrDefault().Sex,
                    Address = account.Members.FirstOrDefault().Address,
                    AvatarResponse = account.Members.FirstOrDefault().Avatar,
                };
            }
            else
            {
                var result = this._accountRepository.GetSingle(s => s.Id == Id, new string[] { "Members" });
                var friend = this._accountFriendRepository.GetMulti(s => (s.Account_one_Id == result.Id && s.Account_two_Id == account.Id) || (s.Account_one_Id == account.Id && s.Account_two_Id == result.Id)).ToList();
                var request = this._accountRequestRepository.GetMulti(s => (s.AccountSend == account.Id && s.AccountReceived == result.Id && s.Status == (int)EnumStatusRequest.Pending)).ToList();
                var receved = this._accountRequestRepository.GetMulti(s => (s.AccountReceived == account.Id && s.AccountSend == result.Id && s.Status == (int)EnumStatusRequest.Pending)).ToList();
                return new AccountRequestModel
                {
                    Id = result.Id,
                    PhoneNumber = result.PhoneNumber,
                    FullName = result.Members.FirstOrDefault().FullName,
                    BirthDate = result.Members.FirstOrDefault().BirthDate,
                    Email = result.Members.FirstOrDefault().Email,
                    Sex = result.Members.FirstOrDefault().Sex,
                    Address = result.Members.FirstOrDefault().Address,
                    AvatarResponse = result.Members.FirstOrDefault().Avatar,
                    IsFriend = friend.Count() > 0 ? 4 : request.Count > 0 ? 2 : receved.Count > 0 ? 3 : 1
                };
            }
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
                Address = s.Members.FirstOrDefault().Address,
                AvatarResponse = s.Members.FirstOrDefault().Avatar
            });
        }

        public AccountRequestModel RegistrationAccount(AccountRequestModel newItem, string savePath, string url)
        {
            string imageName = "";
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == newItem.PhoneNumber);
            if (account != null)
                throw new Exception("Số điện thoại này đã được đăng kí. Vui lòng thử lại số khác");
            if (newItem.AvatarRequest != null)
            {
                imageName = new string(Path.GetFileNameWithoutExtension(newItem.AvatarRequest.FileName).Take(10).ToArray()).Replace(' ', '-');
                imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(newItem.AvatarRequest.FileName);
                var imagePath = Path.Combine(savePath, "wwwroot/Image", imageName);
                using (FileStream fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    newItem.AvatarRequest.CopyTo(fileStream);
                    fileStream.Flush();
                }
            }
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
                Address = newItem.Address,
                Email = newItem.Email,
                Avatar = imageName != "" ? url + "/Avatar//" + imageName : "",
                CreatedDate = DateTime.Now
            });
            this._accountDetailRepository.Commit();
            newItem.Id = account.Id;
            return newItem;
        }

        public IEnumerable<SelectedResponseModel> SearchAccount(string searchValue)
        {
            var result = this._accountDetailRepository.GetMulti(s => s.FullName.Contains(searchValue) && s.FullName.EndsWith(searchValue) && s.AppAccount.Status == true, new string[] { "AppAccount" })
                .Select(s => new AccountSelectedResponseModel
                {
                    Id = s.AppAccount.Id,
                    Text = s.FullName,
                    Avatar = s.Avatar
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
                accountDetail.Address = newItem.Address;
                this._accountDetailRepository.Update(accountDetail);
            }
            else
                throw new Exception("Có lỗi xảy ra.  Vui lòng thử lại");
            this._accountDetailRepository.Commit();
            return newItem;
        }

        public void UploadAvatar(IFormFile newItem, string phoneNumber, string savePath, string url)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber, new string[] { "Members" });
            if (newItem != null)
            {
                string imageName = new string(Path.GetFileNameWithoutExtension(newItem.FileName).Take(10).ToArray()).Replace(' ', '-');
                imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(newItem.FileName);
                var imagePath = Path.Combine(savePath, "wwwroot/Image", imageName);
                using (FileStream fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    newItem.CopyTo(fileStream);
                    fileStream.Flush();
                }
                account.Members.FirstOrDefault().Avatar = url + "/Avatar//" + imageName;
            }
            this._accountDetailRepository.Update(account.Members.FirstOrDefault());
            this._accountDetailRepository.Commit();
        }
    }
}
