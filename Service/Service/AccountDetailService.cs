﻿using System;
using System.Collections.Generic;
using System.Text;
using VG.Common.Helper;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.RequestModel;

namespace VG.Service.Service
{
    public interface IAccountDetailService
    {
        AccountRequestModel RegistrationAccount(AccountRequestModel newItem);
        AccountRequestModel UpdateAccountDetail(AccountRequestModel newItem);
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
        public AccountRequestModel RegistrationAccount(AccountRequestModel newItem)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == newItem.PhoneNumber);
            if (account != null)
                throw new Exception("Số điện thoại này đã được đăng kí. Vui lòng thử lại số khác");
            account = this._accountRepository.Add(new Account
            {
                PhoneNumber = newItem.PhoneNumber,
                PassWord = IdentytiHelper.HashPassword(newItem.Password),
                Lock = false,
                RoleId = 2
            });
            var accountDetail = this._accountDetailRepository.Add(new AccountDetail
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