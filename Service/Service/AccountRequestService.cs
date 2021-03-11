using System;
using System.Collections.Generic;
using System.Text;
using VG.Data.Repository;
using VG.Model.Model;

namespace VG.Service.Service
{
    public interface IAccountRequestService
    {
        AccountRequest Add(AccountRequest newItem);
        void IsComfirm(int Id, int status);
    }
    public class AccountRequestService : IAccountRequestService
    {
        private IAccountRequestRepository _accountRequestRepository;
        private IAccountFriendRepository _accountFriendRepository;
        public AccountRequestService(IAccountRequestRepository accountRequestRepository, IAccountFriendRepository accountFriendRepository)
        {
            _accountRequestRepository = accountRequestRepository;
            _accountFriendRepository = accountFriendRepository;
        }
        public AccountRequest Add(AccountRequest newItem)
        {
            var result = this._accountRequestRepository.Add(newItem);
            this._accountRequestRepository.Commit();
            return result;
        }

        public void IsComfirm(int Id, int status)
        {
            var result = this._accountRequestRepository.GetSingle(s => s.Id == Id);
            result.Status = status;
            this._accountRequestRepository.Update(result);
            this._accountFriendRepository.Add(new AccountFriend
            {
                Account_one_Id = result.AccountSend,
                Account_two_Id = result.AccountReceived,
                AcceptedDate = DateTime.Now
            });
            this._accountRequestRepository.Commit();
        }
    }
}
